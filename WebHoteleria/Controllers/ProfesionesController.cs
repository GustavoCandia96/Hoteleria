using EntidadesHoteleria;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebHoteleria.Class;
using WebHoteleria.Models;

namespace WebHoteleria.Controllers
{
    public class ProfesionesController : Controller
    {

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #region Listado de Profesiones

        [HttpGet]
        [AutorizarUsuario("Profesiones", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ProfesionModel> listaProfesiones = new List<ProfesionModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomProfesion = Convert.ToString(Session["sesionProfesionesNombre"]);
                ViewBag.txtProfesion = sesNomProfesion;

                //OBTENEMOS TODOS LAS PROFESIONES ACTIVAS DE LA BASE DE DATOS
                var profesiones = from p in db.profesiones
                                  where p.estado != null
                                  select new ProfesionModel
                                  {
                                      Id = p.id,
                                      NombreProfesion = p.nombre_profesion,
                                      Estado = p.estado,
                                      EstadoDescrip = p.estado == true ? "Activo" : "Inactivo"
                                  };
                listaProfesiones = profesiones.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomProfesion != "")
                {
                    listaProfesiones = listaProfesiones.Where(p => p.NombreProfesion.ToUpper().Contains(sesNomProfesion.Trim().ToUpper())).ToList();
                }

                listaProfesiones = listaProfesiones.OrderBy(p => p.NombreProfesion).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de profesiones";
            }
            return View(listaProfesiones.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ProfesionModel> listaProfesiones = new List<ProfesionModel>();
            try
            {
                //OBTENEMOS TODOS LAS PROFESIONES ACTIVAS DE LA BASE DE DATOS
                var profesiones = from p in db.profesiones
                                  where p.estado != null
                                  select new ProfesionModel
                                  {
                                      Id = p.id,
                                      NombreProfesion = p.nombre_profesion,
                                      Estado = p.estado,
                                      EstadoDescrip = p.estado == true ? "Activo" : "Inactivo"
                                  };
                listaProfesiones = profesiones.ToList();

                //FILTRAMOS POR NOMBRE PROFESION LA BUSQUEDA
                var fcNombreProfesion = fc["txtProfesion"];
                if (fcNombreProfesion != "")
                {
                    string descripcion = Convert.ToString(fcNombreProfesion);
                    listaProfesiones = listaProfesiones.Where(p => p.NombreProfesion.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }
                listaProfesiones = listaProfesiones.OrderBy(p => p.NombreProfesion).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtProfesion = fcNombreProfesion;
                Session["sesionProfesionesNombre"] = fcNombreProfesion;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar profesiones";
            }
            return View(listaProfesiones.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Profesion

        [HttpGet]
        [AutorizarUsuario("Profesiones", "Create")]
        public ActionResult Create()
        {
            ProfesionModel profesion = new ProfesionModel();
            return View(profesion);
        }

        [HttpPost]
        public ActionResult Create(ProfesionModel profesionModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE LA PROFESION EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.profesiones.Where(p => p.nombre_profesion.ToUpper() == profesionModelo.NombreProfesion.ToUpper() && p.estado != null).Count();
                    if (cantidad == 0)
                    {
                        profesiones profesion = new profesiones
                        {
                            nombre_profesion = profesionModelo.NombreProfesion,
                            estado = true
                        };
                        db.profesiones.Add(profesion);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una profesión registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la profesión en la base de datos");
                    retornoVista = true;
                }
                finally
                {
                    db.Dispose();
                }
            }
            else
            {
                ViewBag.msg = null;
                retornoVista = true;
            }
            if (retornoVista == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(profesionModelo);
            }
        }

        #endregion

        #region Editar Profesion

        [HttpGet]
        [AutorizarUsuario("Profesiones", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProfesionModel profesionEdit = new ProfesionModel();
            try
            {
                var profesion = db.profesiones.Where(p => p.id == id).FirstOrDefault();
                profesionEdit.Id = profesion.id;
                profesionEdit.NombreProfesion = profesion.nombre_profesion;
                profesionEdit.Estado = profesion.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = profesion.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(profesionEdit);
        }

        [HttpPost]
        public ActionResult Edit(ProfesionModel profesionModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE LA PROFESION EN LA BASE DE DATOS PARA PODER ACTUALIZAR
                    int cantidad = db.profesiones.Where(p => p.nombre_profesion.ToUpper() == profesionModelo.NombreProfesion.ToUpper() && p.estado != null && p.id != profesionModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var profesion = db.profesiones.Where(p => p.id == profesionModelo.Id).FirstOrDefault();
                        profesion.nombre_profesion = profesionModelo.NombreProfesion;
                        bool nuevoEstado = profesionModelo.EstadoDescrip == "A" ? true : false;
                        profesion.estado = nuevoEstado;
                        db.Entry(profesion).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una profesión registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar la profesión en la base de datos");
                    retornoVista = true;
                }
                finally
                {
                    db.Dispose();
                }
            }
            else
            {
                ViewBag.msg = null;
                retornoVista = true;
            }
            if (retornoVista == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", profesionModelo.EstadoDescrip);
                return View(profesionModelo);
            }
        }

        #endregion

        #region Eliminar Profesion

        [HttpPost]
        public ActionResult EliminarProfesion(string profesionId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Profesiones", "EliminarProfesion");
                string resultado = autorizarAccion.VerificarPermiso();
                if (resultado == string.Empty)
                {
                    //EJECUTAMOS LA ACTUALIZACION DE LA ELIMINACIÓN DEL REGISTRO
                    using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                int idProfesion = Convert.ToInt32(profesionId);
                                var profesion = context.profesiones.Where(p => p.id == idProfesion).FirstOrDefault();
                                profesion.estado = null;
                                context.Entry(profesion).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();

                                dbContextTransaction.Commit();
                            }
                            catch (Exception)
                            {
                                dbContextTransaction.Rollback();
                                retorno = false;
                            }
                        }
                        context.Database.Connection.Close();
                    }
                }
                else
                {
                    respuesta = autorizarAccion.ObtenerMensajeVerificacion(resultado);
                    retorno = false;
                }
            }
            catch (Exception)
            {
                retorno = false;
            }
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Profesiones") }, JsonRequestBehavior.AllowGet);
        }

        #endregion



    }
}