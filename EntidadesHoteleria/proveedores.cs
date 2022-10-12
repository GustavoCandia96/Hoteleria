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
    
    public partial class proveedores
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public proveedores()
        {
            this.proveedores_contactos = new HashSet<proveedores_contactos>();
        }
    
        public int id { get; set; }
        public Nullable<int> id_tipo_documento { get; set; }
        public Nullable<int> id_actividad_economica { get; set; }
        public string nro_documento { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string razon_social { get; set; }
        public Nullable<int> id_pais { get; set; }
        public Nullable<int> id_departamento { get; set; }
        public Nullable<int> id_ciudad { get; set; }
        public Nullable<int> id_barrio { get; set; }
        public string direccion { get; set; }
        public string email_principal { get; set; }
        public string nro_telefono_principal { get; set; }
        public Nullable<bool> estado { get; set; }
    
        public virtual actividades_economicas actividades_economicas { get; set; }
        public virtual barrios barrios { get; set; }
        public virtual ciudades ciudades { get; set; }
        public virtual departamentos departamentos { get; set; }
        public virtual paises paises { get; set; }
        public virtual tipos_documentos tipos_documentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<proveedores_contactos> proveedores_contactos { get; set; }
    }
}
