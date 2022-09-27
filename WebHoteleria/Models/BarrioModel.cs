using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebHoteleria.Class;

namespace WebHoteleria.Models
{
    public class BarrioModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdCiudad { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NombreBarrio { get; set; }
        public bool? Estado { get; set; }


        public string NombreCiudad { get; set; }
        public string NombreDepartamento { get; set; }
        public string NombrePais { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdDepartamento { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdPais { get; set; }
        public bool Barrio { get; set; }
        public bool Localidad { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

        #region Metodos

        /*
         * DEVUELVE UNA LISTA DE BARRIOS/LOCALIDADES FILTRADO POR CIUDAD
         */
        public List<ListaDinamica> ListadoBarrios(int? ciudadId)
        {
            List<ListaDinamica> lista = new List<ListaDinamica>();
            using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
            {
                lista = (from b in context.barrios
                         where b.id_ciudad == ciudadId && b.estado == true
                         select new ListaDinamica
                         {
                             Id = b.id,
                             Nombre = b.nombre_barrio
                         }).ToList();
            }
            return lista;
        }

        #endregion


    }
}