using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class CajaComposicionDetalleModel
    {

        #region Propiedades

        public string IdDenominacionMoneda { get; set; }
        public string IdTipoDenominacion { get; set; }
        public string Cantidad { get; set; }
        public string Monto { get; set; }
        public string SubTotal { get; set; }

        #endregion

    }
}