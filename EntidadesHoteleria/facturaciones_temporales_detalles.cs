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
    
    public partial class facturaciones_temporales_detalles
    {
        public int id { get; set; }
        public Nullable<int> id_facturacion_temporal { get; set; }
        public Nullable<decimal> cantidad { get; set; }
        public string descripcion { get; set; }
        public Nullable<decimal> precio_unitario { get; set; }
        public Nullable<decimal> iva_excenta { get; set; }
        public Nullable<decimal> iva_5 { get; set; }
        public Nullable<decimal> iva_10 { get; set; }
        public Nullable<decimal> gravada_excenta { get; set; }
        public Nullable<decimal> gravada_5 { get; set; }
        public Nullable<decimal> gravada_10 { get; set; }
        public Nullable<bool> estado { get; set; }
    
        public virtual facturaciones_temporales facturaciones_temporales { get; set; }
    }
}
