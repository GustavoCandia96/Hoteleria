using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class FacturacionTemporalAnulacionModel
    {

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int IdFacturacion { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int IdTimbradoRango { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int IdMotivoAnulacion { get; set; }
        public string StrFechaEmision { get; set; }
        public string NombreCondicionVenta { get; set; }
        public string Ruc { get; set; }
        public string RazonSocial { get; set; }
        public string NroFactura { get; set; }
        public string StrTotalNeto { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Tiene que ingresar {1} caracteres en el campo")]
        public string Observacion { get; set; }

    }
}