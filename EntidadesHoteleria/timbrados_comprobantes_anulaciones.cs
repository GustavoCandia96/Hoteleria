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
    
    public partial class timbrados_comprobantes_anulaciones
    {
        public int id { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<int> id_usuario { get; set; }
        public Nullable<int> id_motivo_anulacion { get; set; }
        public Nullable<int> id_timbrado_rango { get; set; }
        public string nro_comprobante { get; set; }
        public string observacion { get; set; }
        public Nullable<bool> estado { get; set; }
    
        public virtual timbrados_motivos_anulaciones timbrados_motivos_anulaciones { get; set; }
        public virtual timbrados_rangos timbrados_rangos { get; set; }
    }
}
