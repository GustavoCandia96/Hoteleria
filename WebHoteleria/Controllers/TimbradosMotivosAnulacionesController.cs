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
    public class TimbradosMotivosAnulacionesController : Controller
    {

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #region Listado de Motivos Anulaciones

        [HttpGet]
        [AutorizarUsuario("TimbradosMotivosAnulaciones", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TimbradoMotivoAnulacionModel> listaMotivosAnulaciones = new List<TimbradoMotivoAnulacionModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesMotivo = Convert.ToString(Session["sesionTimbradosMotivosAnulacionesMotivo"]);
                ViewBag.txtMotivo = sesMotivo;

                //OBTENEMOS TODOS LOS MOTIVOS ANULACIONES DE LA BASE DE DATOS
                var timbradosMotivosAnulaciones = from tma in db.timbrados_motivos_anulaciones
                                                  where tma.estado != null
                                                  select new TimbradoMotivoAnulacionModel
                                                  {
                                                      Id = tma.id,
                                                      Motivo = tma.motivo,
                                                      Estado = tma.estado,
                                                      EstadoDescrip = tma.estado == true ? "Activo" : "Inactivo"
                                                  };
                listaMotivosAnulaciones = timbradosMotivosAnulaciones.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesMotivo != "")
                {
                    listaMotivosAnulaciones = listaMotivosAnulaciones.Where(tma => tma.Motivo.ToUpper().Contains(sesMotivo.Trim().ToUpper())).ToList();
                }

                listaMotivosAnulaciones = listaMotivosAnulaciones.OrderBy(tma => tma.Motivo).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de motivos anulaciones de un timbrado";
            }
            return View(listaMotivosAnulaciones.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TimbradoMotivoAnulacionModel> listaMotivosAnulaciones = new List<TimbradoMotivoAnulacionModel>();
            try
            {
                //OBTENEMOS TODOS LOS MOTIVOS ANULACIONES DE LA BASE DE DATOS
                var timbradosMotivosAnulaciones = from tma in db.timbrados_motivos_anulaciones
                                                  where tma.estado != null
                                                  select new TimbradoMotivoAnulacionModel
                                                  {
                                                      Id = tma.id,
                                                      Motivo = tma.motivo,
                                                      Estado = tma.estado,
                                                      EstadoDescrip = tma.estado == true ? "Activo" : "Inactivo"
                                                  };
                listaMotivosAnulaciones = timbradosMotivosAnulaciones.ToList();

                //FILTRAMOS POR MOTIVO ANULACION LA BUSQUEDA
                var fcMotivo = fc["txtMotivo"];
                if (fcMotivo != "")
                {
                    string descripcion = Convert.ToString(fcMotivo);
                    listaMotivosAnulaciones = listaMotivosAnulaciones.Where(tma => tma.Motivo.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }
                listaMotivosAnulaciones = listaMotivosAnulaciones.OrderBy(tma => tma.Motivo).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtMotivo = fcMotivo;
                Session["sesionTimbradosMotivosAnulacionesMotivo"] = fcMotivo;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de motivos anulaciones de un timbrado";
            }
            return View(listaMotivosAnulaciones.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Motivo Anulación

        [HttpGet]
        [AutorizarUsuario("TimbradosMotivosAnulaciones", "Create")]
        public ActionResult Create()
        {
            TimbradoMotivoAnulacionModel timbradoMotivoAnulacion = new TimbradoMotivoAnulacionModel();
            return View(timbradoMotivoAnulacion);
        }

        [HttpPost]
        public ActionResult Create(TimbradoMotivoAnulacionModel motivoAnulacionModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN MOTIVO ANULACION EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.timbrados_motivos_anulaciones.Where(tma => tma.motivo.ToUpper() == motivoAnulacionModelo.Motivo.ToUpper() && tma.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //AGREGAMOS EL REGISTRO DE UN MOTIVO ANULACION
                        timbrados_motivos_anulaciones timbradoMotivoAnulacion = new timbrados_motivos_anulaciones
                        {
                            motivo = motivoAnulacionModelo.Motivo,
                            estado = true
                        };
                        db.timbrados_motivos_anulaciones.Add(timbradoMotivoAnulacion);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un motivo anulación registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el motivo anulación en la base de datos");
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
                return View(motivoAnulacionModelo);
            }
        }

        #endregion

        #region Editar Motivo Anulación

        [HttpGet]
        [AutorizarUsuario("TimbradosMotivosAnulaciones", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TimbradoMotivoAnulacionModel motivoAnulacionEdit = new TimbradoMotivoAnulacionModel();
            try
            {
                var motivoAnulacion = db.timbrados_motivos_anulaciones.Where(tma => tma.id == id).FirstOrDefault();
                motivoAnulacionEdit.Id = motivoAnulacion.id;
                motivoAnulacionEdit.Motivo = motivoAnulacion.motivo;
                motivoAnulacionEdit.Estado = motivoAnulacion.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = motivoAnulacion.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(motivoAnulacionEdit);
        }

        [HttpPost]
        public ActionResult Edit(TimbradoMotivoAnulacionModel motivoAnulacionModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN MOTIVO ANULACION EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.timbrados_motivos_anulaciones.Where(tma => tma.motivo.ToUpper() == motivoAnulacionModelo.Motivo.ToUpper() && tma.estado != null && tma.id != motivoAnulacionModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //ACTUALIZAMOS EL REGISTRO DE UN MOTIVO ANULACION
                        var motivoAnulacion = db.timbrados_motivos_anulaciones.Where(tma => tma.id == motivoAnulacionModelo.Id).FirstOrDefault();
                        motivoAnulacion.motivo = motivoAnulacionModelo.Motivo;
                        bool nuevoEstado = motivoAnulacionModelo.EstadoDescrip == "A" ? true : false;
                        motivoAnulacion.estado = nuevoEstado;
                        db.Entry(motivoAnulacion).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un motivo anulación registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el motivo anulación en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", motivoAnulacionModelo.EstadoDescrip);
                return View(motivoAnulacionModelo);
            }
        }

        #endregion

        #region Eliminar Motivo Anulación

        [HttpPost]
        public ActionResult EliminarTimbradoMotivoAnulacion(string timbradoMotivoAnulacionId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("TimbradosMotivosAnulaciones", "EliminarTimbradoMotivoAnulacion");
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
                                int idTimbradoMotivoAnulacion = Convert.ToInt32(timbradoMotivoAnulacionId);
                                var motivoAnulacion = context.timbrados_motivos_anulaciones.Where(tma => tma.id == idTimbradoMotivoAnulacion).FirstOrDefault();
                                motivoAnulacion.estado = null;
                                context.Entry(motivoAnulacion).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "TimbradosMotivosAnulaciones") }, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}