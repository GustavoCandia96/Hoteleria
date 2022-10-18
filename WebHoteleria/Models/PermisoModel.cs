using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class PermisoModel
    {

        public int Id { get; set; }
        public Nullable<int> IdPerfil { get; set; }
        public Nullable<int> IdModuloOperacion { get; set; }
        public bool Habilitado { get; set; }


        public string NombrePerfil { get; set; }
        public string NombreModulo { get; set; }
        public string NombreModuloOperacion { get; set; }
        public Nullable<int> IdModulo { get; set; }

    }
}