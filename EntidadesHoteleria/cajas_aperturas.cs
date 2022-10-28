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
    
    public partial class cajas_aperturas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public cajas_aperturas()
        {
            this.cajas_ajustes = new HashSet<cajas_ajustes>();
            this.cajas_arqueos = new HashSet<cajas_arqueos>();
            this.cajas_composiciones = new HashSet<cajas_composiciones>();
        }
    
        public int id { get; set; }
        public Nullable<int> id_caja { get; set; }
        public Nullable<int> id_usuario { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<System.DateTime> fecha_apertura { get; set; }
        public string nombre_apertura { get; set; }
        public Nullable<decimal> saldo_efectivo_inicial { get; set; }
        public Nullable<decimal> saldo_caja_anterior { get; set; }
        public Nullable<decimal> saldo_efectivo_anterior { get; set; }
        public Nullable<System.DateTime> fecha_cierre { get; set; }
        public Nullable<decimal> saldo_caja_cierre { get; set; }
        public Nullable<decimal> saldo_efectivo_cierre { get; set; }
        public Nullable<decimal> saldo_faltante_cierre { get; set; }
        public Nullable<decimal> saldo_sobrante_cierre { get; set; }
        public Nullable<bool> caja_abierta { get; set; }
        public Nullable<bool> estado { get; set; }
    
        public virtual cajas cajas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cajas_ajustes> cajas_ajustes { get; set; }
        public virtual usuarios usuarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cajas_arqueos> cajas_arqueos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cajas_composiciones> cajas_composiciones { get; set; }
    }
}