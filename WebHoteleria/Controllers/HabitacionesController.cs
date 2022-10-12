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
    public class HabitacionesController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities
();


        #endregion

        #region Listado de Habitaciones

        [HttpGet]
        [AutorizarUsuario("Habitaciones", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<HabitacionModel> listaHabitaciones = new List<HabitacionModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesIdTipoHabitacion = Convert.ToString(Session["sesionHabitacionesIdTipoHabitacion"]);
                ViewBag.ddlTiposHabitaciones = new SelectList(db.habitaciones_tipos.Where(ht => ht.estado == true).OrderBy(ht => ht.nombre).ToList(), "id", "nombre", sesIdTipoHabitacion);
                int idTipoHabitacion = sesIdTipoHabitacion != "" ? Convert.ToInt32(sesIdTipoHabitacion) : 0;
                //OBTENEMOS TODOS LAS HABITACIONES NO ELIMINADOS DE LA BASE DE DATOS
                var habitaciones = from h in db.habitaciones
                                   where h.estado != null
                                   select new HabitacionModel
                                   {
                                       Id = h.id,
                                       IdTipoHabitacion = h.id_tipo_habitacion,
                                       IdHabitacionEstado = h.id_habitacion_estado,
                                       Numero = h.numero,
                                       Estado = h.estado,
                                       NombreTipoHabitacion = h.habitaciones_tipos.nombre,
                                       NombreHabitacionEstado = h.habitaciones_estados.descripcion,
                                       EstadoDescrip = h.estado == true ? "Activo" : "Inactivo"
                                   };

                listaHabitaciones = habitaciones.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesIdTipoHabitacion != "")
                {
                    listaHabitaciones = listaHabitaciones.Where(lh => lh.IdTipoHabitacion == idTipoHabitacion).ToList();
                }

                listaHabitaciones = listaHabitaciones.OrderBy(lh => lh.NombreTipoHabitacion).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de tipos habitaciones";
            }
            return View(listaHabitaciones.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<HabitacionModel> listaHabitaciones = new List<HabitacionModel>();
            try
            {
                //OBTENEMOS TODOS LAS HABITACIONES NO ELIMINADOS DE LA BASE DE DATOS
                var habitaciones = from h in db.habitaciones
                                   where h.estado != null
                                   select new HabitacionModel
                                   {
                                       Id = h.id,
                                       IdTipoHabitacion = h.id_tipo_habitacion,
                                       IdHabitacionEstado = h.id_habitacion_estado,
                                       Numero = h.numero,
                                       Estado = h.estado,
                                       NombreTipoHabitacion = h.habitaciones_tipos.nombre,
                                       NombreHabitacionEstado = h.habitaciones_estados.descripcion,
                                       EstadoDescrip = h.estado == true ? "Activo" : "Inactivo"
                                   };

                listaHabitaciones = habitaciones.ToList();

                //FILTRAMOS POR NOMBRE BARRIO CIUDAD DEPARTAMENTO Y PAIS DE BUSQUEDA DEL USUARIO
                var fcIdTipoHabitacion = fc["ddlTiposHabitaciones"];
                int idTipoHabitacion = fcIdTipoHabitacion != "" ? Convert.ToInt32(fcIdTipoHabitacion) : 0;
                if (fcIdTipoHabitacion != "")
                {
                    listaHabitaciones = listaHabitaciones.Where(lh => lh.IdTipoHabitacion == idTipoHabitacion).ToList();
                }

                listaHabitaciones.OrderBy(lh => lh.NombreTipoHabitacion).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.ddlTiposHabitaciones = new SelectList(db.habitaciones_tipos.Where(ht => ht.estado == true).OrderBy(ht => ht.nombre).ToList(), "id", "nombre", fcIdTipoHabitacion);
                Session["sesionHabitacionesIdTipoHabitacion"] = fcIdTipoHabitacion;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar tipos habitaciones";
            }
            return View(listaHabitaciones.ToPagedList(pageIndex, pageSize));
        }


        #endregion

        #region Crear Habitaciones

        [HttpGet]
        [AutorizarUsuario("Habitaciones", "Create")]
        public ActionResult Create()
        {
            HabitacionModel habitacion = new HabitacionModel();
            //CARGAMOS EL LISTADO DE TIPOS HABITACIONES Y ESTADOS HABITACIONES.SE FILTRAN DESDE LA VISTA
            ViewBag.IdTipoHabitacion = new SelectList(db.habitaciones_tipos.Where(ht => ht.estado == true).OrderBy(ht => ht.nombre).ToList(), "id", "nombre");
            ViewBag.IdHabitacionEstado = new SelectList(db.habitaciones_estados.Where(he => he.estado == true).OrderBy(he => he.descripcion).ToList(), "id", "descripcion");
                 return View(habitacion);
        }

        [HttpPost]
        public ActionResult Create(HabitacionModel habitacionModelo, FormCollection fc)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA HABITACION CON EL MISMO NOMBRE PARA PODER AGREGAR
                    int cantidad = db.habitaciones.Where(h => h.numero.Trim().ToUpper() == habitacionModelo.Numero.Trim().ToUpper() && h.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //AGREGAMOS EL REGISTRO DE HABITACIONES
                        habitaciones habitacion = new habitaciones
                        {
                            numero = habitacionModelo.Numero.Trim(),
                            id_tipo_habitacion = habitacionModelo.IdTipoHabitacion,
                            id_habitacion_estado=habitacionModelo.IdHabitacionEstado,
                            estado = true
                        };
                        db.habitaciones.Add(habitacion);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una habitacion registrada con el mismo numero");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la habitacion en la base de datos");
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
                CargarDatosHabitaciones(habitacionModelo); //CARGAMOS LOS LISTADOS NECESARIOS
                db.Dispose();
                return View(habitacionModelo);
            }
        }


        #endregion

        #region Editar Habitaciones

        [HttpGet]
        [AutorizarUsuario("Habitaciones", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HabitacionModel habitacionEdit = new HabitacionModel();
            try
            {
                //OBTENEMOS EL REGISTRO Y CARGAMOS LOS DATOS EN LAS PROPIEDADES DEL MODELO
                var habitacion = db.habitaciones.Where(h => h.id == id).FirstOrDefault();
                habitacionEdit.Id = habitacion.id;
                habitacionEdit.IdTipoHabitacion = habitacion.id_tipo_habitacion;
                habitacionEdit.IdHabitacionEstado = habitacion.id_habitacion_estado;
                habitacionEdit.Numero = habitacion.numero;
                habitacionEdit.Estado = habitacion.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = habitacion.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
                CargarDatosHabitaciones(habitacionEdit); //CARGAMOS LOS LISTADOS NECESARIOS
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(habitacionEdit);
        }

        [HttpPost]
        public ActionResult Edit(HabitacionModel habitacionModelo, FormCollection fc)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
               if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA HABITACION CON EL MISMO NOMBRE PARA PODER AGREGAR
                    int cantidad = db.habitaciones.Where(b => b.numero.ToUpper() == habitacionModelo.Numero.Trim().ToUpper() && b.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //OBTENEMOS EL REGISTRO PARA PODER ACTUALIZAR HABITACION
                        var habitacion = db.habitaciones.Where(d => d.id == habitacionModelo.Id).FirstOrDefault();
                        habitacion.id_tipo_habitacion = habitacionModelo.IdTipoHabitacion;
                        habitacion.id_habitacion_estado = habitacionModelo.IdHabitacionEstado;
                        habitacion.numero = habitacionModelo.Numero;
                        bool nuevoEstado = habitacionModelo.EstadoDescrip == "A" ? true : false;
                        habitacion.estado = nuevoEstado;
                        db.Entry(habitacion).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una habitacion registrada con el mismo numero");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar la habitacion en la base de datos");
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
                CargarDatosHabitaciones(habitacionModelo); //CARGAMOS LOS LISTADOS NECESARIOS
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", habitacionModelo.EstadoDescrip);
                db.Dispose();
                return View(habitacionModelo);
            }
        }


        #endregion

        #region Eliminar Habitacion
        [HttpPost]
        public ActionResult EliminarHabitacion(string habitacionId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Habitaciones", "EliminarHabitacion");
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
                                int idHabitacion = Convert.ToInt32(habitacionId);
                                var habitacion = context.habitaciones.Where(ht => ht.id == idHabitacion).FirstOrDefault();
                                habitacion.estado = null;
                                context.Entry(habitacion).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Habitaciones") }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Funciones

        /*
         * METODO QUE RECIBE UN MODELO DE HABITACION Y CARGA LA LISTA DE tIPOS HABITACIONES Y ESTADOS HABITACIONES
         */
        private void CargarDatosHabitaciones(HabitacionModel habitacionModelo)
        {
            int idTipoHabitacion = habitacionModelo.IdTipoHabitacion != null ? habitacionModelo.IdTipoHabitacion.Value : 0;
            int idHabitacionEstado = habitacionModelo.IdHabitacionEstado != null ? habitacionModelo.IdHabitacionEstado.Value : 0;

            ViewBag.IdTipoHabitacion = new SelectList(db.habitaciones_tipos.Where(p => p.estado == true).OrderBy(p => p.nombre).ToList(), "id", "nombre", habitacionModelo.IdTipoHabitacion);
            ViewBag.IdHabitacionEstado = new SelectList(db.habitaciones_estados.Where(p => p.estado == true).OrderBy(p => p.descripcion).ToList(), "id", "descripcion", habitacionModelo.IdHabitacionEstado);

          
        }

        #endregion

    }


}