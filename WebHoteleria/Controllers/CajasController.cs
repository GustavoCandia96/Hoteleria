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
    public class CajasController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Cajas

        [HttpGet]
        [AutorizarUsuario("Cajas", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CajaModel> listaCajas = new List<CajaModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesIdSucursal = Convert.ToString(Session["sesionCajasIdSucursal"]);
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", sesIdSucursal);

                //OBTENEMOS TODOS LAS CAJAS NO ELIMINADAS DE LA BASE DE DATOS
                var cajas = from c in db.cajas
                            where c.estado != null
                            select new CajaModel
                            {
                                Id = c.id,
                                IdSucursal = c.id_sucursal,
                                IdMoneda = c.id_moneda,
                                IdUsuario = c.id_usuario,
                                Denominacion = c.denominacion,
                                Abierto = c.abierto.Value,
                                SaldoCaja = c.saldo_caja,
                                SaldoEfectivo = c.saldo_efectivo,
                                Sobrante = c.sobrante,
                                Faltante = c.faltante,
                                Estado = c.estado,
                                NombreSucursal = c.sucursales.nombre_sucursal,
                                NombreMoneda = c.monedas.nombre_moneda,
                                NombreFuncionario = c.usuarios.id_funcionario != null ? c.usuarios.funcionarios.nombre + " " + c.usuarios.funcionarios.apellido : "",
                                EstadoDescrip = c.estado == true ? "Activo" : "Inactivo"
                            };
                listaCajas = cajas.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(sesIdSucursal);
                    listaCajas = listaCajas.Where(c => c.IdSucursal == idSucursal).ToList();
                }

                listaCajas = listaCajas.OrderBy(c => c.Denominacion).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de cajas";
            }
            return View(listaCajas.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CajaModel> listaCajas = new List<CajaModel>();
            try
            {
                //OBTENEMOS TODOS LAS CAJAS NO ELIMINADAS DE LA BASE DE DATOS
                var cajas = from c in db.cajas
                            where c.estado != null
                            select new CajaModel
                            {
                                Id = c.id,
                                IdSucursal = c.id_sucursal,
                                IdMoneda = c.id_moneda,
                                IdUsuario = c.id_usuario,
                                Denominacion = c.denominacion,
                                Abierto = c.abierto.Value,
                                SaldoCaja = c.saldo_caja,
                                SaldoEfectivo = c.saldo_efectivo,
                                Sobrante = c.sobrante,
                                Faltante = c.faltante,
                                Estado = c.estado,
                                NombreSucursal = c.sucursales.nombre_sucursal,
                                NombreMoneda = c.monedas.nombre_moneda,
                                NombreFuncionario = c.usuarios.id_funcionario != null ? c.usuarios.funcionarios.nombre + " " + c.usuarios.funcionarios.apellido : "",
                                EstadoDescrip = c.estado == true ? "Activo" : "Inactivo"
                            };
                listaCajas = cajas.ToList();

                //FILTRAMOS POR SUCURSAL Y TIPO CAJA LA BUSQUEDA
                var fcIdSucursal = fc["ddlSucursales"];
                if (fcIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(fcIdSucursal);
                    listaCajas = listaCajas.Where(c => c.IdSucursal == idSucursal).ToList();
                }

                listaCajas = listaCajas.OrderBy(c => c.Denominacion).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", fcIdSucursal);
                Session["sesionCajasIdSucursal"] = fcIdSucursal;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar cajas";
            }
            return View(listaCajas.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Caja

        [HttpGet]
        [AutorizarUsuario("Cajas", "Create")]
        public ActionResult Create()
        {
            CajaModel cajaModelo = new CajaModel();
            CargarDatosCaja(cajaModelo);
            return View(cajaModelo);
        }

        [HttpPost]
        public ActionResult Create(CajaModel cajaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA CAJA CON LA MISMA DENOMINACION
                    int cantidad = db.cajas.Where(c => c.denominacion.Trim().ToUpper() == cajaModelo.Denominacion.Trim().ToUpper() && c.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //VERIFICAMOS SI EL USUARIO SELECCIONADO YA TIENE UNA CAJA ASIGNADA
                        int cantidadCajaUsuario = db.cajas.Where(c => c.id_usuario == cajaModelo.IdUsuario && c.estado != null).Count();
                        if (cantidadCajaUsuario == 0)
                        {
                            //AGREGAMOS EL REGISTRO DE CAJA EN LA BASE DE DATOS
                            cajas caja = new cajas
                            {
                                id_sucursal = cajaModelo.IdSucursal,
                                id_moneda = cajaModelo.IdMoneda,
                                id_usuario = cajaModelo.IdUsuario,
                                denominacion = cajaModelo.Denominacion.Trim(),
                                abierto = false,
                                saldo_caja = 0,
                                saldo_efectivo = 0,
                                sobrante = 0,
                                faltante = 0,
                                estado = true
                            };
                            db.cajas.Add(caja);
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("Duplicado", "El usuario seleccionado ya tiene asignado una caja");
                            retornoVista = true;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una caja registrada con el mismo nombre");
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
                CargarDatosCaja(cajaModelo);
                return View(cajaModelo);
            }
        }

        #endregion

        #region Editar Caja

        [HttpGet]
        [AutorizarUsuario("Cajas", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CajaModel cajaEdit = new CajaModel();
            try
            {
                //OBTENEMOS EL REGISTRO DE CAJA CARGANDO LOS DATOS A LAS PROPIEDADES AL MODELO
                var caja = db.cajas.Where(c => c.id == id).FirstOrDefault();
                cajaEdit.Id = caja.id;
                cajaEdit.IdSucursal = caja.id_sucursal;
                cajaEdit.IdMoneda = caja.id_moneda;
                cajaEdit.IdUsuario = caja.id_usuario;
                cajaEdit.Denominacion = caja.denominacion;
                cajaEdit.Abierto = caja.abierto.Value;
                cajaEdit.NombreSucursal = caja.sucursales.nombre_sucursal;
                cajaEdit.NombreMoneda = caja.monedas.nombre_moneda;
                cajaEdit.Estado = caja.estado;

                CargarDatosCaja(cajaEdit);

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = caja.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(cajaEdit);
        }

        [HttpPost]
        public ActionResult Edit(CajaModel cajaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA CAJA CON LA MISMA DENOMINACION
                    int cantidad = db.cajas.Where(c => c.denominacion.Trim().ToUpper() == cajaModelo.Denominacion.Trim().ToUpper() && c.estado != null && c.id != cajaModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //VERIFICAMOS SI EL USUARIO SELECCIONADO YA TIENE UNA CAJA ASIGNADA
                        int cantidadCajaUsuario = db.cajas.Where(c => c.id_usuario == cajaModelo.IdUsuario && c.estado != null && c.id != cajaModelo.Id).Count();
                        if (cantidadCajaUsuario == 0)
                        {
                            //OBTENEMOS EL REGISTRO DE CAJA PARA PODER ACTUALIZAR
                            var caja = db.cajas.Where(c => c.id == cajaModelo.Id).FirstOrDefault();
                            caja.id_usuario = cajaModelo.IdUsuario;
                            caja.denominacion = cajaModelo.Denominacion.Trim();
                            bool nuevoEstado = cajaModelo.EstadoDescrip == "A" ? true : false;
                            caja.estado = nuevoEstado;
                            db.Entry(caja).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("Duplicado", "El usuario seleccionado ya tiene asignado una caja");
                            retornoVista = true;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una caja registrada con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar la caja en la base de datos");
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
                CargarDatosCaja(cajaModelo);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", cajaModelo.EstadoDescrip);
                return View(cajaModelo);
            }
        }

        #endregion

        #region Eliminar Caja

        [HttpPost]
        public ActionResult EliminarCaja(string cajaId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Cajas", "EliminarCaja");
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
                                int idCaja = Convert.ToInt32(cajaId);
                                var caja = context.cajas.Where(c => c.id == idCaja).FirstOrDefault();
                                caja.estado = null;
                                context.Entry(caja).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Cajas") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Funciones

        private void CargarDatosCaja(CajaModel cajaModelo)
        {
            //OBTENEMOS EL LISTADO DE SUCURSALES
            ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", cajaModelo.IdSucursal);
            int idSucursal = cajaModelo.IdSucursal == null ? 0 : cajaModelo.IdSucursal.Value;
            //OBTENEMOS EL LISTADO DE MONEDAS
            ViewBag.IdMoneda = new SelectList(db.monedas.Where(m => m.estado == true).OrderBy(m => m.nombre_moneda).ToList(), "id", "nombre_moneda", cajaModelo.IdMoneda);
            //OBTENEMOS EL LISTADO DE USUARIOS FILTRADOS DE LA SUCURSAL
            UsuarioModel usuarioModelo = new UsuarioModel();
            List<ListaDinamica> listaUsuariosFuncionarios = usuarioModelo.ListadoUsuariosFuncionario(idSucursal);
            ViewBag.IdUsuario = new SelectList(listaUsuariosFuncionarios, "id", "nombre", cajaModelo.IdUsuario);
        }

        #endregion

        #region AJAX

        /*
         * METODO QUE DEVUELVE LOS DATOS DE UNA CAJA APERTURA SELECCIONADA
         */
        [HttpGet]
        public JsonResult ObtenerDatosCajaApertura(string cajaId)
        {
            string respuesta = string.Empty;
            string nombreApertura = string.Empty;
            string saldoAnterior = string.Empty;

            try
            {
                //OBTENEMOS LOS DATOS DE LA CAJA SELECCIONADA
                int idCaja = Convert.ToInt32(cajaId);
                var caja = db.cajas.Where(c => c.id == idCaja).FirstOrDefault();

                //VERIFICAMOS CUANTAS CAJAS APERTURAS YA EXISTEN PARA LA NOMENCLATURA
                int cantidad = db.cajas_aperturas.Where(ca => ca.id_caja == idCaja && ca.estado != null).Count();
                cantidad++;
                nombreApertura = "C.A. - ";
                nombreApertura += String.Format("{0:#,##0.##}", cantidad);
                saldoAnterior = String.Format("{0:#,##0.##}", caja.saldo_caja);
            }
            catch (Exception)
            {
                respuesta = "Error";
            }
            return Json(new
            {
                succes = true,
                nombreApertura = nombreApertura,
                saldoAnterior = saldoAnterior,
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}