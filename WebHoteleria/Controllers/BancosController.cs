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
    public class BancosController : Controller
    {

        /*
        * BANCOS
        * 
        * El modulo de bancos registra a todos los bancos que trabajan con la empresa. Está relacionada actualmente solo con bancos cuentas. 
        * Ejemplo (itau, continental).

        */

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Bancos

        [HttpGet]
        [AutorizarUsuario("Bancos", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<BancoModel> listaBancos = new List<BancoModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomBanco = Convert.ToString(Session["sesionBancosNombre"]);
                ViewBag.txtBanco = sesNomBanco;

                //OBTENEMOS TODOS LOS BANCOS NO ELIMINADOS DE LA BASE DE DATOS
                var bancos = from b in db.bancos
                             where b.estado != null
                             select new BancoModel
                             {
                                 Id = b.id,
                                 NombreBanco = b.nombre_banco,
                                 Estado = b.estado,
                                 EstadoDescrip = b.estado == true ? "Activo" : "Inactivo"
                             };
                listaBancos = bancos.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomBanco != "")
                {
                    listaBancos = listaBancos.Where(b => b.NombreBanco.ToUpper().Contains(sesNomBanco.Trim().ToUpper())).ToList();
                }

                listaBancos = listaBancos.OrderBy(b => b.NombreBanco).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de bancos";
            }
            return View(listaBancos.ToPagedList(pageIndex, pageSize));
        }
        public JsonResult BuscarBancos(string term)
        {
            using (hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities())
            {
                var resultado = db.bancos.Where(x => x.nombre_banco.Contains(term) && x.estado != null)
                        .Select(x => x.nombre_banco).Take(5).ToList();


                return Json(resultado, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<BancoModel> listaBancos = new List<BancoModel>();
            try
            {
                //OBTENEMOS TODOS LOS BANCOS NO ELIMINADOS DE LA BASE DE DATOS
                var bancos = from b in db.bancos
                             where b.estado != null
                             select new BancoModel
                             {
                                 Id = b.id,
                                 NombreBanco = b.nombre_banco,
                                 Estado = b.estado,
                                 EstadoDescrip = b.estado == true ? "Activo" : "Inactivo"
                             };
                listaBancos = bancos.ToList();

                //FILTRAMOS POR NOMBRE BANCO LA BUSQUEDA
                var fcNombreBanco = fc["txtBanco"];
                if (fcNombreBanco != "")
                {
                    string descripcion = Convert.ToString(fcNombreBanco);
                    listaBancos = listaBancos.Where(b => b.NombreBanco.Trim().ToUpper().Contains(descripcion.Trim().ToUpper())).ToList();
                }
                listaBancos = listaBancos.OrderBy(b => b.NombreBanco).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtBanco = fcNombreBanco;
                Session["sesionBancosNombre"] = fcNombreBanco;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar bancos";
            }
            return View(listaBancos.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Bancos

        [HttpGet]
        [AutorizarUsuario("Bancos", "Create")]
        public ActionResult Create()
        {
            BancoModel banco = new BancoModel();
            return View(banco);
        }

        [HttpPost]
        public ActionResult Create(BancoModel bancoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL BANCO EN LA BASE DE DATOS PARA PODER AGREGAR
                    int cantidad = db.bancos.Where(b => b.nombre_banco.Trim().ToUpper() == bancoModelo.NombreBanco.Trim().ToUpper() && b.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //AGREGAMOS UN REGISTRO DE BANCO
                        bancos banco = new bancos
                        {
                            nombre_banco = bancoModelo.NombreBanco.Trim(),
                            estado = true
                        };
                        db.bancos.Add(banco);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un banco registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el banco en la base de datos");
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
                return View(bancoModelo);
            }
        }

        #endregion

        #region Editar Bancos

        [HttpGet]
        [AutorizarUsuario("Bancos", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BancoModel bancoEdit = new BancoModel();
            try
            {
                //OBTENEMOS EL REGISTRO DE BANCO Y CARGAMOS LAS PROPIEDADES EN EL MODELO
                var banco = db.bancos.Where(b => b.id == id).FirstOrDefault();
                bancoEdit.Id = banco.id;
                bancoEdit.NombreBanco = banco.nombre_banco;
                bancoEdit.Estado = banco.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = banco.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(bancoEdit);
        }

        [HttpPost]
        public ActionResult Edit(BancoModel bancoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE EL BANCO EN LA BASE DE DATOS PARA PODER ACTUALIZAR
                    int cantidad = db.bancos.Where(b => b.nombre_banco.Trim().ToUpper() == bancoModelo.NombreBanco.Trim().ToUpper() && b.estado != null && b.id != bancoModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //ACTUALIZAMOS EL REGISTRO DE BANCO
                        var banco = db.bancos.Where(b => b.id == bancoModelo.Id).FirstOrDefault();
                        banco.nombre_banco = bancoModelo.NombreBanco.Trim();
                        bool nuevoEstado = bancoModelo.EstadoDescrip == "A" ? true : false;
                        banco.estado = nuevoEstado;
                        db.Entry(banco).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un banco registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el banco en la base de datos");
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
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", bancoModelo.EstadoDescrip);
                return View(bancoModelo);
            }
        }

        #endregion

        #region Eliminar Bancos

        [HttpPost]
        public ActionResult EliminarBanco(string bancoId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Bancos", "EliminarBanco");
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
                                int idBanco = Convert.ToInt32(bancoId);
                                var banco = context.bancos.Where(p => p.id == idBanco).FirstOrDefault();
                                banco.estado = null;
                                context.Entry(banco).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Bancos") }, JsonRequestBehavior.AllowGet);
        }

        #endregion



    }
}