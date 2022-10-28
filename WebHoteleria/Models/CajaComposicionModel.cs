using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class CajaComposicionModel
    {

        #region Propiedades

        public int Id { get; set; }
        public int? IdCaja { get; set; }
        public int? IdCajaApertura { get; set; }
        public int? IdUsuario { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal? TotalBillete { get; set; }
        public decimal? TotalMoneda { get; set; }
        public bool Estado { get; set; }


        public string NombreCaja { get; set; }
        public string NombreApertura { get; set; }
        public string NombreUsuarioCaja { get; set; }
        public string NombreUsuarioSesion { get; set; }
        public string StrFecha { get; set; }
        public string StrTotalBillete { get; set; }
        public string StrTotalMoneda { get; set; }
        public string StrTotalEfectivo { get; set; }
        public string NombreSucursal { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

    }
}