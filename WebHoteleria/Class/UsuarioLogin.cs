using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Class
{
    public class UsuarioLogin
    {

        #region Propiedades

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Usuario { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Clave { get; set; }

        #endregion

    }
}