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
    
    public partial class timbrados
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public timbrados()
        {
            this.timbrados_rangos = new HashSet<timbrados_rangos>();
        }
    
        public int id { get; set; }
        public Nullable<int> id_timbrado_formato { get; set; }
        public Nullable<int> id_timbrado_tipo_documento { get; set; }
        public string nro_timbrado { get; set; }
        public Nullable<System.DateTime> vigencia_desde { get; set; }
        public Nullable<System.DateTime> vigencia_hasta { get; set; }
        public Nullable<bool> estado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<timbrados_rangos> timbrados_rangos { get; set; }
        public virtual timbrados_formatos timbrados_formatos { get; set; }
        public virtual timbrados_tipos_documentos timbrados_tipos_documentos { get; set; }
    }
}