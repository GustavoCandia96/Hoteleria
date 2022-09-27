using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace WebHoteleria.Class
{
    public class Simple3Des
    {

        #region Propiedades

        private TripleDESCryptoServiceProvider TripleDes = new TripleDESCryptoServiceProvider();

        #endregion

        #region Metodos

        public Simple3Des(string key)
        {
            //INICIALIZAR EL PROVEEDOR DE CIFRADO
            TripleDes.Key = TruncateHash(key, TripleDes.KeySize / 8);
            TripleDes.IV = TruncateHash("", TripleDes.BlockSize / 8);
        }

        private byte[] TruncateHash(string key, int length)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

            //HASH DE LLAVE
            byte[] keyBytes = System.Text.Encoding.Unicode.GetBytes(key);
            byte[] hash = sha1.ComputeHash(keyBytes);
            var oldHash = hash;
            hash = new byte[length - 1 + 1];

            //TRUNCAR O RELLENAR EL HASH
            if (oldHash != null)
                Array.Copy(oldHash, hash, Math.Min(length - 1 + 1, oldHash.Length));
            return hash;
        }

        public string EncriptarDato(string plaintext)
        {
            //CONVIERTA LA CADENA DE TEXTO SIN FORMATO EN UNA MATRIZ DE BYTES
            byte[] plaintextBytes = System.Text.Encoding.Unicode.GetBytes(plaintext);

            //CREA LA TRANSMISION
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //CREA EL CODIFICADOR PARA ESCRIBIR EN LA SECUENCIA
            CryptoStream encStream = new CryptoStream(ms, TripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

            //USE LA SECUENCIA DE CIFRADO PARA ESCRIBIR LA MATRIZ DE BYTES EN LA SECUENCIA
            encStream.Write(plaintextBytes, 0, plaintextBytes.Length);
            encStream.FlushFinalBlock();

            //CONVIERTA LA SECUENCIA CIFRADA EN UNA CADENA IMPRIMIBLE
            return Convert.ToBase64String(ms.ToArray());
        }

        public string DesencriptarDato(string encryptedtext)
        {
            //CONVERTA LA CADENA DE TEXTO CIFRADO EN UNA MATRIZ DE BYTES
            byte[] encryptedBytes = Convert.FromBase64String(encryptedtext);

            //CREA LA TRANSMISION
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            //CREA EL DECODIFICADOR PARA ESCRIBIR EN LA SECUENCIA
            CryptoStream decStream = new CryptoStream(ms, TripleDes.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

            // USE LA SECUENCIA DE CIFRADO PARA ESCRIBIR LA MATRIZ DE BYTES EN LA SECUENCIA
            decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            decStream.FlushFinalBlock();

            //CONVIERTA LA SECUENCIA DE TEXTO SIN FORMATO EN UNA CADENA
            return System.Text.Encoding.Unicode.GetString(ms.ToArray());
        }

        #endregion

    }
}