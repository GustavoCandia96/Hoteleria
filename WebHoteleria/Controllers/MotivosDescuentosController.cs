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
    public class MotivosDescuentosController : Controller
    {

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #region Listado de Motivos Descuentos

        [HttpGet]
        [AutorizarUsuario("MotivosDescuentos", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<MotivoDescuentoModel> listaMotivosDescuentos = new List<MotivoDescuentoModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesMotivo = Convert.ToString(Session["sesionMotivosDescuentosNombre"]);
                ViewBag.txtMotivo = sesMotivo;

                //OBTENEMOS TODOS LOS MOTIVOS NO ELIMINADAS DE LA BASE DE DATOS
                var motivosDescuentos = from md in db.motivos_descuentos
                                        where md.estado != null
                                        select new MotivoDescuentoModel
                                        {
                                            Id = md.id,
                                            Motivo = md.motivo,
                                            Estado = md.estado,
                                            EstadoDescrip = md.estado == true ? "Activo" : "Inactivo"
                                        };
                listaMotivosDescuentos = motivosDescuentos.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesMotivo != "")
                {
                    listaMotivosDescuentos = listaMotivosDescuentos.Where(md => md.Motivo.ToUpper().Contains(sesMotivo.Trim().ToUpper())).ToList();
                }

                listaMotivosDescuentos = listaMotivosDescuentos.OrderBy(md => md.Motivo).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de motivos descuentos";
            }
            return View(listaMotivosDescuentos.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<MotivoDescuentoModel> listaMotivosDescuentos = new List<MotivoDescuentoModel>();
            try
            {
                //OBTENEMOS TODOS LOS MOTIVOS NO ELIMINADAS DE LA BASE DE DATOS
                var motivosDescuentos = from md in db.motivos_descuentos
                                        where md.estado != null
                                        select new MotivoDescuentoModel
                                        {
                                            Id = md.id,
                                            Motivo = md.motivo,
                                            Estado = md.estado,
                                            EstadoDescrip = md.estado == true ? "Activo" : "Inactivo"
                                        };
                listaMotivosDescuentos = motivosDescuentos.ToList();

                //FILTRAMOS POR MOTIVO LA BUSQUEDA
                var fcMotivo = fc["txtMotivo"];
                if (fcMotivo != "")
                {
                    string descripcion = Convert.ToString(fcMotivo);
                    listaMotivosDescuentos = listaMotivosDescuentos.Where(md => md.Motivo.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }
                listaMotivosDescuentos.OrderBy(md => md.Motivo).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtMotivo = fcMotivo;
                Session["sesionMotivosDescuentosNombre"] = fcMotivo;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar motivos descuentos";
            }
            return View(listaMotivosDescuentos.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Motivo Descuento

        [HttpGet]
        [AutorizarUsuario("MotivosDescuentos", "Create")]
        public ActionResult Create()
        {
            MotivoDescuentoModel motivoDescuento = new MotivoDescuentoModel();
            return View(motivoDescuento);
        }

        [HttpPost]
        public ActionResult Create(MotivoDescuentoModel motivoDescuentoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL MOTIVO DESCUENTO EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.motivos_descuentos.Where(md => md.motivo.ToUpper() == motivoDescuentoModelo.Motivo.ToUpper() && md.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //AGREGAMOS EL REGISTRO DE UN MOTIVO DESCUENTO
                        motivos_descuentos motivoDescuento = new motivos_descuentos
                        {
                            motivo = motivoDescuentoModelo.Motivo,
                            estado = true
                        };
                        db.motivos_descuentos.Add(motivoDescuento);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un motivo descuento registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el motivo descuento en la base de datos");
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
                return View(motivoDescuentoModelo);
            }
        }

        #endregion

        #region Editar Motivo Descuento

        [HttpGet]
        [AutorizarUsuario("MotivosDescuentos", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MotivoDescuentoModel motivoDescuentoEdit = new MotivoDescuentoModel();
            try
            {
                //OBTENEMOS EL REGISTRO DE MOTIVO DESCUENTO Y CARGAMOS LAS PROPIEDADES EN EL MODELO
                var motivoDescuento = db.motivos_descuentos.Where(md => md.id == id).FirstOrDefault();
                motivoDescuentoEdit.Id = motivoDescuento.id;
                motivoDescuentoEdit.Motivo = motivoDescuento.motivo;
                motivoDescuentoEdit.Estado = motivoDescuento.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = motivoDescuento.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(motivoDescuentoEdit);
        }

        [HttpPost]
        public ActionResult Edit(MotivoDescuentoModel motivoDescuentoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL MOTIVO DESCUENTO EN LA BASE DE DATOS PARA PODER ACTUALIZAR
                    int cantidad = db.motivos_descuentos.Where(md => md.motivo.ToUpper() == motivoDescuentoModelo.Motivo.ToUpper() && md.estado != null && md.id != motivoDescuentoModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //ACTUALIZAMOS EL REGISTRO DE UN MOTIVO DESCUENTO
                        var motivoDescuento = db.motivos_descuentos.Where(md => md.id == motivoDescuentoModelo.Id).FirstOrDefault();
                        motivoDescuento.motivo = motivoDescuentoModelo.Motivo;
                        bool nuevoEstado = motivoDescuentoModelo.EstadoDescrip == "A" ? true : false;
                        motivoDescuento.estado = nuevoEstado;
                        db.Entry(motivoDescuento).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un motivo descuento registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el motivo descuento en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", motivoDescuentoModelo.EstadoDescrip);
                return View(motivoDescuentoModelo);
            }
        }

        #endregion

        #region Eliminar Motivo Descuento

        [HttpPost]
        public ActionResult EliminarMotivoDescuento(string motivoDescuentoId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("MotivosDescuentos", "EliminarMotivoDescuento");
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
                                int idMotivoDescuento = Convert.ToInt32(motivoDescuentoId);
                                var motivoDescuento = context.motivos_descuentos.Where(md => md.id == idMotivoDescuento).FirstOrDefault();
                                motivoDescuento.estado = null;
                                context.Entry(motivoDescuento).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "MotivosDescuentos") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}