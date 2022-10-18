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
    public class ModulosOperacionesController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Modulos Operaciones

        [HttpGet]
        [AutorizarUsuario("ModulosOperaciones", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ModuloOperacionModel> listaModulosOperaciones = new List<ModuloOperacionModel>();
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
                        //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                        string sesNom = Convert.ToString(Session["sesionModulosOperacionesNombre"]);
                        ViewBag.txtNombre = sesNom;
                        string sesIdModulo = Convert.ToString(Session["sesionModulosOperacionesIdModulo"]);
                        ViewBag.ddlModulos = new SelectList(db.modulos.Where(m => m.estado == true).OrderBy(m => m.modulo).ToList(), "id", "modulo", sesIdModulo);

                        //OBTENEMOS TODOS LOS MODULOS OPERACIONES ACTIVOS DE LA BASE DE DATOS
                        var modulosOperaciones = from mo in db.modulos_operaciones
                                                 join m in db.modulos on mo.id_modulo equals m.id
                                                 where mo.estado != null
                                                 select new ModuloOperacionModel
                                                 {
                                                     Id = mo.id,
                                                     IdModulo = mo.id_modulo,
                                                     Operacion = mo.operacion,
                                                     Descripcion = mo.descripcion,
                                                     Estado = mo.estado,
                                                     NombreModulo = m.modulo,
                                                     EstadoDescrip = mo.estado == true ? "Activo" : "Inactivo"
                                                 };

                        listaModulosOperaciones = modulosOperaciones.OrderBy(mo => mo.NombreModulo).ToList();

                        //FILTRAMOS SI EXISTE PAGINACIÓN
                        if (sesNom != "")
                        {
                            listaModulosOperaciones = listaModulosOperaciones.Where(mo => mo.Operacion.ToUpper().Contains(sesNom.Trim().ToUpper())).ToList();
                        }
                        if (sesIdModulo != "")
                        {
                            int idModulo = Convert.ToInt32(sesIdModulo);
                            listaModulosOperaciones = listaModulosOperaciones.Where(mo => mo.IdModulo == idModulo).ToList();
                        }
                        listaModulosOperaciones = listaModulosOperaciones.OrderBy(mo => mo.NombreModulo).ToList();
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
                ViewBag.msg = "Ocurrio un error al cargar el listado de modulos operaciones";
            }
            return View(listaModulosOperaciones.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<ModuloOperacionModel> listaModulosOperaciones = new List<ModuloOperacionModel>();
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
                        //OBTENEMOS TODOS LOS MODULOS OPERACIONES ACTIVOS DE LA BASE DE DATOS
                        var modulosOperaciones = from mo in db.modulos_operaciones
                                                 join m in db.modulos on mo.id_modulo equals m.id
                                                 where mo.estado != null
                                                 select new ModuloOperacionModel
                                                 {
                                                     Id = mo.id,
                                                     IdModulo = mo.id_modulo,
                                                     Operacion = mo.operacion,
                                                     Descripcion = mo.descripcion,
                                                     Estado = mo.estado,
                                                     NombreModulo = m.modulo,
                                                     EstadoDescrip = mo.estado == true ? "Activo" : "Inactivo"
                                                 };

                        listaModulosOperaciones = modulosOperaciones.OrderBy(mo => mo.NombreModulo).ToList();

                        //FILTRAMOS POR NOMBRE Y MODULO
                        var fcNombre = fc["txtNombre"];
                        if (fcNombre != "")
                        {
                            string descripcion = Convert.ToString(fcNombre);
                            listaModulosOperaciones = listaModulosOperaciones.Where(mo => mo.Operacion.ToUpper().Contains(descripcion.ToUpper())).ToList();
                        }

                        var fcIdModulo = fc["ddlModulos"];
                        if (fcIdModulo != "")
                        {
                            int idModulo = Convert.ToInt32(fcIdModulo);
                            listaModulosOperaciones = listaModulosOperaciones.Where(mo => mo.IdModulo == idModulo).ToList();
                        }

                        listaModulosOperaciones.OrderBy(mo => mo.NombreModulo).ToList();

                        //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                        ViewBag.txtNombre = fcNombre;
                        Session["sesionModulosOperacionesNombre"] = fcNombre;
                        ViewBag.ddlModulos = new SelectList(db.modulos.Where(m => m.estado == true).OrderBy(m => m.modulo).ToList(), "id", "modulo", fcIdModulo);
                        Session["sesionModulosOperacionesIdModulo"] = fcIdModulo;
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
                ViewBag.msg = "Ocurrio un error al buscar modulos operaciones";
            }
            return View(listaModulosOperaciones.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Modulo Operación

        [HttpGet]
        [AutorizarUsuario("ModulosOperaciones", "Create")]
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
                        ModuloOperacionModel moduloOperacion = new ModuloOperacionModel();
                        ViewBag.IdModulo = new SelectList(db.modulos.Where(m => m.estado == true).OrderBy(m => m.modulo).ToList(), "id", "modulo");
                        return View(moduloOperacion);
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
        public ActionResult Create(ModuloOperacionModel moduloOperacionModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA OPERACIÓN CON EL MISMO MODULO PARA PODER AGREGAR
                    int cantidad = db.modulos_operaciones.Where(mo => mo.operacion.ToUpper() == moduloOperacionModelo.Operacion.ToUpper() && mo.id_modulo == moduloOperacionModelo.IdModulo && mo.estado != null).Count();
                    if (cantidad == 0)
                    {
                        modulos_operaciones moduloOperacion = new modulos_operaciones
                        {
                            id_modulo = moduloOperacionModelo.IdModulo,
                            operacion = moduloOperacionModelo.Operacion,
                            descripcion = moduloOperacionModelo.Descripcion,
                            estado = true
                        };
                        db.modulos_operaciones.Add(moduloOperacion);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una operación registrado con el mismo nombre y modulo");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la operación en la base de datos");
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
                ViewBag.IdModulo = new SelectList(db.modulos.Where(m => m.estado == true).OrderBy(m => m.modulo).ToList(), "id", "modulo", moduloOperacionModelo.IdModulo);
                db.Dispose();
                return View(moduloOperacionModelo);
            }
        }

        #endregion

        #region Editar Modulo Operación

        [HttpGet]
        [AutorizarUsuario("ModulosOperaciones", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModuloOperacionModel moduloOperacionEdit = new ModuloOperacionModel();
            try
            {
                var moduloOperacion = db.modulos_operaciones.Where(d => d.id == id).FirstOrDefault();
                moduloOperacionEdit.Id = moduloOperacion.id;
                moduloOperacionEdit.IdModulo = moduloOperacion.id_modulo;
                moduloOperacionEdit.Operacion = moduloOperacion.operacion;
                moduloOperacionEdit.Descripcion = moduloOperacion.descripcion;
                moduloOperacionEdit.Estado = moduloOperacion.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = moduloOperacion.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                ViewBag.IdModulo = new SelectList(db.modulos.Where(m => m.estado == true).OrderBy(m => m.modulo).ToList(), "id", "modulo", moduloOperacionEdit.IdModulo);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(moduloOperacionEdit);
        }

        [HttpPost]
        public ActionResult Edit(ModuloOperacionModel moduloOperacionModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA OPERACIÓN CON EL MISMO MODULO PARA PODER AGREGAR
                    int cantidad = db.modulos_operaciones.Where(mo => mo.operacion.ToUpper() == moduloOperacionModelo.Operacion.ToUpper() && mo.id_modulo == moduloOperacionModelo.IdModulo && mo.id != moduloOperacionModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var moduloOperacion = db.modulos_operaciones.Where(mo => mo.id == moduloOperacionModelo.Id).FirstOrDefault();
                        moduloOperacion.id_modulo = moduloOperacionModelo.IdModulo;
                        moduloOperacion.operacion = moduloOperacionModelo.Operacion;
                        moduloOperacion.descripcion = moduloOperacionModelo.Descripcion;
                        bool nuevoEstado = moduloOperacionModelo.EstadoDescrip == "A" ? true : false;
                        moduloOperacion.estado = nuevoEstado;
                        db.Entry(moduloOperacion).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una operación registrado con el mismo nombre y modulo");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al editar la operación del modulo en la base de datos");
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
                ViewBag.IdModulo = new SelectList(db.modulos.Where(m => m.estado == true).OrderBy(m => m.modulo).ToList(), "id", "modulo", moduloOperacionModelo.IdModulo);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", moduloOperacionModelo.EstadoDescrip);
                db.Dispose();
                return View(moduloOperacionModelo);
            }
        }

        #endregion

        #region Eliminar Modulo Operación

        [HttpPost]
        public ActionResult EliminarModuloOperacion(string moduloOperacionId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("ModulosOperaciones", "EliminarModuloOperacion");
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
                                int idModuloOperacion = Convert.ToInt32(moduloOperacionId);
                                var moduloOperacion = context.modulos_operaciones.Where(p => p.id == idModuloOperacion).FirstOrDefault();
                                moduloOperacion.estado = null;
                                context.Entry(moduloOperacion).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "ModulosOperaciones") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region AJAX

        [HttpPost]
        public ActionResult ObtenerListadoOperaciones(string moduloId)
        {
            List<modulos_operaciones> listaModulosOperaciones = new List<modulos_operaciones>();
            try
            {
                int idModulo = Convert.ToInt32(moduloId);
                listaModulosOperaciones = db.modulos_operaciones.Where(mo => mo.id_modulo == idModulo && mo.estado == true).ToList();
            }
            catch (Exception)
            {
            }
            var modulosOperaciones = listaModulosOperaciones.OrderBy(mo => mo.operacion).Select(mo => "<option value='" + mo.id + "'>" + mo.operacion + "</option>");
            return Content(String.Join("", modulosOperaciones));
        }

        [HttpPost]
        public ActionResult ObtenerListadoOperacionesPorDescripcion(string moduloId)
        {
            List<modulos_operaciones> listaModulosOperaciones = new List<modulos_operaciones>();
            try
            {
                int idModulo = Convert.ToInt32(moduloId);
                listaModulosOperaciones = db.modulos_operaciones.Where(mo => mo.id_modulo == idModulo && mo.estado == true).ToList();
            }
            catch (Exception)
            {
            }
            var modulosOperaciones = listaModulosOperaciones.OrderBy(mo => mo.descripcion).Select(mo => "<option value='" + mo.id + "'>" + mo.descripcion + "</option>");
            return Content(String.Join("", modulosOperaciones));
        }

        #endregion


    }
}