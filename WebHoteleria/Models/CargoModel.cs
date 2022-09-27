using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebHoteleria.Class;

namespace WebHoteleria.Models
{
    public class CargoModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdArea { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NombreCargo { get; set; }
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string Descripcion { get; set; }
        public bool? Estado { get; set; }


        public string NombreArea { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

        #region Metodos

        /*
         * DEVUELVE UNA LISTA DE CARGOS FILTRADO POR AREA
         */
        public List<ListaDinamica> ListadoCargos(int? areaId)
        {
            List<ListaDinamica> lista = new List<ListaDinamica>();
            using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
            {
                lista = (from c in context.cargos
                         where c.id_area == areaId && c.estado == true
                         select new ListaDinamica
                         {
                             Id = c.id,
                             Nombre = c.nombre_cargo
                         }).ToList();
            }
            return lista;
        }

        #endregion

    }
}