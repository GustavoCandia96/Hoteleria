using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebHoteleria.Controllers;

namespace WebHoteleria.Class
{
    public class VerificaSesion: ActionFilterAttribute
    {

        #region Propiedades

        private UsuarioLogin usuario;

        #endregion

        #region Metodos

        /*
         * METODO QUE VERIFICA SI EL USUARIO HA INICIADO SESION O NO
         */
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                base.OnActionExecuting(filterContext);
                usuario = (UsuarioLogin)HttpContext.Current.Session["user"];
                if (usuario == null)
                {
                    if (filterContext.Controller is LoginController == false)
                    {
                        filterContext.HttpContext.Response.Redirect("~/Login/Index");
                    }
                }
            }
            catch (Exception)
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
            }

        }

        #endregion

    }
}