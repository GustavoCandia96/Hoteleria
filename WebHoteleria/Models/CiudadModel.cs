﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class CiudadModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdDepartamento { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NombreCiudad { get; set; }
        public bool? Estado { get; set; }

        public string NombreDepartamento { get; set; }
        public string NombrePais { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdPais { get; set; }
        public string EstadoDescrip { get; set; }

    }
}