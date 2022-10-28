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
    public class FacturacionTemporalController : Controller
    {

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();
        private int IdSucursalUsuario;
        private int IdFuncionario;
        private int IdUsuario;

        #region Listado de Facturaciones Temporales

        [HttpGet]
        [AutorizarUsuario("FacturacionTemporal", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<FacturacionTemporalModel> listaFacturaciones = new List<FacturacionTemporalModel>();
            try
            {
                //CARGAMOS LOS DATOS DEL USUARIO EN SESION
                CargarDatosUsuarioSesion();

                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesFecha = Convert.ToString(Session["sesionFacTemporalFecha"]);
                ViewBag.txtFecha = sesFecha;
                string sesFechaHasta = Convert.ToString(Session["sesionFacTemporalFechaHasta"]);
                ViewBag.txtFechaHasta = sesFechaHasta;

                if (sesFecha != "" || sesFechaHasta != "")
                {
                    //OBTENEMOS TODAS LAS FACTURACIONES TEMPORALES DE LA BASE DE DATOS
                    var fact = from ft in db.facturaciones_temporales
                               where ft.estado != null
                               select new FacturacionTemporalModel
                               {
                                   Id = ft.id,
                                   IdCondicionVenta = ft.id_condicion_venta,
                                   FechaAlta = ft.fecha_alta,
                                   IdUsuario = ft.id_usuario,
                                   FechaEmision = ft.fecha_emision,
                                   IdCliente = ft.id_cliente,
                                   Ruc = ft.ruc,
                                   RazonSocial = ft.razon_social,
                                   Direccion = ft.direccion,
                                   Telefono = ft.telefono,
                                   IdTimbrado = ft.id_timbrado,
                                   IdTimbradoRango = ft.id_timbrado_rango,
                                   IdTimbradoTipoRango = ft.timbrados_rangos.id_timbrado_tipo_rango,
                                   NroFactura = ft.nro_factura,
                                   IdMotivoDescuento = ft.id_motivo_descuento,
                                   TotalDescuento = ft.total_descuento,
                                   TotalNeto = ft.total_neto,
                                   TotalBruto = ft.total_bruto,

                                   NombreCondicionVenta = ft.id_condicion_venta != null ? ft.condiciones_compras_ventas.tipo : "",
                                   NombreUsuario = ft.usuarios.id_funcionario != null ? ft.usuarios.funcionarios.nombre + " " + ft.usuarios.funcionarios.apellido : "",
                                   NombreMotivoDescuento = ft.id_motivo_descuento != null ? ft.motivos_descuentos.motivo : "",
                                   EstadoDescrip = ft.estado == true ? "Activo" : "Inactivo"
                               };
                    listaFacturaciones = fact.ToList();
                }

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesFecha != "")
                {
                    DateTime? fecha = null;
                    try
                    {
                        fecha = Convert.ToDateTime(sesFecha);
                    }
                    catch (Exception)
                    {
                        int startIndexDia = 2;
                        int lengthDia = 2;
                        int startIndexMes = 0;
                        int lengthMes = 2;
                        int startIndexAnho = 5;
                        int lengthAnho = 4;
                        String substringDia = sesFecha.Substring(startIndexDia, lengthDia);
                        String substringMes = sesFecha.Substring(startIndexMes, lengthMes);
                        String substringAño = sesFecha.Substring(startIndexAnho, lengthAnho);
                        string strFechaNueva = substringDia + "/" + substringMes + "/" + substringAño;
                        fecha = Convert.ToDateTime(strFechaNueva);
                    }
                    listaFacturaciones = listaFacturaciones.Where(f => f.FechaEmision >= fecha).ToList();
                }

                if (sesFechaHasta != "")
                {
                    DateTime? fechaHasta = null;
                    try
                    {
                        fechaHasta = Convert.ToDateTime(sesFechaHasta);
                    }
                    catch (Exception)
                    {
                        int startIndexDia = 2;
                        int lengthDia = 2;
                        int startIndexMes = 0;
                        int lengthMes = 2;
                        int startIndexAnho = 5;
                        int lengthAnho = 4;
                        String substringDia = sesFechaHasta.Substring(startIndexDia, lengthDia);
                        String substringMes = sesFechaHasta.Substring(startIndexMes, lengthMes);
                        String substringAño = sesFechaHasta.Substring(startIndexAnho, lengthAnho);
                        string strFechaNueva = substringDia + "/" + substringMes + "/" + substringAño;
                        fechaHasta = Convert.ToDateTime(strFechaNueva);
                    }
                    listaFacturaciones = listaFacturaciones.Where(f => f.FechaEmision <= fechaHasta).ToList();
                }

                listaFacturaciones = listaFacturaciones.OrderByDescending(f => f.FechaAlta).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de facturaciones temporales";
            }
            return View(listaFacturaciones.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<FacturacionTemporalModel> listaFacturaciones = new List<FacturacionTemporalModel>();
            try
            {
                //CARGAMOS LOS DATOS DEL USUARIO EN SESION
                CargarDatosUsuarioSesion();

                var fcFecha = fc["txtFecha"];
                var fcFechaHasta = fc["txtFechaHasta"];

                if (fcFecha != "" || fcFechaHasta != "")
                {
                    //OBTENEMOS TODAS LAS FACTURACIONES TEMPORALES DE LA BASE DE DATOS
                    var fact = from ft in db.facturaciones_temporales
                               where ft.estado != null
                               select new FacturacionTemporalModel
                               {
                                   Id = ft.id,
                                   IdCondicionVenta = ft.id_condicion_venta,
                                   FechaAlta = ft.fecha_alta,
                                   IdUsuario = ft.id_usuario,
                                   FechaEmision = ft.fecha_emision,
                                   IdCliente = ft.id_cliente,
                                   Ruc = ft.ruc,
                                   RazonSocial = ft.razon_social,
                                   Direccion = ft.direccion,
                                   Telefono = ft.telefono,
                                   IdTimbrado = ft.id_timbrado,
                                   IdTimbradoRango = ft.id_timbrado_rango,
                                   IdTimbradoTipoRango = ft.timbrados_rangos.id_timbrado_tipo_rango,
                                   NroFactura = ft.nro_factura,
                                   IdMotivoDescuento = ft.id_motivo_descuento,
                                   TotalDescuento = ft.total_descuento,
                                   TotalNeto = ft.total_neto,
                                   TotalBruto = ft.total_bruto,

                                   NombreCondicionVenta = ft.id_condicion_venta != null ? ft.condiciones_compras_ventas.tipo : "",
                                   NombreUsuario = ft.usuarios.id_funcionario != null ? ft.usuarios.funcionarios.nombre + " " + ft.usuarios.funcionarios.apellido : "",
                                   NombreMotivoDescuento = ft.id_motivo_descuento != null ? ft.motivos_descuentos.motivo : "",
                                   EstadoDescrip = ft.estado == true ? "Activo" : "Inactivo"
                               };
                    listaFacturaciones = fact.ToList();
                }

                //FILTRAMOS POR FECHA DESDE Y FECHA HASTA
                if (fcFecha != "")
                {
                    DateTime? fecha = null;
                    try
                    {
                        fecha = Convert.ToDateTime(fcFecha);
                    }
                    catch (Exception)
                    {
                        int startIndexDia = 2;
                        int lengthDia = 2;
                        int startIndexMes = 0;
                        int lengthMes = 2;
                        int startIndexAnho = 5;
                        int lengthAnho = 4;
                        String substringDia = fcFecha.Substring(startIndexDia, lengthDia);
                        String substringMes = fcFecha.Substring(startIndexMes, lengthMes);
                        String substringAño = fcFecha.Substring(startIndexAnho, lengthAnho);
                        string strFechaNueva = substringDia + "/" + substringMes + "/" + substringAño;
                        fecha = Convert.ToDateTime(strFechaNueva);
                    }
                    listaFacturaciones = listaFacturaciones.Where(f => f.FechaEmision >= fecha).ToList();
                }

                if (fcFechaHasta != "")
                {
                    DateTime? fechaHasta = null;
                    try
                    {
                        fechaHasta = Convert.ToDateTime(fcFechaHasta);
                    }
                    catch (Exception)
                    {
                        int startIndexDia = 2;
                        int lengthDia = 2;
                        int startIndexMes = 0;
                        int lengthMes = 2;
                        int startIndexAnho = 5;
                        int lengthAnho = 4;
                        String substringDia = fcFechaHasta.Substring(startIndexDia, lengthDia);
                        String substringMes = fcFechaHasta.Substring(startIndexMes, lengthMes);
                        String substringAño = fcFechaHasta.Substring(startIndexAnho, lengthAnho);
                        string strFechaNueva = substringDia + "/" + substringMes + "/" + substringAño;
                        fechaHasta = Convert.ToDateTime(strFechaNueva);
                    }
                    listaFacturaciones = listaFacturaciones.Where(f => f.FechaEmision <= fechaHasta).ToList();
                }

                listaFacturaciones = listaFacturaciones.OrderByDescending(f => f.FechaEmision).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtFecha = fcFecha;
                Session["sesionFacTemporalFecha"] = fcFecha;
                ViewBag.txtFechaHasta = fcFechaHasta;
                Session["sesionFacTemporalFechaHasta"] = fcFechaHasta;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar facturaciones temporales";
            }
            return View(listaFacturaciones.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Facturación Temporal

        [HttpGet]
        [AutorizarUsuario("FacturacionTemporal", "Create")]
        public ActionResult Create()
        {
            //OBTENEMOS TODOS LOS DATOS NECESARIOS PARA REALIZAR FACTURACIONES TEMPORALES
            FacturacionTemporalModel facturacion = new FacturacionTemporalModel();

            //CARGAMOS LOS DATOS DEL USUARIO EN SESION
            CargarDatosUsuarioSesion();

            facturacion.StrFechaEmision = DateTime.Now.ToShortDateString();
            facturacion.IdTimbradoFormato = 1; //PREIMPRESO
            facturacion.IdTimbradoTipoRango = 1; //AUTOMATICO
            facturacion.IdFuncionarioTimbrado = IdFuncionario;
            facturacion.IdCondicionVenta = 1; //CONTADO


            CargarDatosFacturacionTemporal(facturacion);
            ViewBag.ListaDetalleFacturacion = null;
            ViewBag.ListaDetallePagoFacturacion = null;

            return View(facturacion);
        }

        [HttpPost]
        public ActionResult Create(FacturacionTemporalModel facturacionModelo, FormCollection fc)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            bool imprimirFacturacion = false;
            int facturacionId = 0;
            bool habilitado = true;
            int idTipoDocumento = 1; //FACTURA
            string resultadoVerificacion = string.Empty;

            //OBTENEMOS TODOS LOS DETALLES DE FACTURACION TEMPORAL
            string[] arrCantidad = (fc["arrCantidad"] != null ? fc["arrCantidad"].Split(',') : new string[] { });
            string[] arrDescripcion = (fc["arrDescripcion"] != null ? fc["arrDescripcion"].Split(',') : new string[] { });
            string[] arrPrecioUnitario = (fc["arrPrecioUnitario"] != null ? fc["arrPrecioUnitario"].Split(',') : new string[] { });
            string[] arrSubTotal = (fc["arrSubTotal"] != null ? fc["arrSubTotal"].Split(',') : new string[] { });
            string[] arrExcenta = (fc["arrExcenta"] != null ? fc["arrExcenta"].Split(',') : new string[] { });
            string[] arrIva5 = (fc["arrIva5"] != null ? fc["arrIva5"].Split(',') : new string[] { });
            string[] arrIva10 = (fc["arrIva10"] != null ? fc["arrIva10"].Split(',') : new string[] { });

            //OBTENEMOS TODOS LOS DETALLES DE PAGOS DE LA FACTURA TEMPORAL
            string[] arrIdTipoPago = (fc["arrIdTipoPago"] != null ? fc["arrIdTipoPago"].Split(',') : new string[] { });
            string[] arrNombreTipoPago = (fc["arrNombreTipoPago"] != null ? fc["arrNombreTipoPago"].Split(',') : new string[] { });
            string[] arrMontoPagado = (fc["arrMontoPagado"] != null ? fc["arrMontoPagado"].Split(',') : new string[] { });
            string[] arrObservacion = (fc["arrObservacion"] != null ? fc["arrObservacion"].Split(',') : new string[] { });

            if (ModelState.IsValid) //SI EL MODELO ES VALIDO
            {
                try
                {
                    CargarDatosUsuarioSesion(); //CARGAMOS LOS DATOS DEL USUARIO EN SESION

                    //INICIA LA TRANSACCIÓN DE FACTURACIONES CONSIGNACIONES Y DETALLES
                    using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                DateTime fechaEmision = DateTime.Now;
                                string strDescuento = Convert.ToString(facturacionModelo.StrDescuento);
                                decimal descuento = 0;
                                if (strDescuento != "" && strDescuento != null)
                                {
                                    strDescuento = strDescuento.Replace(".", "");
                                    descuento = Convert.ToDecimal(strDescuento);
                                }

                                string strTotalNeto = Convert.ToString(facturacionModelo.StrTotalNeto);
                                decimal totalNeto = 0;
                                if (strTotalNeto != "" && strTotalNeto != null)
                                {
                                    strTotalNeto = strTotalNeto.Replace(".", "");
                                    totalNeto = Convert.ToDecimal(strTotalNeto);
                                }

                                string strTotalBruto = Convert.ToString(facturacionModelo.StrTotalBruto);
                                decimal totalBruto = 0;
                                if (strTotalBruto != "" && strTotalBruto != null)
                                {
                                    strTotalBruto = strTotalBruto.Replace(".", "");
                                    totalBruto = Convert.ToDecimal(strTotalBruto);
                                }

                                //VERIFICAMOS SI ES TIMBRADO AUTOMATICO PARA HABILITAR LA IMPRESION DE LA FACTURA
                                if (facturacionModelo.IdTimbradoTipoRango == 1) //AUTOMATICO
                                {
                                    imprimirFacturacion = true;
                                }
                                else
                                {
                                    decimal numeroActual = Convert.ToInt32(facturacionModelo.NroFactura);
                                    TimbradoNumeracion timbradoNumeracion = new TimbradoNumeracion();
                                    var timbradoRango = context.timbrados_rangos.Where(tr => tr.id == facturacionModelo.IdTimbradoRango).FirstOrDefault();
                                    facturacionModelo.NroFactura = timbradoRango.codigo_sucursal + "-" + timbradoRango.punto_expedicion + "-";
                                    facturacionModelo.NroFactura += timbradoNumeracion.ObtenerNumeracionFormato(numeroActual, timbradoRango.cantidad_ceros.Value);

                                    //VERIFICAR EL NRO DE FACTURA MANUAL PARA HABILITAR
                                    resultadoVerificacion = timbradoNumeracion.VerificarNroComprobante(context, idTipoDocumento, timbradoRango, numeroActual, facturacionModelo.NroFactura);
                                    if (resultadoVerificacion != string.Empty)
                                    {
                                        habilitado = false;
                                    }
                                }

                                if (habilitado == true)
                                {
                                    //AGREGAMOS LOS DATOS DE CABECERA DE LAS FACTURACIONES TEMPORALES
                                    facturaciones_temporales facturacion = new facturaciones_temporales
                                    {
                                        id_condicion_venta = facturacionModelo.IdCondicionVenta,
                                        fecha_alta = DateTime.Now,
                                        id_usuario = IdUsuario,
                                        fecha_emision = fechaEmision,
                                        ruc = facturacionModelo.Ruc,
                                        razon_social = facturacionModelo.RazonSocial,
                                        direccion = facturacionModelo.Direccion,
                                        telefono = facturacionModelo.Telefono,
                                        id_timbrado = facturacionModelo.IdTimbrado,
                                        id_timbrado_rango = facturacionModelo.IdTimbradoRango,
                                        nro_factura = facturacionModelo.NroFactura,
                                        id_motivo_descuento = facturacionModelo.IdMotivoDescuento,
                                        total_descuento = descuento,
                                        total_neto = totalNeto,
                                        total_bruto = totalBruto,
                                        anulado = false,
                                        estado = true
                                    };
                                    context.facturaciones_temporales.Add(facturacion);
                                    context.SaveChanges();
                                    facturacionId = facturacion.id;

                                    //AGREGAMOS LOS DETALLES DE LAS FACTURACIONES TEMPORALES
                                    for (int i = 0; i < arrCantidad.Length; i++)
                                    {
                                        string strCantidad = Convert.ToString(arrCantidad[i]);
                                        strCantidad = strCantidad.Replace(".", "");
                                        decimal cantidad = Convert.ToDecimal(strCantidad);

                                        string strPrecioUnitario = Convert.ToString(arrPrecioUnitario[i]);
                                        strPrecioUnitario = strPrecioUnitario.Replace(".", "");
                                        decimal precioUnitario = Convert.ToDecimal(strPrecioUnitario);

                                        string strIvaExcenta = Convert.ToString(arrExcenta[i]);
                                        strIvaExcenta = strIvaExcenta.Replace(".", "");
                                        decimal ivaExcenta = Convert.ToDecimal(strIvaExcenta);

                                        string strIva5 = Convert.ToString(arrIva5[i]);
                                        strIva5 = strIva5.Replace(".", "");
                                        decimal iva5 = Convert.ToDecimal(strIva5);

                                        string strIva10 = Convert.ToString(arrIva10[i]);
                                        strIva10 = strIva10.Replace(".", "");
                                        decimal iva10 = Convert.ToDecimal(strIva10);

                                        decimal gravadaExcenta = 0;
                                        decimal gravadaIva5 = 0;
                                        decimal gravadaIva10 = iva10 / 11;
                                        //decimal gravadaIva10 = iva10;
                                        gravadaIva10 = Math.Round(gravadaIva10);

                                        facturaciones_temporales_detalles factDet = new facturaciones_temporales_detalles
                                        {
                                            id_facturacion_temporal = facturacion.id,
                                            cantidad = cantidad,
                                            descripcion = arrDescripcion[i],
                                            precio_unitario = precioUnitario,
                                            iva_excenta = ivaExcenta,
                                            iva_5 = iva5,
                                            iva_10 = iva10,
                                            gravada_excenta = gravadaExcenta,
                                            gravada_5 = gravadaIva5,
                                            gravada_10 = gravadaIva10,
                                            estado = true
                                        };
                                        context.facturaciones_temporales_detalles.Add(factDet);
                                        context.SaveChanges();
                                    }

                                    //AGREGAMOS LOS TIPOS DE PAGOS
                                    for (int i = 0; i < arrIdTipoPago.Length; i++)
                                    {
                                        string striIdTipoPago = Convert.ToString(arrIdTipoPago[i]);
                                        striIdTipoPago = striIdTipoPago.Trim();
                                        int idTipoPago = Convert.ToInt32(striIdTipoPago);

                                        string strMontoPagado = Convert.ToString(arrMontoPagado[i]);
                                        strMontoPagado = strMontoPagado.Replace(".", "");
                                        decimal montoPagado = Convert.ToDecimal(strMontoPagado);

                                        facturaciones_temporales_pagos factPagos = new facturaciones_temporales_pagos
                                        {
                                            id_factura_temporal = facturacion.id,
                                            id_tipo_pago = idTipoPago,
                                            monto_pagado = montoPagado,
                                            observacion = arrObservacion[i],
                                            estado = true
                                        };
                                        context.facturaciones_temporales_pagos.Add(factPagos);
                                        context.SaveChanges();

                                    }

                                    //ACTUALIZAMOS LA NUMERACIÓN DEL RANGO UTILIZADO
                                    var timbradoRangoActualizar = context.timbrados_rangos.Where(tr => tr.id == facturacionModelo.IdTimbradoRango).FirstOrDefault();
                                    timbradoRangoActualizar.utilizado = true;
                                    timbradoRangoActualizar.cantidad_usada += 1;
                                    if (imprimirFacturacion == true) //RANGO AUTOMATICO
                                    {
                                        timbradoRangoActualizar.numeracion_actual += 1;
                                    }
                                    context.Entry(timbradoRangoActualizar).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }
                                else
                                {
                                    ModelState.AddModelError("Error", "El número de comprobante ingresado " + resultadoVerificacion);
                                    retornoVista = true;
                                }

                                if (habilitado == true)
                                {
                                    dbContextTransaction.Commit();
                                }
                                else
                                {
                                    dbContextTransaction.Rollback();
                                }
                            }
                            catch (Exception)
                            {
                                dbContextTransaction.Rollback();
                                ModelState.AddModelError("Error", "Ocurrio un error al procesar los datos de facturación");
                                retornoVista = true;
                            }
                        }
                        context.Database.Connection.Close();
                    }
                    if (imprimirFacturacion == true) //IMPRIMIMOS LA FACTURACION SI ES NECESARIO
                    {
                        ImprimirComprobante(facturacionId);
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la facturación en la base de datos");
                    retornoVista = true;
                }
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                ViewBag.msg = null;
                retornoVista = true;
            }
            if (retornoVista == false)
            {
                if (imprimirFacturacion == false)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Reporte");
                }
            }
            else
            {
                CargarDatosFacturacionTemporal(facturacionModelo);
                CargarDatosDetallesCompraPremios(arrCantidad, arrDescripcion, arrPrecioUnitario, arrSubTotal, arrExcenta, arrIva5, arrIva10);
                CargarDatosDetallesPagos(arrIdTipoPago, arrNombreTipoPago, arrMontoPagado, arrObservacion);
                db.Dispose();
                return View(facturacionModelo);
            }
        }

        #endregion

        #region Anular Facturación Temporal

        [HttpGet]
        [AutorizarUsuario("FacturacionTemporal", "Anular")]
        public ActionResult Anular(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FacturacionTemporalAnulacionModel factTempAnul = new FacturacionTemporalAnulacionModel();
            try
            {
                //OBTENEMOS EL REGISTRO DE LA FACTURACIÓN PARA REALIZAR LA ANULACIÓN DEL COMPROBANTE
                var facturacion = db.facturaciones_temporales.Where(f => f.id == id && f.estado != null).FirstOrDefault();

                //CARGAMOS LOS DATOS NECESARIOS DE FACTURACION AL MODELO
                factTempAnul.IdFacturacion = facturacion.id;
                factTempAnul.IdTimbradoRango = facturacion.id_timbrado_rango.Value;
                factTempAnul.StrFechaEmision = facturacion.fecha_emision.Value.ToShortDateString();
                factTempAnul.NombreCondicionVenta = facturacion.condiciones_compras_ventas.tipo;
                factTempAnul.Ruc = facturacion.ruc;
                factTempAnul.RazonSocial = facturacion.razon_social;
                factTempAnul.NroFactura = facturacion.nro_factura;
                factTempAnul.StrTotalNeto = String.Format("{0:#,##0.##}", facturacion.total_neto);
                ViewBag.IdMotivoAnulacion = new SelectList(db.timbrados_motivos_anulaciones.Where(tma => tma.estado == true).OrderBy(tma => tma.motivo).ToList(), "id", "motivo");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(factTempAnul);
        }

        [HttpPost]
        public ActionResult Anular(FacturacionTemporalAnulacionModel anulacionModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid) //SI EL MODELO ES VALIDO
            {
                try
                {
                    CargarDatosUsuarioSesion(); //CARGAMOS LOS DATOS DEL USUARIO EN SESION
                    //INICIA LA TRANSACCIÓN DE ANULACION DE FACTURACION
                    using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                //OBTENEMOS EL REGISTRO PARA PODER ANULAR (ACTUALIZAR ESTADO A NULL)
                                var facturacion = context.facturaciones_temporales.Where(f => f.id == anulacionModelo.IdFacturacion && f.estado != null).FirstOrDefault();
                                facturacion.anulado = true;
                                facturacion.estado = null;
                                context.Entry(facturacion).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();

                                //OBTNEMOS LA LISTA DETALLE DE FACTURACION
                                var listaFactDet = context.facturaciones_temporales_detalles.Where(fd => fd.id_facturacion_temporal == facturacion.id && fd.estado != null).ToList();

                                listaFactDet.ForEach(lfcd => lfcd.estado = null);
                                context.SaveChanges();

                                //REGISTRAMOS EL MOTIVO DE LA ANULACION EN LA BASE DE DATOS
                                timbrados_comprobantes_anulaciones timComAnul = new timbrados_comprobantes_anulaciones
                                {
                                    fecha = DateTime.Now,
                                    id_usuario = IdUsuario,
                                    id_motivo_anulacion = anulacionModelo.IdMotivoAnulacion,
                                    id_timbrado_rango = anulacionModelo.IdTimbradoRango,
                                    nro_comprobante = facturacion.nro_factura,
                                    observacion = anulacionModelo.Observacion,
                                    estado = true
                                };
                                context.timbrados_comprobantes_anulaciones.Add(timComAnul);
                                context.SaveChanges();

                                //COMPRUEBA SI ESTA RELACCIONADO CON LA CONSUMISION EN RESTAURANTE DE PRODUCTOS PARA PODER ACTUALIZAR STOCK
                                //var consumisionesDetalles = context.consumisiones_detalles.Where(cd => cd.id_facturacion == facturacion.id && cd.estado != null).ToList();
                                //int idConsumision = 0;
                                //if(consumisionesDetalles.Count > 0)
                                //{
                                //    var detalle = context.consumisiones_detalles.Where(cd => cd.id_facturacion == facturacion.id && cd.estado != null).FirstOrDefault();
                                //    idConsumision = detalle.id_consumision.Value;

                                //    var consumision = context.consumisiones.Where(c => c.id == idConsumision).FirstOrDefault();

                                //    //DISMINUIMOS LA CANTIDAD DE CADA PRODUCTO Y PRODUCTO LOTE
                                //    foreach (var item in consumisionesDetalles)
                                //    {
                                //        var producto = context.productos.Where(p => p.id == item.id_producto).FirstOrDefault();
                                //        producto.stock += item.cantidad;
                                //        context.Entry(producto).State = System.Data.Entity.EntityState.Modified;
                                //        context.SaveChanges();

                                //        var productoLote = context.productos_lotes.Where(pl => pl.id == item.id_producto_lote).FirstOrDefault();
                                //        productoLote.cantidad += item.cantidad;
                                //        context.Entry(productoLote).State = System.Data.Entity.EntityState.Modified;
                                //        context.SaveChanges();
                                //    }
                                //    consumision.estado = null;
                                //    context.Entry(consumision).State = System.Data.Entity.EntityState.Modified;
                                //    context.SaveChanges();

                                //    consumisionesDetalles.ForEach(cd => cd.estado = null);
                                //    context.SaveChanges();
                                //}

                                dbContextTransaction.Commit();
                            }
                            catch (Exception)
                            {
                                dbContextTransaction.Rollback();
                                ModelState.AddModelError("Error", "Hubo un problema al procesar la anulación de facturación temporal");
                                retornoVista = true;
                            }
                        }
                        context.Database.Connection.Close();
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al anular la facturación temporal");
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
                ViewBag.IdMotivoAnulacion = new SelectList(db.timbrados_motivos_anulaciones.Where(tma => tma.estado == true).OrderBy(tma => tma.motivo).ToList(), "id", "motivo", anulacionModelo.IdMotivoAnulacion);
                return View(anulacionModelo);
            }
        }

        #endregion

        #region Reimprimir Factura

        [HttpGet]
        [AutorizarUsuario("FacturacionTemporal", "ReimprimirFactura")]
        public ActionResult ReimprimirFactura(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                ImprimirComprobante(id.Value);
                return RedirectToAction("Reporte");

            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        #endregion

        #region Funciones

        /*
        * CARGAMOS LOS DATOS SI ES FUNCIONARIO O NO Y TAMBIEN LA SUCURSAL DONDE CORRESPONDE EL USUARIO SESION
        */
        private void CargarDatosUsuarioSesion()
        {
            IdUsuario = Convert.ToInt32(Session["IdUser"]);
            var usuario = db.usuarios.Where(u => u.id == IdUsuario).FirstOrDefault();
            IdFuncionario = usuario.id_funcionario != null ? usuario.funcionarios.id : 0;
            ViewBag.EsFuncionario = usuario.id_funcionario != null ? true : false;
            ViewBag.IdSucursalUsuarioLog = usuario.id_funcionario != null ? usuario.funcionarios.id_sucursal : null;
            IdSucursalUsuario = usuario.id_funcionario != null ? usuario.funcionarios.id_sucursal.Value : 0;
        }

        private void CargarDatosFacturacionTemporal(FacturacionTemporalModel facturacionModelo)
        {
            CargarDatosUsuarioSesion();

            //OBTENEMOS LOS DATOS DE FUNCIONARIOS PARA REALIZAR FACTURACION
            TimbradoRangoFuncionarioModel timRanFun = new TimbradoRangoFuncionarioModel();
            List<ListaDinamica> funcionariosTimbrados = timRanFun.ListadoFuncionariosTimbrados(IdSucursalUsuario);
            ViewBag.IdFuncionarioTimbrado = new SelectList(funcionariosTimbrados, "Id", "Nombre", IdFuncionario);

            List<ListaDinamica> rangosDisponiblesFuncionario = timRanFun.ListadoRangosDisponiblesFuncionarios(facturacionModelo.IdTimbradoTipoRango != null ? facturacionModelo.IdTimbradoTipoRango : 0, IdSucursalUsuario, 1, facturacionModelo.IdFuncionarioTimbrado != null ? facturacionModelo.IdFuncionarioTimbrado : 0);
            ViewBag.IdTimbradoRango = new SelectList(rangosDisponiblesFuncionario, "Id", "Nombre", facturacionModelo.IdTimbradoRango);

            ViewBag.IdTimbradoFormato = new SelectList(db.timbrados_formatos.Where(tf => tf.estado == true).OrderBy(tf => tf.nombre_formato).ToList(), "id", "nombre_formato", facturacionModelo.IdTimbradoFormato);
            ViewBag.IdTimbradoTipoRango = new SelectList(db.timbrados_rangos_tipos.Where(trt => trt.estado == true).OrderBy(trt => trt.tipo).ToList(), "id", "tipo", facturacionModelo.IdTimbradoTipoRango);

            //CARGAMOS LA LISTA DE MOTIVO DESCUENTO
            ViewBag.IdMotivoDescuento = new SelectList(db.motivos_descuentos.Where(md => md.estado == true).OrderBy(md => md.motivo).ToList(), "id", "motivo", facturacionModelo.IdMotivoDescuento);
            //CARGAMOS LA LISTA DE CONDICION DE VENTA
            ViewBag.IdCondicionVenta = new SelectList(db.condiciones_compras_ventas.Where(ccv => ccv.estado == true).OrderBy(ccv => ccv.tipo).ToList(), "id", "tipo", facturacionModelo.IdCondicionVenta);
            //CARGAMOS LA LISTA DE TIPOS PAGOS
            ViewBag.IdTipoPago = new SelectList(db.tipos_pagos.Where(tp => tp.estado == true).OrderBy(tp => tp.tipo).ToList(), "id", "tipo", facturacionModelo.IdTipoPago);
        }

        private void CargarDatosDetallesCompraPremios(string[] arrCantidad, string[] arrDescripcion, string[] arrPrecioUnitario, string[] arrSubTotal, string[] arrExcenta, string[] arrIva5, string[] arrIva10)
        {
            List<DetalleFacturaTemporalModel> listaDetalles = new List<DetalleFacturaTemporalModel>();
            for (int i = 0; i < arrCantidad.Length; i++)
            {
                DetalleFacturaTemporalModel carga = new DetalleFacturaTemporalModel();
                carga.Cantidad = Convert.ToString(arrCantidad[i]);
                carga.Descripcion = Convert.ToString(arrDescripcion[i]);
                carga.PrecioUnitario = Convert.ToString(arrPrecioUnitario[i]);
                carga.SubTotal = Convert.ToString(arrSubTotal[i]);
                carga.Excenta = Convert.ToString(arrExcenta[i]);
                carga.Iva5 = Convert.ToString(arrIva5[i]);
                carga.Iva10 = Convert.ToString(arrIva10[i]);

                listaDetalles.Add(carga);
            }
            ViewBag.ListaDetalleFacturacion = listaDetalles.Count > 0 ? listaDetalles : null;
        }

        private void CargarDatosDetallesPagos(string[] arrIdTipoPago, string[] arrNombreTipoPago, string[] arrMontoPagado, string[] arrObservacion)
        {
            List<DetallePagoFacturaModel> listaDetalles = new List<DetallePagoFacturaModel>();
            for (int i = 0; i < arrIdTipoPago.Length; i++)
            {
                DetallePagoFacturaModel carga = new DetallePagoFacturaModel();
                carga.IdTipoPago = Convert.ToString(arrIdTipoPago[i]);
                carga.NombreTipoPago = Convert.ToString(arrNombreTipoPago[i]);
                carga.MontoPago = Convert.ToString(arrMontoPagado[i]);
                carga.Observacion = Convert.ToString(arrObservacion[i]);

                listaDetalles.Add(carga);
            }
            ViewBag.ListaDetallePagoFacturacion = listaDetalles.Count > 0 ? listaDetalles : null;
        }

        private void ImprimirComprobante(int idFact)
        {
            //CREAMOS EL OBJETO DATASET DE LA FACTURACION
            WebHoteleria.Reportes.Dataset.DataSetFacturaTemporal dts = new WebHoteleria.Reportes.Dataset.DataSetFacturaTemporal();
            var dt1 = new WebHoteleria.Reportes.Dataset.DataSetFacturaTemporal.DTFacturaTemporalDataTable();
            var dt2 = new WebHoteleria.Reportes.Dataset.DataSetFacturaTemporal.DTFacturaTemporalDetalleDataTable();

            //OBTENEMOS LOS DATOS DE LA FACTURA TEMPORAL Y DETALLE
            var factura = db.facturaciones_temporales.Where(f => f.id == idFact).FirstOrDefault();
            var facturaDet = db.facturaciones_temporales_detalles.Where(fd => fd.id_facturacion_temporal == idFact).ToList();

            //AGREGAMOS EN EL DATATABLE LOS DATOS DE LA CABECERA DE LA FACTURACION TEMPORAL
            var row1 = dt1.NewDTFacturaTemporalRow();
            ComprobanteDocumentoModel comprobante = new ComprobanteDocumentoModel();
            ComprobanteDocumentoModel resultado = comprobante.ObtenerDatosComprobante();
            row1.RazonSocialEmpresa = resultado.RazonSocialEmpresa;
            row1.RucEmpresa = resultado.RucEmpresa;
            row1.ComprobanteLinea1 = resultado.ComprobanteLinea1;
            row1.ComprobanteLinea2 = resultado.ComprobanteLinea2;
            row1.ComprobanteLinea3 = resultado.ComprobanteLinea3;
            row1.ComprobanteLinea4 = resultado.ComprobanteLinea4;
            row1.ComprobanteLinea5 = resultado.ComprobanteLinea5;
            row1.ComprobanteLinea6 = resultado.ComprobanteLinea6;
            row1.NroTimbrado = factura.timbrados.nro_timbrado;
            row1.InicioTimbrado = factura.timbrados.vigencia_desde.Value.ToShortDateString();
            row1.FinTimbrado = factura.timbrados.vigencia_hasta.Value.ToShortDateString();
            row1.NroFactura = factura.nro_factura;
            row1.FechaEmision = factura.fecha_emision.Value.ToShortDateString();
            row1.RucCliente = factura.ruc;
            row1.RazonSocialCliente = factura.razon_social;
            row1.DireccionCliente = factura.direccion;
            row1.TelefonoCliente = factura.telefono;
            string facturaContado = string.Empty;
            string facturaCredito = string.Empty;
            if (factura.id_condicion_venta == 1) //CONTADO
            {
                facturaContado = "Contado";
            }
            else //CREDITO
            {
                facturaCredito = "Credito";
            }
            row1.CondicionVentaContado = facturaContado;
            row1.CondicionVentaCredito = facturaCredito;
            row1.TotalPagar = String.Format("{0:#,##0.##}", factura.total_neto);

            decimal totalExcenta = 0;
            decimal totalIva5 = 0;
            decimal totalIva10 = 0;

            //AGREGAMOS EN EL DATATABLE LOS DETALLES DE LA FACTURA AL REPORTE
            var contadorFilas = 0;
            foreach (var item in facturaDet)
            {
                decimal excenta = item.iva_excenta != null ? item.iva_excenta.Value : 0;
                decimal iva5 = item.iva_5 != null ? item.iva_5.Value : 0;
                decimal iva10 = item.iva_10 != null ? item.iva_10.Value : 0;

                totalExcenta += excenta;
                totalIva5 += iva5;
                totalIva10 += iva10;

                var row2 = dt2.NewDTFacturaTemporalDetalleRow();
                row2.cantidad = String.Format("{0:#,##0.##}", item.cantidad);
                row2.descripcion = item.descripcion;
                row2.precioUnitario = String.Format("{0:#,##0.##}", item.precio_unitario);
                row2.subTotal = String.Format("{0:#,##0.##}", item.precio_unitario * item.cantidad);
                row2.excenta = String.Format("{0:#,##0.##}", excenta);
                row2.iva5 = String.Format("{0:#,##0.##}", iva5);
                row2.iva10 = String.Format("{0:#,##0.##}", iva10);
                dt2.Rows.Add(row2);

                contadorFilas++;
            }

            if (contadorFilas < 23)
            {
                int diferencia = 23 - contadorFilas;
                for (int i = 0; i < diferencia; i++)
                {
                    var row2 = dt2.NewDTFacturaTemporalDetalleRow();
                    row2.cantidad = string.Empty;
                    row2.descripcion = string.Empty;
                    row2.precioUnitario = string.Empty;
                    row2.subTotal = string.Empty;
                    row2.excenta = string.Empty;
                    row2.iva5 = string.Empty;
                    row2.iva10 = string.Empty;
                    dt2.Rows.Add(row2);
                }
            }

            row1.TotalExecenta = String.Format("{0:#,##0.##}", totalExcenta);
            row1.TotalIva5 = String.Format("{0:#,##0.##}", totalIva5);
            row1.TotalIva10 = String.Format("{0:#,##0.##}", totalIva10);
            decimal montoTotalIva = (totalIva5 + totalIva10) / 11;
            montoTotalIva = Math.Round(montoTotalIva);
            row1.MontoTotalIva = String.Format("{0:#,##0.##}", montoTotalIva);
            Conversion conversion = new Conversion();
            string montoEnLetras = "Guaraníes ";
            string numEnviar = factura.total_neto.ToString();
            numEnviar = numEnviar.Replace(".", "");
            montoEnLetras += conversion.ConvertirNumerosEnletras(numEnviar);
            row1.MontoEnLetras = montoEnLetras;
            dt1.Rows.Add(row1);

            dts.Tables["DTFacturaTemporal"].Merge(dt1);
            dts.Tables["DTFacturaTemporalDetalle"].Merge(dt2);

            WebHoteleria.Class.ReportesParametros paramReport = new WebHoteleria.Class.ReportesParametros();
            paramReport.ReportPath = Server.MapPath("~/Reportes/RPT/RptFacturaTemporal.rpt");
            paramReport.ReportSource = (object)dts;
            paramReport.NombreArchivo = "Factura Temporal";
            WebHoteleria.Class.ReportParameters.DatosReporte = paramReport;
        }

        #endregion

        #region AJAX

        [HttpPost]
        public ActionResult ObtenerDetalleFacturaTemporal(string idFacturaTemporal)
        {
            bool control = true;
            string msg = string.Empty;
            List<facturaciones_temporales_detalles> listado = new List<facturaciones_temporales_detalles>();
            var tabla = string.Empty;
            int id = Convert.ToInt32(idFacturaTemporal);
            try
            {
                listado = (from ft in db.facturaciones_temporales_detalles
                           where ft.id_facturacion_temporal == id && ft.estado == true
                           select ft).ToList();

                var tbl = "tbl_" + id;
                var row = "";
                if (listado.Count > 0)
                {
                    foreach (var datosListado in listado)
                    {
                        row += "                   <tr>";
                        row += "                       <td>" + String.Format("{0:#,##0.##}", datosListado.cantidad) + "</td>";
                        row += "                       <td>" + datosListado.descripcion + "</td>";
                        row += "                       <td>" + String.Format("{0:#,##0.##}", datosListado.precio_unitario) + "</td>    ";
                        row += "                       <td>" + String.Format("{0:#,##0.##}", datosListado.cantidad * datosListado.precio_unitario) + "</td>";
                        row += "                   </tr> ";
                    }
                }
                else
                {
                    row += "                   <tr>";
                    row += "                       <td> No se encuentran los detalles de la facturación.</td>";
                    row += "                   </tr> ";
                }

                tabla = " <table class=\"table table-striped jambo_table\" id=\"" + tbl + "\">";
                tabla += " <thead>";
                tabla += "               <tr>";
                tabla += "                   <th>Cantidad</th>";
                tabla += "                   <th>Descripción</th>";
                tabla += "                   <th>Precio</th>";
                tabla += "                   <th>Sub Total</th>";
                tabla += "               </tr>";
                tabla += "           </thead>";
                tabla += "           <tbody >     ";
                tabla += row;
                tabla += "           </tbody>";
                tabla += "       </table>";
            }
            catch (Exception)
            {
                control = false;
                msg = "Hubo un error al consultar los detalles de la facturación.";
            }
            return Json(new { success = control, msg = msg, listaDetalle = tabla }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Imprimir Reporte Facturación

        [HttpGet]
        public ActionResult Reporte()
        {
            return View();
        }

        #endregion

    }
}