using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Class
{
    public class TimbradoNumeracion
    {

        #region Metodos

        /*
         * METODO QUE DEVUELVE EL FORMATO CORRECTO DE UN NUMERO COMPROBANTE
         * RECIBE UN NUMERO Y LA CANTIDAD DE CEROS
         * DESPLAZA LA POSICION DEL CERO SI ES NECESARIO
         */
        public string ObtenerNumeracionFormato(decimal numeroActual, int cantidadCeros)
        {
            string retorno = "";
            string numero = numeroActual.ToString();
            int diferencia = cantidadCeros - numero.Length;
            string ceros = "";
            for (int i = 0; i < diferencia; i++)
            {
                ceros = ceros + "0";
            }
            retorno = ceros + numero;
            return retorno;
        }

        /*
         * DEVUELVE SI LA FECHA DEL COMPROBANTE ES VALIDO O NO (DIAS FALTANTES O DIAS EXPIRADO)
         * RECIBE UNA FECHA Y COMPARA CON LA FECHA ACTUAL DEL SISTEMA
         */
        public string ObtenerValidezTimbrado(DateTime? fechaHasta)
        {
            string retorno = string.Empty;
            var fechadevolver = (fechaHasta.Value - DateTime.Now).TotalDays;
            string relleno = "";
            relleno = fechadevolver >= 0 ? "faltante" : "expirado";
            int dias = Convert.ToInt32(fechadevolver);
            string strdias;
            strdias = "días";
            if (dias == 1) { strdias = "dia"; }
            retorno = "" + dias + " " + strdias + " " + relleno;
            return retorno;
        }

        /*
         * METODO QUE DEVUELVE LA VERIFICACION DE UN NUMERO Y TIPO DE COMPROBANTE SI ESTA DISPONIBLE O NO
         * LOS MOTIVOS PUEDEN SER FUERA DE RANGO, HA SIDO ANULADO, HA SIDO UTILIZADO
         */
        public string VerificarNroComprobante(hoteleria_erp_dbEntities context, int idTipoDocumento, timbrados_rangos timbradoRango, decimal numeroActual, string nroComprobante)
        {
            string retorno = string.Empty;

            //VERIFICAR NUMERO DE REMISION SI ESTA FUERA DEL RANGO DESDE Y HASTA DEL TIMBRADO
            bool resultado = numeroActual >= timbradoRango.desde && numeroActual <= timbradoRango.hasta;
            if (resultado == false)
            {
                retorno = " esta fuera del invervalo de rango del timbrado seleccionado.";
            }
            else
            {
                //VERIFICAR SI LA NUMERACIÓN YA HA SIDO ANULADO
                var comprobanteAnulado = context.timbrados_comprobantes_anulaciones.Where(tca => tca.id_timbrado_rango == timbradoRango.id && tca.nro_comprobante == nroComprobante).FirstOrDefault();
                if (comprobanteAnulado != null)
                {
                    retorno = " ha sido anulado por el motivo de " + comprobanteAnulado.timbrados_motivos_anulaciones.motivo + ".";
                }
                else
                {
                    switch (idTipoDocumento)
                    {
                        case 1: //FACTURA

                            break;
                        case 2: //NOTA REMISION

                            break;
                        case 3: //RECIBO

                            break;
                        case 4: //NOTA DE CREDITO

                            break;
                        case 5: //NOTA DE DEBITO

                            break;
                    }
                }
            }

            return retorno;
        }

        #endregion


    }
}