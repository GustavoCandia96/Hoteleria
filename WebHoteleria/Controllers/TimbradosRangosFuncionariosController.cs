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
    public class TimbradosRangosFuncionariosController : Controller
    {

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();


        #region Listado de Timbrados Rangos Funcionarios

        [HttpGet]
        [AutorizarUsuario("TimbradosRangosFuncionarios", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TimbradoRangoFuncionarioModel> listaTimbradosRangosFuncionarios = new List<TimbradoRangoFuncionarioModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesIdSucursal = Convert.ToString(Session["sesionTimbradosRangosFuncionariosIdSucursal"]);
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", sesIdSucursal);

                //OBTENEMOS TODOS LOS TIMBRADOS RANGOS FUNCIONARIOS ACTIVOS DE LA BASE DE DATOS
                var timbradoRangoFuncionario = from trf in db.timbrados_rangos_funcionarios
                                               where trf.estado != null 
                                               select new TimbradoRangoFuncionarioModel
                                               {
                                                   Id = trf.id,
                                                   IdTimbradoRango = trf.id_timbrado_rango,
                                                   IdFuncionario = trf.id_funcionario,
                                                   Fecha = trf.fecha,
                                                   Observaciones = trf.observacion,
                                                   NombreSucursal = trf.funcionarios.sucursales.nombre_sucursal,
                                                   NombreFuncionario = trf.funcionarios.nombre + " " + trf.funcionarios.apellido,
                                                   NroTimbrado = trf.timbrados_rangos.timbrados.nro_timbrado,
                                                   Documento = trf.timbrados_rangos.timbrados.timbrados_tipos_documentos.nombre_documento,
                                                   TipoDocumento = trf.timbrados_rangos.timbrados_rangos_tipos.tipo,
                                                   Desde = trf.timbrados_rangos.desde,
                                                   Hasta = trf.timbrados_rangos.hasta,
                                                   IdSucursal = trf.funcionarios.id_sucursal,
                                                   EstadoDescrip = trf.estado == true ? "Activo" : "Inactivo"
                                               };
                listaTimbradosRangosFuncionarios = timbradoRangoFuncionario.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(sesIdSucursal);
                    listaTimbradosRangosFuncionarios = listaTimbradosRangosFuncionarios.Where(trf => trf.IdSucursal == idSucursal).ToList();
                }
                listaTimbradosRangosFuncionarios = listaTimbradosRangosFuncionarios.OrderByDescending(trf => trf.Fecha).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de timbrados rangos funcionarios";
            }
            return View(listaTimbradosRangosFuncionarios.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<TimbradoRangoFuncionarioModel> listaTimbradosRangosFuncionarios = new List<TimbradoRangoFuncionarioModel>();
            try
            {
                //OBTENEMOS TODOS LOS TIMBRADOS RANGOS FUNCIONARIOS ACTIVOS DE LA BASE DE DATOS
                var timbradoRangoFuncionario = from trf in db.timbrados_rangos_funcionarios
                                               where trf.estado != null
                                               select new TimbradoRangoFuncionarioModel
                                               {
                                                   Id = trf.id,
                                                   IdTimbradoRango = trf.id_timbrado_rango,
                                                   IdFuncionario = trf.id_funcionario,
                                                   Fecha = trf.fecha,
                                                   Observaciones = trf.observacion,
                                                   NombreSucursal = trf.funcionarios.sucursales.nombre_sucursal,
                                                   NombreFuncionario = trf.funcionarios.nombre + " " + trf.funcionarios.apellido,
                                                   NroTimbrado = trf.timbrados_rangos.timbrados.nro_timbrado,
                                                   Documento = trf.timbrados_rangos.timbrados.timbrados_tipos_documentos.nombre_documento,
                                                   TipoDocumento = trf.timbrados_rangos.timbrados_rangos_tipos.tipo,
                                                   Desde = trf.timbrados_rangos.desde,
                                                   Hasta = trf.timbrados_rangos.hasta,
                                                   IdSucursal = trf.funcionarios.id_sucursal,
                                                   EstadoDescrip = trf.estado == true ? "Activo" : "Inactivo"
                                               };
                listaTimbradosRangosFuncionarios = timbradoRangoFuncionario.ToList();

                //FILTRAMOS POR SUCURSAL EN BUSQUEDA
                var fcIdSucursal = fc["ddlSucursales"];
                if (fcIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(fcIdSucursal);
                    listaTimbradosRangosFuncionarios = listaTimbradosRangosFuncionarios.Where(trf => trf.IdSucursal == idSucursal).ToList();
                }

                listaTimbradosRangosFuncionarios = listaTimbradosRangosFuncionarios.OrderByDescending(trf => trf.Fecha).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", fcIdSucursal);
                Session["sesionTimbradosRangosFuncionariosIdSucursal"] = fcIdSucursal;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar timbrados rangos funcionarios";
            }
            return View(listaTimbradosRangosFuncionarios.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Timbrado Rango Funcionario

        [HttpGet]
        [AutorizarUsuario("TimbradosRangosFuncionarios", "Create")]
        public ActionResult Create()
        {
            TimbradoRangoFuncionarioModel timbradoRangoFuncionario = new TimbradoRangoFuncionarioModel();

            ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal");
            ViewBag.IdTimbradoTipoDocumento = new SelectList(db.timbrados_tipos_documentos.Where(ttd => ttd.estado == true).OrderBy(ttd => ttd.nombre_documento).ToList(), "id", "nombre_documento");
            ViewBag.IdTipoRango = new SelectList(db.timbrados_rangos_tipos.Where(trt => trt.estado == true).OrderBy(trt => trt.tipo).ToList(), "id", "tipo");

            //OBTENEMOS EL LISTADO DE FUNCIONARIOS VACIO
            FuncionarioModel funcionarioModelo = new FuncionarioModel();
            List<ListaDinamica> listaFuncionarios = funcionarioModelo.ListadoFuncionarios(0);
            ViewBag.IdFuncionario = new SelectList(listaFuncionarios, "Id", "Nombre");

            //OBTENEMOS EL LISTADO DE RANGOS DISPONIBLES VACIO
            TimbradoRangoModel timbradoRangoModelo = new TimbradoRangoModel();
            List<ListaDinamica> listaRangosDisponibles = timbradoRangoModelo.ListadoRangosDisponibles(0, 0, 0);
            ViewBag.IdTimbradoRango = new SelectList(listaRangosDisponibles, "Id", "Nombre");

            return View(timbradoRangoFuncionario);
        }

        [HttpPost]
        public ActionResult Create(TimbradoRangoFuncionarioModel timbradoRangoFuncionarioModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN RANGO ASOCIADO A UN FUNCIONARIO PARA PODER AGREGAR
                    int cantidad = db.timbrados_rangos_funcionarios.Where(trf => trf.id_timbrado_rango == timbradoRangoFuncionarioModelo.IdTimbradoRango && trf.estado != null).Count();
                    if (cantidad == 0)
                    {
                        DateTime fecha = Convert.ToDateTime(timbradoRangoFuncionarioModelo.StrFecha);

                        timbrados_rangos_funcionarios timbRangFunc = new timbrados_rangos_funcionarios
                        {
                            id_timbrado_rango = timbradoRangoFuncionarioModelo.IdTimbradoRango,
                            id_funcionario = timbradoRangoFuncionarioModelo.IdFuncionario,
                            fecha = fecha,
                            observacion = timbradoRangoFuncionarioModelo.Observaciones,
                            estado = true
                        };
                        db.timbrados_rangos_funcionarios.Add(timbRangFunc);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un timbrado de rango asociado a un funcionario");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar timbrado rango funcionario en la base de datos");
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
                CargarDatosTimbradoRangoFuncionario(timbradoRangoFuncionarioModelo);
                db.Dispose();
                return View(timbradoRangoFuncionarioModelo);
            }
        }

        #endregion

        #region Editar Timbrado Rango Funcionario

        [HttpGet]
        [AutorizarUsuario("TimbradosRangosFuncionarios", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TimbradoRangoFuncionarioModel timbradoRangoFuncionarioEdit = new TimbradoRangoFuncionarioModel();
            try
            {
                var timbradoRangoFuncionario = db.timbrados_rangos_funcionarios.Where(trf => trf.id == id).FirstOrDefault();
                timbradoRangoFuncionarioEdit.Id = timbradoRangoFuncionario.id;
                timbradoRangoFuncionarioEdit.IdTimbradoRango = timbradoRangoFuncionario.id_timbrado_rango;
                timbradoRangoFuncionarioEdit.IdFuncionario = timbradoRangoFuncionario.id_funcionario;
                timbradoRangoFuncionarioEdit.StrFecha = timbradoRangoFuncionario.fecha.Value.ToShortDateString();
                timbradoRangoFuncionarioEdit.Observaciones = timbradoRangoFuncionario.observacion;
                timbradoRangoFuncionarioEdit.IdTipoRango = timbradoRangoFuncionario.timbrados_rangos.timbrados_rangos_tipos.id;
                timbradoRangoFuncionarioEdit.IdTimbradoTipoDocumento = timbradoRangoFuncionario.timbrados_rangos.timbrados.id_timbrado_tipo_documento;
                timbradoRangoFuncionarioEdit.IdSucursal = timbradoRangoFuncionario.funcionarios.id_sucursal;
                timbradoRangoFuncionarioEdit.Estado = timbradoRangoFuncionario.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = timbradoRangoFuncionario.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                CargarDatosTimbradoRangoFuncionario(timbradoRangoFuncionarioEdit);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(timbradoRangoFuncionarioEdit);
        }

        [HttpPost]
        public ActionResult Edit(TimbradoRangoFuncionarioModel timbradoRangoFuncionarioModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN RANGO ASOCIADO A UN FUNCIONARIO PARA PODER AGREGAR
                    int cantidad = db.timbrados_rangos_funcionarios.Where(trf => trf.id_timbrado_rango == timbradoRangoFuncionarioModelo.IdTimbradoRango && trf.estado != null && trf.id != timbradoRangoFuncionarioModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var timbradoRangoFuncionario = db.timbrados_rangos_funcionarios.Where(trf => trf.id == timbradoRangoFuncionarioModelo.Id).FirstOrDefault();
                        timbradoRangoFuncionario.id_timbrado_rango = timbradoRangoFuncionarioModelo.IdTimbradoRango;
                        timbradoRangoFuncionario.id_funcionario = timbradoRangoFuncionarioModelo.IdFuncionario;
                        DateTime fecha = Convert.ToDateTime(timbradoRangoFuncionarioModelo.StrFecha);
                        timbradoRangoFuncionario.fecha = fecha;
                        timbradoRangoFuncionario.observacion = timbradoRangoFuncionarioModelo.Observaciones;
                        bool nuevoEstado = timbradoRangoFuncionarioModelo.EstadoDescrip == "A" ? true : false;
                        timbradoRangoFuncionario.estado = nuevoEstado;
                        db.Entry(timbradoRangoFuncionario).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un timbrado de rango asociado a un funcionario");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el timbrado rango funcionario en la base de datos");
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
                CargarDatosTimbradoRangoFuncionario(timbradoRangoFuncionarioModelo);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", timbradoRangoFuncionarioModelo.EstadoDescrip);
                db.Dispose();
                return View(timbradoRangoFuncionarioModelo);
            }
        }

        #endregion

        #region Eliminar Timbrado Rango Funcionario

        [HttpPost]
        public ActionResult EliminarTimbradoRangoFuncionario(string timbradoRangoFuncionarioId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("TimbradosRangosFuncionarios", "EliminarTimbradoRangoFuncionario");
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
                                int idTimbradoRangoFuncionario = Convert.ToInt32(timbradoRangoFuncionarioId);
                                var timbradoRangoFuncionario = context.timbrados_rangos_funcionarios.Where(d => d.id == idTimbradoRangoFuncionario).FirstOrDefault();
                                timbradoRangoFuncionario.estado = null;
                                context.Entry(timbradoRangoFuncionario).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "TimbradosRangosFuncionarios") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Funciones

        private void CargarDatosTimbradoRangoFuncionario(TimbradoRangoFuncionarioModel timbradoRangoFuncionarioModelo)
        {
            ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", timbradoRangoFuncionarioModelo.IdSucursal);
            ViewBag.IdTimbradoTipoDocumento = new SelectList(db.timbrados_tipos_documentos.Where(ttd => ttd.estado == true).OrderBy(ttd => ttd.nombre_documento).ToList(), "id", "nombre_documento", timbradoRangoFuncionarioModelo.IdTimbradoTipoDocumento);
            ViewBag.IdTipoRango = new SelectList(db.timbrados_rangos_tipos.Where(trt => trt.estado == true).OrderBy(trt => trt.tipo).ToList(), "id", "tipo", timbradoRangoFuncionarioModelo.IdTipoRango);

            //OBTENEMOS EL LISTADO DE FUNCIONARIOS DE LA VISTA
            FuncionarioModel funcionarioModelo = new FuncionarioModel();
            List<ListaDinamica> listaFuncionarios = funcionarioModelo.ListadoFuncionarios(timbradoRangoFuncionarioModelo.IdSucursal);
            ViewBag.IdFuncionario = new SelectList(listaFuncionarios, "id", "nombre", timbradoRangoFuncionarioModelo.IdFuncionario);

            //OBTENEMOS EL LISTADO DE RANGOS DISPONIBLES DE LA VISTA
            TimbradoRangoModel timbradoRangoModelo = new TimbradoRangoModel();
            List<ListaDinamica> listaRangosDisponibles = timbradoRangoModelo.ListadoRangosDisponibles(timbradoRangoFuncionarioModelo.IdTipoRango, timbradoRangoFuncionarioModelo.IdSucursal, timbradoRangoFuncionarioModelo.IdTimbradoTipoDocumento);
            ViewBag.IdTimbradoRango = new SelectList(listaRangosDisponibles, "id", "nombre", timbradoRangoFuncionarioModelo.IdTimbradoRango);
        }

        #endregion


    }
}