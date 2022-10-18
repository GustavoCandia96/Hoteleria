using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebHoteleria.Class;

namespace WebHoteleria.Models
{
    public class ProveedorSucursalModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdProveedor { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NombreSucursal { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdPais { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdDepartamento { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdCiudad { get; set; }
        public int? IdBarrio { get; set; }
        public string Direccion { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "El campo e-mail no es una dirección de correo electrónico válida")]
        public string EmailPrincipal { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NroTelefonoPrincipal { get; set; }
        public bool CasaMatriz { get; set; }
        public bool? Estado { get; set; }









        public string NombrePais { get; set; }
        public string NombreDepartamento { get; set; }
        public string NombreCiudad { get; set; }
        public string NombreBarrio { get; set; }
        public string NombreProveedor { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

        #region Metodos

        //OBTIENE EL LISTADO DE SUCURSALES DEL PROVEEDOR SELECCIONADO
        public List<ListaDinamica> ListadoProveedoresSucursales(int idProveedor)
        {
            List<ListaDinamica> lista = new List<ListaDinamica>();
            using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
            {
                lista = (from ps in context.proveedores_sucursales
                         where ps.estado == true && ps.id_proveedor == idProveedor
                         select new ListaDinamica
                         {
                             Id = ps.id,
                             Nombre = ps.nombre_sucursal
                         }).ToList();
            }
            return lista;
        }

        #endregion

    }
}