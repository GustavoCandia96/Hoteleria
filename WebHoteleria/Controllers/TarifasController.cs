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
    public class TarifasController : Controller
    {

        #region Propiedad
        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();
        #endregion

        #region Listado de Tarifas

        [HttpGet]
        [AutorizarUsuario("Tarifas", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TarifaModel> listaTarifas = new List<TarifaModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomTarifa = Convert.ToString(Session["sesionTarifasNombre"]);
                ViewBag.txtNombreTarifa = sesNomTarifa;
                string sesIdTipoTarifa = Convert.ToString(Session["sesionTarifasIdTipoTarifa"]);
                ViewBag.ddlTiposTarifas = new SelectList(db.tarifas_tipos.Where(tt => tt.estado == true).OrderBy(tt => tt.descripcion).ToList(), "id", "descripcion", sesIdTipoTarifa);

                //OBTENEMOS TODOS LAS TARIFAS ACTIVOS DE LA BASE DE DATOS Y ORDENAMOS POR DESCRIPCIÓN
                var tarifas = from t in db.tarifas
                              where t.estado != null
                              select new TarifaModel
                              {
                                  Id = t.id,
                                  IdTipoTarifa = t.id_tipo_tarifa,
                                  NombreTarifa = t.nombre_tarifa,
                                  ValidoDesde = t.valido_desde,
                                  ValidoHasta = t.valido_hasta,
                                  Estado = t.estado,
                                  NombreTipoTarifa = t.tarifas_tipos.descripcion,
                                  EstadoDescrip = t.estado == true ? "Activo" : "Inactivo"
                              };

                listaTarifas = tarifas.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomTarifa != "")
                {
                    listaTarifas = listaTarifas.Where(t => t.NombreTarifa.ToUpper().Contains(sesNomTarifa.Trim().ToUpper())).ToList();
                }
                if (sesIdTipoTarifa != "")
                {
                    int idTipoTarifa = Convert.ToInt32(sesIdTipoTarifa);
                    listaTarifas = listaTarifas.Where(t => t.IdTipoTarifa == idTipoTarifa).ToList();
                }

                listaTarifas = listaTarifas.OrderBy(t => t.ValidoDesde).ToList();

                foreach (var item in listaTarifas)
                {
                    item.StrValidoDesde = item.ValidoDesde.Value.ToShortDateString();
                    item.StrValidoHasta = item.ValidoHasta.Value.ToShortDateString();
                }

            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de tarifas";
            }
            return View(listaTarifas.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<TarifaModel> listaTarifas = new List<TarifaModel>();
            try
            {
                //OBTENEMOS TODOS LAS TARIFAS ACTIVOS DE LA BASE DE DATOS Y ORDENAMOS POR DESCRIPCIÓN
                var tarifas = from t in db.tarifas
                              where t.estado != null
                              select new TarifaModel
                              {
                                  Id = t.id,
                                  IdTipoTarifa = t.id_tipo_tarifa,
                                  NombreTarifa = t.nombre_tarifa,
                                  ValidoDesde = t.valido_desde,
                                  ValidoHasta = t.valido_hasta,
                                  Estado = t.estado,
                                  NombreTipoTarifa = t.tarifas_tipos.descripcion,
                                  EstadoDescrip = t.estado == true ? "Activo" : "Inactivo"
                              };

                listaTarifas = tarifas.ToList();

                //FILTRAMOS POR NOMBRE TARIFA Y TIPO TARIFA EN BUSQUEDA
                var fcNombreTarifa = fc["txtNombreTarifa"];
                if (fcNombreTarifa != "")
                {
                    string descripcion = Convert.ToString(fcNombreTarifa);
                    listaTarifas = listaTarifas.Where(t => t.NombreTarifa.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }

                var fcIdTipoTarifa = fc["ddlTiposTarifas"];
                if (fcIdTipoTarifa != "")
                {
                    int idTipoTarifa = Convert.ToInt32(fcIdTipoTarifa);
                    listaTarifas = listaTarifas.Where(t => t.IdTipoTarifa == idTipoTarifa).ToList();
                }

                listaTarifas = listaTarifas.OrderBy(t => t.ValidoDesde).ToList();

                foreach (var item in listaTarifas)
                {
                    item.StrValidoDesde = item.ValidoDesde.Value.ToShortDateString();
                    item.StrValidoHasta = item.ValidoHasta.Value.ToShortDateString();
                }

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtNombreTarifa = fcNombreTarifa;
                Session["sesionTarifasNombre"] = fcNombreTarifa;
                ViewBag.ddlTiposTarifas = new SelectList(db.tarifas_tipos.Where(tt => tt.estado == true).OrderBy(tt => tt.descripcion).ToList(), "id", "descripcion", fcIdTipoTarifa);
                Session["sesionTarifasIdTipoTarifa"] = fcIdTipoTarifa;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar tarifas";
            }
            return View(listaTarifas.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Tarifa

        [HttpGet]
        [AutorizarUsuario("Tarifas", "Create")]
        public ActionResult Create()
        {
            TarifaModel tarifa = new TarifaModel();
            ViewBag.IdTipoTarifa = new SelectList(db.tarifas_tipos.Where(tt => tt.estado == true).OrderBy(tt => tt.descripcion).ToList(), "id", "descripcion");
            return View(tarifa);
        }

        [HttpPost]
        public ActionResult Create(TarifaModel tarifaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA TARIFA CON EL MISMO TIPO PARA PODER AGREGAR
                    int cantidad = db.tarifas.Where(t => t.nombre_tarifa.ToUpper() == tarifaModelo.NombreTarifa.ToUpper() && t.id_tipo_tarifa == tarifaModelo.IdTipoTarifa && t.estado != null).Count();
                    if (cantidad == 0)
                    {
                        DateTime validoDesde = Convert.ToDateTime(tarifaModelo.StrValidoDesde);
                        DateTime validoHasta = Convert.ToDateTime(tarifaModelo.StrValidoHasta);

                        tarifas tarifa = new tarifas
                        {
                            id_tipo_tarifa = tarifaModelo.IdTipoTarifa,
                            nombre_tarifa = tarifaModelo.NombreTarifa,
                            valido_desde = validoDesde,
                            valido_hasta = validoHasta,
                            estado = true
                        };
                        db.tarifas.Add(tarifa);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una tarifa registrada con el mismo nombre y tipo");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la tarifa en la base de datos");
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
                ViewBag.IdTipoTarifa = new SelectList(db.tarifas_tipos.Where(tt => tt.estado == true).OrderBy(tt => tt.descripcion).ToList(), "id", "descripcion", tarifaModelo.IdTipoTarifa);
                db.Dispose();
                return View(tarifaModelo);
            }
        }

        #endregion

        #region Editar Tarifa

        [HttpGet]
        [AutorizarUsuario("Tarifas", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TarifaModel tarifaEdit = new TarifaModel();
            try
            {
                var tarifa = db.tarifas.Where(t => t.id == id).FirstOrDefault();
                tarifaEdit.Id = tarifa.id;
                tarifaEdit.IdTipoTarifa = tarifa.id_tipo_tarifa;
                tarifaEdit.NombreTarifa = tarifa.nombre_tarifa;
                tarifaEdit.ValidoDesde = tarifa.valido_desde;
                tarifaEdit.ValidoHasta = tarifa.valido_hasta;
                tarifaEdit.StrValidoDesde = tarifa.valido_desde.Value.ToShortDateString();
                tarifaEdit.StrValidoHasta = tarifa.valido_hasta.Value.ToShortDateString();
                tarifaEdit.Estado = tarifa.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = tarifa.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                ViewBag.IdTipoTarifa = new SelectList(db.tarifas_tipos.Where(tt => tt.estado == true).OrderBy(tt => tt.descripcion).ToList(), "id", "descripcion", tarifaEdit.IdTipoTarifa);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(tarifaEdit);
        }

        [HttpPost]
        public ActionResult Edit(TarifaModel tarifaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA TARIFA CON EL MISMO TIPO PARA PODER AGREGAR
                    int cantidad = db.tarifas.Where(t => t.nombre_tarifa.ToUpper() == tarifaModelo.NombreTarifa.ToUpper() && t.id_tipo_tarifa == tarifaModelo.IdTipoTarifa && t.estado != null && t.id != tarifaModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        DateTime validoDesde = Convert.ToDateTime(tarifaModelo.StrValidoDesde);
                        DateTime validoHasta = Convert.ToDateTime(tarifaModelo.StrValidoHasta);

                        var tarifa = db.tarifas.Where(t => t.id == tarifaModelo.Id).FirstOrDefault();
                        tarifa.id_tipo_tarifa = tarifaModelo.IdTipoTarifa;
                        tarifa.nombre_tarifa = tarifaModelo.NombreTarifa;
                        tarifa.valido_desde = validoDesde;
                        tarifa.valido_hasta = validoHasta;
                        bool nuevoEstado = tarifaModelo.EstadoDescrip == "A" ? true : false;
                        tarifa.estado = nuevoEstado;
                        db.Entry(tarifa).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una tarifa registrada con el mismo nombre y tipo");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar la tarifa en la base de datos");
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
                ViewBag.IdTipoTarifa = new SelectList(db.tarifas_tipos.Where(tt => tt.estado == true).OrderBy(tt => tt.descripcion).ToList(), "id", "descripcion", tarifaModelo.IdTipoTarifa);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", tarifaModelo.EstadoDescrip);
                db.Dispose();
                return View(tarifaModelo);
            }
        }

        #endregion

        #region Eliminar Tarifa

        [HttpPost]
        public ActionResult EliminarTarifa(string tarifaId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Tarifas", "EliminarTarifa");
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
                                int idTarifa = Convert.ToInt32(tarifaId);
                                var tarifa = context.tarifas.Where(t => t.id == idTarifa).FirstOrDefault();
                                tarifa.estado = null;
                                context.Entry(tarifa).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();

                                var detallesTarifa = context.tarifas_detalles.Where(td => td.id_tarifa == tarifa.id).ToList();
                                foreach (var item in detallesTarifa)
                                {
                                    var detalle = context.tarifas_detalles.Where(td => td.id == item.id).FirstOrDefault();
                                    detalle.estado = null;
                                    context.Entry(detalle).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }

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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Tarifas") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Detalle Tarifa

        [HttpGet]
        [AutorizarUsuario("Tarifas", "DetallesTarifa")]
        public ActionResult DetallesTarifa(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TarifaDetalleModel tarifaDetalleEdit = new TarifaDetalleModel();
            try
            {
                //OBTENEMOS LA TARIFA SELECCIONADA Y POSTERIORMENTE CARGAMOS TODOS LOS DETALLES RELACIONADOS
                var tarifa = db.tarifas.Where(t => t.id == id).FirstOrDefault();
                tarifaDetalleEdit.IdTarifa = tarifa.id;
                tarifaDetalleEdit.NombreTarifa = tarifa.nombre_tarifa;
                tarifaDetalleEdit.NombreTipoTarifa = tarifa.tarifas_tipos.descripcion;

                CargarDatosDetalleTarifa();
                List<TarifaDetalleModel> listaDetalleTarifa = new List<TarifaDetalleModel>();
                var detallesTarifas = db.tarifas_detalles.Where(td => td.id_tarifa == tarifa.id && td.estado == true).ToList();
                foreach (var item in detallesTarifas)
                {
                    TarifaDetalleModel carga = new TarifaDetalleModel
                    {
                        Id = item.id,
                        IdTarifa = item.id_tarifa,
                        IdHabitacion = item.id_habitacion,
                        IdServicioHabitacion = item.id_servicio_habitacion,
                        Precio = item.precio,
                        Estado = item.estado.Value,
                        NombreTarifa = item.tarifas.nombre_tarifa,
                        NombreTipoHabitacion = item.habitaciones.habitaciones_tipos.nombre,
                        NombreHabitacion = item.habitaciones.numero,
                        NombreServicioHabitacion = item.habitaciones_servicios.nombre_servicio,
                        StrPrecio = String.Format("{0:#,##0.##}", item.precio)
                    };
                    listaDetalleTarifa.Add(carga);
                }
                listaDetalleTarifa = listaDetalleTarifa.OrderBy(ldt => ldt.NombreHabitacion).ToList();
                ViewBag.ListaDetalleTarifa = listaDetalleTarifa;
                ViewBag.ListaDetalleTarifaEliminado = null;
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(tarifaDetalleEdit);
        }

        [HttpPost]
        public ActionResult DetallesTarifa(TarifaDetalleModel detalleTarifaModelo, FormCollection fc)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            //LISTADO DE DETALLE TARIFA AGREGADOS
            string[] arrId = (fc["arrId"] != null ? fc["arrId"].Split(',') : new string[] { });
            string[] arrHabitacionId = (fc["arrHabitacionId"] != null ? fc["arrHabitacionId"].Split(',') : new string[] { });
            string[] arrServicioHabitacionId = (fc["arrServicioHabitacionId"] != null ? fc["arrServicioHabitacionId"].Split(',') : new string[] { });
            string[] arrPrecio = (fc["arrPrecio"] != null ? fc["arrPrecio"].Split(',') : new string[] { });

            //LISTADO DE DETALLE TARIFA ELIMINADOS
            string[] arrIdEliminado = (fc["arrIdEliminado"] != null ? fc["arrIdEliminado"].Split(',') : new string[] { });
            string[] arrHabitacionIdEliminado = (fc["arrHabitacionIdEliminado"] != null ? fc["arrHabitacionIdEliminado"].Split(',') : new string[] { });
            string[] arrServicioHabitacionIdEliminado = (fc["arrServicioHabitacionIdEliminado"] != null ? fc["arrServicioHabitacionIdEliminado"].Split(',') : new string[] { });
            string[] arrPrecioEliminado = (fc["arrPrecioEliminado"] != null ? fc["arrPrecioEliminado"].Split(',') : new string[] { });

            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN DETALLE DE TARIFA
                    if (arrId.Length > 0)
                    {
                        var tarifa = db.tarifas.Where(t => t.id == detalleTarifaModelo.IdTarifa).FirstOrDefault();

                        //VERIFICAMOS SI VAMOS A AGREGAR O ACTUALIZAR EL LISTADO DE DETALLE TARIFA
                        int countDetalleTarifa = db.tarifas_detalles.Where(td => td.id_tarifa == tarifa.id && td.estado != null).ToList().Count;
                        if (countDetalleTarifa == 0) //AGREMOS DETALLES TARIFA
                        {
                            for (int i = 0; i < arrHabitacionId.Length; i++)
                            {
                                int idHabitacion = Convert.ToInt32(arrHabitacionId[i]);
                                int idServicioHabitacion = Convert.ToInt32(arrServicioHabitacionId[i]);
                                string strPrecio = Convert.ToString(arrPrecio[i]);
                                strPrecio = strPrecio.Replace(".", "");
                                decimal precio = Convert.ToDecimal(strPrecio);

                                tarifas_detalles tarifaDetalle = new tarifas_detalles
                                {
                                    id_tarifa = tarifa.id,
                                    id_habitacion = idHabitacion,
                                    id_servicio_habitacion = idServicioHabitacion,
                                    precio = precio,
                                    estado = true
                                };
                                db.tarifas_detalles.Add(tarifaDetalle);
                                db.SaveChanges();
                            }
                        }
                        else //ACTUALIZAMOS DETALLES TARIFA
                        {
                            for (int i = 0; i < arrHabitacionId.Length; i++)
                            {
                                int id = Convert.ToInt32(arrId[i]);
                                int idHabitacion = Convert.ToInt32(arrHabitacionId[i]);
                                int idServicioHabitacion = Convert.ToInt32(arrServicioHabitacionId[i]);
                                string strPrecio = Convert.ToString(arrPrecio[i]);
                                strPrecio = strPrecio.Replace(".", "");
                                decimal precio = Convert.ToDecimal(strPrecio);

                                if (id == 0)
                                {
                                    tarifas_detalles tarifaDetalle = new tarifas_detalles
                                    {
                                        id_tarifa = tarifa.id,
                                        id_habitacion = idHabitacion,
                                        id_servicio_habitacion = idServicioHabitacion,
                                        precio = precio,
                                        estado = true
                                    };
                                    db.tarifas_detalles.Add(tarifaDetalle);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    var detalle = db.tarifas_detalles.Where(td => td.id == id).FirstOrDefault();
                                    detalle.id_tarifa = tarifa.id;
                                    detalle.id_habitacion = idHabitacion;
                                    detalle.id_servicio_habitacion = idServicioHabitacion;
                                    detalle.precio = precio;
                                    detalle.estado = true;
                                    db.Entry(detalle).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }

                            for (int i = 0; i < arrIdEliminado.Length; i++)
                            {
                                int idEliminado = Convert.ToInt32(arrIdEliminado[i]);
                                if (idEliminado != 0)
                                {
                                    var detalle = db.tarifas_detalles.Where(td => td.id == idEliminado).FirstOrDefault();
                                    detalle.estado = null;
                                    db.Entry(detalle).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("SinDetalle", "Tiene que ingresar los detalles de la tarifa seleccionada");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar detalles de tarifa en la base de datos");
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
                CargarDatosDetalleTarifa();
                CargarItemsDetalleTarifa(arrId, arrHabitacionId, arrServicioHabitacionId, arrPrecio);
                CargarItemsDetalleTarifaEliminado(arrIdEliminado, arrHabitacionIdEliminado, arrServicioHabitacionIdEliminado, arrPrecioEliminado);
                db.Dispose();
                return View(detalleTarifaModelo);
            }
        }


        #endregion

        #region Funciones

        private void CargarDatosDetalleTarifa()
        {
            ViewBag.IdTipoHabitacion = new SelectList(db.habitaciones_tipos.Where(ht => ht.estado == true).OrderBy(ht => ht.nombre).ToList(), "id", "nombre");
            ViewBag.IdHabitacion = new SelectList(db.habitaciones.Where(h => h.id == 0).OrderBy(h => h.numero).ToList(), "id", "numero");
            ViewBag.IdServicioHabitacion = new SelectList(db.habitaciones_servicios.Where(hs => hs.estado == true).OrderBy(hs => hs.nombre_servicio).ToList(), "id", "nombre_servicio");
        }

        private void CargarItemsDetalleTarifa(string[] arrId, string[] arrHabitacionId, string[] arrServicioHabitacionId, string[] arrPrecio)
        {
            List<TarifaDetalleModel> listaDetalle = new List<TarifaDetalleModel>();
            for (int i = 0; i < arrId.Length; i++)
            {
                TarifaDetalleModel carga = new TarifaDetalleModel();
                carga.Id = Convert.ToInt32(arrId[i]);
                carga.IdHabitacion = Convert.ToInt32(arrHabitacionId[i]);
                carga.IdServicioHabitacion = Convert.ToInt32(arrServicioHabitacionId[i]);
                var habitacion = db.habitaciones.Where(h => h.id == carga.IdHabitacion).FirstOrDefault();
                var servicioHabitacion = db.habitaciones_servicios.Where(hs => hs.id == carga.IdServicioHabitacion).FirstOrDefault();
                carga.NombreTipoHabitacion = habitacion.habitaciones_tipos.nombre;
                carga.NombreHabitacion = habitacion.numero;
                carga.NombreServicioHabitacion = servicioHabitacion.nombre_servicio;
                carga.StrPrecio = Convert.ToString(arrPrecio[i]);
                listaDetalle.Add(carga);
            }
            ViewBag.ListaDetalleTarifa = listaDetalle.Count > 0 ? listaDetalle : null;
        }

        private void CargarItemsDetalleTarifaEliminado(string[] arrIdEliminado, string[] arrHabitacionIdEliminado, string[] arrServicioHabitacionIdEliminado, string[] arrPrecioEliminado)
        {
            List<TarifaDetalleModel> listaDetalleEliminado = new List<TarifaDetalleModel>();
            for (int i = 0; i < arrIdEliminado.Length; i++)
            {
                TarifaDetalleModel carga = new TarifaDetalleModel();
                carga.Id = Convert.ToInt32(arrIdEliminado[i]);
                carga.IdHabitacion = Convert.ToInt32(arrHabitacionIdEliminado[i]);
                carga.IdServicioHabitacion = Convert.ToInt32(arrServicioHabitacionIdEliminado[i]);
                var habitacion = db.habitaciones.Where(h => h.id == carga.IdHabitacion).FirstOrDefault();
                var servicioHabitacion = db.habitaciones_servicios.Where(hs => hs.id == carga.IdServicioHabitacion).FirstOrDefault();
                carga.NombreTipoHabitacion = habitacion.habitaciones_tipos.nombre;
                carga.NombreHabitacion = habitacion.numero;
                carga.NombreServicioHabitacion = servicioHabitacion.nombre_servicio;
                carga.StrPrecio = Convert.ToString(arrPrecioEliminado[i]);
                listaDetalleEliminado.Add(carga);
            }
            ViewBag.ListaDetalleTarifaEliminado = listaDetalleEliminado.Count > 0 ? listaDetalleEliminado : null;
        }

        #endregion

    }
}