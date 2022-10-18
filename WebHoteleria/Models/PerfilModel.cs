using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class PerfilModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NombrePerfil { get; set; }
        public bool? Estado { get; set; }


        public Nullable<int> CantidadPermisos { get; set; }
        public string EstadoDescrip { get; set; }

    }
}