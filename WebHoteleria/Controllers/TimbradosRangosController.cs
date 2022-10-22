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
    public class TimbradosRangosController : Controller
    {

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #region Listado de Timbrados Rangos

        [HttpGet]
        [AutorizarUsuario("TimbradosRangos", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TimbradoRangoModel> listaTimbradosRangos = new List<TimbradoRangoModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesIdTimbradoTipoRango = Convert.ToString(Session["sesionTimbradosRangosTipos"]);
                ViewBag.ddlTiposRangos = new SelectList(db.timbrados_rangos_tipos.Where(trt => trt.estado == true).OrderBy(trt => trt.tipo).ToList(), "id", "tipo", sesIdTimbradoTipoRango);
                string sesIdSucursal = Convert.ToString(Session["sesionTimbradosRangosIdSucursal"]);
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", sesIdSucursal);

                //OBTENEMOS TODOS LOS TIMBRADOS RANGOS ACTIVOS DE LA BASE DE DATOS Y ORDENAMOS POR FECHA DE REGISTRO
                var timbradosRangos = from tr in db.timbrados_rangos
                                      where tr.estado != null
                                      select new TimbradoRangoModel
                                      {
                                          Id = tr.id,
                                          IdTimbradoTipoRango = tr.id_timbrado_tipo_rango,
                                          IdTimbrado = tr.id_timbrado,
                                          IdSucursal = tr.id_sucursal,
                                          CodigoSucursal = tr.codigo_sucursal,
                                          PuntoExpedicion = tr.punto_expedicion,
                                          CantidadCeros = tr.cantidad_ceros,
                                          Desde = tr.desde,
                                          Hasta = tr.hasta,
                                          NumeracionActual = tr.numeracion_actual,
                                          Utilizado = tr.utilizado,
                                          TipoRango = tr.timbrados_rangos_tipos.tipo,
                                          NroTimbrado = tr.timbrados.nro_timbrado,
                                          NombreSucursal = tr.sucursales.nombre_sucursal,
                                          CantidadUsada = tr.cantidad_usada,
                                          NombreTimbradoTipoDocumento = tr.timbrados.timbrados_tipos_documentos.nombre_documento,
                                          EstadoDescrip = tr.estado == true ? "Activo" : "Inactivo"
                                      };

                listaTimbradosRangos = timbradosRangos.OrderByDescending(tr => tr.Id).ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesIdTimbradoTipoRango != "")
                {
                    int idTipoRango = Convert.ToInt32(sesIdTimbradoTipoRango);
                    listaTimbradosRangos = listaTimbradosRangos.Where(tr => tr.IdTimbradoTipoRango == idTipoRango).ToList();
                }
                if (sesIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(sesIdSucursal);
                    listaTimbradosRangos = listaTimbradosRangos.Where(tr => tr.IdSucursal == idSucursal).ToList();
                }
                listaTimbradosRangos = listaTimbradosRangos.OrderByDescending(tr => tr.Id).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de timbrados rangos";
            }
            return View(listaTimbradosRangos.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<TimbradoRangoModel> listaTimbradosRangos = new List<TimbradoRangoModel>();
            try
            {
                //OBTENEMOS TODOS LOS TIMBRADOS RANGOS ACTIVOS DE LA BASE DE DATOS Y ORDENAMOS POR FECHA DE REGISTRO
                var timbradosRangos = from tr in db.timbrados_rangos
                                      where tr.estado != null
                                      select new TimbradoRangoModel
                                      {
                                          Id = tr.id,
                                          IdTimbradoTipoRango = tr.id_timbrado_tipo_rango,
                                          IdTimbrado = tr.id_timbrado,
                                          IdSucursal = tr.id_sucursal,
                                          CodigoSucursal = tr.codigo_sucursal,
                                          PuntoExpedicion = tr.punto_expedicion,
                                          CantidadCeros = tr.cantidad_ceros,
                                          Desde = tr.desde,
                                          Hasta = tr.hasta,
                                          NumeracionActual = tr.numeracion_actual,
                                          Utilizado = tr.utilizado,
                                          TipoRango = tr.timbrados_rangos_tipos.tipo,
                                          NroTimbrado = tr.timbrados.nro_timbrado,
                                          NombreSucursal = tr.sucursales.nombre_sucursal,
                                          CantidadUsada = tr.cantidad_usada,
                                          NombreTimbradoTipoDocumento = tr.timbrados.timbrados_tipos_documentos.nombre_documento,
                                          EstadoDescrip = tr.estado == true ? "Activo" : "Inactivo"
                                      };

                listaTimbradosRangos = timbradosRangos.OrderByDescending(tr => tr.Id).ToList();

                //FILTRAMOS POR TIPO RANGO Y SUCURSAL EN BUSQUEDA
                var fcIdTipoRango = fc["ddlTiposRangos"];
                if (fcIdTipoRango != "")
                {
                    int idTipoRango = Convert.ToInt32(fcIdTipoRango);
                    listaTimbradosRangos = listaTimbradosRangos.Where(tr => tr.IdTimbradoTipoRango == idTipoRango).ToList();
                }
                var fcIdSucursal = fc["ddlSucursales"];
                if (fcIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(fcIdSucursal);
                    listaTimbradosRangos = listaTimbradosRangos.Where(tr => tr.IdSucursal == idSucursal).ToList();
                }

                listaTimbradosRangos.OrderBy(tr => tr.Id).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.ddlTiposRangos = new SelectList(db.timbrados_rangos_tipos.Where(trt => trt.estado == true).OrderBy(trt => trt.tipo).ToList(), "id", "tipo", fcIdTipoRango);
                Session["sesionTimbradosRangosTipos"] = fcIdTipoRango;
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", fcIdSucursal);
                Session["sesionTimbradosRangosIdSucursal"] = fcIdSucursal;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar timbrados rangos";
            }
            return View(listaTimbradosRangos.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Timbrado Rango

        [HttpGet]
        [AutorizarUsuario("TimbradosRangos", "Create")]
        public ActionResult Create()
        {
            TimbradoRangoModel timbradoRango = new TimbradoRangoModel();
            ViewBag.IdTimbradoFormato = new SelectList(db.timbrados_formatos.Where(tf => tf.estado == true).OrderBy(tf => tf.nombre_formato).ToList(), "id", "nombre_formato");
            ViewBag.IdTimbradoTipoDocumento = new SelectList(db.timbrados_tipos_documentos.Where(ttd => ttd.estado == true).OrderBy(ttd => ttd.nombre_documento).ToList(), "id", "nombre_documento");
            ViewBag.IdTimbrado = new SelectList(db.timbrados.Where(t => t.id == 0).OrderBy(t => t.nro_timbrado).ToList(), "id", "nro_timbrado");
            ViewBag.IdTimbradoTipoRango = new SelectList(db.timbrados_rangos_tipos.Where(trt => trt.estado == true).OrderBy(trt => trt.tipo).ToList(), "id", "tipo");
            ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal");
            return View(timbradoRango);
        }

        [HttpPost]
        public ActionResult Create(TimbradoRangoModel timbradoRangoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI EL RANGO A INGRESAR DEL TIMBRADO YA NO EXISTE
                    bool rangoHabilitado = true;
                    int countDesde = db.timbrados_rangos.Where(t => t.id_timbrado == timbradoRangoModelo.IdTimbrado && timbradoRangoModelo.Desde >= t.desde && timbradoRangoModelo.Desde <= t.hasta && t.estado != null).Count();
                    if (countDesde > 0) { rangoHabilitado = false; }
                    int countHasta = db.timbrados_rangos.Where(t => t.id_timbrado == timbradoRangoModelo.IdTimbrado && timbradoRangoModelo.Hasta >= t.desde && timbradoRangoModelo.Hasta <= t.hasta && t.estado != null).Count();
                    if (countHasta > 0) { rangoHabilitado = false; }

                    if (rangoHabilitado == true)
                    {
                        //VERIFICAMOS SI LA CANTIDAD DE CEROS PARAMETRIZADO EXISTE EN LA BASE DE DATOS
                        int count = db.parametros.Where(p => p.parametro.ToUpper() == "CANTIDADCEROSCOMPROBANTE").Count();
                        int cantidadCeros = 0;
                        if (count > 0)
                        {
                            var parametro = db.parametros.Where(p => p.parametro.ToUpper() == "CANTIDADCEROSCOMPROBANTE").FirstOrDefault();
                            cantidadCeros = Convert.ToInt32(parametro.valor);

                            if (timbradoRangoModelo.Hasta > timbradoRangoModelo.Desde)
                            {
                                timbrados_rangos timbradoRango = new timbrados_rangos
                                {
                                    id_timbrado_tipo_rango = timbradoRangoModelo.IdTimbradoTipoRango,
                                    id_timbrado = timbradoRangoModelo.IdTimbrado,
                                    id_sucursal = timbradoRangoModelo.IdSucursal,
                                    codigo_sucursal = timbradoRangoModelo.CodigoSucursal,
                                    punto_expedicion = timbradoRangoModelo.PuntoExpedicion,
                                    cantidad_ceros = cantidadCeros,
                                    desde = timbradoRangoModelo.Desde,
                                    hasta = timbradoRangoModelo.Hasta,
                                    numeracion_actual = timbradoRangoModelo.Desde,
                                    utilizado = false,
                                    cantidad_usada = 0,
                                    estado = true
                                };
                                db.timbrados_rangos.Add(timbradoRango);
                                db.SaveChanges();
                            }
                            else
                            {
                                ModelState.AddModelError("RangoHastaMenor", "El rango hasta no puede ser menor o igual que el rango desde");
                                retornoVista = true;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("SinParametro", "Tiene que ingresar un parametro para la cantidad de ceros del comprobante");
                            retornoVista = true;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un rango registrado con el timbrado seleccionado");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el rango timbrado en la base de datos");
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
                CargarDatosTimbradoRango(timbradoRangoModelo);
                db.Dispose();
                return View(timbradoRangoModelo);
            }
        }

        #endregion

        #region Editar Timbrado Rango

        [HttpGet]
        [AutorizarUsuario("TimbradosRangos", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TimbradoRangoModel timbradoRangoEdit = new TimbradoRangoModel();
            try
            {
                var timbradoRango = db.timbrados_rangos.Where(tr => tr.id == id).FirstOrDefault();
                timbradoRangoEdit.Id = timbradoRango.id;
                timbradoRangoEdit.IdTimbradoFormato = timbradoRango.timbrados.id_timbrado_formato;
                timbradoRangoEdit.IdTimbradoTipoDocumento = timbradoRango.timbrados.id_timbrado_tipo_documento;
                timbradoRangoEdit.IdTimbradoTipoRango = timbradoRango.id_timbrado_tipo_rango;
                timbradoRangoEdit.IdTimbrado = timbradoRango.id_timbrado;
                timbradoRangoEdit.IdSucursal = timbradoRango.id_sucursal;
                timbradoRangoEdit.CodigoSucursal = timbradoRango.codigo_sucursal;
                timbradoRangoEdit.PuntoExpedicion = timbradoRango.punto_expedicion;
                timbradoRangoEdit.CantidadCeros = timbradoRango.cantidad_ceros;
                timbradoRangoEdit.Desde = timbradoRango.desde;
                timbradoRangoEdit.Hasta = timbradoRango.hasta;
                timbradoRangoEdit.NumeracionActual = timbradoRango.numeracion_actual;
                timbradoRangoEdit.Utilizado = timbradoRango.utilizado;
                timbradoRangoEdit.Estado = timbradoRango.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = timbradoRango.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                CargarDatosTimbradoRango(timbradoRangoEdit);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(timbradoRangoEdit);
        }

        [HttpPost]
        public ActionResult Edit(TimbradoRangoModel timbradoRangoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI EL RANGO A INGRESAR DEL TIMBRADO YA NO EXISTE
                    bool rangoHabilitado = true;
                    int countDesde = db.timbrados_rangos.Where(t => t.id_timbrado == timbradoRangoModelo.IdTimbrado && timbradoRangoModelo.Desde >= t.desde && timbradoRangoModelo.Desde <= t.hasta && t.estado != null && t.id != timbradoRangoModelo.Id).Count();
                    if (countDesde > 0) { rangoHabilitado = false; }
                    int countHasta = db.timbrados_rangos.Where(t => t.id_timbrado == timbradoRangoModelo.IdTimbrado && timbradoRangoModelo.Hasta >= t.desde && timbradoRangoModelo.Hasta <= t.hasta && t.estado != null && t.id != timbradoRangoModelo.Id).Count();
                    if (countHasta > 0) { rangoHabilitado = false; }

                    if (rangoHabilitado == true)
                    {
                        if (timbradoRangoModelo.Hasta > timbradoRangoModelo.Desde)
                        {
                            var timbradoRango = db.timbrados_rangos.Where(tr => tr.id == timbradoRangoModelo.Id).FirstOrDefault();
                            timbradoRango.id_timbrado_tipo_rango = timbradoRangoModelo.IdTimbradoTipoRango;
                            timbradoRango.id_timbrado = timbradoRangoModelo.IdTimbrado;
                            timbradoRango.id_sucursal = timbradoRangoModelo.IdSucursal;
                            timbradoRango.codigo_sucursal = timbradoRangoModelo.CodigoSucursal;
                            timbradoRango.punto_expedicion = timbradoRangoModelo.PuntoExpedicion;
                            timbradoRango.desde = timbradoRangoModelo.Desde;
                            timbradoRango.hasta = timbradoRangoModelo.Hasta;
                            bool nuevoEstado = timbradoRangoModelo.EstadoDescrip == "A" ? true : false;
                            timbradoRango.estado = nuevoEstado;
                            db.Entry(timbradoRango).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("RangoHastaMenor", "El rango hasta no puede ser menor o igual que el rango desde");
                            retornoVista = true;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un rango registrado con el timbrado seleccionado");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el rango timbrado en la base de datos");
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
                CargarDatosTimbradoRango(timbradoRangoModelo);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", timbradoRangoModelo.EstadoDescrip);
                db.Dispose();
                return View(timbradoRangoModelo);
            }
        }

        #endregion

        #region Eliminar Timbrado Rango

        [HttpPost]
        public ActionResult EliminarTimbradoRango(string timbradoRangoId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("TimbradosRangos", "EliminarTimbradoRango");
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
                                int idTimbradoRango = Convert.ToInt32(timbradoRangoId);
                                var timbradoRango = context.timbrados_rangos.Where(t => t.id == idTimbradoRango).FirstOrDefault();
                                timbradoRango.estado = null;
                                context.Entry(timbradoRango).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "TimbradosRangos") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Funciones

        private void CargarDatosTimbradoRango(TimbradoRangoModel timbradoRangoModelo)
        {
            ViewBag.IdTimbradoFormato = new SelectList(db.timbrados_formatos.Where(tf => tf.estado == true).OrderBy(tf => tf.nombre_formato).ToList(), "id", "nombre_formato", timbradoRangoModelo.IdTimbradoFormato);
            ViewBag.IdTimbradoTipoDocumento = new SelectList(db.timbrados_tipos_documentos.Where(ttd => ttd.estado == true).OrderBy(ttd => ttd.nombre_documento).ToList(), "id", "nombre_documento", timbradoRangoModelo.IdTimbradoTipoDocumento);
            ViewBag.IdTimbradoTipoRango = new SelectList(db.timbrados_rangos_tipos.Where(trt => trt.estado == true).OrderBy(trt => trt.tipo).ToList(), "id", "tipo", timbradoRangoModelo.IdTimbradoTipoRango);
            ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", timbradoRangoModelo.IdSucursal);
            if (timbradoRangoModelo.IdTimbradoFormato != null && timbradoRangoModelo.IdTimbradoTipoDocumento != null)
            {
                ViewBag.IdTimbrado = new SelectList(db.timbrados.Where(t => t.id_timbrado_formato == timbradoRangoModelo.IdTimbradoFormato && t.id_timbrado_tipo_documento == timbradoRangoModelo.IdTimbradoTipoDocumento && t.estado == true).OrderBy(t => t.nro_timbrado).ToList(), "id", "nro_timbrado", timbradoRangoModelo.IdTimbrado);
            }
            else
            {
                ViewBag.IdTimbrado = new SelectList(db.timbrados.Where(t => t.id == 0).OrderBy(t => t.nro_timbrado).ToList(), "id", "nro_timbrado");
            }
        }

        #endregion

        #region AJAX

        [HttpPost]
        public ActionResult ObtenerListadoTimbradosRangos(string sucursalId, string timbradoTipoRangoId, string timbradoTipoDocumentoId)
        {
            int idSucursal = sucursalId != "" ? Convert.ToInt32(sucursalId) : 0;
            int idTimbradoTipoRango = timbradoTipoRangoId != "" ? Convert.ToInt32(timbradoTipoRangoId) : 0;
            int idTimbradoTipoDocumento = timbradoTipoDocumentoId != "" ? Convert.ToInt32(timbradoTipoDocumentoId) : 0;
            TimbradoRangoModel timbradoRango = new TimbradoRangoModel();
            List<ListaDinamica> listaRangosDisponibles = timbradoRango.ListadoRangosDisponibles(idTimbradoTipoRango, idSucursal, idTimbradoTipoDocumento);
            var rangos = listaRangosDisponibles.OrderBy(f => f.Nombre).Select(f => "<option value='" + f.Id + "'>" + f.Nombre + "</option>");
            return Content(String.Join("", rangos));
        }

        [HttpPost]
        public ActionResult ObtenerListadoRangosTimbradosFuncionario(string funcionarioId, string timbradoFormatoId, string tipoRangoId, string timbradoTipoDocumentoId)
        {
            int idFuncionario = funcionarioId != "" ? Convert.ToInt32(funcionarioId) : 0;
            int idTimbradoFormato = timbradoFormatoId != "" ? Convert.ToInt32(timbradoFormatoId) : 0;
            int idTipoRango = tipoRangoId != "" ? Convert.ToInt32(tipoRangoId) : 0;
            int idTimbradoTipoDocumento = timbradoTipoDocumentoId != "" ? Convert.ToInt32(timbradoTipoDocumentoId) : 0;
            TimbradoRangoModel timbradoRango = new TimbradoRangoModel();
            List<ListaDinamica> listaRangosDisponibles = timbradoRango.ListadoRangosDisponiblesFuncionario(idFuncionario, idTimbradoFormato, idTipoRango, idTimbradoTipoDocumento);
            var rangos = listaRangosDisponibles.OrderBy(f => f.Nombre).Select(f => "<option value='" + f.Id + "'>" + f.Nombre + "</option>");
            return Content(String.Join("", rangos));
        }

        [HttpGet]
        public ActionResult ObtenerDatosTimbradoRango(string timbradoRangoId)
        {
            string respuesta = string.Empty;
            string idTimbrado = string.Empty;
            string nroTimbrado = string.Empty;
            string fechaInicio = string.Empty;
            string fechaFin = string.Empty;
            string nroDocumento = string.Empty;
            string validezTimbrado = string.Empty;
            string utilizado = string.Empty;
            try
            {
                int idTimbradoRango = Convert.ToInt32(timbradoRangoId);
                var timbradoRango = db.timbrados_rangos.Where(tr => tr.id == idTimbradoRango).FirstOrDefault();
                idTimbrado = timbradoRango.timbrados.id.ToString();
                nroTimbrado = timbradoRango.timbrados.nro_timbrado;
                fechaInicio = timbradoRango.timbrados.vigencia_desde.Value.ToShortDateString();
                fechaFin = timbradoRango.timbrados.vigencia_hasta.Value.ToShortDateString();
                nroDocumento = timbradoRango.codigo_sucursal + "-" + timbradoRango.punto_expedicion + "-";
                TimbradoNumeracion timbradoNumeracion = new TimbradoNumeracion();
                nroDocumento += timbradoNumeracion.ObtenerNumeracionFormato(timbradoRango.numeracion_actual.Value, timbradoRango.cantidad_ceros.Value);
                validezTimbrado = timbradoNumeracion.ObtenerValidezTimbrado(timbradoRango.timbrados.vigencia_hasta);
                utilizado = String.Format("{0:#,##0.##}", timbradoRango.cantidad_usada);
                respuesta = timbradoRango.timbrados_rangos_tipos.id == 1 ? "A" : "M";
            }
            catch (Exception)
            {
                respuesta = "Error";
            }
            return Json(new
            {
                succes = true,
                respuesta = respuesta,
                idTimbrado = idTimbrado,
                nroTimbrado = nroTimbrado,
                fechaInicio = fechaInicio,
                fechaFin = fechaFin,
                nroDocumento = nroDocumento,
                validezTimbrado = validezTimbrado,
                CantidadUtilizada = utilizado
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}