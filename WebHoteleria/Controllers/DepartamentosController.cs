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
    public class DepartamentosController : Controller
    {
        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities
();


        #endregion

        #region Listado de Departamentos

        [HttpGet]
        [AutorizarUsuario("Departamentos", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<DepartamentoModel> listaDepartamentos = new List<DepartamentoModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomDep = Convert.ToString(Session["sesionDepartamentosNombre"]);
                ViewBag.txtDepartamento = sesNomDep;
                string sesIdPais = Convert.ToString(Session["sesionDepartamentosIdPais"]);
                ViewBag.ddlPaises = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", sesIdPais);

                //OBTENEMOS TODOS LOS DEPARTAMENTOS ACTIVOS DE LA BASE DE DATOS Y ORDENAMOS POR DESCRIPCIÓN
                var departamentos = from d in db.departamentos
                                    join p in db.paises on d.id_pais equals p.id
                                    where d.estado != null && p.estado == true
                                    select new DepartamentoModel
                                    {
                                        Id = d.id,
                                        IdPais = d.id_pais,
                                        NombreDepartamento = d.nombre_departamento,
                                        Estado = d.estado,
                                        NombrePais = p.nombre_pais,
                                        EstadoDescrip = d.estado == true ? "Activo" : "Inactivo"
                                    };

                listaDepartamentos = departamentos.OrderBy(d => d.NombreDepartamento).ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomDep != "")
                {
                    listaDepartamentos = listaDepartamentos.Where(p => p.NombreDepartamento.ToUpper().Contains(sesNomDep.Trim().ToUpper())).ToList();
                }
                if (sesIdPais != "")
                {
                    int idPais = Convert.ToInt32(sesIdPais);
                    listaDepartamentos = listaDepartamentos.Where(p => p.IdPais == idPais).ToList();
                }
                listaDepartamentos = listaDepartamentos.OrderBy(p => p.NombreDepartamento).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de departamentos";
            }
            return View(listaDepartamentos.ToPagedList(pageIndex, pageSize));
        }

        public JsonResult BuscarDepartamentos(string term)
        {
            using (hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities())
            {
                var resultado = db.departamentos.Where(x => x.nombre_departamento.Contains(term) && x.estado != null)
                        .Select(x => x.nombre_departamento).Take(5).ToList();


                return Json(resultado, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<DepartamentoModel> listaDepartamentos = new List<DepartamentoModel>();
            try
            {
                //OBTENEMOS TODOS LOS DEPARTAMENTOS ACTIVOS DE LA BASE DE DATOS Y ORDENAMOS POR DESCRIPCIÓN
                var departamentos = from d in db.departamentos
                                    join p in db.paises on d.id_pais equals p.id
                                    where d.estado != null && p.estado == true
                                    select new DepartamentoModel
                                    {
                                        Id = d.id,
                                        IdPais = d.id_pais,
                                        NombreDepartamento = d.nombre_departamento,
                                        Estado = d.estado,
                                        NombrePais = p.nombre_pais,
                                        EstadoDescrip = d.estado == true ? "Activo" : "Inactivo"
                                    };
                listaDepartamentos = departamentos.ToList();

                //FILTRAMOS POR NOMBRE DEPARTAMENTO Y PAIS EN BUSQUEDA
                var fcNombreDepartamento = fc["txtDepartamento"];
                if (fcNombreDepartamento != "")
                {
                    string descripcion = Convert.ToString(fcNombreDepartamento);
                    listaDepartamentos = listaDepartamentos.Where(p => p.NombreDepartamento.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }

                var fcIdPais = fc["ddlPaises"];
                if (fcIdPais != "")
                {
                    int idPais = Convert.ToInt32(fcIdPais);
                    listaDepartamentos = listaDepartamentos.Where(p => p.IdPais == idPais).ToList();
                }

                listaDepartamentos.OrderBy(p => p.NombreDepartamento).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtDepartamento = fcNombreDepartamento;
                Session["sesionDepartamentosNombre"] = fcNombreDepartamento;
                ViewBag.ddlPaises = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", fcIdPais);
                Session["sesionDepartamentosIdPais"] = fcIdPais;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar departamentos";
            }
            return View(listaDepartamentos.ToPagedList(pageIndex, pageSize));
        }


        #endregion

        #region Crear Departamento

        [HttpGet]
        [AutorizarUsuario("Departamentos", "Create")]
        public ActionResult Create()
        {
            DepartamentoModel departamento = new DepartamentoModel();
            ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais");
            return View(departamento);
        }

        [HttpPost]
        public ActionResult Create(DepartamentoModel departamentoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN DEPARTAMENTO CON EL MISMO PAIS PARA PODER AGREGAR
                    int cantidad = db.departamentos.Where(d => d.nombre_departamento.ToUpper() == departamentoModelo.NombreDepartamento.ToUpper() && d.id_pais == departamentoModelo.IdPais && d.estado != null).Count();
                    if (cantidad == 0)
                    {
                        departamentos departamento = new departamentos
                        {
                            id_pais = departamentoModelo.IdPais,
                            nombre_departamento = departamentoModelo.NombreDepartamento,
                            estado = true
                        };
                        db.departamentos.Add(departamento);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un departamento registrado con el mismo nombre y país");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el departamento en la base de datos");
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
                ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", departamentoModelo.IdPais);
                db.Dispose();
                return View(departamentoModelo);
            }
        }

        #endregion

        #region Editar Departamento

        [HttpGet]
        [AutorizarUsuario("Departamentos", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DepartamentoModel departamentoEdit = new DepartamentoModel();
            try
            {
                var departamento = db.departamentos.Where(d => d.id == id).FirstOrDefault();
                departamentoEdit.Id = departamento.id;
                departamentoEdit.IdPais = departamento.id_pais;
                departamentoEdit.NombreDepartamento = departamento.nombre_departamento;
                departamentoEdit.Estado = departamento.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = departamento.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", departamentoEdit.IdPais);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(departamentoEdit);
        }


        [HttpPost]
        public ActionResult Edit(DepartamentoModel departamentoModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN DEPARTAMENTO CON EL MISMO PAIS PARA PODER ACTUALIZAR
                    int cantidad = db.departamentos.Where(d => d.nombre_departamento.ToUpper() == departamentoModelo.NombreDepartamento.ToUpper() && d.id_pais == departamentoModelo.IdPais && d.estado != null && d.id != departamentoModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var departamento = db.departamentos.Where(d => d.id == departamentoModelo.Id).FirstOrDefault();
                        departamento.id_pais = departamentoModelo.IdPais;
                        departamento.nombre_departamento = departamentoModelo.NombreDepartamento;
                        bool nuevoEstado = departamentoModelo.EstadoDescrip == "A" ? true : false;
                        departamento.estado = nuevoEstado;
                        db.Entry(departamento).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un departamento registrado con el mismo nombre");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el departamento en la base de datos");
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
                ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", departamentoModelo.IdPais);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", departamentoModelo.EstadoDescrip);
                db.Dispose();
                return View(departamentoModelo);
            }
        }

        #endregion

        #region Eliminar Departamento

        [HttpPost]
        public ActionResult EliminarDepartamento(string departamentoId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Departamentos", "EliminarDepartamento");
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
                                int idDepartamento = Convert.ToInt32(departamentoId);
                                var departamento = context.departamentos.Where(d => d.id == idDepartamento).FirstOrDefault();
                                departamento.estado = null;
                                context.Entry(departamento).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Departamentos") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region AJAX
        [HttpPost]
        public ActionResult ObtenerListadoDepartamentos(string paisId)
        {
            List<departamentos> listaDepartamentos = new List<departamentos>();
            try
            {
                int idPais = Convert.ToInt32(paisId);
                listaDepartamentos = db.departamentos.Where(d => d.id_pais == idPais && d.estado == true).ToList();
            }
            catch (Exception)
            {
            }
            var departamentos = listaDepartamentos.OrderBy(d => d.nombre_departamento).Select(d => "<option value='" + d.id + "'>" + d.nombre_departamento + "</option>");
            return Content(String.Join("", departamentos));
        }

        #endregion
    }
}