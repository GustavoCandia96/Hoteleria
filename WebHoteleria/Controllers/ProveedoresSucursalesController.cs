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
    public class ProveedoresSucursalesController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Proveedores Sucursales

        [HttpGet]
        [AutorizarUsuario("ProveedoresSucursales", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ProveedorSucursalModel> listaProveedoresSucursales = new List<ProveedorSucursalModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesIdProveedor = Convert.ToString(Session["sesionProveedoresSucursalesIdProveedor"]);
                ViewBag.ddlProveedores = new SelectList(db.proveedores.Where(p => p.estado == true && p.id_tipo_documento == 2).OrderBy(p => p.razon_social).ToList(), "id", "razon_social", sesIdProveedor);
                string sesNombre = Convert.ToString(Session["sesionProveedoresSucursalesNombre"]);
                ViewBag.txtNombre = sesNombre;


                //OBTENEMOS TODOS LOS PROVEEDORES SUCURSALES ACTIVOS DE LA BASE DE DATOS
                var proveedoresSucursales = from ps in db.proveedores_sucursales
                                            where ps.estado != null
                                            select new ProveedorSucursalModel
                                            {
                                                Id = ps.id,
                                                IdProveedor = ps.id_proveedor,
                                                NombreSucursal = ps.nombre_sucursal,
                                                IdPais = ps.id_pais,
                                                IdDepartamento = ps.id_departamento,
                                                IdCiudad = ps.id_ciudad,
                                                IdBarrio = ps.id_barrio,
                                                Direccion = ps.direccion,
                                                EmailPrincipal = ps.email_principal,
                                                NroTelefonoPrincipal = ps.nro_telefono_principal,
                                                CasaMatriz = ps.casa_matriz.Value,
                                                NombreProveedor = ps.proveedores.razon_social,
                                                NombreDepartamento = ps.id_departamento != null ? ps.departamentos.nombre_departamento : "",
                                                NombreCiudad = ps.id_ciudad != null ? ps.ciudades.nombre_ciudad : "",
                                                EstadoDescrip = ps.estado == true ? "Activo" : "Inactivo",
                                            };

                listaProveedoresSucursales = proveedoresSucursales.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesIdProveedor != "")
                {
                    int idProveedor = Convert.ToInt32(sesIdProveedor);
                    listaProveedoresSucursales = listaProveedoresSucursales.Where(ps => ps.IdProveedor == idProveedor).ToList();
                }
                if (sesNombre != "")
                {
                    listaProveedoresSucursales = listaProveedoresSucursales.Where(ps => ps.NombreSucursal.ToUpper().Contains(sesNombre.Trim().ToUpper())).ToList();
                }

                listaProveedoresSucursales = listaProveedoresSucursales.OrderBy(ps => ps.NombreSucursal).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de proveedores sucursales";
            }
            return View(listaProveedoresSucursales.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ProveedorSucursalModel> listaProveedoresSucursales = new List<ProveedorSucursalModel>();
            try
            {
                //OBTENEMOS TODOS LOS PROVEEDORES SUCURSALES ACTIVOS DE LA BASE DE DATOS
                var proveedoresSucursales = from ps in db.proveedores_sucursales
                                            where ps.estado != null
                                            select new ProveedorSucursalModel
                                            {
                                                Id = ps.id,
                                                IdProveedor = ps.id_proveedor,
                                                NombreSucursal = ps.nombre_sucursal,
                                                IdPais = ps.id_pais,
                                                IdDepartamento = ps.id_departamento,
                                                IdCiudad = ps.id_ciudad,
                                                IdBarrio = ps.id_barrio,
                                                Direccion = ps.direccion,
                                                EmailPrincipal = ps.email_principal,
                                                NroTelefonoPrincipal = ps.nro_telefono_principal,
                                                CasaMatriz = ps.casa_matriz.Value,
                                                NombreProveedor = ps.proveedores.razon_social,
                                                NombreDepartamento = ps.id_departamento != null ? ps.departamentos.nombre_departamento : "",
                                                NombreCiudad = ps.id_ciudad != null ? ps.ciudades.nombre_ciudad : "",
                                                EstadoDescrip = ps.estado == true ? "Activo" : "Inactivo",
                                            };

                listaProveedoresSucursales = proveedoresSucursales.ToList();

                //FILTRAMOS POR SUCURSAL Y NOMBRE
                var fcIdSucursal = fc["ddlProveedores"];
                if (fcIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(fcIdSucursal);
                    listaProveedoresSucursales = listaProveedoresSucursales.Where(ps => ps.IdProveedor == idSucursal).ToList();
                }
                var fcNombre = fc["txtNombre"];
                if (fcNombre != "")
                {
                    string nombre = Convert.ToString(fcNombre);
                    listaProveedoresSucursales = listaProveedoresSucursales.Where(ps => ps.NombreSucursal.ToUpper().Contains(nombre.ToUpper())).ToList();
                }

                listaProveedoresSucursales = listaProveedoresSucursales.OrderBy(ps => ps.NombreSucursal).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtNombre = fcNombre;
                Session["sesionProveedoresSucursalesNombre"] = fcNombre;
                ViewBag.ddlProveedores = new SelectList(db.proveedores.Where(p => p.estado == true && p.id_tipo_documento == 2).OrderBy(p => p.razon_social).ToList(), "id", "razon_social", fcIdSucursal);
                Session["sesionProveedoresSucursalesIdProveedor"] = fcIdSucursal;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar proveedores sucursales";
            }
            return View(listaProveedoresSucursales.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Proveedor Sucursal

        [HttpGet]
        [AutorizarUsuario("ProveedoresSucursales", "Create")]
        public ActionResult Create()
        {
            ProveedorSucursalModel proveedorSucursal = new ProveedorSucursalModel();
            ViewBag.IdProveedor = new SelectList(db.proveedores.Where(p => p.estado == true && p.id_tipo_documento == 2).OrderBy(p => p.razon_social).ToList(), "id", "razon_social");
            ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais");
            ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.id == 0).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento");
            ViewBag.IdCiudad = new SelectList(db.ciudades.Where(c => c.id == 0).OrderBy(c => c.nombre_ciudad).ToList(), "id", "nombre_ciudad");
            ViewBag.IdBarrio = new SelectList(db.barrios.Where(b => b.id == 0).OrderBy(b => b.nombre_barrio).ToList(), "id", "nombre_barrio");
            return View(proveedorSucursal);
        }

        [HttpPost]
        public ActionResult Create(ProveedorSucursalModel proveedorSucursalModelo, FormCollection fc)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;

            if (ModelState.IsValid)
            {
                using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        bool habilitado = true;
                        try
                        {
                            //VERIFICAMOS SI YA EXISTE UN PROVEEDOR SUCURSAL CON EL MISMO NOMBRE Y PROVEEDOR
                            int cantidad = context.proveedores_sucursales.Where(ps => ps.nombre_sucursal.ToUpper() == proveedorSucursalModelo.NombreSucursal.Trim().ToUpper() && ps.id_proveedor == proveedorSucursalModelo.IdProveedor && ps.estado != null).Count();
                            if (cantidad == 0)
                            {
                                int cantidadSucMat = 0;
                                if (proveedorSucursalModelo.CasaMatriz == true)
                                {
                                    cantidadSucMat = context.proveedores_sucursales.Where(ps => ps.id_proveedor == proveedorSucursalModelo.IdProveedor && ps.casa_matriz == true && ps.estado != null).Count();
                                }
                                if (cantidadSucMat == 0)
                                {
                                    //AGREGAMOS EL REGISTRO DE PROVEEDOR SUCURSAL CON TODOS LOS DATOS DISPONIBLES
                                    proveedores_sucursales proveedorSucursal = new proveedores_sucursales
                                    {
                                        id_proveedor = proveedorSucursalModelo.IdProveedor,
                                        nombre_sucursal = proveedorSucursalModelo.NombreSucursal,
                                        id_pais = proveedorSucursalModelo.IdPais,
                                        id_departamento = proveedorSucursalModelo.IdDepartamento,
                                        id_ciudad = proveedorSucursalModelo.IdCiudad,
                                        id_barrio = proveedorSucursalModelo.IdBarrio,
                                        direccion = proveedorSucursalModelo.Direccion,
                                        email_principal = proveedorSucursalModelo.EmailPrincipal,
                                        nro_telefono_principal = proveedorSucursalModelo.NroTelefonoPrincipal,
                                        casa_matriz = proveedorSucursalModelo.CasaMatriz,
                                        estado = true
                                    };
                                    context.proveedores_sucursales.Add(proveedorSucursal);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    habilitado = false;
                                    ModelState.AddModelError("DuplicadoCasaMatriz", "Ya existe una casa matriz de sucursal del proveedor seleccionado");
                                    retornoVista = true;
                                }
                            }
                            else
                            {
                                habilitado = false;
                                ModelState.AddModelError("Duplicado", "Ya existe una sucursal con el mismo nombre relacionado al proveedor seleccionado");
                                retornoVista = true;
                            }
                        }
                        catch (Exception)
                        {
                            habilitado = false;
                            ModelState.AddModelError("Error", "Ocurrio un error al agregar sucursal del proveedor en la base de datos");
                            retornoVista = true;
                        }
                        if (habilitado == true)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                    context.Database.Connection.Close();
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
                CargarDatosProveedorSucursal(proveedorSucursalModelo);
                return View(proveedorSucursalModelo);
            }
        }

        #endregion

        #region Editar Proveedor Sucursal

        [HttpGet]
        [AutorizarUsuario("ProveedoresSucursales", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProveedorSucursalModel proveedorSucursalEdit = new ProveedorSucursalModel();
            try
            {
                var proveedorSucursal = db.proveedores_sucursales.Where(ps => ps.id == id).FirstOrDefault();
                proveedorSucursalEdit.Id = proveedorSucursal.id;
                proveedorSucursalEdit.IdProveedor = proveedorSucursal.id_proveedor;
                proveedorSucursalEdit.NombreSucursal = proveedorSucursal.nombre_sucursal;
                proveedorSucursalEdit.EmailPrincipal = proveedorSucursal.email_principal;
                proveedorSucursalEdit.NroTelefonoPrincipal = proveedorSucursal.nro_telefono_principal;
                proveedorSucursalEdit.IdPais = proveedorSucursal.id_pais;
                proveedorSucursalEdit.IdDepartamento = proveedorSucursal.id_departamento;
                proveedorSucursalEdit.IdCiudad = proveedorSucursal.id_ciudad;
                proveedorSucursalEdit.IdBarrio = proveedorSucursal.id_barrio;
                proveedorSucursalEdit.Direccion = proveedorSucursal.direccion;
                proveedorSucursalEdit.CasaMatriz = proveedorSucursal.casa_matriz.Value;
                proveedorSucursalEdit.Estado = proveedorSucursal.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = proveedorSucursal.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                CargarDatosProveedorSucursal(proveedorSucursalEdit);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(proveedorSucursalEdit);
        }

        [HttpPost]
        public ActionResult Edit(ProveedorSucursalModel proveedorSucursalModelo, FormCollection fc)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;

            if (ModelState.IsValid)
            {
                using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        bool habilitado = true;
                        try
                        {
                            //VERIFICAMOS SI YA EXISTE UN PROVEEDOR SUCURSAL CON EL MISMO NOMBRE Y PROVEEDOR
                            int cantidad = context.proveedores_sucursales.Where(ps => ps.nombre_sucursal.ToUpper() == proveedorSucursalModelo.NombreSucursal.Trim().ToUpper() && ps.id_proveedor == proveedorSucursalModelo.IdProveedor && ps.estado != null && ps.id != proveedorSucursalModelo.Id).Count();
                            if (cantidad == 0)
                            {
                                int cantidadSucMat = 0;
                                if (proveedorSucursalModelo.CasaMatriz == true)
                                {
                                    cantidadSucMat = context.proveedores_sucursales.Where(ps => ps.id_proveedor == proveedorSucursalModelo.IdProveedor && ps.casa_matriz == true && ps.estado != null && ps.id != proveedorSucursalModelo.Id).Count();
                                }
                                if (cantidadSucMat == 0)
                                {
                                    //ACTUALIZAMOS LOS DATOS DEL PROVEEDOR SUCURSAL CON TODOS LOS VALORES OBTENIDOS
                                    var proveedorSucursal = db.proveedores_sucursales.Where(ps => ps.id == proveedorSucursalModelo.Id).FirstOrDefault();
                                    proveedorSucursal.id_proveedor = proveedorSucursalModelo.IdProveedor;
                                    proveedorSucursal.nombre_sucursal = proveedorSucursalModelo.NombreSucursal;
                                    proveedorSucursal.email_principal = proveedorSucursalModelo.EmailPrincipal;
                                    proveedorSucursal.nro_telefono_principal = proveedorSucursalModelo.NroTelefonoPrincipal;
                                    proveedorSucursal.id_pais = proveedorSucursalModelo.IdPais;
                                    proveedorSucursal.id_departamento = proveedorSucursalModelo.IdDepartamento;
                                    proveedorSucursal.id_ciudad = proveedorSucursalModelo.IdCiudad;
                                    proveedorSucursal.id_barrio = proveedorSucursalModelo.IdBarrio;
                                    proveedorSucursal.direccion = proveedorSucursalModelo.Direccion;
                                    proveedorSucursal.casa_matriz = proveedorSucursalModelo.CasaMatriz;
                                    bool nuevoEstado = proveedorSucursalModelo.EstadoDescrip == "A" ? true : false;
                                    proveedorSucursal.estado = nuevoEstado;
                                    db.Entry(proveedorSucursal).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    habilitado = false;
                                    ModelState.AddModelError("DuplicadoCasaMatriz", "Ya existe una casa matriz de sucursal del proveedor seleccionado");
                                    retornoVista = true;
                                }
                            }
                            else
                            {
                                habilitado = false;
                                ModelState.AddModelError("Duplicado", "Ya existe una sucursal con el mismo nombre relacionado al proveedor seleccionado");
                                retornoVista = true;
                            }
                        }
                        catch (Exception)
                        {
                            habilitado = false;
                            ModelState.AddModelError("Error", "Ocurrio un error al actualizar los datos de sucursal del proveedor en la base de datos");
                            retornoVista = true;
                        }
                        if (habilitado == true)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                    context.Database.Connection.Close();
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
                CargarDatosProveedorSucursal(proveedorSucursalModelo);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", proveedorSucursalModelo.EstadoDescrip);
                db.Dispose();
                return View(proveedorSucursalModelo);
            }
        }

        #endregion

        #region Eliminar Proveedor Sucursal

        [HttpPost]
        public ActionResult EliminarProveedorSucursal(string proveedorSucursalId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("ProveedoresSucursales", "EliminarProveedorSucursal");
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
                                int idProveedorSucursal = Convert.ToInt32(proveedorSucursalId);
                                var proveedorSucursal = context.proveedores_sucursales.Where(ps => ps.id == idProveedorSucursal).FirstOrDefault();
                                proveedorSucursal.estado = null;
                                context.Entry(proveedorSucursal).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "ProveedoresSucursales") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Funciones

        private void CargarDatosProveedorSucursal(ProveedorSucursalModel proveedorSucursalModelo)
        {
            ViewBag.IdProveedor = new SelectList(db.proveedores.Where(p => p.estado == true && p.id_tipo_documento == 2).OrderBy(p => p.razon_social).ToList(), "id", "razon_social", proveedorSucursalModelo.IdProveedor);
            ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", proveedorSucursalModelo.IdPais);
            int idPais = proveedorSucursalModelo.IdPais != null ? proveedorSucursalModelo.IdPais.Value : 0;
            ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.id_pais == idPais).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento", proveedorSucursalModelo.IdDepartamento);
            int idDepartamento = proveedorSucursalModelo.IdDepartamento != null ? proveedorSucursalModelo.IdDepartamento.Value : 0;
            ViewBag.IdCiudad = new SelectList(db.ciudades.Where(c => c.id_departamento == idDepartamento).OrderBy(c => c.nombre_ciudad).ToList(), "id", "nombre_ciudad", proveedorSucursalModelo.IdCiudad);
            int idCiudad = proveedorSucursalModelo.IdCiudad != null ? proveedorSucursalModelo.IdCiudad.Value : 0;
            ViewBag.IdBarrio = new SelectList(db.barrios.Where(b => b.id_ciudad == idCiudad).OrderBy(b => b.nombre_barrio).ToList(), "id", "nombre_barrio", proveedorSucursalModelo.IdBarrio);
        }

        #endregion

        #region AJAX

        [HttpPost]
        public ActionResult ObtenerListadoSucursalesProveedores(string proveedorId)
        {
            int idProveedor = proveedorId != "" ? Convert.ToInt32(proveedorId) : 0;
            ProveedorSucursalModel proveedorSucursalModelo = new ProveedorSucursalModel();
            List<ListaDinamica> listaSucursalesProveedor = proveedorSucursalModelo.ListadoProveedoresSucursales(idProveedor);
            var sucursalesProveedor = listaSucursalesProveedor.OrderBy(ps => ps.Nombre).Select(ps => "<option value='" + ps.Id + "'>" + ps.Nombre + "</option>");
            return Content(String.Join("", sucursalesProveedor));
        }

        #endregion


    }
}