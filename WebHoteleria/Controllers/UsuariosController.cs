using EntidadesHoteleria;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebHoteleria.Class;
using WebHoteleria.Models;

namespace WebHoteleria.Controllers
{
    public class UsuariosController : Controller
    {

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #region Listado de Usuarios

        [HttpGet]
        [AutorizarUsuario("Usuarios", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<UsuarioModel> listaUsuarios = new List<UsuarioModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesIdPerfil = Convert.ToString(Session["sesionUsuariosIdPerfil"]);
                ViewBag.ddlPerfiles = new SelectList(db.perfiles.Where(p => p.estado == true).OrderBy(p => p.perfil).ToList(), "id", "perfil", sesIdPerfil);
                string sesUsuario = Convert.ToString(Session["sesionUsuariosUsuario"]);
                ViewBag.txtUsuario = sesUsuario;
                string sesNroDoc = Convert.ToString(Session["sesionUsuariosNroDocumento"]);
                ViewBag.txtNroDocumento = sesNroDoc;

                //OBTENEMOS TODOS LOS USUARIOS ACTIVOS DE LA BASE DE DATOS
                var usuarios = from u in db.usuarios
                               where u.estado != null && u.id_perfil != null
                               select new UsuarioModel
                               {
                                   Id = u.id,
                                   IdFuncionario = u.id_funcionario,
                                   IdPerfil = u.id_perfil,
                                   Usuario = u.usuario,
                                   Email = u.email,
                                   PrimeraVez = u.primera_vez,
                                   FechaAlta = u.fecha_alta,
                                   Estado = u.estado,
                                   NombreFuncionario = u.id_funcionario != null ? u.funcionarios.nombre + " " + u.funcionarios.apellido : "",
                                   NombrePerfil = u.perfiles.perfil,
                                   NroDocumento = u.id_funcionario != null ? u.funcionarios.nro_documento : "",
                                   EstadoDescrip = u.estado == true ? "Activo" : "Inactivo"
                               };

                listaUsuarios = usuarios.OrderBy(u => u.Usuario).ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesIdPerfil != "")
                {
                    int idPerfil = Convert.ToInt32(sesIdPerfil);
                    listaUsuarios = listaUsuarios.Where(u => u.IdPerfil == idPerfil).ToList();
                }
                if (sesUsuario != "")
                {
                    listaUsuarios = listaUsuarios.Where(u => u.Usuario.ToUpper().Contains(sesUsuario.Trim().ToUpper())).ToList();
                }
                if (sesNroDoc != "")
                {
                    listaUsuarios = listaUsuarios.Where(u => u.NroDocumento.ToUpper().Contains(sesNroDoc.Trim().ToUpper())).ToList();
                }

                listaUsuarios = usuarios.OrderBy(u => u.Usuario).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de usuarios";
            }
            return View(listaUsuarios.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<UsuarioModel> listaUsuarios = new List<UsuarioModel>();
            try
            {
                //OBTENEMOS TODOS LOS USUARIOS ACTIVOS DE LA BASE DE DATOS
                var usuarios = from u in db.usuarios
                               where u.estado != null && u.id_perfil != null
                               select new UsuarioModel
                               {
                                   Id = u.id,
                                   IdFuncionario = u.id_funcionario,
                                   IdPerfil = u.id_perfil,
                                   Usuario = u.usuario,
                                   Email = u.email,
                                   PrimeraVez = u.primera_vez,
                                   FechaAlta = u.fecha_alta,
                                   Estado = u.estado,
                                   NombreFuncionario = u.id_funcionario != null ? u.funcionarios.nombre + " " + u.funcionarios.apellido : "",
                                   NombrePerfil = u.perfiles.perfil,
                                   NroDocumento = u.id_funcionario != null ? u.funcionarios.nro_documento : "",
                                   EstadoDescrip = u.estado == true ? "Activo" : "Inactivo"
                               };

                listaUsuarios = usuarios.OrderBy(u => u.Usuario).ToList();

                //FILTRAMOS POR TIPO PERFIL USUARIO Y N° DOCUMENTO
                var fcIdPerfil = fc["ddlPerfiles"];
                int idPerfil = fcIdPerfil != "" ? Convert.ToInt32(fcIdPerfil) : 0;
                if (fcIdPerfil != "")
                {
                    listaUsuarios = listaUsuarios.Where(u => u.IdPerfil == idPerfil).ToList();
                }
                var fcUsuario = fc["txtUsuario"];
                if (fcUsuario != "")
                {
                    string usuario = Convert.ToString(fcUsuario);
                    listaUsuarios = listaUsuarios.Where(u => u.Usuario.ToUpper().Contains(usuario.ToUpper())).ToList();
                }

                var fcNroDocumento = fc["txtNroDocumento"];
                if (fcNroDocumento != "")
                {
                    string nroDocumento = Convert.ToString(fcNroDocumento);
                    listaUsuarios = listaUsuarios.Where(c => c.NroDocumento.ToUpper().Contains(nroDocumento.ToUpper())).ToList();
                }

                listaUsuarios.OrderBy(u => u.Usuario).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtUsuario = fcUsuario;
                Session["sesionUsuariosUsuario"] = fcUsuario;
                ViewBag.txtNroDocumento = fcNroDocumento;
                Session["sesionUsuariosNroDocumento"] = fcNroDocumento;
                ViewBag.ddlPerfiles = new SelectList(db.perfiles.Where(p => p.estado == true).OrderBy(p => p.perfil).ToList(), "id", "perfil", fcIdPerfil);
                Session["sesionUsuariosIdPerfil"] = fcIdPerfil;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar usuarios";
            }
            return View(listaUsuarios.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Usuario

        [HttpGet]
        [AutorizarUsuario("Usuarios", "Create")]
        public ActionResult Create()
        {
            UsuarioModel usuario = new UsuarioModel();
            ViewBag.IdPerfil = new SelectList(db.perfiles.Where(p => p.estado == true).OrderBy(p => p.perfil).ToList(), "id", "perfil");
            var listaFuncionarios = (from d in db.funcionarios
                                     where d.estado == true
                                     select new
                                     {
                                         funcionario = d.nombre + " " + d.apellido,
                                         id = d.id
                                     }).ToList();
            ViewBag.IdFuncionario = new SelectList(listaFuncionarios.OrderBy(lf => lf.funcionario), "id", "funcionario");
            return View(usuario);
        }

        [HttpPost]
        public ActionResult Create(UsuarioModel usuarioModelo)
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

                    //VERIFICAMOS SI YA EXISTE UN USUARIO CON EL MISMO NOMBRE Y CONTRASEÑA
                    int cantidad = db.usuarios.Where(u => u.usuario == usuarioModelo.Usuario /*&& u.clave == claveEncriptada*/ && u.estado != null).Count();
                    if (cantidad == 0)
                    {
                        int cantidadEmail = db.usuarios.Where(u => u.email == usuarioModelo.Email).Count();
                        if (cantidadEmail == 0)
                        {
                            usuarios usuario = new usuarios
                            {
                                id_funcionario = usuarioModelo.IdFuncionario,
                                id_perfil = usuarioModelo.IdPerfil,
                                usuario = usuarioModelo.Usuario,
                                email = usuarioModelo.Email,
                                clave = claveEncriptada,
                                primera_vez = true,
                                fecha_alta = DateTime.Now,
                                estado = true
                            };
                            db.usuarios.Add(usuario);
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("EmailDuplicado", "El e-mail ingresado ya existe registrado en el sistema");
                            retornoVista = true;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un usuario registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar al usuario en la base de datos");
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
                return RedirectToAction("Index");
            }
            else
            {
                CargarDatosUsuario(usuarioModelo);
                db.Dispose();
                return View(usuarioModelo);
            }
        }

        #endregion

        #region Editar Usuario

        [HttpGet]
        [AutorizarUsuario("Usuarios", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UsuarioModel usuarioEdit = new UsuarioModel();
            try
            {
                var usuario = db.usuarios.Where(u => u.id == id).FirstOrDefault();
                usuarioEdit.Id = usuario.id;
                usuarioEdit.IdFuncionario = usuario.id_funcionario;
                usuarioEdit.IdPerfil = usuario.id_perfil;
                usuarioEdit.Usuario = usuario.usuario;

                //DESENCRIPTAMOS LA CONTRASEÑA CON LA LLAVE DE NOMBRE DE USUARIO
                Simple3Des desencriptar = new Simple3Des(usuario.usuario);
                string claveDesencriptada = desencriptar.DesencriptarDato(usuario.clave);

                usuarioEdit.Clave = claveDesencriptada;
                usuarioEdit.RepetirClave = claveDesencriptada;
                usuarioEdit.Email = usuario.email;
                usuarioEdit.Estado = usuario.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = usuario.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                CargarDatosUsuario(usuarioEdit);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(usuarioEdit);
        }

        [HttpPost]
        public ActionResult Edit(UsuarioModel usuarioModelo)
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

                    //VERIFICAMOS SI YA EXISTE UN USUARIO CON EL MISMO NOMBRE Y CONTRASEÑA
                    int cantidad = db.usuarios.Where(u => u.usuario == usuarioModelo.Usuario /*&& u.clave == claveEncriptada*/ && u.estado != null && u.id != usuarioModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        int cantidadEmail = db.usuarios.Where(u => u.email == usuarioModelo.Email && u.id != usuarioModelo.Id).Count();
                        if (cantidadEmail == 0)
                        {
                            var usuario = db.usuarios.Where(u => u.id == usuarioModelo.Id).FirstOrDefault();
                            usuario.id_funcionario = usuarioModelo.IdFuncionario;
                            usuario.id_perfil = usuarioModelo.IdPerfil;
                            usuario.usuario = usuarioModelo.Usuario;
                            usuario.clave = claveEncriptada;
                            usuario.email = usuarioModelo.Email;
                            bool nuevoEstado = usuarioModelo.EstadoDescrip == "A" ? true : false;
                            usuario.estado = nuevoEstado;
                            db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("EmailDuplicado", "El e-mail ingresado ya existe registrado en el sistema");
                            retornoVista = true;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un usuario registrado con el mismo nombre");
                        retornoVista = true;
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
                return RedirectToAction("Index");
            }
            else
            {
                CargarDatosUsuario(usuarioModelo);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", usuarioModelo.EstadoDescrip);
                db.Dispose();
                return View(usuarioModelo);
            }
        }

        #endregion

        #region Eliminar Usuario

        [HttpPost]
        public ActionResult EliminarUsuario(string usuarioId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Usuarios", "EliminarUsuario");
                string resultado = autorizarAccion.VerificarPermiso();
                if (resultado == string.Empty)
                {
                    //EJECUTAMOS LA ACTUALIZACION DE LA ELIMINACIÓN DEL REGISTRO
                    using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                int idUsuario = Convert.ToInt32(usuarioId);
                                var usuario = context.usuarios.Where(u => u.id == idUsuario).FirstOrDefault();
                                usuario.estado = null;
                                context.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();

                                dbContextTransaction.Commit();
                            }
                            catch (Exception)
                            {
                                dbContextTransaction.Rollback();
                                retorno = false;
                            }
                        }
                        context.Database.Connection.Close();
                    }
                }
                else
                {
                    respuesta = autorizarAccion.ObtenerMensajeVerificacion(resultado);
                    retorno = false;
                }
            }
            catch (Exception)
            {
                retorno = false;
            }
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Usuarios") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Funciones

        private void CargarDatosUsuario(UsuarioModel usuarioModelo)
        {
            ViewBag.IdPerfil = new SelectList(db.perfiles.Where(p => p.estado == true).OrderBy(p => p.perfil).ToList(), "id", "perfil", usuarioModelo.IdPerfil);
            var listaFuncionarios = (from d in db.funcionarios
                                     where d.estado == true
                                     select new
                                     {
                                         funcionario = d.nombre + " " + d.apellido,
                                         id = d.id
                                     }).ToList();
            ViewBag.IdFuncionario = new SelectList(listaFuncionarios.OrderBy(lf => lf.funcionario), "id", "funcionario", usuarioModelo.IdFuncionario);
        }

        #endregion

        #region AJAX

        [HttpPost]
        public ActionResult ObtenerListadoUsuarios(string sucursalId)
        {
            int idSucursal = Convert.ToInt32(sucursalId);
            var listaUsuarios = (from u in db.usuarios
                                 join f in db.funcionarios on u.id_funcionario equals f.id
                                 where u.estado == true && u.id_funcionario != null && u.id_perfil != null && f.id_sucursal == idSucursal
                                 select new
                                 {
                                     id = u.id,
                                     nombre = f.nombre + " " + f.apellido + " - (" + u.usuario + ")"
                                 }).ToList();
            var usuarios = listaUsuarios.OrderBy(u => u.nombre).Select(u => "<option value='" + u.id + "'>" + u.nombre + "</option>");
            return Content(String.Join("", usuarios));
        }

        #endregion




    }
}