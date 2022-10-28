using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebHoteleria.Controllers
{
    public class PlanningController : Controller
    {
        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        //#region Planning de Reservas

        //[HttpGet]
        //[AutorizarUsuario("Planning", "Index")]
        //public ActionResult Index()
        //{
        //    //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
        //    string sesIdAño = Convert.ToString(Session["sesionPlanningIdAño"]);
        //    PlanningHotel planningHotel = new PlanningHotel();
        //    ViewBag.ddlAnhos = new SelectList(planningHotel.ObtenerListadoAños(), "Id", "Descripcion", sesIdAño);
        //    string sesIdMes = Convert.ToString(Session["sesionPlanningIdMes"]);
        //    ViewBag.ddlMeses = new SelectList(planningHotel.ObtenerListadoMeses(), "Id", "Descripcion", sesIdMes);

        //    List<PlanningModel> listaPlanning = new List<PlanningModel>();
        //    try
        //    {
        //        //CONSTRUIMOS EL PLANEL DEPENDIENDO DEL AÑO Y EL MES SELECCIONADO
        //        var listaHabitaciones = db.habitaciones.Where(h => h.estado == true).ToList();
        //        int año = sesIdAño == "" ? DateTime.Now.Year: Convert.ToInt32(sesIdAño);
        //        int mes = sesIdMes == "" ? DateTime.Now.Month : Convert.ToInt32(sesIdMes);
        //        string mesFormateado = mes <= 9 ? "0" + mes.ToString() : mes.ToString();
        //        string strFechaInicial = "01-" + mesFormateado + "-" + año.ToString();
        //        DateTime fechaInicial = Convert.ToDateTime(strFechaInicial);
        //        var ultimoDiaDelMes = DateTime.DaysInMonth(año, mes);
        //        string strFechaFinal = ultimoDiaDelMes.ToString() + "-" + mesFormateado + "-" + año.ToString();
        //        DateTime fechaFinal = Convert.ToDateTime(strFechaFinal);

        //        var listaReservas = db.reservas.Where(r => r.estado == true && r.fecha_desde >= fechaInicial && r.fecha_desde <= fechaFinal).ToList();

        //        //ARMAMOS LAS COLUMNAS DEL PLANNING
        //        List<Int32> columnaDias = new List<int>();
        //        for (int i = 0; i < ultimoDiaDelMes; i++)
        //        {
        //            columnaDias.Add(i + 1);
        //        }
        //        ViewBag.ListaColumnas = columnaDias;

        //        foreach (var item in listaHabitaciones)
        //        {
        //            if(listaReservas.Count > 0)
        //            {
        //                PlanningModel carga = new PlanningModel();
        //                carga.NroHabitacion = item.habitaciones_tipos.abreviatura + " - " + item.numero;
        //                carga.Dia1 = ObtenerSiEstaReservado(listaReservas, "01" + "-" + mesFormateado + "-" + año.ToString(), item.id);
        //                carga.Dia2 = ObtenerSiEstaReservado(listaReservas, "02" + "-" + mesFormateado + "-" + año.ToString(), item.id);
        //                carga.Dia3 = ObtenerSiEstaReservado(listaReservas, "03" + "-" + mesFormateado + "-" + año.ToString(), item.id);
        //                carga.Dia4 = ObtenerSiEstaReservado(listaReservas, "04" + "-" + mesFormateado + "-" + año.ToString(), item.id);
        //                carga.Dia5 = ObtenerSiEstaReservado(listaReservas, "05" + "-" + mesFormateado + "-" + año.ToString(), item.id);
        //                carga.Dia6 = ObtenerSiEstaReservado(listaReservas, "06" + "-" + mesFormateado + "-" + año.ToString(), item.id);
        //                carga.Dia7 = ObtenerSiEstaReservado(listaReservas, "07" + "-" + mesFormateado + "-" + año.ToString(), item.id);
        //                carga.Dia8 = ObtenerSiEstaReservado(listaReservas, "08" + "-" + mesFormateado + "-" + año.ToString(), item.id);
        //                carga.Dia9 = ObtenerSiEstaReservado(listaReservas, "09" + "-" + mesFormateado + "-" + año.ToString(), item.id);
        //                carga.Dia10 = ObtenerSiEstaReservado(listaReservas, "10" + "-" + mesFormateado + "-" + año.ToString(), item.id);
        //                listaPlanning.Add(carga);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.msg = "Ocurrio un error al cargar el listado de planning";
        //    }
        //    return View(listaPlanning);
        //}

        //#endregion

        //private string ObtenerSiEstaReservado(List<reservas> listaReservas, string strFecha, int idHabitacion)
        //{
        //    string retorno = "";
        //    DateTime fecha = Convert.ToDateTime(strFecha);
        //    var count = listaReservas.Where(r => r.fecha_desde <= fecha && r.fecha_hasta >= fecha && r.tarifas_detalles.habitaciones.id == idHabitacion).ToList().Count;
        //    if(count > 0)
        //    {
        //        retorno = "R";
        //    }
        //    return retorno;
        //}

    }
}