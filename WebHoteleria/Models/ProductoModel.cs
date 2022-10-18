using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class ProductoModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string NombreProducto { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdCategoria { get; set; }
        public int? IdMarca { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdUnidad { get; set; }
        public int? IdDeposito { get; set; }
        public decimal? PrecioVenta { get; set; }
        public decimal? PrecioCosto { get; set; }
        public decimal? Stock { get; set; }
        public string Ubicacion { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTipoIva { get; set; }
        public bool Estado { get; set; }






        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrPrecioVenta { get; set; }
        public string StrStock { get; set; }
        public int? IdSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public string NombreCategoria { get; set; }
        public string NombreMarca { get; set; }
        public string NombreUnidad { get; set; }
        public string NombreDeposito { get; set; }
        public string NombreTipoIva { get; set; }
        public string ColorFila { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

        #region Metodos

        public decimal CalcularPromedioCostoProducto(hoteleria_erp_dbEntities context, int idProducto)
        {
            decimal promedio = 0;
            //VEMOS LA FORMA DE CALCULAR EL PROMEDIO
            var listaComProDet = context.compras_productos_detalles.Where(cpd => cpd.id_producto == idProducto && cpd.estado != null).ToList();
            decimal totalProducto = 0;
            decimal totalSubTotal = 0;
            foreach (var item in listaComProDet)
            {
                totalProducto += item.cantidad.Value;
                totalSubTotal += item.cantidad.Value * item.precio_unitario.Value;
            }

            if (totalProducto != 0)
            {
                promedio = totalSubTotal / totalProducto;
                promedio = Math.Round(promedio);
            }

            return promedio;
        }

        #endregion


    }
}