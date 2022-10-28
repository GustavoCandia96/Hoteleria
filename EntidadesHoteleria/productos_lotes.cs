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
    
    public partial class productos_lotes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public productos_lotes()
        {
            this.productos_lotes_ajustes = new HashSet<productos_lotes_ajustes>();
            this.consumisiones_detalles = new HashSet<consumisiones_detalles>();
            this.cargas_cuentas_detalles = new HashSet<cargas_cuentas_detalles>();
            this.facturaciones_detalles = new HashSet<facturaciones_detalles>();
        }
    
        public int id { get; set; }
        public Nullable<int> id_producto { get; set; }
        public Nullable<decimal> cantidad { get; set; }
        public Nullable<int> id_sucursal { get; set; }
        public Nullable<int> id_deposito { get; set; }
        public string ubicacion { get; set; }
        public Nullable<bool> estado { get; set; }
    
        public virtual depositos depositos { get; set; }
        public virtual productos productos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<productos_lotes_ajustes> productos_lotes_ajustes { get; set; }
        public virtual sucursales sucursales { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<consumisiones_detalles> consumisiones_detalles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cargas_cuentas_detalles> cargas_cuentas_detalles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<facturaciones_detalles> facturaciones_detalles { get; set; }
    }
}
