using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class ProductoLoteModel
    {

        #region Propiedades

        public int Id { get; set; }
        public int? IdProducto { get; set; }
        public decimal? Cantidad { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdSucursal { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdDeposito { get; set; }
        public string Ubicacion { get; set; }
        [StringLength(300, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Observacion { get; set; }
        public bool Estado { get; set; }




        public string NombreProducto { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrCantidad { get; set; }
        public decimal? Precio { get; set; }
        public string StrPrecio { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

    }
}