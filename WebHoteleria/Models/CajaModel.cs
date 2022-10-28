using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class CajaModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public Nullable<int> IdSucursal { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public Nullable<int> IdMoneda { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public Nullable<int> IdUsuario { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string Denominacion { get; set; }
        public bool Abierto { get; set; }
        public Nullable<decimal> SaldoCaja { get; set; }
        public Nullable<decimal> SaldoEfectivo { get; set; }
        public Nullable<decimal> Sobrante { get; set; }
        public Nullable<decimal> Faltante { get; set; }
        public Nullable<bool> Estado { get; set; }


        public string NombreSucursal { get; set; }
        public string NombreMoneda { get; set; }
        public string NombreFuncionario { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

    }
}