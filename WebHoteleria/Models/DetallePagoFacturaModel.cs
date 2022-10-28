using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class DetallePagoFacturaModel
    {

        public string IdTipoPago { get; set; }
        public string NombreTipoPago { get; set; }
        public string MontoPago { get; set; }
        public string Observacion { get; set; }

    }
}