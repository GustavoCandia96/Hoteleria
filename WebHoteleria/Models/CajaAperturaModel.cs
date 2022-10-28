using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class CajaAperturaModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdCaja { get; set; }
        public int? IdUsuario { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? FechaApertura { get; set; }
        public string NombreApertura { get; set; }
        public decimal? SaldoEfectivoInicial { get; set; }
        public decimal? SaldoCajaAnterior { get; set; }
        public decimal? SaldoEfectivoAnterior { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal? SaldoCajaCierre { get; set; }
        public decimal? SaldoEfectivoCierre { get; set; }
        public decimal? SaldoFaltanteCierre { get; set; }
        public decimal? SaldoSobranteCierre { get; set; }
        public bool CajaAbierta { get; set; }
        public bool Estado { get; set; }



        public string NombreCaja { get; set; }
        public string NombreUsuario { get; set; }
        public string StrFechaApertura { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrSaldoEfectivoInicial { get; set; }
        public string StrSaldoAnterior { get; set; }
        public string StrFechaCierre { get; set; }
        public string StrSaldoCajaCierre { get; set; }
        public string StrSaldoEfectivoCierre { get; set; }
        public string StrSaldoFaltanteCierre { get; set; }
        public string StrSaldoSobranteCierre { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public decimal? SaldoEfectivoActual { get; set; }
        public decimal? SaldoCajaActual { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion


    }
}