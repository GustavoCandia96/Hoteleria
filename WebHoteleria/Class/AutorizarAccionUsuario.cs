using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Class
{
    public class AutorizarAccionUsuario
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

        public AutorizarAccionUsuario(string modulo, string operacion)
        {
            this.modulo = modulo;
            this.operacion = operacion;
        }

        /*
       * METODO QUE VERIFICA LOS PERMISOS DEL USUARIO LOGUEADO EN SESION POR ACCION (COMO EliminarCliente)
       */

        public string VerificarPermiso()
        {
            string retorno = string.Empty;
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
                                    var count = db.permisos.Where(p => p.id_perfil == usuario.id_perfil && p.id_modulo_operacion == registroOperacion.id && p.habilitado == true).Count();
                                    if (count == 0) //SI NO TIENE PERMISO RECHAZAMOS ACCESO AL MODULO
                                    {
                                        retorno = "PERMISONOEXISTE";
                                    }
                                }
                                else
                                {
                                    retorno = "MODULOOPERACIONDESACTIVADO";
                                }
                            }
                            else
                            {
                                retorno = "OPERACIONNOEXISTE";
                            }
                        }
                        else
                        {
                            retorno = "MODULODESACTIVADO";
                        }
                    }
                    else
                    {
                        retorno = "MODULONOEXISTE";
                    }
                }
            }
            catch (Exception)
            {
                retorno = "ERROR";
            }
            return retorno;
        }

        /*
         * METODO QUE OBTIENE UN MENSAJE DE VERIFICACION DEPENDIENDO DEL CASO RECIBIDO
         */
        public string ObtenerMensajeVerificacion(string resultado)
        {
            string retorno = string.Empty;
            switch (resultado)
            {
                case "PERMISONOEXISTE":
                    retorno = "No posee permisos para la operación";
                    break;
                case "OPERACIONNOEXISTE":
                    retorno = "La operación solicitada no existe en la base de datos";
                    break;
                case "MODULONOEXISTE":
                    retorno = "El modulo no esta registrado en la base de datos";
                    break;
                case "MODULODESACTIVADO":
                    retorno = "El modulo esta desactivado temporalmente";
                    break;
                case "MODULOOPERACIONDESACTIVADO":
                    retorno = "La operación del modulo esta desactivado temporalmente";
                    break;
                case "ERROR":
                    retorno = "Ocurrio un error al verificar los permisos";
                    break;
                default:
                    break;
            }
            return retorno;
        }
        #endregion
    }
}