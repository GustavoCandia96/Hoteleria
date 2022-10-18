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
    public class MarcasController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion
        
        #region Listado de Marcas

        [HttpGet]
        [AutorizarUsuario("Marcas", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<MarcaModel> listaMarcas = new List<MarcaModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomMarca = Convert.ToString(Session["sesionMarcasNombre"]);
                ViewBag.txtMarca = sesNomMarca;

                //OBTENEMOS TODOS LAS MARCAS ACTIVAS DE LA BASE DE DATOS
                var marcas = from m in db.marcas
                             where m.estado != null
                             select new MarcaModel
                             {
                                 Id = m.id,
                                 NombreMarca = m.nombre_marca,
                                 Estado = m.estado,
                                 EstadoDescrip = m.estado == true ? "Activo" : "Inactivo"
                             };
                listaMarcas = marcas.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomMarca != "")
                {
                    listaMarcas = listaMarcas.Where(m => m.NombreMarca.ToUpper().Contains(sesNomMarca.Trim().ToUpper())).ToList();
                }

                listaMarcas = listaMarcas.OrderBy(m => m.NombreMarca).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de marcas";
            }
            return View(listaMarcas.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<MarcaModel> listaMarcas = new List<MarcaModel>();
            try
            {
                //OBTENEMOS TODOS LAS MARCAS ACTIVAS DE LA BASE DE DATOS
                var marcas = from m in db.marcas
                             where m.estado != null
                             select new MarcaModel
                             {
                                 Id = m.id,
                                 NombreMarca = m.nombre_marca,
                                 Estado = m.estado,
                                 EstadoDescrip = m.estado == true ? "Activo" : "Inactivo"
                             };
                listaMarcas = marcas.ToList();

                //FILTRAMOS POR NOMBRE MARCA LA BUSQUEDA
                var fcNombreMarca = fc["txtMarca"];
                if (fcNombreMarca != "")
                {
                    string descripcion = Convert.ToString(fcNombreMarca);
                    listaMarcas = listaMarcas.Where(m => m.NombreMarca.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }
                listaMarcas = listaMarcas.OrderBy(m => m.NombreMarca).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtMarca = fcNombreMarca;
                Session["sesionMarcasNombre"] = fcNombreMarca;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar marcas";
            }
            return View(listaMarcas.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Marca

        [HttpGet]
        [AutorizarUsuario("Marcas", "Create")]
        public ActionResult Create()
        {
            MarcaModel marca = new MarcaModel();
            return View(marca);
        }

        [HttpPost]
        public ActionResult Create(MarcaModel marcaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA MARCA EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.marcas.Where(m => m.nombre_marca.ToUpper() == marcaModelo.NombreMarca.ToUpper() && m.estado != null).Count();
                    if (cantidad == 0)
                    {
                        marcas marca = new marcas
                        {
                            nombre_marca = marcaModelo.NombreMarca,
                            estado = true
                        };
                        db.marcas.Add(marca);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una marca registrada con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar una marca en la base de datos");
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
                return View(marcaModelo);
            }
        }

        #endregion

        #region Editar Marca

        [HttpGet]
        [AutorizarUsuario("Marcas", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MarcaModel marcaEdit = new MarcaModel();
            try
            {
                var marca = db.marcas.Where(m => m.id == id).FirstOrDefault();
                marcaEdit.Id = marca.id;
                marcaEdit.NombreMarca = marca.nombre_marca;
                marcaEdit.Estado = marca.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = marca.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(marcaEdit);
        }

        [HttpPost]
        public ActionResult Edit(MarcaModel marcaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA MARCA EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.marcas.Where(m => m.nombre_marca.ToUpper() == marcaModelo.NombreMarca.ToUpper() && m.estado != null && m.id != marcaModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var marca = db.marcas.Where(m => m.id == marcaModelo.Id).FirstOrDefault();
                        marca.nombre_marca = marcaModelo.NombreMarca;
                        bool nuevoEstado = marcaModelo.EstadoDescrip == "A" ? true : false;
                        marca.estado = nuevoEstado;
                        db.Entry(marca).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una marca registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar la marca en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", marcaModelo.EstadoDescrip);
                return View(marcaModelo);
            }
        }

        #endregion

        #region Eliminar Marca

        [HttpPost]
        public ActionResult EliminarMarca(string marcaId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Marcas", "EliminarMarca");
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
                                int idMarca = Convert.ToInt32(marcaId);
                                var marca = context.marcas.Where(m => m.id == idMarca).FirstOrDefault();
                                marca.estado = null;
                                context.Entry(marca).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Marcas") }, JsonRequestBehavior.AllowGet);
        }

        #endregion



    }
}