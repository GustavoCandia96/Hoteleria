using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class DetalleCompraModel
    {

        #region Propiedades

        public string IdProducto { get; set; }
        public string Producto { get; set; }
        public string Cantidad { get; set; }
        public string Precio { get; set; }
        public string SubTotal { get; set; }
        public string GravadaExcenta { get; set; }
        public string Gravada5 { get; set; }
        public string Gravada10 { get; set; }
        public string Sucursal { get; set; }
        public string Deposito { get; set; }
        public string IdSucursal { get; set; }
        public string IdDeposito { get; set; }
        public string IdIvaProducto { get; set; }

        #endregion

    }
}