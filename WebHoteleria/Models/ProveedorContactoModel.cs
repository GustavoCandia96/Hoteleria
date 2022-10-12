using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class ProveedorContactoModel
    {

        public int Id { get; set; }
        public Nullable<int> IdProveedor { get; set; }
        public Nullable<int> IdTipoContacto { get; set; }
        public string Contacto { get; set; }

        public string NombreTipoContacto { get; set; }


    }
}