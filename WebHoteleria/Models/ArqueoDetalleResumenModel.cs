using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class ArqueoDetalleResumenModel
    {

        #region Propiedades

        public string Id { get; set; }
        public string Concepto { get; set; }
        public decimal? Ingreso { get; set; }
        public string StrIngreso { get; set; }
        public decimal? Egreso { get; set; }
        public string StrEgreso { get; set; }
        public decimal? Saldo { get; set; }
        public string StrSaldo { get; set; }

        #endregion

    }
}