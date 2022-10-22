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
    
    public partial class proveedores_sucursales
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public proveedores_sucursales()
        {
            this.compras_productos = new HashSet<compras_productos>();
            this.cuentas_proveedores = new HashSet<cuentas_proveedores>();
        }
    
        public int id { get; set; }
        public Nullable<int> id_proveedor { get; set; }
        public string nombre_sucursal { get; set; }
        public Nullable<int> id_pais { get; set; }
        public Nullable<int> id_departamento { get; set; }
        public Nullable<int> id_ciudad { get; set; }
        public Nullable<int> id_barrio { get; set; }
        public string direccion { get; set; }
        public string email_principal { get; set; }
        public string nro_telefono_principal { get; set; }
        public Nullable<bool> casa_matriz { get; set; }
        public Nullable<bool> estado { get; set; }
    
        public virtual barrios barrios { get; set; }
        public virtual ciudades ciudades { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<compras_productos> compras_productos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cuentas_proveedores> cuentas_proveedores { get; set; }
        public virtual departamentos departamentos { get; set; }
        public virtual paises paises { get; set; }
        public virtual proveedores proveedores { get; set; }
    }
}