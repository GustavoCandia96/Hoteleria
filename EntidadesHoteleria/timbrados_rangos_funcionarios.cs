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
    
    public partial class timbrados_rangos_funcionarios
    {
        public int id { get; set; }
        public Nullable<int> id_timbrado_rango { get; set; }
        public Nullable<int> id_funcionario { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public string observacion { get; set; }
        public Nullable<bool> estado { get; set; }
    
        public virtual funcionarios funcionarios { get; set; }
        public virtual timbrados_rangos timbrados_rangos { get; set; }
    }
}