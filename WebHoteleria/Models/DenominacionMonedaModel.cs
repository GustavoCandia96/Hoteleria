using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class DenominacionMonedaModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTipoDenominacion { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string Denominacion { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdMoneda { get; set; }
        public int? Orden { get; set; }
        public bool Estado { get; set; }


        public string NombreTipoDenominacion { get; set; }
        public string NombreMoneda { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrOrden { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

    }
}