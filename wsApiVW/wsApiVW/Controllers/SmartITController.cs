using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using wsApiVW.Bussine;
using wsApiVW.Models;
using wsApiVW.Models.Aplicaciones;
using wsApiVW.Models.Audi;
using wsApiVW.Models.AutoModels;
using wsApiVW.Models.Factura;
using wsApiVW.Models.SmartIT;
using wsApiVW.Models.SubirDocumentos;

namespace wsApiVW.Controllers
{
    public class SmartITController : ApiController
    {
        private string Ruta;
        private OrdenCompraSmartIT obj;
        private MetodosDocumentos service;
        public SmartITController()
        {
            obj = new OrdenCompraSmartIT();
            Ruta = ConfigurationManager.AppSettings["Ruta"];
            service = new MetodosDocumentos();
        }

        [Route("api/SmartIT/PostConfiguraApartado", Name = "PostConfiguraApartado")]
        public Respuesta PostConfiguraApartado(PostConfiguraApartado con)
        {
            return obj.PostConfiguraApartado(con);
        }

        [Route("api/SmartIT/PutConfiguracionApartado", Name = "PutConfiguracionApartado")]
        public Respuesta PutConfiguracionApartado(ConfiguraApartado put)
        {
            return obj.PutConfiguracionApartado(put);
        }

        [Route("api/SmartIT/GetValoresConfigurados", Name = "GetValoresConfigurados")]
        public List<RestConfiguraApartado> GetValoresConfigurados(string IdApps)
        {
            return obj.GetValoresConfigurados(IdApps);
        }


        [Route("api/SmartIT/GetNoticias", Name = "GetNoticias")]
        public List<Noticia> GetNoticias(int IdApps)
        {
            return obj.Obtnernoticias(IdApps);
        }

        [Route("api/SmartIT/PostSubirNoticias", Name = "PostSubirNoticias")]
        [HttpPost]
        public async Task<Respuesta> PostSubirNoticias([FromBody]SubirNoticia archivo)
        {
            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            ApuntadorDeServicio _prod = new ApuntadorDeServicio();
            SQLTransaction _save = new SQLTransaction();
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/Noticias/");
            string salida = string.Empty;
            string IdNoticia = string.Empty;
            string sql = string.Empty;
            try
            {
                sql = $@"SELECT FIAPIDNOTI FROM NEW TABLE(INSERT INTO PRODAPPS.APCNOTIC(
                            FIAPIDNOTI,
                            FSAPNOMBNT,
                            FSAPDESCNT,
                            FSAPRUTDOC,
                            FIAPSTATUS,
                            USERCREAT,
                            FSRUTAIMNT,
                            FIAPIDAPPS)
                                VALUES(
                                (SELECT COALESCE(MAX(FIAPIDNOTI), 0) + 1 ID FROM PRODAPPS.APCNOTIC),
                                '{archivo.Nombre}',
                                '{archivo.Descripcion}',
                                '',
                                {1},
                                '{archivo.IdUser}',
                                '',
                                {archivo.IdApps}
                                ))";
                IdNoticia = dbCnx.GetDataSet(sql).Tables[0].Rows[0]["FIAPIDNOTI"].ToString();

                string filePath = root.ToString() + IdNoticia;
                if (!(await service.creaPDF(filePath + "_PDF.pdf", archivo.PDF)).Equals("SI"))
                    throw new Exception("500");
                else
                {
                    sql = $@"UPDATE PRODAPPS.APCNOTIC
                                SET FSAPRUTDOC= '{_prod.Respuestservicio() + "/Noticias/" + IdNoticia + "_PDF.pdf"}'
                                WHERE FIAPIDNOTI = " + IdNoticia + " AND FIAPIDAPPS = " + archivo.IdApps;
                    if (!_save.SQLGuardaTabla(sql))
                        throw new Exception("500");
                }


