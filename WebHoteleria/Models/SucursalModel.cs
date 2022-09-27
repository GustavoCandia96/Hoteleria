using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class SucursalModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NombreSucursal { get; set; }
        public Nullable<int> IdPais { get; set; }
        public Nullable<int> IdDepartamento { get; set; }
        public Nullable<int> IdCiudad { get; set; }
        public Nullable<int> IdBarrio { get; set; }
        public string Direccion { get; set; }
        public bool CasaMatriz { get; set; }
        public Nullable<bool> Estado { get; set; }




        public string NombrePais { get; set; }
        public string NombreDepartamento { get; set; }
        public string NombreCiudad { get; set; }
        public string NombreBarrio { get; set; }
        public string EstadoDescrip { get; set; }


    }
}