using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebHoteleria.Class;

namespace WebHoteleria.Models
{
    public class DepositoModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdSucursal { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NombreDeposito { get; set; }
        public bool? Estado { get; set; }



        public string NombreSucursal { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

        #region Metodos

        //OBTIENE EL LISTADO DE TODOS LOS DEPOSITOS ACTIVOS DE LA SUCURSAL
        public List<ListaDinamica> ListadoDepositos(int? sucursalId)
        {
            List<ListaDinamica> lista = new List<ListaDinamica>();
            using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
            {
                lista = (from d in context.depositos
                         where d.id_sucursal == sucursalId && d.estado == true
                         select new ListaDinamica
                         {
                             Id = d.id,
                             Nombre = d.nombre_deposito
                         }).ToList();
            }
            return lista;
        }

        #endregion

    }
}