using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class HabitacionModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTipoHabitacion { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdHabitacionEstado { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Numero { get; set; }
        public bool? Estado { get; set; }

        public string NombreTipoHabitacion { get; set; }
        public string NombreHabitacionEstado { get; set; }
        public string EstadoDescrip { get; set; }

    }
}