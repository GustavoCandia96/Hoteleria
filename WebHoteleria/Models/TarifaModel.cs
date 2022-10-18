using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class TarifaModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTipoTarifa { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NombreTarifa { get; set; }
        public DateTime? ValidoDesde { get; set; }
        public DateTime? ValidoHasta { get; set; }
        public Nullable<bool> Estado { get; set; }



        public string EstadoDescrip { get; set; }
        public string NombreTipoTarifa { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrValidoDesde { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrValidoHasta { get; set; }

    }
}