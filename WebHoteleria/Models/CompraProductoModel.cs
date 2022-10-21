using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class CompraProductoModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdCondicionCompra { get; set; }
        public DateTime? Fecha { get; set; }
        public int? IdUsuario { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdProveedor { get; set; }
        public int? IdProveedorSucursal { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NroFactura { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? TotalNeto { get; set; }
        public decimal? TotalBruto { get; set; }
        public decimal? SaldoFactura { get; set; }
        public decimal? TotalGravadaIva { get; set; }
        public string Observacion { get; set; }
        public int? IdSucursal { get; set; }
        public int? IdDeposito { get; set; }
        public bool Estado { get; set; }



        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrFecha { get; set; }
        public string NombreCondicionCompra { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreProveedor { get; set; }
        public string NombreProveedorSucursal { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrTotalBruto { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrTotalNeto { get; set; }
        public string StrTotalGravada { get; set; }
        public string StrDescuento { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

    }
}