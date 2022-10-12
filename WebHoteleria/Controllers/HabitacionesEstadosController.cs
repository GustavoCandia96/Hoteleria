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
    public class HabitacionesEstadosController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Habitaciones Estados

        [HttpGet]
        [AutorizarUsuario("HabitacionesEstados", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<HabitacionEstadoModel> listaHabitacionesEstados = new List<HabitacionEstadoModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesDes = Convert.ToString(Session["sesionHabitacionesEs"]);
                ViewBag.txtHabitacionEstado = sesDes;

                //OBTENEMOS LAS HABITACIONES ESTADOS NO ELIMINADAS DE LA BASE DE DATOS
                var habitacionEstado = from he in db.habitaciones_estados
                                     where he.estado != null
                                     select new HabitacionEstadoModel
                                     {
                                         Id = he.id,
                                         Descripcion = he.descripcion,
                                         Estado = he.estado,
                                         EstadoDescrip = he.estado == true ? "Activo" : "Inactivo"
                                     };
                listaHabitacionesEstados = habitacionEstado.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesDes != "")
                {
                    listaHabitacionesEstados = listaHabitacionesEstados.Where(lhe => lhe.Descripcion.ToUpper().Contains(sesDes.Trim().ToUpper())).ToList();
                }

                listaHabitacionesEstados = listaHabitacionesEstados.OrderBy(lhe => lhe.Descripcion).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de habitaciones estados";
            }
            return View(listaHabitacionesEstados.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<HabitacionEstadoModel> listaHabitacionesEstados = new List<HabitacionEstadoModel>();
            try
            {
                //OBTENEMOS LAS HABITACIONES ESTADOS NO ELIMINADAS DE LA BASE DE DATOS
                var habitacionEstado = from he in db.habitaciones_estados
                                       where he.estado != null
                                       select new HabitacionEstadoModel
                                       {
                                           Id = he.id,
                                           Descripcion = he.descripcion,
                                           Estado = he.estado,
                                           EstadoDescrip = he.estado == true ? "Activo" : "Inactivo"
                                       };
                listaHabitacionesEstados = habitacionEstado.ToList();

                //FILTRAMOS POR NOMBRE LA BUSQUEDA
                var fcDescripcion = fc["txtHabitacionEstado"];
                if (fcDescripcion != "")
                {
                    string descripcion = Convert.ToString(fcDescripcion);
                    listaHabitacionesEstados = listaHabitacionesEstados.Where(lhe => lhe.Descripcion.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }

                listaHabitacionesEstados = listaHabitacionesEstados.OrderBy(lhe => lhe.Descripcion).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtHabitacionEstado = fcDescripcion;
                Session["sesionHabitacionesEs"] = fcDescripcion;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar habitaciones estados";
            }
            return View(listaHabitacionesEstados.ToPagedList(pageIndex, pageSize));
        }


        #endregion

        #region Crear Habitaciones Estados

        [HttpGet]
        [AutorizarUsuario("HabitacionesEstados", "Create")]
        public ActionResult Create()
        {
            HabitacionEstadoModel habitacionEstado = new HabitacionEstadoModel();
            return View(habitacionEstado);
        }

        [HttpPost]
        public ActionResult Create(HabitacionEstadoModel habitacionEstadoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE HABITACION ESTADO IGUAL EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.habitaciones_estados.Where(he => he.descripcion.Trim().ToUpper() == habitacionEstadoModelo.Descripcion.Trim().ToUpper() && he.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //AGREGAMOS EL REGISTRO DE HABITACIONES ESTADOS
                        habitaciones_estados habitacionesEstados = new habitaciones_estados
                        {
                            descripcion = habitacionEstadoModelo.Descripcion.Trim(),
                            estado = true
                        };
                        db.habitaciones_estados.Add(habitacionesEstados);
                        db.SaveChanges();
                    }
                    else
                    { 
                        ModelState.AddModelError("Duplicado", "Ya existe habitacion estado registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el habitacion estado en la base de datos");
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
                return View(habitacionEstadoModelo);
            }
        }

        #endregion


        #region Eliminar Habitacion Estado
        [HttpPost]
        public ActionResult EliminarHabitacionEstado(string habitacionEstadoId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("HabitacionesEstados", "EliminarHabitacionEstado");
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
                                //OBTENEMOS EL REGISTRO PARA PODER ELIMINAR (ACTUALIZAR ESTADO A NULL)
                                int idHabitacionEstado = Convert.ToInt32(habitacionEstadoId);
                                var habitacionEstado = context.habitaciones_estados.Where(he => he.id == idHabitacionEstado).FirstOrDefault();
                                habitacionEstado.estado = null;
                                context.Entry(habitacionEstado).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "HabitacionesEstados") }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Editar Habitaciones Estados


        [HttpGet]
        [AutorizarUsuario("HabitacionesEstados", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HabitacionEstadoModel habitacionEstadoEdit = new HabitacionEstadoModel();
            try
            {
                //OBTENEMOS EL REGISTRO Y CARGAMOS LOS DATOS EN LAS PROPIEDADES DEL MODELO
                var habitacionEstado = db.habitaciones_estados.Where(he => he.id == id).FirstOrDefault();
                habitacionEstadoEdit.Id = habitacionEstado.id;
                habitacionEstadoEdit.Descripcion = habitacionEstado.descripcion;
                habitacionEstadoEdit.Estado = habitacionEstado.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = habitacionEstado.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(habitacionEstadoEdit);
        }

        [HttpPost]
        public ActionResult Edit(HabitacionEstadoModel habitacionEstadoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE HABITACION ESTADO PARA PODER ACTUALIZAR
                    int cantidad = db.habitaciones_estados.Where(he => he.descripcion.Trim().ToUpper() == habitacionEstadoModelo.Descripcion.Trim().ToUpper() && he.estado != null && he.id != habitacionEstadoModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //OBTENEMOS EL REGISTRO DE HABITACIONES ESTADOS PARA PODER ACTUALIZAR
                        var habitacionEstado = db.habitaciones_estados.Where(he => he.id == habitacionEstadoModelo.Id).FirstOrDefault();
                        habitacionEstado.descripcion = habitacionEstadoModelo.Descripcion;
                        bool nuevoEstado = habitacionEstadoModelo.EstadoDescrip == "A" ? true : false;
                        habitacionEstado.estado = nuevoEstado;
                        db.Entry(habitacionEstado).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un habitacion estado registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el habitacion estado en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", habitacionEstadoModelo.EstadoDescrip);
                return View(habitacionEstadoModelo);
            }
        }

        #endregion

        #region Ajax
        [HttpGet]
        public ActionResult ObtenerListadoDeHabitacionesEstados(string nombre)
        {
            string respuesta = string.Empty;
            string msg = string.Empty;
            string row = string.Empty;
            try
            {
                List<habitaciones_estados> listaHabitacionesEstados = new List<habitaciones_estados>();
                if (nombre != string.Empty)
                {
                    listaHabitacionesEstados = db.habitaciones_estados.Where(s => s.descripcion.Trim().ToUpper().Contains(nombre.Trim().ToUpper()) && s.estado != null).ToList();
                }
                foreach (var item in listaHabitacionesEstados)
                {
                    var elHidden = "<input type=\"hidden\"  name=\"HabitacionesEstados\" value=\"" + item.id + "\"> ";

                    row += "<tr>" +
                        "<td>" + item.id + " </td>" +
                        "<td>" + item.descripcion + " </td>";
                    row += "<td><button type=\'button\' title='Seleccionar Habitacion Estado' class=\'btn btn-success btn-xs seleccionarHabitacionEstado\'><i class=\'glyphicon glyphicon-saved\'></i></button></td>" + "<td>" + elHidden + " </td>" + "</tr>";
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