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
    public class PerfilController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Perfiles

        [HttpGet]
        [AutorizarUsuario("Perfil", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<PerfilModel> listaPerfiles = new List<PerfilModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomPerfil = Convert.ToString(Session["sesionPerfilesNombre"]);
                ViewBag.txtPerfil = sesNomPerfil;

                //OBTENEMOS TODOS LOS PERFILES NO ELIMINADOS DE LA BASE DE DATOS
                var perfiles = from p in db.perfiles
                               where p.estado != null
                               select new PerfilModel
                               {
                                   Id = p.id,
                                   NombrePerfil = p.perfil,
                                   Estado = p.estado,
                                   EstadoDescrip = p.estado == true ? "Activo" : "Inactivo"
                               };
                listaPerfiles = perfiles.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomPerfil != "")
                {
                    listaPerfiles = listaPerfiles.Where(p => p.NombrePerfil.ToUpper().Contains(sesNomPerfil.Trim().ToUpper())).ToList();
                }

                listaPerfiles = listaPerfiles.OrderBy(p => p.NombrePerfil).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de perfiles";
            }
            return View(listaPerfiles.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<PerfilModel> listaPerfiles = new List<PerfilModel>();
            try
            {
                //OBTENEMOS TODOS LOS PERFILES NO ELIMINADOS DE LA BASE DE DATOS
                var perfiles = from p in db.perfiles
                               where p.estado != null
                               select new PerfilModel
                               {
                                   Id = p.id,
                                   NombrePerfil = p.perfil,
                                   Estado = p.estado,
                                   EstadoDescrip = p.estado == true ? "Activo" : "Inactivo"
                               };
                listaPerfiles = perfiles.ToList();

                //FILTRAMOS POR NOMBRE PERFIL LA BUSQUEDA
                var fcNombrePerfil = fc["txtPerfil"];
                if (fcNombrePerfil != "")
                {
                    string descripcion = Convert.ToString(fcNombrePerfil);
                    listaPerfiles = listaPerfiles.Where(p => p.NombrePerfil.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }

                listaPerfiles = listaPerfiles.OrderBy(p => p.NombrePerfil).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtPerfil = fcNombrePerfil;
                Session["sesionPerfilesNombre"] = fcNombrePerfil;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar perfiles";
            }
            return View(listaPerfiles.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Perfil

        [HttpGet]
        [AutorizarUsuario("Perfil", "Create")]
        public ActionResult Create()
        {
            PerfilModel perfil = new PerfilModel();
            return View(perfil);
        }

        [HttpPost]
        public ActionResult Create(PerfilModel perfilModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN PERFIL EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.perfiles.Where(p => p.perfil.ToUpper() == perfilModelo.NombrePerfil.Trim().ToUpper() && p.estado != null).Count();
                    if (cantidad == 0)
                    {
                        perfiles perfil = new perfiles
                        {
                            perfil = perfilModelo.NombrePerfil.Trim(),
                            estado = true
                        };
                        db.perfiles.Add(perfil);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un perfil registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el perfil en la base de datos");
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
                return View(perfilModelo);
            }
        }

        #endregion

        #region Editar Perfil

        [HttpGet]
        [AutorizarUsuario("Perfil", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PerfilModel perfilEdit = new PerfilModel();
            try
            {
                var perfil = db.perfiles.Where(p => p.id == id).FirstOrDefault();
                perfilEdit.Id = perfil.id;
                perfilEdit.NombrePerfil = perfil.perfil;
                perfilEdit.Estado = perfil.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = perfil.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(perfilEdit);
        }

        [HttpPost]
        public ActionResult Edit(PerfilModel perfilModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN PERFIL EN LA BASE DE DATOS PARA PODER ACTUALIZAR
                    int cantidad = db.perfiles.Where(p => p.perfil.ToUpper() == perfilModelo.NombrePerfil.Trim().ToUpper() && p.estado != null && p.id != perfilModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var perfil = db.perfiles.Where(p => p.id == perfilModelo.Id).FirstOrDefault();
                        perfil.perfil = perfilModelo.NombrePerfil.Trim();
                        bool nuevoEstado = perfilModelo.EstadoDescrip == "A" ? true : false;
                        perfil.estado = nuevoEstado;
                        db.Entry(perfil).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un perfil registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el perfil en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", perfilModelo.EstadoDescrip);
                return View(perfilModelo);
            }
        }

        #endregion

        #region Eliminar Perfil

        [HttpPost]
        public ActionResult EliminarPerfil(string perfilId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Perfil", "EliminarPerfil");
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
                                //OBTENEMOS EL REGISTRO PARA PODER ELIMINAR (ACTUALIZAR ESTADO A NULL)
                                int idPerfil = Convert.ToInt32(perfilId);
                                var perfil = context.perfiles.Where(p => p.id == idPerfil).FirstOrDefault();
                                perfil.estado = null;
                                context.Entry(perfil).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();

                                //DESHABILITAMOS LOS PERMISOS RELACIONADOS AL PERFIL SELECCIONADO
                                var listaPermisos = context.permisos.Where(p => p.id_perfil == idPerfil && p.habilitado == true).ToList();
                                listaPermisos.ForEach(p => p.habilitado = false);
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Perfil") }, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}