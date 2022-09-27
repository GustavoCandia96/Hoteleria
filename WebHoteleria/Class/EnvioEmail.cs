using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace WebHoteleria.Class
{
    public class EnvioEmail
    {

        #region Propiedades

        public string EmailOrigen { get; set; }
        public string EmailOrigenClave { get; set; }
        public string EmailDestino { get; set; }
        public string Token { get; set; }
        public string Smtp { get; set; }
        public int Puerto { get; set; }
        public string UrlDomain { get; set; }

        #endregion

        #region Metodos

        /*
         * METODO QUE DEVUELVE SI REALIZO BIEN O NO LA OPERACIÓN DE ENVIAR UN CORREO DE ELECTRONICO DE RECUPERACION DE CONTRASEÑA
         */
        public bool EnviarCorreoElectronico()
        {
            bool retorno = true;
            try
            {
                string url = UrlDomain + "Login/Recovery/?token=" + Token; //URL DE RECUPERACION CON TOKEN
                string tema = "Recuperación de Contraseña";
                string body = "<h3>Hola! Hemos recibido tu solicitud de recuperación de contraseña.</h3><p>En el siguiente enlace podras recuperar tu cuenta de usuario del sistema ERP Itape. Te recomendamos guardar tus datos de acceso en un lugar seguro.</p><br><a href='" + url + "'>Click aquí</a>";
                MailMessage mensajeCorreo = new MailMessage(EmailOrigen, EmailDestino, tema, body);
                mensajeCorreo.IsBodyHtml = true;
                SmtpClient oSmtpClient = new SmtpClient(Smtp);
                oSmtpClient.EnableSsl = true;
                oSmtpClient.UseDefaultCredentials = false;
                oSmtpClient.Port = Puerto;
                oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, EmailOrigenClave);
                oSmtpClient.Send(mensajeCorreo);
                oSmtpClient.Dispose();
                retorno = true;
            }
            catch (Exception)
            {
                retorno = false;
            }
            return retorno;
        }

        #endregion

    }
}