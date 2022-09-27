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
    public class CargosController : Controller
    {

        /*
        * CARGOS
        * 
        * El modulo de cargos registra todos los cargos que tiene un funcionario dentro de la empresa. Está relacionado con el modulo de funcionarios. 
        * Ejemplo (jefe de ventas, asistente TI).
        * 

        */

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Cargos

        [HttpGet]
        [AutorizarUsuario("Cargos", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
            string sesIdArea = Convert.ToString(Session["sesionCargosIdArea"]);
            ViewBag.ddlAreas = new SelectList(db.areas.Where(a => a.estado == true).OrderBy(a => a.nombre_area).ToList(), "id", "nombre_area", sesIdArea);
            string sesNomCargo = Convert.ToString(Session["sesionCargosNombre"]);
            ViewBag.txtCargo = sesNomCargo;

            List<CargoModel> listaCargos = new List<CargoModel>();
            try
            {
                //OBTENEMOS TODOS LOS CARGOS NO ELIMINADOS DE LA BASE DE DATOS
                var cargos = from c in db.cargos
                             where c.estado != null
                             select new CargoModel
                             {
                                 Id = c.id,
                                 IdArea = c.id_area,
                                 NombreCargo = c.nombre_cargo,
                                 NombreArea = c.areas.nombre_area,
                                 Estado = c.estado,
                                 EstadoDescrip = c.estado == true ? "Activo" : "Inactivo"
                             };
                listaCargos = cargos.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesIdArea != "")
                {
                    int idArea = Convert.ToInt32(sesIdArea);
                    listaCargos = listaCargos.Where(c => c.IdArea == idArea).ToList();
                }
                if (sesNomCargo != "")
                {
                    listaCargos = listaCargos.Where(c => c.NombreCargo.ToUpper().Contains(sesNomCargo.Trim().ToUpper())).ToList();
                }

                listaCargos = listaCargos.OrderBy(c => c.NombreCargo).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de cargos";
            }
            return View(listaCargos.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CargoModel> listaCargos = new List<CargoModel>();
            try
            {
                //OBTENEMOS TODOS LOS CARGOS NO ELIMINADOS DE LA BASE DE DATOS
                var cargos = from c in db.cargos
                             where c.estado != null
                             select new CargoModel
                             {
                                 Id = c.id,
                                 IdArea = c.id_area,
                                 NombreCargo = c.nombre_cargo,
                                 NombreArea = c.areas.nombre_area,
                                 Estado = c.estado,
                                 EstadoDescrip = c.estado == true ? "Activo" : "Inactivo"
                             };
                listaCargos = cargos.ToList();

                //FILTRAMOS POR AREA Y NOMBRE CARGO LA BUSQUEDA
                var fcIdArea = fc["ddlAreas"];
                if (fcIdArea != "")
                {
                    int idArea = Convert.ToInt32(fcIdArea);
                    listaCargos = listaCargos.Where(c => c.IdArea == idArea).ToList();
                }
                var fcNombreCargo = fc["txtCargo"];
                if (fcNombreCargo != "")
                {
                    string descripcion = Convert.ToString(fcNombreCargo);
                    listaCargos = listaCargos.Where(c => c.NombreCargo.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }
                listaCargos = listaCargos.OrderBy(c => c.NombreCargo).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.ddlAreas = new SelectList(db.areas.Where(a => a.estado == true).OrderBy(a => a.nombre_area).ToList(), "id", "nombre_area", fcIdArea);
                Session["sesionCargosIdArea"] = fcIdArea;
                ViewBag.txtCargo = fcNombreCargo;
                Session["sesionCargosNombre"] = fcNombreCargo;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar cargos";
            }
            return View(listaCargos.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Cargo

        [HttpGet]
        [AutorizarUsuario("Cargos", "Create")]
        public ActionResult Create()
        {
            CargoModel cargo = new CargoModel();
            //OBTENEMOS EL LISTADO DE AREAS ACTIVAS PARA RELACIONAR CON EL CARGO
            ViewBag.IdArea = new SelectList(db.areas.Where(a => a.estado == true).OrderBy(a => a.nombre_area).ToList(), "id", "nombre_area");
            return View(cargo);
        }

        [HttpPost]
        public ActionResult Create(CargoModel cargoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN CARGO RELACIONADO CON UN AREA
                    int cantidad = db.cargos.Where(c => c.nombre_cargo.Trim().ToUpper() == cargoModelo.NombreCargo.Trim().ToUpper() && c.id_area == cargoModelo.IdArea && c.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //AGREGAMOS EL REGISTRO DE CARGO
                        cargos cargo = new cargos
                        {
                            id_area = cargoModelo.IdArea,
                            nombre_cargo = cargoModelo.NombreCargo.Trim(),
                            descripcion = cargoModelo.Descripcion,
                            estado = true
                        };
                        db.cargos.Add(cargo);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un cargo registrado con el mismo nombre y area");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el cargo en la base de datos");
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
                //OBTENEMOS EL LISTADODE AREAS PARA RELACIONAR CON EL CARGO
                ViewBag.IdArea = new SelectList(db.areas.Where(a => a.estado == true).OrderBy(a => a.nombre_area).ToList(), "id", "nombre_area", cargoModelo.IdArea);
                db.Dispose();
                return View(cargoModelo);
            }
        }

        #endregion

        #region Editar Cargo

        [HttpGet]
        [AutorizarUsuario("Cargos", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CargoModel cargoEdit = new CargoModel();
            try
            {
                //OBTENEMOS EL REGISTRO Y CARGAMOS LOS DATOS EN LAS PROPIEDADES DEL MODELO
                var cargo = db.cargos.Where(p => p.id == id).FirstOrDefault();
                cargoEdit.Id = cargo.id;
                cargoEdit.IdArea = cargo.id_area;
                cargoEdit.NombreCargo = cargo.nombre_cargo;
                cargoEdit.Descripcion = cargo.descripcion;
                cargoEdit.Estado = cargo.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = cargo.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                //OBTENEMOS EL LISTADO DE AREAS ACTIVAS PARA RELACIONAR CON EL CARGO
                ViewBag.IdArea = new SelectList(db.areas.Where(a => a.estado == true).OrderBy(a => a.nombre_area).ToList(), "id", "nombre_area", cargoEdit.IdArea);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(cargoEdit);
        }

        [HttpPost]
        public ActionResult Edit(CargoModel cargoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN CARGO RELACIONADO CON UN AREA PARA PODER ACTUALIZAR
                    int cantidad = db.cargos.Where(c => c.nombre_cargo.Trim().ToUpper() == cargoModelo.NombreCargo.Trim().ToUpper() && c.id_area == cargoModelo.IdArea && c.estado != null && c.id != cargoModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //OBTENEMOS EL REGISTRO DE CARGOS PARA PODER ACTUALIZAR
                        var cargo = db.cargos.Where(p => p.id == cargoModelo.Id).FirstOrDefault();
                        cargo.id_area = cargoModelo.IdArea;
                        cargo.nombre_cargo = cargoModelo.NombreCargo.Trim();
                        cargo.descripcion = cargoModelo.Descripcion;
                        bool nuevoEstado = cargoModelo.EstadoDescrip == "A" ? true : false;
                        cargo.estado = nuevoEstado;
                        db.Entry(cargo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un cargo registrado con el mismo nombre y area");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el cargo en la base de datos");
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
                //OBTENEMOS EL LISTADO DE AREAS ACTIVAS PARA RELACIONAR CON UN CARGO
                ViewBag.IdArea = new SelectList(db.areas.Where(a => a.estado == true).OrderBy(a => a.nombre_area).ToList(), "id", "nombre_area", cargoModelo.IdArea);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", cargoModelo.EstadoDescrip);
                db.Dispose();
                return View(cargoModelo);
            }
        }

        #endregion

        #region Eliminar Cargo

        [HttpPost]
        public ActionResult EliminarCargo(string cargoId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Cargos", "EliminarCargo");
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
                                int idCargo = Convert.ToInt32(cargoId);
                                var cargo = context.cargos.Where(p => p.id == idCargo).FirstOrDefault();
                                cargo.estado = null;
                                context.Entry(cargo).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Cargos") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region AJAX

        /*
         * OBTIENE EL LISTADO DE CARGOS FILTRADO POR AREA
         */
        [HttpPost]
        public ActionResult ObtenerListadoCargos(string areaId)
        {
            int idArea = areaId != "" ? Convert.ToInt32(areaId) : 0;
            CargoModel cargoModelo = new CargoModel();
            List<ListaDinamica> listaCargos = cargoModelo.ListadoCargos(idArea);
            var cargos = listaCargos.OrderBy(c => c.Nombre).Select(c => "<option value='" + c.Id + "'>" + c.Nombre + "</option>");
            return Content(String.Join("", cargos));
        }

        #endregion


    }
}