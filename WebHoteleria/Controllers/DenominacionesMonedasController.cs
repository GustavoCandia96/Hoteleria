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
    public class DenominacionesMonedasController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Denominaciones Monedas

        [HttpGet]
        [AutorizarUsuario("DenominacionesMonedas", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<DenominacionMonedaModel> listaDenominacionesMonedas = new List<DenominacionMonedaModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesIdMoneda = Convert.ToString(Session["sesionDenominacionesMonedasIdMoneda"]);
                ViewBag.ddlMonedas = new SelectList(db.monedas.Where(m => m.estado == true).OrderBy(m => m.nombre_moneda).ToList(), "id", "nombre_moneda", sesIdMoneda);
                string sesIdTipoDenominacion = Convert.ToString(Session["sesionDenominacionesMonedasIdTipoDenominacion"]);
                ViewBag.ddlTiposDenominaciones = new SelectList(db.denominaciones_monedas_tipos.Where(dmt => dmt.estado == true).OrderBy(dmt => dmt.tipo).ToList(), "id", "tipo", sesIdTipoDenominacion);

                //OBTENEMOS TODAS LAS DENOMINACIONES MONEDAS NO ELIMINADAS DE LA BASE DE DATOS
                var denominacionesMonedas = from dm in db.denominaciones_monedas
                                            where dm.estado != null
                                            select new DenominacionMonedaModel
                                            {
                                                Id = dm.id,
                                                IdTipoDenominacion = dm.id_tipo_denominacion,
                                                Denominacion = dm.denominacion,
                                                IdMoneda = dm.id_moneda,
                                                Orden = dm.orden,
                                                Estado = dm.estado.Value,
                                                NombreTipoDenominacion = dm.denominaciones_monedas_tipos.tipo,
                                                NombreMoneda = dm.monedas.nombre_moneda,
                                                EstadoDescrip = dm.estado == true ? "Activo" : "Inactivo"
                                            };
                listaDenominacionesMonedas = denominacionesMonedas.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesIdMoneda != "")
                {
                    int idMoneda = Convert.ToInt32(sesIdMoneda);
                    listaDenominacionesMonedas = listaDenominacionesMonedas.Where(dm => dm.IdMoneda == idMoneda).ToList();
                }
                if (sesIdTipoDenominacion != "")
                {
                    int idTipoDenominacion = Convert.ToInt32(sesIdTipoDenominacion);
                    listaDenominacionesMonedas = listaDenominacionesMonedas.Where(dm => dm.IdTipoDenominacion == idTipoDenominacion).ToList();
                }

                listaDenominacionesMonedas = listaDenominacionesMonedas.OrderBy(dm => dm.Denominacion).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de denominaciones monedas";
            }
            return View(listaDenominacionesMonedas.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<DenominacionMonedaModel> listaDenominacionesMonedas = new List<DenominacionMonedaModel>();
            try
            {
                //OBTENEMOS TODAS LAS DENOMINACIONES MONEDAS NO ELIMINADAS DE LA BASE DE DATOS
                var denominacionesMonedas = from dm in db.denominaciones_monedas
                                            where dm.estado != null
                                            select new DenominacionMonedaModel
                                            {
                                                Id = dm.id,
                                                IdTipoDenominacion = dm.id_tipo_denominacion,
                                                Denominacion = dm.denominacion,
                                                IdMoneda = dm.id_moneda,
                                                Orden = dm.orden,
                                                Estado = dm.estado.Value,
                                                NombreTipoDenominacion = dm.denominaciones_monedas_tipos.tipo,
                                                NombreMoneda = dm.monedas.nombre_moneda,
                                                EstadoDescrip = dm.estado == true ? "Activo" : "Inactivo"
                                            };
                listaDenominacionesMonedas = denominacionesMonedas.ToList();

                //FILTRAMOS POR MONEDA Y TIPO DENOMINACION EN BUSQUEDA
                var fcIdMoneda = fc["ddlMonedas"];
                if (fcIdMoneda != "")
                {
                    int idMoneda = Convert.ToInt32(fcIdMoneda);
                    listaDenominacionesMonedas = listaDenominacionesMonedas.Where(dm => dm.IdMoneda == idMoneda).ToList();
                }
                var fcIdTipoDenominacion = fc["ddlTiposDenominaciones"];
                if (fcIdTipoDenominacion != "")
                {
                    int idTipoDenominacion = Convert.ToInt32(fcIdTipoDenominacion);
                    listaDenominacionesMonedas = listaDenominacionesMonedas.Where(dm => dm.IdTipoDenominacion == idTipoDenominacion).ToList();
                }

                listaDenominacionesMonedas = listaDenominacionesMonedas.OrderBy(dm => dm.Denominacion).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.ddlMonedas = new SelectList(db.monedas.Where(m => m.estado == true).OrderBy(m => m.nombre_moneda).ToList(), "id", "nombre_moneda", fcIdMoneda);
                Session["sesionDenominacionesMonedasIdMoneda"] = fcIdMoneda;
                ViewBag.ddlTiposDenominaciones = new SelectList(db.denominaciones_monedas_tipos.Where(dmt => dmt.estado == true).OrderBy(dmt => dmt.tipo).ToList(), "id", "tipo", fcIdTipoDenominacion);
                Session["sesionDenominacionesMonedasIdTipoDenominacion"] = fcIdTipoDenominacion;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar denominaciones monedas";
            }
            return View(listaDenominacionesMonedas.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Denominación Moneda

        [HttpGet]
        [AutorizarUsuario("DenominacionesMonedas", "Create")]
        public ActionResult Create()
        {
            DenominacionMonedaModel denominacionMoneda = new DenominacionMonedaModel();
            CargarDatosDenominacionMoneda(denominacionMoneda);
            return View(denominacionMoneda);
        }

        [HttpPost]
        public ActionResult Create(DenominacionMonedaModel denominacionMonedaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA DENOMINACION MONEDA CON EL MISMO TIPO Y MISMA MONEDA PODER AGREGAR
                    int cantidad = db.denominaciones_monedas.Where(dm => dm.id_tipo_denominacion == denominacionMonedaModelo.IdTipoDenominacion && dm.id_moneda == denominacionMonedaModelo.IdMoneda && dm.denominacion.Trim().ToUpper() == denominacionMonedaModelo.Denominacion.Trim().ToUpper() && dm.estado != null).Count();
                    if (cantidad == 0)
                    {
                        string strOrden = denominacionMonedaModelo.StrOrden;
                        strOrden = strOrden.Replace(".", "");
                        int orden = Convert.ToInt32(strOrden);
                        int cantidadOrden = db.denominaciones_monedas.Where(dm => dm.id_moneda == denominacionMonedaModelo.IdMoneda && dm.orden == orden && dm.estado != null).Count();
                        if (cantidadOrden == 0)
                        {
                            //AGREGAMOS EL REGISTRO DE DENOMINACION MONEDA
                            denominaciones_monedas denominacionMoneda = new denominaciones_monedas
                            {
                                id_tipo_denominacion = denominacionMonedaModelo.IdTipoDenominacion,
                                denominacion = denominacionMonedaModelo.Denominacion,
                                id_moneda = denominacionMonedaModelo.IdMoneda,
                                orden = orden,
                                estado = true
                            };
                            db.denominaciones_monedas.Add(denominacionMoneda);
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("Duplicado", "Ya existe el número de orden con la misma moneda");
                            retornoVista = true;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una denominación de moneda con el mismo tipo y misma moneda");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la denominación moneda en la base de datos");
                    retornoVista = true;
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
                CargarDatosDenominacionMoneda(denominacionMonedaModelo);
                db.Dispose();
                return View(denominacionMonedaModelo);
            }
        }

        #endregion

        #region Editar Denominación Moneda

        [HttpGet]
        [AutorizarUsuario("DenominacionesMonedas", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DenominacionMonedaModel denominacionMonedaEdit = new DenominacionMonedaModel();
            try
            {
                //OBTENEMOS EL REGISTRO DE DENOMINACIONES MONEDAS Y CARGAMOS AL MODELO PARA PODER EDITAR
                var denominacionMoneda = db.denominaciones_monedas.Where(dm => dm.id == id).FirstOrDefault();
                denominacionMonedaEdit.Id = denominacionMoneda.id;
                denominacionMonedaEdit.IdTipoDenominacion = denominacionMoneda.id_tipo_denominacion;
                denominacionMonedaEdit.IdMoneda = denominacionMoneda.id_moneda;
                denominacionMonedaEdit.Denominacion = denominacionMoneda.denominacion;
                denominacionMonedaEdit.StrOrden = String.Format("{0:#,##0.##}", denominacionMoneda.orden);
                denominacionMonedaEdit.Estado = denominacionMoneda.estado.Value;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = denominacionMoneda.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                //CARGAMOS LOS DATOS NECESARIOS PARA PODER EDITAR EL REGISTRO
                CargarDatosDenominacionMoneda(denominacionMonedaEdit);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(denominacionMonedaEdit);
        }

        [HttpPost]
        public ActionResult Edit(DenominacionMonedaModel denominacionMonedaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA DENOMINACION MONEDA CON EL MISMO TIPO Y MISMA MONEDA PODER ACTUALIZAR
                    int cantidad = db.denominaciones_monedas.Where(dm => dm.id_tipo_denominacion == denominacionMonedaModelo.IdTipoDenominacion && dm.id_moneda == denominacionMonedaModelo.IdMoneda && dm.denominacion.Trim().ToUpper() == denominacionMonedaModelo.Denominacion.Trim().ToUpper() && dm.estado != null && dm.id != denominacionMonedaModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        string strOrden = denominacionMonedaModelo.StrOrden;
                        strOrden = strOrden.Replace(".", "");
                        int orden = Convert.ToInt32(strOrden);
                        int cantidadOrden = db.denominaciones_monedas.Where(dm => dm.id_moneda == denominacionMonedaModelo.IdMoneda && dm.orden == orden && dm.estado != null && dm.id != denominacionMonedaModelo.Id).Count();
                        if (cantidadOrden == 0)
                        {
                            //OBTENEMOS EL REGISTRO DE DENOMINACION MONEDA Y ACTUALIZAMOS LOS DATOS
                            var denominacionMoneda = db.denominaciones_monedas.Where(dm => dm.id == denominacionMonedaModelo.Id).FirstOrDefault();
                            denominacionMoneda.id_tipo_denominacion = denominacionMonedaModelo.IdTipoDenominacion;
                            denominacionMoneda.id_moneda = denominacionMonedaModelo.IdMoneda;
                            denominacionMoneda.denominacion = denominacionMonedaModelo.Denominacion;
                            denominacionMoneda.orden = orden;
                            bool nuevoEstado = denominacionMonedaModelo.EstadoDescrip == "A" ? true : false;
                            denominacionMoneda.estado = nuevoEstado;
                            db.Entry(denominacionMoneda).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("Duplicado", "Ya existe el número de orden con la misma moneda");
                            retornoVista = true;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una denominación de moneda con el mismo tipo y misma moneda");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar la denominación moneda en la base de datos");
                    retornoVista = true;
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
                //CARGAMOS LOS DATOS NECESARIOS DE DENOMINACION MONEDA PARA ENVIAR A LA VISTA EN EDICION
                CargarDatosDenominacionMoneda(denominacionMonedaModelo);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", denominacionMonedaModelo.EstadoDescrip);
                db.Dispose();
                return View(denominacionMonedaModelo);
            }
        }

        #endregion

        #region Eliminar Denominación Moneda

        [HttpPost]
        public ActionResult EliminarDenominacionMoneda(string denominacionMonedaId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("DenominacionesMonedas", "EliminarDenominacionMoneda");
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
                                //OBTENEMOS EL REGISTRO Y ELIMINAMOS (ACTUALIZAMOS ESTADO A NULL)
                                int idDenominacionMoneda = Convert.ToInt32(denominacionMonedaId);
                                var denominacionMoneda = context.denominaciones_monedas.Where(dm => dm.id == idDenominacionMoneda).FirstOrDefault();
                                denominacionMoneda.estado = null;
                                context.Entry(denominacionMoneda).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "DenominacionesMonedas") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Propiedades

        /*
         * METODO QUE CARGA LOS DATOS NECESARIOS DE DENOMINACION MONEDA PARA DEVOLVER A LA VISTA EN AGREGAR O ACTUALIZAR REGISTROS
         */
        private void CargarDatosDenominacionMoneda(DenominacionMonedaModel denominacionMonedaModelo)
        {
            ViewBag.IdTipoDenominacion = new SelectList(db.denominaciones_monedas_tipos.Where(dmt => dmt.estado == true).OrderBy(dmt => dmt.tipo).ToList(), "id", "tipo", denominacionMonedaModelo.IdTipoDenominacion);
            ViewBag.IdMoneda = new SelectList(db.monedas.Where(m => m.estado == true).OrderBy(m => m.nombre_moneda).ToList(), "id", "nombre_moneda", denominacionMonedaModelo.IdMoneda);
        }

        #endregion


    }
}