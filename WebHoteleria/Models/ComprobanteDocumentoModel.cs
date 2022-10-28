using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Models
{
    public class ComprobanteDocumentoModel
    {

        #region Propiedades

        public string RazonSocialEmpresa { get; set; }
        public string RucEmpresa { get; set; }
        public string ComprobanteLinea1 { get; set; }
        public string ComprobanteLinea2 { get; set; }
        public string ComprobanteLinea3 { get; set; }
        public string ComprobanteLinea4 { get; set; }
        public string ComprobanteLinea5 { get; set; }
        public string ComprobanteLinea6 { get; set; }

        #endregion

        #region Metodos

        public ComprobanteDocumentoModel ObtenerDatosComprobante()
        {
            ComprobanteDocumentoModel retorno = new ComprobanteDocumentoModel();
            using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
            {
                retorno.RazonSocialEmpresa = context.parametros.Where(p => p.parametro == "NOMBREEMPRESA" && p.estado != null).FirstOrDefault().valor;
                retorno.RucEmpresa = context.parametros.Where(p => p.parametro == "RUCEMPRESA" && p.estado != null).FirstOrDefault().valor;
                retorno.ComprobanteLinea1 = context.parametros.Where(p => p.parametro == "COMPROBANTELINEA1" && p.estado != null).FirstOrDefault().valor;
                retorno.ComprobanteLinea2 = context.parametros.Where(p => p.parametro == "COMPROBANTELINEA2" && p.estado != null).FirstOrDefault().valor;
                retorno.ComprobanteLinea3 = context.parametros.Where(p => p.parametro == "COMPROBANTELINEA3" && p.estado != null).FirstOrDefault().valor;
                retorno.ComprobanteLinea4 = context.parametros.Where(p => p.parametro == "COMPROBANTELINEA4" && p.estado != null).FirstOrDefault().valor;
                retorno.ComprobanteLinea5 = context.parametros.Where(p => p.parametro == "COMPROBANTELINEA5" && p.estado != null).FirstOrDefault().valor;
                retorno.ComprobanteLinea6 = context.parametros.Where(p => p.parametro == "COMPROBANTELINEA6" && p.estado != null).FirstOrDefault().valor;
            }
            return retorno;
        }

        #endregion

    }
}