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
    public class TiposHabitacionesController : Controller
    {

        /*
     * TIPOS HABITACIONES
     * 
     * El modulo de tipos habitaciones registra los tipos de habitaciones de la empresa. Está relacionada con los módulos de habitaciones y habitaciones estados. 
    
     */

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Tipos Habitaciones

        [HttpGet]
        [AutorizarUsuario("TiposHabitaciones", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TipoHabitacionModel> listaTiposHabitaciones = new List<TipoHabitacionModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNom = Convert.ToString(Session["sesionTiposHabitacionesNombre"]);
                ViewBag.txtTipoHabitacion = sesNom;
                string sesAbre = Convert.ToString(Session["sesionAbreviatura"]);
                ViewBag.txtAbreviatura = sesAbre;

                //OBTENEMOS TODOS LOS TIPOS DE HABITACIONES NO ELIMINADAS DE LA BASE DE DATOS
                var tipoHabitacion = from th in db.habitaciones_tipos
                            where th.estado != null
                            select new TipoHabitacionModel
                            {
                                Id = th.id,
                                Nombre = th.nombre,
                                Abreviatura = th.abreviatura,
                                Estado = th.estado,
                                EstadoDescrip = th.estado == true ? "Activo" : "Inactivo"
                            };
                listaTiposHabitaciones = tipoHabitacion.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNom != "")
                {
                    listaTiposHabitaciones = listaTiposHabitaciones.Where(lth => lth.Nombre.ToUpper().Contains(sesNom.Trim().ToUpper())).ToList();
                }

                if (sesAbre != "")
                {
                    listaTiposHabitaciones = listaTiposHabitaciones.Where(lth => lth.Abreviatura.ToUpper().Contains(sesAbre.Trim().ToUpper())).ToList();
                }

                listaTiposHabitaciones = listaTiposHabitaciones.OrderBy(lth => lth.Nombre).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de tipos habitaciones";
            }
            return View(listaTiposHabitaciones .ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TipoHabitacionModel> listaTiposHabitaciones = new List<TipoHabitacionModel>();
            try
            {
                //OBTENEMOS TODOS LOS TIPOS HABITACIONES NO ELIMINADAS DE LA BASE DE DATOS
                var tipoHabitacion = from th in db.habitaciones_tipos
                                     where th.estado != null
                                     select new TipoHabitacionModel
                                     {
                                         Id = th.id,
                                         Nombre = th.nombre,
                                         Abreviatura = th.abreviatura,
                                         Estado = th.estado,
                                         EstadoDescrip = th.estado == true ? "Activo" : "Inactivo"
                                     };
                listaTiposHabitaciones = tipoHabitacion.ToList();

                //FILTRAMOS POR NOMBRE Y ABREVIATURA LA BUSQUEDA
                var fcNombre = fc["txtTipoHabitacion"];
                if (fcNombre != "")
                {
                    string descripcion = Convert.ToString(fcNombre);
                    listaTiposHabitaciones = listaTiposHabitaciones.Where(lth => lth.Nombre.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }

                var fcAbreviatura = fc["txtAbreviatura"];
                if (fcAbreviatura != "")
                {
                    string descripcion = Convert.ToString(fcAbreviatura);
                    listaTiposHabitaciones = listaTiposHabitaciones.Where(lth => lth.Abreviatura.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }

                listaTiposHabitaciones = listaTiposHabitaciones.OrderBy(lth => lth.Nombre).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtTipoHabitacion = fcNombre;
                Session["sesionTiposHabitacionesNombre"] = fcNombre;
                ViewBag.txtAbreviatura = fcAbreviatura;
                Session["sesionAbreviatura"] = fcAbreviatura;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar tipos habitaciones";
            }
            return View(listaTiposHabitaciones.ToPagedList(pageIndex, pageSize));
        }


        #endregion

        #region Crear Tipos Habitaciones

        [HttpGet]
        [AutorizarUsuario("TiposHabitaciones", "Create")]
        public ActionResult Create()
        {
            TipoHabitacionModel tipoHabitacion = new TipoHabitacionModel();
            return View(tipoHabitacion);
        }

        [HttpPost]
        public ActionResult Create(TipoHabitacionModel tipoHabitacionModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL TIPO HABITACION EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.habitaciones_tipos.Where(ht => ht.nombre.Trim().ToUpper() == tipoHabitacionModelo.Nombre.Trim().ToUpper() && ht.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //AGREGAMOS EL REGISTRO DE TIPOS HABITACIONES
                        habitaciones_tipos habitacionesTipos = new habitaciones_tipos
                        {
                            nombre = tipoHabitacionModelo.Nombre.Trim(),
                            abreviatura = tipoHabitacionModelo.Abreviatura.Trim(),
                            estado = true
                        };
                        db.habitaciones_tipos.Add(habitacionesTipos);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe tipo habitacion registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el tipo habitacion en la base de datos");
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
                return View(tipoHabitacionModelo);
            }
        }

        #endregion

        #region Editar Tipos Habitaciones

        [HttpGet]
        [AutorizarUsuario("TiposHabitaciones", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TipoHabitacionModel tipoHabitacionEdit = new TipoHabitacionModel();
            try
            {
                //OBTENEMOS EL REGISTRO Y CARGAMOS LOS DATOS EN LAS PROPIEDADES DEL MODELO
                var tipoHabitacion = db.habitaciones_tipos.Where(ht => ht.id == id).FirstOrDefault();
                tipoHabitacionEdit.Id = tipoHabitacion.id;
                tipoHabitacionEdit.Nombre = tipoHabitacion.nombre;
                tipoHabitacionEdit.Abreviatura = tipoHabitacion.abreviatura;
                tipoHabitacionEdit.Estado = tipoHabitacion.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = tipoHabitacion.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
                
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(tipoHabitacionEdit);
        }

        [HttpPost]
        public ActionResult Edit(TipoHabitacionModel tipoHabitacionModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE TIPO DE HABITACION PARA PODER ACTUALIZAR
                    int cantidad = db.habitaciones_tipos.Where(ht => ht.nombre.Trim().ToUpper() == tipoHabitacionModelo.Nombre.Trim().ToUpper() && ht.estado != null && ht.id != tipoHabitacionModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //OBTENEMOS EL REGISTRO DE TIPOS HABITACIONES PARA PODER ACTUALIZAR
                        var tipoHabitacion = db.habitaciones_tipos.Where(ht => ht.id == tipoHabitacionModelo.Id).FirstOrDefault();
                        tipoHabitacion.nombre = tipoHabitacionModelo.Nombre;
                        tipoHabitacion.abreviatura = tipoHabitacionModelo.Abreviatura;
                        bool nuevoEstado = tipoHabitacionModelo.EstadoDescrip == "A" ? true : false;
                        tipoHabitacion.estado = nuevoEstado;
                        db.Entry(tipoHabitacion).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un tipo de habitacion registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el tipo de habitacion en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", tipoHabitacionModelo.EstadoDescrip);
                return View(tipoHabitacionModelo);
            }
        }

        #endregion

        #region Eliminar Tipo Habitacion
        [HttpPost]
        public ActionResult EliminarTipoHabitacion(string tipoHabitacionId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("TiposHabitaciones", "EliminarTipoHabitacion");
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
                                int idTipoHabitacion = Convert.ToInt32(tipoHabitacionId);
                                var tipoHabitacion = context.habitaciones_tipos.Where(ht => ht.id == idTipoHabitacion).FirstOrDefault();
                                tipoHabitacion.estado = null;
                                context.Entry(tipoHabitacion).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "TiposHabitaciones") }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Ajax
        [HttpGet]
        public ActionResult ObtenerListadoTiposServicios(string descripcion)
        {
            string respuesta = string.Empty;
            string msg = string.Empty;
            string row = string.Empty;
            try
            {
                List<habitaciones_tipos> listaTiposServicios = new List<habitaciones_tipos>();
                if (descripcion != string.Empty)
                {
                    listaTiposServicios = db.habitaciones_tipos.Where(s => s.nombre.Trim().ToUpper().Contains(descripcion.Trim().ToUpper()) && s.estado != null).ToList();
                }
                foreach (var item in listaTiposServicios)
                {
                    var elHidden = "<input type=\"hidden\"  name=\"TiposHabitaciones\" value=\"" + item.id + "\"> ";

                    row += "<tr>" +
                        "<td>" + item.id + " </td>" +
                        "<td>" + item.nombre + " </td>" +
                         "<td>" + item.abreviatura + " </td>";
                    row += "<td><button type=\'button\' title='Seleccionar Tipo Habitacion' class=\'btn btn-success btn-xs seleccionarTipoHabitacion\'><i class=\'glyphicon glyphicon-saved\'></i></button></td>" + "<td>" + elHidden + " </td>" + "</tr>";
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