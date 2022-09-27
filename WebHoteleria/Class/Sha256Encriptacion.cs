using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebHoteleria.Class
{
    public class Sha256Encriptacion
    {

        /*
         * ENCRIPTACION UTILIZADA PARA GENERACION DE TOKEN DE RECUPERACION DE CUENTA USUARIO
         */
        public string EncriptarSHA256(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding codificacion = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(codificacion.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

    }
}