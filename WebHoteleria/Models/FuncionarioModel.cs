using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebHoteleria.Class;

namespace WebHoteleria.Models
{
    public class FuncionarioModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTipoDocumento { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NroDocumento { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdArea { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdCargo { get; set; }
        public string Direccion { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdSucursal { get; set; }
        public bool? Estado { get; set; }

        public string NombreTipoDocumento { get; set; }
        public string NombreArea { get; set; }
        public string NombreCargo { get; set; }
        public string NombreCompleto { get; set; }
        public string NombreSucursal { get; set; }
        public string EstadoDescrip { get; set; }

        #region Metodos

        /*
         * OBTIENE EL LISTADO DE TODOS LOS FUNCIONARIOS ACTIVOS DE LA SUCURSAL
         */
        public List<ListaDinamica> ListadoFuncionarios(int? sucursalId)
        {
            List<ListaDinamica> lista = new List<ListaDinamica>();
            using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
            {
                lista = (from f in context.funcionarios
                         join u in context.usuarios on f.id equals u.id_funcionario
                         where f.id_sucursal == sucursalId && f.estado == true
                         select new ListaDinamica
                         {
                             Id = f.id,
                             Nombre = f.nombre + " " + f.apellido + " - (" + u.usuario + ")"
                         }).ToList();
            }
            return lista;
        }

        #endregion



    }
}