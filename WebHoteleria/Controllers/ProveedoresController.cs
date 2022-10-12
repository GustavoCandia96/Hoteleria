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
    public class ProveedoresController : Controller
    {

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #region Listado de Proveedores

        [HttpGet]
        [AutorizarUsuario("Proveedores", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ProveedorModel> listaProveedores = new List<ProveedorModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNroDoc = Convert.ToString(Session["sesionProveedoresNroDocumento"]);
                ViewBag.txtNroDocumento = sesNroDoc;
                string sesNombre = Convert.ToString(Session["sesionProveedoresNombre"]);
                ViewBag.txtNombre = sesNombre;
                string sesIdTipoDocumento = Convert.ToString(Session["sesionProveedoresIdTipoDocumento"]);
                ViewBag.ddlTiposDocumentos = new SelectList(db.tipos_documentos.Where(td => td.estado == true).OrderBy(td => td.tipo).ToList(), "id", "tipo", sesIdTipoDocumento);

                //OBTENEMOS TODOS LOS PROVEEDORES ACTIVOS DE LA BASE DE DATOS
                var proveedores = from p in db.proveedores
                                  where p.estado != null
                                  select new ProveedorModel
                                  {
                                      Id = p.id,
                                      IdTipoDocumento = p.id_tipo_documento,
                                      IdActividadEconomica = p.id_actividad_economica,
                                      NroDocumento = p.nro_documento,
                                      NombreCompleto = p.id_tipo_documento == 1 ? p.nombre + " " + p.apellido : p.razon_social,
                                      EmailPrincipal = p.email_principal,
                                      NroTelefonoPrincipal = p.nro_telefono_principal,
                                      IdPais = p.id_pais,
                                      IdDepartamento = p.id_departamento,
                                      IdCiudad = p.id_ciudad,
                                      IdBarrio = p.id_barrio,
                                      Direccion = p.direccion,
                                      Estado = p.estado,
                                      NombreTipoDocumento = p.tipos_documentos.tipo,
                                      NombreDepartamento = p.id_departamento != null ? p.departamentos.nombre_departamento : "",
                                      NombreCiudad = p.id_ciudad != null ? p.ciudades.nombre_ciudad : "",
                                      EstadoDescrip = p.estado == true ? "Activo" : "Inactivo"
                                  };

                listaProveedores = proveedores.OrderBy(c => c.NombreCompleto).ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNroDoc != "")
                {
                    listaProveedores = listaProveedores.Where(p => p.NroDocumento.ToUpper().Contains(sesNroDoc.Trim().ToUpper())).ToList();
                }
                if (sesNombre != "")
                {
                    listaProveedores = listaProveedores.Where(p => p.NombreCompleto.ToUpper().Contains(sesNombre.Trim().ToUpper())).ToList();
                }
                if (sesIdTipoDocumento != "")
                {
                    int idTipoDocumento = Convert.ToInt32(sesIdTipoDocumento);
                    listaProveedores = listaProveedores.Where(p => p.IdTipoDocumento == idTipoDocumento).ToList();
                }
                listaProveedores = listaProveedores.OrderBy(p => p.NombreCompleto).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de proveedores";
            }
            return View(listaProveedores.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<ProveedorModel> listaProveedores = new List<ProveedorModel>();
            try
            {
                //OBTENEMOS TODOS LOS PROVEEDORES ACTIVOS DE LA BASE DE DATOS
                var proveedores = from p in db.proveedores
                                  where p.estado != null
                                  select new ProveedorModel
                                  {
                                      Id = p.id,
                                      IdTipoDocumento = p.id_tipo_documento,
                                      IdActividadEconomica = p.id_actividad_economica,
                                      NroDocumento = p.nro_documento,
                                      NombreCompleto = p.id_tipo_documento == 1 ? p.nombre + " " + p.apellido : p.razon_social,
                                      EmailPrincipal = p.email_principal,
                                      NroTelefonoPrincipal = p.nro_telefono_principal,
                                      IdPais = p.id_pais,
                                      IdDepartamento = p.id_departamento,
                                      IdCiudad = p.id_ciudad,
                                      IdBarrio = p.id_barrio,
                                      Direccion = p.direccion,
                                      Estado = p.estado,
                                      NombreTipoDocumento = p.tipos_documentos.tipo,
                                      NombreDepartamento = p.id_departamento != null ? p.departamentos.nombre_departamento : "",
                                      NombreCiudad = p.id_ciudad != null ? p.ciudades.nombre_ciudad : "",
                                      EstadoDescrip = p.estado == true ? "Activo" : "Inactivo"
                                  };

                listaProveedores = proveedores.OrderBy(c => c.NombreCompleto).ToList();

                //FILTRAMOS POR TIPO DOCUMENTO N° DOCUMENTO Y NOMBRE
                var fcNroDocumento = fc["txtNroDocumento"];
                if (fcNroDocumento != "")
                {
                    string nroDocumento = Convert.ToString(fcNroDocumento);
                    listaProveedores = listaProveedores.Where(p => p.NroDocumento.ToUpper().Contains(nroDocumento.ToUpper())).ToList();
                }

                var fcNombre = fc["txtNombre"];
                if (fcNombre != "")
                {
                    string nombre = Convert.ToString(fcNombre);
                    listaProveedores = listaProveedores.Where(p => p.NombreCompleto.ToUpper().Contains(nombre.ToUpper())).ToList();
                }

                var fcIdTipoDocumento = fc["ddlTiposDocumentos"];
                int idTipoDocumento = fcIdTipoDocumento != "" ? Convert.ToInt32(fcIdTipoDocumento) : 0;
                if (fcIdTipoDocumento != "")
                {
                    listaProveedores = listaProveedores.Where(p => p.IdTipoDocumento == idTipoDocumento).ToList();
                }

                listaProveedores = listaProveedores.OrderBy(p => p.NombreCompleto).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtNroDocumento = fcNroDocumento;
                Session["sesionProveedoresNroDocumento"] = fcNroDocumento;
                ViewBag.txtNombre = fcNombre;
                Session["sesionProveedoresNombre"] = fcNombre;
                ViewBag.ddlTiposDocumentos = new SelectList(db.tipos_documentos.Where(td => td.estado == true).OrderBy(td => td.tipo).ToList(), "id", "tipo", fcIdTipoDocumento);
                Session["sesionProveedoresIdTipoDocumento"] = fcIdTipoDocumento;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar proveedores";
            }
            return View(listaProveedores.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Proveedor

        [HttpGet]
        [AutorizarUsuario("Proveedores", "Create")]
        public ActionResult Create()
        {
            ProveedorModel proveedor = new ProveedorModel();
            ViewBag.IdTipoDocumento = new SelectList(db.tipos_documentos.Where(td => td.estado == true).OrderBy(td => td.tipo).ToList(), "id", "tipo");
            ViewBag.IdActividadEconomica = new SelectList(db.actividades_economicas.Where(ae => ae.estado == true).OrderBy(ae => ae.actividad).ToList(), "id", "actividad");
            ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais");
            ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.id == 0).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento");
            ViewBag.IdCiudad = new SelectList(db.ciudades.Where(c => c.id == 0).OrderBy(c => c.nombre_ciudad).ToList(), "id", "nombre_ciudad");
            ViewBag.IdBarrio = new SelectList(db.barrios.Where(b => b.id == 0).OrderBy(b => b.nombre_barrio).ToList(), "id", "nombre_barrio");
            ViewBag.IdTipoContacto = new SelectList(db.tipos_contactos.Where(tc => tc.estado == true).OrderBy(tc => tc.descripcion).ToList(), "id", "descripcion");
            return View(proveedor);
        }

        [HttpPost]
        public ActionResult Create(ProveedorModel proveedorModelo, FormCollection fc)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            string[] arrId = (fc["arrId"] != null ? fc["arrId"].Split(',') : new string[] { });
            string[] arrIdTipoContacto = (fc["arrIdTipoContacto"] != null ? fc["arrIdTipoContacto"].Split(',') : new string[] { });
            string[] arrContacto = (fc["arrContacto"] != null ? fc["arrContacto"].Split(',') : new string[] { });

            if (ModelState.IsValid)
            {
                using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        bool habilitado = true;
                        try
                        {
                            //VERIFICAMOS SI YA EXISTE UN PROVEEDOR CON EL MISMO TIPO DE DOCUMENTO Y NUMERO
                            int cantidad = context.proveedores.Where(p => p.nro_documento.ToUpper() == proveedorModelo.NroDocumento.Trim().ToUpper() && p.id_tipo_documento == proveedorModelo.IdTipoDocumento && p.estado != null).Count();
                            if (cantidad == 0)
                            {
                                //AGREGAMOS EL REGISTRO DE PROVEEDOR CON TODOS LOS DATOS DISPONIBLES
                                proveedores proveedor = new proveedores
                                {
                                    id_actividad_economica = proveedorModelo.IdActividadEconomica,
                                    id_tipo_documento = proveedorModelo.IdTipoDocumento,
                                    nro_documento = proveedorModelo.NroDocumento,
                                    nombre = proveedorModelo.Nombre,
                                    apellido = proveedorModelo.Apellido,
                                    razon_social = proveedorModelo.RazonSocial,
                                    email_principal = proveedorModelo.EmailPrincipal,
                                    nro_telefono_principal = proveedorModelo.NroTelefonoPrincipal,
                                    id_pais = proveedorModelo.IdPais,
                                    id_departamento = proveedorModelo.IdDepartamento,
                                    id_ciudad = proveedorModelo.IdCiudad,
                                    id_barrio = proveedorModelo.IdBarrio,
                                    direccion = proveedorModelo.Direccion,
                                    estado = true
                                };
                                context.proveedores.Add(proveedor);
                                context.SaveChanges();

                                //AGREGAMOS LOS DATOS DE CONTACTO DEL PROVEEDOR REGISTRADO
                                for (int i = 0; i < arrIdTipoContacto.Length; i++)
                                {
                                    int idTipoContacto = Convert.ToInt32(arrIdTipoContacto[i]);
                                    string contacto = Convert.ToString(arrContacto[i]);

                                    proveedores_contactos proveedorContacto = new proveedores_contactos
                                    {
                                        id_proveedor = proveedor.id,
                                        id_tipo_contacto = idTipoContacto,
                                        contacto = contacto,
                                        estado = true
                                    };
                                    context.proveedores_contactos.Add(proveedorContacto);
                                    context.SaveChanges();
                                }
                            }
                            else
                            {
                                habilitado = false;
                                ModelState.AddModelError("Duplicado", "Ya existe un proveedor registrado con el mismo número de documento y tipo documento");
                                retornoVista = true;
                            }
                        }
                        catch (Exception)
                        {
                            habilitado = false;
                            ModelState.AddModelError("Error", "Ocurrio un error al agregar al proveedor en la base de datos");
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
                CargarDatosProveedor(proveedorModelo);
                CargarDatosContactoDesdeVista(arrId, arrIdTipoContacto, arrContacto);
                return View(proveedorModelo);
            }
        }

        #endregion

        #region Editar Proveedor

        [HttpGet]
        [AutorizarUsuario("Proveedores", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProveedorModel proveedorEdit = new ProveedorModel();
            try
            {
                var proveedor = db.proveedores.Where(p => p.id == id).FirstOrDefault();
                proveedorEdit.Id = proveedor.id;
                proveedorEdit.IdActividadEconomica = proveedor.id_actividad_economica;
                proveedorEdit.IdTipoDocumento = proveedor.id_tipo_documento;
                proveedorEdit.NroDocumento = proveedor.nro_documento;
                proveedorEdit.Nombre = proveedor.nombre;
                proveedorEdit.Apellido = proveedor.apellido;
                proveedorEdit.RazonSocial = proveedor.razon_social;
                proveedorEdit.EmailPrincipal = proveedor.email_principal;
                proveedorEdit.NroTelefonoPrincipal = proveedor.nro_telefono_principal;
                proveedorEdit.IdPais = proveedor.id_pais;
                proveedorEdit.IdDepartamento = proveedor.id_departamento;
                proveedorEdit.IdCiudad = proveedor.id_ciudad;
                proveedorEdit.IdBarrio = proveedor.id_barrio;
                proveedorEdit.Direccion = proveedor.direccion;
                proveedorEdit.Estado = proveedor.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = proveedor.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                CargarDatosProveedor(proveedorEdit);
                CargarDatosContactoDesdeBD(proveedorEdit.Id);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(proveedorEdit);
        }

        [HttpPost]
        public ActionResult Edit(ProveedorModel proveedorModelo, FormCollection fc)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            string[] arrId = (fc["arrId"] != null ? fc["arrId"].Split(',') : new string[] { });
            string[] arrIdTipoContacto = (fc["arrIdTipoContacto"] != null ? fc["arrIdTipoContacto"].Split(',') : new string[] { });
            string[] arrContacto = (fc["arrContacto"] != null ? fc["arrContacto"].Split(',') : new string[] { });

            if (ModelState.IsValid)
            {
                using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        bool habilitado = true;
                        try
                        {
                            //VERIFICAMOS SI YA EXISTE UN PROVEEDOR CON EL MISMO TIPO DE DOCUMENTO Y NUMERO
                            int cantidad = context.proveedores.Where(p => p.nro_documento.ToUpper() == proveedorModelo.NroDocumento.Trim().ToUpper() && p.id_tipo_documento == proveedorModelo.IdTipoDocumento && p.estado != null && p.id != proveedorModelo.Id).Count();
                            if (cantidad == 0)
                            {
                                //ACTUALIZAMOS LOS DATOS DEL PROVEEDOR CON TODOS LOS VALORES OBTENIDOS
                                var proveedor = db.proveedores.Where(p => p.id == proveedorModelo.Id).FirstOrDefault();
                                proveedor.id_actividad_economica = proveedorModelo.IdActividadEconomica;
                                proveedor.id_tipo_documento = proveedorModelo.IdTipoDocumento;
                                proveedor.nro_documento = proveedorModelo.NroDocumento;
                                proveedor.nombre = proveedorModelo.Nombre;
                                proveedor.apellido = proveedorModelo.Apellido;
                                proveedor.razon_social = proveedorModelo.RazonSocial;
                                proveedor.email_principal = proveedorModelo.EmailPrincipal;
                                proveedor.nro_telefono_principal = proveedorModelo.NroTelefonoPrincipal;
                                proveedor.id_pais = proveedorModelo.IdPais;
                                proveedor.id_departamento = proveedorModelo.IdDepartamento;
                                proveedor.id_ciudad = proveedorModelo.IdCiudad;
                                proveedor.id_barrio = proveedorModelo.IdBarrio;
                                proveedor.direccion = proveedorModelo.Direccion;
                                bool nuevoEstado = proveedorModelo.EstadoDescrip == "A" ? true : false;
                                proveedor.estado = nuevoEstado;
                                db.Entry(proveedor).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                                //ACTUALIZAMOS LOS DATOS DE CONTACTO OBTENIDO DEL PROVEEDOR
                                var listaContactoBD = db.proveedores_contactos.Where(pc => pc.id_proveedor == proveedor.id).ToList();
                                foreach (var item in listaContactoBD)
                                {
                                    var detalle = db.proveedores_contactos.Where(pc => pc.id == item.id).FirstOrDefault();
                                    db.Entry(detalle).State = System.Data.Entity.EntityState.Deleted;
                                    db.SaveChanges();
                                }
                                for (int i = 0; i < arrIdTipoContacto.Length; i++)
                                {
                                    int idTipoContacto = Convert.ToInt32(arrIdTipoContacto[i]);
                                    string contacto = Convert.ToString(arrContacto[i]);

                                    proveedores_contactos proveedorContacto = new proveedores_contactos
                                    {
                                        id_proveedor = proveedor.id,
                                        id_tipo_contacto = idTipoContacto,
                                        contacto = contacto,
                                        estado = true
                                    };
                                    db.proveedores_contactos.Add(proveedorContacto);
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                habilitado = false;
                                ModelState.AddModelError("Duplicado", "Ya existe un proveedor registrado con el mismo número de documento y tipo documento");
                                retornoVista = true;
                            }
                        }
                        catch (Exception)
                        {
                            habilitado = false;
                            ModelState.AddModelError("Error", "Ocurrio un error al actualizar los datos del proveedor en la base de datos");
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
                CargarDatosProveedor(proveedorModelo);
                CargarDatosContactoDesdeVista(arrId, arrIdTipoContacto, arrContacto);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", proveedorModelo.EstadoDescrip);
                db.Dispose();
                return View(proveedorModelo);
            }
        }

        #endregion

        #region Eliminar Proveedor

        [HttpPost]
        public ActionResult EliminarProveedor(string proveedorId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Proveedores", "EliminarProveedor");
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
                                int idProveedor = Convert.ToInt32(proveedorId);
                                var proveedor = context.proveedores.Where(p => p.id == idProveedor).FirstOrDefault();
                                proveedor.estado = null;
                                context.Entry(proveedor).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Proveedores") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Llamada a Vistas Parciales

        [HttpGet]
        public ActionResult PartialProveedorFisico()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PartialProveedorJuridico()
        {
            return View();
        }

        #endregion

        #region Funciones

        private void CargarDatosProveedor(ProveedorModel proveedorModelo)
        {
            ViewBag.IdActividadEconomica = new SelectList(db.actividades_economicas.Where(ae => ae.estado == true).OrderBy(ae => ae.actividad).ToList(), "id", "actividad", proveedorModelo.IdActividadEconomica);
            ViewBag.IdTipoDocumento = new SelectList(db.tipos_documentos.Where(td => td.estado == true).OrderBy(td => td.tipo).ToList(), "id", "tipo", proveedorModelo.IdTipoDocumento);
            ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", proveedorModelo.IdPais);
            int idPais = proveedorModelo.IdPais != null ? proveedorModelo.IdPais.Value : 0;
            ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.id_pais == idPais).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento", proveedorModelo.IdDepartamento);
            int idDepartamento = proveedorModelo.IdDepartamento != null ? proveedorModelo.IdDepartamento.Value : 0;
            ViewBag.IdCiudad = new SelectList(db.ciudades.Where(c => c.id_departamento == idDepartamento).OrderBy(c => c.nombre_ciudad).ToList(), "id", "nombre_ciudad", proveedorModelo.IdCiudad);
            int idCiudad = proveedorModelo.IdCiudad != null ? proveedorModelo.IdCiudad.Value : 0;
            ViewBag.IdBarrio = new SelectList(db.barrios.Where(b => b.id_ciudad == idCiudad).OrderBy(b => b.nombre_barrio).ToList(), "id", "nombre_barrio", proveedorModelo.IdBarrio);
            ViewBag.IdTipoContacto = new SelectList(db.tipos_contactos.Where(tc => tc.estado == true).OrderBy(tc => tc.descripcion).ToList(), "id", "descripcion");
        }

        private void CargarDatosContactoDesdeVista(string[] arrId, string[] arrIdTipoContacto, string[] arrContacto)
        {
            List<ProveedorContactoModel> listaProveedorContacto = new List<ProveedorContactoModel>();
            for (int i = 0; i < arrIdTipoContacto.Length; i++)
            {
                ProveedorContactoModel carga = new ProveedorContactoModel();
                int id = Convert.ToInt32(arrId[i]);
                carga.Id = id;
                int idTipoContacto = Convert.ToInt32(arrIdTipoContacto[i]);
                carga.IdTipoContacto = idTipoContacto;
                carga.NombreTipoContacto = db.tipos_contactos.Where(tc => tc.id == idTipoContacto).FirstOrDefault().descripcion.ToString();
                carga.Contacto = Convert.ToString(arrContacto[i]);
                listaProveedorContacto.Add(carga);
            }
            ViewBag.ListaProveedorContacto = listaProveedorContacto.Count > 0 ? listaProveedorContacto : null;
        }

        private void CargarDatosContactoDesdeBD(int idProveedor)
        {
            List<ProveedorContactoModel> listaProveedorContacto = new List<ProveedorContactoModel>();
            var lista = db.proveedores_contactos.Where(pc => pc.id_proveedor == idProveedor).ToList();
            foreach (var item in lista)
            {
                ProveedorContactoModel carga = new ProveedorContactoModel();
                carga.Id = item.id;
                carga.IdTipoContacto = item.id_tipo_contacto;
                carga.NombreTipoContacto = item.tipos_contactos.descripcion;
                carga.Contacto = item.contacto;
                listaProveedorContacto.Add(carga);
            }
            ViewBag.ListaProveedorContacto = listaProveedorContacto.Count > 0 ? listaProveedorContacto : null;
        }

        #endregion



    }
}