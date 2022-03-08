using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class RecuperarPasswordModel
    {

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string Usuario { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "El campo e-mail no es una dirección de correo electrónico válida")]
        public string Email { get; set; }


    }
}