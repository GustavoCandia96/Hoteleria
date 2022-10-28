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
    public class CajasAjustesController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();
        private int IdSucursalUsuario;
        private int IdFuncionario;
        private int IdUsuario;

        #endregion

        #region Listado de Cajas Ajustes

        [HttpGet]
        [AutorizarUsuario("CajasAjustes", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CajaAjusteModel> listaCajasAjustes = new List<CajaAjusteModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesFecha = Convert.ToString(Session["sesionCajasAjustesFecha"]);
                ViewBag.txtFecha = sesFecha;
                string sesIdSucursal = Convert.ToString(Session["sesionCajasAjustesIdSucursal"]);
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", sesIdSucursal);

                CargarDatosUsuarioSesion();

                //OBTENEMOS TODAS LAS CAJAS AJUSTES NO ELIMINADAS DE LA BASE DE DATOS
                var cajasAjustes = from ca in db.cajas_ajustes
                                   where ca.estado != null
                                   select new CajaAjusteModel
                                   {
                                       Id = ca.id,
                                       IdCaja = ca.id_caja,
                                       IdCajaApertura = ca.id_caja_apertura,
                                       IdUsuario = ca.id_usuario,
                                       Fecha = ca.fecha,
                                       Faltante = ca.faltante.Value,
                                       MontoAjuste = ca.monto_ajuste,
                                       Justificacion = ca.justificacion,
                                       Estado = ca.estado.Value,
                                       NombreUsuarioCaja = ca.usuarios.id_funcionario != null ? ca.usuarios.funcionarios.nombre + " " + ca.usuarios.funcionarios.apellido : "",
                                       IdSucursal = ca.cajas.id_sucursal,
                                       NombreSucursal = ca.cajas.sucursales.nombre_sucursal,
                                       NombreCaja = ca.cajas.denominacion,
                                       NombreApertura = ca.cajas_aperturas.nombre_apertura,
                                       NombreTipoAjuste = ca.faltante == true ? "Faltante" : "Sobrante",
                                       HabilitadoEdicion = ca.cajas_aperturas.caja_abierta == true ? true : false,
                                       EstadoDescrip = ca.estado == true ? "Activo" : "Inactivo"
                                   };
                listaCajasAjustes = cajasAjustes.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesFecha != "")
                {
                    DateTime fecha = Convert.ToDateTime(sesFecha);
                    listaCajasAjustes = listaCajasAjustes.Where(ca => ca.Fecha >= fecha).ToList();
                }

                if (sesIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(sesIdSucursal);
                    listaCajasAjustes = listaCajasAjustes.Where(ca => ca.IdSucursal >= idSucursal).ToList();
                }

                listaCajasAjustes = listaCajasAjustes.OrderByDescending(ca => ca.Fecha).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de cajas ajustes";
            }
            return View(listaCajasAjustes.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CajaAjusteModel> listaCajasAjustes = new List<CajaAjusteModel>();
            try
            {
                CargarDatosUsuarioSesion();

                //OBTENEMOS TODAS LAS CAJAS AJUSTES NO ELIMINADAS DE LA BASE DE DATOS
                var cajasAjustes = from ca in db.cajas_ajustes
                                   where ca.estado != null
                                   select new CajaAjusteModel
                                   {
                                       Id = ca.id,
                                       IdCaja = ca.id_caja,
                                       IdCajaApertura = ca.id_caja_apertura,
                                       IdUsuario = ca.id_usuario,
                                       Fecha = ca.fecha,
                                       Faltante = ca.faltante.Value,
                                       MontoAjuste = ca.monto_ajuste,
                                       Justificacion = ca.justificacion,
                                       Estado = ca.estado.Value,
                                       NombreUsuarioCaja = ca.usuarios.id_funcionario != null ? ca.usuarios.funcionarios.nombre + " " + ca.usuarios.funcionarios.apellido : "",
                                       IdSucursal = ca.cajas.id_sucursal,
                                       NombreSucursal = ca.cajas.sucursales.nombre_sucursal,
                                       NombreCaja = ca.cajas.denominacion,
                                       NombreApertura = ca.cajas_aperturas.nombre_apertura,
                                       NombreTipoAjuste = ca.faltante == true ? "Faltante" : "Sobrante",
                                       HabilitadoEdicion = ca.cajas_aperturas.caja_abierta == true ? true : false,
                                       EstadoDescrip = ca.estado == true ? "Activo" : "Inactivo"
                                   };
                listaCajasAjustes = cajasAjustes.ToList();

                //FILTRAMOS POR NOMBRE AREA LA BUSQUEDA
                var fcFecha = fc["txtFecha"];
                if (fcFecha != "")
                {
                    DateTime fecha = Convert.ToDateTime(fcFecha);
                    listaCajasAjustes = listaCajasAjustes.Where(ca => ca.Fecha >= fecha).ToList();
                }

                var fcIdSucursal = fc["ddlSucursales"];
                if (fcIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(fcIdSucursal);
                    listaCajasAjustes = listaCajasAjustes.Where(ca => ca.IdSucursal >= idSucursal).ToList();
                }

                listaCajasAjustes = listaCajasAjustes.OrderByDescending(ca => ca.Fecha).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtFecha = fcFecha;
                Session["sesionCajasAjustesFecha"] = fcFecha;
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", fcIdSucursal);
                Session["sesionCajasAjustesIdSucursal"] = fcIdSucursal;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar cajas ajustes";
            }
            return View(listaCajasAjustes.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Caja Ajuste

        [HttpGet]
        [AutorizarUsuario("CajasAjustes", "Create")]
        public ActionResult Create()
        {
            CajaAjusteModel cajaAjusteModelo = new CajaAjusteModel();
            CargarDatosUsuarioSesion();
            cajaAjusteModelo.IdUsuario = IdUsuario;

            var cajaApertura = db.cajas_aperturas.Where(ca => ca.caja_abierta == true && ca.cajas.id_usuario == IdUsuario && ca.estado != null).FirstOrDefault();
            if (cajaApertura != null)
            {
                cajaAjusteModelo.NombreCaja = cajaApertura.cajas.denominacion;
                cajaAjusteModelo.NombreApertura = cajaApertura.nombre_apertura;
            }

            CargarDatosCajaAjuste(cajaAjusteModelo);
            return View(cajaAjusteModelo);
        }

        [HttpPost]
        public ActionResult Create(CajaAjusteModel cajaAjusteModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;

            if (ModelState.IsValid) //SI EL MODELO ES VALIDO
            {
                try
                {
                    //OBTENEMOS LOS DATOS DEL USUARIO EN SESION
                    CargarDatosUsuarioSesion();

                    //VERIFICAMOS SI HAY AJUSTE SOBRANTE O FALTANTE DEPENDIENDO SI SE AGREGO UN AJUSTE SOBRANTE O FALTANTE
                    //SI HAY REGISTRO DE SOBRANTE NO SE PUEDE AGREGAR FALTANTE Y VICEVERSA
                    bool habilitado = true;
                    bool faltante = cajaAjusteModelo.IdTipoAjuste == 1 ? false : true;

                    //OBTENEMOS EL TIPO DE CAJA (RECAUDACION O FONDO PREMIO PARA PODER AGREGAR EL AJUSTE)
                    int? idCajaApertura = null;
                    int? idCaja = null;

                    var cajaApertura = db.cajas_aperturas.Where(ca => ca.id_usuario == IdUsuario && ca.caja_abierta == true && ca.estado != null).FirstOrDefault();
                    if (cajaApertura != null)
                    {
                        idCajaApertura = cajaApertura.id;
                        idCaja = cajaApertura.id_caja;

                        var count = db.cajas_ajustes.Where(ca => ca.faltante == faltante && ca.id_caja_apertura == cajaApertura.id && ca.estado != null).Count();
                        habilitado = count > 0 ? false : true;
                    }

                    if (habilitado == true)
                    {
                        string strMonto = cajaAjusteModelo.StrMontoAjuste;
                        strMonto = strMonto.Replace(".", "");
                        decimal montoAjuste = Convert.ToDecimal(strMonto);

                        if (idCajaApertura != null)
                        {
                            //AGREGAMOS EL REGISTRO DE AJUSTE DE CAJA
                            cajas_ajustes cajaAjuste = new cajas_ajustes
                            {
                                id_caja = idCaja,
                                id_caja_apertura = idCajaApertura,
                                id_usuario = IdUsuario,
                                fecha = DateTime.Now,
                                faltante = cajaAjusteModelo.IdTipoAjuste == 1 ? true : false,
                                monto_ajuste = montoAjuste,
                                justificacion = cajaAjusteModelo.Justificacion,
                                estado = true
                            };
                            db.cajas_ajustes.Add(cajaAjuste);
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("Error", "No existe ninguna caja abierta relacionada al usuario");
                            retornoVista = true;
                        }
                    }
                    else
                    {
                        string descripcion = faltante == true ? "faltante" : "sobrante";
                        ModelState.AddModelError("Error", "Ya existe ajuste de tipo " + descripcion + ". Solo podras agregar ajuste de tipo " + descripcion);
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la caja en la base de datos");
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
                CargarDatosCajaAjuste(cajaAjusteModelo);
                return View(cajaAjusteModelo);
            }
        }

        #endregion

        #region Editar Caja Ajuste

        [HttpGet]
        [AutorizarUsuario("CajasAjustes", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CajaAjusteModel cajaAjusteEdit = new CajaAjusteModel();
            try
            {
                //OBTENEMOS EL REGISTRO Y CARGAMOS LOS DATOS EN LAS PROPIEDADES DEL MODELO
                var cajaAjuste = db.cajas_ajustes.Where(ca => ca.id == id).FirstOrDefault();

                cajaAjusteEdit.Id = cajaAjuste.id;
                cajaAjusteEdit.IdCaja = cajaAjuste.id_caja;
                cajaAjusteEdit.IdCajaApertura = cajaAjuste.id_caja_apertura;
                cajaAjusteEdit.IdUsuario = cajaAjuste.id_usuario;
                cajaAjusteEdit.Fecha = cajaAjuste.fecha;
                cajaAjusteEdit.Faltante = cajaAjuste.faltante.Value;
                cajaAjusteEdit.StrMontoAjuste = String.Format("{0:#,##0.##}", cajaAjuste.monto_ajuste);
                cajaAjusteEdit.Justificacion = cajaAjuste.justificacion;
                cajaAjusteEdit.IdTipoAjuste = cajaAjuste.faltante.Value == true ? 1 : 2;
                cajaAjusteEdit.NombreCaja = cajaAjuste.cajas.denominacion;
                cajaAjusteEdit.NombreApertura = cajaAjuste.cajas_aperturas.nombre_apertura;
                cajaAjusteEdit.Estado = cajaAjuste.estado.Value;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = cajaAjuste.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                CargarDatosCajaAjuste(cajaAjusteEdit);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(cajaAjusteEdit);
        }

        [HttpPost]
        public ActionResult Edit(CajaAjusteModel cajaAjusteModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;

            if (ModelState.IsValid) //SI EL MODELO ES VALIDO
            {
                try
                {
                    string strMonto = cajaAjusteModelo.StrMontoAjuste;
                    strMonto = strMonto.Replace(".", "");
                    decimal montoAjuste = Convert.ToDecimal(strMonto);

                    //OBTENEMOS EL REGISTRO DE CAJA DE AJUSTE PARA ACTUALIZAR LOS CAMBIOS
                    var cajaAjuste = db.cajas_ajustes.Where(ca => ca.id == cajaAjusteModelo.Id).FirstOrDefault();

                    cajaAjuste.faltante = cajaAjusteModelo.IdTipoAjuste == 1 ? true : false;
                    cajaAjuste.monto_ajuste = montoAjuste;
                    cajaAjuste.justificacion = cajaAjusteModelo.Justificacion;
                    bool nuevoEstado = cajaAjusteModelo.EstadoDescrip == "A" ? true : false;
                    cajaAjuste.estado = nuevoEstado;
                    db.Entry(cajaAjuste).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el ajuste de caja en la base de datos");
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
                CargarDatosCajaAjuste(cajaAjusteModelo);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", cajaAjusteModelo.EstadoDescrip);
                db.Dispose();
                return View(cajaAjusteModelo);
            }
        }

        #endregion

        #region Eliminar Caja Ajuste

        [HttpPost]
        public ActionResult EliminarCajaAjuste(string cajaAjusteId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("CajasAjustes", "EliminarCajaAjuste");
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
                                int idCajaAjuste = Convert.ToInt32(cajaAjusteId);
                                var cajaAjuste = context.cajas_ajustes.Where(ca => ca.id == idCajaAjuste).FirstOrDefault();
                                cajaAjuste.estado = null;
                                context.Entry(cajaAjuste).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "CajasAjustes") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Funciones

        /*
        * CARGAMOS LOS DATOS SI ES FUNCIONARIO O NO Y TAMBIEN LA SUCURSAL DONDE CORRESPONDE EL USUARIO SESION
        */
        private void CargarDatosUsuarioSesion()
        {
            IdUsuario = Convert.ToInt32(Session["IdUser"]);
            ViewBag.IdUsuarioLog = IdUsuario;
            var usuario = db.usuarios.Where(u => u.id == IdUsuario).FirstOrDefault();
            IdFuncionario = usuario.id_funcionario != null ? usuario.funcionarios.id : 0;
            ViewBag.EsFuncionario = usuario.id_funcionario != null ? true : false;
            ViewBag.IdSucursalUsuarioLog = usuario.id_funcionario != null ? usuario.funcionarios.id_sucursal : null;
            IdSucursalUsuario = usuario.id_funcionario != null ? usuario.funcionarios.id_sucursal.Value : 0;
        }

        /*
         * FUNCION QUE CARGA TODOS LOS DATOS NECESARIOS PARA REALIZAR UN AJUSTE
         */
        private void CargarDatosCajaAjuste(CajaAjusteModel cajaAjusteModelo)
        {
            CajaAjusteModel cajaAjuste = new CajaAjusteModel();
            var listaTipoAjuste = cajaAjuste.ListadoTiposAjustesCaja();
            ViewBag.IdTipoAjuste = new SelectList(listaTipoAjuste, "Id", "Descripcion", cajaAjusteModelo.IdTipoAjuste);
        }

        #endregion


    }
}