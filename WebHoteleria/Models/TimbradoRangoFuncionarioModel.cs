using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebHoteleria.Class;

namespace WebHoteleria.Models
{
    public class TimbradoRangoFuncionarioModel
    {

        #region Propiedades

        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTimbradoRango { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdFuncionario { get; set; }
        public DateTime? Fecha { get; set; }
        public string Observaciones { get; set; }
        public bool? Estado { get; set; }







        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTipoRango { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdTimbradoTipoDocumento { get; set; }
        public string NombreSucursal { get; set; }
        public string NombreFuncionario { get; set; }
        public string NroTimbrado { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }
        public decimal? Desde { get; set; }
        public decimal? Hasta { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? IdSucursal { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string StrFecha { get; set; }
        public string EstadoDescrip { get; set; }

        #endregion

        #region Metodos

        //OBTIENE EL LISTADO DE TODOS LOS FUNCIONARIOS CON RANGO DE TIMBRADO ACTIVOS POR SUCURSAL
        public List<ListaDinamica> ListadoFuncionariosTimbrados(int? sucursalId)
        {
            List<ListaDinamica> lista = new List<ListaDinamica>();
            using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
            {
                var listaFuncionarios = context.funcionarios.Where(f => f.id_sucursal == sucursalId && f.estado == true).ToList();
                foreach (var item in listaFuncionarios)
                {
                    var timbradoFuncionario = context.timbrados_rangos_funcionarios.Where(trf => trf.estado == true && trf.id_funcionario == item.id).FirstOrDefault();
                    if (timbradoFuncionario != null)
                    {
                        ListaDinamica carga = new ListaDinamica();
                        carga.Id = item.id;
                        carga.Nombre = item.nombre + " " + item.apellido;
                        lista.Add(carga);
                    }
                }
            }
            return lista;
        }

        //OBTIENE EL LISTA DE RANGOS DISPONIBLES POR TIPO DE RANGO, SUCURSAL,TIPO DOCUMENTO DEL TIMBRADO Y FUNCIONARIO
        public List<ListaDinamica> ListadoRangosDisponiblesFuncionarios(int? timbradoTipoRangoId, int? sucursalId, int? timbradoTipoDocumentoId, int? funcionarioId)
        {
            List<ListaDinamica> lista = new List<ListaDinamica>();
            using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
            {
                lista = (from tr in context.timbrados_rangos
                         join t in context.timbrados on tr.id_timbrado equals t.id
                         join trf in context.timbrados_rangos_funcionarios on tr.id equals trf.id_timbrado_rango
                         where tr.id_timbrado_tipo_rango == timbradoTipoRangoId && tr.id_sucursal == sucursalId && tr.estado == true && t.id_timbrado_tipo_documento == timbradoTipoDocumentoId && t.estado == true && trf.id_funcionario == funcionarioId
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