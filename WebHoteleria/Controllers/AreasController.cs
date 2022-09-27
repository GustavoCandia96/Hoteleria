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
    public class AreasController : Controller
    {

        /*
       * AREAS
       * 
       * El modulo de áreas registra las áreas de la empresa. Está relacionada con los módulos de cargos y funcionarios. 
       * Ejemplo (marketing, ventas).
       */

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Areas

        [HttpGet]
        [AutorizarUsuario("Areas", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<AreaModel> listaAreas = new List<AreaModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomArea = Convert.ToString(Session["sesionAreasNombre"]);
                ViewBag.txtArea = sesNomArea;

                //OBTENEMOS TODOS LAS AREAS NO ELIMINADAS DE LA BASE DE DATOS
                var areas = from a in db.areas
                            where a.estado != null
                            select new AreaModel
                            {
                                Id = a.id,
                                NombreArea = a.nombre_area,
                                Estado = a.estado,
                                EstadoDescrip = a.estado == true ? "Activo" : "Inactivo"
                            };
                listaAreas = areas.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomArea != "")
                {
                    listaAreas = listaAreas.Where(a => a.NombreArea.ToUpper().Contains(sesNomArea.Trim().ToUpper())).ToList();
                }

                listaAreas = listaAreas.OrderBy(a => a.NombreArea).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de areas";
            }
            return View(listaAreas.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<AreaModel> listaAreas = new List<AreaModel>();
            try
            {
                //OBTENEMOS TODOS LAS AREAS NO ELIMINADAS DE LA BASE DE DATOS
                var areas = from a in db.areas
                            where a.estado != null
                            select new AreaModel
                            {
                                Id = a.id,
                                NombreArea = a.nombre_area,
                                Estado = a.estado,
                                EstadoDescrip = a.estado == true ? "Activo" : "Inactivo"
                            };
                listaAreas = areas.ToList();

                //FILTRAMOS POR NOMBRE AREA LA BUSQUEDA
                var fcNombreArea = fc["txtArea"];
                if (fcNombreArea != "")
                {
                    string descripcion = Convert.ToString(fcNombreArea);
                    listaAreas = listaAreas.Where(a => a.NombreArea.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }

                listaAreas = listaAreas.OrderBy(a => a.NombreArea).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtArea = fcNombreArea;
                Session["sesionAreasNombre"] = fcNombreArea;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar areas";
            }
            return View(listaAreas.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Area

        [HttpGet]
        [AutorizarUsuario("Areas", "Create")]
        public ActionResult Create()
        {
            AreaModel area = new AreaModel();
            return View(area);
        }

        [HttpPost]
        public ActionResult Create(AreaModel areaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL AREA EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.areas.Where(a => a.nombre_area.Trim().ToUpper() == areaModelo.NombreArea.Trim().ToUpper() && a.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //AGREGAMOS EL REGISTRO DE UN AREA
                        areas area = new areas
                        {
                            nombre_area = areaModelo.NombreArea.Trim(),
                            estado = true
                        };
                        db.areas.Add(area);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un area registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el area en la base de datos");
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
                return View(areaModelo);
            }
        }

        #endregion


        #region Editar Area

        [HttpGet]
        [AutorizarUsuario("Areas", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AreaModel areaEdit = new AreaModel();
            try
            {
                //OBTENEMOS EL REGISTRO Y CARGAMOS LOS DATOS EN LAS PROPIEDADES DEL MODELO
                var area = db.areas.Where(a => a.id == id).FirstOrDefault();
                areaEdit.Id = area.id;
                areaEdit.NombreArea = area.nombre_area;
                areaEdit.Estado = area.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = area.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                ViewBag.idArea = new SelectList(db.areas.Where(a => a.estado == true).OrderBy(a => a.nombre_area).ToList(), "id", "nombre_area");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(areaEdit);
        }

        [HttpPost]
        public ActionResult Edit(AreaModel areaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN AREA PARA PODER ACTUALIZAR
                    int cantidad = db.areas.Where(a => a.nombre_area.Trim().ToUpper() == areaModelo.NombreArea.Trim().ToUpper() && a.estado != null && a.id != areaModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //OBTENEMOS EL REGISTRO DE AREAS PARA PODER ACTUALIZAR
                        var area = db.areas.Where(a => a.id == areaModelo.Id).FirstOrDefault();
                        area.nombre_area = areaModelo.NombreArea;
                        bool nuevoEstado = areaModelo.EstadoDescrip == "A" ? true : false;
                        area.estado = nuevoEstado;
                        db.Entry(area).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un area registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el area en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", areaModelo.EstadoDescrip);
                return View(areaModelo);
            }
        }

        #endregion

        #region Eliminar Area
        [HttpPost]
        public ActionResult EliminarArea(string areaId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Areas", "EliminarArea");
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
                                int idArea = Convert.ToInt32(areaId);
                                var area = context.areas.Where(a => a.id == idArea).FirstOrDefault();
                                area.estado = null;
                                context.Entry(area).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Areas") }, JsonRequestBehavior.AllowGet);
        }
        #endregion



    }
}