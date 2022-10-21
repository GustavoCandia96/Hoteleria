using EntidadesHoteleria;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebHoteleria.Class;
using WebHoteleria.Models;

namespace WebHoteleria.Controllers
{
    public class ComprasProductosController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Compras Productos

        [HttpGet]
        [AutorizarUsuario("ComprasProductos", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CompraProductoModel> listaCompraProductos = new List<CompraProductoModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesFecha = Convert.ToString(Session["sesionComprasProductosFecha"]);
                ViewBag.txtFecha = sesFecha;

                //OBTENEMOS TODAS LAS COMPRAS DE PRODUCTOS NO ELIMINADAS
                var comprasProductos = from cp in db.compras_productos
                                       where cp.estado != null
                                       select new CompraProductoModel
                                       {
                                           Id = cp.id,
                                           IdCondicionCompra = cp.id_condicion_compra,
                                           Fecha = cp.fecha,
                                           IdUsuario = cp.id_usuario,
                                           IdProveedor = cp.id_proveedor,
                                           IdProveedorSucursal = cp.id_proveedor_sucursal,
                                           NroFactura = cp.nro_factura,
                                           Descuento = cp.descuento,
                                           TotalNeto = cp.total_neto,
                                           TotalBruto = cp.total_bruto,
                                           SaldoFactura = cp.saldo_factura,
                                           TotalGravadaIva = cp.total_bruto,
                                           Observacion = cp.observarcion,
                                           Estado = cp.estado.Value,
                                           NombreCondicionCompra = cp.condiciones_compras_ventas.tipo,
                                           NombreUsuario = cp.id_usuario != null ? cp.usuarios.funcionarios.nombre + " " + cp.usuarios.funcionarios.apellido : "",
                                           NombreProveedor = cp.id_proveedor != null ? (cp.proveedores.id_tipo_documento == 1 ? cp.proveedores.nombre + " " + cp.proveedores.apellido : cp.proveedores.razon_social) : "",
                                           NombreProveedorSucursal = cp.id_proveedor_sucursal != null ? cp.proveedores_sucursales.nombre_sucursal : "",
                                           EstadoDescrip = cp.estado == true ? "Activo" : "Inactivo"
                                       };
                listaCompraProductos = comprasProductos.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesFecha != "")
                {
                    DateTime fecha = Convert.ToDateTime(sesFecha);
                    listaCompraProductos = listaCompraProductos.Where(cp => cp.Fecha >= fecha).ToList();
                }

                listaCompraProductos = listaCompraProductos.OrderByDescending(cp => cp.Fecha).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de compras productos";
            }
            return View(listaCompraProductos.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CompraProductoModel> listaCompraProductos = new List<CompraProductoModel>();
            try
            {
                //OBTENEMOS TODAS LAS COMPRAS DE PRODUCTOS NO ELIMINADAS
                var comprasProductos = from cp in db.compras_productos
                                       where cp.estado != null
                                       select new CompraProductoModel
                                       {
                                           Id = cp.id,
                                           IdCondicionCompra = cp.id_condicion_compra,
                                           Fecha = cp.fecha,
                                           IdUsuario = cp.id_usuario,
                                           IdProveedor = cp.id_proveedor,
                                           IdProveedorSucursal = cp.id_proveedor_sucursal,
                                           NroFactura = cp.nro_factura,
                                           Descuento = cp.descuento,
                                           TotalNeto = cp.total_neto,
                                           TotalBruto = cp.total_bruto,
                                           SaldoFactura = cp.saldo_factura,
                                           TotalGravadaIva = cp.total_bruto,
                                           Observacion = cp.observarcion,
                                           Estado = cp.estado.Value,
                                           NombreCondicionCompra = cp.condiciones_compras_ventas.tipo,
                                           NombreUsuario = cp.id_usuario != null ? cp.usuarios.funcionarios.nombre + " " + cp.usuarios.funcionarios.apellido : "",
                                           NombreProveedor = cp.id_proveedor != null ? (cp.proveedores.id_tipo_documento == 1 ? cp.proveedores.nombre + " " + cp.proveedores.apellido : cp.proveedores.razon_social) : "",
                                           NombreProveedorSucursal = cp.id_proveedor_sucursal != null ? cp.proveedores_sucursales.nombre_sucursal : "",
                                           EstadoDescrip = cp.estado == true ? "Activo" : "Inactivo"
                                       };
                listaCompraProductos = comprasProductos.ToList();

                //FILTRAMOS POR FECHA LA BUSQUEDA
                var fcFecha = fc["txtFecha"];
                if (fcFecha != "")
                {
                    DateTime fecha = Convert.ToDateTime(fcFecha);
                    listaCompraProductos = listaCompraProductos.Where(cp => cp.Fecha >= fecha).ToList();
                }


                listaCompraProductos = listaCompraProductos.OrderByDescending(cp => cp.Fecha).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtFecha = fcFecha;
                Session["sesionComprasProductosFecha"] = fcFecha;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar las compras de productos";
            }
            return View(listaCompraProductos.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Compra Productos

        [HttpGet]
        [AutorizarUsuario("ComprasProductos", "Create")]
        public ActionResult Create()
        {
            CompraProductoModel compraProductoModelo = new CompraProductoModel();
            CargarDatosCompraProductos(compraProductoModelo);
            ViewBag.ListaDetalleCompra = null;
            return View(compraProductoModelo);
        }

        [HttpPost]
        public ActionResult Create(CompraProductoModel compraProductoModelo, FormCollection fc)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            //OBTENEMOS TODOS LOS DETALLES DE COMPRAS PREMIOS
            #region Detalle Compra Producto
            string[] arrIdProducto = (fc["arrIdProducto"] != null ? fc["arrIdProducto"].Split(',') : new string[] { });
            string[] arrProducto = (fc["arrProducto"] != null ? fc["arrProducto"].Split(',') : new string[] { });
            string[] arrCantidad = (fc["arrCantidad"] != null ? fc["arrCantidad"].Split(',') : new string[] { });
            string[] arrPrecio = (fc["arrPrecio"] != null ? fc["arrPrecio"].Split(',') : new string[] { });
            string[] arrSubTotal = (fc["arrSubTotal"] != null ? fc["arrSubTotal"].Split(',') : new string[] { });
            string[] arrGravadaExcenta = (fc["arrGravadaExcenta"] != null ? fc["arrGravadaExcenta"].Split(',') : new string[] { });
            string[] arrGravada5 = (fc["arrGravada5"] != null ? fc["arrGravada5"].Split(',') : new string[] { });
            string[] arrGravada10 = (fc["arrGravada10"] != null ? fc["arrGravada10"].Split(',') : new string[] { });
            string[] arrSucursal = (fc["arrSucursal"] != null ? fc["arrSucursal"].Split(',') : new string[] { });
            string[] arrDeposito = (fc["arrDeposito"] != null ? fc["arrDeposito"].Split(',') : new string[] { });
            string[] arrIdSucursal = (fc["arrIdSucursal"] != null ? fc["arrIdSucursal"].Split(',') : new string[] { });
            string[] arrIdDeposito = (fc["arrIdDeposito"] != null ? fc["arrIdDeposito"].Split(',') : new string[] { });
            string[] arrIdIvaProducto = (fc["arrIdIvaProducto"] != null ? fc["arrIdIvaProducto"].Split(',') : new string[] { });
            #endregion

            if (ModelState.IsValid) //SI EL MODELO ES VALIDO
            {
                try
                {
                    //OBTENEMOS AL USUARIO EN SESSION
                    int IdUsuario = Convert.ToInt32(Session["IdUser"]);

                    //VERIFICAMOS SI INGRESAMOS DETALLES DE COMPRAS PRODUCTOS
                    if (arrIdProducto.Length > 0)
                    {
                        //DECLARAMOS LISTAS CON EL FORMATO CORRECTO A CADA CAMPO DEL DETALLE
                        //CONVERTIMOS LOS DATOS OBTENIDOS DESDE LA VISTA
                        List<CompraProductoDetalleModel> detalleLista = new List<CompraProductoDetalleModel>();
                        for (int i = 0; i < arrIdProducto.Length; i++)
                        {
                            string strIdProducto = Convert.ToString(arrIdProducto[i]); strIdProducto = strIdProducto.Replace(".", "");
                            string strCantidad = Convert.ToString(arrCantidad[i]); strCantidad = strCantidad.Replace(".", "");
                            string strPrecio = Convert.ToString(arrPrecio[i]); strPrecio = strPrecio.Replace(".", "");
                            string strGravadaExcenta = Convert.ToString(arrGravadaExcenta[i]); strGravadaExcenta = strGravadaExcenta.Replace(".", "");
                            string strGravada5 = Convert.ToString(arrGravada5[i]); strGravada5 = strGravada5.Replace(".", "");
                            string strGravada10 = Convert.ToString(arrGravada10[i]); strGravada10 = strGravada10.Replace(".", "");
                            string strIdSucursal = Convert.ToString(arrIdSucursal[i]); strIdSucursal = strIdSucursal.Replace(".", "");
                            string strIdDeposito = Convert.ToString(arrIdDeposito[i]); strIdDeposito = strIdDeposito.Replace(".", "");

                            int idProducto = Convert.ToInt32(strIdProducto);
                            decimal cantidad = Convert.ToDecimal(strCantidad);
                            decimal precioUnitario = Convert.ToDecimal(strPrecio);
                            decimal gravadaExcenta = Convert.ToDecimal(strGravadaExcenta);
                            decimal gravada5 = Convert.ToDecimal(strGravada5);
                            decimal gravada10 = Convert.ToDecimal(strGravada10);
                            int idSucursal = Convert.ToInt32(strIdSucursal);
                            int idDeposito = Convert.ToInt32(strIdDeposito);

                            CompraProductoDetalleModel carga = new CompraProductoDetalleModel();
                            carga.IdProducto = idProducto;
                            carga.Cantidad = cantidad;
                            carga.PrecioUnitario = precioUnitario;
                            carga.GravadaExcenta = gravadaExcenta;
                            carga.Gravada5 = gravada5;
                            carga.Gravada10 = gravada10;
                            carga.IdSucursal = idSucursal;
                            carga.IdDeposito = idDeposito;
                            detalleLista.Add(carga);
                        }

                        //INICIA LA TRANSACCIÓN DE COMPRAS DE PRODUCTOS Y DETALLES
                        using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                        {
                            using (var dbContextTransaction = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    bool operacionExitosa = true;

                                    DateTime? fechaCompra = Convert.ToDateTime(compraProductoModelo.StrFecha);
                                    string strDescuento = compraProductoModelo.StrDescuento;
                                    decimal descuentoCompra = 0;
                                    if (strDescuento != "" && strDescuento != null)
                                    {
                                        descuentoCompra = Convert.ToDecimal(strDescuento);
                                    }
                                    decimal totalNeto = Convert.ToDecimal(compraProductoModelo.StrTotalNeto);
                                    decimal totalBruto = Convert.ToDecimal(compraProductoModelo.StrTotalBruto);
                                    decimal totalGravada = Convert.ToDecimal(compraProductoModelo.StrTotalGravada);

                                    //AGREGAMOS EL REGISTRO DE CABECERA DE LA COMPRA DE PRODUCTOS
                                    compras_productos compraProductos = new compras_productos
                                    {
                                        id_condicion_compra = compraProductoModelo.IdCondicionCompra,
                                        fecha = fechaCompra,
                                        id_usuario = IdUsuario,
                                        id_proveedor = compraProductoModelo.IdProveedor,
                                        id_proveedor_sucursal = compraProductoModelo.IdProveedorSucursal,
                                        nro_factura = compraProductoModelo.NroFactura,
                                        descuento = descuentoCompra,
                                        total_neto = totalNeto,
                                        total_bruto = totalBruto,
                                        saldo_factura = compraProductoModelo.IdCondicionCompra == 1 ? 0 : totalNeto,
                                        total_gravada_iva = totalGravada,
                                        observarcion = compraProductoModelo.Observacion,
                                        estado = true
                                    };
                                    context.compras_productos.Add(compraProductos);
                                    context.SaveChanges();

                                    //AGREGAMOS EL DETALLE DE COMPRAS PRODUCTOS Y PRODUCTOS LOTES
                                    foreach (var item in detalleLista)
                                    {
                                        compras_productos_detalles cpd = new compras_productos_detalles
                                        {
                                            id_sucursal = item.IdSucursal,
                                            id_deposito = item.IdDeposito,
                                            id_compra_producto = compraProductos.id,
                                            id_producto = item.IdProducto,
                                            cantidad = item.Cantidad,
                                            precio_unitario = item.PrecioUnitario,
                                            gravada_excenta = item.GravadaExcenta,
                                            gravada_10 = item.Gravada10,
                                            gravada_5 = item.Gravada5,
                                            estado = true
                                        };
                                        context.compras_productos_detalles.Add(cpd);
                                        context.SaveChanges();

                                        //AGREGAMOS O ACTUALIZAMOS LA CANTIDAD DE PRODUCTO LOTE EN LA BASE DE DATOS
                                        var productoLote = context.productos_lotes.Where(p => p.id_producto == item.IdProducto && p.id_sucursal == item.IdSucursal && item.IdDeposito == item.IdDeposito && p.estado != null).FirstOrDefault();

                                        if (productoLote == null) //AGREGAMOS PRODUCTO LOTE
                                        {
                                            productos_lotes pl = new productos_lotes
                                            {
                                                id_producto = item.IdProducto,
                                                cantidad = item.Cantidad,
                                                id_sucursal = cpd.id_sucursal,
                                                id_deposito = item.IdDeposito,
                                                ubicacion = null,
                                                estado = true
                                            };
                                            context.productos_lotes.Add(pl);
                                            context.SaveChanges();
                                        }
                                        else //ACTUALIZAMOS PRODUCTO LOTE
                                        {
                                            productoLote.cantidad += item.Cantidad;
                                            context.Entry(productoLote).State = System.Data.Entity.EntityState.Modified;
                                            context.SaveChanges();
                                        }

                                        //ACTUALIZAMOS EL STOCK DE PRODUCTOS Y OBTENEMOS EL PROMEDIO DE COSTO
                                        var producto = context.productos.Where(p => p.id == item.IdProducto).FirstOrDefault();
                                        producto.stock += item.Cantidad;
                                        ProductoModel productoModel = new ProductoModel();
                                        decimal promedio = productoModel.CalcularPromedioCostoProducto(context, item.IdProducto);
                                        producto.precio_costo = promedio;
                                        context.Entry(producto).State = System.Data.Entity.EntityState.Modified;
                                        context.SaveChanges();
                                    }

                                    if (compraProductoModelo.IdCondicionCompra == 2)
                                    {
                                        cuentas_proveedores cuentProv = new cuentas_proveedores
                                        {
                                            id_proveedor = compraProductoModelo.IdProveedor,
                                            id_proveedor_sucursal = compraProductoModelo.IdProveedorSucursal,
                                            id_compra_producto = compraProductos.id,
                                            deuda = compraProductos.total_neto,
                                            saldo = compraProductos.total_neto,
                                            saldo_modificado = false,
                                            estado = true
                                        };
                                        context.cuentas_proveedores.Add(cuentProv);
                                        context.SaveChanges();
                                    }

                                    //SI TODA LA OPERACION FUE EXITOSA EJECUTAMOS LA TRANSACCIÓN
                                    if (operacionExitosa == true)
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
                                }
                            }
                            context.Database.Connection.Close();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("SinDetalle", "Tiene que ingresar el detalle de la compra de productos");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la compra de productos en la base de datos");
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
                //OBTENEMOS TODOS LOS DATOS DE COMPRAS PRODUCTOS PARA DEVOLVER A LA VISTA
                CargarDatosCompraProductos(compraProductoModelo);
                CargarDatosDetallesCompraProductos(arrIdProducto, arrProducto, arrCantidad, arrPrecio, arrSubTotal, arrGravadaExcenta, arrGravada5, arrGravada10, arrSucursal, arrDeposito, arrIdSucursal, arrIdDeposito, arrIdIvaProducto);
                db.Dispose();
                return View(compraProductoModelo);
            }
        }

        #endregion
        
        #region Anular Compra Productos

        [HttpPost]
        public ActionResult AnularCompraProducto(string compraProductoId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("ComprasProductos", "AnularCompraProducto");
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
                                int idCompraProducto = Convert.ToInt32(compraProductoId);

                                var cuentaProveedor = context.cuentas_proveedores.Where(cp => cp.id_compra_producto == idCompraProducto && cp.saldo_modificado == false && cp.estado != null).FirstOrDefault();

                                if (cuentaProveedor != null)
                                {
                                    //OBTENEMOS LOS DATOS DE LA COMPRA CABECERA LOS DETALLES DE LA COMPRA Y LOS PRODUCTOS LOTES
                                    var compraProducto = context.compras_productos.Where(cp => cp.id == idCompraProducto).FirstOrDefault();
                                    var compraProductoDetalles = context.compras_productos_detalles.Where(cpd => cpd.id_compra_producto == compraProducto.id && cpd.estado != null).ToList();

                                    foreach (var item in compraProductoDetalles)
                                    {
                                        //ACTUALIZAMOS EL STOCK DE PRODUCTOS LOTES
                                        var productoLote = context.productos_lotes.Where(pl => pl.id_producto == item.id_producto && item.id_sucursal == item.id_sucursal && item.id_deposito == item.id_deposito && pl.estado != null).FirstOrDefault();
                                        productoLote.cantidad = item.cantidad;
                                        context.Entry(productoLote).State = System.Data.Entity.EntityState.Modified;
                                        context.SaveChanges();

                                        var compraDetalle = context.compras_productos_detalles.Where(cpd => cpd.id == item.id && cpd.estado != null).FirstOrDefault();
                                        compraDetalle.estado = null;
                                        context.Entry(compraDetalle).State = System.Data.Entity.EntityState.Modified;
                                        context.SaveChanges();

                                        //CALCULAMOS EL PROMEDIO
                                        ProductoModel productoModel = new ProductoModel();
                                        decimal promedio = productoModel.CalcularPromedioCostoProducto(context, item.id_producto.Value);

                                        //ACTUALIZAMOS EL STOCK DE PRODUCTOS
                                        var producto = context.productos.Where(p => p.id == productoLote.id_producto).FirstOrDefault();
                                        producto.stock -= item.cantidad;
                                        producto.precio_costo = promedio;
                                        context.Entry(producto).State = System.Data.Entity.EntityState.Modified;
                                        context.SaveChanges();
                                    }

                                    compraProducto.estado = null;
                                    context.Entry(compraProducto).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();

                                    cuentaProveedor.estado = null;
                                    context.Entry(cuentaProveedor).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();


                                }
                                else
                                {
                                    retorno = false;
                                    respuesta = "Esta compra ya no se puede modificar debido a que ya esta relacionado a una cobranza";
                                }

                                if (retorno == true)
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "ComprasProductos") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Funciones

        private void CargarDatosCompraProductos(CompraProductoModel compraProductoModelo)
        {
            //CARGAMOS EL LISTADO DE CONDICIONES COMPRAS
            ViewBag.IdCondicionCompra = new SelectList(db.condiciones_compras_ventas.Where(ccv => ccv.estado == true).OrderBy(ccv => ccv.tipo).ToList(), "id", "tipo", compraProductoModelo.IdCondicionCompra != null ? compraProductoModelo.IdCondicionCompra : 0);
            //OBTENEMOS EL LISTADO DE PROVEEDORES Y SUCURSALES PROVEEDORES
            ProveedorModel proveedorModelo = new ProveedorModel();
            List<ListaDinamica> listaProveedores = proveedorModelo.ListadoProveedores();
            ViewBag.IdProveedor = new SelectList(listaProveedores, "id", "nombre", compraProductoModelo.IdProveedor != null ? compraProductoModelo.IdProveedor : 0);
            int idProveedor = compraProductoModelo.IdProveedor != null ? compraProductoModelo.IdProveedor.Value : 0;
            ProveedorSucursalModel proveedorSucursalModelo = new ProveedorSucursalModel();
            List<ListaDinamica> listaProveedoresSucursales = proveedorSucursalModelo.ListadoProveedoresSucursales(idProveedor);
            ViewBag.IdProveedorSucursal = new SelectList(listaProveedoresSucursales, "id", "nombre", compraProductoModelo.IdProveedorSucursal != null ? compraProductoModelo.IdProveedorSucursal : 0);
            //OBTIENE EL LISTADO DE SUCURSALES DEPOSITOS Y PERECEDERO
            ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", compraProductoModelo.IdSucursal != null ? compraProductoModelo.IdSucursal : 0);
            int idSucursal = compraProductoModelo.IdSucursal != null ? compraProductoModelo.IdSucursal.Value : 0;
            ViewBag.IdDeposito = new SelectList(db.depositos.Where(d => d.estado == true && d.id_sucursal == idSucursal).OrderBy(d => d.nombre_deposito).ToList(), "id", "nombre_deposito", compraProductoModelo.IdDeposito != null ? compraProductoModelo.IdDeposito.Value : 0);
        }

        private void CargarDatosDetallesCompraProductos(string[] arrIdProducto, string[] arrProducto, string[] arrCantidad, string[] arrPrecio, string[] arrSubTotal, string[] arrGravadaExcenta, string[] arrGravada5, string[] arrGravada10, string[] arrSucursal, string[] arrDeposito, string[] arrIdSucursal, string[] arrIdDeposito, string[] arrIdIvaProducto)
        {
            List<DetalleCompraModel> listaDetallesCompra = new List<DetalleCompraModel>();
            for (int i = 0; i < arrIdProducto.Length; i++)
            {
                DetalleCompraModel carga = new DetalleCompraModel();
                carga.IdProducto = Convert.ToString(arrIdProducto[i]);
                carga.Producto = Convert.ToString(arrProducto[i]);
                carga.Cantidad = Convert.ToString(arrCantidad[i]);
                carga.Precio = Convert.ToString(arrPrecio[i]);
                carga.SubTotal = Convert.ToString(arrSubTotal[i]);
                carga.GravadaExcenta = Convert.ToString(arrGravadaExcenta[i]);
                carga.Gravada10 = Convert.ToString(arrGravada10[i]);
                carga.Gravada5 = Convert.ToString(arrGravada5[i]);
                carga.Sucursal = Convert.ToString(arrSucursal[i]);
                carga.Deposito = Convert.ToString(arrDeposito[i]);
                carga.IdSucursal = Convert.ToString(arrIdSucursal[i]);
                carga.IdDeposito = Convert.ToString(arrIdDeposito[i]);
                carga.IdIvaProducto = Convert.ToString(arrIdIvaProducto[i]);

                listaDetallesCompra.Add(carga);
            }
            ViewBag.ListaDetalleCompra = listaDetallesCompra.Count > 0 ? listaDetallesCompra : null;
        }

        #endregion

        #region AJAX

        [HttpPost]
        public ActionResult ObtenerListadoDetalleComprasProductos(string compraProductoId)
        {
            bool control = true;
            string msg = string.Empty;
            List<compras_productos_detalles> listado = new List<compras_productos_detalles>();
            var tabla = string.Empty;
            int idCompraProducto = Convert.ToInt32(compraProductoId);
            try
            {
                listado = (from cpd in db.compras_productos_detalles
                           where cpd.id_compra_producto == idCompraProducto && cpd.estado == true
                           select cpd).ToList();

                var tbl = "tbl_" + idCompraProducto;
                var row = "";
                if (listado.Count > 0)
                {
                    foreach (var datosListado in listado)
                    {
                        row += "                   <tr> ";
                        row += "                       <td>" + datosListado.productos.nombre_producto + "</td>";
                        row += "                       <td>" + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("es-ES"), "{0:#,##0.##}", datosListado.precio_unitario) + "</td>    ";
                        row += "                       <td>" + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("es-ES"), "{0:#,##0.##}", datosListado.cantidad) + "</td>";
                        decimal subTotal = datosListado.precio_unitario.Value * datosListado.cantidad.Value;
                        row += "                       <td>" + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("es-ES"), "{0:#,##0.##}", subTotal) + "</td>";
                        row += "                   </tr> ";
                    }
                }
                else
                {
                    row += "                   <tr>";
                    row += "                       <td> No se encuentran detalles de productos.</td>";
                    row += "                   </tr> ";
                }

                tabla = " <table class=\"table table-striped jambo_table\" id=\"" + tbl + "\">";
                tabla += " <thead>";
                tabla += "               <tr>";
                tabla += "                   <th>Producto</th>";
                tabla += "                   <th>Precio U.</th>";
                tabla += "                   <th>Cantidad</th>";
                tabla += "                   <th>Sub Total</th>";
                tabla += "                   <th>Acciones</th>";
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
                msg = "Hubo un error al consultar el detalle de compras productos seleccionado.";
            }
            return Json(new { success = control, msg = msg, listaDetalle = tabla }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}