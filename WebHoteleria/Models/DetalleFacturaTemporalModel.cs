using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class DetalleFacturaTemporalModel
    {

        public string Cantidad { get; set; }
        public string Descripcion { get; set; }
        public string PrecioUnitario { get; set; }
        public string SubTotal { get; set; }
        public string Excenta { get; set; }
        public string Iva5 { get; set; }
        public string Iva10 { get; set; }

    }
}