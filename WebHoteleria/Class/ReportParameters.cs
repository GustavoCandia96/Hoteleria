using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Class
{
    public class ReportParameters
    {

        public static ReportesParametros DatosReporte
        {
            get
            {
                return (ReportesParametros)HttpContext.Current.Session["ReportParams"];
            }

            set
            {
                HttpContext.Current.Session["ReportParams"] = value;
            }
        }

    }

    public class ReportesParametros
    {

        #region Propiedades

        public object ReportSource { set; get; }
        public string ReportPath { set; get; }
        public string NombreArchivo { get; set; }

        #endregion

    }
}