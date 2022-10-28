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
    public class ProductosAjustesStockController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Productos Ajustes Stock

        [HttpGet]
        [AutorizarUsuario("ProductosAjustesStock", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<ProductoAjusteLoteModel> listaAjusteProductos = new List<ProductoAjusteLoteModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesFecha = Convert.ToString(Session["sesionProductosAjustesStockFecha"]);
                ViewBag.txtFecha = sesFecha;
                string sesNomProduc = Convert.ToString(Session["sesionProductosAjustesStockNombre"]);
                ViewBag.txtProducto = sesNomProduc;

                //OBTENEMOS TODOS LOS PRODUCTOS AJUSTE DE STOCK
                var productosAjustes = from pla in db.productos_lotes_ajustes
                                       where pla.estado != null && pla.ingreso_lote == false
                                       select new ProductoAjusteLoteModel
                                       {
                                           Id = pla.id,
                                           Fecha = pla.fecha,
                                           IdUsuario = pla.id_usuario,
                                           IdProductoLote = pla.id_producto_lote,
                                           Cantidad = pla.cantidad,
                                           Observacion = pla.observacion,
                                           IngresoLote = pla.ingreso_lote.Value,
                                           NombreUsuario = pla.usuarios.usuario,
                                           NombreProducto = pla.productos_lotes.productos.nombre_producto,
                                           Estado = pla.estado,
                                           EstadoDescrip = pla.estado == true ? "Activo" : "Inactivo"
                                       };
                listaAjusteProductos = productosAjustes.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomProduc != "")
                {
                    listaAjusteProductos = listaAjusteProductos.Where(pla => pla.NombreProducto.ToUpper().Contains(sesNomProduc.Trim().ToUpper())).ToList();
                }
                if (sesFecha != "")
                {
                    DateTime fecha = Convert.ToDateTime(sesFecha);
                    listaAjusteProductos = listaAjusteProductos.Where(pla => pla.Fecha >= fecha).ToList();
                }
                listaAjusteProductos = listaAjusteProductos.OrderBy(pla => pla.NombreProducto).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de productos ajustes stock";
            }
            return View(listaAjusteProductos.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<ProductoAjusteLoteModel> listaAjusteProductos = new List<ProductoAjusteLoteModel>();
            try
            {
                //OBTENEMOS TODOS LOS PRODUCTOS AJUSTE DE STOCK
                var productosAjustes = from pla in db.productos_lotes_ajustes
                                       where pla.estado != null && pla.ingreso_lote == false
                                       select new ProductoAjusteLoteModel
                                       {
                                           Id = pla.id,
                                           Fecha = pla.fecha,
                                           IdUsuario = pla.id_usuario,
                                           IdProductoLote = pla.id_producto_lote,
                                           Cantidad = pla.cantidad,
                                           Observacion = pla.observacion,
                                           IngresoLote = pla.ingreso_lote.Value,
                                           NombreUsuario = pla.usuarios.usuario,
                                           NombreProducto = pla.productos_lotes.productos.nombre_producto,
                                           Estado = pla.estado,
                                           EstadoDescrip = pla.estado == true ? "Activo" : "Inactivo"
                                       };
                listaAjusteProductos = productosAjustes.ToList();

                //FILTRAMOS POR NOMBRE CATEGORIA SUCURSAL DEPOSITO
                var fcNombreProducto = fc["txtProducto"];
                if (fcNombreProducto != "")
                {
                    string descripcion = Convert.ToString(fcNombreProducto);
                    listaAjusteProductos = listaAjusteProductos.Where(p => p.NombreProducto.ToUpper().Contains(descripcion.Trim().ToUpper())).ToList();
                }

                var fcFecha = fc["txtFecha"];
                if (fcFecha != "")
                {
                    DateTime fecha = Convert.ToDateTime(fcFecha);
                    listaAjusteProductos = listaAjusteProductos.Where(p => p.Fecha >= fecha).ToList();
                }

                listaAjusteProductos = listaAjusteProductos.OrderBy(p => p.NombreProducto).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtFecha = fcFecha;
                Session["sesionProductosAjustesStockFecha"] = fcFecha;
                ViewBag.txtProducto = fcNombreProducto;
                Session["sesionProductosAjustesStockNombre"] = fcNombreProducto;

            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar productos ajustes stock";
            }
            return View(listaAjusteProductos.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Ajuste Stock Producto

        [HttpGet]
        [AutorizarUsuario("ProductosAjustesStock", "Create")]
        public ActionResult Create()
        {
            ProductoAjusteLoteModel productoAjusteLote = new ProductoAjusteLoteModel();
            return View(productoAjusteLote);
        }

        //[HttpPost]
        //public ActionResult Create(ProductoModel productoModelo)
        //{
        //    bool retornoVista = false;
        //    ViewBag.msg = string.Empty;
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            //VERIFICAMOS SI YA EXISTE UN PRODUCTO CON EL MISMO NOMBRE PARA AGREGAR
        //            int cantidad = db.productos.Where(p => p.nombre_producto.ToUpper() == productoModelo.NombreProducto.Trim().ToUpper() && p.estado != null).Count();
        //            if (cantidad == 0)
        //            {
        //                string strPrecioVenta = productoModelo.StrPrecioVenta;
        //                strPrecioVenta = strPrecioVenta.Replace(".", "");
        //                decimal precioVenta = Convert.ToDecimal(strPrecioVenta);

        //                productos producto = new productos
        //                {
        //                    nombre_producto = productoModelo.NombreProducto,
        //                    id_categoria = productoModelo.IdCategoria,
        //                    id_marca = productoModelo.IdMarca,
        //                    id_unidad = productoModelo.IdUnidad,
        //                    precio_venta = precioVenta,
        //                    precio_costo = 0,
        //                    stock = 0,
        //                    id_tipo_iva = productoModelo.IdTipoIva,
        //                    estado = true
        //                };
        //                db.productos.Add(producto);
        //                db.SaveChanges();
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("Duplicado", "Ya existe un producto registrado con el mismo nombre");
        //                retornoVista = true;
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            ModelState.AddModelError("Error", "Ocurrio un error al agregar el producto en la base de datos");
        //            retornoVista = true;
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.msg = null;
        //        retornoVista = true;
        //    }
        //    if (retornoVista == false)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        CargarDatosProducto(productoModelo);
        //        db.Dispose();
        //        return View(productoModelo);
        //    }
        //}


        #endregion


    }
}