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
    public class SucursalesController : Controller
    {

      #region Propiedades
        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities
();
        #endregion

        #region Listado de Sucursales

        [HttpGet]
        [AutorizarUsuario("Sucursales", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<SucursalModel> listaSucursales = new List<SucursalModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNombre = Convert.ToString(Session["sesionSucursalesNombre"]);
                ViewBag.txtNombreSucursal = sesNombre;

                //OBTENEMOS TODAS LAS SUCURSALES ACTIVOS DE LA BASE DE DATOS
                var sucursales = from s in db.sucursales
                                 where s.estado != null
                                 select new SucursalModel
                                 {
                                     Id = s.id,
                                     NombreSucursal = s.nombre_sucursal,
                                     IdPais = s.id_pais,
                                     IdDepartamento = s.id_departamento,
                                     IdCiudad = s.id_ciudad,
                                     IdBarrio = s.id_barrio,
                                     Direccion = s.direccion,
                                     Estado = s.estado,
                                     NombreDepartamento = s.id_departamento != null ? s.departamentos.nombre_departamento : "",
                                     NombreCiudad = s.id_ciudad != null ? s.ciudades.nombre_ciudad : "",
                                     EstadoDescrip = s.estado == true ? "Activo" : "Inactivo"
                                 };

                listaSucursales = sucursales.OrderBy(s => s.NombreSucursal).ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNombre != "")
                {
                    listaSucursales = listaSucursales.Where(s => s.NombreSucursal.ToUpper().Contains(sesNombre.Trim().ToUpper())).ToList();
                }
                listaSucursales = listaSucursales.OrderBy(s => s.NombreSucursal).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de sucursales";
            }
            return View(listaSucursales.ToPagedList(pageIndex, pageSize));
        }

        public JsonResult BuscarSucursales(string term)
        {
            using (hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities())
            {
                var resultado = db.sucursales.Where(x => x.nombre_sucursal.Contains(term) && x.estado != null)
                        .Select(x => x.nombre_sucursal).Take(5).ToList();


                return Json(resultado, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<SucursalModel> listaSucursales = new List<SucursalModel>();
            try
            {
                //OBTENEMOS TODAS LAS SUCURSALES ACTIVOS DE LA BASE DE DATOS
                var sucursales = from s in db.sucursales
                                 where s.estado != null
                                 select new SucursalModel
                                 {
                                     Id = s.id,
                                     NombreSucursal = s.nombre_sucursal,
                                     IdPais = s.id_pais,
                                     IdDepartamento = s.id_departamento,
                                     IdCiudad = s.id_ciudad,
                                     IdBarrio = s.id_barrio,
                                     Direccion = s.direccion,
                                     Estado = s.estado,
                                     NombreDepartamento = s.id_departamento != null ? s.departamentos.nombre_departamento : "",
                                     NombreCiudad = s.id_ciudad != null ? s.ciudades.nombre_ciudad : "",
                                     EstadoDescrip = s.estado == true ? "Activo" : "Inactivo"
                                 };

                listaSucursales = sucursales.OrderBy(s => s.NombreSucursal).ToList();

                //FILTRAMOS POR NOMBRE SUCURSAL
                var fcNombre = fc["txtNombreSucursal"];
                if (fcNombre != "")
                {
                    string nombreSucursal = Convert.ToString(fcNombre);
                    listaSucursales = listaSucursales.Where(s => s.NombreSucursal.ToUpper().Contains(nombreSucursal.ToUpper())).ToList();
                }

                listaSucursales.OrderBy(c => c.NombreSucursal).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtNombreSucursal = fcNombre;
                Session["sesionSucursalesNombre"] = fcNombre;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar sucursales";
            }
            return View(listaSucursales.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Sucursal

        [HttpGet]
        [AutorizarUsuario("Sucursales", "Create")]
        public ActionResult Create()
        {
            SucursalModel sucursal = new SucursalModel();
            ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais");
            ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.id == 0).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento");
            ViewBag.IdCiudad = new SelectList(db.ciudades.Where(c => c.id == 0).OrderBy(c => c.nombre_ciudad).ToList(), "id", "nombre_ciudad");
            ViewBag.IdBarrio = new SelectList(db.barrios.Where(b => b.id == 0).OrderBy(b => b.nombre_barrio).ToList(), "id", "nombre_barrio");
            return View(sucursal);
        }

        [HttpPost]
        public ActionResult Create(SucursalModel sucursalModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA SUCURSAL CON EL MISMO NOMBRE
                    int cantidad = db.sucursales.Where(s => s.nombre_sucursal.ToUpper() == sucursalModelo.NombreSucursal.Trim().ToUpper() && s.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //VERIFICAMOS SI YA EXISTE UNA SUCURSAL MATRIZ
                        int cantidadSucMat = 0;
                        if (sucursalModelo.CasaMatriz == true)
                        {
                            cantidadSucMat = db.sucursales.Where(s => s.casa_matriz == true && s.estado != null).Count();
                        }

                        if (cantidadSucMat == 0)
                        {
                            sucursales sucursal = new sucursales
                            {
                                nombre_sucursal = sucursalModelo.NombreSucursal,
                                id_pais = sucursalModelo.IdPais,
                                id_departamento = sucursalModelo.IdDepartamento,
                                id_ciudad = sucursalModelo.IdCiudad,
                                id_barrio = sucursalModelo.IdBarrio,
                                direccion = sucursalModelo.Direccion,
                                casa_matriz = sucursalModelo.CasaMatriz,
                                estado = true
                            };
                            db.sucursales.Add(sucursal);
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("DuplicadoCasaMatriz", "Ya existe una casa matriz de sucursal en la base de datos");
                            retornoVista = true;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una sucursal registrada con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la sucursal en la base de datos");
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
                CargarDatosSucursal(sucursalModelo);
                db.Dispose();
                return View(sucursalModelo);
            }
        }

        #endregion

        #region Editar Sucursal

        [HttpGet]
        [AutorizarUsuario("Sucursales", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SucursalModel sucursalEdit = new SucursalModel();
            try
            {
                var sucursal = db.sucursales.Where(c => c.id == id).FirstOrDefault();
                sucursalEdit.Id = sucursal.id;
                sucursalEdit.NombreSucursal = sucursal.nombre_sucursal;
                sucursalEdit.IdPais = sucursal.id_pais;
                sucursalEdit.IdDepartamento = sucursal.id_departamento;
                sucursalEdit.IdCiudad = sucursal.id_ciudad;
                sucursalEdit.IdBarrio = sucursal.id_barrio;
                sucursalEdit.Direccion = sucursal.direccion;
                sucursalEdit.CasaMatriz = sucursal.casa_matriz.Value;
                sucursalEdit.Estado = sucursal.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = sucursal.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                CargarDatosSucursal(sucursalEdit);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(sucursalEdit);
        }

        [HttpPost]
        public ActionResult Edit(SucursalModel sucursalModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA SUCURSAL CON EL MISMO NOMBRE
                    int cantidad = db.sucursales.Where(s => s.nombre_sucursal.ToUpper() == sucursalModelo.NombreSucursal.Trim().ToUpper() && s.id != sucursalModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //VERIFICAMOS SI YA EXISTE UNA SUCURSAL MATRIZ
                        int cantidadSucMat = 0;
                        if (sucursalModelo.CasaMatriz == true)
                        {
                            cantidadSucMat = db.sucursales.Where(s => s.casa_matriz == true && s.estado != null && s.id != sucursalModelo.Id).Count();
                        }

                        if (cantidadSucMat == 0)
                        {
                            var sucursal = db.sucursales.Where(s => s.id == sucursalModelo.Id).FirstOrDefault();
                            sucursal.nombre_sucursal = sucursalModelo.NombreSucursal;
                            sucursal.id_pais = sucursalModelo.IdPais;
                            sucursal.id_departamento = sucursalModelo.IdDepartamento;
                            sucursal.id_ciudad = sucursalModelo.IdCiudad;
                            sucursal.id_barrio = sucursalModelo.IdBarrio;
                            sucursal.direccion = sucursalModelo.Direccion;
                            sucursal.casa_matriz = sucursalModelo.CasaMatriz;
                            bool nuevoEstado = sucursalModelo.EstadoDescrip == "A" ? true : false;
                            sucursal.estado = nuevoEstado;
                            db.Entry(sucursal).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("DuplicadoCasaMatriz", "Ya existe una casa matriz de sucursal en la base de datos");
                            retornoVista = true;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una sucursal registrada con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar la sucursal en la base de datos");
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
                CargarDatosSucursal(sucursalModelo);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", sucursalModelo.EstadoDescrip);
                db.Dispose();
                return View(sucursalModelo);
            }
        }

        #endregion

        #region Eliminar Sucursal

        [HttpPost]
        public ActionResult EliminarSucursal(string sucursalId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Sucursales", "EliminarSucursal");
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
                                int idSucursal = Convert.ToInt32(sucursalId);
                                var sucursal = context.sucursales.Where(s => s.id == idSucursal).FirstOrDefault();
                                sucursal.estado = null;
                                context.Entry(sucursal).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Sucursales") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Funciones

        private void CargarDatosSucursal(SucursalModel sucursalModelo)
        {
            ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", sucursalModelo.IdPais);
            if (sucursalModelo.IdPais != null)
            {
                ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.id_pais == sucursalModelo.IdPais).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento", sucursalModelo.IdDepartamento);
            }
            else
            {
                ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.id == 0).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento");
            }
            if (sucursalModelo.IdDepartamento != null)
            {
                ViewBag.IdCiudad = new SelectList(db.ciudades.Where(c => c.id_departamento == sucursalModelo.IdDepartamento).OrderBy(c => c.nombre_ciudad).ToList(), "id", "nombre_ciudad", sucursalModelo.IdCiudad);
            }
            else
            {
                ViewBag.IdCiudad = new SelectList(db.ciudades.Where(c => c.id == 0).OrderBy(c => c.nombre_ciudad).ToList(), "id", "nombre_ciudad");
            }
            if (sucursalModelo.IdCiudad != null)
            {
                ViewBag.IdBarrio = new SelectList(db.barrios.Where(b => b.id_ciudad == sucursalModelo.IdCiudad).OrderBy(b => b.nombre_barrio).ToList(), "id", "nombre_barrio", sucursalModelo.IdBarrio);
            }
            else
            {
                ViewBag.IdBarrio = new SelectList(db.barrios.Where(b => b.id == 0).OrderBy(b => b.nombre_barrio).ToList(), "id", "nombre_barrio");
            }
        }

        #endregion


    }
}