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
    
    public partial class compras_productos_detalles
    {
        public int id { get; set; }
        public Nullable<int> id_sucursal { get; set; }
        public Nullable<int> id_deposito { get; set; }
        public Nullable<int> id_compra_producto { get; set; }
        public Nullable<int> id_producto { get; set; }
        public Nullable<decimal> cantidad { get; set; }
        public Nullable<decimal> precio_unitario { get; set; }
        public Nullable<decimal> gravada_10 { get; set; }
        public Nullable<decimal> gravada_5 { get; set; }
        public Nullable<decimal> gravada_excenta { get; set; }
        public Nullable<bool> estado { get; set; }
    
        public virtual compras_productos compras_productos { get; set; }
        public virtual depositos depositos { get; set; }
        public virtual productos productos { get; set; }
        public virtual sucursales sucursales { get; set; }
    }
}