                if (!(await service.creaPDF(filePath + "_Imagen.Jpeg", archivo.Imagen)).Equals("SI"))
                    throw new Exception("500");
                else
                {
                    sql = $@"UPDATE PRODAPPS.APCNOTIC
                                SET FSRUTAIMNT = '{_prod.Respuestservicio() + "/Noticias/" + IdNoticia + "_Imagen.Jpeg"}'
                                WHERE FIAPIDNOTI = " + IdNoticia + " AND FIAPIDAPPS = " + archivo.IdApps;
                    if (!_save.SQLGuardaTabla(sql))
                        throw new Exception("500");
                }
                return new Respuesta() { Ok = "SI", Mensaje = "Si se pudo registrar.", Objeto = "" };
            }
            catch (Exception ex)
            {
                if (!ex.Message.Equals("500"))
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();
                }
                return new Respuesta() { Ok = "NO", Mensaje = "No se pudo subir el archivo.", Objeto = "" };
            }
        }

        [Route("api/SmartIT/GetDescuentos", Name = "GetDescuentos")]
        public List<TipoDescuento> GetDescuentos()
        {
            List<TipoDescuento> _coleccionDescuento = new List<TipoDescuento>();
            _coleccionDescuento = JsonConvert.DeserializeObject<
                List<TipoDescuento>>(File.ReadAllText(
                    (Ruta + "RecursosAudi\\TipoDescuento.json")));
            return _coleccionDescuento.Count == 0 ? new List<TipoDescuento>() : _coleccionDescuento;
        }

        [Route("api/SmartIT/PutProgramaEspecial", Name = "PutProgramaEspecial")]
        public Respuesta PutProgramaEspecial(int Id, int status, int IdApps)
        {
            return obj.ActualizarProgramaEspecial(Id, status, IdApps);
        }

        [Route("api/SmartIT/GetProgramasEspeciales", Name = "GetProgramasEspeciales")]
        public List<ResponsePromo> GetProgramasEspeciales(int IdApps)
        {
            return obj.ObtnerProgramasEspeciales(IdApps);
        }

        [Route("api/SmartIT/PostProgramasEspeciales", Name = "PostProgramasEspeciales")]
        [HttpPost]
        public async Task<Respuesta> SubirProgramasEspeciales([FromBody] Subirprograma archivo)
        {
            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            ApuntadorDeServicio _prod = new ApuntadorDeServicio();
            SQLTransaction _save = new SQLTransaction();
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~" + "/ProgramasEspeciales/");
            string salida = string.Empty;
            string IdPromo = string.Empty;
            string sql = string.Empty;
            try
            {
                sql = $@"SELECT FIAPIDPRO FROM NEW TABLE(INSERT INTO PRODAPPS.APCPROGR( 
                                            FIAPIDPRO, 
                                            FSAPTITUGR,
                                            FFAPFECHAI,
                                            FFAPFECHAF,
                                            FSAPRUTDOC,
                                            FIAPSTATUS,
                                            USERCREAT,
                                            FSAPDESCGR,
                                            FIAPTPDESC,
                                            FIAVALORDC,
                                            FIAPIDAPPS)
                        VALUES(
                        (SELECT COALESCE(MAX(FIAPIDPRO), 0) + 1 ID FROM PRODAPPS.APCPROGR),
                        '{archivo.Nombre}',
                        '{archivo.FechaInicio}',
                        '{archivo.FechaFinal}',
                        '',
                        1,
                        '{archivo.IdUser}',
                        '{archivo.Descripcion}',
                        {archivo.IdTipoDescuento},
                         {archivo.Valor},
                        {archivo.IdApps}))";
                IdPromo = dbCnx.GetDataSet(sql).Tables[0].Rows[0]["FIAPIDPRO"].ToString();

                string filePath = root.ToString() + IdPromo;
                if (!(await service.creaPDF(filePath + "_PDF.pdf", archivo.Base64)).Equals("SI"))
                    throw new Exception("500");
                else
                {
                    sql = $@"UPDATE PRODAPPS.APCPROGR
                                SET FSAPRUTDOC= '{_prod.Respuestservicio() + "/ProgramasEspeciales/" + IdPromo + "_PDF.pdf"}'
                                WHERE FIAPIDPRO = " + IdPromo + " AND FIAPIDAPPS= " + archivo.IdApps;
                    if (!_save.SQLGuardaTabla(sql))
                        throw new Exception("500");
                }

                return new Respuesta() { Ok = "SI", Mensaje = "Si se pudo registrar.", Objeto = "" };
            }
            catch (Exception ex)
            {
                if (!ex.Message.Equals("500"))
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();
                }
                return new Respuesta() { Ok = "NO", Mensaje = "No se pudo subir el archivo.", Objeto = "" };
            }
        }

        [Route("api/SmartIT/GetTipoVehciuloAudi", Name = "GetTipoVehciuloAudi")]
        public List<TipoVehiculoAudi> GetTipoVehciuloAudi()
        {
            List<TipoVehiculoAudi> _coleccionVehiculo = new List<TipoVehiculoAudi>();
            _coleccionVehiculo = JsonConvert.DeserializeObject
                <List<TipoVehiculoAudi>>
                (File.ReadAllText(
                    (Ruta + "RecursosAudi\\TipoVehiculo.json")));
            return _coleccionVehiculo.Count == 0 ? new List<TipoVehiculoAudi>() : _coleccionVehiculo;
        }

        [Route("api/SmartIT/CatalogoSeguro", Name = "CatalogoSeguro")]
        public List<TipoSeguro> GetTipoSeguro()
        {
            return obj.GetTipoSeguro();
        }

        [Route("api/SmartIT/PUTCita", Name = "PUTCita")]
        public Respuesta PUTCita(Citas _cita)
        {
            return obj.PutCita(_cita);
        }

        [Route("api/SmartIT/GetObtenerDocumentoComunicacion", Name = "GetObtenerDocumentoComunicacion")]
        public List<DocumentoComunicacion> GetObtenerDocumentoComunicacion(string aIdCompra, string aIdApps)
        {
            return obj.GetObtenerDocumentoComunicacion(aIdCompra, aIdApps);
        }

        [Route("api/SmartIT/GetObtieneHistorialMovimientosConAgencia", Name = "GetObtieneHistorialMovimientosConAgencia")]
        public List<HistorialMovimientosContactoAgencia> GetObtieneHistorialMovimientosConAgencia(string aIdCompra, string aIdApps)
        {
            return obj.GetObtieneHistorialMovimientosConAgencia(aIdCompra, aIdApps);
        }

        [Route("api/SmartIT/PostRegistraHistoricoContactoAgencia", Name = "PostRegistraHistoricoContactoAgencia")]
        public Respuesta PostRegistraHistoricoContactoAgencia([FromBody] HistoricoComunicacionAgencia historico)
        {
            return obj.PostRegistraHistoricoContactoAgencia(historico);
        }

        [Route("api/SmartIT/GetCatalogoIdApps", Name = "GetCatalogoIdApps")]
        public List<CatalogoApps> GetCatalogoIdApps()
        {
            return obj.GetCatalogoIdApps();
        }

        [Route("api/SmartIT/GetCatalogoMarca", Name = "GetCatalogoMarca")]
        public List<CatalogoMarca> GetCatalogoMarca()
        {
            return obj.GetCatalogoMarca();
        }

        [Route("api/SmartIT/PutActualizaCheckContactoAgencia", Name = "PutActualizaCheckContactoAgencia")]
        public Respuesta PutActualizaCheckContactoAgencia([FromBody] ActualizaCheckAgencia check)
        {
            return obj.PostActualizaCheckContactoAgencia(check);
        }

        [Route("api/SmartIT/GetCheckContactoAgenciaPorIdCompra", Name = "GetCheckContactoAgenciaPorIdCompra")]
        public List<DatoCheckAgencia> GetCheckContactoAgenciaPorIdCompra(long aIdCompra, string aIdApps)
        {
            return obj.GetCheckContactoAgenciaPorIdCompra(aIdCompra, aIdApps);
        }

        [Route("api/SmartIT/PutDeshacePasoRealizado", Name = "PutDeshacePasoRealizado")]
        public Respuesta PutDeshacePasoRealizado(long aIdCompra, int aIdPaso, string aIdApps)
        {
            return obj.UpdateDeshacePasoRealizado(aIdCompra, aIdPaso, aIdApps);
        }

        [Route("api/SmartIT/PostRegistraCheckContactoAgencia", Name = "PostRegistraCheckContactoAgencia")]
        public Respuesta PostRegistraCheckContactoAgencia([FromBody] CheckAgencia check)
        {
            return obj.RegistraCheckContactoAgencia(check);
        }

        [Route("api/SmartIT/GetCatalogoProcesoCompra", Name = "GetCatalogoProcesoCompra")]
        public List<ProcesoCompraSmartIt> GetCatalogoProcesoCompra()
        {
            return obj.GetCatalogoProcesoCompra();
        }

        [Route("api/SmartIT/GetDatosSeguro", Name = "GetDatosSeguro")]
        public Seguro GetDatosSeguro(long IdCompra)
        {
            return obj.GetDatosSeguro(IdCompra);
        }

        [Route("api/SmartIT/GetDatosGestoria", Name = "GetDatosGestoria")]
        public DatosGestoria GetDatosGestoria(long aIdCompra)
        {
            return obj.GetDatosGestoria(aIdCompra);
        }

        [Route("api/SmartIT/GetObtieneComprobantesPago", Name = "GetObtieneComprobantesPago")]
        public List<DocumentoSmartIT> GetObtieneComprobantesPago(long aIdCompra, string aIdApps)
        {
            return obj.GetObtieneComprobantesPago(aIdCompra, aIdApps);
        }

        [Route("api/SmartIT/GetObtenerSeguimientoCompra", Name = "GetObtenerSeguimientoCompra")]
        public List<SeguimientoCompra> GetSeguimientoCompra(long aIdCompra, string aIdApps)
        {
            return obj.GetSeguimientoCompra(aIdCompra, aIdApps);
        }

        [Route("api/SmartIT/GetDocumento", Name = "GetDocumento")]
        public List<DocumentoSmartIT> GetDocumento(long aIdCompra, string aIdApps)
        {
            return obj.GetDocumento(aIdCompra, aIdApps);
        }

        [Route("api/SmartIT/GetOrdenCompra", Name = "GetOrdenCompra")]
        public OrdenCompraSmartIt GetOrdenCompra(long aIdCompra, string aIdApps)
        {
            return obj.GetOrdenCompra(aIdCompra, aIdApps);
        }

        [Route("api/SmartIT/GetOrdenesCompra", Name = "GetOrdenesCompra")]
        public async Task<List<OrdenCompraSmartIt>> GetOrdenesCompra(string aIdApps)
        {
            return await obj.GetOrdenesCompra(aIdApps);
        }

        [Route("api/SmartIT/GetOrdenesDeCompraEnProcesoPorAgencia", Name = "GetOrdenesDeCompraEnProcesoPorAgencia")]
        public List<OrdenCompraSmartIt> GetOrdenesDeCompraEnProcesoPorAgencia(int aIdAgencia, string aIdApps)
        {
            return obj.GetOrdenesDeCompraEnProcesoPorAgencia(aIdAgencia, aIdApps);
        }

        [Route("api/SmartIT/PostEnviaNotificaciones", Name = "PostEnviaNotificaciones")]
        public Respuesta PostEnviaNotificaciones([FromBody]NotificacionCompra NotificacionCompra)
        {
            return obj.PostEnviaNotificaciones(NotificacionCompra);
        }

        [Route("api/SmartIT/GetCheckListOrdenCompra", Name = "GetCheckListOrdenCompra")]
        public List<Checklist> GetCheckListOrdenCompra(long aIdCompra, string aIdApps)
        {
            ProcesoCompra _compra = new ProcesoCompra();
            return _compra.CheckListOrden(aIdCompra, aIdApps);
        }

        [Route("api/SmartIT/GetDatosFiscales", Name = "GetDatosFiscales")]
        public DatosFiscales GetDatosFiscales(long aIdCompra, string aIdApps)
        {
            FClienteAuto _datos = new FClienteAuto();
            return _datos.GetDatosFiscales(aIdCompra, aIdApps);
        }

        [Route("api/SmartIT/GetAsuntos", Name = "GetAsuntos")]
        public Asunto GetAsuntos(string aIdEstado)
        {
            FClienteAuto _datos = new FClienteAuto();
            return _datos.GetAsuntos(aIdEstado);
        }

        [Route("api/SmartIT/GetMensajesPorIdAsunto", Name = "GetMensajesPorIdAsunto")]
        public MensajeNotificacion GetMensajesPorIdAsunto(string aIdAsunto)
        {
            FClienteAuto _datos = new FClienteAuto();
            return _datos.GetMensajesPorIdAsunto(aIdAsunto);
        }

        [Route("api/SmartIT/PostEnviaMotivoAutorizacion", Name = "PostEnviaMotivoAutorizacion")]
        public string PostEnviaMotivoAutorizacion([FromBody]AutorizacionDocumento doc)
        {
            return obj.GetEnviaMotivoAutorizacion(doc);
        }
    }
}