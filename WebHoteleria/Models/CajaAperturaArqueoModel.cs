using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class CajaAperturaArqueoModel
    {

        #region Propiedades

        public int? IdCajaApertura { get; set; }
        public bool ArqueoFinal { get; set; }
        public string StrFechaApertura { get; set; }
        public string Responsable { get; set; }
        public string NombreCajaApertura { get; set; }
        public string StrSaldoAnterior { get; set; }
        public string StrSaldoEfectivo { get; set; }
        public string StrSaldoCaja { get; set; }
        public string StrSaldoEfectivoInicial { get; set; }

        #endregion

    }
}