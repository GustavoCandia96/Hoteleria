using EntidadesHoteleria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebHoteleria.Class;
using WebHoteleria.Models;

namespace WebHoteleria.Controllers
{
    public class LoginController : Controller
    {
        #region Inicio de Sesión

        [HttpGet]
        public ActionResult Index()
        {
            UsuarioLogin usuario = new UsuarioLogin();
            return View(usuario);
        }


        [HttpPost]
        public ActionResult Index(UsuarioLogin modeloUsuario)
        {
            string msg = null;
            bool habilitado = false;
            bool primeraVez = false;
            try
            {
                if (ModelState.IsValid) // VERIFICAR SI INGRESARON DATOS
                {
                    using (hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities())
                    {
                        Simple3Des encriptar = new Simple3Des(modeloUsuario.Usuario); // LLAVE DE ENCRIPTACIÓN
                        string claveEncriptada = encriptar.EncriptarDato(modeloUsuario.Clave); // CONTRASEÑA ENCRIPTADA

                        //VERIFICAMOS SI EL USUARIO EXISTE EN LA BASE DE DATOS
                        var countUsuario = db.usuarios.Where(u => u.usuario.Trim().ToUpper() == modeloUsuario.Usuario.Trim().ToUpper() && u.estado != null).Count();
                        if (countUsuario == 0)
                        {
                            msg = "El usuario ingresado no existe";
                        }
                        else
                        {
                            //VERIFICAMOS SI LA CLAVE INGRESADA DEL USUARIO ES CORRECTA
                            var count = db.usuarios.Where(u => u.usuario.Trim().ToUpper() == modeloUsuario.Usuario.Trim().ToUpper() && u.clave.Trim().ToUpper() == claveEncriptada.ToUpper() && u.estado != null).Count();
                            if (count == 0)
                            {
                                msg = "La clave ingresada es incorrecta";
                            }
                            else
                            {
                                //VERIFICAMOS SI EL USUARIO ESTA ACTIVO
                                var user = db.usuarios.Where(u => u.usuario.Trim().ToUpper() == modeloUsuario.Usuario.Trim().ToUpper() && u.clave.Trim().ToUpper() == claveEncriptada.ToUpper() && u.estado != null).FirstOrDefault();
                                if (user.estado == false)
                                {
                                    msg = "El usuario esta temporalmente inactivo";
                                }
                                else
                                {
                                    //EL USUARIO ESTA HABILITADO PARA INICIAR SESION
                                    habilitado = true;
                                    Session["user"] = modeloUsuario;
                                    Session["IdUser"] = user.id;
                                    Session["NombreUsuario"] = modeloUsuario.Usuario;
                                    primeraVez = user.primera_vez == null ? false : user.primera_vez.Value;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message.ToString();
                return View(modeloUsuario);
            }
            if (habilitado)
            {
                if (primeraVez)
                {
                    return RedirectToAction("ModificarClave", "Login");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.msg = msg;
                return View(modeloUsuario);
            }
        }

        #endregion

        #region Modificar Password

        [HttpGet]
        public ActionResult ModificarClave()
        {
            UsuarioModel usuarioModelo = new UsuarioModel();
            try
            {
                int id = Convert.ToInt32(Session["IdUser"]);
                using (hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities())
                {
                    var usuario = db.usuarios.Where(u => u.id == id).FirstOrDefault();
                    usuarioModelo.Id = usuario.id;
                    usuarioModelo.IdFuncionario = usuario.id_funcionario;
                    usuarioModelo.IdPerfil = usuario.id_perfil;
                    usuarioModelo.Usuario = usuario.usuario;
                    usuarioModelo.Clave = string.Empty;
                    usuarioModelo.RepetirClave = string.Empty;
                    usuarioModelo.Email = usuario.email;
                }
                Session["user"] = null;
                Session["IdUser"] = null;
                Session["NombreUsuario"] = null;
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(usuarioModelo);
        }

        [HttpPost]
        public ActionResult ModificarClave(UsuarioModel usuarioModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //ENCRIPTAMOS LA CONTRASEÑA UTILIZANDO LA LLAVE DEL NOMBRE USUARIO
                    Simple3Des encriptar = new Simple3Des(usuarioModelo.Usuario);
                    string claveEncriptada = encriptar.EncriptarDato(usuarioModelo.Clave);

                    using (hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities())
                    {
                        //VERIFICAMOS SI YA EXISTE UN USUARIO CON EL MISMO NOMBRE Y CONTRASEÑA
                        int cantidad = db.usuarios.Where(u => u.usuario == usuarioModelo.Usuario && u.clave == claveEncriptada && u.estado != null && u.id != usuarioModelo.Id).Count();
                        if (cantidad == 0)
                        {
                            var usuario = db.usuarios.Where(u => u.id == usuarioModelo.Id).FirstOrDefault();
                            usuario.clave = claveEncriptada;
                            usuario.primera_vez = false;
                            db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("Duplicado", "Ya existe un usuario registrado con el nombre y la contraseña ingresada");
                            retornoVista = true;
                        }
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar los datos del usuario en la base de datos");
                    retornoVista = true;
                }
            }
            else
            {
                ViewBag.msg = null;
                retornoVista = true;
            }
            if (retornoVista == false)
            {
                return RedirectToAction("Index", "CerrarSesion");
            }
            else
            {
                return View(usuarioModelo);
            }
        }

        #endregion

        #region Recuperar Password

        [HttpGet]
        public ActionResult RecuperarPassword()
        {
            RecuperarPasswordModel RecuperarPassword = new RecuperarPasswordModel();
            return View(RecuperarPassword);
        }

        [HttpPost]
        public ActionResult RecuperarPassword(RecuperarPasswordModel recuperarPasswordModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    using (hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities())
                    {
                        //VERIFICAMOS SI EXISTE EL USUARIO Y EL CORREO EN LA BASE DE DATOS
                        int cantidadUser = db.usuarios.Where(u => u.usuario.Trim().ToUpper() == recuperarPasswordModelo.Usuario.Trim().ToUpper() && u.email.Trim().ToUpper() == recuperarPasswordModelo.Email.Trim().ToUpper() && u.estado != null).Count();
                        if (cantidadUser > 0)
                        {
                            //CREAR TOKEN PARA RECUPERACION DE CONTRASEÑA
                            Sha256Encriptacion sha256 = new Sha256Encriptacion();
                            string token = sha256.EncriptarSHA256(Guid.NewGuid().ToString());
                            var usuario = db.usuarios.Where(u => u.usuario.Trim().ToUpper() == recuperarPasswordModelo.Usuario.Trim().ToUpper() && u.email.Trim().ToUpper() == recuperarPasswordModelo.Email.Trim().ToUpper() && u.estado != null).FirstOrDefault();
                            usuario.token_recovery = token;
                            db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            EnvioEmail envioEmail = new EnvioEmail();
                            string emailOrigen = db.parametros.Where(p => p.parametro == "SMTPUSER" && p.estado == true).FirstOrDefault().valor;
                            string emailOrigenClave = db.parametros.Where(p => p.parametro == "SMTPPASS" && p.estado == true).FirstOrDefault().valor;
                            string smtp = db.parametros.Where(p => p.parametro == "SMTP" && p.estado == true).FirstOrDefault().valor;
                            string strPuerto = db.parametros.Where(p => p.parametro == "SMTPPORT" && p.estado == true).FirstOrDefault().valor;
                            int puerto = Convert.ToInt32(strPuerto);
                            string urlDomain = db.parametros.Where(p => p.parametro == "SMTPURLDOMAIN" && p.estado == true).FirstOrDefault().valor;

                            envioEmail.EmailOrigen = emailOrigen;
                            envioEmail.EmailOrigenClave = emailOrigenClave;
                            envioEmail.EmailDestino = usuario.email;
                            envioEmail.Token = usuario.token_recovery;
                            envioEmail.Smtp = smtp;
                            envioEmail.Puerto = puerto;
                            envioEmail.UrlDomain = urlDomain;

                            bool resultado = envioEmail.EnviarCorreoElectronico();
                            if (resultado == false)
                            {
                                ModelState.AddModelError("ErrorEnvio", "Hubo un error al enviar el e-mail de recuperación de contraseña");
                                retornoVista = true;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Aviso", "Los datos ingresados son incorrectos");
                            retornoVista = true;
                        }
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al recuperar la contraseña del usuario");
                    retornoVista = true;
                }
            }
            else
            {
                ViewBag.msg = null;
                retornoVista = true;
            }
            if (retornoVista == false)
            {
                return RedirectToAction("CorreoEnviado");
            }
            else
            {
                return View(recuperarPasswordModelo);
            }
        }

        [HttpGet]
        public ActionResult CorreoEnviado()
        {
            ViewBag.Mensaje = "Se ha enviado un link de recuperación de contraseña a su correo exitosamente.";
            return View();
        }

        #endregion

        #region Token Actualizar Password

        [HttpGet]
        public ActionResult Recovery(string token)
        {
            UsuarioModel usuarioEdit = new UsuarioModel();
            try
            {
                using (hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities())
                {
                    var usuario = db.usuarios.Where(u => u.token_recovery == token).FirstOrDefault();
                    if (usuario != null)
                    {
                        usuarioEdit.Id = usuario.id;
                        usuarioEdit.IdFuncionario = usuario.id_funcionario;
                        usuarioEdit.IdPerfil = usuario.id_perfil;
                        usuarioEdit.Usuario = usuario.usuario;
                        usuarioEdit.Clave = string.Empty;
                        usuarioEdit.RepetirClave = string.Empty;
                        usuarioEdit.Email = usuario.email;
                        usuarioEdit.Estado = usuario.estado;
                    }
                    else
                    {
                        return RedirectToAction("TokenExpirado");
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("Error", "Ocurrio un error al tratar de cambiar la contraseña del usuario. Intentelo de nuevo.");
            }
            return View(usuarioEdit);
        }

        [HttpPost]
        public ActionResult Recovery(UsuarioModel usuarioModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //ENCRIPTAMOS LA CONTRASEÑA UTILIZANDO LA LLAVE DEL NOMBRE USUARIO
                    Simple3Des encriptar = new Simple3Des(usuarioModelo.Usuario);
                    string claveEncriptada = encriptar.EncriptarDato(usuarioModelo.Clave);

                    using (hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities())
                    {
                        //VERIFICAMOS SI YA EXISTE UN USUARIO CON EL MISMO NOMBRE Y CONTRASEÑA
                        int cantidad = db.usuarios.Where(u => u.usuario == usuarioModelo.Usuario && u.clave == claveEncriptada && u.estado != null && u.id != usuarioModelo.Id).Count();
                        if (cantidad == 0)
                        {
                            var usuario = db.usuarios.Where(u => u.id == usuarioModelo.Id).FirstOrDefault();
                            usuario.clave = claveEncriptada;
                            usuario.token_recovery = null;
                            db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("Duplicado", "Ya existe un usuario registrado con el nombre y la contraseña ingresada");
                            retornoVista = true;
                        }
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar los datos del usuario en la base de datos");
                    retornoVista = true;
                }
            }
            else
            {
                ViewBag.msg = null;
                retornoVista = true;
            }
            if (retornoVista == false)
            {
                return RedirectToAction("PasswordActualizado");
            }
            else
            {
                return View(usuarioModelo);
            }
        }

        [HttpGet]
        public ActionResult PasswordActualizado()
        {
            ViewBag.Mensaje = "La contraseña del usuario ha sido actualizado exitosamente.";
            return View();
        }

        [HttpGet]
        public ActionResult TokenExpirado()
        {
            ViewBag.Mensaje = "El token de recuperación de contraseña ya fue expirado.";
            return View();
        }

        #endregion
    }
}