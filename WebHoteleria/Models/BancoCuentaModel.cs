using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class BancoCuentaModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public Nullable<int> IdBanco { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public Nullable<int> IdTipoCuenta { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NroCuenta { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public Nullable<int> IdMoneda { get; set; }
        public Nullable<bool> Estado { get; set; }


        public string NombreBanco { get; set; }
        public string NombreTipoCuenta { get; set; }
        public string NombreMoneda { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

    }
}