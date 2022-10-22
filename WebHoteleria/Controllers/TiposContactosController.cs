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
    public class TiposContactosController : Controller
    {
        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #region Listado de Tipos Contactos

        [HttpGet]
        [AutorizarUsuario("TiposContactos", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TipoContactoModel> listaTiposContactos = new List<TipoContactoModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomTipoContacto = Convert.ToString(Session["sesionTiposContactosNombre"]);
                ViewBag.txtTipoContacto = sesNomTipoContacto;

                //OBTENEMOS TODOS LOS TIPOS CONTACTOS ACTIVOS DE LA BASE DE DATOS
                var tiposContactos = from tc in db.tipos_contactos
                                     where tc.estado != null
                                     select new TipoContactoModel
                                     {
                                         Id = tc.id,
                                         Descripcion = tc.descripcion,
                                         Estado = tc.estado,
                                         EstadoDescrip = tc.estado == true ? "Activo" : "Inactivo"
                                     };
                listaTiposContactos = tiposContactos.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomTipoContacto != "")
                {
                    listaTiposContactos = listaTiposContactos.Where(tc => tc.Descripcion.ToUpper().Contains(sesNomTipoContacto.Trim().ToUpper())).ToList();
                }

                listaTiposContactos = listaTiposContactos.OrderBy(tc => tc.Descripcion).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de tipos contactos";
            }
            return View(listaTiposContactos.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TipoContactoModel> listaTiposContactos = new List<TipoContactoModel>();
            try
            {
                //OBTENEMOS TODOS LOS TIPOS CONTACTOS ACTIVOS DE LA BASE DE DATOS
                var tiposContactos = from tc in db.tipos_contactos
                                     where tc.estado != null
                                     select new TipoContactoModel
                                     {
                                         Id = tc.id,
                                         Descripcion = tc.descripcion,
                                         Estado = tc.estado,
                                         EstadoDescrip = tc.estado == true ? "Activo" : "Inactivo"
                                     };
                listaTiposContactos = tiposContactos.ToList();

                //FILTRAMOS POR NOMBRE TIPO CONTACTO LA BUSQUEDA
                var fcNombreTipoContacto = fc["txtTipoContacto"];
                if (fcNombreTipoContacto != "")
                {
                    string descripcion = Convert.ToString(fcNombreTipoContacto);
                    listaTiposContactos = listaTiposContactos.Where(tc => tc.Descripcion.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }
                listaTiposContactos = listaTiposContactos.OrderBy(tc => tc.Descripcion).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtTipoContacto = fcNombreTipoContacto;
                Session["sesionTiposContactosNombre"] = fcNombreTipoContacto;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar tipos contactos";
            }
            return View(listaTiposContactos.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Tipo Contacto

        [HttpGet]
        [AutorizarUsuario("TiposContactos", "Create")]
        public ActionResult Create()
        {
            TipoContactoModel tipoContacto = new TipoContactoModel();
            return View(tipoContacto);
        }

        [HttpPost]
        public ActionResult Create(TipoContactoModel tipoContactoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL TIPO CONTACTO EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.tipos_contactos.Where(tc => tc.descripcion.ToUpper() == tipoContactoModelo.Descripcion.ToUpper() && tc.estado != null).Count();
                    if (cantidad == 0)
                    {
                        tipos_contactos tipoContacto = new tipos_contactos
                        {
                            descripcion = tipoContactoModelo.Descripcion,
                            estado = true
                        };
                        db.tipos_contactos.Add(tipoContacto);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un tipo contacto registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el tipo contacto en la base de datos");
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
                return View(tipoContactoModelo);
            }
        }

        #endregion

        #region Editar Tipo Contacto

        [HttpGet]
        [AutorizarUsuario("TiposContactos", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TipoContactoModel tipoContactoEdit = new TipoContactoModel();
            try
            {
                var tipoContacto = db.tipos_contactos.Where(tc => tc.id == id).FirstOrDefault();
                tipoContactoEdit.Id = tipoContacto.id;
                tipoContactoEdit.Descripcion = tipoContacto.descripcion;
                tipoContactoEdit.Estado = tipoContacto.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = tipoContacto.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(tipoContactoEdit);
        }

        [HttpPost]
        public ActionResult Edit(TipoContactoModel tipoContactoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL TIPO CONTACTO EN LA BASE DE DATOS PARA PODER ACTUALIZAR
                    int cantidad = db.tipos_contactos.Where(tc => tc.descripcion.ToUpper() == tipoContactoModelo.Descripcion.ToUpper() && tc.estado != null && tc.id != tipoContactoModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var tipoContacto = db.tipos_contactos.Where(tc => tc.id == tipoContactoModelo.Id).FirstOrDefault();
                        tipoContacto.descripcion = tipoContactoModelo.Descripcion;
                        bool nuevoEstado = tipoContactoModelo.EstadoDescrip == "A" ? true : false;
                        tipoContacto.estado = nuevoEstado;
                        db.Entry(tipoContacto).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un tipo contacto registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el tipo contacto en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", tipoContactoModelo.EstadoDescrip);
                return View(tipoContactoModelo);
            }
        }

        #endregion

        #region Eliminar Tipo Contacto

        [HttpPost]
        public ActionResult EliminarTipoContacto(string tipoContactoId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("TiposContactos", "EliminarTipoContacto");
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
                                int idTipoContacto = Convert.ToInt32(tipoContactoId);
                                var tipoContacto = context.tipos_contactos.Where(tc => tc.id == idTipoContacto).FirstOrDefault();
                                tipoContacto.estado = null;
                                context.Entry(tipoContacto).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "TiposContactos") }, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}