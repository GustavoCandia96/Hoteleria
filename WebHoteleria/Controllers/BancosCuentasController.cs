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
    public class BancosCuentasController : Controller
    {
        /*
         * BANCOS CUENTAS
        * 
        * El modulo de bancos cuentas registra todas las cuentas de la empresa en un banco.
        * Actualmente no está relacionada con ningún modulo pero se relacionara con las cobranzas.
        * 

        */


        #region Listado de Bancos Cuentas


        [HttpGet]
        [AutorizarUsuario("BancosCuentas", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<BancoCuentaModel> listaBancosCuentas = new List<BancoCuentaModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesIdBanco = Convert.ToString(Session["sesionBancosCuentasIdBanco"]);
                ViewBag.ddlBancos = new SelectList(db.bancos.Where(b => b.estado == true).OrderBy(b => b.nombre_banco).ToList(), "id", "nombre_banco", sesIdBanco);
                string sesIdTipoCuenta = Convert.ToString(Session["sesionBancosCuentasIdTipoCuenta"]);
                ViewBag.ddlTiposCuentas = new SelectList(db.bancos_tipos_cuentas.Where(btc => btc.estado == true).OrderBy(btc => btc.descripcion).ToList(), "id", "descripcion", sesIdTipoCuenta);
                string sesIdMoneda = Convert.ToString(Session["sesionBancosCuentasIdMoneda"]);
                ViewBag.ddlMonedas = new SelectList(db.monedas.Where(m => m.estado == true).OrderBy(m => m.nombre_moneda).ToList(), "id", "nombre_moneda", sesIdMoneda);

                //OBTENEMOS TODOS LAS CUENTAS DE BANCOS NO ELIMINADOS DE LA BASE DE DATOS
                var bancosCuentas = from bc in db.bancos_cuentas
                                    join b in db.bancos on bc.id_banco equals b.id
                                    where bc.estado != null
                                    select new BancoCuentaModel
                                    {
                                        Id = bc.id,
                                        IdBanco = bc.id_banco,
                                        IdTipoCuenta = bc.id_tipo_cuenta,
                                        NroCuenta = bc.nro_cuenta,
                                        IdMoneda = bc.id_moneda,
                                        Estado = bc.estado,
                                        NombreBanco = b.nombre_banco,
                                        NombreTipoCuenta = bc.bancos_tipos_cuentas.descripcion,
                                        NombreMoneda = bc.monedas.nombre_moneda,
                                        EstadoDescrip = bc.estado == true ? "Activo" : "Inactivo"
                                    };

                listaBancosCuentas = bancosCuentas.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesIdBanco != "")
                {
                    int idBanco = Convert.ToInt32(sesIdBanco);
                    listaBancosCuentas = listaBancosCuentas.Where(bc => bc.IdBanco == idBanco).ToList();
                }
                if (sesIdTipoCuenta != "")
                {
                    int idTipoCuenta = Convert.ToInt32(sesIdTipoCuenta);
                    listaBancosCuentas = listaBancosCuentas.Where(bc => bc.IdTipoCuenta == idTipoCuenta).ToList();
                }
                if (sesIdMoneda != "")
                {
                    int idMoneda = Convert.ToInt32(sesIdMoneda);
                    listaBancosCuentas = listaBancosCuentas.Where(bc => bc.IdMoneda == idMoneda).ToList();
                }

                listaBancosCuentas = listaBancosCuentas.OrderByDescending(bc => bc.NroCuenta).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de las cuentas de bancos";
            }
            return View(listaBancosCuentas.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<BancoCuentaModel> listaBancosCuentas = new List<BancoCuentaModel>();
            try
            {
                //OBTENEMOS TODOS LAS CUENTAS DE BANCOS NO ELIMINADOS DE LA BASE DE DATOS
                var bancosCuentas = from bc in db.bancos_cuentas
                                    join b in db.bancos on bc.id_banco equals b.id
                                    where bc.estado != null
                                    select new BancoCuentaModel
                                    {
                                        Id = bc.id,
                                        IdBanco = bc.id_banco,
                                        IdTipoCuenta = bc.id_tipo_cuenta,
                                        NroCuenta = bc.nro_cuenta,
                                        IdMoneda = bc.id_moneda,
                                        Estado = bc.estado,
                                        NombreBanco = b.nombre_banco,
                                        NombreTipoCuenta = bc.bancos_tipos_cuentas.descripcion,
                                        NombreMoneda = bc.monedas.nombre_moneda,
                                        EstadoDescrip = bc.estado == true ? "Activo" : "Inactivo"
                                    };

                listaBancosCuentas = bancosCuentas.ToList();

                //FILTRAMOS POR BANCO TIPO DE CUENTA Y MONEDA LA BUSQUEDA
                var fcBancoId = fc["ddlBancos"];
                if (fcBancoId != "")
                {
                    int idBanco = Convert.ToInt32(fcBancoId);
                    listaBancosCuentas = listaBancosCuentas.Where(bc => bc.IdBanco == idBanco).ToList();
                }
                var fcTipoCuentaId = fc["ddlTiposCuentas"];
                if (fcTipoCuentaId != "")
                {
                    int idTipoCuenta = Convert.ToInt32(fcTipoCuentaId);
                    listaBancosCuentas = listaBancosCuentas.Where(bc => bc.IdTipoCuenta == idTipoCuenta).ToList();
                }
                var fcMonedaId = fc["ddlMonedas"];
                if (fcMonedaId != "")
                {
                    int idMoneda = Convert.ToInt32(fcMonedaId);
                    listaBancosCuentas = listaBancosCuentas.Where(bc => bc.IdMoneda == idMoneda).ToList();
                }

                listaBancosCuentas = listaBancosCuentas.OrderByDescending(bc => bc.Id).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.ddlBancos = new SelectList(db.bancos.Where(b => b.estado == true).OrderBy(b => b.nombre_banco).ToList(), "id", "nombre_banco", fcBancoId);
                Session["sesionBancosCuentasIdBanco"] = fcBancoId;
                ViewBag.ddlTiposCuentas = new SelectList(db.bancos_tipos_cuentas.Where(btc => btc.estado == true).OrderBy(btc => btc.descripcion).ToList(), "id", "descripcion", fcTipoCuentaId);
                Session["sesionBancosCuentasIdTipoCuenta"] = fcTipoCuentaId;
                ViewBag.ddlMonedas = new SelectList(db.monedas.Where(m => m.estado == true).OrderBy(m => m.nombre_moneda).ToList(), "id", "nombre_moneda", fcMonedaId);
                Session["sesionBancosCuentasIdMoneda"] = fcMonedaId;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar las cuentas de bancos";
            }
            return View(listaBancosCuentas.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Bancos Cuentas 

        [HttpGet]
        [AutorizarUsuario("BancosCuentas", "Create")]
        public ActionResult Create()
        {
            BancoCuentaModel bancoCuenta = new BancoCuentaModel();
            //OBTENEMOS LISTADO DE BANCOS TIPOS CUENTAS Y MONEDAS QUE VAN RELACIONADOS A UNA CUENTA DE BANCO
            ViewBag.IdBanco = new SelectList(db.bancos.Where(b => b.estado == true).OrderBy(b => b.nombre_banco).ToList(), "id", "nombre_banco");
            ViewBag.IdTipoCuenta = new SelectList(db.bancos_tipos_cuentas.Where(btc => btc.estado == true).OrderBy(btc => btc.descripcion).ToList(), "id", "descripcion");
            ViewBag.IdMoneda = new SelectList(db.monedas.Where(m => m.estado == true).OrderBy(m => m.nombre_moneda).ToList(), "id", "nombre_moneda");
            return View(bancoCuenta);
        }

        [HttpPost]
        public ActionResult Create(BancoCuentaModel cuentaBancoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN NUMERO DE CUENTA CON EL MISMO BANCO PARA PODER AGREGAR
                    int cantidad = db.bancos_cuentas.Where(bc => bc.nro_cuenta.Trim().ToUpper() == cuentaBancoModelo.NroCuenta.Trim().ToUpper() && bc.id_banco == cuentaBancoModelo.IdBanco && bc.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //AGREGAMOS EL REGISTRO DE CUENTA DE BANCO
                        bancos_cuentas bancoCuenta = new bancos_cuentas
                        {
                            id_banco = cuentaBancoModelo.IdBanco,
                            id_tipo_cuenta = cuentaBancoModelo.IdTipoCuenta,
                            nro_cuenta = cuentaBancoModelo.NroCuenta.Trim(),
                            id_moneda = cuentaBancoModelo.IdMoneda,
                            estado = true
                        };
                        db.bancos_cuentas.Add(bancoCuenta);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un número de cuenta registrado con el mismo banco");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la cuenta del banco en la base de datos");
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
                CargarDatosCuentaBanco(cuentaBancoModelo); //CARGAMOS LOS LISTADOS NECESARIOS
                db.Dispose();
                return View(cuentaBancoModelo);
            }
        }


        #endregion

        #region Editar Bancos Cuentas
        [HttpGet]
        [AutorizarUsuario("BancosCuentas", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BancoCuentaModel bancoCuentaEdit = new BancoCuentaModel();
            try
            {
                //OBTENEMOS EL REGISTRO Y CARGAMOS AL MODELO PARA PODER EDITAR
                var bancoCuenta = db.bancos_cuentas.Where(bc => bc.id == id).FirstOrDefault();
                bancoCuentaEdit.Id = bancoCuenta.id;
                bancoCuentaEdit.IdBanco = bancoCuenta.id_banco;
                bancoCuentaEdit.IdTipoCuenta = bancoCuenta.id_tipo_cuenta;
                bancoCuentaEdit.NroCuenta = bancoCuenta.nro_cuenta;
                bancoCuentaEdit.IdMoneda = bancoCuenta.id_moneda;
                bancoCuentaEdit.Estado = bancoCuenta.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = bancoCuenta.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
                CargarDatosCuentaBanco(bancoCuentaEdit); //CARGAMOS LOS LISTADOS NECESARIOS PARA EDITAR
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(bancoCuentaEdit);
        }

        [HttpPost]
        public ActionResult Edit(BancoCuentaModel cuentaBancoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN NUMERO DE CUENTA CON EL MISMO BANCO PARA PODER AGREGAR
                    int cantidad = db.bancos_cuentas.Where(bc => bc.nro_cuenta.Trim().ToUpper() == cuentaBancoModelo.NroCuenta.Trim().ToUpper() && bc.id_banco == cuentaBancoModelo.IdBanco && bc.estado != null && bc.id != cuentaBancoModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //OBTENEMOS EL REGISTRO PARA ACTUALIZAR LOS DATOS
                        var bancoCuenta = db.bancos_cuentas.Where(bc => bc.id == cuentaBancoModelo.Id).FirstOrDefault();
                        bancoCuenta.id_banco = cuentaBancoModelo.IdBanco;
                        bancoCuenta.id_tipo_cuenta = cuentaBancoModelo.IdTipoCuenta;
                        bancoCuenta.nro_cuenta = cuentaBancoModelo.NroCuenta.Trim();
                        bancoCuenta.id_moneda = cuentaBancoModelo.IdMoneda;
                        bool nuevoEstado = cuentaBancoModelo.EstadoDescrip == "A" ? true : false;
                        bancoCuenta.estado = nuevoEstado;
                        db.Entry(bancoCuenta).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un número de cuenta registrado con el mismo banco");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar la cuenta del banco en la base de datos");
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
                CargarDatosCuentaBanco(cuentaBancoModelo); //CARGAMOS LOS LISTADOS NECESARIOS
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", cuentaBancoModelo.EstadoDescrip);
                db.Dispose();
                return View(cuentaBancoModelo);
            }
        }

        #endregion  

        #region Eliminar Cuenta Banco

        [HttpPost]
        public ActionResult EliminarBancoCuenta(string bancoCuentaId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("BancosCuentas", "EliminarBancoCuenta");
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
                                int idBancoCuenta = Convert.ToInt32(bancoCuentaId);
                                var bancoCuenta = context.bancos_cuentas.Where(bc => bc.id == idBancoCuenta).FirstOrDefault();
                                bancoCuenta.estado = null;
                                context.Entry(bancoCuenta).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "BancosCuentas") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region AJAX

        [HttpPost]
        public ActionResult ObtenerListadoCuentas(string bancoReceptorId)
        {
            List<bancos_cuentas> listaBancosCuentas = new List<bancos_cuentas>();
            try
            {
                int idBancoReceptor = Convert.ToInt32(bancoReceptorId);
                listaBancosCuentas = db.bancos_cuentas.Where(bc => bc.id_banco == idBancoReceptor && bc.estado == true).ToList();
            }
            catch (Exception)
            {
            }
            var bancosCuentas = listaBancosCuentas.OrderBy(bc => bc.nro_cuenta).Select(bc => "<option value='" + bc.id + "'>" + bc.nro_cuenta + "</option>");
            return Content(String.Join("", bancosCuentas));
        }

        #endregion

        #region Funciones

        /*
         * METODO QUE RECIBE UN MODELO DE CUENTA BANCO Y CARGA LA LISTA DE BANCOS TIPOS CUENTAS Y MONEDAS
         */
        private void CargarDatosCuentaBanco(BancoCuentaModel cuentaBancoModelo)
        {
            ViewBag.IdBanco = new SelectList(db.bancos.Where(b => b.estado == true).OrderBy(b => b.nombre_banco).ToList(), "id", "nombre_banco", cuentaBancoModelo.IdBanco);
            ViewBag.IdTipoCuenta = new SelectList(db.bancos_tipos_cuentas.Where(btc => btc.estado == true).OrderBy(btc => btc.descripcion).ToList(), "id", "descripcion", cuentaBancoModelo.IdTipoCuenta);
            ViewBag.IdMoneda = new SelectList(db.monedas.Where(m => m.estado == true).OrderBy(m => m.nombre_moneda).ToList(), "id", "nombre_moneda", cuentaBancoModelo.IdMoneda);
        }

        #endregion

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities
();

        #endregion

    }
}