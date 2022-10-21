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
    public class FuncionariosController : Controller
    {

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #region Listado de Funcionarios

        [HttpGet]
        [AutorizarUsuario("Funcionarios", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<FuncionarioModel> listaFuncionarios = new List<FuncionarioModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesIdSucursal = Convert.ToString(Session["sesionFuncionariosIdSucursal"]);
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", sesIdSucursal);
                string sesNroDoc = Convert.ToString(Session["sesionFuncionariosNroDocumento"]);
                ViewBag.txtNroDocumento = sesNroDoc;
                string sesNombre = Convert.ToString(Session["sesionFuncionariosNombre"]);
                ViewBag.txtNombre = sesNombre;
                string sesApellido = Convert.ToString(Session["sesionFuncionariosApellido"]);
                ViewBag.txtApellido = sesApellido;

                //OBTENEMOS TODOS LOS FUNCIONARIOS ACTIVOS DE LA BASE DE DATOS
                var funcionarios = from f in db.funcionarios
                                   join td in db.tipos_documentos on f.id_tipo_documento equals td.id
                                   join c in db.cargos on f.id_cargo equals c.id
                                   join s in db.sucursales on f.id_sucursal equals s.id
                                   join a in db.areas on f.id_area equals a.id
                                   where f.estado != null
                                   select new FuncionarioModel
                                   {
                                       Id = f.id,
                                       IdTipoDocumento = f.id_tipo_documento,
                                       NroDocumento = f.nro_documento,
                                       Nombre = f.nombre,
                                       Apellido = f.apellido,
                                       IdArea = f.id_area,
                                       IdCargo = f.id_cargo,
                                       Direccion = f.direccion,
                                       Estado = f.estado,
                                       NombreTipoDocumento = td.tipo,
                                       NombreArea = a.nombre_area,
                                       NombreCargo = c.nombre_cargo,
                                       NombreCompleto = f.nombre + " " + f.apellido,
                                       IdSucursal = f.id_sucursal,
                                       NombreSucursal = s.nombre_sucursal,
                                       EstadoDescrip = f.estado == true ? "Activo" : "Inactivo"
                                   };

                listaFuncionarios = funcionarios.OrderBy(f => f.NombreCompleto).ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(sesIdSucursal);
                    listaFuncionarios = listaFuncionarios.Where(f => f.IdSucursal == idSucursal).ToList();
                }
                if (sesNroDoc != "")
                {
                    listaFuncionarios = listaFuncionarios.Where(f => f.NroDocumento.ToUpper().Contains(sesNroDoc.Trim().ToUpper())).ToList();
                }
                if (sesNombre != "")
                {
                    listaFuncionarios = listaFuncionarios.Where(f => f.Nombre.ToUpper().Contains(sesNombre.Trim().ToUpper())).ToList();
                }
                if (sesApellido != "")
                {
                    listaFuncionarios = listaFuncionarios.Where(f => f.Apellido.ToUpper().Contains(sesApellido.Trim().ToUpper())).ToList();
                }

                listaFuncionarios = listaFuncionarios.OrderBy(f => f.NombreCompleto).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de funcionarios";
            }
            return View(listaFuncionarios.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<FuncionarioModel> listaFuncionarios = new List<FuncionarioModel>();
            try
            {
                //OBTENEMOS TODOS LOS FUNCIONARIOS ACTIVOS DE LA BASE DE DATOS
                var funcionarios = from f in db.funcionarios
                                   join td in db.tipos_documentos on f.id_tipo_documento equals td.id
                                   join c in db.cargos on f.id_cargo equals c.id
                                   join s in db.sucursales on f.id_sucursal equals s.id
                                   join a in db.areas on f.id_area equals a.id
                                   where f.estado != null
                                   select new FuncionarioModel
                                   {
                                       Id = f.id,
                                       IdTipoDocumento = f.id_tipo_documento,
                                       NroDocumento = f.nro_documento,
                                       Nombre = f.nombre,
                                       Apellido = f.apellido,
                                       IdArea = f.id_area,
                                       IdCargo = f.id_cargo,
                                       Direccion = f.direccion,
                                       Estado = f.estado,
                                       NombreTipoDocumento = td.tipo,
                                       NombreArea = a.nombre_area,
                                       NombreCargo = c.nombre_cargo,
                                       NombreCompleto = f.nombre + " " + f.apellido,
                                       IdSucursal = f.id_sucursal,
                                       NombreSucursal = s.nombre_sucursal,
                                       EstadoDescrip = f.estado == true ? "Activo" : "Inactivo"
                                   };

                listaFuncionarios = funcionarios.OrderBy(f => f.NombreCompleto).ToList();

                //FILTRAMOS POR SUCURSAL N° DOCUMENTO NOMBRE Y CARGO
                var fcIdSucursal = fc["ddlSucursales"];
                int idSucursal = fcIdSucursal != "" ? Convert.ToInt32(fcIdSucursal) : 0;
                if (fcIdSucursal != "")
                {
                    listaFuncionarios = listaFuncionarios.Where(f => f.IdSucursal == idSucursal).ToList();
                }

                var fcNroDocumento = fc["txtNroDocumento"];
                if (fcNroDocumento != "")
                {
                    string nroDocumento = Convert.ToString(fcNroDocumento);
                    listaFuncionarios = listaFuncionarios.Where(f => f.NroDocumento.ToUpper().Contains(nroDocumento.ToUpper())).ToList();
                }

                var fcNombre = fc["txtNombre"];
                if (fcNombre != "")
                {
                    string nombre = Convert.ToString(fcNombre);
                    listaFuncionarios = listaFuncionarios.Where(f => f.Nombre.ToUpper().Contains(nombre.ToUpper())).ToList();
                }

                var fcApellido = fc["txtApellido"];
                if (fcApellido != "")
                {
                    string apellido = Convert.ToString(fcApellido);
                    listaFuncionarios = listaFuncionarios.Where(f => f.Apellido.ToUpper().Contains(apellido.ToUpper())).ToList();
                }

                listaFuncionarios.OrderBy(f => f.NombreCompleto).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", fcIdSucursal);
                Session["sesionFuncionariosIdSucursal"] = fcIdSucursal;
                ViewBag.txtNroDocumento = fcNroDocumento;
                Session["sesionFuncionariosNroDocumento"] = fcNroDocumento;
                ViewBag.txtNombre = fcNombre;
                Session["sesionFuncionariosNombre"] = fcNombre;
                ViewBag.txtApellido = fcApellido;
                Session["sesionFuncionariosApellido"] = fcApellido;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar funcionarios";
            }
            return View(listaFuncionarios.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Funcionario

        [HttpGet]
        [AutorizarUsuario("Funcionarios", "Create")]
        public ActionResult Create()
        {
            FuncionarioModel funcionario = new FuncionarioModel();
            ViewBag.IdTipoDocumento = new SelectList(db.tipos_documentos.Where(td => td.estado == true).OrderBy(td => td.tipo).ToList(), "id", "tipo");
            ViewBag.IdArea = new SelectList(db.areas.Where(a => a.estado == true).OrderBy(a => a.nombre_area).ToList(), "id", "nombre_area");
            ViewBag.IdCargo = new SelectList(db.cargos.Where(c => c.id == 0).OrderBy(c => c.nombre_cargo).ToList(), "id", "nombre_cargo");
            ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal");
            return View(funcionario);
        }

        [HttpPost]
        public ActionResult Create(FuncionarioModel funcionarioModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN FUNCIONARIO CON EL MISMO NUMERO DOCUMENTO
                    int cantidad = db.funcionarios.Where(f => f.nro_documento.ToUpper() == funcionarioModelo.NroDocumento.Trim().ToUpper() && f.estado != null).Count();
                    if (cantidad == 0)
                    {
                        funcionarios funcionario = new funcionarios
                        {
                            id_tipo_documento = funcionarioModelo.IdTipoDocumento,
                            nro_documento = funcionarioModelo.NroDocumento,
                            nombre = funcionarioModelo.Nombre,
                            apellido = funcionarioModelo.Apellido,
                            id_area = funcionarioModelo.IdArea,
                            id_cargo = funcionarioModelo.IdCargo,
                            direccion = funcionarioModelo.Direccion,
                            id_sucursal = funcionarioModelo.IdSucursal,
                            estado = true
                        };
                        db.funcionarios.Add(funcionario);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un funcionario registrado con el mismo número de documento");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar al funcionario en la base de datos");
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
                CargarDatosFuncionarios(funcionarioModelo);
                db.Dispose();
                return View(funcionarioModelo);
            }
        }

        #endregion
        
        #region Editar Funcionario

        [HttpGet]
        [AutorizarUsuario("Funcionarios", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FuncionarioModel funcionarioEdit = new FuncionarioModel();
            try
            {
                var funcionario = db.funcionarios.Where(f => f.id == id).FirstOrDefault();
                funcionarioEdit.Id = funcionario.id;
                funcionarioEdit.IdTipoDocumento = funcionario.id_tipo_documento;
                funcionarioEdit.NroDocumento = funcionario.nro_documento;
                funcionarioEdit.Nombre = funcionario.nombre;
                funcionarioEdit.Apellido = funcionario.apellido;
                funcionarioEdit.IdArea = funcionario.id_area;
                funcionarioEdit.IdCargo = funcionario.id_cargo;
                funcionarioEdit.Direccion = funcionario.direccion;
                funcionarioEdit.IdSucursal = funcionario.id_sucursal;
                funcionarioEdit.Estado = funcionario.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = funcionario.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                CargarDatosFuncionarios(funcionarioEdit);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(funcionarioEdit);
        }

        [HttpPost]
        public ActionResult Edit(FuncionarioModel funcionarioModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN FUNCIONARIO CON EL MISMO NUMERO DOCUMENTO
                    int cantidad = db.funcionarios.Where(f => f.nro_documento.ToUpper() == funcionarioModelo.NroDocumento.Trim().ToUpper() && f.estado != null && f.id != funcionarioModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var funcionario = db.funcionarios.Where(f => f.id == funcionarioModelo.Id).FirstOrDefault();
                        funcionario.id_tipo_documento = funcionarioModelo.IdTipoDocumento;
                        funcionario.nro_documento = funcionarioModelo.NroDocumento;
                        funcionario.nombre = funcionarioModelo.Nombre;
                        funcionario.apellido = funcionarioModelo.Apellido;
                        funcionario.id_area = funcionarioModelo.IdArea;
                        funcionario.id_cargo = funcionarioModelo.IdCargo;
                        funcionario.direccion = funcionarioModelo.Direccion;
                        funcionario.id_sucursal = funcionarioModelo.IdSucursal;
                        bool nuevoEstado = funcionarioModelo.EstadoDescrip == "A" ? true : false;
                        funcionario.estado = nuevoEstado;
                        db.Entry(funcionario).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un funcionario registrado con el mismo número de documento");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar los datos del funcionario en la base de datos");
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
                CargarDatosFuncionarios(funcionarioModelo);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", funcionarioModelo.EstadoDescrip);
                db.Dispose();
                return View(funcionarioModelo);
            }
        }

        #endregion

        #region Eliminar Funcionario

        [HttpPost]
        public ActionResult EliminarFuncionario(string funcionarioId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Funcionarios", "EliminarFuncionario");
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
                                int idFuncionario = Convert.ToInt32(funcionarioId);
                                var funcionario = context.funcionarios.Where(f => f.id == idFuncionario).FirstOrDefault();
                                funcionario.estado = null;
                                context.Entry(funcionario).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Funcionarios") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Funciones

        private void CargarDatosFuncionarios(FuncionarioModel funcionarioModelo)
        {
            ViewBag.IdTipoDocumento = new SelectList(db.tipos_documentos.Where(td => td.estado == true).OrderBy(td => td.tipo).ToList(), "id", "tipo", funcionarioModelo.IdTipoDocumento);
            ViewBag.IdArea = new SelectList(db.areas.Where(a => a.estado == true).OrderBy(a => a.nombre_area).ToList(), "id", "nombre_area", funcionarioModelo.IdArea);
            if (funcionarioModelo.IdArea != null)
            {
                ViewBag.IdCargo = new SelectList(db.cargos.Where(c => c.estado == true && c.id_area == funcionarioModelo.IdArea).OrderBy(c => c.nombre_cargo).ToList(), "id", "nombre_cargo", funcionarioModelo.IdCargo);
            }
            else
            {
                ViewBag.IdCargo = new SelectList(db.cargos.Where(c => c.id == 0).OrderBy(c => c.nombre_cargo).ToList(), "id", "nombre_cargo");
            }
            ViewBag.IdSucursal = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", funcionarioModelo.IdSucursal);
        }

        #endregion

        #region AJAX

        [HttpPost]
        public ActionResult ObtenerListadoFuncionarios(string sucursalId)
        {
            int idSucursal = sucursalId != "" ? Convert.ToInt32(sucursalId) : 0;
            FuncionarioModel funcionarioModelo = new FuncionarioModel();
            List<ListaDinamica> listaFuncionarios = funcionarioModelo.ListadoFuncionarios(idSucursal);
            var funcionario = listaFuncionarios.OrderBy(f => f.Nombre).Select(f => "<option value='" + f.Id + "'>" + f.Nombre + "</option>");
            return Content(String.Join("", funcionario));
        }

        #endregion



    }
}