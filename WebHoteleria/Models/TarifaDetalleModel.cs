using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class TarifaDetalleModel
    {

        public int Id { get; set; }
        public int? IdTarifa { get; set; }
        public int? IdHabitacion { get; set; }
        public int? IdServicioHabitacion { get; set; }
        public decimal? Precio { get; set; }
        public bool Estado { get; set; }






        public int? IdTipoHabitacion { get; set; }
        public string NombreTipoTarifa { get; set; }
        public string NombreTarifa { get; set; }
        public string NombreTipoHabitacion { get; set; }
        public string NombreHabitacion { get; set; }
        public string NombreServicioHabitacion { get; set; }
        public string StrPrecio { get; set; }
        public string EstadoDescrip { get; set; }

    }
}