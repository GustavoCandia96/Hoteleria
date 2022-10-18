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
    public class MozosController : Controller
    {
        
        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Mozos

        [HttpGet]
        [AutorizarUsuario("Mozos", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<MozoModel> listaMozos = new List<MozoModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNroDocumento = Convert.ToString(Session["sesionMozosNroDocumento"]);
                ViewBag.txtNroDocumento = sesNroDocumento;

                //OBTENEMOS LA LISTA DE MOZOS NO ELIMINADAS DE LA BASE DE DATOS
                var mozos = from m in db.mozos
                            where m.estado != null
                            select new MozoModel
                            {
                                Id = m.id,
                                NroDocumento = m.nro_documento,
                                NombreCompleto = m.nombre + " " + m.apellido,
                                Direccion = m.direccion,
                                Celular = m.celular,
                                EstadoDescrip = m.estado == true ? "Activo" : "Inactivo"
                            };
                listaMozos = mozos.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNroDocumento != "")
                {
                    listaMozos = listaMozos.Where(m => m.NroDocumento.ToUpper().Contains(sesNroDocumento.Trim().ToUpper())).ToList();
                }

                listaMozos = listaMozos.OrderBy(m => m.NombreCompleto).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de mozos";
            }
            return View(listaMozos.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<MozoModel> listaMozos = new List<MozoModel>();
            try
            {
                //OBTENEMOS LA LISTA DE MOZOS NO ELIMINADAS DE LA BASE DE DATOS
                var mozos = from m in db.mozos
                            where m.estado != null
                            select new MozoModel
                            {
                                Id = m.id,
                                NroDocumento = m.nro_documento,
                                NombreCompleto = m.nombre + " " + m.apellido,
                                Direccion = m.direccion,
                                Celular = m.celular,
                                EstadoDescrip = m.estado == true ? "Activo" : "Inactivo"
                            };
                listaMozos = mozos.ToList();

                //FILTRAMOS POR NRO DOCUMENTO LA BUSQUEDA
                var fcNroDocumento = fc["txtNroDocumento"];
                if (fcNroDocumento != "")
                {
                    string descripcion = Convert.ToString(fcNroDocumento);
                    listaMozos = listaMozos.Where(m => m.NroDocumento.ToUpper().Contains(fcNroDocumento.Trim().ToUpper())).ToList();
                }

                listaMozos = listaMozos.OrderBy(m => m.NombreCompleto).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtNroDocumento = fcNroDocumento;
                Session["sesionMozosNroDocumento"] = fcNroDocumento;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar mozos";
            }
            return View(listaMozos.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Mozo

        [HttpGet]
        [AutorizarUsuario("Mozos", "Create")]
        public ActionResult Create()
        {
            MozoModel mozo = new MozoModel();
            return View(mozo);
        }

        [HttpPost]
        public ActionResult Create(MozoModel mozoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL N° DOCUMENTO DEL MOZO EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.mozos.Where(m => m.nro_documento.ToUpper() == mozoModelo.NroDocumento.ToUpper() && m.estado != null).Count();
                    if (cantidad == 0)
                    {
                        mozos mozo = new mozos
                        {
                            nro_documento = mozoModelo.NroDocumento,
                            nombre = mozoModelo.Nombre,
                            apellido = mozoModelo.Apellido,
                            direccion = mozoModelo.Direccion,
                            celular = mozoModelo.Celular,
                            estado = true
                        };
                        db.mozos.Add(mozo);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un número de documento registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar al mozo en la base de datos");
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
                return View(mozoModelo);
            }
        }

        #endregion

        #region Editar Mozo

        [HttpGet]
        [AutorizarUsuario("Mozos", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MozoModel mozoEdit = new MozoModel();
            try
            {
                var mozo = db.mozos.Where(m => m.id == id).FirstOrDefault();
                mozoEdit.Id = mozo.id;
                mozoEdit.NroDocumento = mozo.nro_documento;
                mozoEdit.Nombre = mozo.nombre;
                mozoEdit.Apellido = mozo.apellido;
                mozoEdit.Direccion = mozo.direccion;
                mozoEdit.Celular = mozo.celular;
                mozoEdit.Estado = mozo.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = mozo.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(mozoEdit);
        }

        [HttpPost]
        public ActionResult Edit(MozoModel mozoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL N° DOCUMENTO DEL MOZO EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.mozos.Where(m => m.nro_documento.ToUpper() == mozoModelo.NroDocumento.ToUpper() && m.estado != null && m.id != mozoModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var mozo = db.mozos.Where(m => m.id == mozoModelo.Id).FirstOrDefault();
                        mozo.nro_documento = mozoModelo.NroDocumento;
                        mozo.nombre = mozoModelo.Nombre;
                        mozo.apellido = mozoModelo.Apellido;
                        mozo.direccion = mozoModelo.Direccion;
                        mozo.celular = mozoModelo.Celular;
                        bool nuevoEstado = mozoModelo.EstadoDescrip == "A" ? true : false;
                        mozo.estado = nuevoEstado;
                        db.Entry(mozo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un número de documento registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar al mozo en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", mozoModelo.EstadoDescrip);
                return View(mozoModelo);
            }
        }

        #endregion

        #region Eliminar Mozo

        [HttpPost]
        public ActionResult EliminarMozo(string mozoId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Mozos", "EliminarMozo");
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
                                int idMozo = Convert.ToInt32(mozoId);
                                var mozo = context.mozos.Where(m => m.id == idMozo).FirstOrDefault();
                                mozo.estado = null;
                                context.Entry(mozo).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Mozos") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region AJAX

        [HttpGet]
        public ActionResult ObtenerListadoMozos(string nombreApellidoMozo)
        {
            string respuesta = string.Empty;
            string msg = string.Empty;
            string row = string.Empty;
            try
            {
                List<MozoModel> listaMozos = new List<MozoModel>();
                if (nombreApellidoMozo != string.Empty)
                {
                    //OBTENEMOS EL LISTADO DE TODOS LOS MOZOS DE LA BASE DATOS ACTIVOS Y CARGAMOS EN UN MODELO PARA LUEGO FILTRAR
                    var mozos = from m in db.mozos
                                where m.estado == true
                                select new MozoModel
                                {
                                    Id = m.id,
                                    NroDocumento = m.nro_documento,
                                    NombreCompleto = m.nombre + " " + m.apellido,
                                };
                    listaMozos = mozos.ToList();

                    //FILTRAMOS LA BUSQUEDA
                    if (nombreApellidoMozo != string.Empty)
                    {
                        listaMozos = listaMozos.Where(m => m.NombreCompleto.Trim().ToUpper().Contains(nombreApellidoMozo.Trim().ToUpper())).ToList();
                    }
                }

                foreach (var item in listaMozos)
                {
                    row += "<tr>" +
                        "<td>" + item.Id + " </td>" +
                        "<td>" + item.NombreCompleto + " </td>";
                    row += "<td><button type=\'button\' title='Seleccionar Mozo' class=\'btn btn-success btn-xs seleccionarMozo\'><i class=\'glyphicon glyphicon-saved\'></i></button></td>" + "</tr>";
                }
            }
            catch (Exception)
            {
                respuesta = "Error";
            }
            return Json(new { succes = true, respuesta = respuesta, msg = msg, row = row }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}