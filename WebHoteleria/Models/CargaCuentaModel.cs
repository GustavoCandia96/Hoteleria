using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class CargaCuentaModel
    {

        #region Propiedades

        public int Id { get; set; }
        public int? IdConsumision { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? Fecha { get; set; }
        public int? IdUsuario { get; set; }
        public decimal? TotalConsumision { get; set; }
        public string Observacion { get; set; }
        public bool? Anulado { get; set; }
        public bool? Estado { get; set; }


        public string EstadoDescrip { get; set; }

        #endregion

    }
}