//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntidadesHoteleria
{
    using System;
    using System.Collections.Generic;
    
    public partial class facturaciones_detalles
    {
        public int id { get; set; }
        public Nullable<int> id_facturacion { get; set; }
        public Nullable<decimal> cantidad { get; set; }
        public Nullable<int> id_producto { get; set; }
        public Nullable<int> id_producto_lote { get; set; }
        public Nullable<decimal> precio_unitario { get; set; }
        public Nullable<decimal> iva_excenta { get; set; }
        public Nullable<decimal> iva_5 { get; set; }
        public Nullable<decimal> iva_10 { get; set; }
        public Nullable<decimal> gravada_excenta { get; set; }
        public Nullable<decimal> gravada_5 { get; set; }
        public Nullable<decimal> gravada_10 { get; set; }
        public Nullable<bool> estado { get; set; }
    
        public virtual facturaciones facturaciones { get; set; }
        public virtual productos productos { get; set; }
        public virtual productos_lotes productos_lotes { get; set; }
    }
}