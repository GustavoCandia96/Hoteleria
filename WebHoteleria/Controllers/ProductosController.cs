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
    public class ProductosController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Productos

        [HttpGet]
        [AutorizarUsuario("Productos", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<ProductoModel> listaProductos = new List<ProductoModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomProduc = Convert.ToString(Session["sesionProductosNombre"]);
                ViewBag.txtProducto = sesNomProduc;
                string sesIdCategoria = Convert.ToString(Session["sesionProductosIdCategoria"]);
                ViewBag.ddlCategorias = new SelectList(db.categorias.Where(c => c.estado == true).OrderBy(c => c.nombre_categoria).ToList(), "id", "nombre_categoria", sesIdCategoria);

                //OBTENEMOS TODOS LOS PRODUCTOS ACTIVOS DE LA BASE DE DATOS
                var productos = from p in db.productos
                                where p.estado != null
                                select new ProductoModel
                                {
                                    Id = p.id,
                                    NombreProducto = p.nombre_producto,
                                    IdCategoria = p.id_categoria,
                                    IdMarca = p.id_marca,
                                    IdUnidad = p.id_unidad,
                                    PrecioVenta = p.precio_venta,
                                    PrecioCosto = p.precio_costo,
                                    Stock = p.stock,
                                    IdTipoIva = p.id_tipo_iva,
                                    Estado = p.estado.Value,
                                    NombreCategoria = p.id_categoria != null ? p.categorias.nombre_categoria : "",
                                    NombreMarca = p.id_marca != null ? p.marcas.nombre_marca : "",
                                    NombreUnidad = p.id_unidad != null ? p.unidades.nombre_unidad : "",
                                    NombreTipoIva = p.id_tipo_iva != null ? p.tipos_ivas.iva : "",
                                    ColorFila = p.productos_lotes.Count > 0 ? "background-color: transparent;" : "style=background-color:rgb(249,255,129);",
                                    EstadoDescrip = p.estado == true ? "Activo" : "Inactivo"
                                };
                listaProductos = productos.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomProduc != "")
                {
                    listaProductos = listaProductos.Where(p => p.NombreProducto.ToUpper().Contains(sesNomProduc.Trim().ToUpper())).ToList();
                }
                if (sesIdCategoria != "")
                {
                    int idCategoria = Convert.ToInt32(sesIdCategoria);
                    listaProductos = listaProductos.Where(p => p.IdCategoria == idCategoria).ToList();
                }
                listaProductos = listaProductos.OrderBy(p => p.NombreProducto).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de productos";
            }
            return View(listaProductos.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<ProductoModel> listaProductos = new List<ProductoModel>();
            try
            {
                //OBTENEMOS TODOS LOS PRODUCTOS ACTIVOS DE LA BASE DE DATOS
                var productos = from p in db.productos
                                where p.estado != null
                                select new ProductoModel
                                {
                                    Id = p.id,
                                    NombreProducto = p.nombre_producto,
                                    IdCategoria = p.id_categoria,
                                    IdMarca = p.id_marca,
                                    IdUnidad = p.id_unidad,
                                    PrecioVenta = p.precio_venta,
                                    PrecioCosto = p.precio_costo,
                                    Stock = p.stock,
                                    IdTipoIva = p.id_tipo_iva,
                                    Estado = p.estado.Value,
                                    NombreCategoria = p.id_categoria != null ? p.categorias.nombre_categoria : "",
                                    NombreMarca = p.id_marca != null ? p.marcas.nombre_marca : "",
                                    NombreUnidad = p.id_unidad != null ? p.unidades.nombre_unidad : "",
                                    NombreTipoIva = p.id_tipo_iva != null ? p.tipos_ivas.iva : "",
                                    ColorFila = p.productos_lotes.Count > 0 ? "background-color: transparent;" : "style=background-color:rgb(249,255,129);",
                                    EstadoDescrip = p.estado == true ? "Activo" : "Inactivo"
                                };
                listaProductos = productos.ToList();

                //FILTRAMOS POR NOMBRE CATEGORIA SUCURSAL DEPOSITO
                var fcNombreProducto = fc["txtProducto"];
                if (fcNombreProducto != "")
                {
                    string descripcion = Convert.ToString(fcNombreProducto);
                    listaProductos = listaProductos.Where(p => p.NombreProducto.ToUpper().Contains(descripcion.Trim().ToUpper())).ToList();
                }

                var fcIdCategoria = fc["ddlCategorias"];
                if (fcIdCategoria != "")
                {
                    int idCategoria = Convert.ToInt32(fcIdCategoria);
                    listaProductos = listaProductos.Where(p => p.IdCategoria == idCategoria).ToList();
                }

                listaProductos = listaProductos.OrderBy(p => p.NombreProducto).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtProducto = fcNombreProducto;
                Session["sesionProductosNombre"] = fcNombreProducto;
                ViewBag.ddlCategorias = new SelectList(db.categorias.Where(c => c.estado == true).OrderBy(c => c.nombre_categoria).ToList(), "id", "nombre_categoria", fcIdCategoria);
                Session["sesionProductosIdCategoria"] = fcIdCategoria;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar depositos";
            }
            return View(listaProductos.ToPagedList(pageIndex, pageSize));
        }

        #endregion


        #region Crear Producto

        [HttpGet]
        [AutorizarUsuario("Productos", "Create")]
        public ActionResult Create()
        {
            ProductoModel producto = new ProductoModel();
            ViewBag.IdCategoria = new SelectList(db.categorias.Where(c => c.estado == true).OrderBy(c => c.nombre_categoria).ToList(), "id", "nombre_categoria");
            ViewBag.IdMarca = new SelectList(db.marcas.Where(m => m.estado == true).OrderBy(m => m.nombre_marca).ToList(), "id", "nombre_marca");
            ViewBag.IdUnidad = new SelectList(db.unidades.Where(u => u.estado == true).OrderBy(u => u.nombre_unidad).ToList(), "id", "nombre_unidad");
            ViewBag.IdTipoIva = new SelectList(db.tipos_ivas.Where(ti => ti.estado == true).OrderBy(ti => ti.iva).ToList(), "id", "iva");
            return View(producto);
        }

        [HttpPost]
        public ActionResult Create(ProductoModel productoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN PRODUCTO CON EL MISMO NOMBRE PARA AGREGAR
                    int cantidad = db.productos.Where(p => p.nombre_producto.ToUpper() == productoModelo.NombreProducto.Trim().ToUpper() && p.estado != null).Count();
                    if (cantidad == 0)
                    {
                        string strPrecioVenta = productoModelo.StrPrecioVenta;
                        strPrecioVenta = strPrecioVenta.Replace(".", "");
                        decimal precioVenta = Convert.ToDecimal(strPrecioVenta);

                        productos producto = new productos
                        {
                            nombre_producto = productoModelo.NombreProducto,
                            id_categoria = productoModelo.IdCategoria,
                            id_marca = productoModelo.IdMarca,
                            id_unidad = productoModelo.IdUnidad,
                            precio_venta = precioVenta,
                            precio_costo = 0,
                            stock = 0,
                            id_tipo_iva = productoModelo.IdTipoIva,
                            estado = true
                        };
                        db.productos.Add(producto);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un producto registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el producto en la base de datos");
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
                CargarDatosProducto(productoModelo);
                db.Dispose();
                return View(productoModelo);
            }
        }

        #endregion

        #region Editar Producto

        [HttpGet]
        [AutorizarUsuario("Productos", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductoModel productoEdit = new ProductoModel();
            try
            {
                var producto = db.productos.Where(p => p.id == id).FirstOrDefault();
                productoEdit.Id = producto.id;
                productoEdit.NombreProducto = producto.nombre_producto;
                productoEdit.IdCategoria = producto.id_categoria;
                productoEdit.IdUnidad = producto.id_unidad;
                productoEdit.IdMarca = producto.id_marca;
                productoEdit.StrPrecioVenta = String.Format("{0:#,##0.##}", producto.precio_venta);
                productoEdit.StrStock = String.Format("{0:#,##0.##}", producto.stock);
                productoEdit.IdTipoIva = producto.id_tipo_iva;
                productoEdit.Estado = producto.estado.Value;

                CargarDatosProducto(productoEdit);

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = producto.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(productoEdit);
        }

        [HttpPost]
        public ActionResult Edit(ProductoModel productoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN PRODUCTO CON EL MISMO NOMBRE PARA EDITAR
                    int cantidad = db.productos.Where(p => p.nombre_producto.ToUpper() == productoModelo.NombreProducto.Trim().ToUpper() && p.estado != null && p.id != productoModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        string strPrecioVenta = productoModelo.StrPrecioVenta;
                        strPrecioVenta = strPrecioVenta.Replace(".", "");
                        decimal precioVenta = Convert.ToDecimal(strPrecioVenta);

                        var producto = db.productos.Where(p => p.id == productoModelo.Id).FirstOrDefault();
                        producto.nombre_producto = productoModelo.NombreProducto;
                        producto.id_categoria = productoModelo.IdCategoria;
                        producto.id_marca = productoModelo.IdMarca;
                        producto.id_unidad = productoModelo.IdUnidad;
                        producto.precio_venta = precioVenta;
                        producto.id_tipo_iva = productoModelo.IdTipoIva;
                        db.Entry(producto).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un producto registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al editar el producto en la base de datos");
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
                CargarDatosProducto(productoModelo);
                db.Dispose();
                return View(productoModelo);
            }
        }

        #endregion

        #region Eliminar Producto

        [HttpPost]
        public ActionResult EliminarProducto(string productoId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Productos", "EliminarProducto");
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
                                int idProducto = Convert.ToInt32(productoId);
                                var producto = context.productos.Where(p => p.id == idProducto).FirstOrDefault();
                                producto.estado = null;
                                context.Entry(producto).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Productos") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Agregar Premio Lote

        [HttpGet]
        [AutorizarUsuario("Productos", "AgregarProductoLote")]
        public ActionResult AgregarProductoLote(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductoLoteModel productoLote = new ProductoLoteModel();
            try
            {
                var producto = db.productos.Where(p => p.id == id).FirstOrDefault();
                productoLote.IdProducto = producto.id;
                productoLote.NombreProducto = producto.nombre_producto;

                CargarDatosProductoLote(productoLote);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(productoLote);
        }

        [HttpPost]
        public ActionResult AgregarProductoLote(ProductoLoteModel productoLoteModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    decimal cantidad = 0;
                    string strCantidad = productoLoteModelo.StrCantidad;
                    strCantidad = strCantidad.Replace(".", "");
                    cantidad = Convert.ToDecimal(strCantidad);

                    //INICIAMOS LA TRANSACCIÓN DE AGREGAR O ACTUALIZAR PRODUCTO LOTE EN LA BASE DE DATOS
                    using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                //VARIFICAMOS SI EXISTE UN PRODUCTO EN LA SUCURSAL SELECCIONADA
                                //SI NO EXISTE AGREGAMOS UN REGISTRO DE PRODUCTO LOTE, SI YA EXISTE ACTUALIZAMOS LA CANTIDAD DE PRODUCTO LOTE DE LA SUCURSAL
                                var count = context.productos_lotes.Where(pl => pl.id_producto == productoLoteModelo.IdProducto && pl.id_sucursal == productoLoteModelo.IdSucursal && pl.id_deposito == productoLoteModelo.IdDeposito && pl.estado != null).Count();
                                int idProductoLote = 0;
                                if (count == 0)
                                {
                                    productos_lotes productoLote = new productos_lotes
                                    {
                                        id_producto = productoLoteModelo.IdProducto,
                                        cantidad = cantidad,
                                        id_sucursal = productoLoteModelo.IdSucursal,
                                        id_deposito = productoLoteModelo.IdDeposito,
                                        ubicacion = productoLoteModelo.Ubicacion,
                                        estado = true
                                    };
                                    context.productos_lotes.Add(productoLote);
                                    context.SaveChanges();
                                    idProductoLote = productoLote.id;
                                }
                                else
                                {
                                    var productoLote = context.productos_lotes.Where(pl => pl.id_producto == productoLoteModelo.IdProducto && pl.id_sucursal == productoLoteModelo.IdSucursal && pl.id_deposito == productoLoteModelo.IdDeposito && pl.estado != null).FirstOrDefault();
                                    productoLote.cantidad += cantidad;
                                    productoLote.ubicacion = productoLoteModelo.Ubicacion;
                                    context.Entry(productoLote).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                    idProductoLote = productoLote.id;
                                }

                                //ACTUALIZAMOS LA CANTIDAD DE PRODUCTOS
                                var producto = context.productos.Where(p => p.id == productoLoteModelo.IdProducto).FirstOrDefault();
                                producto.stock += cantidad;
                                context.Entry(producto).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();

                                //AGREGAMOS EL AJUSTE REALIZADO A TRAVES DE PRODUCTO LOTE
                                int idUsuario = Convert.ToInt32(Session["IdUser"]);
                                productos_lotes_ajustes productoLoteAjuste = new productos_lotes_ajustes
                                {
                                    fecha = DateTime.Now,
                                    id_usuario = idUsuario,
                                    id_producto_lote = idProductoLote,
                                    cantidad = cantidad,
                                    observacion = productoLoteModelo.Observacion,
                                    ingreso_lote = true,
                                    estado = true
                                };
                                context.productos_lotes_ajustes.Add(productoLoteAjuste);
                                context.SaveChanges();

                                dbContextTransaction.Commit();
                            }
                            catch (Exception)
                            {
                                dbContextTransaction.Rollback();
                                ModelState.AddModelError("Error", "Ocurrio un error al realizar la transacción de producto lote");
                                retornoVista = true;
                            }
                        }
                        context.Database.Connection.Close();
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el producto en la base de datos");
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
                CargarDatosProductoLote(productoLoteModelo);
                return View(productoLoteModelo);
            }
        }

        #endregion

        #region Disminuir Premio Lote

        [HttpGet]
        [AutorizarUsuario("Productos", "DisminuirProductoLote")]
        public ActionResult DisminuirProductoLote(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductoLoteModel productoLote = new ProductoLoteModel();
            try
            {
                var producto = db.productos.Where(p => p.id == id).FirstOrDefault();
                productoLote.IdProducto = producto.id;
                productoLote.NombreProducto = producto.nombre_producto;

                CargarDatosProductoLote(productoLote);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(productoLote);
        }

        [HttpPost]
        public ActionResult DisminuirProductoLote(ProductoLoteModel productoLoteModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    decimal cantidad = 0;
                    string strCantidad = productoLoteModelo.StrCantidad;
                    strCantidad = strCantidad.Replace(".", "");
                    cantidad = Convert.ToDecimal(strCantidad);

                    //INICIAMOS LA TRANSACCIÓN DE DISMINUIR O ACTUALIZAR PRODUCTO LOTE EN LA BASE DE DATOS
                    using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                //VARIFICAMOS SI EXISTE UN PRODUCTO EN LA SUCURSAL SELECCIONADA
                                //SI NO EXISTE AGREGAMOS UN REGISTRO DE PRODUCTO LOTE, SI YA EXISTE ACTUALIZAMOS LA CANTIDAD DE PRODUCTO LOTE DE LA SUCURSAL
                                var count = context.productos_lotes.Where(pl => pl.id_producto == productoLoteModelo.IdProducto && pl.id_sucursal == productoLoteModelo.IdSucursal && pl.id_deposito == productoLoteModelo.IdDeposito && pl.estado != null).Count();
                                int idProductoLote = 0;
                                if (count == 0)
                                {

                                }
                                else
                                {
                                    var productoLote = context.productos_lotes.Where(pl => pl.id_producto == productoLoteModelo.IdProducto && pl.id_sucursal == productoLoteModelo.IdSucursal && pl.id_deposito == productoLoteModelo.IdDeposito && pl.estado != null).FirstOrDefault();
                                    productoLote.cantidad -= cantidad;
                                    context.Entry(productoLote).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                    idProductoLote = productoLote.id;

                                    //ACTUALIZAMOS LA CANTIDAD DE PRODUCTOS
                                    var producto = context.productos.Where(p => p.id == productoLoteModelo.IdProducto).FirstOrDefault();
                                    producto.stock -= cantidad;
                                    context.Entry(producto).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();

                                    //AGREGAMOS EL AJUSTE REALIZADO A TRAVES DE PRODUCTO LOTE
                                    int idUsuario = Convert.ToInt32(Session["IdUser"]);
                                    productos_lotes_ajustes productoLoteAjuste = new productos_lotes_ajustes
                                    {
                                        fecha = DateTime.Now,
                                        id_usuario = idUsuario,
                                        id_producto_lote = idProductoLote,
                                        cantidad = cantidad,
                                        observacion = productoLoteModelo.Observacion,
                                        ingreso_lote = false,
                                        estado = true
                                    };
                                    context.productos_lotes_ajustes.Add(productoLoteAjuste);
                                    context.SaveChanges();
                                }

                                dbContextTransaction.Commit();
                            }
                            catch (Exception)
                            {
                                dbContextTransaction.Rollback();
                                ModelState.AddModelError("Error", "Ocurrio un error al realizar la transacción de producto lote");
                                retornoVista = true;
                            }
                        }
                        context.Database.Connection.Close();
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al disminuir el producto en la base de datos");
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
                CargarDatosProductoLote(productoLoteModelo);
                return View(productoLoteModelo);
            }
        }

        #endregion

        #region Funciones

        private void CargarDatosProducto(ProductoModel productoModelo)
        {
            ViewBag.IdCategoria = new SelectList(db.categorias.Where(c => c.estado == true).OrderBy(c => c.nombre_categoria).ToList(), "id", "nombre_categoria", productoModelo.IdCategoria);
            ViewBag.IdMarca = new SelectList(db.marcas.Where(m => m.estado == true).OrderBy(m => m.nombre_marca).ToList(), "id", "nombre_marca", productoModelo.IdMarca);
            ViewBag.IdUnidad = new SelectList(db.unidades.Where(u => u.estado == true).OrderBy(u => u.nombre_unidad).ToList(), "id", "nombre_unidad", productoModelo.IdUnidad);
            ViewBag.IdTipoIva = new SelectList(db.tipos_ivas.Where(ti => ti.estado == true).OrderBy(ti => ti.iva).ToList(), "id", "iva", productoModelo.IdTipoIva);
        }

        private void CargarDatosProductoLote(ProductoLoteModel productoLoteModelo)
        {
            ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", productoLoteModelo.IdSucursal);
            int idSucursal = productoLoteModelo.IdSucursal != null ? productoLoteModelo.IdSucursal.Value : 0;
            ViewBag.IdDeposito = new SelectList(db.depositos.Where(d => d.estado == true && d.id_sucursal == idSucursal).OrderBy(d => d.nombre_deposito).ToList(), "id", "nombre_deposito", productoLoteModelo.IdDeposito);
        }

        #endregion

        #region AJAX

        [HttpPost]
        public ActionResult ObtenerListadoDetalleStockSucursales(string idPremio)
        {
            bool control = true;
            string msg = string.Empty;
            List<productos_lotes> listado = new List<productos_lotes>();
            var tabla = string.Empty;
            int id = Convert.ToInt32(idPremio);
            try
            {
                listado = (from pl in db.productos_lotes
                           where pl.id_producto == id && pl.estado == true
                           select pl).ToList();

                List<Int32> listaIdSucursal = new List<Int32>();
                foreach (var item in listado)
                {
                    bool resultado = listaIdSucursal.Contains(item.id_sucursal.Value);
                    if (resultado == false)
                    {
                        listaIdSucursal.Add(item.id_sucursal.Value);
                    }
                }

                var tbl = "tbl_" + id;
                var row = "";
                if (listado.Count > 0)
                {
                    foreach (var idSuc in listaIdSucursal)
                    {
                        var sucursal = db.sucursales.Where(s => s.id == idSuc).FirstOrDefault();
                        var listaProductoLote = db.productos_lotes.Where(pl => pl.id_producto == id && pl.estado == true && pl.id_sucursal == idSuc).ToList();
                        decimal cantidad = listaProductoLote.Sum(lpl => lpl.cantidad.Value);

                        row += "                   <tr>";
                        row += "                       <td>" + sucursal.nombre_sucursal + "</td>";
                        row += "                       <td>" + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("es-ES"), "{0:#,##0.##}", cantidad) + "</td>    ";
                        row += "                   </tr> ";
                    }
                }
                else
                {
                    row += "                   <tr>";
                    row += "                       <td> No se encuentran productos.</td>";
                    row += "                   </tr> ";
                }

                tabla = " <table class=\"table table-striped jambo_table\" id=\"" + tbl + "\">";
                tabla += " <thead>";
                tabla += "               <tr>";
                tabla += "                   <th>Sucursal</th>";
                tabla += "                   <th>Cantidad</th>";
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
                msg = "Hubo un error al consultar los productos de las sucursales";
            }
            return Json(new { success = control, msg = msg, listaDetalle = tabla }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObtenerListadoProductos(string descripcion)
        {
            string respuesta = string.Empty;
            string msg = string.Empty;
            string row = string.Empty;
            try
            {
                List<productos> listaProductos = new List<productos>();
                if (descripcion != string.Empty)
                {
                    listaProductos = db.productos.Where(p => p.nombre_producto.Trim().ToUpper().Contains(descripcion.Trim().ToUpper()) && p.estado != null).ToList();
                }
                foreach (var item in listaProductos)
                {
                    var elHidden = "<input type=\"hidden\"  name=\"ivaProducto\" value=\"" + item.id_tipo_iva + "\"> ";

                    row += "<tr>" +
                        "<td>" + item.id + " </td>" +
                        "<td>" + item.nombre_producto + " </td>";
                    row += "<td><button type=\'button\' title='Seleccionar Producto' class=\'btn btn-success btn-xs seleccionarProducto\'><i class=\'glyphicon glyphicon-saved\'></i></button></td>" + "<td>" + elHidden + " </td>" + "</tr>";
                }
            }
            catch (Exception)
            {
                respuesta = "Error";
            }
            return Json(new { succes = true, respuesta = respuesta, msg = msg, row = row }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObtenerListadoProductosVentas(string descripcion)
        {
            string respuesta = string.Empty;
            string msg = string.Empty;
            string row = string.Empty;
            try
            {
                int IdUsuario = Convert.ToInt32(Session["IdUser"]);
                var usuario = db.usuarios.Where(u => u.id == IdUsuario).FirstOrDefault();
                int idSucursal = usuario.id_funcionario != null ? usuario.funcionarios.id_sucursal.Value : 0;

                List<productos> listaProductos = new List<productos>();
                if (descripcion != string.Empty)
                {
                    listaProductos = db.productos.Where(p => p.nombre_producto.Trim().ToUpper().Contains(descripcion.Trim().ToUpper()) && p.estado == true).ToList();
                }
                foreach (var item in listaProductos)
                {
                    var listaProductosLotes = db.productos_lotes.Where(pl => pl.id_producto == item.id && pl.id_sucursal == idSucursal && pl.estado == true).ToList();
                    decimal totalProductos = listaProductosLotes.Sum(s => s.cantidad.Value);

                    var ivaProducto = "<input type=\"hidden\"  name=\"ivaProducto\" value=\"" + item.id_tipo_iva + "\"> ";
                    var precioVenta = "<input type=\"hidden\"  name=\"precioVenta\" value=\"" + String.Format("{0:#,##0.##}", item.precio_venta) + "\"> ";

                    row += "<tr>" +
                        "<td>" + item.id + " </td>" +
                        "<td>" + item.nombre_producto + " </td>" +
                        "<td>" + String.Format("{0:#,##0.##}", totalProductos) + " </td>";
                    row += "<td><button type=\'button\' title='Seleccionar Producto' class=\'btn btn-success btn-xs seleccionarProducto\'><i class=\'glyphicon glyphicon-saved\'></i></button></td>" + "<td>" + ivaProducto + " </td>" + "<td>" + precioVenta + " </td>" + "</tr>";
                }
            }
            catch (Exception)
            {
                respuesta = "Error";
            }
            return Json(new { succes = true, respuesta = respuesta, msg = msg, row = row }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObtenerListadoProductosRestaurante(string productoNombre)
        {
            string respuesta = string.Empty;
            string msg = string.Empty;
            string row = string.Empty;
            try
            {
                List<ProductoLoteModel> listaProductosLotes = new List<ProductoLoteModel>();
                if (productoNombre != string.Empty)
                {
                    //OBTENEMOS EL LISTADO DE TODOS LOS PRODUCTOS DE LA BASE DATOS ACTIVOS Y CARGAMOS EN UN MODELO PARA LUEGO FILTRAR
                    var productoLote = from pl in db.productos_lotes
                                       where pl.estado == true
                                       select new ProductoLoteModel
                                       {
                                           Id = pl.id,
                                           NombreProducto = pl.productos.nombre_producto,
                                           Precio = pl.productos.precio_venta
                                       };
                    listaProductosLotes = productoLote.ToList();

                    //FILTRAMOS LA BUSQUEDA
                    if (productoNombre != string.Empty)
                    {
                        listaProductosLotes = listaProductosLotes.Where(pl => pl.NombreProducto.Trim().ToUpper().Contains(productoNombre.Trim().ToUpper())).Take(50).ToList();
                    }
                }

                foreach (var item in listaProductosLotes)
                {
                    row += "<tr>" +
                        "<td>" + item.Id + " </td>" +
                        "<td>" + item.NombreProducto + " </td>" +
                        "<td>" + String.Format("{0:#,##0.##}", item.Precio) + " </td>";
                    row += "<td><button type=\'button\' title='Seleccionar Producto' class=\'btn btn-success btn-xs seleccionarProducto\'><i class=\'glyphicon glyphicon-saved\'></i></button></td>" + "</tr>";
                }
            }
            catch (Exception)
            {
                respuesta = "Error";
            }
            return Json(new { succes = true, respuesta = respuesta, msg = msg, row = row }, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}