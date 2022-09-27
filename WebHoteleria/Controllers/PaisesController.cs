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
    public class PaisesController : Controller
    {

        #region Propiedades
        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();
        #endregion

        #region Listado de Paises

        [HttpGet]
        [AutorizarUsuario("Paises", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<PaisModel> listaPaises = new List<PaisModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomPais = Convert.ToString(Session["sesionPaisesNombre"]);
                ViewBag.txtPais = sesNomPais;

                //OBTENEMOS TODOS LOS PAISES ACTIVOS DE LA BASE DE DATOS Y ORDENAMOS POR DESCRIPCIÓN
                var paises = from p in db.paises
                             where p.estado != null
                             select new PaisModel
                             {
                                 Id = p.id,
                                 NombrePais = p.nombre_pais,
                                 Estado = p.estado,
                                 EstadoDescrip = p.estado == true ? "Activo" : "Inactivo"
                             };
                listaPaises = paises.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomPais != "")
                {
                    listaPaises = listaPaises.Where(p => p.NombrePais.Trim().ToUpper().Contains(sesNomPais.Trim().ToUpper())).ToList();
                }

                listaPaises = listaPaises.OrderBy(p => p.NombrePais).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de paises";
            }
            return View(listaPaises.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<PaisModel> listaPaises = new List<PaisModel>();
            try
            {
                //OBTENEMOS TODOS LOS PAISES ACTIVOS DE LA BASE DE DATOS
                var paises = from p in db.paises
                             where p.estado != null
                             select new PaisModel
                             {
                                 Id = p.id,
                                 NombrePais = p.nombre_pais,
                                 Estado = p.estado,
                                 EstadoDescrip = p.estado == true ? "Activo" : "Inactivo"
                             };
                listaPaises = paises.ToList();

                //FILTRAMOS POR NOMBRE PAIS LA BUSQUEDA
                var fcNombrePais = fc["txtPais"];
                if (fcNombrePais != "")
                {
                    string descripcion = Convert.ToString(fcNombrePais);
                    listaPaises = listaPaises.Where(p => p.NombrePais.Trim().ToUpper().Contains(descripcion.ToUpper())).ToList();
                }
                listaPaises.OrderBy(p => p.NombrePais).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtPais = fcNombrePais;
                Session["sesionPaisesNombre"] = fcNombrePais;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar paises";
            }
            return View(listaPaises.ToPagedList(pageIndex, pageSize));
        }


        #endregion

        #region Crear Pais

        [HttpGet]
        [AutorizarUsuario("Paises", "Create")]
        public ActionResult Create()
        {
            PaisModel pais = new PaisModel();
            return View(pais);
        }

        [HttpPost]
        public ActionResult Create(PaisModel paisModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL PAÍS EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.paises.Where(p => p.nombre_pais.Trim().ToUpper() == paisModelo.NombrePais.Trim().ToUpper() && p.estado != null).Count();
                    if (cantidad == 0)
                    {
                        paises pais = new paises
                        {
                            nombre_pais = paisModelo.NombrePais,
                            estado = true
                        };
                        db.paises.Add(pais);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un país registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el país en la base de datos");
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
                return View(paisModelo);
            }
        }

        #endregion

        #region Editar Pais

        [HttpGet]
        [AutorizarUsuario("Paises", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PaisModel paisEdit = new PaisModel();
            try
            {
                var pais = db.paises.Where(p => p.id == id).FirstOrDefault();
                paisEdit.Id = pais.id;
                paisEdit.NombrePais = pais.nombre_pais;
                paisEdit.Estado = pais.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = pais.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(paisEdit);
        }

        [HttpPost]
        public ActionResult Edit(PaisModel paisModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL PAÍS EN LA BASE DE DATOS PARA PODER ACTUALIZAR
                    int cantidad = db.paises.Where(p => p.nombre_pais.ToUpper() == paisModelo.NombrePais.ToUpper() && p.estado != null && p.id != paisModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var pais = db.paises.Where(p => p.id == paisModelo.Id).FirstOrDefault();
                        pais.nombre_pais = paisModelo.NombrePais;
                        bool nuevoEstado = paisModelo.EstadoDescrip == "A" ? true : false;
                        pais.estado = nuevoEstado;
                        db.Entry(pais).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un país registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el país en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", paisModelo.EstadoDescrip);
                return View(paisModelo);
            }
        }

        #endregion

        #region Eliminar Pais
        [HttpPost]
        public ActionResult EliminarPais(string paisId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Paises", "EliminarPais");
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
                                int idPais = Convert.ToInt32(paisId);
                                var pais = context.paises.Where(p => p.id == idPais).FirstOrDefault();
                                pais.estado = null;
                                context.Entry(pais).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Paises") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}