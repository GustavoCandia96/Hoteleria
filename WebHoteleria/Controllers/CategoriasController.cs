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
    public class CategoriasController : Controller
    {

        /*
        * CATEGORIAS
        * 
        * El modulo de categorías registra todos las categorías que tiene un premio. Está relacionado con el modulo de premios. 
        * Ejemplo (motos, vehículos).
        * 
        */

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Categorias

        [HttpGet]
        [AutorizarUsuario("Categorias", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CategoriaModel> listaCategorias = new List<CategoriaModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomCategoria = Convert.ToString(Session["sesionCategoriasNombre"]);
                ViewBag.txtCategoria = sesNomCategoria;

                //OBTENEMOS TODOS LAS CATEGORIAS NO ELIMINADAS DE LA BASE DE DATOS
                var categorias = from c in db.categorias
                                 where c.estado != null
                                 select new CategoriaModel
                                 {
                                     Id = c.id,
                                     NombreCategoria = c.nombre_categoria,
                                     Estado = c.estado,
                                     EstadoDescrip = c.estado == true ? "Activo" : "Inactivo"
                                 };
                listaCategorias = categorias.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomCategoria != "")
                {
                    listaCategorias = listaCategorias.Where(c => c.NombreCategoria.ToUpper().Contains(sesNomCategoria.Trim().ToUpper())).ToList();
                }

                listaCategorias = listaCategorias.OrderBy(c => c.NombreCategoria).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de categorias";
            }
            return View(listaCategorias.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CategoriaModel> listaCategorias = new List<CategoriaModel>();
            try
            {
                //OBTENEMOS TODOS LAS CATEGORIAS NO ELIMINADAS DE LA BASE DE DATOS
                var categorias = from c in db.categorias
                                 where c.estado != null
                                 select new CategoriaModel
                                 {
                                     Id = c.id,
                                     NombreCategoria = c.nombre_categoria,
                                     Estado = c.estado,
                                     EstadoDescrip = c.estado == true ? "Activo" : "Inactivo"
                                 };
                listaCategorias = categorias.ToList();

                //FILTRAMOS POR NOMBRE CATEGORIA LA BUSQUEDA
                var fcNombreCategoria = fc["txtCategoria"];
                if (fcNombreCategoria != "")
                {
                    string descripcion = Convert.ToString(fcNombreCategoria);
                    listaCategorias = listaCategorias.Where(c => c.NombreCategoria.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }
                listaCategorias = listaCategorias.OrderBy(c => c.NombreCategoria).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtCategoria = fcNombreCategoria;
                Session["sesionCategoriasNombre"] = fcNombreCategoria;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar categorias";
            }
            return View(listaCategorias.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Categoria

        [HttpGet]
        [AutorizarUsuario("Categorias", "Create")]
        public ActionResult Create()
        {
            CategoriaModel categoria = new CategoriaModel();
            return View(categoria);
        }

        [HttpPost]
        public ActionResult Create(CategoriaModel categoriaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE LA CATEGORIA EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.categorias.Where(c => c.nombre_categoria.Trim().ToUpper() == categoriaModelo.NombreCategoria.Trim().ToUpper() && c.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //AGREGAMOS EL REGISTRO DE CATEGORIAS
                        categorias categoria = new categorias
                        {
                            nombre_categoria = categoriaModelo.NombreCategoria.Trim(),
                            estado = true
                        };
                        db.categorias.Add(categoria);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una categoria registrada con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la categoria en la base de datos");
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
                return View(categoriaModelo);
            }
        }

        #endregion

        #region Editar Categoria

        [HttpGet]
        [AutorizarUsuario("Categorias", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CategoriaModel categoriaEdit = new CategoriaModel();
            try
            {
                //OBTENEMOS EL REGISTRO PARA CARGAR LOS DATOS A LAS PROPIEDADES DEL MODELO
                var categoria = db.categorias.Where(c => c.id == id).FirstOrDefault();
                categoriaEdit.Id = categoria.id;
                categoriaEdit.NombreCategoria = categoria.nombre_categoria;
                categoriaEdit.Estado = categoria.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = categoria.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(categoriaEdit);
        }

        [HttpPost]
        public ActionResult Edit(CategoriaModel categoriaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA CATEGORIA EN LA BASE DE DATOS PARA PODER ACTUALIZAR
                    int cantidad = db.categorias.Where(c => c.nombre_categoria.Trim().ToUpper() == categoriaModelo.NombreCategoria.Trim().ToUpper() && c.estado != null && c.id != categoriaModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //OBTENEMOS EL REGISTRO PARA ACTUALIZAR EN LA BASE DE DATOS
                        var categoria = db.categorias.Where(c => c.id == categoriaModelo.Id).FirstOrDefault();
                        categoria.nombre_categoria = categoriaModelo.NombreCategoria.Trim();
                        bool nuevoEstado = categoriaModelo.EstadoDescrip == "A" ? true : false;
                        categoria.estado = nuevoEstado;
                        db.Entry(categoria).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una categoria registrada con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar la categoria en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", categoriaModelo.EstadoDescrip);
                return View(categoriaModelo);
            }
        }

        #endregion


        #region Eliminar Categoria

        [HttpPost]
        public ActionResult EliminarCategoria(string categoriaId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Categorias", "EliminarCategoria");
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
                                int idCategoria = Convert.ToInt32(categoriaId);
                                var categoria = context.categorias.Where(c => c.id == idCategoria).FirstOrDefault();
                                categoria.estado = null;
                                context.Entry(categoria).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Categorias") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}