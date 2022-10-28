using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class ClienteModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTipoDocumento { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NroDocumento { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string RazonSocial { get; set; }
        public int? IdPais { get; set; }
        public int? IdDepartamento { get; set; }
        public int? IdCiudad { get; set; }
        public int? IdBarrio { get; set; }
        public string Direccion { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "El campo e-mail no es una dirección de correo electrónico válida")]
        public string EmailPrincipal { get; set; }
        public string NroTelefonoPrincipal { get; set; }
        public Nullable<DateTime> FechaNacimiento { get; set; }
        public Nullable<int> IdProfesion { get; set; }
        public Nullable<int> IdEstadoCivil { get; set; }
        public string Genero { get; set; }
        public bool? Estado { get; set; }


        public string NombreTipoDocumento { get; set; }
        public string NombreCompleto { get; set; }
        public string NombrePais { get; set; }
        public string NombreDepartamento { get; set; }
        public string NombreCiudad { get; set; }
        public string NombreBarrio { get; set; }
        public string StrFechaNacimiento { get; set; }
        public int? IdTipoContacto { get; set; }
        public string NombreProfesion { get; set; }
        public string NombreEstadoCivil { get; set; }
        public string EstadoDescrip { get; set; }

        public bool Masculino { get; set; }
        public bool Femenino { get; set; }
        public bool Otro { get; set; }

    }
}