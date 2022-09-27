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
    public class BarriosController : Controller
    {

        /*
               * BARRIOS/LOCALIDAD
               * 
               * El modulo de barrios/localidades registra todos los barrios o localidades de una ciudad. 
               * Está relacionado actualmente con los módulos de clientes, distribuidores, distribuidores sucursales, proveedores, proveedores sucursales, 
               * sucursales, ganadores.
               * 
               * FECHA DOCUMENTACION: 16/02/2022
               */

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();

        #endregion

        #region Listado de Barrios/Localidades

        [HttpGet]
        [AutorizarUsuario("Barrios", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<BarrioModel> listaBarrios = new List<BarrioModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNomBar = Convert.ToString(Session["sesionBarriosNombre"]);
                ViewBag.txtBarrio = sesNomBar;
                string sesIdPais = Convert.ToString(Session["sesionBarriosIdPais"]);
                ViewBag.ddlPaises = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", sesIdPais);
                string sesIdDepartamento = Convert.ToString(Session["sesionBarriosIdDepartamento"]);
                int idPais = sesIdPais != "" ? Convert.ToInt32(sesIdPais) : 0;
                ViewBag.ddlDepartamentos = new SelectList(db.departamentos.Where(d => d.id_pais == idPais && d.estado == true).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento", sesIdDepartamento);
                string sesIdCiudad = Convert.ToString(Session["sesionBarriosIdCiudad"]);
                int idDepartamento = sesIdDepartamento != "" ? Convert.ToInt32(sesIdDepartamento) : 0;
                ViewBag.ddlCiudades = new SelectList(db.ciudades.Where(c => c.id_departamento == idDepartamento && c.estado == true).OrderBy(c => c.nombre_ciudad).ToList(), "id", "nombre_ciudad", sesIdCiudad);
                int idCiudad = sesIdCiudad != "" ? Convert.ToInt32(sesIdCiudad) : 0;

                //OBTENEMOS TODOS LOS BARRIOS NO ELIMINADOS DE LA BASE DE DATOS
                var barrios = from b in db.barrios
                              where b.estado != null
                              select new BarrioModel
                              {
                                  Id = b.id,
                                  IdCiudad = b.id_ciudad,
                                  NombreBarrio = b.nombre_barrio,
                                  Estado = b.estado,
                                  NombreCiudad = b.ciudades.nombre_ciudad,
                                  NombreDepartamento = b.ciudades.departamentos.nombre_departamento,
                                  NombrePais = b.ciudades.departamentos.paises.nombre_pais,
                                  IdDepartamento = b.ciudades.id_departamento,
                                  IdPais = b.ciudades.departamentos.id_pais,
                                  EstadoDescrip = b.estado == true ? "Activo" : "Inactivo"
                              };

                listaBarrios = barrios.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNomBar != "")
                {
                    listaBarrios = listaBarrios.Where(p => p.NombreBarrio.ToUpper().Contains(sesNomBar.Trim().ToUpper())).ToList();
                }
                if (sesIdPais != "")
                {
                    listaBarrios = listaBarrios.Where(p => p.IdPais == idPais).ToList();
                }
                if (sesIdDepartamento != "")
                {
                    listaBarrios = listaBarrios.Where(p => p.IdDepartamento == idDepartamento).ToList();
                }
                if (sesIdCiudad != "")
                {
                    listaBarrios = listaBarrios.Where(p => p.IdCiudad == idCiudad).ToList();
                }
                listaBarrios = listaBarrios.OrderBy(p => p.NombreBarrio).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de barrios";
            }
            return View(listaBarrios.ToPagedList(pageIndex, pageSize));
        }

        public JsonResult BuscarBarrios(string term)
        {
            using (hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities())
            {
                var resultado = db.barrios.Where(x => x.nombre_barrio.Contains(term) && x.estado != null)
                        .Select(x => x.nombre_barrio).Take(5).ToList();


                return Json(resultado, JsonRequestBehavior.AllowGet);

            }
        }


        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<BarrioModel> listaBarrios = new List<BarrioModel>();
            try
            {
                //OBTENEMOS TODOS LOS BARRIOS NO ELIMINADOS DE LA BASE DE DATOS
                var barrios = from b in db.barrios
                              where b.estado != null
                              select new BarrioModel
                              {
                                  Id = b.id,
                                  IdCiudad = b.id_ciudad,
                                  NombreBarrio = b.nombre_barrio,
                                  Estado = b.estado,
                                  NombreCiudad = b.ciudades.nombre_ciudad,
                                  NombreDepartamento = b.ciudades.departamentos.nombre_departamento,
                                  NombrePais = b.ciudades.departamentos.paises.nombre_pais,
                                  IdDepartamento = b.ciudades.id_departamento,
                                  IdPais = b.ciudades.departamentos.id_pais,
                                  EstadoDescrip = b.estado == true ? "Activo" : "Inactivo"
                              };

                listaBarrios = barrios.ToList();

                //FILTRAMOS POR NOMBRE BARRIO CIUDAD DEPARTAMENTO Y PAIS DE BUSQUEDA DEL USUARIO
                var fcNombreBarrio = fc["txtBarrio"];
                if (fcNombreBarrio != "")
                {
                    string descripcion = Convert.ToString(fcNombreBarrio);
                    listaBarrios = listaBarrios.Where(c => c.NombreBarrio.ToUpper().Contains(descripcion.Trim().ToUpper())).ToList();
                }

                var fcIdPais = fc["ddlPaises"];
                int idPais = fcIdPais != "" ? Convert.ToInt32(fcIdPais) : 0;
                if (fcIdPais != "")
                {
                    listaBarrios = listaBarrios.Where(c => c.IdPais == idPais).ToList();
                }

                var fcIdDepartamento = fc["ddlDepartamentos"];
                int idDepartamento = fcIdDepartamento != "" ? Convert.ToInt32(fcIdDepartamento) : 0;
                if (fcIdDepartamento != "")
                {
                    listaBarrios = listaBarrios.Where(c => c.IdDepartamento == idDepartamento).ToList();
                }

                var fcIdCiudad = fc["ddlCiudades"];
                int idCiudad = fcIdCiudad != "" ? Convert.ToInt32(fcIdCiudad) : 0;
                if (fcIdCiudad != "")
                {
                    listaBarrios = listaBarrios.Where(c => c.IdCiudad == idCiudad).ToList();
                }

                listaBarrios.OrderBy(c => c.NombreBarrio).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtBarrio = fcNombreBarrio;
                Session["sesionBarriosNombre"] = fcNombreBarrio;
                ViewBag.ddlPaises = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", fcIdPais);
                Session["sesionBarriosIdPais"] = fcIdPais;
                ViewBag.ddlDepartamentos = new SelectList(db.departamentos.Where(d => d.estado == true && d.id_pais == idPais).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento", fcIdDepartamento);
                Session["sesionBarriosIdDepartamento"] = fcIdDepartamento;
                ViewBag.ddlCiudades = new SelectList(db.ciudades.Where(c => c.estado == true && c.id_departamento == idDepartamento).OrderBy(d => d.nombre_ciudad).ToList(), "id", "nombre_ciudad", fcIdCiudad);
                Session["sesionBarriosIdCiudad"] = fcIdCiudad;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar barrios";
            }
            return View(listaBarrios.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Crear Barrio/Localidad

        [HttpGet]
        [AutorizarUsuario("Barrios", "Create")]
        public ActionResult Create()
        {
            BarrioModel barrio = new BarrioModel();
            //AL AGREGAR UN BARRIO SE PUEDE ELEGIR LA OPCION DE BARRIO O LOCALIDAD (COMPAÑIAS)
            barrio.Barrio = true; //POR DEFECTO SE SELECCIONA BARRIOS
            //CARGAMOS EL LISTADO DE PAISES. DEPARTAMENTOS Y CIUDADES SE FILTRAN DESDE LA VISTA
            ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais");
            ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.id == 0).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento");
            ViewBag.IdCiudad = new SelectList(db.ciudades.Where(c => c.id == 0).OrderBy(c => c.nombre_ciudad).ToList(), "id", "nombre_ciudad");
            return View(barrio);
        }

        [HttpPost]
        public ActionResult Create(BarrioModel barrioModelo, FormCollection fc)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            string areaSeleccionada = fc["Area"]; //CAPTURAMOS LO SELECCIONADO EN BARRIO O LOCALIDAD
            barrioModelo.Localidad = areaSeleccionada == "L" ? true : false;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN BARRIO/LOCALIDAD CON LA MISMA CIUDAD PARA PODER AGREGAR
                    int cantidad = db.barrios.Where(b => b.nombre_barrio.ToUpper() == barrioModelo.NombreBarrio.Trim().ToUpper() && b.id_ciudad == barrioModelo.IdCiudad && b.estado != null).Count();
                    if (cantidad == 0)
                    {
                        //AGREGAMOS EL REGISTRO DE BARRIOS/LOCALIDAD
                        barrios barrio = new barrios
                        {
                            id_ciudad = barrioModelo.IdCiudad,
                            nombre_barrio = barrioModelo.NombreBarrio.Trim(),
                            localidad = barrioModelo.Localidad,
                            estado = true
                        };
                        db.barrios.Add(barrio);
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un barrio registrada con el mismo nombre y ciudad");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el barrio en la base de datos");
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
                CargarDatosBarrio(barrioModelo); //CARGAMOS LOS LISTADOS NECESARIOS
                db.Dispose();
                return View(barrioModelo);
            }
        }

        #endregion

        #region Editar Barrio/Localidad

        [HttpGet]
        [AutorizarUsuario("Barrios", "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BarrioModel barrioEdit = new BarrioModel();
            try
            {
                //OBTENEMOS EL REGISTRO Y CARGAMOS LOS DATOS EN LAS PROPIEDADES DEL MODELO
                var barrio = db.barrios.Where(d => d.id == id).FirstOrDefault();
                barrioEdit.Id = barrio.id;
                barrioEdit.IdPais = barrio.ciudades.departamentos.id_pais;
                barrioEdit.IdDepartamento = barrio.ciudades.id_departamento;
                barrioEdit.IdCiudad = barrio.id_ciudad;
                barrioEdit.NombreBarrio = barrio.nombre_barrio;
                barrioEdit.Localidad = barrio.localidad.Value;
                barrioEdit.Localidad = barrio.localidad.Value;
                barrioEdit.Estado = barrio.estado;

                EstadoRegistro Estado = new EstadoRegistro();
                string strEstado = barrio.estado == true ? "A" : "I";
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", strEstado);
                CargarDatosBarrio(barrioEdit); //CARGAMOS LOS LISTADOS NECESARIOS
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(barrioEdit);
        }

        [HttpPost]
        public ActionResult Edit(BarrioModel barrioModelo, FormCollection fc)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;
            string areaSeleccionada = fc["Area"]; //CAPTURAMOS LO SELECCIONADO EN BARRIO O LOCALIDAD
            barrioModelo.Localidad = areaSeleccionada == "L" ? true : false;
            if (ModelState.IsValid)
            {
                try
                {
                    //VERIFICAMOS SI YA EXISTE UN BARRIO/LOCALIDAD CON LA MISMA CIUDAD PARA PODER AGREGAR
                    int cantidad = db.barrios.Where(b => b.nombre_barrio.ToUpper() == barrioModelo.NombreBarrio.Trim().ToUpper() && b.id_ciudad == barrioModelo.IdCiudad && b.estado != null && b.id != barrioModelo.Id).Count();
                    if (cantidad == 0)
                    {
                        //OBTENEMOS EL REGISTRO PARA PODER ACTUALIZAR BARRIO/LOCALIDAD
                        var barrio = db.barrios.Where(d => d.id == barrioModelo.Id).FirstOrDefault();
                        barrio.id_ciudad = barrioModelo.IdCiudad;
                        barrio.nombre_barrio = barrioModelo.NombreBarrio;
                        barrio.localidad = barrioModelo.Localidad;
                        bool nuevoEstado = barrioModelo.EstadoDescrip == "A" ? true : false;
                        barrio.estado = nuevoEstado;
                        db.Entry(barrio).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Duplicado", "Ya existe un barrio registrada con el mismo nombre y ciudad");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al actualizar el barrio en la base de datos");
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
                CargarDatosBarrio(barrioModelo); //CARGAMOS LOS LISTADOS NECESARIOS
                EstadoRegistro Estado = new EstadoRegistro();
                ViewBag.EstadoDescrip = new SelectList(Estado.ObtenerListadoEstadosRegistros(), "Id", "Descripcion", barrioModelo.EstadoDescrip);
                db.Dispose();
                return View(barrioModelo);
            }
        }

        #endregion

        #region Eliminar Barrio/Localidad

        [HttpPost]
        public ActionResult EliminarBarrio(string barrioId)
        {
            bool retorno = true;
            string respuesta = string.Empty;
            try
            {
                //VERIFICAMOS EL PERMISO Y LA ACCIÓN DEL USUARIO
                AutorizarAccionUsuario autorizarAccion = new AutorizarAccionUsuario("Barrios", "EliminarBarrio");
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
                                int idBarrio = Convert.ToInt32(barrioId);
                                var barrio = context.barrios.Where(b => b.id == idBarrio).FirstOrDefault();
                                barrio.estado = null;
                                context.Entry(barrio).State = System.Data.Entity.EntityState.Modified;
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
            return Json(new { success = retorno, respuesta = respuesta, urlRedirect = Url.Action("Index", "Barrios") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Funciones

        /*
         * METODO QUE RECIBE UN MODELO DE BARRIO/LOCALIDAD Y CARGA LA LISTA DE PAISES DEPARTAMENTOS Y CIUDADES
         * TAMBIEN INDICA LA OPCION SELECCIONADA DEL RADIOBUTTON (SI ES BARRIO O LOCALIDAD)
         */
        private void CargarDatosBarrio(BarrioModel barrioModelo)
        {
            int idPais = barrioModelo.IdPais != null ? barrioModelo.IdPais.Value : 0;
            int idDepartamento = barrioModelo.IdDepartamento != null ? barrioModelo.IdDepartamento.Value : 0;

            ViewBag.IdPais = new SelectList(db.paises.Where(p => p.estado == true).OrderBy(p => p.nombre_pais).ToList(), "id", "nombre_pais", barrioModelo.IdPais);
            ViewBag.IdDepartamento = new SelectList(db.departamentos.Where(d => d.id_pais == idPais).OrderBy(d => d.nombre_departamento).ToList(), "id", "nombre_departamento", barrioModelo.IdDepartamento);
            ViewBag.IdCiudad = new SelectList(db.ciudades.Where(c => c.id_departamento == idDepartamento).OrderBy(c => c.nombre_ciudad).ToList(), "id", "nombre_ciudad", barrioModelo.IdCiudad);

            if (barrioModelo.Localidad == false)
            {
                barrioModelo.Barrio = true;
            }
        }

        #endregion

        #region AJAX

        /*
         * OBTIENE EL LISTADO DE BARRIOS/LOCALIDADES FILTRADO POR CIUDAD
         */
        [HttpPost]
        public ActionResult ObtenerListaBarrios(string ciudadId)
        {
            int idCiudad = ciudadId != "" ? Convert.ToInt32(ciudadId) : 0;
            BarrioModel barrioModelo = new BarrioModel();
            List<ListaDinamica> listaBarrios = barrioModelo.ListadoBarrios(idCiudad);
            var barrios = listaBarrios.OrderBy(b => b.Nombre).Select(b => "<option value='" + b.Id + "'>" + b.Nombre + "</option>");
            return Content(String.Join("", barrios));
        }

        #endregion



    }
}