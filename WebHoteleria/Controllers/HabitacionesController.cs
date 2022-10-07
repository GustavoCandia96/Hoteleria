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

        #endregion
    }


}