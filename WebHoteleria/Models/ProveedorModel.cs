using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebHoteleria.Class;

namespace WebHoteleria.Models
{
    public class ProveedorModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTipoDocumento { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdActividadEconomica { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
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
        public Nullable<bool> Estado { get; set; }






        public int? IdTipoContacto { get; set; }
        public string NombrePais { get; set; }
        public string NombreDepartamento { get; set; }
        public string NombreCiudad { get; set; }
        public string NombreBarrio { get; set; }
        public string NombreCompleto { get; set; }
        public string NombreTipoDocumento { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

        #region Metodos

        //OBTIENE EL LISTADO DE TODOS LOS PROVEEDORES ACTIVOS
        public List<ListaDinamica> ListadoProveedores()
        {
            List<ListaDinamica> lista = new List<ListaDinamica>();
            using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
            {
                lista = (from p in context.proveedores
                         where p.estado == true
                         select new ListaDinamica
                         {
                             Id = p.id,
                             Nombre = p.id_tipo_documento == 1 ? p.nombre + " " + p.apellido : p.razon_social
                         }).ToList();
            }
            return lista;
        }

        #endregion


    }
}