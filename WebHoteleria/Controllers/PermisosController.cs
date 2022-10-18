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
    public class PermisosController : Controller
    {
        #region Propiedad

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();



        #endregion

        #region Listado de Permisos

        [HttpGet]
        [AutorizarUsuario("Permisos", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<PerfilModel> listaPerfiles = new List<PerfilModel>();
            try
            {
                //CAPTURAMOS VALORES DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesNombrePerfil = Convert.ToString(Session["sesionPermisosNombrePerfil"]);
                ViewBag.txtNombrePerfil = sesNombrePerfil;

                //OBTENEMOS TODOS LOS PERFILES ACTIVOS DE LA BASE DE DATOS
                var perfiles = db.perfiles.Where(p => p.estado == true).ToList();
                foreach (var item in perfiles) //RECORREMOS CADA PERFIL
                {
                    PerfilModel carga = new PerfilModel();
                    carga.Id = item.id;
                    carga.NombrePerfil = item.perfil;
                    var listaPermisos = db.permisos.Where(p => p.id_perfil == item.id && p.habilitado == true).ToList(); //OBTENEMOS TODOS LOS PERMISOS HABILITADOS RELACIONADOS AL PERFIL
                    carga.CantidadPermisos = listaPermisos.Count; // INGRESAMOS LA CANTIDAD DE PERMISOS QUE TIENE EL PERFIL
                    listaPerfiles.Add(carga);
                }

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesNombrePerfil != "")
                {
                    listaPerfiles = listaPerfiles.Where(p => p.NombrePerfil.ToUpper().Contains(sesNombrePerfil.Trim().ToUpper())).ToList();
                }
                listaPerfiles = listaPerfiles.OrderBy(p => p.NombrePerfil).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de permisos";
            }
            return View(listaPerfiles.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<PerfilModel> listaPerfiles = new List<PerfilModel>();
            try
            {
                //OBTENEMOS TODOS LOS PERFILES ACTIVOS DE LA BASE DE DATOS
                var perfiles = db.perfiles.Where(p => p.estado == true).ToList();
                foreach (var item in perfiles) //RECORREMOS CADA PERFIL
                {
                    PerfilModel carga = new PerfilModel();
                    carga.Id = item.id;
                    carga.NombrePerfil = item.perfil;
                    var listaPermisos = db.permisos.Where(p => p.id_perfil == item.id && p.habilitado == true).ToList(); //OBTENEMOS TODOS LOS PERMISOS HABILITADOS RELACIONADOS AL PERFIL
                    carga.CantidadPermisos = listaPermisos.Count; // INGRESAMOS LA CANTIDAD DE PERMISOS QUE TIENE EL PERFIL
                    listaPerfiles.Add(carga);
                }

                //FILTRAMOS POR NOMBRE PERFIL
                var sesNombrePerfil = fc["txtNombrePerfil"];
                if (sesNombrePerfil != "")
                {
                    string descripcion = Convert.ToString(sesNombrePerfil);
                    listaPerfiles = listaPerfiles.Where(p => p.NombrePerfil.ToUpper().Contains(descripcion.Trim().ToUpper())).ToList();
                }

                listaPerfiles = listaPerfiles.OrderBy(p => p.NombrePerfil).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.txtNombrePerfil = sesNombrePerfil;
                Session["sesionPermisosNombrePerfil"] = sesNombrePerfil;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar permisos";
            }
            return View(listaPerfiles.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Asignar Permisos

        [HttpGet]
        [AutorizarUsuario("Permisos", "Asignar")]
        public ActionResult Asignar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PermisoModel permisoAsignar = new PermisoModel();
            try
            {
                //OBTENEMOS EL PERFIL SELECCIONADO Y POSTERIORMENTE CARGAMOS TODOS LOS PERMISOS RELACIONADOS
                var perfil = db.perfiles.Where(p => p.id == id).FirstOrDefault();
                permisoAsignar.IdPerfil = perfil.id;
                permisoAsignar.NombrePerfil = perfil.perfil;
                ViewBag.IdModulo = new SelectList(db.modulos.Where(m => m.estado == true).OrderBy(m => m.modulo).ToList(), "id", "modulo");
                ViewBag.IdModuloOperacion = new SelectList(db.modulos_operaciones.Where(mo => mo.id == 0).OrderBy(m => m.descripcion).ToList(), "id", "descripcion");
                List<PermisoModel> listaPermisos = new List<PermisoModel>();
                var permisos = db.permisos.Where(p => p.id_perfil == perfil.id && p.habilitado == true).ToList();
                foreach (var item in permisos)
                {
                    PermisoModel carga = new PermisoModel
                    {
                        Id = item.id,
                        IdModuloOperacion = item.id_modulo_operacion,
                        NombreModulo = item.modulos_operaciones.modulos.modulo,
                        NombreModuloOperacion = item.modulos_operaciones.descripcion
                    };
                    listaPermisos.Add(carga);
                }
                listaPermisos = listaPermisos.OrderBy(lp => lp.NombreModulo).ToList();
                ViewBag.ListaPermisos = listaPermisos;
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(permisoAsignar);
        }

        [HttpPost]
        public ActionResult Asignar(PermisoModel modeloPermiso, FormCollection fc)
        {
            try
            {
                var perfil = db.perfiles.Where(p => p.id == modeloPermiso.IdPerfil).FirstOrDefault();

                //LISTADO DE PERMISOS AGREGADOS AL PERFIL
                string[] arrIdPermiso = (fc["arrIdPermiso"] != null ? fc["arrIdPermiso"].Split(',') : new string[] { });
                string[] arrIdOperacion = (fc["arrIdOperacion"] != null ? fc["arrIdOperacion"].Split(',') : new string[] { });

                //LISTADO DE PERMISOS ELIMINADOS AL PERFIL
                string[] arrIdPermisoEliminado = (fc["arrIdPermisoEliminado"] != null ? fc["arrIdPermisoEliminado"].Split(',') : new string[] { });
                string[] arrIdOperacionEliminado = (fc["arrIdOperacionEliminado"] != null ? fc["arrIdOperacionEliminado"].Split(',') : new string[] { });

                //VERIFICAMOS SI VAMOS A AGREGAR O ACTUALIZAR EL LISTADO DE PERMISOS
                int countPermisos = db.permisos.Where(p => p.id_perfil == perfil.id && p.habilitado == true).ToList().Count;
                if (countPermisos == 0) //AGREMOS PERMISOS
                {
                    for (int i = 0; i < arrIdPermiso.Length; i++)
                    {
                        int idModuloOperacion = Convert.ToInt32(arrIdOperacion[i]);
                        permisos permiso = new permisos
                        {
                            id_perfil = perfil.id,
                            id_modulo_operacion = idModuloOperacion,
                            habilitado = true
                        };
                        db.permisos.Add(permiso);
                        db.SaveChanges();
                    }
                }
                else //ACTUALIZAMOS PERMISOS
                {
                    for (int i = 0; i < arrIdPermiso.Length; i++)
                    {
                        int idModuloOperacion = Convert.ToInt32(arrIdOperacion[i]);
                        int idPermiso = Convert.ToInt32(arrIdPermiso[i]);
                        if (idPermiso == 0)
                        {
                            permisos permiso = new permisos
                            {
                                id_perfil = perfil.id,
                                id_modulo_operacion = idModuloOperacion,
                                habilitado = true
                            };
                            db.permisos.Add(permiso);
                            db.SaveChanges();
                        }
                    }

                    for (int i = 0; i < arrIdPermisoEliminado.Length; i++)
                    {
                        int idOperacionEliminado = Convert.ToInt32(arrIdOperacionEliminado[i]);
                        if (idOperacionEliminado != 0)
                        {
                            var permiso = db.permisos.Where(p => p.id_modulo_operacion == idOperacionEliminado && p.id_perfil == modeloPermiso.IdPerfil).FirstOrDefault();
                            db.Entry(permiso).State = System.Data.Entity.EntityState.Deleted;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index");
        }

        #endregion

    }
}