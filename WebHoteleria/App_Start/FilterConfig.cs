using System.Web;
using System.Web.Mvc;

namespace WebHoteleria
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new Class.VerificaSesion()); //FILTRO PARA VERIFICACIÓN DE SESION DEL SISTEMA
        }
    }
}
