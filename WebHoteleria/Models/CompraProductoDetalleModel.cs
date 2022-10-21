using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class CompraProductoDetalleModel
    {
        
         #region Propiedades

        public int Id { get; set; }
        public int IdCompraProducto { get; set; }
        public int IdSucursal { get; set; }
        public int IdDeposito { get; set; }
        public int IdProducto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal GravadaExcenta { get; set; }
        public decimal Gravada5 { get; set; }
        public decimal Gravada10 { get; set; }

        #endregion

    }
}