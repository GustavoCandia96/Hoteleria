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
    public class ReservasHabitacionesController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Reservas Habitaciones

        [HttpGet]
        [AutorizarUsuario("ReservasHabitaciones", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ReservaHabitacionModel> listaReserHab = new List<ReservaHabitacionModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesFecha = Convert.ToString(Session["sesionReservasHabitacionesFecha"]);
                ViewBag.txtFecha = sesFecha;

                //OBTENEMOS TODAS LAS RESERVAS DE HABITACIONES NO ELIMINADAS
                var reservas = from rh in db.reservas_habitaciones
                               where rh.estado != null
                               select new ReservaHabitacionModel
                               {
                                   Id = rh.id,
                                   IdCliente = rh.id_cliente,
                                   FechaAlta = rh.fecha_alta,
                                   IdUsuarioApertura = rh.id_usuario_apertura,
                                   FechaDesde = rh.fecha_desde,
                                   FechaHasta = rh.fecha_hasta,
                                   IdTipoHabitacion = rh.id_tipo_habitacion,
                                   IdHabitacion = rh.id_habitacion,
                                   IdReservaEstado = rh.id_reserva_estado,
                                   CheckIn = rh.check_in,
                                   CheckOut = rh.check_out,
                                   CantidadAdultos = rh.cantidad_adultos,
                                   CantidadMenores = rh.cantidad_menores,
                                   IdUsuarioCierre = rh.id_usuario_cierre,
                                   FechaCierre = rh.fecha_cierre,
                                   Estado = rh.estado,
                                   NombreUsuarioApertura = rh.id_usuario_apertura != null ? (rh.usuarios.id_funcionario != null ? rh.usuarios.funcionarios.nombre + " " + rh.usuarios.funcionarios.apellido : "") : "",
                                   NombreTipoHabitacion = rh.id_tipo_habitacion != null ? rh.habitaciones_tipos.nombre : "",
                                   NombreHabitacion = rh.id_habitacion != null ? rh.habitaciones.numero : "",
                                   NombreEstadoReserva = rh.id_reserva_estado != null ? rh.reservas_estados.descripcion : "",
                                   NombreUsuarioCierre = rh.id_usuario_cierre != null ? (rh.usuarios1.id_funcionario != null ? rh.usuarios1.funcionarios.nombre + " " + rh.usuarios1.funcionarios.apellido : "") : "",

                                   EstadoDescrip = rh.estado == true ? "Activo" : "Inactivo"
                               };
                listaReserHab = reservas.ToList();

                if (sesFecha != "")
                {
                    DateTime fecha = Convert.ToDateTime(sesFecha);
                    listaReserHab = listaReserHab.Where(rh => rh.FechaDesde >= fecha).ToList();
                }

                listaReserHab = listaReserHab.OrderByDescending(rh => rh.FechaDesde).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de reservas habitaciones";
            }
            return View(listaReserHab.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<ReservaHabitacionModel> listaReserHab = new List<ReservaHabitacionModel>();
            try
            {
                //OBTENEMOS TODAS LAS RESERVAS DE HABITACIONES NO ELIMINADAS
                var reservas = from rh in db.reservas_habitaciones
                               where rh.estado != null
                               select new ReservaHabitacionModel
                               {
                                   Id = rh.id,
                                   FechaAlta = rh.fecha_alta,
                                   IdUsuarioApertura = rh.id_usuario_apertura,
                                   FechaDesde = rh.fecha_desde,
                                   FechaHasta = rh.fecha_hasta,
                                   IdTipoHabitacion = rh.id_tipo_habitacion,
                                   IdHabitacion = rh.id_habitacion,
                                   IdReservaEstado = rh.id_reserva_estado,
                                   CheckIn = rh.check_in,
                                   CheckOut = rh.check_out,
                                   CantidadAdultos = rh.cantidad_adultos,
                                   CantidadMenores = rh.cantidad_menores,
                                   IdUsuarioCierre = rh.id_usuario_cierre,
                                   FechaCierre = rh.fecha_cierre,
                                   Estado = rh.estado,
                                   NombreUsuarioApertura = rh.id_usuario_apertura != null ? (rh.usuarios.id_funcionario != null ? rh.usuarios.funcionarios.nombre + " " + rh.usuarios.funcionarios.apellido : "") : "",
                                   NombreTipoHabitacion = rh.id_tipo_habitacion != null ? rh.habitaciones_tipos.nombre : "",
                                   NombreHabitacion = rh.id_habitacion != null ? rh.habitaciones.numero : "",
                                   NombreEstadoReserva = rh.id_reserva_estado != null ? rh.reservas_estados.descripcion : "",
                                   NombreUsuarioCierre = rh.id_usuario_cierre != null ? (rh.usuarios1.id_funcionario != null ? rh.usuarios1.funcionarios.nombre + " " + rh.usuarios1.funcionarios.apellido : "") : "",
                                   EstadoDescrip = rh.estado == true ? "Activo" : "Inactivo"
                               };
                listaReserHab = reservas.ToList();

                //FILTRAMOS POR FECHA LA BUSQUEDA
                var fcFecha = fc["txtFecha"];
                if (fcFecha != "")
                {
                    DateTime fecha = Convert.ToDateTime(fcFecha);
                    listaReserHab = listaReserHab.Where(rh => rh.FechaDesde >= fecha).ToList();
                }

                listaReserHab = listaReserHab.OrderByDescending(rh => rh.FechaDesde).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtFecha = fcFecha;
                Session["sesionReservasHabitacionesFecha"] = fcFecha;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar reservas habitaciones";
            }
            return View(listaReserHab.ToPagedList(pageIndex, pageSize));
        }

        #endregion



    }
}