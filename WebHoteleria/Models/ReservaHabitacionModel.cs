using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class ReservaHabitacionModel
    {

        #region Propiedades

        public int Id { get; set; }
        public int? IdCliente { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? IdUsuarioApertura { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? IdTipoHabitacion { get; set; }
        public int? IdHabitacion { get; set; }
        public int? IdReservaEstado { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? CantidadAdultos { get; set; }
        public int? CantidadMenores { get; set; }
        public int? IdUsuarioCierre { get; set; }
        public DateTime? FechaCierre { get; set; }
        public bool? Estado { get; set; }


        public string NombreUsuarioApertura { get; set; }
        public string StrFechaDesde { get; set; }
        public string StrFechaHasta { get; set; }
        public string NombreTipoHabitacion { get; set; }
        public string NombreHabitacion { get; set; }
        public string NombreEstadoReserva { get; set; }
        public string StrCantidadAdultos { get; set; }
        public string StrCantidadMenores { get; set; }
        public string NombreUsuarioCierre { get; set; }
        public string NroDocumentoCliente { get; set; }
        public string NombreCliente { get; set; }
        public string NroDocumento { get; set; }
        public string NombreCompleto { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion



    }
}