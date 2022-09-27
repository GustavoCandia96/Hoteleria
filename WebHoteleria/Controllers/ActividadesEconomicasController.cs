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
    public class ActividadesEconomicasController : Controller
    {

        /*
        * ACTIVIDADES ECONOMICAS
        * 
        * El modulo de actividades económicas registra las actividades que puede hacer un cliente o un proveedor. 
        * Ejemplo (comerciante, importador de vehículos).
        */
        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Actividades Economicas
        [HttpGet]
        [AutorizarUsuario("ActividadesEconomicas", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ActividadEconomicaModel> listaActividadesEconomicas = new List<ActividadEconomicaModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomActividad = Convert.ToString(Session["sesionActividadesEconomicasNombre"]);
                ViewBag.txtActividad = sesNomActividad;

                //OBTENEMOS TODOS LAS ACTIVIDADES ECONOMICAS NO ELIMINADAS DE LA BASE DE DATOS
                var actividadesEconomicas = from ae in db.actividades_economicas
                                            where ae.estado != null
                                            select new ActividadEconomicaModel
                                            {
                                                Id = ae.id,
                                                Actividad = ae.actividad,
                                                Estado = ae.estado,
                                                EstadoDescrip = ae.estado == true ? "Activo" : "Inactivo"
                                            };
                listaActividadesEconomicas = actividadesEconomicas.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomActividad != "")
                {
                    listaActividadesEconomicas = listaActividadesEconomicas.Where(ae => ae.Actividad.ToUpper().Contains(sesNomActividad.Trim().ToUpper())).ToList();
                }

                listaActividadesEconomicas = listaActividadesEconomicas.OrderBy(ae => ae.Actividad).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de actividades economicas";
            }
            return View(listaActividadesEconomicas.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ActividadEconomicaModel> listaActividadesEconomicas = new List<ActividadEconomicaModel>();
            try
            {
                //OBTENEMOS TODOS LAS ACTIVIDADES ECONOMICAS NO ELIMINADAS DE LA BASE DE DATOS
                var actividadesEconomicas = from ae in db.actividades_economicas
                                            where ae.estado != null
                                            select new ActividadEconomicaModel
                                            {
                                                Id = ae.id,
                                                Actividad = ae.actividad,
                                                Estado = ae.estado,
                                                EstadoDescrip = ae.estado == true ? "Activo" : "Inactivo"
                                            };
                listaActividadesEconomicas = actividadesEconomicas.ToList();

                //FILTRAMOS POR NOMBRE ACTIVIDAD LA BUSQUEDA
                var fcNombreActividad = fc["txtActividad"];
                if (fcNombreActividad != "")
                {
                    string descripcion = Convert.ToString(fcNombreActividad);
                    listaActividadesEconomicas = listaActividadesEconomicas.Where(ae => ae.Actividad.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }
                listaActividadesEconomicas.OrderBy(ae => ae.Actividad).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtActividad = fcNombreActividad;
                Session["sesionActividadesEconomicasNombre"] = fcNombreActividad;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar actividades economicas";
            }
            return View(listaActividadesEconomicas.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Actividad Economica

        [HttpGet]
        [AutorizarUsuario("ActividadesEconomicas", "Create")]
        public ActionResult Create()
        {
            ActividadEconomicaModel actividadEconomica = new ActividadEconomicaModel();
            return View(actividadEconomica);
        }

        [HttpPost]
        public ActionResult Create(ActividadEconomicaModel actividadEconomicaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE ACTIVIDAD ECONOMICA EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.actividades_economicas.Where(ae => ae.actividad.Trim().ToUpper() == actividadEconomicaModelo.Actividad.Trim().ToUpper() && ae.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //AGREGAMOS EL REGISTRO DE ACTIVIDAD ECONOMICA
                        actividades_economicas actividadEconomica = new actividades_economicas
                        {
                            actividad = actividadEconomicaModelo.Actividad.Trim(),
                            estado = true
                        };
                        db.actividades_economicas.Add(actividadEconomica);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe actividad economica registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar actividad economica en la base de datos");
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
                return View(actividadEconomicaModelo);
            }
        }



        #endregion

        #region Editar Actividad Economica

        [HttpGet]
        [AutorizarUsuario("ActividadesEconomicas", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ActividadEconomicaModel actividadEconomicaEdit = new ActividadEconomicaModel();
            try
            {
                //OBTENEMOS EL REGISTRO DE ACTIVIDAD ECONOMICA Y CARGAMOS LAS PROPIEDADES AL MODELO PARA EDITAR
                var actividadEconomica = db.actividades_economicas.Where(ae => ae.id == id).FirstOrDefault();
                actividadEconomicaEdit.Id = actividadEconomica.id;
                actividadEconomicaEdit.Actividad = actividadEconomica.actividad;
                actividadEconomicaEdit.Estado = actividadEconomica.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = actividadEconomica.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(actividadEconomicaEdit);
        }

        [HttpPost]
        public ActionResult Edit(ActividadEconomicaModel actividadEconomicaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA ACTIVIDAD ECONOMICA EN LA BASE DE DATOS PARA PODER ACTUALIZAR
                    int cantidad = db.actividades_economicas.Where(ae => ae.actividad.Trim().ToUpper() == actividadEconomicaModelo.Actividad.Trim().ToUpper() && ae.estado != null && ae.id != actividadEconomicaModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //ACTUALIZAMOS EL REGISTRO DE UNA ACTIVIDAD ECONOMICA
                        var actividadEconomica = db.actividades_economicas.Where(ae => ae.id == actividadEconomicaModelo.Id).FirstOrDefault();
                        actividadEconomica.actividad = actividadEconomicaModelo.Actividad.Trim();
                        bool nuevoEstado = actividadEconomicaModelo.EstadoDescrip == "A" ? true : false;
                        actividadEconomica.estado = nuevoEstado;
                        db.Entry(actividadEconomica).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una actividad economica registrada con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar la actividad economica en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", actividadEconomicaModelo.EstadoDescrip);
                return View(actividadEconomicaModelo);
            }
        }


        #endregion

        #region Eliminar Actividad Economica

        [HttpPost]
        public ActionResult EliminarActividadEconomica(string actividadEconomicaId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("ActividadesEconomicas", "EliminarActividadEconomica");
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
                                //OBTENEMOS EL REGISTRO Y ACTUALIZAMOS A ESTADO NULL (ELIMINADO)
                                int idActividadEconomica = Convert.ToInt32(actividadEconomicaId);
                                var actividadEconomica = context.actividades_economicas.Where(ae => ae.id == idActividadEconomica).FirstOrDefault();
                                actividadEconomica.estado = null;
                                context.Entry(actividadEconomica).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "ActividadesEconomicas") }, JsonRequestBehavior.AllowGet);
        }

        #endregion



    }
}