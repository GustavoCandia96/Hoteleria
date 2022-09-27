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
    public class CiudadesController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();


        #endregion

        #region Listado de Ciudades

        [HttpGet]
        [AutorizarUsuario("Ciudades", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CiudadModel> listaCiudades = new List<CiudadModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomCiu = Convert.ToString(Session["sesionCiudadesNombre"]);
                ViewBag.txtCiudad = sesNomCiu;
                string sesIdPais = Convert.ToString(Session["sesionCiudadesIdPais"]);
                ViewBag.ddlPaises = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", sesIdPais);
                string sesIdDepartamento = Convert.ToString(Session["sesionCiudadesIdDepartamento"]);

                int idPais = sesIdPais != "" ? Convert.ToInt32(sesIdPais) : 0;
                int idDepartamento = sesIdDepartamento != "" ? Convert.ToInt32(sesIdDepartamento) : 0;

                if (sesIdPais != "")
                {
                    ViewBag.ddlDepartamentos = new SelectList(db.departamentos.Where(d => d.id_pais == idPais && d.estado == true).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento", sesIdDepartamento);
                }
                else
                {
                    ViewBag.ddlDepartamentos = new SelectList(db.departamentos.Where(d => d.id == 0).ToList(), "id", "nombre_departamento");
                }

                //OBTENEMOS TODOS LAS CIUDADES DE LOS DEPARTAMENTOS Y PAISES ACTIVOS
                var ciudades = from c in db.ciudades
                               join d in db.departamentos on c.id_departamento equals d.id
                               join p in db.paises on d.id_pais equals p.id
                               where c.estado != null && d.estado == true && p.estado == true
                               select new CiudadModel
                               {
                                   Id = c.id,
                                   IdDepartamento = c.id_departamento,
                                   NombreCiudad = c.nombre_ciudad,
                                   Estado = c.estado,
                                   NombreDepartamento = d.nombre_departamento,
                                   NombrePais = p.nombre_pais,
                                   IdPais = p.id,
                                   EstadoDescrip = c.estado == true ? "Activo" : "Inactivo"
                               };

                listaCiudades = ciudades.OrderBy(c => c.NombreCiudad).ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomCiu != "")
                {
                    listaCiudades = listaCiudades.Where(p => p.NombreCiudad.ToUpper().Contains(sesNomCiu.Trim().ToUpper())).ToList();
                }
                if (sesIdPais != "")
                {
                    listaCiudades = listaCiudades.Where(p => p.IdPais == idPais).ToList();
                }
                if (sesIdDepartamento != "")
                {
                    listaCiudades = listaCiudades.Where(p => p.IdDepartamento == idDepartamento).ToList();
                }
                listaCiudades = listaCiudades.OrderBy(p => p.NombreCiudad).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de ciudades";
            }
            return View(listaCiudades.ToPagedList(pageIndex, pageSize));
        }

        public JsonResult BuscarCiudades(string term)
        {
            using (hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities())
            {
                var resultado = db.ciudades.Where(x => x.nombre_ciudad.Contains(term) && x.estado != null)
                        .Select(x => x.nombre_ciudad).Take(5).ToList();


                return Json(resultado, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<CiudadModel> listaCiudades = new List<CiudadModel>();
            try
            {
                //OBTENEMOS TODOS LAS CIUDADES DE LOS DEPARTAMENTOS Y PAISES ACTIVOS
                var ciudades = from c in db.ciudades
                               join d in db.departamentos on c.id_departamento equals d.id
                               join p in db.paises on d.id_pais equals p.id
                               where c.estado != null && d.estado == true && p.estado == true
                               select new CiudadModel
                               {
                                   Id = c.id,
                                   IdDepartamento = c.id_departamento,
                                   NombreCiudad = c.nombre_ciudad,
                                   Estado = c.estado,
                                   NombreDepartamento = d.nombre_departamento,
                                   NombrePais = p.nombre_pais,
                                   IdPais = p.id,
                                   EstadoDescrip = c.estado == true ? "Activo" : "Inactivo"
                               };

                listaCiudades = ciudades.OrderBy(c => c.NombreCiudad).ToList();

                //FILTRAMOS POR NOMBRE DE LA CIUDAD, DEPARTAMENTO Y PAIS
                var fcNombreCiudad = fc["txtCiudad"];
                if (fcNombreCiudad != "")
                {
                    string descripcion = Convert.ToString(fcNombreCiudad);
                    listaCiudades = listaCiudades.Where(c => c.NombreCiudad.ToUpper().Contains(descripcion.ToUpper())).ToList();
                }

                var fcIdPais = fc["ddlPaises"];
                int idPais = fcIdPais != "" ? Convert.ToInt32(fcIdPais) : 0;
                if (fcIdPais != "")
                {
                    listaCiudades = listaCiudades.Where(c => c.IdPais == idPais).ToList();
                }

                var fcIdDepartamento = fc["ddlDepartamentos"];
                int idDepartamento = fcIdPais != "" ? Convert.ToInt32(fcIdDepartamento) : 0;
                if (fcIdDepartamento != "")
                {
                    listaCiudades = listaCiudades.Where(c => c.IdDepartamento == idDepartamento).ToList();
                }

                listaCiudades = listaCiudades.OrderBy(c => c.NombreCiudad).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtCiudad = fcNombreCiudad;
                Session["sesionCiudadesNombre"] = fcNombreCiudad;
                ViewBag.ddlPaises = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", fcIdPais);
                Session["sesionCiudadesIdPais"] = fcIdPais;
                Session["sesionCiudadesIdDepartamento"] = fcIdDepartamento;
                if (idPais != 0)
                {
                    ViewBag.ddlDepartamentos = new SelectList(db.departamentos.Where(d => d.estado == true && d.id_pais == idPais).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento", fcIdDepartamento);
                }
                else
                {
                    ViewBag.ddlDepartamentos = new SelectList(db.departamentos.Where(d => d.id == 0).ToList(), "id", "nombre_departamento");
                }
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar ciudades";
            }
            return View(listaCiudades.ToPagedList(pageIndex, pageSize));
        }


        #endregion

        #region Crear Ciudad

        [HttpGet]
        [AutorizarUsuario("Ciudades", "Create")]
        public ActionResult Create()
        {
            CiudadModel ciudad = new CiudadModel();
            ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais");
            ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.id == 0).ToList(), "id", "nombre_departamento");
            return View(ciudad);
        }

        [HttpPost]
        public ActionResult Create(CiudadModel ciudadModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA CIUDAD CON EL MISMO DEPARTAMENTO PARA PODER AGREGAR
                    int cantidad = db.ciudades.Where(c => c.nombre_ciudad.ToUpper() == ciudadModelo.NombreCiudad.Trim().ToUpper() && c.id_departamento == ciudadModelo.IdDepartamento && c.estado != null).Count();
                    if (cantidad == 0)
                    {
                        ciudades ciudad = new ciudades
                        {
                            id_departamento = ciudadModelo.IdDepartamento,
                            nombre_ciudad = ciudadModelo.NombreCiudad,
                            estado = true
                        };
                        db.ciudades.Add(ciudad);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una ciudad registrada con el mismo nombre y departamento");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la ciudad en la base de datos");
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
                ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", ciudadModelo.IdPais);
                ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.estado == true && d.id_pais == ciudadModelo.IdPais).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento", ciudadModelo.IdDepartamento);
                db.Dispose();
                return View(ciudadModelo);
            }
        }

        #endregion

        #region Editar Ciudad

        [HttpGet]
        [AutorizarUsuario("Ciudades", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CiudadModel ciudadEdit = new CiudadModel();
            try
            {
                var ciudad = db.ciudades.Where(d => d.id == id).FirstOrDefault();
                ciudadEdit.Id = ciudad.id;
                ciudadEdit.IdPais = ciudad.departamentos.id_pais;
                ciudadEdit.IdDepartamento = ciudad.id_departamento;
                ciudadEdit.NombreCiudad = ciudad.nombre_ciudad;
                ciudadEdit.Estado = ciudad.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = ciudad.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);

                ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", ciudadEdit.IdPais);
                ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.estado == true && d.id_pais == ciudadEdit.IdPais).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento", ciudadEdit.IdDepartamento);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(ciudadEdit);
        }

        [HttpPost]
        public ActionResult Edit(CiudadModel ciudadModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UNA CIUDAD CON EL MISMO DEPARTAMENTO PARA PODER AGREGAR
                    int cantidad = db.ciudades.Where(c => c.nombre_ciudad.ToUpper() == ciudadModelo.NombreCiudad.ToUpper() && c.id_departamento == ciudadModelo.IdDepartamento && c.estado != null && c.id != ciudadModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        var ciudad = db.ciudades.Where(c => c.id == ciudadModelo.Id).FirstOrDefault();
                        ciudad.id_departamento = ciudadModelo.IdDepartamento;
                        ciudad.nombre_ciudad = ciudadModelo.NombreCiudad;
                        bool nuevoEstado = ciudadModelo.EstadoDescrip == "A" ? true : false;
                        ciudad.estado = nuevoEstado;
                        db.Entry(ciudad).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe una ciudad registrado con el mismo nombre y departamento");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar la ciudad en la base de datos");
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
                ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", ciudadModelo.IdPais);
                ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.estado == true && d.id_pais == ciudadModelo.IdPais).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento", ciudadModelo.IdDepartamento);
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", ciudadModelo.EstadoDescrip);
                db.Dispose();
                return View(ciudadModelo);
            }
        }

        #endregion

        #region Eliminar Ciudad

        [HttpPost]
        public ActionResult EliminarCiudad(string ciudadId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Ciudades", "EliminarCiudad");
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
                                int idCiudad = Convert.ToInt32(ciudadId);
                                var ciudad = context.ciudades.Where(c => c.id == idCiudad).FirstOrDefault();
                                ciudad.estado = null;
                                context.Entry(ciudad).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Ciudades") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region AJAX

        [HttpPost]
        public ActionResult ObtenerListadoCiudades(string departamentoId)
        {
            List<ciudades> listaCiudades = new List<ciudades>();
            try
            {
                int idDepartamento = Convert.ToInt32(departamentoId);
                listaCiudades = db.ciudades.Where(d => d.id_departamento == idDepartamento && d.estado == true).ToList();
            }
            catch (Exception)
            {
            }
            var ciudades = listaCiudades.OrderBy(c => c.nombre_ciudad).Select(c => "<option value='" + c.id + "'>" + c.nombre_ciudad + "</option>");
            return Content(String.Join("", ciudades));
        }

        #endregion



    }
}