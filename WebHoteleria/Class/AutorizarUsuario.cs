using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebHoteleria.Class
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AutorizarUsuario: AuthorizeAttribute
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();
        private string modulo;
        private string operacion;

        #endregion

        #region Metodos

        /*
         * CONSTRUCTOR DE LA CLASE PARA CARGAR LAS PROPIEDADES
         */
        public AutorizarUsuario(string modulo, string operacion)
        {
            this.modulo = modulo;
            this.operacion = operacion;
        }

        /*
         * METODO QUE VERIFICA LOS PERMISOS DEL USUARIO LOGUEADO EN SESION POR VISTAS
         */
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            string respuesta = string.Empty;
            try
            {
                //OBTENEMOS LOS DATOS DEL USUARIO QUE ESTA EN SESION
                UsuarioLogin usuarioLogin = (UsuarioLogin)HttpContext.Current.Session["user"];
                Simple3Des encriptar = new Simple3Des(usuarioLogin.Usuario); // LLAVE DE ENCRIPTACIÓN
                string claveEncriptada = encriptar.EncriptarDato(usuarioLogin.Clave); // CONTRASEÑA ENCRIPTADA
                var usuario = db.usuarios.Where(u => u.usuario.Trim().ToUpper() == usuarioLogin.Usuario.Trim().ToUpper() && u.clave.Trim().ToUpper() == claveEncriptada.ToUpper()).FirstOrDefault();

                //EL USUARIO CON ID 1 ES EL USUARIO OCULTO QUIEN TIENE ACCESO EN TODOS LOS MODULOS (USUARIO DESARROLLADOR)
                if (usuario.id != 1)
                {
                    //VERIFICAMOS SI EL MODULO EXISTE EN LA BASE DE DATOS
                    var registroModulo = db.modulos.Where(m => m.modulo.ToUpper() == modulo.ToUpper() && m.estado != null).FirstOrDefault();
                    if (registroModulo != null) //SI EXISTE EL MODULO
                    {
                        //VERIFICAMOS SI EL MODULO ESTA ACTIVO EN LA BASE DE DATOS
                        if (registroModulo.estado == true) //SI ESTA ACTIVO
                        {
                            //VERIFICAMOS SI LA OPERACION ESTA RELACIONADO CON EL MODULO
                            var registroOperacion = db.modulos_operaciones.Where(mo => mo.operacion.ToUpper() == operacion.ToUpper() && mo.id_modulo == registroModulo.id && mo.estado != null).FirstOrDefault();
                            if (registroOperacion != null) //SI ESTA RELACIONADO
                            {
                                //VERIFICAMOS SI LA OPERACION ESTA ACTIVO EN LA BASE DE DATOS
                                if (registroOperacion.estado == true) //SI ESTA ACTIVO
                                {
                                    //VERIFICAMOS SI TIENE PERMISO EL USUARIO CON SU PERFIL Y LA OPERACIÓN
                                    var count = db.permisos.Where(p => p.id_perfil == usuario.id_perfil && p.id_modulo == registroOperacion.id && p.habilitado == true).Count();
                                    if (count == 0) //SI NO TIENE PERMISO RECHAZAMOS ACCESO AL MODULO
                                    {
                                        respuesta = "PERMISONOEXISTE";
                                    }
                                }
                                else
                                {
                                    respuesta = "MODULOOPERACIONDESACTIVADO";
                                }
                            }
                            else
                            {
                                respuesta = "OPERACIONNOEXISTE";
                            }
                        }
                        else
                        {
                            respuesta = "MODULODESACTIVADO";
                        }
                    }
                    else
                    {
                        respuesta = "MODULONOEXISTE";
                    }
                }
            }
            catch (Exception)
            {
                respuesta = "ERROR";
            }
            //SI LA RESPUESTA TIENE CARGADA ALGUNA INFORMACIÓN REDIRECCIONAMOS A LA VISTA PARA MOSTRAR EL RESULTADO DE LA ACCIÓN
            if (respuesta != string.Empty)
            {
                filterContext.Result = new RedirectResult("~/Error/AutorizarOperacion?respuesta=" + respuesta);
            }
        }

        #endregion

    }
}