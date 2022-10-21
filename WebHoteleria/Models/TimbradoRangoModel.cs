using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebHoteleria.Class;

namespace WebHoteleria.Models
{
    public class TimbradoRangoModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public Nullable<int> IdTimbradoTipoRango { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public Nullable<int> IdTimbrado { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public Nullable<int> IdSucursal { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Tiene que ingresar {1} caracteres en el campo")]
        public string CodigoSucursal { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Tiene que ingresar {1} caracteres en el campo")]
        public string PuntoExpedicion { get; set; }
        public Nullable<int> CantidadCeros { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "Por favor ingrese un número entero válido")]
        public Nullable<int> Desde { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "Por favor ingrese un número entero válido")]
        public Nullable<int> Hasta { get; set; }
        public Nullable<int> NumeracionActual { get; set; }
        public Nullable<bool> Utilizado { get; set; }
        public Nullable<int> CantidadUsada { get; set; }
        public Nullable<bool> Estado { get; set; }





        public string NombreTimbradoTipoDocumento { get; set; }
        public string TipoRango { get; set; }
        public string NroTimbrado { get; set; }
        public string NombreSucursal { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public Nullable<int> IdTimbradoFormato { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public Nullable<int> IdTimbradoTipoDocumento { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

        #region Metodos

        //OBTIENE EL LISTA DE RANGOS DISPONIBLES POR TIPO DE RANGO, SUCURSAL Y TIPO DOCUMENTO DEL TIMBRADO
        public List<ListaDinamica> ListadoRangosDisponibles(int? timbradoTipoRangoId, int? sucursalId, int? timbradoTipoDocumentoId)
        {
            List<ListaDinamica> lista = new List<ListaDinamica>();
            using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
            {
                lista = (from tr in context.timbrados_rangos
                         join t in context.timbrados on tr.id_timbrado equals t.id
                         where tr.id_timbrado_tipo_rango == timbradoTipoRangoId && tr.id_sucursal == sucursalId && tr.estado == true && t.id_timbrado_tipo_documento == timbradoTipoDocumentoId && t.estado == true
                         orderby tr.desde
                         select new ListaDinamica
                         {
                             Id = tr.id,
                             Nombre = tr.desde + " - " + tr.hasta
                         }).ToList();
            }
            return lista;
        }


        //OBTIENE EL LISTA DE RANGOS DISPONIBLES DE UN TIMBRADO ESPECIFICAMENTE DE UN FUNCIONARIO
        public List<ListaDinamica> ListadoRangosDisponiblesFuncionario(int? idFuncionario, int? idTimbradoFormato, int? idTipoRango, int? idTimbradoTipoDocumento)
        {
            List<ListaDinamica> lista = new List<ListaDinamica>();
            using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
            {
                lista = (from trf in context.timbrados_rangos_funcionarios
                         join tr in context.timbrados_rangos on trf.id_timbrado_rango equals tr.id
                         join t in context.timbrados on tr.id_timbrado equals t.id
                         where trf.estado == true && tr.estado == true && t.estado == true && trf.id_funcionario == idFuncionario && t.id_timbrado_formato == idTimbradoFormato && tr.id_timbrado_tipo_rango == idTipoRango && t.id_timbrado_tipo_documento == idTimbradoTipoDocumento
                         orderby tr.desde
                         select new ListaDinamica
                         {
                             Id = tr.id,
                             Nombre = tr.desde + " - " + tr.hasta
                         }).ToList();
            }
            return lista;
        }


        #endregion


    }
}