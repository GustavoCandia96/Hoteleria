using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class ReporteVentaRestauranteModel
    {

        public string Producto { get; set; }
        public string StrCantidad { get; set; }
        public string StrSubTotal { get; set; }

        public decimal? Cantidad { get; set; }
        public decimal? SubTotal { get; set; }

    }
}