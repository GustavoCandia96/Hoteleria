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
    public class ModulosController : Controller
    {

       #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();


        #endregion

        #region Listado de Modulos

        [HttpGet]
        [AutorizarUsuario("Modulos", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ModuloModel> listaModulos = new List<ModuloModel>();
            try
            {
                //VERIFICAMOS QUE TIPO DE USUARIO ES
                UsuarioLogin usuarioLogin = (UsuarioLogin)Session["user"];
                Simple3Des encriptar = new Simple3Des(usuarioLogin.Usuario); // LLAVE DE ENCRIPTACIÓN
                string claveEncriptada = encriptar.EncriptarDato(usuarioLogin.Clave); // CONTRASEÑA ENCRIPTADA
                var usuario = db.usuarios.Where(u => u.usuario.Trim().ToUpper() == usuarioLogin.Usuario.Trim().ToUpper() && u.clave.Trim().ToUpper() == claveEncriptada.ToUpper()).FirstOrDefault();
                if (usuario != null)
                {
                    if (usuario.id_funcionario == null)
                    {
                        //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                        string sesNomModulo = Convert.ToString(Session["sesionModulosNombre"]);
                        ViewBag.txtModulo = sesNomModulo;

                        //OBTENEMOS TODOS LOS MODULOS ACTIVOS DE LA BASE DE DATOS Y ORDENAMOS POR DESCRIPCIÓN
                        var modulos = from m in db.modulos
                                      where m.estado != null
                                      select new ModuloModel
                                      {
                                          Id = m.id,
                                          Modulo = m.modulo,
                                          Estado = m.estado,
                                          EstadoDescrip = m.estado == true ? "Activo" : "Inactivo"
                                      };
                        listaModulos = modulos.ToList();

                        //FILTRAMOS SI EXISTE PAGINACIÓN
                        if (sesNomModulo != "")
                        {
                            listaModulos = listaModulos.Where(m => m.Modulo.ToUpper().Contains(sesNomModulo.Trim().ToUpper())).ToList();
                        }

                        listaModulos = listaModulos.OrderBy(m => m.Modulo).ToList();
                    }
                    else
                    {
                        return Redirect("Home");
                    }
                }
                else
                {
                    return Redirect("Login");
                }
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de modulos";
            }
            return View(listaModulos.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ModuloModel> listaModulos = new List<ModuloModel>();
            try
            {
                //VERIFICAMOS QUE TIPO DE USUARIO ES
                UsuarioLogin usuarioLogin = (UsuarioLogin)Session["user"];
                Simple3Des encriptar = new Simple3Des(usuarioLogin.Usuario); // LLAVE DE ENCRIPTACIÓN
                string claveEncriptada = encriptar.EncriptarDato(usuarioLogin.Clave); // CONTRASEÑA ENCRIPTADA
                var usuario = db.usuarios.Where(u => u.usuario.Trim().ToUpper() == usuarioLogin.Usuario.Trim().ToUpper() && u.clave.Trim().ToUpper() == claveEncriptada.ToUpper()).FirstOrDefault();
                if (usuario != null)
                {
                    if (usuario.id_funcionario == null)
                    {
                        //OBTENEMOS TODOS LOS MODULOS ACTIVOS DE LA BASE DE DATOS Y ORDENAMOS POR DESCRIPCIÓN
                        var modulos = from m in db.modulos
                                      where m.estado != null
                                      select new ModuloModel
                                      {
                                          Id = m.id,
                                          Modulo = m.modulo,
                                          Estado = m.estado,
                                          EstadoDescrip = m.estado == true ? "Activo" : "Inactivo"
                                      };
                        listaModulos = modulos.ToList();

                        //FILTRAMOS POR NOMBRE MODULO LA BUSQUEDA
                        var fcNombreModulo = fc["txtModulo"];
                        if (fcNombreModulo != "")
                        {
                            string descripcion = Convert.ToString(fcNombreModulo);
                            listaModulos = listaModulos.Where(m => m.Modulo.ToUpper().Contains(descripcion.ToUpper())).ToList();
                        }
                        listaModulos.OrderBy(m => m.Modulo).ToList();

                        //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                        ViewBag.txtModulo = fcNombreModulo;
                        Session["sesionModulosNombre"] = fcNombreModulo;
                    }
                    else
                    {
                        return Redirect("Home");
                    }
                }
                else
                {
                    return Redirect("Login");
                }
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar modulos";
            }
            return View(listaModulos.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Modulo

        [HttpGet]
        [AutorizarUsuario("Modulos", "Create")]
        public ActionResult Create()
        {
            try
            {
                //VERIFICAMOS QUE TIPO DE USUARIO ES
                UsuarioLogin usuarioLogin = (UsuarioLogin)Session["user"];
                Simple3Des encriptar = new Simple3Des(usuarioLogin.Usuario); // LLAVE DE ENCRIPTACIÓN
                string claveEncriptada = encriptar.EncriptarDato(usuarioLogin.Clave); // CONTRASEÑA ENCRIPTADA
                var usuario = db.usuarios.Where(u => u.usuario.Trim().ToUpper() == usuarioLogin.Usuario.Trim().ToUpper() && u.clave.Trim().ToUpper() == claveEncriptada.ToUpper()).FirstOrDefault();
                if (usuario != null)
                {
                    if (usuario.id_funcionario == null)
                    {
                        ModuloModel modulo = new ModuloModel();
                        return View(modulo);
                    }
                    else
                    {
                        return Redirect("Home");
                    }
                }
                else
                {
                    return Redirect("Login");
                }
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Index", "Error", new { id = 0 });
        }

        [HttpPost]
        public ActionResult Create(ModuloModel moduloModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN MODULO EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.modulos.Where(m => m.modulo.ToUpper() == moduloModelo.Modulo.ToUpper() && m.estado != null).Count();
                    if (cantidad == 0)
                    {
                        modulos modulo = new modulos
                        {
                            modulo = moduloModelo.Modulo,
                            estado = true
                        };
                        db.modulos.Add(modulo);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un modulo registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el modulo en la base de datos");
                    retornoVista = true;
                }
                finally
                {
                    db.Dispose();
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
                return View(moduloModelo);
            }
        }

        #endregion

        #region Editar Modulo

        [HttpGet]
        [AutorizarUsuario("Modulos", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModuloModel moduloEdit = new ModuloModel();
            try
            {
                var modulo = db.modulos.Where(p => p.id == id).FirstOrDefault();
                moduloEdit.Id = modulo.id;
                moduloEdit.Modulo = modulo.modulo;
                moduloEdit.Estado = modulo.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = modulo.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(moduloEdit);
        }

        [HttpPost]
        public ActionResult Edit(ModuloModel moduloModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN MODULO EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.modulos.Where(m => m.modulo.ToUpper() == moduloModelo.Modulo.ToUpper() && m.estado != null && m.id != moduloModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var modulo = db.modulos.Where(p => p.id == moduloModelo.Id).FirstOrDefault();
                        modulo.modulo = moduloModelo.Modulo;
                        bool nuevoEstado = moduloModelo.EstadoDescrip == "A" ? true : false;
                        modulo.estado = nuevoEstado;
                        db.Entry(modulo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un modulo registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el modulo en la base de datos");
                    retornoVista = true;
                }
                finally
                {
                    db.Dispose();
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
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", moduloModelo.EstadoDescrip);
                return View(moduloModelo);
            }
        }

        #endregion


        #region Eliminar Modulo

        [HttpPost]
        public ActionResult EliminarModulo(string moduloId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Modulos", "EliminarModulo");
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
                                int idModulo = Convert.ToInt32(moduloId);
                                var modulo = context.modulos.Where(p => p.id == idModulo).FirstOrDefault();
                                modulo.estado = null;
                                context.Entry(modulo).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Modulos") }, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}