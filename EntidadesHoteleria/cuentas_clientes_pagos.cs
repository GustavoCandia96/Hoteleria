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
    
    public partial class cuentas_clientes_pagos
    {
        public int id { get; set; }
        public Nullable<System.DateTime> fecha_pago { get; set; }
        public Nullable<int> id_cuenta_cliente { get; set; }
        public Nullable<int> id_tipo_pago { get; set; }
        public Nullable<decimal> monto { get; set; }
        public string observacion { get; set; }
        public Nullable<bool> cenha { get; set; }
        public Nullable<bool> estado { get; set; }
    
        public virtual cuentas_clientes cuentas_clientes { get; set; }
        public virtual tipos_pagos tipos_pagos { get; set; }
    }
}
