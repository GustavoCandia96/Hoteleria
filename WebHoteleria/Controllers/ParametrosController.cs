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
    public class ParametrosController : Controller
    {


        #region Propiedad

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();


        #endregion

        #region Listado de Parametros

        [HttpGet]
        [AutorizarUsuario("Parametros", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ParametroModel> listaParametros = new List<ParametroModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesParametro = Convert.ToString(Session["sesionParametrosParametro"]);
                ViewBag.txtParametro = sesParametro;

                //OBTENEMOS TODOS LOS PARAMETROS ACTIVOS DE LA BASE DE DATOS Y ORDENAMOS POR NOMBRE PARAMETRO
                var parametros = from p in db.parametros
                                 where p.estado != null
                                 select new ParametroModel
                                 {
                                     Id = p.id,
                                     Parametro = p.parametro,
                                     Valor = p.valor,
                                     Descripcion = p.descripcion,
                                     Estado = p.estado,
                                     EstadoDescrip = p.estado == true ? "Activo" : "Inactivo"
                                 };
                listaParametros = parametros.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesParametro != "")
                {
                    listaParametros = listaParametros.Where(p => p.Parametro.ToUpper().Contains(sesParametro.Trim().ToUpper())).ToList();
                }

                listaParametros = listaParametros.OrderBy(p => p.Parametro).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de parametros";
            }
            return View(listaParametros.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ParametroModel> listaParametros = new List<ParametroModel>();
            try
            {
                //OBTENEMOS TODOS LOS PARAMETROS ACTIVOS DE LA BASE DE DATOS Y ORDENAMOS POR NOMBRE PARAMETRO
                var parametros = from p in db.parametros
                                 where p.estado != null
                                 select new ParametroModel
                                 {
                                     Id = p.id,
                                     Parametro = p.parametro,
                                     Valor = p.valor,
                                     Descripcion = p.descripcion,
                                     Estado = p.estado,
                                     EstadoDescrip = p.estado == true ? "Activo" : "Inactivo"
                                 };
                listaParametros = parametros.ToList();

                //FILTRAMOS POR NOMBRE PARAMETRO LA BUSQUEDA
                var fcParametro = fc["txtParametro"];
                if (fcParametro != "")
                {
                    string parametro = Convert.ToString(fcParametro);
                    listaParametros = listaParametros.Where(p => p.Parametro.ToUpper().Contains(parametro.ToUpper())).ToList();
                }
                listaParametros.OrderBy(p => p.Parametro).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtParametro = fcParametro;
                Session["sesionParametrosParametro"] = fcParametro;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar parametros";
            }
            return View(listaParametros.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Agregar Parametro

        [HttpGet]
        [AutorizarUsuario("Parametros", "Create")]
        public ActionResult Create()
        {
            ParametroModel parametro = new ParametroModel();
            return View(parametro);
        }

        [HttpPost]
        public ActionResult Create(ParametroModel parametroModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN PARAMETRO EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.parametros.Where(p => p.parametro.ToUpper() == parametroModelo.Parametro.ToUpper() && p.estado != null).Count();
                    if (cantidad == 0)
                    {
                        parametros parametro = new parametros
                        {
                            parametro = parametroModelo.Parametro,
                            valor = parametroModelo.Valor,
                            descripcion = parametroModelo.Descripcion,
                            estado = true
                        };
                        db.parametros.Add(parametro);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un parametro registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el parametro en la base de datos");
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
                return View(parametroModelo);
            }
        }

        #endregion

        #region Editar Parametro

        [HttpGet]
        [AutorizarUsuario("Parametros", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ParametroModel parametroEdit = new ParametroModel();
            try
            {
                var parametro = db.parametros.Where(p => p.id == id).FirstOrDefault();
                parametroEdit.Id = parametro.id;
                parametroEdit.Parametro = parametro.parametro;
                parametroEdit.Valor = parametro.valor;
                parametroEdit.Descripcion = parametro.descripcion;
                parametroEdit.Estado = parametro.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = parametro.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(parametroEdit);
        }

        [HttpPost]
        public ActionResult Edit(ParametroModel parametroModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN PARAMETRO EN LA BASE DE DATOS PARA PODER EDITAR
                    int cantidad = db.parametros.Where(p => p.parametro.ToUpper() == parametroModelo.Parametro.ToUpper() && p.estado != null && p.id != parametroModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var parametro = db.parametros.Where(a => a.id == parametroModelo.Id).FirstOrDefault();
                        parametro.valor = parametroModelo.Valor;
                        parametro.descripcion = parametroModelo.Descripcion;
                        bool nuevoEstado = parametroModelo.EstadoDescrip == "A" ? true : false;
                        parametro.estado = nuevoEstado;
                        db.Entry(parametro).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un parametro registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el parametro en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", parametroModelo.EstadoDescrip);
                return View(parametroModelo);
            }
        }

        #endregion

        #region Eliminar Parametro

        [HttpPost]
        public ActionResult EliminarParametro(string parametroId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Parametros", "EliminarParametro");
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
                                int idParametro = Convert.ToInt32(parametroId);
                                var parametro = context.parametros.Where(p => p.id == idParametro).FirstOrDefault();
                                parametro.estado = null;
                                context.Entry(parametro).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Parametros") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}