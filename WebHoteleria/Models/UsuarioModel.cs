using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebHoteleria.Class;

namespace WebHoteleria.Models
{
    public class UsuarioModel
    {

        #region Propiedades

        public int Id { get; set; }
        public Nullable<int> IdFuncionario { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public Nullable<int> IdPerfil { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string Usuario { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        [DataType(DataType.Password)]
        [Compare(nameof(Clave), ErrorMessage = "La contraseña de verificación no coincide")]
        public string RepetirClave { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "El campo e-mail no es una dirección de correo electrónico válida")]
        public string Email { get; set; }
        public Nullable<bool> PrimeraVez { get; set; }
        public Nullable<System.DateTime> FechaAlta { get; set; }
        public Nullable<bool> Estado { get; set; }


        public string NombreFuncionario { get; set; }
        public string NombrePerfil { get; set; }
        public string NroDocumento { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

        #region Metodos

        /*
         * DEVUELVE UNA LISTA DE USUARIOS FUNCIONARIOS FILTRADO POR SUCURSAL
         */
        public List<ListaDinamica> ListadoUsuariosFuncionario(int? sucursalId)
        {
            List<ListaDinamica> lista = new List<ListaDinamica>();
            using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
            {
                lista = (from u in context.usuarios
                         join f in context.funcionarios on u.id_funcionario equals f.id
                         where f.id_sucursal == sucursalId && f.estado == true && u.estado == true
                         select new ListaDinamica
                         {
                             Id = u.id,
                             Nombre = f.nombre + " " + f.apellido
                         }).ToList();
            }
            return lista;
        }

        #endregion

    }
}