using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class FacturacionModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdCondicionVenta { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? IdUsuario { get; set; }
        public DateTime? FechaEmision { get; set; }
        public int? IdCliente { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Ruc { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string RazonSocial { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public int? IdTimbrado { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTimbradoRango { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Tiene que ingresar {1} caracteres en el campo")]
        public string NroFactura { get; set; }
        public int? IdMotivoDescuento { get; set; }
        public decimal? TotalDescuento { get; set; }
        public decimal? TotalNeto { get; set; }
        public decimal? TotalBruto { get; set; }
        public bool Estado { get; set; }



        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string NroTimbrado { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string InicioVigencia { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string FinVigencia { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string DiasValidez { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string CantidadUtilizada { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTimbradoTipoRango { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTimbradoFormato { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdFuncionarioTimbrado { get; set; }
        public int? IdSucursal { get; set; }

        public string NombreCondicionVenta { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCliente { get; set; }
        public string NombreMotivoDescuento { get; set; }
        public string StrFechaEmision { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrTotalBruto { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrTotalNeto { get; set; }
        public string StrDescuento { get; set; }
        public int? IdMesa { get; set; }
        public string EstadoDescrip { get; set; }



        public string StrCantidad { get; set; }
        public string StrDescripcion { get; set; }
        public string StrPrecioUnitario { get; set; }
        public string StrTotalIva5 { get; set; }
        public string StrTotalIva10 { get; set; }
        public string StrTotalIva { get; set; }

        public int? IdTipoPago { get; set; }
        public string StrMontoPagado { get; set; }
        public string StrObservacion { get; set; }
        public string StrTotalPagado { get; set; }










        public string StrFechaFacturacion { get; set; }
        public string NroMeza { get; set; }
        public string NombreMozo { get; set; }
        public string RucCliente { get; set; }
        public string StrTotalFacturacion { get; set; }
        public decimal? TotalFacturacion { get; set; }

        #endregion

    }
}