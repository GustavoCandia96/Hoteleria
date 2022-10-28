using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class CargaCuentaAnulacionModel
    {

        #region Propiedades

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int IdCargaCuenta { get; set; }
        public string StrFechaEmision { get; set; }
        public string Descripcion { get; set; }
        public string StrTotalNeto { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Tiene que ingresar {1} caracteres en el campo")]
        public string Observacion { get; set; }

        #endregion

    }
}