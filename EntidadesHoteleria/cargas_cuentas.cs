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
    
    public partial class cargas_cuentas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public cargas_cuentas()
        {
            this.cargas_cuentas_anulaciones = new HashSet<cargas_cuentas_anulaciones>();
            this.cargas_cuentas_detalles = new HashSet<cargas_cuentas_detalles>();
        }
    
        public int id { get; set; }
        public Nullable<int> id_consumision { get; set; }
        public Nullable<System.DateTime> fecha_alta { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<int> id_usuario { get; set; }
        public Nullable<decimal> total_consumision { get; set; }
        public string observacion { get; set; }
        public Nullable<bool> anulado { get; set; }
        public Nullable<bool> estado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cargas_cuentas_anulaciones> cargas_cuentas_anulaciones { get; set; }
        public virtual consumisiones consumisiones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cargas_cuentas_detalles> cargas_cuentas_detalles { get; set; }
        public virtual usuarios usuarios { get; set; }
    }
}
