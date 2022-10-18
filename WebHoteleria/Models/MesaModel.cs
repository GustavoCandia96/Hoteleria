using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class MesaModel
    {

        #region Propiedades

        public int Id { get; set; }
        public string Denominacion { get; set; }
        public int? IdMesaEstado { get; set; }
        public bool Estado { get; set; }

        #endregion

    }
}