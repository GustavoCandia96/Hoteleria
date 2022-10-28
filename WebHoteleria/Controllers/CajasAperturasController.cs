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
    public class CajasAperturasController : Controller
    {

        #region Propiedades

        private hoteleria_erp_dbEntities db = new hoteleria_erp_dbEntities();
        private int IdSucursalUsuario;
        private int IdFuncionario;
        private int IdUsuario;

        private decimal SaldoEfectivo = 0;
        private decimal SaldoCaja = 0;
        private decimal SaldoEfectivoInicial = 0;
        private decimal TotalComposicion = 0;
        private decimal TotalConsumiciones = 0;
        private decimal TotalFaltante = 0;
        private decimal TotalSobrante = 0;
        private decimal TotalFaltanteAjuste = 0;
        private decimal TotalSobranteAjuste = 0;
        private decimal TotalDiferencia = 0;

        private List<cajas_arqueos_detalles> listaArqueosDetalles = new List<cajas_arqueos_detalles>();
        private List<cajas_arqueos_ajustes> listaArqueosAjustes = new List<cajas_arqueos_ajustes>();
        private List<cajas_arqueos_composiciones> listaArqueosComposiciones = new List<cajas_arqueos_composiciones>();
        private List<consumisiones> listaConsumisiones = new List<consumisiones>();

        #endregion

        #region Listado de Cajas Aperturas

        [HttpGet]
        [AutorizarUsuario("CajasAperturas", "Index")]
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CajaAperturaModel> listaCajasAperturas = new List<CajaAperturaModel>();
            try
            {
                //OBTENEMOS LOS DATOS DEL USUARIO EN SESION
                CargarDatosUsuarioSesion();

                //CAPTURAMOS EL VALOR DE LA VARIABLE DE SESION PARA EL PAGINADO
                string sesIdSucursal = Convert.ToString(Session["sesionCajasAperturasIdSucursal"]);
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", sesIdSucursal);
                string sesFechaApertura = Convert.ToString(Session["sesionCajasAperturasFechaApertura"]);
                ViewBag.txtFechaApertura = sesFechaApertura;

                //OBTENEMOS TODOS LAS CAJAS APERTURAS NO ELIMINADAS DE LA BASE DE DATOS
                var cajasAperturas = from ca in db.cajas_aperturas
                                     where ca.estado != null
                                     select new CajaAperturaModel
                                     {
                                         Id = ca.id,
                                         IdCaja = ca.id_caja,
                                         IdUsuario = ca.id_usuario,
                                         Fecha = ca.fecha,
                                         FechaApertura = ca.fecha_apertura,
                                         NombreApertura = ca.nombre_apertura,
                                         SaldoEfectivoInicial = ca.saldo_efectivo_inicial,
                                         SaldoCajaAnterior = ca.saldo_caja_anterior,
                                         SaldoEfectivoAnterior = ca.saldo_efectivo_anterior,
                                         FechaCierre = ca.fecha_cierre,
                                         SaldoCajaCierre = ca.caja_abierta == false ? ca.saldo_caja_cierre : ca.cajas.saldo_caja,
                                         SaldoEfectivoCierre = ca.caja_abierta == false ? ca.saldo_efectivo_cierre : ca.cajas.saldo_efectivo,
                                         SaldoFaltanteCierre = ca.saldo_faltante_cierre,
                                         SaldoSobranteCierre = ca.saldo_sobrante_cierre,
                                         CajaAbierta = ca.caja_abierta.Value,
                                         Estado = ca.estado.Value,
                                         IdSucursal = ca.cajas.id_sucursal,
                                         NombreCaja = ca.cajas.denominacion,
                                         NombreUsuario = ca.usuarios.id_funcionario != null ? ca.usuarios.funcionarios.nombre + " " + ca.usuarios.funcionarios.apellido : "",
                                         NombreSucursal = ca.cajas.sucursales.nombre_sucursal,
                                         SaldoEfectivoActual = ca.cajas.saldo_efectivo,
                                         SaldoCajaActual = ca.cajas.saldo_caja,
                                         EstadoDescrip = ca.estado == true ? "Activo" : "Inactivo"
                                     };
                listaCajasAperturas = cajasAperturas.ToList();

                //FILTRAMOS SI EXISTE PAGINACIÓN
                if (sesIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(sesIdSucursal);
                    listaCajasAperturas = listaCajasAperturas.Where(ca => ca.IdSucursal == idSucursal).ToList();
                }
                if (sesFechaApertura != "")
                {
                    DateTime fechaApertura = Convert.ToDateTime(sesFechaApertura);
                    listaCajasAperturas = listaCajasAperturas.Where(ca => ca.Fecha >= fechaApertura).ToList();
                }

                listaCajasAperturas = listaCajasAperturas.OrderBy(ca => ca.Fecha).ToList();
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al cargar el listado de cajas aperturas";
            }
            return View(listaCajasAperturas.ToPagedList(pageIndex, pageSize));
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection fc)
        {
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<CajaAperturaModel> listaCajasAperturas = new List<CajaAperturaModel>();
            try
            {
                //OBTENEMOS LOS DATOS DEL USUARIO EN SESION
                CargarDatosUsuarioSesion();

                //OBTENEMOS TODOS LAS CAJAS APERTURAS NO ELIMINADAS DE LA BASE DE DATOS
                var cajasAperturas = from ca in db.cajas_aperturas
                                     where ca.estado != null
                                     select new CajaAperturaModel
                                     {
                                         Id = ca.id,
                                         IdCaja = ca.id_caja,
                                         IdUsuario = ca.id_usuario,
                                         Fecha = ca.fecha,
                                         FechaApertura = ca.fecha_apertura,
                                         NombreApertura = ca.nombre_apertura,
                                         SaldoEfectivoInicial = ca.saldo_efectivo_inicial,
                                         SaldoCajaAnterior = ca.saldo_caja_anterior,
                                         SaldoEfectivoAnterior = ca.saldo_efectivo_anterior,
                                         FechaCierre = ca.fecha_cierre,
                                         SaldoCajaCierre = ca.caja_abierta == false ? ca.saldo_caja_cierre : ca.cajas.saldo_caja,
                                         SaldoEfectivoCierre = ca.caja_abierta == false ? ca.saldo_efectivo_cierre : ca.cajas.saldo_efectivo,
                                         SaldoFaltanteCierre = ca.saldo_faltante_cierre,
                                         SaldoSobranteCierre = ca.saldo_sobrante_cierre,
                                         CajaAbierta = ca.caja_abierta.Value,
                                         Estado = ca.estado.Value,
                                         IdSucursal = ca.cajas.id_sucursal,
                                         NombreCaja = ca.cajas.denominacion,
                                         NombreUsuario = ca.usuarios.id_funcionario != null ? ca.usuarios.funcionarios.nombre + " " + ca.usuarios.funcionarios.apellido : "",
                                         NombreSucursal = ca.cajas.sucursales.nombre_sucursal,
                                         SaldoEfectivoActual = ca.cajas.saldo_efectivo,
                                         SaldoCajaActual = ca.cajas.saldo_caja,
                                         EstadoDescrip = ca.estado == true ? "Activo" : "Inactivo"
                                     };
                listaCajasAperturas = cajasAperturas.ToList();

                //FILTRAMOS POR SUCURSAL Y FECHA APERTURA LA BUSQUEDA
                var fcIdSucursal = fc["ddlSucursales"];
                if (fcIdSucursal != "")
                {
                    int idSucursal = Convert.ToInt32(fcIdSucursal);
                    listaCajasAperturas = listaCajasAperturas.Where(ca => ca.IdSucursal == idSucursal).ToList();
                }
                var fcFechaApertura = fc["txtFechaApertura"];
                if (fcFechaApertura != "")
                {
                    DateTime fechaApertura = Convert.ToDateTime(fcFechaApertura);
                    listaCajasAperturas = listaCajasAperturas.Where(ca => ca.Fecha >= fechaApertura).ToList();
                }

                listaCajasAperturas = listaCajasAperturas.OrderBy(ca => ca.Fecha).ToList();

                //DEVOLVEMOS EL VALOR CARGADO EN EL BUSCADOR
                ViewBag.ddlSucursales = new SelectList(db.sucursales.Where(s => s.estado == true).OrderBy(s => s.nombre_sucursal).ToList(), "id", "nombre_sucursal", fcIdSucursal);
                Session["sesionCajasAperturasIdSucursal"] = fcIdSucursal;
                ViewBag.txtFechaApertura = fcFechaApertura;
                Session["sesionCajasAperturasFechaApertura"] = fcFechaApertura;
            }
            catch (Exception)
            {
                ViewBag.msg = "Ocurrio un error al buscar cajas aperturas";
            }
            return View(listaCajasAperturas.ToPagedList(pageIndex, pageSize));
        }

        #endregion

        #region Apertura Caja Apertura

        [HttpGet]
        [AutorizarUsuario("CajasAperturas", "Create")]
        public ActionResult Create()
        {
            CajaAperturaModel cajaAperturaModelo = new CajaAperturaModel();

            //OBTENEMOS LOS DATOS DEL USUARIO EN SESION
            CargarDatosUsuarioSesion();

            //SI ES FUNCIONARIO OBTENEMOS LA SUCURSAL
            if (IdSucursalUsuario != 0)
            {
                var sucursal = db.sucursales.Where(s => s.id == IdSucursalUsuario).FirstOrDefault();
                cajaAperturaModelo.NombreSucursal = sucursal.nombre_sucursal;
            }
            cajaAperturaModelo.IdSucursal = IdSucursalUsuario;

            CargarDatosCajaApertura(cajaAperturaModelo);

            return View(cajaAperturaModelo);
        }

        [HttpPost]
        public ActionResult Create(CajaAperturaModel cajaAperturaModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;

            //OBTENEMOS LOS DATOS DEL USUARIO EN SESION
            CargarDatosUsuarioSesion();

            if (ModelState.IsValid) //SI EL MODELO ES VALIDO
            {
                try
                {
                    //VERIFICAR SI LA CAJA ESTA ABIERTA O CERRADA
                    var count = db.cajas.Where(c => c.id == cajaAperturaModelo.IdCaja && c.estado == true && c.abierto == true).Count();
                    if (count == 0)
                    {
                        var caja = db.cajas.Where(c => c.id == cajaAperturaModelo.IdCaja).FirstOrDefault();

                        //INICIA LA TRANSACCION DE AGREGAR CAJA APERTURA Y ACTUALIZACION DE ESTADO DE CAJA
                        using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                        {
                            using (var dbContextTransaction = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    decimal saldoInicial = 0;
                                    if (cajaAperturaModelo.StrSaldoEfectivoInicial != string.Empty)
                                    {
                                        string strSaldoInicial = cajaAperturaModelo.StrSaldoEfectivoInicial;
                                        strSaldoInicial = strSaldoInicial.Replace(".", "");
                                        saldoInicial = Convert.ToDecimal(strSaldoInicial);
                                    }

                                    //AGREGAMOS REGISTRA DE CAJA APERTURA
                                    cajas_aperturas cajaAper = new cajas_aperturas
                                    {
                                        id_caja = cajaAperturaModelo.IdCaja,
                                        id_usuario = IdUsuario,
                                        fecha = DateTime.Now,
                                        fecha_apertura = DateTime.Now,
                                        nombre_apertura = cajaAperturaModelo.NombreApertura,
                                        saldo_efectivo_inicial = saldoInicial,
                                        saldo_caja_anterior = caja.saldo_caja,
                                        saldo_efectivo_anterior = caja.saldo_efectivo,
                                        saldo_caja_cierre = caja.saldo_caja + saldoInicial,
                                        saldo_efectivo_cierre = caja.saldo_efectivo + saldoInicial,
                                        saldo_faltante_cierre = caja.faltante,
                                        saldo_sobrante_cierre = caja.sobrante,
                                        caja_abierta = true,
                                        estado = true
                                    };
                                    context.cajas_aperturas.Add(cajaAper);
                                    context.SaveChanges();

                                    //ACTUALIZAMOS EL ESTADO DE LA CAJA SELECCIONADA
                                    var cajaActualizar = context.cajas.Where(c => c.id == cajaAperturaModelo.IdCaja).FirstOrDefault();
                                    cajaActualizar.abierto = true;
                                    cajaActualizar.saldo_caja += saldoInicial;
                                    cajaActualizar.saldo_efectivo += saldoInicial;
                                    context.Entry(cajaActualizar).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();

                                    dbContextTransaction.Commit();
                                }
                                catch (Exception)
                                {
                                    dbContextTransaction.Rollback();
                                }
                            }
                            context.Database.Connection.Close();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "Para realizar una nueva apertura la caja seleccionada debe estar cerrada");
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar la caja en la base de datos");
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
                CargarDatosCajaApertura(cajaAperturaModelo);
                return View(cajaAperturaModelo);
            }
        }

        #endregion

        #region Composición Caja Apertura

        [HttpGet]
        [AutorizarUsuario("CajasAperturas", "ComposicionCaja")]
        public ActionResult ComposicionCaja(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CajaComposicionModel cajaComposicionModelo = new CajaComposicionModel();
            try
            {
                //OBTENEMOS EL REGISTRO DE LA CAJA APERTURA Y CARGAMOS AL MODELO PARA PODER REALIZAR COMPOSICION DE CAJA
                var cajaApertura = db.cajas_aperturas.Where(ca => ca.id == id).FirstOrDefault();
                string nombreUsuarioCaja = cajaApertura.cajas.usuarios.funcionarios.nombre + " " + cajaApertura.cajas.usuarios.funcionarios.apellido;

                cajaComposicionModelo.IdCajaApertura = id;
                cajaComposicionModelo.NombreSucursal = cajaApertura.cajas.sucursales.nombre_sucursal;
                cajaComposicionModelo.NombreUsuarioCaja = nombreUsuarioCaja;
                cajaComposicionModelo.StrFecha = DateTime.Now.ToShortDateString();
                cajaComposicionModelo.NombreCaja = cajaApertura.cajas.denominacion;
                cajaComposicionModelo.NombreApertura = cajaApertura.nombre_apertura;
                cajaComposicionModelo.StrTotalEfectivo = String.Format("{0:#,##0.##}", cajaApertura.cajas.saldo_efectivo);

                //VERIFICAMOS SI HAY DENOMINACION DE LA MONEDA ASOCIADA A LA CAJA APERTURA
                int cantidadDenominacion = db.denominaciones_monedas.Where(dm => dm.estado == true && dm.id_moneda == cajaApertura.cajas.id_moneda).Count();
                if (cantidadDenominacion > 0)
                {
                    //OBTENEMOS LA LISTA DE DENOMINAIONES DE LA MONEDA
                    var denominacionesMonedas = db.denominaciones_monedas.Where(dm => dm.estado == true && dm.id_moneda == cajaApertura.cajas.id_moneda).OrderBy(dm => dm.orden).ToList();
                    //VERIFICAMOS SI YA SE HIZO O NO DENOMINACION DE MONEDA DE LA CAJA
                    int count = db.cajas_composiciones.Where(cc => cc.id_caja_apertura == cajaApertura.id && cc.estado != null).Count();
                    if (count > 0) //YA SE HIZO COMPOSICION
                    {
                        //OBTENEMOS LA ULTIMA COMPOSICION DE CAJA
                        var cajaComposicion = db.cajas_composiciones.Where(cc => cc.id_caja_apertura == cajaApertura.id && cc.estado != null).OrderByDescending(cc => cc.fecha).FirstOrDefault();
                        var detalleCajaComposicion = db.cajas_composiciones_detalles.Where(ccd => ccd.id_caja_composicion == cajaComposicion.id && ccd.estado != null).ToList();
                        //AGREGAMOS LOS DETALLES DE COMPOSICION CAJA
                        List<CajaComposicionDetalleModel> listaComposicionDetalle = CargarComposicionCajaDetalle(denominacionesMonedas, detalleCajaComposicion);
                        ViewBag.ComposicionCajaDetalle = listaComposicionDetalle;
                    }
                    else //NO SE HIZO AUN COMPOSICION
                    {
                        //AGREGAMOS LOS DETALLES DE COMPOSICION CAJA
                        List<CajaComposicionDetalleModel> listaComposicionDetalle = CargarComposicionCajaDetalle(denominacionesMonedas);
                        ViewBag.ComposicionCajaDetalle = listaComposicionDetalle;
                    }
                }
                else
                {
                    ModelState.AddModelError("Error", "La composición de caja no puede realizarse, no existe las denominaciones para esta moneda");
                    ViewBag.ComposicionCajaDetalle = null;
                }
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(cajaComposicionModelo);
        }

        [HttpPost]
        public ActionResult ComposicionCaja(cajas_composiciones cajaComposicion, List<cajas_composiciones_detalles> cajaComposicionDetalles)
        {
            bool success = true;
            string mensaje = string.Empty;
            try
            {
                //INICIA LA TRANSACCIÓN DE AGREGAR COMPOSICION DE CAJA RECAUDACION
                using (hoteleria_erp_dbEntities context = new hoteleria_erp_dbEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            //VERIFICAMOS SI SE INGRESO O NO COMPOSICION DE EFECTIVO
                            if (cajaComposicion.total_billete != 0 || cajaComposicion.total_moneda != 0)
                            {
                                //OBTENEMOS LOS DATOS DE LA CAJA APERTURA RELACIONADA A LA COMPOSICION
                                var cajaApertura = context.cajas_aperturas.Where(car => car.id == cajaComposicion.id_caja_apertura && car.estado != null).FirstOrDefault();

                                //CARGAMOS LOS DATOS DEL USUARIO EN SESION
                                CargarDatosUsuarioSesion();

                                //ELIMINAMOS (ACTUALIZAMOS ESTADO A NULL) LAS COMPOSICIONES DE CAJAS APERTURAS REALIZADAS ANTERIORMENTE RELACIONADO
                                var listaCompCaj = context.cajas_composiciones.Where(c => c.id_caja_apertura == cajaApertura.id && c.estado != null).ToList();
                                foreach (var item in listaCompCaj)
                                {
                                    var listaCompoCajDet = context.cajas_composiciones_detalles.Where(c => c.id_caja_composicion == item.id && c.estado != null).ToList();
                                    listaCompoCajDet.ForEach(crd => crd.estado = null);
                                    context.SaveChanges();
                                }
                                listaCompCaj.ForEach(crd => crd.estado = null);
                                context.SaveChanges();

                                //AGREGAMOS LA CABECERA DE COMPOSICION DE LA CAJA APERTURA
                                cajas_composiciones cc = new cajas_composiciones
                                {
                                    id_caja = cajaApertura.id_caja,
                                    id_caja_apertura = cajaApertura.id,
                                    id_usuario = IdUsuario,
                                    fecha = DateTime.Now,
                                    total_billete = cajaComposicion.total_billete,
                                    total_moneda = cajaComposicion.total_moneda,
                                    estado = true
                                };
                                context.cajas_composiciones.Add(cc);
                                context.SaveChanges();

                                //AGREGAMOS LOS DETALLES DE COMPOSICION DE LA CAJA APERTURA
                                foreach (var item in cajaComposicionDetalles)
                                {
                                    decimal cantidad = item.cantidad != null ? item.cantidad.Value : 0;
                                    cajas_composiciones_detalles ccd = new cajas_composiciones_detalles
                                    {
                                        id_caja_composicion = cc.id,
                                        id_denominacion_moneda = item.id_denominacion_moneda,
                                        cantidad = cantidad,
                                        monto = item.monto,
                                        estado = true
                                    };
                                    context.cajas_composiciones_detalles.Add(ccd);
                                    context.SaveChanges();
                                }
                                context.SaveChanges();

                                dbContextTransaction.Commit();
                            }
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();
                            success = false;
                            mensaje = "Hubo un error al procesar los datos de la composición de caja apertura";
                        }
                    }
                    context.Database.Connection.Close();
                }
            }
            catch (Exception)
            {
                success = false;
                mensaje = "Hubo un error al guardar las composición de caja apertura";
            }
            return Json(new { success, mensaje, urlRedirect = Url.Action("Index", "CajasAperturas") }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Ajuste Caja Apertura

        [HttpGet]
        [AutorizarUsuario("CajasAperturas", "CajaAjuste")]
        public ActionResult CajaAjuste(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CajaAjusteModel cajaAjusteModelo = new CajaAjusteModel();
            try
            {
                //OBTENEMOS EL REGISTRO DE LA CAJA APERTURA TIPO RECAUDACION Y CARGAMOS AL MODELO PARA PODER REALIZAR EL AJUSTE DE CAJA
                var cajaApertura = db.cajas_aperturas.Where(ca => ca.id == id).FirstOrDefault();
                string nombreUsuarioCaja = cajaApertura.cajas.usuarios.funcionarios.nombre + " " + cajaApertura.cajas.usuarios.funcionarios.apellido;

                cajaAjusteModelo.IdCajaApertura = cajaApertura.id;
                cajaAjusteModelo.NombreSucursal = cajaApertura.cajas.sucursales.nombre_sucursal;
                cajaAjusteModelo.NombreUsuarioCaja = nombreUsuarioCaja;
                cajaAjusteModelo.StrFecha = DateTime.Now.ToShortDateString();
                cajaAjusteModelo.NombreCaja = cajaApertura.cajas.denominacion;
                cajaAjusteModelo.NombreApertura = cajaApertura.nombre_apertura;

                CargarDatosCajaAjuste(cajaAjusteModelo);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(cajaAjusteModelo);
        }

        [HttpPost]
        public ActionResult CajaAjuste(CajaAjusteModel cajaAjusteModelo)
        {
            bool retornoVista = false;
            ViewBag.msg = string.Empty;

            if (ModelState.IsValid) // SI EL MODELO ES VALIDO
            {
                try
                {
                    //OBTENEMOS LOS DATOS DEL USUARIO EN SESION
                    CargarDatosUsuarioSesion();

                    //OBTENEMOS EL REGISTRO DE LA CAJA APERTURA RELACIONADA
                    var cajaApertura = db.cajas_aperturas.Where(car => car.id == cajaAjusteModelo.IdCajaApertura).FirstOrDefault();

                    //VERIFICAMOS SI HAY AJUSTE SOBRANTE O FALTANTE DEPENDIENDO SI SE AGREGO UN AJUSTE SOBRANTE O FALTANTE
                    //SI HAY REGISTRO DE SOBRANTE NO SE PUEDE AGREGAR FALTANTE Y VICEVERSA
                    bool habilitado = true;
                    bool faltante = cajaAjusteModelo.IdTipoAjuste == 1 ? false : true;
                    var count = db.cajas_ajustes.Where(ca => ca.faltante == faltante && ca.id_caja_apertura == cajaApertura.id && ca.estado != null).Count();
                    habilitado = count > 0 ? false : true;

                    if (habilitado == true)
                    {
                        string StrMonto = cajaAjusteModelo.StrMontoAjuste;
                        StrMonto = StrMonto.Replace(".", "");
                        decimal montoAjuste = Convert.ToDecimal(StrMonto);

                        //AGREGAMOS EL REGISTRO DE CAJA AJUSTE
                        cajas_ajustes cajaAjuste = new cajas_ajustes
                        {
                            id_caja = cajaApertura.cajas.id,
                            id_caja_apertura = cajaApertura.id,
                            id_usuario = IdUsuario,
                            fecha = DateTime.Now,
                            faltante = cajaAjusteModelo.IdTipoAjuste == 1 ? true : false,
                            monto_ajuste = montoAjuste,
                            justificacion = cajaAjusteModelo.Justificacion,
                            estado = true
                        };
                        db.cajas_ajustes.Add(cajaAjuste);
                        db.SaveChanges();
                    }
                    else
                    {
                        string descripcion = faltante == true ? "faltante" : "sobrante";
                        ModelState.AddModelError("Error", "Ya existe ajuste de tipo " + descripcion + ". Solo podras agregar ajuste de tipo " + descripcion);
                        retornoVista = true;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Ocurrio un error al agregar el ajuste de caja");
                    retornoVista = true;
                }
                finally
                {
                    db.Dispose();
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
                CargarDatosCajaAjuste(cajaAjusteModelo);
                return View(cajaAjusteModelo);
            }
        }

        #endregion

        #region Arqueo Parcial Caja Apertura

        [HttpGet]
        [AutorizarUsuario("CajasAperturas", "ArqueoCajaParcial")]
        public ActionResult ArqueoCajaParcial(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CajaAperturaArqueoModel cajAperArqueoModelo = new CajaAperturaArqueoModel();
            try
            {
                cajAperArqueoModelo.IdCajaApertura = id;
                cajAperArqueoModelo.ArqueoFinal = false;
                var cajaApertura = db.cajas_aperturas.Where(ca => ca.id == id).FirstOrDefault();
                cajAperArqueoModelo.StrFechaApertura = cajaApertura.fecha_apertura.Value.ToLongDateString();
                cajAperArqueoModelo.Responsable = cajaApertura.id_usuario != null ? (cajaApertura.usuarios.id_funcionario != null ? cajaApertura.usuarios.funcionarios.nombre + " " + cajaApertura.usuarios.funcionarios.apellido : "") : "";
                cajAperArqueoModelo.NombreCajaApertura = cajaApertura.nombre_apertura;
                cajAperArqueoModelo.StrSaldoAnterior = String.Format("{0:#,##0.##}", cajaApertura.saldo_efectivo_anterior + cajaApertura.saldo_caja_anterior);
                cajAperArqueoModelo.StrSaldoEfectivo = String.Format("{0:#,##0.##}", cajaApertura.cajas.saldo_efectivo);
                cajAperArqueoModelo.StrSaldoCaja = String.Format("{0:#,##0.##}", cajaApertura.cajas.saldo_caja);
                cajAperArqueoModelo.StrSaldoEfectivoInicial = String.Format("{0:#,##0.##}", cajaApertura.saldo_efectivo_inicial);

                CargarDatosArqueoCajaApertura(cajAperArqueoModelo);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            return View(cajAperArqueoModelo);
        }

        //[HttpPost]
        //public ActionResult ArqueoCajaParcial(CajaRecaudacionArqueoModel cajaRecArqueoModelo)
        //{
        //    bool retornoVista = false;
        //    ViewBag.msg = string.Empty;

        //    if (ModelState.IsValid) // SI EL MODELO ES VALIDO
        //    {
        //        try
        //        {
        //            //OBTENEMOS LOS DATOS DEL USUARIO EN SESION
        //            CargarDatosUsuarioSesion();

        //            CargarDatosArqueoCajaRecaudacion(cajaRecArqueoModelo);

        //            //INICIA LA TRANSACCION DE CARGA DE DATOS PARA IMPRIMIR EL ARQUEO
        //            using (gambling_erp_dbEntities context = new gambling_erp_dbEntities())
        //            {
        //                using (var dbContextTransaction = context.Database.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        //CARGAMOS LOS OBJETOS CON DATOS QUE SERAN EL ARQUEO PARCIAL
        //                        //CARGAMOS LOS DATOS DE CABECERA DE ARQUEO TIPO RECAUDACIÓN
        //                        cajas_arqueos cajaArqueo = new cajas_arqueos
        //                        {
        //                            id_usuario = IdUsuario,
        //                            fecha_alta = DateTime.Now,
        //                            id_caja_apertura_recaudacion = cajaRecArqueoModelo.IdCajaRecaudacion,
        //                            total_ingreso = TotalIngreso,
        //                            total_egreso = TotalEgreso,
        //                            total_composicion = TotalComposicion,
        //                            saldo_efectivo = SaldoEfectivo,
        //                            saldo_caja = SaldoCaja,
        //                            faltante = TotalFaltante,
        //                            sobrante = TotalSobrante,
        //                            total_ajuste_faltante = TotalFaltanteAjuste,
        //                            total_ajuste_sobrante = TotalSobranteAjuste,
        //                            diferencia = TotalDiferencia,
        //                            observacion = cajaRecArqueoModelo.Observacion,
        //                            arqueo_final = false,
        //                            estado = true
        //                        };
        //                        context.cajas_arqueos.Add(cajaArqueo);
        //                        context.SaveChanges();

        //                        //AGREGAMOS LOS DATOS DE ARQUEOS DETALLES RECAUDACION
        //                        foreach (var item in listaArqueosDetalles)
        //                        {
        //                            cajas_arqueos_detalles arqDet = new cajas_arqueos_detalles
        //                            {
        //                                id_caja_arqueo = cajaArqueo.id,
        //                                concepto = item.concepto,
        //                                ingreso = item.ingreso,
        //                                egreso = item.egreso,
        //                                saldo = item.saldo,
        //                                estado = true
        //                            };
        //                            context.cajas_arqueos_detalles.Add(arqDet);
        //                            context.SaveChanges();
        //                        }

        //                        //AGREGAMOS LOS DATOS DE ARQUEOS AJUSTES RECAUDACIÓN
        //                        foreach (var item in listaArqueosAjustes)
        //                        {
        //                            cajas_arqueos_ajustes cajArqAjust = new cajas_arqueos_ajustes
        //                            {
        //                                id_caja_arqueo = cajaArqueo.id,
        //                                tipo_ajuste = item.tipo_ajuste,
        //                                justificacion = item.justificacion,
        //                                monto = item.monto,
        //                                estado = true
        //                            };
        //                            context.cajas_arqueos_ajustes.Add(cajArqAjust);
        //                            context.SaveChanges();
        //                        }

        //                        //AGREGAMOS LOS DATOS DE ARQUEOS COMPOSICIONES RECAUDACIÓN
        //                        foreach (var item in listaArqueosComposiciones)
        //                        {
        //                            cajas_arqueos_composiciones cajArqCom = new cajas_arqueos_composiciones
        //                            {
        //                                id_caja_arqueo = cajaArqueo.id,
        //                                cantidad = item.cantidad,
        //                                monto = item.monto,
        //                                sub_total = item.sub_total,
        //                                id_tipo_denominacion = item.id_tipo_denominacion,
        //                                estado = true
        //                            };
        //                            context.cajas_arqueos_composiciones.Add(cajArqCom);
        //                            context.SaveChanges();
        //                        }

        //                        //AGREGAMOS LOS DATOS DE ARQUEOS DE VENTAS DE CARTONES RECAUDACIÓN
        //                        foreach (var item in listaArqueosVentasCartones)
        //                        {
        //                            cajas_arqueos_ventas_cartones cajArqVenCart = new cajas_arqueos_ventas_cartones
        //                            {
        //                                id_caja_arqueo = cajaArqueo.id,
        //                                fecha = item.fecha,
        //                                nombre_tipo_juego = item.nombre_tipo_juego,
        //                                nombre_sorteo = item.nombre_sorteo,
        //                                nro_factura = item.nro_factura,
        //                                ruc = item.ruc,
        //                                razon_social = item.razon_social,
        //                                total_cartones = item.total_cartones,
        //                                total_neto = item.total_neto,
        //                                descuento = item.descuento,
        //                                estado = true
        //                            };
        //                            context.cajas_arqueos_ventas_cartones.Add(cajArqVenCart);
        //                            context.SaveChanges();
        //                        }

        //                        if (CajaMostrador == true) //CAJA MOSTRADOR
        //                        {

        //                        }
        //                        else //CAJA PRINCIPAL
        //                        {

        //                        }

        //                        dbContextTransaction.Commit();

        //                        ImprimirCajaArqueo(cajaArqueo.id);
        //                    }
        //                    catch (Exception)
        //                    {
        //                        dbContextTransaction.Rollback();
        //                    }
        //                }
        //                context.Database.Connection.Close();
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            ModelState.AddModelError("Error", "Ocurrio un error al agregar el arqueo parcial de caja recaudación");
        //            retornoVista = true;
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.msg = null;
        //        retornoVista = true;
        //    }
        //    if (retornoVista == false)
        //    {
        //        return RedirectToAction("ReporteArqueoParcial");
        //    }
        //    else
        //    {
        //        CargarDatosArqueoCajaRecaudacion(cajaRecArqueoModelo);
        //        return View(cajaRecArqueoModelo);
        //    }
        //}

        #endregion

        #region Funciones

        /*
        * CARGAMOS LOS DATOS SI ES FUNCIONARIO O NO Y TAMBIEN LA SUCURSAL DONDE CORRESPONDE EL USUARIO SESION
        */
        private void CargarDatosUsuarioSesion()
        {
            IdUsuario = Convert.ToInt32(Session["IdUser"]);
            var usuario = db.usuarios.Where(u => u.id == IdUsuario).FirstOrDefault();
            ViewBag.IdUsuarioLogueado = IdUsuario;
            IdFuncionario = usuario.id_funcionario != null ? usuario.funcionarios.id : 0;
            ViewBag.EsFuncionario = usuario.id_funcionario != null ? true : false;
            ViewBag.IdSucursalUsuarioLog = usuario.id_funcionario != null ? usuario.funcionarios.id_sucursal : null;
            IdSucursalUsuario = usuario.id_funcionario != null ? usuario.funcionarios.id_sucursal.Value : 0;
        }

        /*
        * CARGAMOS LOS DATOS NECESARIO PARA LA APERTURA DE CAJA APERTURA PARA DEVOLVER A LA VISTA
        */
        private void CargarDatosCajaApertura(CajaAperturaModel cajaAperturaModelo)
        {
            //OBTENEMOS EL LISTADO DE CAJAS RELACIONADOS AL USUARIO LOGUEADO
            ViewBag.IdCaja = new SelectList(db.cajas.Where(c => c.estado == true && c.id_usuario == IdUsuario).OrderBy(c => c.denominacion).ToList(), "id", "denominacion", cajaAperturaModelo.IdCaja);
        }

        /*
         * CARGAMOS LOS DETALLES DE COMPOSICION NO REALIZADA AUN Y CARGADA SOLO DE LA BASE DEATOS LAS DENOMINACIONES
         */
        private List<CajaComposicionDetalleModel> CargarComposicionCajaDetalle(List<denominaciones_monedas> listaDenominaciones)
        {
            decimal totalBillete = 0;
            decimal totalMoneda = 0;
            List<CajaComposicionDetalleModel> retorno = new List<CajaComposicionDetalleModel>();
            foreach (var item in listaDenominaciones)
            {
                CajaComposicionDetalleModel carga = new CajaComposicionDetalleModel
                {
                    IdDenominacionMoneda = item.id.ToString(),
                    IdTipoDenominacion = item.id_tipo_denominacion.ToString(),
                    Cantidad = "",
                    Monto = item.denominacion,
                    SubTotal = "0"
                };
                retorno.Add(carga);
            }
            ViewBag.TotalBilletes = String.Format("{0:#,##0.##}", totalBillete);
            ViewBag.TotalMonedas = String.Format("{0:#,##0.##}", totalMoneda);
            ViewBag.MontoTotalComposicion = String.Format("{0:#,##0.##}", totalBillete + totalMoneda);
            return retorno;
        }

        /*
         * CARGAMOS LOS DETALLES DE COMPOSICION YA REALIZADA Y LA COMPOSICION QUE EXISTE EN LA BASE DE DATOS
         */
        private List<CajaComposicionDetalleModel> CargarComposicionCajaDetalle(List<denominaciones_monedas> listaDenominaciones, List<cajas_composiciones_detalles> listaComposicionesDetalles)
        {
            decimal totalBillete = 0;
            decimal totalMoneda = 0;
            List<CajaComposicionDetalleModel> retorno = new List<CajaComposicionDetalleModel>();
            foreach (var item in listaDenominaciones)
            {
                string cantidad = "";
                string subTotal = "0";
                foreach (var item2 in listaComposicionesDetalles)
                {
                    if (item.id == item2.id_denominacion_moneda)
                    {
                        string strDenominacion = item.denominacion;
                        strDenominacion = strDenominacion.Replace(".", "");
                        decimal valor = Convert.ToDecimal(strDenominacion);
                        cantidad = String.Format("{0:#,##0.##}", item2.cantidad);
                        subTotal = String.Format("{0:#,##0.##}", item2.cantidad * valor);

                        if (item.id_tipo_denominacion == 1)
                        {
                            totalBillete += item2.cantidad.Value * valor;
                        }
                        else
                        {
                            totalMoneda += item2.cantidad.Value * valor;
                        }
                    }
                }

                CajaComposicionDetalleModel carga = new CajaComposicionDetalleModel
                {
                    IdDenominacionMoneda = item.id.ToString(),
                    IdTipoDenominacion = item.id_tipo_denominacion.ToString(),
                    Cantidad = cantidad != "0" ? cantidad : "",
                    Monto = item.denominacion,
                    SubTotal = subTotal
                };
                retorno.Add(carga);
            }
            ViewBag.TotalBilletes = String.Format("{0:#,##0.##}", totalBillete);
            ViewBag.TotalMonedas = String.Format("{0:#,##0.##}", totalMoneda);
            ViewBag.MontoTotalComposicion = String.Format("{0:#,##0.##}", totalBillete + totalMoneda);
            return retorno;
        }

        /*
         * FUNCION QUE GARGA LOS DATOS DE CAJA AJUSTE PARA JUSTIFICAR EL SOBRANTE O EL FALTANTE DE EFECTIVO
         */
        private void CargarDatosCajaAjuste(CajaAjusteModel cajaAjusteModelo)
        {
            CajaAjusteModel cajaAjuste = new CajaAjusteModel();
            var listaTipoAjuste = cajaAjuste.ListadoTiposAjustesCaja();
            ViewBag.IdTipoAjuste = new SelectList(listaTipoAjuste, "Id", "Descripcion", cajaAjusteModelo.IdTipoAjuste);
        }

        /*
         * FUNCION QUE OBTIENE TODAS LAS TRANSACCIONES QUE REALIZO HASTA EL MOMENTO LA CAJA APERTURA SELECCIONADA
         */
        private void CargarDatosArqueoCajaApertura(CajaAperturaArqueoModel cajAperArqueoModelo)
        {
            //OBTENEMOS LOS DATOS DE CAJA APERTURA
            var cajaApertura = db.cajas_aperturas.Where(ca => ca.id == cajAperArqueoModelo.IdCajaApertura).FirstOrDefault();
            SaldoEfectivo = cajaApertura.cajas.saldo_efectivo.Value;
            SaldoCaja = cajaApertura.cajas.saldo_caja.Value;
            SaldoEfectivoInicial = cajaApertura.saldo_efectivo_inicial.Value;

            #region Sub Region de Composicion de Monedas
            //OBTENEMOS LA COMPOSICION DE LA CAJA APERTURA PARA MOSTRAR EN EL ARQUEO
            var cajaComposicion = db.cajas_composiciones.Where(cc => cc.id_caja_apertura == cajaApertura.id && cc.estado != null).OrderByDescending(cc => cc.fecha).FirstOrDefault();
            //OBTENEMOS EL LISTADO DE DETALLE DE COMPOSICION SI ES QUE EXISTE UNA COMPOSICION
            List<cajas_composiciones_detalles> cajaComposicionDet = new List<cajas_composiciones_detalles>();
            if (cajaComposicion != null)
            {
                cajaComposicionDet = db.cajas_composiciones_detalles.Where(ccd => ccd.id_caja_composicion == cajaComposicion.id && ccd.estado != null).ToList();
            }

            //OBTENEMOS LAS DENOMINACIONES QUE TENEMOS EN LA BASE DE DATOS HABILITADAS
            var denominacionesMonedas = db.denominaciones_monedas.Where(dm => dm.estado == true && dm.id_moneda == cajaApertura.cajas.id_moneda).OrderBy(dm => dm.orden).ToList();

            //OBTENEMOS EL TOTAL DEL BILLETE Y EL TOTAL DE MONEDA SI EXISTE COMPOSICION
            decimal totalBillete = cajaComposicion != null ? cajaComposicion.total_billete.Value : 0;
            decimal totalMoneda = cajaComposicion != null ? cajaComposicion.total_moneda.Value : 0;

            //CARGAMOS A LA LISTA LAS COMPOSICIONES REALIZADAS POR EL CAJERO
            List<CajaComposicionDetalleModel> listaComposicionCaja = new List<CajaComposicionDetalleModel>();
            foreach (var item in denominacionesMonedas)
            {
                string strCantidad = "0";
                string strSubTotal = "0";

                if (cajaComposicion != null)
                {
                    var fila = cajaComposicionDet.Where(ccd => ccd.id_denominacion_moneda == item.id).FirstOrDefault();
                    if (fila != null)
                    {
                        strCantidad = String.Format("{0:#,##0.##}", fila.cantidad.Value);
                        strSubTotal = String.Format("{0:#,##0.##}", fila.monto.Value);
                    }
                }

                CajaComposicionDetalleModel carga = new CajaComposicionDetalleModel
                {
                    IdDenominacionMoneda = item.id.ToString(),
                    IdTipoDenominacion = item.id_tipo_denominacion.ToString(),
                    Cantidad = strCantidad,
                    Monto = item.denominacion,
                    SubTotal = strSubTotal
                };
                listaComposicionCaja.Add(carga);
            }
            ViewBag.TotalBilletes = String.Format("{0:#,##0.##}", totalBillete);
            ViewBag.TotalMonedas = String.Format("{0:#,##0.##}", totalMoneda);
            ViewBag.MontoTotalComposicion = String.Format("{0:#,##0.##}", totalBillete + totalMoneda);
            ViewBag.DatosComposicionDetalle = listaComposicionCaja.Count > 0 ? listaComposicionCaja : null;
            TotalComposicion = totalBillete + totalMoneda;

            listaArqueosComposiciones.Clear();
            foreach (var item in listaComposicionCaja)
            {
                string strCantidad = item.Cantidad;
                strCantidad = strCantidad.Replace(".", "");
                decimal cantidad = Convert.ToDecimal(strCantidad);

                string strMonto = item.Monto;
                strMonto = strMonto.Replace(".", "");
                decimal monto = Convert.ToDecimal(strMonto);

                string strSubTotal = item.SubTotal;
                strSubTotal = strSubTotal.Replace(".", "");
                decimal subTotal = Convert.ToDecimal(strSubTotal);

                int idTipoDenominacion = Convert.ToInt32(item.IdTipoDenominacion);

                cajas_arqueos_composiciones cac = new cajas_arqueos_composiciones
                {
                    cantidad = cantidad,
                    monto = monto,
                    sub_total = subTotal,
                    id_tipo_denominacion = idTipoDenominacion
                };
                listaArqueosComposiciones.Add(cac);
            }
            #endregion

            #region Listado de ajustes cajas
            decimal totalAjusteFaltante = 0;
            decimal totalAjusteSobrante = 0;
            List<CajaAjusteModel> listaCajaAjustes = new List<CajaAjusteModel>();
            var listadoCajAju = db.cajas_ajustes.Where(ca => ca.id_caja_apertura == cajaApertura.id && ca.estado != null).ToList();
            foreach (var item in listadoCajAju)
            {
                CajaAjusteModel carga = new CajaAjusteModel
                {
                    Faltante = item.faltante.Value,
                    NombreTipoAjuste = item.faltante == true ? "Faltante" : "Sobrante",
                    Justificacion = item.justificacion,
                    StrMontoAjuste = String.Format("{0:#,##0.##}", item.monto_ajuste),
                    MontoAjuste = item.monto_ajuste
                };
                listaCajaAjustes.Add(carga);
            }
            ViewBag.ListaAjustesCaja = listaCajaAjustes.Count > 0 ? listaCajaAjustes : null;

            totalAjusteFaltante = listaCajaAjustes.Where(lca => lca.Faltante == true).Sum(lca => lca.MontoAjuste.Value);
            totalAjusteSobrante = listaCajaAjustes.Where(lca => lca.Faltante == false).Sum(lca => lca.MontoAjuste.Value);

            listaArqueosAjustes.Clear();
            foreach (var item in listaCajaAjustes)
            {
                cajas_arqueos_ajustes caa = new cajas_arqueos_ajustes
                {
                    tipo_ajuste = item.NombreTipoAjuste,
                    justificacion = item.Justificacion,
                    monto = item.MontoAjuste
                };
                listaArqueosAjustes.Add(caa);
            }
            #endregion

            #region Ventas en Restaurante

            string strFechaHoy = DateTime.Now.ToString();
            DateTime fechaHoy = Convert.ToDateTime(strFechaHoy);

            List<ConsumicionModel> listaConsumisionesRestaurante = new List<ConsumicionModel>();
            var listaConsumiciones = db.consumisiones.Where(f => f.estado != null && f.fecha_alta >= cajaApertura.fecha_apertura).ToList();

            foreach (var item in listaConsumiciones)
            {
                ConsumicionModel carga = new ConsumicionModel
                {
                    StrFechaAlta = item.fecha_alta != null ? item.fecha_alta.ToString() : "-",
                    NroMeza = item.id_mesa != null ? item.mesas.denominacion : "-",
                    NombreMozo = item.id_mozo != null ? item.mozos.nombre + " " + item.mozos.apellido : "-",
                    NroDocumentoCliente = item.id_cliente != null ? item.clientes.nro_documento : "-",
                    NombreCliente = item.id_cliente != null ? (item.clientes.id_tipo_documento == 1 ? item.clientes.nombre + " " + item.clientes.apellido : item.clientes.razon_social) : "-",
                    StrTotalConsumicion = String.Format("{0:#,##0.##}", item.total_consumision)
                };
                listaConsumisionesRestaurante.Add(carga);
            }
            ViewBag.ListaConsumicionesRestaurante = listaConsumisionesRestaurante.Count > 0 ? listaConsumisionesRestaurante : null;
            TotalConsumiciones = listaConsumiciones.Count > 0 ? listaConsumiciones.Sum(s => s.total_consumision.Value) : 0;

            listaConsumisiones.Clear();
            foreach (var item in listaConsumiciones)
            {
                listaConsumisiones.Add(item);
            }
            #endregion

            #region Resumen Consumiciones Restaurante
            List<CajaAperturaArqueoResumenModel> listaResumen = new List<CajaAperturaArqueoResumenModel>();

            listaResumen.Add(new CajaAperturaArqueoResumenModel()
            {
                Descripcion = "Total Consumisión Restaurante",
                Monto = String.Format("{0:#,##0.##}", TotalConsumiciones)
            });

            listaResumen.Add(new CajaAperturaArqueoResumenModel()
            {
                Descripcion = "Efectivo Caja",
                Monto = String.Format("{0:#,##0.##}", cajaApertura.cajas.saldo_efectivo + cajaApertura.saldo_efectivo_inicial)
            });

            listaResumen.Add(new CajaAperturaArqueoResumenModel()
            {
                Descripcion = "Total Composición",
                Monto = String.Format("{0:#,##0.##}", totalBillete + totalMoneda)
            });

            decimal sobrante = 0;
            decimal faltante = 0;

            decimal resultado = cajaApertura.cajas.saldo_efectivo.Value + cajaApertura.saldo_efectivo_inicial.Value - (totalBillete + totalMoneda);
            if (resultado > 0)
            {
                faltante = Math.Abs(resultado);
            }
            if (resultado < 0)
            {
                sobrante = Math.Abs(resultado);
            }

            listaResumen.Add(new CajaAperturaArqueoResumenModel()
            {
                Descripcion = "Faltante",
                Monto = String.Format("{0:#,##0.##}", faltante)
            });

            listaResumen.Add(new CajaAperturaArqueoResumenModel()
            {
                Descripcion = "Ajuste Faltante",
                Monto = String.Format("{0:#,##0.##}", totalAjusteFaltante)
            });

            listaResumen.Add(new CajaAperturaArqueoResumenModel()
            {
                Descripcion = "Sobrante",
                Monto = String.Format("{0:#,##0.##}", sobrante)
            });

            listaResumen.Add(new CajaAperturaArqueoResumenModel()
            {
                Descripcion = "Ajuste Sobrante",
                Monto = String.Format("{0:#,##0.##}", totalAjusteSobrante)
            });

            //CALCULAMOS LA DIFERENCIA SI HAY FALTANTE O SOBRANTE
            decimal diferencia = 0;
            if (faltante > 0)
            {
                diferencia = faltante - totalAjusteFaltante;
            }
            if (sobrante > 0)
            {
                diferencia = sobrante - totalAjusteSobrante;
            }

            listaResumen.Add(new CajaAperturaArqueoResumenModel()
            {
                Descripcion = "Diferencia",
                Monto = String.Format("{0:#,##0.##}", diferencia)
            });

            ViewBag.ResumenCaja = listaResumen.Count > 0 ? listaResumen : null;
            TotalFaltante = faltante;
            TotalSobrante = sobrante;
            TotalFaltanteAjuste = totalAjusteFaltante;
            TotalSobranteAjuste = totalAjusteSobrante;
            TotalDiferencia = diferencia;

            #endregion

            #region Detalle Arqueo Resumen
            List<ArqueoDetalleResumenModel> listaDetalleResumen = new List<ArqueoDetalleResumenModel>();

            listaDetalleResumen.Add(new ArqueoDetalleResumenModel()
            {
                Id = "1",
                Concepto = "Saldo Anterior",
                Ingreso = cajaApertura.saldo_caja_anterior,
                StrIngreso = String.Format("{0:#,##0.##}", cajaApertura.saldo_caja_anterior),
                Egreso = 0,
                StrEgreso = String.Format("{0:#,##0.##}", 0),
                Saldo = cajaApertura.saldo_caja_anterior,
                StrSaldo = String.Format("{0:#,##0.##}", cajaApertura.saldo_caja_anterior),
            });

            listaDetalleResumen.Add(new ArqueoDetalleResumenModel()
            {
                Id = "2",
                Concepto = "Ingreso Consumisiones",
                Ingreso = TotalConsumiciones,
                StrIngreso = String.Format("{0:#,##0.##}", TotalConsumiciones),
                Egreso = 0,
                StrEgreso = String.Format("{0:#,##0.##}", 0),
                Saldo = cajaApertura.saldo_caja_anterior + TotalConsumiciones,
                StrSaldo = String.Format("{0:#,##0.##}", cajaApertura.saldo_caja_anterior + TotalConsumiciones),
            });

            ViewBag.ArqueoDetalleResumen = listaDetalleResumen.Count > 0 ? listaDetalleResumen : null;

            listaArqueosDetalles.Clear();
            foreach (var item in listaDetalleResumen)
            {
                cajas_arqueos_detalles cad = new cajas_arqueos_detalles
                {
                    concepto = item.Concepto,
                    ingreso = item.Ingreso,
                    egreso = item.Egreso,
                    saldo = item.Saldo
                };
                listaArqueosDetalles.Add(cad);
            }
            #endregion
        }

        #endregion

        #region AJAX

        /*
         * OBTIENE EL HISTORICO DE CAJAS APERTURAS PARA PODER VOLVER A IMPRIMIR ARQUEOS
         */
        //[HttpPost]
        //public ActionResult ObtenerListadoHistoricoArqueos(string idCajaAperturaRecaudacion)
        //{
        //    bool control = true;
        //    string msg = string.Empty;
        //    List<cajas_arqueos> listado = new List<cajas_arqueos>();
        //    var tabla = string.Empty;
        //    int id = Convert.ToInt32(idCajaAperturaRecaudacion);
        //    try
        //    {
        //        listado = (from ca in db.cajas_arqueos
        //                   where ca.id_caja_apertura_recaudacion == id && ca.estado == true
        //                   select ca).ToList();

        //        var tbl = "tbl_" + id;
        //        var row = "";
        //        if (listado.Count > 0)
        //        {
        //            foreach (var item in listado)
        //            {
        //                string tipoArqueo = item.arqueo_final.Value == true ? "Arqueo Final" : "Arqueo Parcial";

        //                row += "                   <tr>";
        //                row += "                       <td>" + item.fecha_alta.Value + "</td>";
        //                row += "                       <td>" + tipoArqueo + "</td>    ";
        //                row += "                       <td><a href='/CajasRecaudaciones/ReimprimirArqueoRecaudacion/" + item.id + "' title='Reimprimir'><span class='glyphicon glyphicon-print'></span></a></td>    ";
        //                row += "                   </tr> ";
        //            }
        //        }
        //        else
        //        {
        //            row += "                   <tr>";
        //            row += "                       <td> No se encuentran ningún arqueo realizado.</td>";
        //            row += "                   </tr> ";
        //        }

        //        tabla = " <table class=\"table table-striped jambo_table\" id=\"" + tbl + "\">";
        //        tabla += " <thead>";
        //        tabla += "               <tr>";
        //        tabla += "                   <th>Fecha Apertura</th>";
        //        tabla += "                   <th>Tipo Arqueo</th>";
        //        tabla += "                   <th>Acciones</th>";
        //        tabla += "               </tr>";
        //        tabla += "           </thead>";
        //        tabla += "           <tbody >     ";
        //        tabla += row;
        //        tabla += "           </tbody>";
        //        tabla += "       </table>";
        //    }
        //    catch (Exception)
        //    {
        //        control = false;
        //        msg = "Hubo un error al consultar los arqueos de la caja recaudación seleccionada";
        //    }
        //    return Json(new { success = control, msg = msg, listaDetalle = tabla }, JsonRequestBehavior.AllowGet);
        //}

        #endregion

    }
}