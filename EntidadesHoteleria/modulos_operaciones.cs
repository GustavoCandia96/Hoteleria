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
    
    public partial class modulos_operaciones
    {
        public int id { get; set; }
        public Nullable<int> id_modulo { get; set; }
        public string operacion { get; set; }
        public string descripcion { get; set; }
        public Nullable<bool> estado { get; set; }
    
        public virtual modulos modulos { get; set; }
    }
}