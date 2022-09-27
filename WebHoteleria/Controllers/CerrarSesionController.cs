using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebHoteleria.Controllers
{
    public class CerrarSesionController : Controller
    {

        #region Cerrar Sesión

        [HttpGet]
        public ActionResult Index()
        {
            Session["user"] = null;
            Session["NombreUsuario"] = null;
            return RedirectToAction("Index", "Login");
        }

        #endregion

    }
}