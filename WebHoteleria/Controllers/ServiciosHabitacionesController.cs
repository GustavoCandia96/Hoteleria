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
    public class ServiciosHabitacionesController : Controller
    {
        #region Propiedad
        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();
        #endregion

        #region Listado de Servicios Habitaciones

        [HttpGet]
        [AutorizarUsuario("ServiciosHabitaciones", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ServicioHabitacionModel> listaServiciosHabitaciones = new List<ServicioHabitacionModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomServicio = Convert.ToString(Session["sesionServiciosHabitacionesNombre"]);
                ViewBag.txtServicioHabitacion = sesNomServicio;

                //OBTENEMOS TODOS LOS SERVICIOS HABITACIONES ACTIVOS DE LA BASE DE DATOS
                var serviciosHabitaciones = from hs in db.habitaciones_servicios
                                            where hs.estado != null
                                            select new ServicioHabitacionModel
                                            {
                                                Id = hs.id,
                                                NombreServicio = hs.nombre_servicio,
                                                Estado = hs.estado,
                                                EstadoDescrip = hs.estado == true ? "Activo" : "Inactivo"
                                            };
                listaServiciosHabitaciones = serviciosHabitaciones.ToList();


                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomServicio != "")
                {
                    listaServiciosHabitaciones = listaServiciosHabitaciones.Where(hs => hs.NombreServicio.ToUpper().Contains(sesNomServicio.Trim().ToUpper())).ToList();
                }

                listaServiciosHabitaciones = listaServiciosHabitaciones.OrderBy(hs => hs.NombreServicio).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de servicios habitaciones";
            }
            return View(listaServiciosHabitaciones.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ServicioHabitacionModel> listaServiciosHabitaciones = new List<ServicioHabitacionModel>();
            try
            {
                //OBTENEMOS TODOS LOS SERVICIOS HABITACIONES ACTIVOS DE LA BASE DE DATOS
                var serviciosHabitaciones = from hs in db.habitaciones_servicios
                                            where hs.estado != null
                                            select new ServicioHabitacionModel
                                            {
                                                Id = hs.id,
                                                NombreServicio = hs.nombre_servicio,
                                                Estado = hs.estado,
                                                EstadoDescrip = hs.estado == true ? "Activo" : "Inactivo"
                                            };
                listaServiciosHabitaciones = serviciosHabitaciones.ToList();

                //FILTRAMOS POR NOMBRE PAIS LA BUSQUEDA
                var sesNomServicio = fc["txtServicioHabitacion"];
                if (sesNomServicio != "")
                {
                    string descripcion = Convert.ToString(sesNomServicio);
                    listaServiciosHabitaciones = listaServiciosHabitaciones.Where(hs => hs.NombreServicio.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }
                listaServiciosHabitaciones = listaServiciosHabitaciones.OrderBy(hs => hs.NombreServicio).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtServicioHabitacion = sesNomServicio;
                Session["sesionServiciosHabitacionesNombre"] = sesNomServicio;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar servicios habitaciones";
            }
            return View(listaServiciosHabitaciones.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Servicio Habitacion

        [HttpGet]
        [AutorizarUsuario("ServiciosHabitaciones", "Create")]
        public ActionResult Create()
        {
            ServicioHabitacionModel servicioHabitacion = new ServicioHabitacionModel();
            return View(servicioHabitacion);
        }

        [HttpPost]
        public ActionResult Create(ServicioHabitacionModel servicioHabitacionModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL SERVICIO HABITACIÓN EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.habitaciones_servicios.Where(hs => hs.nombre_servicio.ToUpper() == servicioHabitacionModelo.NombreServicio.ToUpper() && hs.estado != null).Count();
                    if (cantidad == 0)
                    {
                        habitaciones_servicios servicioHabitacion = new habitaciones_servicios
                        {
                            nombre_servicio = servicioHabitacionModelo.NombreServicio,
                            estado = true
                        };
                        db.habitaciones_servicios.Add(servicioHabitacion);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un servicio de habitación registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el servicio de habitación en la base de datos");
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
                return View(servicioHabitacionModelo);
            }
        }

        #endregion

        #region Editar Servicio Habitacion

        [HttpGet]
        [AutorizarUsuario("ServiciosHabitaciones", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ServicioHabitacionModel servicioHabitacionEdit = new ServicioHabitacionModel();
            try
            {
                var servicioHabitacion = db.habitaciones_servicios.Where(hs => hs.id == id).FirstOrDefault();
                servicioHabitacionEdit.Id = servicioHabitacion.id;
                servicioHabitacionEdit.NombreServicio = servicioHabitacion.nombre_servicio;
                servicioHabitacionEdit.Estado = servicioHabitacion.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = servicioHabitacion.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(servicioHabitacionEdit);
        }

        [HttpPost]
        public ActionResult Edit(ServicioHabitacionModel servicioHabitacionModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL SERVICIO HABITACIÓN EN LA BASE DE DATOS PARA PODER ACTUALIZAR
                    int cantidad = db.habitaciones_servicios.Where(hs => hs.nombre_servicio.ToUpper() == servicioHabitacionModelo.NombreServicio.ToUpper() && hs.estado != null && hs.id != servicioHabitacionModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var servicioHabitacion = db.habitaciones_servicios.Where(hs => hs.id == servicioHabitacionModelo.Id).FirstOrDefault();
                        servicioHabitacion.nombre_servicio = servicioHabitacionModelo.NombreServicio;
                        bool nuevoEstado = servicioHabitacionModelo.EstadoDescrip == "A" ? true : false;
                        servicioHabitacion.estado = nuevoEstado;
                        db.Entry(servicioHabitacion).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un servicio de habitación registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el servicio habitación en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", servicioHabitacionModelo.EstadoDescrip);
                return View(servicioHabitacionModelo);
            }
        }

        #endregion

        #region Eliminar Servicio Habitacion

        [HttpPost]
        public ActionResult EliminarServicioHabitacion(string servicioHabitacionId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("ServiciosHabitaciones", "EliminarServicioHabitacion");
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
                                int idServicioHabitacion = Convert.ToInt32(servicioHabitacionId);
                                var servicioHabitacion = context.habitaciones_servicios.Where(hs => hs.id == idServicioHabitacion).FirstOrDefault();
                                servicioHabitacion.estado = null;
                                context.Entry(servicioHabitacion).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "ServiciosHabitaciones") }, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}