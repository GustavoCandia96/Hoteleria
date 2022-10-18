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
    public class DepositosController : Controller
    {

        #region Propiedades


        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Depositos

        [HttpGet]
        [AutorizarUsuario("Depositos", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<DepositoModel> listaDepositos = new List<DepositoModel>();
            try
            {
                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomDeposito = Convert.ToString(Session["sesionDepositosNombre"]);
                ViewBag.txtDeposito = sesNomDeposito;
                string sesIdSucursal = Convert.ToString(Session["sesionDepositosIdSucursal"]);
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", sesIdSucursal);

                //OBTENEMOS TODOS LOS DEPOSITOS DE LA BASE DE DATOS Y ORDENAMOS POR DESCRIPCIÓN
                var depositos = from d in db.depositos
                                where d.estado != null
                                select new DepositoModel
                                {
                                    Id = d.id,
                                    NombreDeposito = d.nombre_deposito,
                                    NombreSucursal = d.sucursales.nombre_sucursal,
                                    Estado = d.estado,
                                    EstadoDescrip = d.estado == true ? "Activo" : "Inactivo"
                                };
                listaDepositos = depositos.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomDeposito != "")
                {
                    listaDepositos = listaDepositos.Where(d => d.NombreDeposito.ToUpper().Contains(sesNomDeposito.Trim().ToUpper())).ToList();
                }
                if (sesIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(sesIdSucursal);
                    listaDepositos = listaDepositos.Where(d => d.IdSucursal == idSucursal).ToList();
                }

                listaDepositos = listaDepositos.OrderBy(d => d.NombreDeposito).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de depositos";
            }
            return View(listaDepositos.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<DepositoModel> listaDepositos = new List<DepositoModel>();
            try
            {
                //OBTENEMOS TODOS LOS DEPOSITOS DE LA BASE DE DATOS Y ORDENAMOS POR DESCRIPCIÓN
                var depositos = from d in db.depositos
                                where d.estado != null
                                select new DepositoModel
                                {
                                    Id = d.id,
                                    NombreDeposito = d.nombre_deposito,
                                    NombreSucursal = d.sucursales.nombre_sucursal,
                                    Estado = d.estado,
                                    EstadoDescrip = d.estado == true ? "Activo" : "Inactivo"
                                };
                listaDepositos = depositos.ToList();

                //FILTRAMOS POR NOMBRE DEPOSITO Y SUCURSAL LA BUSQUEDA
                var fcNombreDeposito = fc["txtDeposito"];
                if (fcNombreDeposito != "")
                {
                    string descripcion = Convert.ToString(fcNombreDeposito);
                    listaDepositos = listaDepositos.Where(d => d.NombreDeposito.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }

                var fcIdSucursal = fc["ddlSucursales"];
                if (fcIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(fcIdSucursal);
                    listaDepositos = listaDepositos.Where(d => d.IdSucursal == idSucursal).ToList();
                }

                listaDepositos = listaDepositos.OrderBy(d => d.NombreDeposito).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtDeposito = fcNombreDeposito;
                Session["sesionDepositosNombre"] = fcNombreDeposito;
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", fcIdSucursal);
                Session["sesionDepositosIdSucursal"] = fcIdSucursal;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar depositos";
            }
            return View(listaDepositos.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Deposito

        [HttpGet]
        [AutorizarUsuario("Depositos", "Create")]
        public ActionResult Create()
        {
            DepositoModel deposito = new DepositoModel();
            ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal");
            return View(deposito);
        }

        [HttpPost]
        public ActionResult Create(DepositoModel depositoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN DEPOSITO CON LA MISMA SUCURSAL PARA PODER AGREGAR
                    int cantidad = db.depositos.Where(d => d.nombre_deposito.ToUpper() == depositoModelo.NombreDeposito.ToUpper() && d.id_sucursal == depositoModelo.IdSucursal && d.estado != null).Count();
                    if (cantidad == 0)
                    {
                        depositos deposito = new depositos
                        {
                            id_sucursal = depositoModelo.IdSucursal,
                            nombre_deposito = depositoModelo.NombreDeposito,
                            estado = true
                        };
                        db.depositos.Add(deposito);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un deposito registrado con el mismo nombre y sucursal");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el deposito en la base de datos");
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
                ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", depositoModelo.IdSucursal);
                db.Dispose();
                return View(depositoModelo);
            }
        }

        #endregion

        #region Editar Deposito

        [HttpGet]
        [AutorizarUsuario("Depositos", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DepositoModel depositoEdit = new DepositoModel();
            try
            {
                var deposito = db.depositos.Where(d => d.id == id).FirstOrDefault();
                depositoEdit.Id = deposito.id;
                depositoEdit.IdSucursal = deposito.id_sucursal;
                depositoEdit.NombreDeposito = deposito.nombre_deposito;
                depositoEdit.Estado = deposito.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = deposito.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", depositoEdit.IdSucursal);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(depositoEdit);
        }

        [HttpPost]
        public ActionResult Edit(DepositoModel depositoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN DEPOSITO CON LA MISMA SUCURSAL PARA PODER AGREGAR
                    int cantidad = db.depositos.Where(d => d.nombre_deposito.ToUpper() == depositoModelo.NombreDeposito.ToUpper() && d.id_sucursal == depositoModelo.IdSucursal && d.estado != null && d.id != depositoModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var deposito = db.depositos.Where(d => d.id == depositoModelo.Id).FirstOrDefault();
                        deposito.id_sucursal = depositoModelo.IdSucursal;
                        deposito.nombre_deposito = depositoModelo.NombreDeposito;
                        bool nuevoEstado = depositoModelo.EstadoDescrip == "A" ? true : false;
                        deposito.estado = nuevoEstado;
                        db.Entry(deposito).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un deposito registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el deposito en la base de datos");
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
                ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", depositoModelo.IdSucursal);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", depositoModelo.EstadoDescrip);
                db.Dispose();
                return View(depositoModelo);
            }
        }

        #endregion

        #region Eliminar Deposito

        [HttpPost]
        public ActionResult EliminarDeposito(string depositoId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Depositos", "EliminarDeposito");
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
                                int idDeposito = Convert.ToInt32(depositoId);
                                var deposito = context.depositos.Where(d => d.id == idDeposito).FirstOrDefault();
                                deposito.estado = null;
                                context.Entry(deposito).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Depositos") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region AJAX

        [HttpPost]
        public ActionResult ObtenerListadoDepositos(string sucursalId)
        {
            int idSucursal = sucursalId != "" ? Convert.ToInt32(sucursalId) : 0;
            DepositoModel depositoModelo = new DepositoModel();
            List<ListaDinamica> listaDepositos = depositoModelo.ListadoDepositos(idSucursal);
            var depositos = listaDepositos.OrderBy(d => d.Nombre).Select(d => "<option value='" + d.Id + "'>" + d.Nombre + "</option>");
            return Content(String.Join("", depositos));
        }

        #endregion

    }
}