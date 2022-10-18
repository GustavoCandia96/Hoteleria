using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class ProductoAjusteLoteModel
    {

        #region Propiedades

        public int Id { get; set; }
        public DateTime? Fecha { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdProductoLote { get; set; }
        public decimal? Cantidad { get; set; }
        public string Observacion { get; set; }
        public bool IngresoLote { get; set; }
        public Nullable<bool> Estado { get; set; }


        public string StrFecha { get; set; }
        public string NombreUsuario { get; set; }
        public int? IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

    }
}