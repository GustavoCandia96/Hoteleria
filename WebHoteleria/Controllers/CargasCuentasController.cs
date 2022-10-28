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
    public class CargasCuentasController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();
        private int IdSucursalUsuario;
        private int IdFuncionario;
        private int IdUsuario;

        #endregion

        #region Listado de Cargas Cuentas

        [HttpGet]
        [AutorizarUsuario("CargasCuentas", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CargaCuentaModel> listaCargasCuentas = new List<CargaCuentaModel>();
            try
            {
                //CARGAMOS LOS DATOS DEL USUARIO EN SESION
                CargarDatosUsuarioSesion();

                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesFecha = Convert.ToString(Session["sesionCargasCuentasFecha"]);
                ViewBag.txtFecha = sesFecha;
                string sesFechaHasta = Convert.ToString(Session["sesionCargasCuentasFechaHasta"]);
                ViewBag.txtFechaHasta = sesFechaHasta;

                if (sesFecha != "" || sesFechaHasta != "")
                {
                    //OBTENEMOS TODAS LAS CARGAS CUENTAS NO ANULADAS DE LA BASE DE DATOS
                    var cargaCuenta = from cc in db.cargas_cuentas
                                      where cc.estado != null
                                      select new CargaCuentaModel
                                      {
                                          Id = cc.id,
                                          IdConsumision = cc.id_consumision,
                                          FechaAlta = cc.fecha_alta,
                                          Fecha = cc.fecha,
                                          IdUsuario = cc.id_usuario,
                                          TotalConsumision = cc.total_consumision,
                                          Observacion = cc.observacion,
                                          Anulado = cc.anulado,
                                          Estado = cc.estado,
                                          EstadoDescrip = cc.estado == true ? "Activo" : "Inactivo"
                                      };
                    listaCargasCuentas = cargaCuenta.ToList();
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
                    listaCargasCuentas = listaCargasCuentas.Where(lcc => lcc.Fecha >= fecha).ToList();
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
                    listaCargasCuentas = listaCargasCuentas.Where(lcc => lcc.Fecha <= fechaHasta).ToList();
                }

                listaCargasCuentas = listaCargasCuentas.OrderByDescending(lcc => lcc.FechaAlta).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de cargas a cuentas";
            }
            return View(listaCargasCuentas.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CargaCuentaModel> listaCargasCuentas = new List<CargaCuentaModel>();
            try
            {
                //CARGAMOS LOS DATOS DEL USUARIO EN SESION
                CargarDatosUsuarioSesion();

                var fcFecha = fc["txtFecha"];
                var fcFechaHasta = fc["txtFechaHasta"];

                if (fcFecha != "" || fcFechaHasta != "")
                {
                    //OBTENEMOS TODAS LAS CARGAS CUENTAS NO ANULADAS DE LA BASE DE DATOS
                    var cargaCuenta = from cc in db.cargas_cuentas
                                      where cc.estado != null
                                      select new CargaCuentaModel
                                      {
                                          Id = cc.id,
                                          IdConsumision = cc.id_consumision,
                                          FechaAlta = cc.fecha_alta,
                                          Fecha = cc.fecha,
                                          IdUsuario = cc.id_usuario,
                                          TotalConsumision = cc.total_consumision,
                                          Observacion = cc.observacion,
                                          Anulado = cc.anulado,
                                          Estado = cc.estado,
                                          EstadoDescrip = cc.estado == true ? "Activo" : "Inactivo"
                                      };
                    listaCargasCuentas = cargaCuenta.ToList();
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
                    listaCargasCuentas = listaCargasCuentas.Where(lcc => lcc.Fecha >= fecha).ToList();
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
                    listaCargasCuentas = listaCargasCuentas.Where(lcc => lcc.Fecha <= fechaHasta).ToList();
                }

                listaCargasCuentas = listaCargasCuentas.OrderByDescending(lcc => lcc.FechaAlta).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtFecha = fcFecha;
                Session["sesionCargasCuentasFecha"] = fcFecha;
                ViewBag.txtFechaHasta = fcFechaHasta;
                Session["sesionCargasCuentasFechaHasta"] = fcFechaHasta;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar cargas a cuentas";
            }
            return View(listaCargasCuentas.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Anular Carga a Cuenta

        [HttpGet]
        [AutorizarUsuario("CargasCuentas", "Anular")]
        public ActionResult Anular(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CargaCuentaAnulacionModel cargaCuentaAnul = new CargaCuentaAnulacionModel();
            try
            {
                //OBTENEMOS EL REGISTRO DE CARGA DE CUENTA
                var cargaCuenta = db.cargas_cuentas.Where(cc => cc.id == id && cc.estado != null).FirstOrDefault();

                //CARGAMOS LOS DATOS NECESARIOS AL MODELO
                cargaCuentaAnul.IdCargaCuenta = cargaCuenta.id;
                cargaCuentaAnul.StrFechaEmision = cargaCuenta.fecha_alta.Value.ToShortDateString();
                cargaCuentaAnul.Descripcion = cargaCuenta.observacion;
                cargaCuentaAnul.StrTotalNeto = String.Format("{0:#,##0.##}", cargaCuenta.total_consumision);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(cargaCuentaAnul);
        }

        [HttpPost]
        public ActionResult Anular(CargaCuentaAnulacionModel anulacionModelo)
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
                                var cargaCuenta = context.cargas_cuentas.Where(cc => cc.id == anulacionModelo.IdCargaCuenta && cc.estado != null).FirstOrDefault();
                                cargaCuenta.anulado = true;
                                cargaCuenta.estado = null;
                                context.Entry(cargaCuenta).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();

                                //OBTNEMOS LA LISTA DETALLE DE CARGAS CUENTAS
                                var listaCarDet = context.cargas_cuentas_detalles.Where(ccd => ccd.id_carga_cuenta == cargaCuenta.id && ccd.estado != null).ToList();

                                listaCarDet.ForEach(lfcd => lfcd.estado = null);
                                context.SaveChanges();

                                //REGISTRAMOS EL MOTIVO DE LA ANULACION DE CARGA A CUENTAS
                                cargas_cuentas_anulaciones carCueAnul = new cargas_cuentas_anulaciones
                                {
                                    fecha_alta = DateTime.Now,
                                    id_usuario = IdUsuario,
                                    id_carga_cuenta = anulacionModelo.IdCargaCuenta,
                                    motivo_anulacion = anulacionModelo.Observacion,
                                    estado = true
                                };
                                context.cargas_cuentas_anulaciones.Add(carCueAnul);
                                context.SaveChanges();

                                //COMPRUEBA SI ESTA RELACCIONADO CON LA CONSUMISION EN RESTAURANTE DE PRODUCTOS PARA PODER ACTUALIZAR STOCK
                                var consumision = context.consumisiones.Where(c => c.id == cargaCuenta.id_consumision).FirstOrDefault();
                                var consumisionesDetalles = context.consumisiones_detalles.Where(cd => cd.id_consumision == consumision.id && cd.estado != null).ToList();

                                if (consumisionesDetalles.Count > 0)
                                {
                                    //DISMINUIMOS LA CANTIDAD DE CADA PRODUCTO Y PRODUCTO LOTE
                                    foreach (var item in consumisionesDetalles)
                                    {
                                        var producto = context.productos.Where(p => p.id == item.id_producto).FirstOrDefault();
                                        producto.stock += item.cantidad;
                                        context.Entry(producto).State = System.Data.Entity.EntityState.Modified;
                                        context.SaveChanges();

                                        var productoLote = context.productos_lotes.Where(pl => pl.id == item.id_producto_lote).FirstOrDefault();
                                        productoLote.cantidad += item.cantidad;
                                        context.Entry(productoLote).State = System.Data.Entity.EntityState.Modified;
                                        context.SaveChanges();
                                    }
                                    consumision.estado = null;
                                    context.Entry(consumision).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();

                                    consumisionesDetalles.ForEach(cd => cd.estado = null);
                                    context.SaveChanges();
                                }

                                dbContextTransaction.Commit();
                            }
                            catch (Exception)
                            {
                                dbContextTransaction.Rollback();
                                ModelState.AddModelError("Error", "Hubo un problema al procesar la anulación de carga de cuenta");
                                retornoVista = true;
                            }
                        }
                        context.Database.Connection.Close();
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al anular la carga de cuenta");
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
                return View(anulacionModelo);
            }
        }

        #endregion

        #region Reimprimir Carga Cuenta

        [HttpGet]
        [AutorizarUsuario("CargasCuentas", "ReimprimirCargaCuenta")]
        public ActionResult ReimprimirCargaCuenta(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                ImprimirCargaCuenta(id.Value);
                return RedirectToAction("Reporte");

            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        #endregion

        #region Funciones

        private void CargarDatosUsuarioSesion()
        {
            IdUsuario = Convert.ToInt32(Session["IdUser"]);
            var usuario = db.usuarios.Where(u => u.id == IdUsuario).FirstOrDefault();
            IdFuncionario = usuario.id_funcionario != null ? usuario.funcionarios.id : 0;
            ViewBag.EsFuncionario = usuario.id_funcionario != null ? true : false;
            ViewBag.IdSucursalUsuarioLog = usuario.id_funcionario != null ? usuario.funcionarios.id_sucursal : null;
            IdSucursalUsuario = usuario.id_funcionario != null ? usuario.funcionarios.id_sucursal.Value : 0;
        }

        private void ImprimirCargaCuenta(int idCargaCuenta)
        {
            //CREAMOS EL OBJETO DATASET DE CARGA DE CUENTA
            WebHoteleria.Reportes.Dataset.DataSetCargaCuenta dts = new WebHoteleria.Reportes.Dataset.DataSetCargaCuenta();
            var dt1 = new WebHoteleria.Reportes.Dataset.DataSetCargaCuenta.DTCargaCuentaDataTable();
            var dt2 = new WebHoteleria.Reportes.Dataset.DataSetCargaCuenta.DTCargaCuentaDetalleDataTable();

            //OBTENEMOS LOS DATOS DE LA CARGA DE CUENTA
            var cargaCuenta = db.cargas_cuentas.Where(cc => cc.id == idCargaCuenta).FirstOrDefault();
            var cargaCuentaDetalle = db.cargas_cuentas_detalles.Where(cc => cc.id_carga_cuenta == idCargaCuenta && cc.estado != null).ToList();

            //AGREGAMOS EN EL DATATABLE LOS DATOS DE LA CABECERA DE CARGA CUENTA
            var row1 = dt1.NewDTCargaCuentaRow();
            row1.NroMesa = cargaCuenta.consumisiones.mesas.denominacion;
            row1.Fecha = cargaCuenta.fecha_alta.ToString();
            row1.Observacion = cargaCuenta.observacion;
            row1.TotalConsumision = String.Format("{0:#,##0.##}", cargaCuenta.total_consumision);
            dt1.Rows.Add(row1);

            //AGREGAMOS EN EL DATATABLE LOS DETALLES DE LA CARGA DE CUENTA
            foreach (var item in cargaCuentaDetalle)
            {
                var row2 = dt2.NewDTCargaCuentaDetalleRow();
                row2.Cantidad = String.Format("{0:#,##0.##}", item.cantidad);
                row2.Descripcion = item.productos.nombre_producto;
                row2.PrecioUnitario = String.Format("{0:#,##0.##}", item.precio_unitario);
                row2.SubTotal = String.Format("{0:#,##0.##}", item.precio_unitario * item.cantidad);
                dt2.Rows.Add(row2);
            }

            dts.Tables["DTCargaCuenta"].Merge(dt1);
            dts.Tables["DTCargaCuentaDetalle"].Merge(dt2);

            WebHoteleria.Class.ReportesParametros paramReport = new WebHoteleria.Class.ReportesParametros();
            paramReport.ReportPath = Server.MapPath("~/Reportes/RPT/RptCargaCuenta.rpt");
            paramReport.ReportSource = (object)dts;
            paramReport.NombreArchivo = "Carga Cuenta" + cargaCuenta.id.ToString();
            WebHoteleria.Class.ReportParameters.DatosReporte = paramReport;
        }

        #endregion

        #region AJAX

        [HttpPost]
        public ActionResult ObtenerDetalleCargaCuenta(string idCargaCuenta)
        {
            bool control = true;
            string msg = string.Empty;
            List<cargas_cuentas_detalles> listado = new List<cargas_cuentas_detalles>();
            var tabla = string.Empty;
            int id = Convert.ToInt32(idCargaCuenta);
            try
            {
                listado = (from ccd in db.cargas_cuentas_detalles
                           where ccd.id_carga_cuenta == id && ccd.estado == true
                           select ccd).ToList();

                var tbl = "tbl_" + id;
                var row = "";
                if (listado.Count > 0)
                {
                    foreach (var datosListado in listado)
                    {
                        row += "                   <tr>";
                        row += "                       <td>" + String.Format("{0:#,##0.##}", datosListado.cantidad) + "</td>";
                        row += "                       <td>" + datosListado.productos.nombre_producto + "</td>";
                        row += "                       <td>" + String.Format("{0:#,##0.##}", datosListado.precio_unitario) + "</td>    ";
                        row += "                       <td>" + String.Format("{0:#,##0.##}", datosListado.cantidad * datosListado.precio_unitario) + "</td>";
                        row += "                   </tr> ";
                    }
                }
                else
                {
                    row += "                   <tr>";
                    row += "                       <td> No se encuentran los detalles de la carga a cuenta.</td>";
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
                msg = "Hubo un error al consultar los detalles de carga a cuenta.";
            }
            return Json(new { success = control, msg = msg, listaDetalle = tabla }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Imprimir Reporte Carga Cuenta

        [HttpGet]
        public ActionResult Reporte()
        {
            return View();
        }

        #endregion


    }
}