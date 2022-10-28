using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class CajaAjusteModel
    {

        #region Propiedades

        public int Id { get; set; }
        public int? IdCaja { get; set; }
        public int? IdCajaApertura { get; set; }
        public int? IdUsuario { get; set; }
        public DateTime? Fecha { get; set; }
        public bool Faltante { get; set; }
        public decimal? MontoAjuste { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Tiene que ingresar entre {2} a {1} caracteres en el campo")]
        public string Justificacion { get; set; }
        public bool Estado { get; set; }



        public string EstadoDescrip { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrMontoAjuste { get; set; }
        public string NombreUsuarioCaja { get; set; }
        public int? IdSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public string StrFecha { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string NombreCaja { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string NombreApertura { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTipoAjuste { get; set; }
        public string NombreTipoAjuste { get; set; }
        public bool HabilitadoEdicion { get; set; }

        #endregion

        #region Metodos

        public List<CajaTipoAjusteModel> ListadoTiposAjustesCaja()
        {
            List<CajaTipoAjusteModel> ListaRetorno = new List<CajaTipoAjusteModel>();

            ListaRetorno.Add(new CajaTipoAjusteModel()
            {
                Id = "1",
                Descripcion = "Faltante"
            });

            ListaRetorno.Add(new CajaTipoAjusteModel()
            {
                Id = "2",
                Descripcion = "Sobrante"
            });

            return ListaRetorno;
        }

        #endregion

    }
}