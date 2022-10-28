using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class CargarACuentaModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string Observacion { get; set; }
        public int? IdMesa { get; set; }
        public string StrTotalBruto { get; set; }

    }
}