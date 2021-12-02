using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Controllers;
using wsApiVW.Models;
using wsApiVW.Models.Aplicaciones;
using wsApiVW.Models.Audi;
using wsApiVW.Models.AutoModels;
using wsApiVW.Models.User;
using wsApiVW.Services;
using static wsApiVW.Models.Audi.AutosAudi;
using static wsApiVW.Services.CorreoService;

namespace wsApiVW.Bussine
{
    public class AutosBussine
    {
        private string Ruta;
        private CorreoService service;
        public AutosBussine()
        {
            Ruta = ConfigurationManager.AppSettings["Ruta"];
            service = new CorreoService();
        }

        static string RutaVersiones = ConfigurationManager.AppSettings["RutaVersiones"];
        static string RutaMaquetas = ConfigurationManager.AppSettings["RutaMaquetas"];
        static string VersionesCarros = ConfigurationManager.AppSettings["Versiones"];
        static string Agencias = ConfigurationManager.AppSettings["Agencias"]; 
        static string DatosBancarios = ConfigurationManager.AppSettings["DatosBancarios"];
        static string PoliticasXMarca = ConfigurationManager.AppSettings["PoliticasXMarca"];
        static string RutaVehiculosModelos = ConfigurationManager.AppSettings["RutaVehiculosModelos"];

        public Respuesta PutProgramaEspecialDocumentos(string IdCompra, string IdCuenta, string IdApps, string IdProgramaEspecial, string IdEstado,string MotivoRechazo, string IdConsecutivo)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            SQLTransaction _save = new SQLTransaction();
            try
            {
                string sql = string.Empty;
                sql = $@"UPDATE PRODAPPS.APCPRERC
                        SET FIAPIDESTA = {IdEstado}, 
                            FSAPCOMENT = '{MotivoRechazo}',
                            FIAPIDCONS = {IdConsecutivo}             
                        WHERE FIAPIDCOMP = {IdCompra} 
                            AND FIAPIDCUEN = {IdCuenta} 
                            AND FIAPIDAPPS = {IdApps}
                            AND FIAPIDPRO = {IdProgramaEspecial}";
                if (!_save.SQLGuardaTabla(sql))
                    throw new Exception();
                return new Respuesta() { Ok = "SI", Mensaje = "Se actualizo con exito", Objeto = "" };
            }
            catch (Exception)
            {
                return new Respuesta() { Ok = "No", Mensaje = "Intente nuevamnete", Objeto = "" };
            }
        }

        public List<EstadoProgramasEspeciales> GetConsultaEstadosProgramasEspeciales()
        {
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                string sql = string.Empty;
                List<EstadoProgramasEspeciales> list = new List<EstadoProgramasEspeciales>();
                EstadoProgramasEspeciales ob;
                sql = @"SELECT * FROM PRODAPPS.PACPRESP";
                DataTable dt = dbCnx.GetDataSet(sql).Tables[0];
                if (dt.Rows.Count == 0)
                    throw new Exception();

                foreach (DataRow dr in dt.Rows)
                {
                    ob = new EstadoProgramasEspeciales();
                    ob.Id = dr["FIAPIDESTA"].ToString().Trim();
                    ob.Descripcion = dr["FSAPDESCNS"].ToString().Trim();
                    list.Add(ob);
                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<EstadoProgramasEspeciales>();
            }
        }
        public List<ProgramasEspeciales> GetPorgramaEspecial(string IdCuenta, string IdApps, string IdCompra)
        {
            SQLTransaction _save = new SQLTransaction();
            SmartITController controller = new SmartITController();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            try
            {
                List<ProgramasEspeciales> ListP = new List<ProgramasEspeciales>();
                ProgramasEspeciales Op;
                string sql = string.Empty;
                sql = $@"SELECT 
                        A.FIAPIDCUEN,
                        A.FIAPIDPRO,
                        A.FIAPIDAPPS,
                        A.FSAPRUTDOC RUTAPDF,
                        A.FIAPIDPRO,
                        A.FIAPIDESTA,
                        A.FIAPIDCOMP,
                        A.FSAPTITUGR,
                        A.FIAPTPDESC,
                        A.FSAPCOMENT,
                        A.FIAPIDCONS
                        FROM PRODAPPS.APCPRERC A
                        WHERE A.FIAPIDAPPS= {IdApps} AND A.FIAPIDCUEN = {IdCuenta} AND FIAPIDCOMP = {IdCompra}";
                DataTable count = dbCnx.GetDataSet(sql).Tables[0];

                if (count.Rows.Count == 0)
                {
                    if (!_save.SQLGuardaTabla($@"INSERT INTO PRODAPPS.APCPRERC ( 
                                    FIAPIDCOMP,FIAPIDCUEN, FIAPIDPRO, FIAPIDAPPS,
                                    FSAPTITUGR, FIAPTPDESC, FIAVALORDC,
                                    FFAPFECHAI, FFAPFECHAF, FSAPRUTDOC,
                                    FSAPDESCGR, FIAPIDESTA) 
                                    SELECT {IdCompra},{IdCuenta},FIAPIDPRO,FIAPIDAPPS,
                                    FSAPTITUGR, FIAPTPDESC, FIAVALORDC,
                                    FFAPFECHAI, FFAPFECHAF, FSAPRUTDOC,
                                FSAPDESCGR, 0 FROM PRODAPPS.APCPROGR
                                WHERE FIAPIDAPPS = {IdApps}
                                AND FFAPFECHAF >= CURRENT_DATE 
                                AND FIAPSTATUS = 1"))
                        throw new Exception("No se guardo correctamente");
                }

                DataTable dt = count.Rows.Count == 0 ?  dbCnx.GetDataSet(sql).Tables[0] : count;

                if (dt.Rows.Count == 0)
                    throw new Exception("No se guardo la relacion de clientes");

                foreach (DataRow dr in dt.Rows)
                {
                    List<ProgramaEspecialcliente> ListPEc = new List<ProgramaEspecialcliente>();
                    Op = new ProgramasEspeciales();
                    Op.IdProgramaEspecial = dr["FIAPIDPRO"].ToString().Trim();
                    Op.IdConsecutivo = dr["FIAPIDCONS"].ToString().Trim();
                    Op.IdApps = dr["FIAPIDAPPS"].ToString().Trim();
                    Op.RutaPDF = dr["RUTAPDF"].ToString().Trim();
                    Op.IdEstado = dr["FIAPIDESTA"].ToString().Trim() == "0" ? "" : dr["FIAPIDESTA"].ToString().Trim();
                    Op.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    Op.Titulo = dr["FSAPTITUGR"].ToString().Trim();
                    Op.IdDescuento = dr["FIAPTPDESC"].ToString().Trim();
                    Op.MotivoRechazo = dr["FSAPCOMENT"].ToString().Trim();
                    Op.Descuento = controller.GetDescuentos().Where(x => x.Id == Op.IdDescuento).First().Nombre;
                    Op.EstadoProgramaEspecial = (string.IsNullOrEmpty(Op.IdEstado) ? "" : GetConsultaEstadosProgramasEspeciales().Where(x => x.Id == Op.IdEstado).FirstOrDefault().Descripcion);
                    sql = $@"SELECT * FROM PRODAPPS.APCCVSRT
                             WHERE FIAPIDAPPS= {IdApps} 
                                    AND FIAPIDCUEN = {IdCuenta} 
                                    AND FIAPIDCOMP = {IdCompra} 
                                    AND FIAPIDPRO = {Op.IdProgramaEspecial}";
                    DataTable dw = dbCnx.GetDataSet(sql).Tables[0];
                    ProgramaEspecialcliente OpC;
                    foreach (DataRow di in dw.Rows)
                    {
                        OpC = new ProgramaEspecialcliente();
                        OpC.IdDocumento = di["FIAPIDRTDC"].ToString().Trim(); 
                        OpC.RutaDoc = di["FSAPRUTDOC"].ToString().Trim();
                        ListPEc.Add(OpC);
                    }
                    Op.Documentos = ListPEc;
                    ListP.Add(Op);
                }
                return ListP;
            }
            catch (Exception)
            {
                return new List<ProgramasEspeciales>();
            }
        }

        public Respuesta GenerarProgramaEspecial(long idCliente, int idEstado)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Respuesta respuesta = new Respuesta();
            respuesta.Ok = "NO";
            respuesta.Mensaje = "No fue posible migrar los datos";
            RespuestaPedido resp = new RespuestaPedido();
            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                #region VolcadoDeDatos
                string strConsulta = "";
                strConsulta += "SELECT FIAPIDPRO, FIAPIDAPPS,";
                strConsulta += "FSAPTITUGR, FIAPTPDESC, FIAVALORDC,";
                strConsulta += "FFAPFECHAI, FFAPFECHAF, FSAPRUTDOC,";
                strConsulta += "FSAPDESCGR FROM PRODAPPS.APCPROGR";
                DataTable datos = dbCnx.GetDataSet(strConsulta).Tables[0];
                if (datos.Rows.Count <= 0)
                {
                    throw new Exception("No hay registros en el catálogo de programas especiales");
                }
                else
                {
                    string strInserta = "";
                    strInserta += "INSERT INTO PRODAPPS.APCPRERC ( ";
                    strInserta += "FIAPIDCLIE, FIAPIDPRO, FIAPIDAPPS,";
                    strInserta += "FSAPTITUGR, FIAPTPDESC, FIAVALORDC,";
                    strInserta += "FFAPFECHAI, FFAPFECHAF, FSAPRUTDOC,";
                    strInserta += "FSAPDESCGR, FIAPIDESTA) ";
                    strInserta += "SELECT " + idCliente + "," + "FIAPIDPRO, FIAPIDAPPS, ";
                    strInserta += "FSAPTITUGR, FIAPTPDESC, FIAVALORDC,";
                    strInserta += "FFAPFECHAI, FFAPFECHAF, FSAPRUTDOC,";
                    strInserta += "FSAPDESCGR, '' FROM PRODAPPS.APCPROGR";
                    dbCnx.SetQuery(strInserta);
                }
                #endregion
                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se registró correctamente.";
                respuesta.Objeto = "";
            }
            catch (Exception)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No fue posible migrar los datos";
                respuesta.Objeto = null;
                return respuesta;
            }
            return respuesta;
        }
        internal CatalogoPolizas RecuperaPoliticasXMarca(string aIdMarca)
        {
            List<CatalogoPolizas> _politicas = new List<CatalogoPolizas>();
            string strJSON = File.ReadAllText(PoliticasXMarca);
            _politicas = JsonConvert.DeserializeObject<List<CatalogoPolizas>>(strJSON);
            return _politicas.Where(x => x.IdMarca == aIdMarca).Count() == 0 ? new CatalogoPolizas() : _politicas.Where(x => x.IdMarca == aIdMarca).FirstOrDefault();
        }
        internal Respuesta RegistraHistorico(HistoricoComunicacionAgencia historico)
        {
            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            SQLTransaction _sql = new SQLTransaction();
            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                string strHist = string.Empty;
                strHist += "INSERT INTO PRODAPPS.APDHSMVW (";
                strHist += "FIAPIDCIAU, FIAPIDCOMP,";
                strHist += "FFAPFECHA, FHAPHORA,";
                strHist += "FIAPIDESTA, FSAPDESEST,";
                strHist += "FIAPIDPCKL, FSAPDESCCK,";
                strHist += "FSAPNOTIFI, FIAPSTATUS,";
                strHist += "USERCREAT, PROGCREAT,FIAPIDAPPS)";
                strHist += "VALUES (";
                strHist += (string.IsNullOrEmpty(historico.IdAgencia) ? "Default" : historico.IdAgencia.Trim().ToUpper()) + "," + (string.IsNullOrEmpty(historico.IdCompra) ? "Default" : historico.IdCompra.Trim().ToUpper()) + ",";
                strHist += "CURRENT DATE, CURRENT TIME,";
                strHist += (string.IsNullOrEmpty(historico.IdEstadoMovimiento) ? "Default" : historico.IdEstadoMovimiento.Trim().ToUpper()) + " ," + "'" + (string.IsNullOrEmpty(historico.DescripcionEstadoMovimiento) ? "Default" : historico.DescripcionEstadoMovimiento.Trim().ToUpper()) + "'" + ",";
                strHist += (string.IsNullOrEmpty(historico.IdCheck) ? "Default" : historico.IdCheck.Trim().ToUpper()) + " ," + "'" + (string.IsNullOrEmpty(historico.DescripcionCheck) ? "Default" : historico.DescripcionCheck.Trim().ToUpper()) + "'" + ",";
                strHist += "'" + (string.IsNullOrEmpty(historico.Mensaje) ? "Default" : historico.Mensaje.Trim().ToUpper()) + "'" + "," + "1" + ",";
                strHist += "'APP', 'APP'," + historico.IdApps;
                strHist += ")";
                if (_sql.SQLGuardaTabla(strHist))
                    throw new Exception();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Datos registrados satisfactoriamente.";
                respuesta.Objeto = "";
            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo guardar el histórico.";
                respuesta.Objeto = "";
            }
            return respuesta;
        }
        internal Respuesta PostDeshacePasoRealizado(long aIdCompra, int IdPaso, string aIdApps)
        {
            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            SQLTransaction _sql = new SQLTransaction();
            string strActualiza =  string.Empty;
            try
            {
                strActualiza = @"UPDATE PRODAPPS.APDCKLST " +
                                 "SET FIAPREALIZ = 0, " +
                                 "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' " +
                                 "WHERE FIAPIDCOMP = " + aIdCompra.ToString() + " " +
                                 "AND FIAPIDPCKL = " + IdPaso.ToString() + " " +
                                 "AND FIAPIDAPPS = " + aIdApps.ToString() + " " +
                                 "AND FIAPSTATUS = 1";
                if (!_sql.SQLGuardaTabla(strActualiza))
                    throw new Exception();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Paso actualizado correctamente";
                respuesta.Objeto = "";
            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo actualizar el paso";
                respuesta.Objeto = "";
            }
            return respuesta;
        }

        internal List<EstadoOrdenCompra> GetObtenerEstadosOrdenCompra()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string strSql = string.Empty;
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APCESCVW ";
            strSql += "WHERE FIAPSTATUS = 1";
            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            EstadoOrdenCompra orden;
            List<EstadoOrdenCompra> coleccionOrdenes = new List<EstadoOrdenCompra>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    orden = new EstadoOrdenCompra();
                    orden.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
                    orden.DescripcionEstado = dr["FSAPESTADO"].ToString().Trim();
                    coleccionOrdenes.Add(orden);
                }
            }
            else
            {
                coleccionOrdenes = new List<EstadoOrdenCompra>();
            }
            return coleccionOrdenes;
        }
        internal Task<Respuesta> UpdateSeleccionaSeguro([FromBody]SeleccionaSeguroCliente seguro)
        {
            return Task.Run(() =>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                Respuesta respuesta = new Respuesta();
                try
                {
                    string strSql = string.Empty;
                    strSql = @"SELECT FIAPSELECT FROM PRODAPPS.APEDVPOL" + "\n" +
                                "WHERE 1=1" + "\n" +
                                    "AND FIAPIDCOMP= " + seguro.IdCompra + "\n" +
                                    "AND FIAPIDCUEN= " + seguro.IdCuenta + "\n" +
                                    "AND FIAPIDAPPS = " + seguro.IdApps + "\n" +
                                    "AND FIAPSELECT= 1";
                    DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        strSql = string.Empty;
                        strSql = @"UPDATE PRODAPPS.APEDVPOL SET FIAPSELECT=" + 0 + "\n" +
                               "WHERE 1=1" + "\n" +
                                   "AND FIAPIDCOMP= " + seguro.IdCompra + "\n" +
                                   "AND FIAPIDCUEN= " + seguro.IdCuenta + "\n" +
                                    "AND FIAPIDAPPS = " + seguro.IdApps;
                        dbCnx.SetQuery(strSql);
                    }

                    strSql = string.Empty;
                    strSql = @"UPDATE PRODAPPS.APEDVPOL SET FIAPSELECT=" + 1 + "\n" +
                                "WHERE 1=1" + "\n" +
                                    "AND FIAPIDCOMP= " + seguro.IdCompra + "\n" +
                                    "AND FIAPIDCUEN= " + seguro.IdCuenta + "\n" +
                                    "AND FIAPIDAPPS = " + seguro.IdApps + "\n" +
                                    "AND FIAPIDTIPO= " + seguro.Tipo + "\n" +
                                    "AND FIAPIDSPOL= " + seguro.IdConsecutivo;
                    dbCnx.SetQuery(strSql);
                    respuesta = new Respuesta() { Ok = "SI", Mensaje = $"Se ha selecionado la poliza de la compra {seguro.IdCompra}" };
                }
                catch (Exception)
                {
                    respuesta = new Respuesta() { Ok = "NO", Mensaje = "No se pudo seleccionar seguro.", Objeto = null };
                }
                return respuesta;
            });
        }
        internal List<SegurosCliente> GetObtenerSeguros(long IdCompra, long Idcuenta, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            string strSql = string.Empty;
            strSql = @"
                SELECT C.FIAPIDCOMP, C.FIAPIDSPOL, C.FIAPIDTIPO, D.* FROM

                    (SELECT B.FIAPIDCOMP, B.FIAPIDSPOL, A.FIAPIDTIPO, B.FIAPIDCUEN,  B.FIAPIDAPPS  FROM  PRODAPPS.APECTPVW A
                    CROSS JOIN(
                    SELECT DISTINCT FIAPIDSPOL, FIAPIDCOMP, FIAPIDCUEN, FIAPIDAPPS FROM PRODAPPS.APEDVPOL
                            ) B) C
                            LEFT JOIN(SELECT * FROM PRODAPPS.APEDVPOL) D
                            ON C.FIAPIDCOMP = D.FIAPIDCOMP AND C.FIAPIDTIPO = D.FIAPIDTIPO AND C.FIAPIDSPOL = D.FIAPIDSPOL AND C.FIAPIDCUEN = D.FIAPIDCUEN
                            WHERE C.FIAPIDCOMP =" + IdCompra + " AND C.FIAPIDCUEN = " + Idcuenta + " AND C.FIAPIDAPPS =" + aIdApps + " ORDER BY C.FIAPIDCOMP, C.FIAPIDSPOL, C.FIAPIDTIPO";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            SegurosCliente _ob;
            List<SegurosCliente> colecciones = new List<SegurosCliente>();

            foreach (DataRow dr in dt.Rows)
            {
                _ob = new SegurosCliente();
                _ob.IdConsecutivo = dr["FIAPIDSPOL"].ToString().Trim();
                _ob.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                _ob.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                _ob.Nombre = dr["FSAPNOASEG"].ToString().Trim();
                _ob.Cobertura = dr["FSAPCOBERT"].ToString().Trim();
                _ob.Cantidad = dr["FDAPCANTID"].ToString().Trim();
                _ob.Tipo = dr["FIAPIDTIPO"].ToString().Trim();
                _ob.NombreDocumento = dr["FSAPDOCUME"].ToString().Trim();
                _ob.Ruta = dr["FSAPRUTDOC"].ToString().Trim();
                _ob.selecciono = string.IsNullOrEmpty(dr["FIAPSELECT"].ToString().Trim()) ? "0" : dr["FIAPSELECT"].ToString().Trim();
                colecciones.Add(_ob);
            }

            return colecciones;
        }

        internal Respuesta GetObtenerReferenciaDePago(long IdCompra, string aIdApps)
        {
            Respuesta _respuesta = new Respuesta();
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                string strSql = string.Empty;
                strSql = @"SELECT FIAPIDCOMP IDCOMPRA, FSAPRUTRFB LINEACAPTURA  FROM PRODAPPS.APECMPVW " +
                                 "WHERE FIAPIDCOMP = " + IdCompra + " AND FIAPIDAPPS=" + aIdApps;
                _respuesta.Mensaje = dbCnx.GetDataSet(strSql).Tables[0].Rows[0]["LINEACAPTURA"].ToString().Trim();
                if (string.IsNullOrEmpty(_respuesta.Mensaje))
                    throw new Exception();
                _respuesta.Ok = "SI";
                _respuesta.Objeto = "";
            }
            catch (Exception)
            {
                _respuesta.Ok = "NO";
                _respuesta.Mensaje = "";
                _respuesta.Objeto = "";
            }
            return _respuesta;
        }
        internal CuentasBancariasPorAgencia GetDatosBancario(int aIdAgencia)
        {
            List<DatosBancariosXIdAgencia> _Colecciondatos = new List<DatosBancariosXIdAgencia>();
            CuentasBancariasPorAgencia _datos = new CuentasBancariasPorAgencia();
            string strJSON = File.ReadAllText(DatosBancarios);
            _Colecciondatos = JsonConvert.DeserializeObject<List<DatosBancariosXIdAgencia>>(strJSON);
            return _Colecciondatos.Where(x => x.IdAgencia == aIdAgencia.ToString()).ToList().Count == 0 ? new CuentasBancariasPorAgencia() : _Colecciondatos.Where(x => x.IdAgencia == aIdAgencia.ToString()).First().DatosBancarios.First(); ;
        }
        internal Ruta360Maquetas MaquetasXMarca(string aIdMarca)
        {
            List<Ruta360Maquetas> maquetas = new List<Ruta360Maquetas>();
            string strJSON = File.ReadAllText(RutaMaquetas);
            maquetas = JsonConvert.DeserializeObject<List<Ruta360Maquetas>>(strJSON);
            return maquetas.Where(x => x.IdMarca == aIdMarca).Count() == 0 ? new Ruta360Maquetas() : maquetas.Where(x => x.IdMarca == aIdMarca).First();
        }

        internal Vehiculos ModelosXMarca(string aIdMarca)
        {
            List<Vehiculos> _coleccionVehiculo = new List<Vehiculos>();
            string strJSON = File.ReadAllText(RutaVehiculosModelos);
            _coleccionVehiculo = JsonConvert.DeserializeObject<List<Vehiculos>>(strJSON);
            return _coleccionVehiculo.Count == 0 ? new Vehiculos() : _coleccionVehiculo.Where(x => x.IdMarca == aIdMarca).FirstOrDefault();
        }

        internal List<ModeloAudi> ModelosXMarcaAudi()
        {
            List<ModeloAudi> _coleccionVehiculo = new List<ModeloAudi>();
            _coleccionVehiculo = JsonConvert.DeserializeObject
                <List<ModeloAudi>>
                (File.ReadAllText(
                    (Ruta + "RecursosAudi\\Versiones.json")));
            return _coleccionVehiculo.Count == 0 ? new List<ModeloAudi>() : _coleccionVehiculo;
        }

        internal List<Inventario> GetObtenerInventarioXAgencia(string aIdAgencia, string aIdVersion, string aIdMarca)
        {
            System.Globalization.CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;
            DVADB.DB2 dbCnx = new DVADB.DB2();
            string _aIdMarca = string.Empty;
            _aIdMarca = aIdMarca == "0" ? "41" : aIdMarca;          
            try
            {
                List<InventarioZona> lstZonas = new List<InventarioZona>();
                InventarioZona zona = new InventarioZona();
                string strSql = string.Empty;
                strSql = $@"SELECT  COLORCARROAPP.FSAPRUTVEH,AUTOM.FNAUTOAGE, AUTOM.FIANIDINVE, AUTOM.FNAUTOMID, AUTOM.FNAUTOVAN, 'N' || '-' || FNAUTOANO || '-' || FNAUTONUM NumeroInventario, TRIM(AUTOM.FSAUTOSER) FSAUTOSER, AUTOM.FNAUTOSUV, AUTOM.FNAUTOIVV, AUTOM.FNAUTOTAV, TRIM(COLORCARROAPP.FIAPCOLOR) IDCOLOR,TRIM(COLORCARROAPP.FSAPDESCRI) FSCOLODES 
                            FROM PRODAUT.ANCMARCA MARCA

                            INNER JOIN PRODAUT.ANCFAMIL FAMIL 
                            ON MARCA.FNMARCCLA = FAMIL.FNFAMISUB

                            INNER JOIN PRODAUT.ANCMODEL MODEL
                            ON FAMIL.FNFAMICLA = MODEL.FNMODEFAM

                            INNER JOIN PRODAPPS.APCVERSI VERSIAPP
                            ON VERSIAPP.FIAPIDVERS =  {aIdVersion}
                            
                            INNER JOIN PRODAUT.ANCVERSI VERSISMART
                            ON MODEL.FNMODEIDM = VERSISMART.FNVERSIDM 
                            AND VERSISMART.FNVERSIDV = VERSIAPP.FNVERSIDV 

                            INNER JOIN PRODAUT.ANCAUTOM AUTOM
                            ON VERSISMART.FNVERSIDV = AUTOM.FNAUTOVAN
                            AND AUTOM.FNAUTOAGE = {aIdAgencia}
                            AND  AUTOM.FNAUTOEST IN (6,10)
                            AND AUTOM.FIANAUTMAV = 1

                            INNER JOIN PRODAPPS.APCAAGEN APPAGEN
                            ON AUTOM.FNAUTOAGE =  APPAGEN.FIAPIDAGEN
                            AND APPAGEN.FIAPIDMARC = {aIdMarca}
                            AND APPAGEN.FIAPSTATUS = 1

                            INNER JOIN  PRODAUT.ANCCOLOR COLOR 
                            ON AUTOM.FNAUTOCOE = COLOR.FNCOLOCLA 

                            INNER JOIN PRODAPPS.APCSCOLO COLORSMARTITVSCOLORAPP
                            ON  COLORSMARTITVSCOLORAPP.FIAPIDVERS = VERSIAPP.FIAPIDVERS
                            AND COLORSMARTITVSCOLORAPP.FIAPSCOLOR = COLOR.FNCOLOCLA

                            INNER JOIN PRODAPPS.APCCOLOR COLORCARROAPP
                            ON COLORCARROAPP.FIAPCOLOR = COLORSMARTITVSCOLORAPP.FIAPCOLOR
                            AND COLORCARROAPP.FIAPIDVERS = COLORSMARTITVSCOLORAPP.FIAPIDVERS                            
                            WHERE MARCA.FNMARCCLA =" + _aIdMarca;

                DataTable dtInventario = dbCnx.GetDataSet(strSql).Tables[0];
                Inventario inventario;
                List<Inventario> coleccionInventario = new List<Inventario>();

                if (dtInventario.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtInventario.Rows)
                    {
                        inventario = new Inventario();
                        inventario.Color = textInfo.ToTitleCase(dr["FSCOLODES"].ToString().ToLower().Trim());
                        inventario.IdAgencia = dr["FNAUTOAGE"].ToString().Trim();
                        inventario.IdInventario = dr["FIANIDINVE"].ToString().Trim();
                        inventario.IdVehiculo = dr["FNAUTOMID"].ToString().Trim();
                        inventario.IdVersion = dr["FNAUTOVAN"].ToString().Trim();
                        inventario.IdColor = dr["IDCOLOR"].ToString().Trim();
                        inventario.Ruta = dr["FSAPRUTVEH"].ToString().Trim();
                        inventario.NumeroSerie = dr["FSAUTOSER"].ToString().Trim();
                        inventario.NumeroInventario = dr["NumeroInventario"].ToString().Trim();
                        coleccionInventario.Add(inventario);
                    }
                }
                return coleccionInventario;
            }
            catch (Exception)
            {
                return new List<Inventario>();
            }
        }



        internal Politicas RecuperaPoliticas(string aIdMarca, string aIdAgencia)
        {
            Politicas _Opoliticas = new Politicas();
            List<CatalogoPolizas> _politicas = new List<CatalogoPolizas>();
            string strJSON = File.ReadAllText(PoliticasXMarca);
            _politicas = JsonConvert.DeserializeObject<List<CatalogoPolizas>>(strJSON);
            var PoliticasAgencias = _politicas.Where(x => x.IdMarca == aIdMarca).SelectMany(x => x.PoliticasAgencias).ToList();
            foreach (var item in PoliticasAgencias)
            {
                if (item.IdAgencia == aIdAgencia)
                {
                    _Opoliticas.PoliticasAgencia = item.Politicas.ToString();
                    break;
                }
            }
            _Opoliticas.RutaPrivacidad = _politicas.Where(x => x.IdMarca == aIdMarca).Select(x => x.AvisoDePrivacidad).FirstOrDefault();
            _Opoliticas.RutaTerminosyCondiciones = _politicas.Where(x => x.IdMarca == aIdMarca).Select(x => x.AvisoDePrivacidad).FirstOrDefault();
            return _Opoliticas;
        }

        public List<InventarioAudi> GetObtenerInventarioAudi(string aIdModelo)
        {
            string sql = string.Empty;
            try
            {
                System.Globalization.CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;
                DVADB.DB2 dbCnx = new DVADB.DB2();
                InventarioAudi inventario;
                List<InventarioAudi> coleccionInventario = new List<InventarioAudi>();
                sql = $@"SELECT 
                        VERSI.FNVERSIDV,
                        AUTOM.FNAUTOAGE, 
                        AUTOM.FIANIDINVE, 
                        AUTOM.FNAUTOMID, 
                        VERSIAPP.FIAPIDVERS FNAUTOVAN, 'N' || '-' || FNAUTOANO || '-' || FNAUTONUM NumeroInventario,
                        TRIM(AUTOM.FSAUTOSER) FSAUTOSER, 
                        AUTOM.FNAUTOSUV, 
                        AUTOM.FNAUTOIVV, 
                        AUTOM.FNAUTOTAV, 
                        MODEL.FNMODEIDM,
                        MODEL.FSMODECLA NOMBREMODELO,
                        TRIM(COLORCARROAPP.FIAPCOLOR) IDCOLOR,
                        TRIM(COLORCARROAPP.FSAPDESCRI) FSCOLODES,
                        COLORCARROAPP.FSAPRUTVEH,
                        AUTOM.FNAUTOPBC, 
                        AUTOM.FDANPREESP,
                        VERSI.FSVERSDES  
                     FROM PRODAUT.ANCMARCA MARCA 
                 
                     INNER JOIN   PRODAUT.ANCFAMIL FAMIL 
                     ON   MARCA.FNMARCCLA = FAMIL.FNFAMISUB 

                     INNER JOIN   PRODAUT.ANCMODEL MODEL 
                     ON   FAMIL.FNFAMICLA = MODEL.FNMODEFAM 
                     AND MODEL.FNMODEIDM = {aIdModelo}
                 
                     INNER JOIN   PRODAUT.ANCVERSI VERSI 
                     ON   MODEL.FNMODEIDM = VERSI.FNVERSIDM 
                 
                     INNER JOIN   PRODAUT.ANCAUTOM AUTOM 
                     ON   VERSI.FNVERSIDV = AUTOM.FNAUTOVAN 
                     AND AUTOM.FNAUTOAGE IN (81,23) 
                     AND  AUTOM.FNAUTOEST IN (6,10)
                     AND AUTOM.FIANAUTMAV = 1 
                 
                     INNER JOIN   PRODAUT.ANCCOLOR COLOR 
                     ON   AUTOM.FNAUTOCOE = COLOR.FNCOLOCLA
                 
                     INNER JOIN  PRODAPPS.APCVERSI VERSIAPP
                     ON VERSIAPP.FNVERSIDV = VERSI.FNVERSIDV
                 
                     INNER JOIN PRODAPPS.APCSCOLO COLORSMARTITVSCOLORAPP
                     ON  COLORSMARTITVSCOLORAPP.FIAPIDVERS = VERSIAPP.FIAPIDVERS
                     AND COLORSMARTITVSCOLORAPP.FIAPSCOLOR = COLOR.FNCOLOCLA

                     INNER JOIN PRODAPPS.APCCOLOR COLORCARROAPP
                     ON COLORCARROAPP.FIAPCOLOR = COLORSMARTITVSCOLORAPP.FIAPCOLOR
                     AND COLORCARROAPP.FIAPIDVERS = COLORSMARTITVSCOLORAPP.FIAPIDVERS
                     WHERE    MARCA.FNMARCCLA = 39";
                DataTable dtInventario = dbCnx.GetDataSet(sql).Tables[0];

                if (dtInventario.Rows.Count == 0)
                    throw new Exception();

                foreach (DataRow dr in dtInventario.Rows)
                {
                    inventario = new InventarioAudi();
                    inventario.Color = textInfo.ToTitleCase(dr["FSCOLODES"].ToString().ToLower().Trim());
                    inventario.IdAgencia = dr["FNAUTOAGE"].ToString().Trim();
                    inventario.IdModelo = dr["FNMODEIDM"].ToString().Trim();
                    inventario.IdInventario = dr["FIANIDINVE"].ToString().Trim();
                    inventario.IdVehiculo = dr["FNAUTOMID"].ToString().Trim();
                    inventario.IdVersion = dr["FNAUTOVAN"].ToString().Trim();
                    inventario.IdColor = dr["IDCOLOR"].ToString().Trim();
                    inventario.Ruta = dr["FSAPRUTVEH"].ToString().Trim();
                    inventario.NumeroSerie = dr["FSAUTOSER"].ToString().Trim();
                    inventario.NumeroInventario = dr["NumeroInventario"].ToString().Trim();
                    inventario.PrecioBase = dr["FDANPREESP"].ToString().Trim() == "0.00" ? dr["FNAUTOPBC"].ToString().Trim() : "";
                    inventario.PrecioEspecial = dr["FDANPREESP"].ToString().Trim() == "0.00" ? "" : dr["FDANPREESP"].ToString().Trim();

                    inventario.Titulo = dr["FSVERSDES"].ToString().Trim();
                    sql = $@"SELECT FNEQUICLA, FSEQUIDES FROM PRODAUT.ANCAUTEQ EQV
                            INNER JOIN PRODAUT.ANCEQUIP EQU
                            ON EQU.FNEQUICLA = eqv.FSAUEQCLA
                            WHERE FNAUEQAGE = {inventario.IdAgencia}
                            AND FNAUEQVEI = {inventario.IdVehiculo}";
                    DataTable dtAccesorio = dbCnx.GetDataSet(sql).Tables[0];

                    if (dtAccesorio.Rows.Count > 0)
                    {
                        AccesorioAudi Oace;
                        List<AccesorioAudi> lisAce = new List<AccesorioAudi>();
                        foreach (DataRow dt in dtAccesorio.Rows)
                        {
                            Oace = new AccesorioAudi();
                            Oace.Id = dt["FNEQUICLA"].ToString();
                            Oace.Descripcion = dt["FSEQUIDES"].ToString().ToLower().Trim();
                            lisAce.Add(Oace);
                        }
                        inventario.Accesorios = lisAce;
                    }
                    coleccionInventario.Add(inventario);
                }
                return coleccionInventario;
            }
            catch (Exception)
            {
                return new List<InventarioAudi>();
            }
        }

        internal List<Inventario> ObtenerInventario(string aIdMarca, string aIdVersion)
        {
            System.Globalization.CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            try
            {
                string _aIdMarca = string.Empty;
                _aIdMarca = aIdMarca == "0" ? "41" : aIdMarca;
                string strSql = string.Empty;
                strSql = $@"SELECT  COLORCARROAPP.FSAPRUTVEH,AUTOM.FNAUTOAGE, AUTOM.FIANIDINVE, AUTOM.FNAUTOMID, AUTOM.FNAUTOVAN, 'N' || '-' || FNAUTOANO || '-' || FNAUTONUM NumeroInventario, TRIM(AUTOM.FSAUTOSER) FSAUTOSER, AUTOM.FNAUTOSUV, AUTOM.FNAUTOIVV, AUTOM.FNAUTOTAV, TRIM(COLORCARROAPP.FIAPCOLOR) IDCOLOR,TRIM(COLORCARROAPP.FSAPDESCRI) FSCOLODES 
                            FROM PRODAUT.ANCMARCA MARCA

                            INNER JOIN PRODAUT.ANCFAMIL FAMIL 
                            ON MARCA.FNMARCCLA = FAMIL.FNFAMISUB

                            INNER JOIN PRODAUT.ANCMODEL MODEL
                            ON FAMIL.FNFAMICLA = MODEL.FNMODEFAM

                            INNER JOIN PRODAPPS.APCVERSI VERSIAPP
                            ON VERSIAPP.FIAPIDVERS = {aIdVersion}
                            
                            INNER JOIN PRODAUT.ANCVERSI VERSISMART
                            ON MODEL.FNMODEIDM = VERSISMART.FNVERSIDM 
                            AND VERSISMART.FNVERSIDV = VERSIAPP.FNVERSIDV 

                            INNER JOIN PRODAUT.ANCAUTOM AUTOM
                            ON VERSISMART.FNVERSIDV = AUTOM.FNAUTOVAN

                            AND  AUTOM.FNAUTOEST IN (6,10)
                            AND AUTOM.FIANAUTMAV = 1

                            INNER JOIN PRODAPPS.APCAAGEN APPAGEN
                            ON AUTOM.FNAUTOAGE =  APPAGEN.FIAPIDAGEN
                            AND APPAGEN.FIAPIDMARC = {aIdMarca}
                            AND APPAGEN.FIAPSTATUS = 1

                            INNER JOIN  PRODAUT.ANCCOLOR COLOR 
                            ON AUTOM.FNAUTOCOE = COLOR.FNCOLOCLA 

                            INNER JOIN PRODAPPS.APCSCOLO COLORSMARTITVSCOLORAPP
                            ON  COLORSMARTITVSCOLORAPP.FIAPIDVERS = VERSIAPP.FIAPIDVERS
                            AND COLORSMARTITVSCOLORAPP.FIAPSCOLOR = COLOR.FNCOLOCLA

                            INNER JOIN PRODAPPS.APCCOLOR COLORCARROAPP
                            ON COLORCARROAPP.FIAPCOLOR = COLORSMARTITVSCOLORAPP.FIAPCOLOR
                            AND COLORCARROAPP.FIAPIDVERS = COLORSMARTITVSCOLORAPP.FIAPIDVERS                            
                            WHERE MARCA.FNMARCCLA =" + _aIdMarca;

                DataTable dtInventario = dbCnx.GetDataSet(strSql).Tables[0];
                Inventario inventario;
                List<Inventario> coleccionInventario = new List<Inventario>();

                if (dtInventario.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtInventario.Rows)
                    {
                        inventario = new Inventario();
                        inventario.Color = textInfo.ToTitleCase(dr["FSCOLODES"].ToString().ToLower().Trim());
                        inventario.IdAgencia = dr["FNAUTOAGE"].ToString().Trim();
                        inventario.IdInventario = dr["FIANIDINVE"].ToString().Trim();
                        inventario.IdVehiculo = dr["FNAUTOMID"].ToString().Trim();
                        inventario.IdVersion = dr["FNAUTOVAN"].ToString().Trim();
                        inventario.IdColor = dr["IDCOLOR"].ToString().Trim();
                        inventario.Ruta = dr["FSAPRUTVEH"].ToString().Trim();
                        inventario.NumeroSerie = dr["FSAUTOSER"].ToString().Trim();
                        inventario.NumeroInventario = dr["NumeroInventario"].ToString().Trim();
                        coleccionInventario.Add(inventario);
                    }
                }
                return coleccionInventario;
            }
            catch (Exception)
            {
               return new List<Inventario>();
            }

        }

        public Task<RespuestaTest<OrdenCompraPedido>> RegistraApartadoAudi(OrdenCompraPedido compra)
        {
            return Task.Run(() =>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                AutosBussine _au = new AutosBussine();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                RespuestaTest<OrdenCompraPedido> respuesta = new RespuestaTest<OrdenCompraPedido>();
                respuesta.Ok = "NO";
                respuesta.Mensaje = string.Empty;

                string strSql = string.Empty;

                try
                {
                    dbCnx.AbrirConexion();
                    dbCnx.BeginTransaccion();
                    long idCuenta = 0;
                    #region validacionInventarioDisponible

                    strSql = string.Empty;
                    strSql += "SELECT FNAUTOEST FROM PRODAUT.ANCAUTOM ";
                    strSql += "WHERE FNAUTOAGE = " + compra.IdAgencia.ToString().Trim() + " ";
                    strSql += "AND FIANIDINVE=" + compra.IdInventario + "\t";
                    strSql += "AND FIANSTATU = 1 \t";
                    strSql += "AND FNAUTOEST in (6,10) AND FIANAUTMAV = 1";

                    DataTable dtVa = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dtVa.Rows.Count == 0)
                    {
                        respuesta.Ok = "NO";
                        respuesta.Mensaje = "Inventario no disponible.";
                        respuesta.Objeto = null;
                        throw new Exception();
                    }
                    #endregion

                    #region REGISTRA CUENTA

                    strSql = string.Empty;

                    strSql += "SELECT A.FIAPIDCUEN Id, A.FSAPNOMBRE Nombre, A.FSAPAPEPAT ApellidoPaterno, A.FSAPAPEMAT ApellidoMaterno,";
                    strSql += "A.FIAPLADMOV Lada,";
                    strSql += "A.FIAPNUMMOV numero FROM PRODAPPS.APCCTAVW A ";
                    strSql += "INNER JOIN PRODAPPS.APCTOKEN B ";
                    strSql += "ON A.FIAPIDCUEN = B.FIAPIDCUEN ";
                    strSql += "AND A.FIAPIDAPPS = B.FIAPIDAPPS ";
                    strSql += "AND B.FIAPSTATUS = 1 ";
                    strSql += "WHERE lower(TRIM(A.FSAPCORREO)) = " + "'" + compra.CuentaUsuario.Correo.Trim().ToLower() + "'" + " ";
                    strSql += "AND A.FIAPSTATUS = 1 AND A.FIAPIDAPPS = " + compra.IdApp;


                    DataTable dtValidaCuenta = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dtValidaCuenta.Rows.Count > 0)
                    {
                        idCuenta = long.Parse(dtValidaCuenta.Rows[0]["Id"].ToString());
                        compra.CuentaUsuario.IdCuenta = idCuenta.ToString();
                        compra.CuentaUsuario.Nombre = dtValidaCuenta.Rows[0]["Nombre"].ToString().ToUpper().Trim();
                        compra.CuentaUsuario.ApellidoPaterno = dtValidaCuenta.Rows[0]["ApellidoPaterno"].ToString().ToUpper().Trim();
                        compra.CuentaUsuario.ApellidoMaterno = dtValidaCuenta.Rows[0]["ApellidoMaterno"].ToString().ToUpper().Trim();
                        compra.CuentaUsuario.LadaMovil = dtValidaCuenta.Rows[0]["Lada"].ToString().Trim();
                        compra.CuentaUsuario.TelefonoMovil = dtValidaCuenta.Rows[0]["numero"].ToString().Trim();


                        strSql = string.Empty;
                        strSql += "UPDATE " + constantes.Ambiente + "APPS.APCTOKEN ";
                        strSql += " SET FIAPSTATUS = 1, FSAPTOKEN = '" + (!string.IsNullOrEmpty(compra.CuentaUsuario.Token.Trim()) ? compra.CuentaUsuario.Token.Trim() : "") + "', USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FIAPIDCUEN =  " + idCuenta.ToString() + " AND FIAPIDAPPS = " + compra.IdApp;
                        dbCnx.SetQuery(strSql);

                    }
                    else
                    {

                        RespuestaTest<Cuenta> salida = new RespuestaTest<Cuenta>();
                        CuentasUsuarios _user = new CuentasUsuarios();
                        RegistraCuenta cuenta = new RegistraCuenta();

                        cuenta.IdApp = compra.IdApp.ToString();
                        cuenta.Nombre = compra.CuentaUsuario.Nombre.Trim().ToUpper();
                        cuenta.ApellidoPaterno = compra.CuentaUsuario.ApellidoPaterno.Trim().ToUpper();
                        cuenta.ApellidoMaterno = compra.CuentaUsuario.ApellidoMaterno.Trim().ToUpper();
                        cuenta.Correo = compra.CuentaUsuario.Correo.Trim().ToLower();
                        cuenta.Token = compra.CuentaUsuario.Token.Trim();
                        cuenta.LadaMovil = compra.CuentaUsuario.LadaMovil;
                        cuenta.TelefonoMovil = (string.IsNullOrEmpty(compra.CuentaUsuario.TelefonoMovil.ToString().Trim()) ? "5554820300" : compra.CuentaUsuario.TelefonoMovil.ToString().Trim());
                        salida = _user.RegistraCuenta(cuenta);
                        if (salida.Mensaje.Equals("NO"))
                            throw new Exception();
                    }

                    #endregion

                    #region COMPRA ACTIVA 

                    strSql = string.Empty;
                    strSql += "select FIAPIDCOMP Cuenta ";
                    strSql += "FROM " + constantes.Ambiente + "apps.APECMPVW ape ";
                    strSql += "inner join " + constantes.Ambiente + "apps.APCESCVW est ";
                    strSql += "ON ape.FIAPIDESTA = est.FIAPIDESTA ";
                    strSql += "WHERE ape.FIAPSTATUS = 1 AND ape.FIAPIDESTA NOT IN(15,24) AND ape.FIAPIDCUEN = " + compra.CuentaUsuario.IdCuenta + " AND ape.FIAPIDAPPS = " + compra.IdApp; //(1,2,3,4,5,6,7,8,9,10,11,12,13,14,16,17,18,19,20,21,22,23)
                    DataTable dtCount = dbCnx.GetDataSet(strSql).Tables[0];

                    #endregion

                    if (dtCount.Rows.Count == 0)
                    {
                        #region REGISTRO COMPRA

                        strSql = "";
                        strSql = @"SELECT FIAPIDCOMP FROM NEW TABLE ( INSERT INTO PRODAPPS.APECMPVW
                            (
                            FIAPIDAPPS,      /*ID APP*/
                            FIAPIDCOMP,      /*ID COMPRA   */
                            FIAPIDMARC,      /*ID MARCA */
                            FSAPDESCRI,      /*DESCRIPCION*/
                            FIAPFOLCOM,      /* FOLIO COMPRA*/
                            FIAPIDCUEN,      /*ID CUENTA   */
                            FFAPFECCOM,      /*FECHA COMPRA*/
                            FHAPHORCOM,      /*HORA COMPRA */
                            FDAPSUBTOT,      /*SUBTOTAL */
                            FDAPDESCUE,      /*DESCUENTO*/
                            FDAPIVA,         /*IVA      */
                            FDAPTOTAL,       /* TOTAL*/    
                            FIAPIDESTA,      /*ID ESTADO*/
                            FIAPIDPROC,      /*ID PROCESO*/ 
                            FIAPIDPASO,      /*ID PASO*/
                            FIAPSTATUS,      /*ESTATUS     */     
                            USERCREAT,       /*USUARIO CREACION */
                            DATECREAT,       /*FECHA CREACION   */
                            TIMECREAT,       /*HORA CREACION    */
                            PROGCREAT        /*PROGRAMA CREACION*/
                        ) VALUES(" +
                        compra.IdApp + ", " +
                        "(SELECT COALESCE(MAX(FIAPIDCOMP), 0) + 1 Id FROM PRODAPPS.APECMPVW WHERE FIAPIDAPPS = " + compra.IdApp + ")" + " , " +
                        compra.IdMarca + "," +
                        "'" + compra.Descripcion + "'," +
                        "(SELECT COALESCE(MAX(FIAPIDCOMP), 0) + 1 Id FROM PRODAPPS.APECMPVW WHERE FIAPIDAPPS = " + compra.IdApp + ")" + " , " +   /*revisar el tema del select max + 1 debe de ser el mismo de compra.idcompra l 741*/
                        compra.CuentaUsuario.IdCuenta + " , " +
                        "CURRENT DATE , " +
                        "CURRENT TIME , " +
                        /*compra.Subtotal*/ ((Math.Round(Convert.ToDecimal(compra.Total), 2)) / (Decimal)1.16).ToString() + " , " + /*se calculca con el total*/
                        0 + " , " + //DESCUENTO
                                    /*compra.IVA*/ (Math.Round(Convert.ToDecimal(compra.Total), 2) / (Decimal)1.16 * (Decimal)0.16).ToString() + " , " + /*se calcula con el total*/
                                                                                                                                                         //compra.Total + " , '" +
                                                                                                                                                         //compra.RutaReferenciaBancaria + "' , " +
                        compra.Total + " , " + /* recino del json*/
                        1 + " , " + //ID ESTADO
                        2 + " , " + //ID PROCESO
                        1 + " , " + //ID PASO
                        1 + " , " +
                        "'APP' , " +
                        "CURRENT DATE , " +
                        "CURRENT TIME , " +
                        "'APP'" + ")" + ")";

                        compra.IdCompra = dbCnx.GetDataSet(strSql).Tables[0].Rows[0]["FIAPIDCOMP"].ToString();

                        if (string.IsNullOrEmpty(compra.IdCompra))
                            throw new Exception();


                        #endregion

                        #region REGISTRO PEDIDOP APP AUTO              

                        strSql = "";
                        strSql = @"INSERT INTO PRODAPPS.APEPANVW
                            (
                            FIAPIDAPPS,
                            FIAPIDCOMP,      /* ID COMPRA    */
                            FIAPIDMARC,      /*ID MARCA */
                            FSAPDESCRI,      /*DESCRIPCION*/
                            FIAPIDCIAU,      /*ID CIA. UNICA*/
                            FIAPIDVEHI,      /*ID VEHICULO  */
                            FIAPIDINVE,      /*ID INVENTARIO*/
                            FSAPMODELO,
                            FSAPVERSIO,
                            FSAPTRANSM,      /*TRANSMISION*/ 
                            FSAPCOLEXT,
                            FSAPNUMINV,

                            FSAPNUMSER,      /*NUMERO SERIE*/ 
                            FDAPSUBTOT,      /*SUBTOTAL     */
                            FDAPDESCUE,      /*DESCUENTO      */ 
                            FDAPIVA,         /*IVA             */
                            FDAPTOTAL,       /*TOTAL      */     
                            FSAPRUTFOT,      /*RUTA FOTO*/
                            FIAPSTATUS,      /*ESTATUS       */  
                            USERCREAT,       /*USUARIO CREACION*/
                            DATECREAT,       /*FECHA CREACION   */
                            TIMECREAT,       /*HORA CREACION    */
                            PROGCREAT,       /*PROGRAMA CREACION*/
                            FDAPPRECIO,      /*PRECIO DEL VALOR DE VEHICULO SIN DESCUENTO*/
                            FDAPAPARTA       /*COSTO APARTADO*/
                            ) VALUES(" +
                        compra.IdApp + "," +
                        compra.IdCompra + "," +
                        compra.IdMarca + "," +
                        "'" + compra.Descripcion + "'," +
                        compra.IdAgencia.ToString().Trim() + "," +
                        (string.IsNullOrEmpty(compra.IdVehiculo) ? "0" : compra.IdVehiculo) + "," +
                        compra.IdInventario + ",'" +
                        compra.Modelo + "','" +
                        compra.Version + "','" +
                        compra.Transmision.ToString().Trim() + "','" +
                        compra.ColorExterior + "'," +
                        "'" + compra.NumeroInventario + "'" + "," +
                        "'" + compra.NumeroDeSerie + "'" + "," +
                        /*compra.SubtotalPedido*/ ((Math.Round(Convert.ToDecimal(compra.TotalPedido), 2)) / (Decimal)1.16).ToString() + ",0," +
                        /*compra.IVAPedido*/ (Math.Round(Convert.ToDecimal(compra.TotalPedido), 2) / (Decimal)1.16 * (Decimal)0.16).ToString() + "," +
                        compra.TotalPedido + "," +
                        "'" + compra.RutaFoto + "'" +
                       "," + 1 + " , " +
                        "'APP' , " +
                        "CURRENT DATE , " +
                        "CURRENT TIME , " +
                        "'APP'," +
                        compra.TotalPedido + "," +
                        (string.IsNullOrEmpty(compra.IdVehiculo) || compra.IdVehiculo  == "0" ? _au.Valorconfigurador("2", compra.IdApp, Math.Round(Convert.ToDecimal(compra.TotalPedido), 2).ToString()) : _au.Valorconfigurador("1", compra.IdApp, Math.Round(Convert.ToDecimal(compra.TotalPedido), 2).ToString())) + ")";
                        dbCnx.SetQuery(strSql);

                        #endregion

                        #region APARTADO

                        strSql = string.Empty;

                        strSql += "UPDATE PRODAUT.ANCAUTOM  SET FNAUTOEST = 50, \t";
                        strSql += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' \t";
                        strSql += "WHERE FNAUTOAGE = " + compra.IdAgencia.ToString().Trim() + " ";
                        strSql += "AND FIANIDINVE=" + compra.IdInventario + "\t";
                        strSql += "AND FIANSTATU = 1 \t";
                        strSql += "AND FNAUTOEST in (6,10) AND FIANAUTMAV = 1";

                        dbCnx.SetQuery(strSql);

                        #endregion

                        #region REGISTRO DETALLE PEDIDO ACCESORIOS

                        List<AccesoriosUOtros> lstAccesorios = new List<AccesoriosUOtros>();
                        if (compra.AccesoriosOtros != null)
                        {
                            foreach (var accesorios in compra.AccesoriosOtros)
                            {

                                strSql = string.Empty;
                                strSql = @"INSERT INTO PRODAPPS.APDPANVW
                                (
                                FIAPIDCOMP,      /*Id Compra*/
                                FIAPIDMARC,      /*Id Marca */
                                FSAPDESCRI,      /* Descripcion*/
                                FIAPIDCIAU,      /*ID CIA. UNICA */
                                FIAPIDCONS,      /*ID CONSECUTIVO*/
                                FSAPCONCEP,      /*CONCEPTO      */
                                FDAPSUBTOT,      /*SUBTOTAL      */
                                FDAPDESCUE,      /*DESCUENTO       */
                                FDAPIVA,         /*IVA             */
                                FDAPTOTAL,       /*TOTAL      */    
                                FSAPRUTFOT,     /*RUTA FOTO*/
                                FIAPSTATUS,     /*ESTATUS       */  
                                USERCREAT,       /*USUARIO CREACION*/
                                DATECREAT,       /*FECHA CREACION   */
                                TIMECREAT,       /*HORA CREACION    */
                                PROGCREAT       /*PROGRAMA CREACION*/
                                ) VALUES 
                                (" +
                                        compra.IdCompra + " , " +
                                        compra.IdMarca + "," +
                                        "'" + compra.Descripcion + "'," +
                                        compra.IdAgencia.ToString().Trim() + " , " +
                                        accesorios.Id + " , " +
                                        "'" + accesorios.Concepto.Replace("'", "") + "'" + " , " +
                                        accesorios.SubTotal.Replace("", "'").Replace(null, "'") + "'" + " , " +
                                        0 + " , " + // DESCUENTO
                                        accesorios.Iva.Replace("'", "").Replace(null, "'") + "'" + " , " +
                                        accesorios.Total.Replace("'", "").Replace(null, "'") + "'" + " , " +
                                        "'" + accesorios.Ruta + "'" + "," +
                                        1 + " , " +
                                        "'APP' , " +
                                        "CURRENT DATE , " +
                                        "CURRENT TIME , " +
                                        "'APP'" + ")";

                                dbCnx.SetQuery(strSql);

                                //}

                            }

                        }

                        #endregion

                        #region REGISTRO SEGUIMIENTO PEDIDO
                        strSql = "";
                        strSql = "SELECT COALESCE(MAX(FIAPIDSEGU),0) + 1 Id ";
                        strSql += "FROM " + constantes.Ambiente + "APPS.APDSGCVW WHERE  FIAPIDCOMP = " + compra.IdCompra + " AND FIAPIDAPPS = " + compra.IdApp;

                        DataTable dtdt = dbCnx.GetDataSet(strSql).Tables[0];
                        if (dtdt.Rows.Count == 1)
                        {
                            int idOut = 0;
                            bool isCorrect = int.TryParse(dtdt.Rows[0]["Id"].ToString(), out idOut);

                            strSql = "";
                            strSql = @"INSERT INTO PRODAPPS.APDSGCVW
                            (
                                FIAPIDCOMP,      /*ID COMPRA     */
                                FIAPIDAPPS,      /*ID APP*/
                                FIAPIDSEGU,      /*ID SEGUIMIENTO*/
                                FSAPTITSEG,     /*TITULO SEGUIM */
                                FIAPIDESTA,      /*ID ESTADO     */
                                FIAPSTATUS,      /*ESTATUS       */
                                USERCREAT,       /*USUARIO CREACION */
                                DATECREAT,       /*FECHA CREACION   */
                                TIMECREAT,       /*HORA CREACION    */
                                PROGCREAT       /*PROGRAMA CREACION*/
                                ) VALUES
                                (" +
                                "(SELECT COALESCE(MAX(FIAPIDCOMP), 0) + 1 Id FROM PRODAPPS.APDSGCVW WHERE FIAPIDAPPS = " + compra.IdApp + ")" + " , " +
                                compra.IdApp + "," +
                                idOut + " , " +
                                "'Registro de orden de compra' , " +
                                1 + " , " +
                                1 + " , " +
                                "'APP' , " +
                                "CURRENT DATE , " +
                                "CURRENT TIME , " +
                                "'APP'" + ")";

                            dbCnx.SetQuery(strSql);
                        }

                        #endregion

                        #region CheckList

                        string strSqlCh = string.Empty;

                        strSqlCh += "INSERT INTO PRODAPPS.APDCKLVW( ";
                        strSqlCh += " FIAPIDCOMP, FIAPIDPROC, FIAPIDPCKL, FSAPDESCCK, FIAPSMARTI, ";
                        strSqlCh += " FIAPAPPVIS, FIAPSISTEM, FIAPREALIZ, FIAPSTATUS, USERCREAT, ";
                        strSqlCh += " DATECREAT, TIMECREAT, PROGCREAT,FIAPIDAPPS)";
                        strSqlCh += " SELECT " + compra.IdCompra + ", FIAPIDPROC, FIAPIDPCKL,  FSAPDESCCK, FIAPSMARTI, ";
                        strSqlCh += " FIAPAPPVIS, FIAPSISTEM,  0, FIAPSTATUS, 'APP', ";
                        strSqlCh += " CURRENT DATE, CURRENT TIME, 'APP' ," + compra.IdApp + " AS FIAPIDAPPS";
                        strSqlCh += " FROM PRODAPPS.APCCKLIS  WHERE FIAPSTATUS = 1";

                        dbCnx.SetQuery(strSqlCh);
                        string strActualizaCh = string.Empty;

                        strActualizaCh += "UPDATE PRODAPPS.APDCKLVW ";
                        strActualizaCh += "SET FIAPREALIZ = 1, ";
                        strActualizaCh += "PROGUPDAT = 'APP', USERUPDAT = 'APP', TIMEUPDAT = CURRENT TIME, DATEUPDAT = CURRENT DATE ";
                        strActualizaCh += "WHERE FIAPIDCOMP = " + compra.IdCompra;
                        strActualizaCh += " AND FIAPIDPCKL = 1" + " AND FIAPIDAPPS= " + compra.IdApp;
                        dbCnx.SetQuery(strActualizaCh);

                        #endregion

                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();
                        respuesta.Ok = "SI";
                        respuesta.Mensaje = "Se registró correctamente.";
                    }
                    else
                    {

                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();
                        respuesta.Mensaje = "No fue posible registrar la solicitud debido a que existe una compra activa.";
                        var cuenta = compra.CuentaUsuario;
                        compra = GetOrdenCompraXIdMarca(long.Parse(compra.CuentaUsuario.IdCuenta), compra.IdMarca, compra.IdApp).Objeto;
                        cuenta.IdCuenta = compra.CuentaUsuario.IdCuenta;
                        compra.CuentaUsuario = cuenta;
                    }
                    respuesta.Objeto = compra;
                }
                catch (Exception)
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();
                    if (respuesta.Mensaje == "")
                    {
                        respuesta.Ok = "NO";
                        respuesta.Mensaje = "No fue posible registrar la solicitud.";
                    }
                }

                #region envia Correo
                if (respuesta.Ok.Equals("SI"))
                {

                    string consultaCorreos = string.Empty;
                    List<string> lstCorreos = new List<string>();
                    string MarcaCarroCorreo = string.Empty;

                    consultaCorreos = $@"SELECT FIAPIDCIAU, FSAPCORREO FROM 
                                        PRODAPPS.APDCRSVW 
                                        WHERE 1=1
                                            AND FIAPIDTIPO = 1
                                            AND FIAPIDAPPS = {compra.IdApp}  
                                            AND FIAPIDCIAU = " + compra.IdAgencia.ToString().Trim();
                    DataTable dtCorreos = dbCnx.GetDataSet(consultaCorreos).Tables[0];

                    if (dtCorreos.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtCorreos.Rows)
                        {
                            string correo = string.Empty;
                            correo = dr["FSAPCORREO"].ToString().Trim();
                            lstCorreos.Add(correo);
                        }
                    }

                    service.EnviarCorreoGerentes("APP", compra.IdCompra, "Se ha registrado un folio nuevo ", "Apártalo", lstCorreos);
                }
                #endregion
                return respuesta;

            });
        }

        internal Task<RespuestaTest<OrdenCompraPedido>> RegistraApartado(OrdenCompraPedido compra)
        {
            return Task.Run(() =>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                RespuestaTest<OrdenCompraPedido> respuesta = new RespuestaTest<OrdenCompraPedido>();
                respuesta.Ok = "NO";
                respuesta.Mensaje = string.Empty;

                string strSql = string.Empty;

                try
                {
                    dbCnx.AbrirConexion();
                    dbCnx.BeginTransaccion();
                    long idCuenta = 0;
                    #region validacionInventarioDisponible

                    strSql = string.Empty;
                    strSql += "SELECT FNAUTOEST FROM PRODAUT.ANCAUTOM ";
                    strSql += "WHERE FNAUTOAGE = " + compra.IdAgencia.ToString().Trim() + " ";
                    strSql += "AND FIANIDINVE=" + compra.IdInventario + "\t";
                    strSql += "AND FIANSTATU = 1 \t";
                    strSql += "AND FNAUTOEST in (6,10) AND FIANAUTMAV = 1";

                    DataTable dtVa = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dtVa.Rows.Count == 0)
                    {
                        respuesta.Ok = "NO";
                        respuesta.Mensaje = "Inventario no disponible.";
                        respuesta.Objeto = null;
                        throw new Exception();
                    }
                    #endregion

                    #region REGISTRA CUENTA

                    strSql = string.Empty;

                    strSql += "SELECT A.FIAPIDCUEN Id, A.FSAPNOMBRE Nombre, A.FSAPAPEPAT ApellidoPaterno, A.FSAPAPEMAT ApellidoMaterno,";
                    strSql += "A.FIAPLADMOV Lada,";
                    strSql += "A.FIAPNUMMOV numero FROM PRODAPPS.APCCTAVW A ";
                    strSql += "INNER JOIN PRODAPPS.APCTOKEN B ";
                    strSql += "ON A.FIAPIDCUEN = B.FIAPIDCUEN ";
                    strSql += "AND A.FIAPIDAPPS = B.FIAPIDAPPS ";
                    strSql += "AND B.FIAPSTATUS = 1 ";
                    strSql += "WHERE lower(TRIM(A.FSAPCORREO)) = " + "'" + compra.CuentaUsuario.Correo.Trim().ToLower() + "'" + " ";
                    strSql += "AND A.FIAPSTATUS = 1 AND A.FIAPIDAPPS = " + compra.IdApp;


                    DataTable dtValidaCuenta = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dtValidaCuenta.Rows.Count > 0)
                    {
                        idCuenta = long.Parse(dtValidaCuenta.Rows[0]["Id"].ToString());
                        compra.CuentaUsuario.IdCuenta = idCuenta.ToString();
                        compra.CuentaUsuario.Nombre = dtValidaCuenta.Rows[0]["Nombre"].ToString().ToUpper().Trim();
                        compra.CuentaUsuario.ApellidoPaterno = dtValidaCuenta.Rows[0]["ApellidoPaterno"].ToString().ToUpper().Trim();
                        compra.CuentaUsuario.ApellidoMaterno = dtValidaCuenta.Rows[0]["ApellidoMaterno"].ToString().ToUpper().Trim();
                        compra.CuentaUsuario.LadaMovil = dtValidaCuenta.Rows[0]["Lada"].ToString().Trim();
                        compra.CuentaUsuario.TelefonoMovil = dtValidaCuenta.Rows[0]["numero"].ToString().Trim();


                        strSql = string.Empty;
                        strSql += "UPDATE " + constantes.Ambiente + "APPS.APCTOKEN ";
                        strSql += " SET FIAPSTATUS = 1, FSAPTOKEN = '" + (!string.IsNullOrEmpty(compra.CuentaUsuario.Token.Trim()) ? compra.CuentaUsuario.Token.Trim() : "") + "', USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FIAPIDCUEN =  " + idCuenta.ToString() + " AND FIAPIDAPPS = " + compra.IdApp;
                        dbCnx.SetQuery(strSql);

                    }
                    else
                    {

                        RespuestaTest<Cuenta> salida = new RespuestaTest<Cuenta>();
                        CuentasUsuarios _user = new CuentasUsuarios();
                        RegistraCuenta cuenta = new RegistraCuenta();

                        cuenta.IdApp = compra.IdApp.ToString();
                        cuenta.Nombre = compra.CuentaUsuario.Nombre.Trim().ToUpper();
                        cuenta.ApellidoPaterno = compra.CuentaUsuario.ApellidoPaterno.Trim().ToUpper();
                        cuenta.ApellidoMaterno = compra.CuentaUsuario.ApellidoMaterno.Trim().ToUpper();
                        cuenta.Correo = compra.CuentaUsuario.Correo.Trim().ToLower();
                        cuenta.Token = compra.CuentaUsuario.Token.Trim();
                        cuenta.LadaMovil = compra.CuentaUsuario.LadaMovil;
                        cuenta.TelefonoMovil = (string.IsNullOrEmpty(compra.CuentaUsuario.TelefonoMovil.ToString().Trim()) ? "5554820300" : compra.CuentaUsuario.TelefonoMovil.ToString().Trim());
                        salida = _user.RegistraCuenta(cuenta);
                        if (salida.Mensaje.Equals("NO"))
                            throw new Exception();
                    }

                    #endregion

                    #region COMPRA ACTIVA 

                    strSql = string.Empty;
                    strSql += "select FIAPIDCOMP Cuenta ";
                    strSql += "FROM " + constantes.Ambiente + "apps.APECMPVW ape ";
                    strSql += "inner join " + constantes.Ambiente + "apps.APCESCVW est ";
                    strSql += "ON ape.FIAPIDESTA = est.FIAPIDESTA ";
                    strSql += "WHERE ape.FIAPSTATUS = 1 AND ape.FIAPIDESTA NOT IN(15,24) AND ape.FIAPIDCUEN = " + compra.CuentaUsuario.IdCuenta + " AND ape.FIAPIDAPPS = " + compra.IdApp; //(1,2,3,4,5,6,7,8,9,10,11,12,13,14,16,17,18,19,20,21,22,23)
                    DataTable dtCount = dbCnx.GetDataSet(strSql).Tables[0];

                    #endregion

                    if (dtCount.Rows.Count == 0)
                    {
                        #region REGISTRO COMPRA

                        strSql = "";
                        strSql = @"SELECT FIAPIDCOMP FROM NEW TABLE ( INSERT INTO PRODAPPS.APECMPVW
                            (
                            FIAPIDAPPS,      /*ID APP*/
                            FIAPIDCOMP,      /*ID COMPRA   */
                            FIAPIDMARC,      /*ID MARCA */
                            FSAPDESCRI,      /*DESCRIPCION*/
                            FIAPFOLCOM,      /* FOLIO COMPRA*/
                            FIAPIDCUEN,      /*ID CUENTA   */
                            FFAPFECCOM,      /*FECHA COMPRA*/
                            FHAPHORCOM,      /*HORA COMPRA */
                            FDAPSUBTOT,      /*SUBTOTAL */
                            FDAPDESCUE,      /*DESCUENTO*/
                            FDAPIVA,         /*IVA      */
                            FDAPTOTAL,       /* TOTAL*/    
                            FIAPIDESTA,      /*ID ESTADO*/
                            FIAPIDPROC,      /*ID PROCESO*/ 
                            FIAPIDPASO,      /*ID PASO*/
                            FIAPSTATUS,      /*ESTATUS     */     
                            USERCREAT,       /*USUARIO CREACION */
                            DATECREAT,       /*FECHA CREACION   */
                            TIMECREAT,       /*HORA CREACION    */
                            PROGCREAT        /*PROGRAMA CREACION*/) VALUES(" +
                        compra.IdApp + ", " +
                        "(SELECT COALESCE(MAX(FIAPIDCOMP), 0) + 1 Id FROM PRODAPPS.APECMPVW WHERE FIAPIDAPPS = " + compra.IdApp + ")" + " , " +
                        compra.IdMarca + "," +
                        "'" + compra.Descripcion + "'," +
                        "(SELECT COALESCE(MAX(FIAPIDCOMP), 0) + 1 Id FROM PRODAPPS.APECMPVW WHERE FIAPIDAPPS = " + compra.IdApp + ")" + " , " +   /*revisar el tema del select max + 1 debe de ser el mismo de compra.idcompra l 741*/
                        compra.CuentaUsuario.IdCuenta + " , " +
                        "CURRENT DATE , " +
                        "CURRENT TIME , " +
                        /*compra.Subtotal*/ ((Math.Round(Convert.ToDecimal(compra.Total), 2)) / (Decimal)1.16).ToString() + " , " + /*se calculca con el total*/
                        0 + " , " + //DESCUENTO
                                    /*compra.IVA*/ (Math.Round(Convert.ToDecimal(compra.Total), 2) / (Decimal)1.16 * (Decimal)0.16).ToString() + " , " + /*se calcula con el total*/
                                                                                                                                            //compra.Total + " , '" +
                                                                                                                                            //compra.RutaReferenciaBancaria + "' , " +
                        compra.Total + " , " + /* recino del json*/
                        1 + " , " + //ID ESTADO
                        2 + " , " + //ID PROCESO
                        1 + " , " + //ID PASO
                        1 + " , " +
                        "'APP' , " +
                        "CURRENT DATE , " +
                        "CURRENT TIME , " +
                        "'APP'" + ")" + ")";

                        compra.IdCompra = dbCnx.GetDataSet(strSql).Tables[0].Rows[0]["FIAPIDCOMP"].ToString();

                        if (string.IsNullOrEmpty(compra.IdCompra))
                            throw new Exception();
                        

                        #endregion

                        #region REGISTRO PEDIDOP APP AUTO              

                        strSql = "";
                        strSql = @"INSERT INTO PRODAPPS.APEPANVW
                            (
                            FIAPIDAPPS,
                            FIAPIDCOMP,     /* ID COMPRA    */
                            FIAPIDMARC,      /*ID MARCA */
                            FSAPDESCRI,      /*DESCRIPCION*/
                            FIAPIDCIAU,      /*ID CIA. UNICA*/
                            FIAPIDVEHI,     /*ID VEHICULO  */
                            FIAPIDINVE,      /*ID INVENTARIO*/
                            FSAPMODELO,
                            FSAPVERSIO,
                            FSAPTRANSM,      /*TRANSMISION*/ 
                            FSAPCOLEXT,
                            FSAPNUMINV,

                            FSAPNUMSER,      /*NUMERO SERIE*/ 
                            FDAPSUBTOT,      /*SUBTOTAL     */
                            FDAPDESCUE,      /*DESCUENTO      */ 
                            FDAPIVA,         /*IVA             */
                            FDAPTOTAL,       /*TOTAL      */     
                            FSAPRUTFOT,    /*RUTA FOTO*/
                            FIAPSTATUS,      /*ESTATUS       */  
                            USERCREAT,       /*USUARIO CREACION*/
                            DATECREAT,       /*FECHA CREACION   */
                            TIMECREAT,       /*HORA CREACION    */
                            PROGCREAT       /*PROGRAMA CREACION*/
                            ) VALUES(" +
                        compra.IdApp + "," +
                        compra.IdCompra + "," +
                        compra.IdMarca + "," +
                        "'" + compra.Descripcion + "'," +
                        compra.IdAgencia.ToString().Trim() + "," +
                        compra.IdVehiculo + "," +
                        compra.IdInventario + ",'" +
                        compra.Modelo + "','" +
                        compra.Version + "','" +
                        compra.Transmision.ToString().Trim() + "','" +
                        compra.ColorExterior + "'," +
                        "'" + compra.NumeroInventario + "'" + "," +
                        "'" + compra.NumeroDeSerie + "'" + "," +
                        /*compra.SubtotalPedido*/ ((Math.Round(Convert.ToDecimal(compra.TotalPedido), 2)) / (Decimal)1.16).ToString() + ",0," +
                        /*compra.IVAPedido*/ (Math.Round(Convert.ToDecimal(compra.TotalPedido), 2) / (Decimal)1.16 * (Decimal)0.16).ToString() + "," +
                        compra.TotalPedido + "," +
                        "'" + compra.RutaFoto + "'" +
                       "," + 1 + " , " +
                        "'APP' , " +
                        "CURRENT DATE , " +
                        "CURRENT TIME , " +
                        "'APP'" + ")";
                        dbCnx.SetQuery(strSql);

                        #endregion

                        #region APARTADO

                        strSql = string.Empty;

                        strSql += "UPDATE PRODAUT.ANCAUTOM  SET FNAUTOEST = 50, \t";
                        strSql += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' \t";
                        strSql += "WHERE FNAUTOAGE = " + compra.IdAgencia.ToString().Trim() + " ";
                        strSql += "AND FIANIDINVE=" + compra.IdInventario + "\t";
                        strSql += "AND FIANSTATU = 1 \t";
                        strSql += "AND FNAUTOEST in (6,10) AND FIANAUTMAV = 1";

                        dbCnx.SetQuery(strSql);

                        #endregion

                        #region REGISTRO DETALLE PEDIDO ACCESORIOS

                        List<AccesoriosUOtros> lstAccesorios = new List<AccesoriosUOtros>();
                        if (compra.AccesoriosOtros != null)
                        {
                            foreach (var accesorios in compra.AccesoriosOtros)
                            {

                                strSql = string.Empty;
                                strSql = @"INSERT INTO PRODAPPS.APDPANVW
                                (
                                FIAPIDCOMP,      /*Id Compra*/
                                FIAPIDMARC,      /*Id Marca */
                                FSAPDESCRI,      /* Descripcion*/
                                FIAPIDCIAU,      /*ID CIA. UNICA */
                                FIAPIDCONS,      /*ID CONSECUTIVO*/
                                FSAPCONCEP,      /*CONCEPTO      */
                                FDAPSUBTOT,      /*SUBTOTAL      */
                                FDAPDESCUE,      /*DESCUENTO       */
                                FDAPIVA,         /*IVA             */
                                FDAPTOTAL,       /*TOTAL      */    
                                FSAPRUTFOT,     /*RUTA FOTO*/
                                FIAPSTATUS,     /*ESTATUS       */  
                                USERCREAT,       /*USUARIO CREACION*/
                                DATECREAT,       /*FECHA CREACION   */
                                TIMECREAT,       /*HORA CREACION    */
                                PROGCREAT       /*PROGRAMA CREACION*/
                                ) VALUES 
                                (" +
                                        compra.IdCompra + " , " +
                                        compra.IdMarca + "," +
                                        "'" + compra.Descripcion + "'," +
                                        compra.IdAgencia.ToString().Trim() + " , " +
                                        accesorios.Id + " , " +
                                        "'" + accesorios.Concepto.Replace("'", "") + "'" + " , " +
                                        accesorios.SubTotal.Replace("", "'").Replace(null, "'") + "'" + " , " +
                                        0 + " , " + // DESCUENTO
                                        accesorios.Iva.Replace("'", "").Replace(null, "'") + "'" + " , " +
                                        accesorios.Total.Replace("'", "").Replace(null, "'") + "'" + " , " +
                                        "'" + accesorios.Ruta + "'" + "," +
                                        1 + " , " +
                                        "'APP' , " +
                                        "CURRENT DATE , " +
                                        "CURRENT TIME , " +
                                        "'APP'" + ")";

                                dbCnx.SetQuery(strSql);

                                //}

                            }

                        }

                        #endregion

                        #region REGISTRO SEGUIMIENTO PEDIDO
                        strSql = "";
                        strSql = "SELECT COALESCE(MAX(FIAPIDSEGU),0) + 1 Id ";
                        strSql += "FROM " + constantes.Ambiente + "APPS.APDSGCVW WHERE  FIAPIDCOMP = " + compra.IdCompra + " AND FIAPIDAPPS = " + compra.IdApp;

                        DataTable dtdt = dbCnx.GetDataSet(strSql).Tables[0];
                        if (dtdt.Rows.Count == 1)
                        {
                            int idOut = 0;
                            bool isCorrect = int.TryParse(dtdt.Rows[0]["Id"].ToString(), out idOut);

                            strSql = "";
                            strSql = @"INSERT INTO PRODAPPS.APDSGCVW
                            (
                                FIAPIDCOMP,      /*ID COMPRA     */
                                FIAPIDAPPS,      /*ID APP*/
                                FIAPIDSEGU,      /*ID SEGUIMIENTO*/
                                FSAPTITSEG,     /*TITULO SEGUIM */
                                FIAPIDESTA,      /*ID ESTADO     */
                                FIAPSTATUS,      /*ESTATUS       */
                                USERCREAT,       /*USUARIO CREACION */
                                DATECREAT,       /*FECHA CREACION   */
                                TIMECREAT,       /*HORA CREACION    */
                                PROGCREAT       /*PROGRAMA CREACION*/
                                ) VALUES
                                (" +
                                "(SELECT COALESCE(MAX(FIAPIDCOMP), 0) + 1 Id FROM PRODAPPS.APDSGCVW WHERE FIAPIDAPPS = " + compra.IdApp + ")" + " , " +
                                compra.IdApp + "," +
                                idOut + " , " +
                                "'Registro de orden de compra' , " +
                                1 + " , " +
                                1 + " , " +
                                "'APP' , " +
                                "CURRENT DATE , " +
                                "CURRENT TIME , " +
                                "'APP'" + ")";

                            dbCnx.SetQuery(strSql);
                        }

                        #endregion

                        #region CheckList

                        string strSqlCh = string.Empty;

                        strSqlCh += "INSERT INTO PRODAPPS.APDCKLVW( ";
                        strSqlCh += " FIAPIDCOMP, FIAPIDPROC, FIAPIDPCKL, FSAPDESCCK, FIAPSMARTI, ";
                        strSqlCh += " FIAPAPPVIS, FIAPSISTEM, FIAPREALIZ, FIAPSTATUS, USERCREAT, ";
                        strSqlCh += " DATECREAT, TIMECREAT, PROGCREAT,FIAPIDAPPS)";
                        strSqlCh += " SELECT " + compra.IdCompra + ", FIAPIDPROC, FIAPIDPCKL,  FSAPDESCCK, FIAPSMARTI, ";
                        strSqlCh += " FIAPAPPVIS, FIAPSISTEM,  0, FIAPSTATUS, 'APP', ";
                        strSqlCh += " CURRENT DATE, CURRENT TIME, 'APP' ," + compra.IdApp + " AS FIAPIDAPPS";
                        strSqlCh += " FROM PRODAPPS.APCCKLIS  WHERE FIAPSTATUS = 1";

                        dbCnx.SetQuery(strSqlCh);
                        string strActualizaCh = string.Empty;

                        strActualizaCh += "UPDATE PRODAPPS.APDCKLVW ";
                        strActualizaCh += "SET FIAPREALIZ = 1, ";
                        strActualizaCh += "PROGUPDAT = 'APP', USERUPDAT = 'APP', TIMEUPDAT = CURRENT TIME, DATEUPDAT = CURRENT DATE ";
                        strActualizaCh += "WHERE FIAPIDCOMP = " + compra.IdCompra;
                        strActualizaCh += " AND FIAPIDPCKL = 1" + " AND FIAPIDAPPS= " + compra.IdApp;
                        dbCnx.SetQuery(strActualizaCh);

                        #endregion

                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();
                        respuesta.Ok = "SI";
                        respuesta.Mensaje = "Se registró correctamente.";
                    }
                    else
                    {

                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();
                        respuesta.Mensaje = "No fue posible registrar la solicitud debido a que existe una compra activa.";
                        var cuenta = compra.CuentaUsuario;
                        compra = GetOrdenCompraXIdMarca(long.Parse(compra.CuentaUsuario.IdCuenta), compra.IdMarca, compra.IdApp).Objeto;
                        cuenta.IdCuenta = compra.CuentaUsuario.IdCuenta;
                        compra.CuentaUsuario = cuenta;
                    }
                    respuesta.Objeto = compra;
                }
                catch (Exception)
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();
                    if (respuesta.Mensaje == "")
                    {
                        respuesta.Ok = "NO";
                        respuesta.Mensaje = "No fue posible registrar la solicitud.";
                    }
                }

                #region envia Correo
                if (respuesta.Ok.Equals("SI"))
                {

                    string consultaCorreos = string.Empty;
                    List<string> lstCorreos = new List<string>();
                    string MarcaCarroCorreo = string.Empty;

                    consultaCorreos = $@"SELECT FIAPIDCIAU, FSAPCORREO FROM 
                                        PRODAPPS.APDCRSVW 
                                        WHERE 1=1
                                            AND FIAPIDTIPO = 1
                                            AND FIAPIDAPPS = {compra.IdApp}  
                                            AND FIAPIDCIAU = " + compra.IdAgencia.ToString().Trim();
                    DataTable dtCorreos = dbCnx.GetDataSet(consultaCorreos).Tables[0];

                    if (dtCorreos.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtCorreos.Rows)
                        {
                            string correo = string.Empty;
                            correo = dr["FSAPCORREO"].ToString().Trim();
                            lstCorreos.Add(correo);
                        }
                    }

                    string subject = "APP";
                    EnvioCorreoSoporte hiloEnvioCorreoAGerentesYBack = new EnvioCorreoSoporte(subject, compra.IdCompra, "Se ha registrado un folio nuevo ", "Apártalo", lstCorreos);
                    hiloEnvioCorreoAGerentesYBack.EnvioCorreoGerentes();
                }
                #endregion
                return respuesta;

            });

        }

        internal Task<Respuesta> CancelaOrdenCompra(long aIdCompra, string aIdApps)
        {
            return Task.Run(() =>
            {
                Respuesta respuesta = new Respuesta();
                DVADB.DB2 dbCnx = new DVADB.DB2();
                try
                {
                    dbCnx.AbrirConexion();
                    dbCnx.BeginTransaccion();
                    string strSql = string.Empty;

                    strSql = @"SELECT  pedan.FIAPIDINVE, pedan.FIAPIDPEDI, factu.FSCAIDEDO, factu.FFCAFECHA, factu.FDCASUBTOT, factu.FICAIDCIAU, " +
                                "factu.FICAIDCLIE, factu.FCCAPREIN, factu.FICAFOLIN FROM PRODAPPS.APEPANVW pedan " +
                                "INNER JOIN PRODCAJA.CAEFACAN facan ON pedan.FIAPIDCIAU = facan.FICAIDCIAU AND pedan.FIAPIDPEDI = facan.FICAIDPEDI " +
                                "INNER JOIN PRODCAJA.CAEFACTU factu ON facan.FICAIDCIAU = factu.FICAIDCIAU AND facan.FICAIDFACT = factu.FICAIDFACT " +
                                "WHERE pedan.FIAPSTATUS = 1  AND pedan.FIAPIDCOMP = " + aIdCompra.ToString() + " AND pedan.FIAPIDAPPS = " + aIdApps.ToString();
                    DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        respuesta.Ok = "NO";
                        respuesta.Mensaje = "No se puede cancelar la orden de compra porque está facturada.";
                        respuesta.Objeto = "";
                    }
                    else
                    {
                        string consultaPed = string.Empty;
                        consultaPed = @"SELECT FIAPIDINVE, FIAPIDPEDI, FIAPIDCIAU " +
                                        "FROM PRODAPPS.APEPANVW " +
                                        "WHERE FIAPSTATUS = 1 " + 
                                        "AND FIAPIDCOMP = " + aIdCompra.ToString() + " AND FIAPIDAPPS = " + aIdApps.ToString();

                        DataTable dtPed = dbCnx.GetDataSet(consultaPed).Tables[0];
                        if (dtPed.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtPed.Rows)
                            {

                                string IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                                string IdPedido = dr["FIAPIDPEDI"].ToString().Trim();
                                string IdInventario = dr["FIAPIDINVE"].ToString().Trim();

                                strSql = string.Empty;
                                strSql = "UPDATE PRODAPPS.APECMPVW SET FIAPIDESTA = 15, USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP'   WHERE FIAPIDCOMP = " + aIdCompra + " AND FIAPIDAPPS= " + aIdApps;
                                dbCnx.SetQuery(strSql);

                                if (Convert.ToUInt64(IdPedido) > 0)
                                {

                                    strSql = string.Empty;
                                    strSql += "UPDATE PRODAUT.ANEPEDAU SET FIANPASTP = 4, USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FNANPAAGE = " + IdAgencia + " AND FNANPAIDE = " + IdPedido;
                                    dbCnx.SetQuery(strSql);
                                }

                                strSql = string.Empty;
                                strSql = @"UPDATE PRODAUT.ANCAUTOM SET FNAUTOEST = 10, USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FNAUTOEST IN (11, 50) AND FNAUTOAGE = " + IdAgencia + " AND FIANIDINVE = " + IdInventario ;
                                dbCnx.SetQuery(strSql);

                                strSql = string.Empty; 
                                strSql = @"INSERT INTO PRODAPPS.APDSGCVW " +
                                "(FIAPIDCOMP, FIAPIDSEGU, FSAPTITSEG, FIAPIDESTA, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT,FIAPIDAPPS)" +
                                "VALUES (" +
                                aIdCompra + ", " +
                                "(SELECT coalesce(MAX(FIAPIDSEGU),0)+1 ID FROM PRODAPPS.APDSGCVW WHERE FIAPIDCOMP = " + aIdCompra + " AND FIAPIDAPPS = " + aIdApps  + ")," +
                                "'Movimiento generado en Servicios' ," +
                                "15," +
                                "1, 'APPS' ,CURRENT DATE, CURRENT TIME, 'APP'" +",'" + aIdApps + "'"+
                                ")";
                                dbCnx.SetQuery(strSql);
                            }

                            respuesta.Ok = "SI";
                            respuesta.Mensaje = "Se canceló de forma satisfactoria la orden de compra.";
                            respuesta.Objeto = "";
                        }
                        else
                        {
                            respuesta.Ok = "NO";
                            respuesta.Mensaje = "No se encontró la orden de compra.";
                            respuesta.Objeto = "";
                        }
                    }


                    dbCnx.CommitTransaccion();
                    dbCnx.CerrarConexion();
                }
                catch (Exception)
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();
                    respuesta = new Respuesta();
                }
                return respuesta;
            });
        }

        public string Valorconfigurador(string IdTipoVehiculo,string IdApps, string TotalVehiculo)
        {
            try
            {
                string sql = string.Empty;
                string valor = string.Empty;
                string TipoDescuento = string.Empty;
                DVADB.DB2 dbCnx = new DVADB.DB2();
                sql = $@"SELECT * FROM PRODAPPS.APDAPART 
                    WHERE FIAPIDAPPS = {IdApps} AND FIAPTIPOAU = {IdTipoVehiculo}";
                valor = dbCnx.GetDataSet(sql).Tables[0].Rows[0]["FDAPVALOR"].ToString().Trim();
                TipoDescuento = dbCnx.GetDataSet(sql).Tables[0].Rows[0]["FIATIPODES"].ToString().Trim();
                if (IdTipoVehiculo == "1")
                {
                    if (TipoDescuento == "1")
                        valor = Math.Round((Convert.ToDecimal(TotalVehiculo) * (Convert.ToDecimal(valor))) / 100, 2).ToString();
                }
                else
                {
                    if (TipoDescuento == "1")
                        valor = Math.Round((Convert.ToDecimal(TotalVehiculo) * (Convert.ToDecimal(valor))) / 100, 2).ToString();
                }
                return valor;
            }
            catch (Exception)
            {
                return "";
            }
        }

        internal RespuestaTest<CompraVehiculo> GetCompraVehiculoXIdMarca(long aIdCuenta, string aIdMarca, string aIdApps)
        {
            RespuestaTest<CompraVehiculo> respuesta = new RespuestaTest<CompraVehiculo>();
            CompraVehiculo compra = new CompraVehiculo();
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                string strlSQL = string.Empty;
                strlSQL = $@"SELECT ape.FIAPIDCOMP IDCOMPRA, 
                                    ape.FIAPIDAPPS,
                                    ape.FIAPIDMARC, 
                                    ape.FIAPFOLCOM, 
                                    ape.FIAPIDCUEN IDCUENTA, 
                                    ape.FFAPFECCOM, 
                                    ape.FHAPHORCOM, 
                                    ape.FDAPSUBTOT SUBTOTAL, 
                                    ape.FDAPDESCUE DESCUENTO, 
                                    ape.FDAPIVA IVA, 
                                    ape.FDAPTOTAL TOTAL, 
                                    ape.FSAPRUTRFB, 
                                    ape.FIAPIDESTA,  
                                    est.FSAPESTADO, 
                                    ape.FIAPIDPROC, 
                                    ape.FIAPIDPASO PASO, 
                                    dan.FIAPIDCIAU, 
                                    dan.FIAPIDPEDI, 
                                    dan.FFAPFECPED, 
                                    dan.FHAPHORPED, 
                                    dan.FIAPIDPERS, 
                                    dan.FIAPIDVEHI, 
                                    dan.FIAPIDINVE, 
                                    dan.FSAPMODELO, 
                                    dan.FSAPVERSIO, 
                                    dan.FSAPTRANSM ,
                                    dan.FSAPCOLEXT, 
                                    dan.FSAPNUMINV,
                                    dan.FSAPNUMSER, 
                                    dan.FDAPSUBTOT SUBTOTALPEDIDO, 
                                    dan.FDAPDESCUE, 
                                    dan.FDAPIVA IVAPEDIDO, 
                                    dan.FDAPTOTAL TOTALPEDIDO, 
                                    dan.FIAPCOTSEG, 
                                    dan.FSAPRUTFOT,
                                    dan.FDAPAPARTA,
                                    dan.FDAPPRECIO PRECIOINICIAL
                                    FROM PRODapps.APECMPVW ape  
                           INNER JOIN PRODapps.APCESCVW est ON ape.FIAPIDESTA = est.FIAPIDESTA  
                           INNER JOIN PRODapps.APEPANVW dan ON dan.FIAPIDCOMP = ape.FIAPIDCOMP  
                           AND ape.FIAPIDAPPS =  dan.FIAPIDAPPS  
                           AND  dan.FIAPIDMARC = ape.FIAPIDMARC  
                           WHERE 1 = 1  
                           AND ape.FIAPSTATUS = 1 
                           AND ape.FIAPIDESTA NOT IN(15,24) 
                           AND ape.FIAPIDCUEN =   {aIdCuenta}
                           AND ape.FIAPIDMARC =   {aIdMarca}   
                           AND ape.FIAPIDAPPS =   {aIdApps}   
                           ORDER BY ape.FIAPIDCOMP ASC";
                DataTable dt = dbCnx.GetDataSet(strlSQL).Tables[0];

                if (dt.Rows.Count == 0)
                    throw new Exception();

                foreach (DataRow dr in dt.Rows)
                {
                    compra = new CompraVehiculo();
                    compra.IdApp = dr["FIAPIDAPPS"].ToString().Trim();
                    compra.IdCompra = dr["IDCOMPRA"].ToString().Trim();
                    compra.IdMarca = dr["FIAPIDMARC"].ToString().Trim();
                    compra.CuentaUsuario = new Cuenta
                    {
                        IdCuenta = dr["IDCUENTA"].ToString().Trim(),
                        IdPersona = dr["FIAPIDPERS"].ToString().Trim()
                    };

                    //Precio incial del vehiculo
                    compra.PrecioInicial = Math.Round(Convert.ToDecimal(dr["PRECIOINICIAL"].ToString().Trim()),2).ToString();
                    compra.IdPaso = dr["PASO"].ToString().Trim();
                    compra.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                    compra.IdVehiculo = dr["FIAPIDVEHI"].ToString().Trim();
                    compra.IdInventario = dr["FIAPIDINVE"].ToString().Trim();
                    compra.NumeroDeSerie = dr["FSAPNUMSER"].ToString().Trim();
                    compra.TotalPedido = Math.Round(Convert.ToDecimal(dr["TOTALPEDIDO"].ToString().Trim()), 2).ToString();
                    compra.SubtotalPedido = dr["SUBTOTALPEDIDO"].ToString().Trim();
                    compra.IVAPedido = dr["IVAPEDIDO"].ToString().Trim();
                    compra.Modelo = dr["FSAPMODELO"].ToString().Trim();
                    compra.Version = dr["FSAPVERSIO"].ToString().Trim();
                    compra.ColorExterior = dr["FSAPCOLEXT"].ToString().Trim();
                    compra.NumeroInventario = dr["FSAPNUMINV"].ToString().Trim();
                    compra.RutaFoto = dr["FSAPRUTFOT"].ToString().Trim();
                    compra.Transmision = dr["FSAPTRANSM"].ToString().Trim();
                    compra.Descripcion = dr["FSAPESTADO"].ToString().Trim();
                    compra.TipoVehiculo = dr["FIAPIDVEHI"].ToString().Trim() == "0" ? TipoVehiculo(2) : TipoVehiculo(1); //Id 2 significa vehiculo configurado, por ende 1 representa Existencia
                    compra.TotalApartado = dr["FDAPAPARTA"].ToString().Trim();
                    compra.TotalDescuentos = dr["DESCUENTO"].ToString().Trim();
                    //proceso de compra
                    compra.PrecioALiquidar = (Math.Round(Convert.ToDecimal(compra.PrecioInicial) - Convert.ToDecimal(compra.TotalApartado) - Convert.ToDecimal(compra.TotalDescuentos),2)).ToString();
                    compra.Total = dr["TOTAL"].ToString().Trim();
                    compra.Subtotal = dr["SUBTOTAL"].ToString().Trim();
                    compra.Descuento = dr["DESCUENTO"].ToString().Trim();
                    compra.IVA = dr["IVA"].ToString().Trim();

                }
                return new RespuestaTest<CompraVehiculo>()
                {
                    Ok = "SI",
                    Mensaje = "",
                    Objeto = compra
                };
            }
            catch (Exception)
            {
                return new RespuestaTest<CompraVehiculo>()
                {
                    Ok = "No",
                    Mensaje = string.Empty,
                    Objeto = null
                };
            }
        }

        public string TotalDescuentos(string TotalVehiculo, string TotalApartado, string IdApps, string Idcompra,string Idcuenta)
        {
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                string sql = string.Empty;
                string TotalDescuentos = Math.Round(Convert.ToDecimal(TotalVehiculo), 2).ToString(); //Math.Round(Convert.ToDecimal(TotalVehiculo) - Convert.ToDecimal(TotalApartado), 2).ToString(); ;
                sql = $@"SELECT * FROM PRODAPPS.APCPRERC
                    WHERE 1=1
                    AND FIAPIDAPPS = {IdApps}
                    AND FIAPIDCUEN = {Idcuenta}
                    AND FIAPIDCOMP = {Idcompra}
                    AND FIAPIDESTA NOT IN(0,2,3)
                    ORDER BY FIAPIDCONS ASC";
                DataTable dt = dbCnx.GetDataSet(sql).Tables[0];
                
                foreach (DataRow dr in dt.Rows)
                {
                     if (dr["FIAPTPDESC"].ToString().Trim() == "1") //representa el valor en porcentaje
                        TotalDescuentos = Math.Round(Convert.ToDecimal(TotalDescuentos) - (Convert.ToDecimal(TotalDescuentos) * (Convert.ToDecimal(dr["FIAVALORDC"].ToString().Trim()))) / 100, 2).ToString();
                    else
                        TotalDescuentos = Math.Round(Convert.ToDecimal(TotalDescuentos) - Convert.ToDecimal(dr["FIAVALORDC"].ToString().Trim()),2).ToString();
                }

                return Math.Round(Convert.ToDecimal(TotalVehiculo) - Convert.ToDecimal(TotalDescuentos), 2).ToString();
            }
            catch (Exception)
            {
                return "0";
            }
        }

        public string TipoVehiculo(int IdTipo)
        {
            List<TipoVehiculoAudi> _coleccionVehiculo = new List<TipoVehiculoAudi>();
            _coleccionVehiculo = JsonConvert.DeserializeObject
                <List<TipoVehiculoAudi>>
                (File.ReadAllText(
                    (Ruta + "RecursosAudi\\TipoVehiculo.json")));
            return _coleccionVehiculo.Where(x => x.Id == IdTipo.ToString()).FirstOrDefault().Id;
        }

        internal RespuestaTest<OrdenCompraPedido> GetOrdenCompraXIdMarca(long aIdCuenta , string aIdMarca, string aIdApps)
        {
            RespuestaTest<OrdenCompraPedido> respuesta = new RespuestaTest<OrdenCompraPedido>();
            OrdenCompraPedido compra = new OrdenCompraPedido();
            List<Accesorio> _coleccionaccesorio = new List<Accesorio>();
            Accesorio _accesorio = new Accesorio();
            DataTable dtA;
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                
                string strlSQL = string.Empty;
                strlSQL = @"SELECT ape.FIAPIDCOMP IDCOMPRA, ape.FIAPIDAPPS,ape.FIAPIDMARC, ape.FIAPFOLCOM, ape.FIAPIDCUEN IDCUENTA, ape.FFAPFECCOM, ape.FHAPHORCOM, ape.FDAPSUBTOT SUBTOTAL, ape.FDAPDESCUE DESCUENTO, ape.FDAPIVA IVA, ape.FDAPTOTAL TOTAL, ape.FSAPRUTRFB, ape.FIAPIDESTA,  est.FSAPESTADO, ape.FIAPIDPROC, ape.FIAPIDPASO PASO, dan.FIAPIDCIAU, dan.FIAPIDPEDI, dan.FFAPFECPED, dan.FHAPHORPED, dan.FIAPIDPERS, dan.FIAPIDVEHI, dan.FIAPIDINVE, dan.FSAPMODELO, dan.FSAPVERSIO, dan.FSAPTRANSM ,dan.FSAPCOLEXT, dan.FSAPNUMINV,dan.FSAPNUMSER, dan.FDAPSUBTOT SUBTOTALPEDIDO, dan.FDAPDESCUE, dan.FDAPIVA IVAPEDIDO, dan.FDAPTOTAL TOTALPEDIDO, dan.FIAPCOTSEG, dan.FSAPRUTFOT FROM PRODapps.APECMPVW ape " +
                           "INNER JOIN PRODapps.APCESCVW est ON ape.FIAPIDESTA = est.FIAPIDESTA " +
                           "INNER JOIN PRODapps.APEPANVW dan ON dan.FIAPIDCOMP = ape.FIAPIDCOMP " +
                           "AND ape.FIAPIDAPPS =  dan.FIAPIDAPPS " +
                           "AND  dan.FIAPIDMARC = ape.FIAPIDMARC " +
                           "WHERE 1 = 1 " +
                           "AND ape.FIAPSTATUS = 1 AND ape.FIAPIDESTA NOT IN(15,24) AND ape.FIAPIDCUEN = " + aIdCuenta + " AND ape.FIAPIDMARC = " + aIdMarca + " AND ape.FIAPIDAPPS = " + aIdApps + " ORDER BY ape.FIAPIDCOMP ASC";
                DataTable dt = dbCnx.GetDataSet(strlSQL).Tables[0];

                if (dt.Rows.Count == 0)
                    throw new Exception();

                foreach (DataRow dr in dt.Rows)
                {
                    compra = new OrdenCompraPedido();
                    compra.IdApp = dr["FIAPIDAPPS"].ToString().Trim();
                    compra.IdCompra = dr["IDCOMPRA"].ToString().Trim();
                    compra.IdMarca = dr["FIAPIDMARC"].ToString().Trim();
                    compra.CuentaUsuario = new Cuenta
                    {
                        IdCuenta = dr["IDCUENTA"].ToString().Trim(),
                        IdPersona = dr["FIAPIDPERS"].ToString().Trim()
                    };
                    compra.Subtotal = dr["SUBTOTAL"].ToString().Trim(); 
                    compra.Descuento = dr["DESCUENTO"].ToString().Trim();
                    compra.IVA = dr["IVA"].ToString().Trim();
                    compra.Total = dr["TOTAL"].ToString().Trim();
                    compra.IdPaso = dr["PASO"].ToString().Trim();
                    compra.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                    compra.IdVehiculo = dr["FIAPIDVEHI"].ToString().Trim();
                    compra.IdInventario = dr["FIAPIDINVE"].ToString().Trim();
                    compra.NumeroDeSerie = dr["FSAPNUMSER"].ToString().Trim();
                    compra.SubtotalPedido = dr["SUBTOTALPEDIDO"].ToString().Trim();
                    compra.IVAPedido = dr["IVAPEDIDO"].ToString().Trim();
                    compra.TotalPedido = dr["TOTALPEDIDO"].ToString().Trim();
                    compra.Modelo = dr["FSAPMODELO"].ToString().Trim();
                    compra.Version = dr["FSAPVERSIO"].ToString().Trim();
                    compra.ColorExterior = dr["FSAPCOLEXT"].ToString().Trim();
                    compra.NumeroInventario = dr["FSAPNUMINV"].ToString().Trim();
                    compra.RutaFoto = dr["FSAPRUTFOT"].ToString().Trim();
                    compra.Transmision = dr["FSAPTRANSM"].ToString().Trim();
                    compra.Descripcion =  dr["FSAPESTADO"].ToString().Trim();

                    strlSQL = string.Empty;
                    strlSQL = @"SELECT dedan.FIAPIDCIAU, dedan.FIAPIDPEDI, dedan.FIAPIDCONS, dedan.FSAPCONCEP, dedan.FDAPSUBTOT, dedan.FDAPDESCUE, dedan.FDAPIVA, dedan.FDAPTOTAL, dedan.FSAPRUTFOT " +
                               "FROM " + constantes.Ambiente + "apps.APDPANVW dedan " +
                               "WHERE dedan.FIAPSTATUS=1 " +
                               "AND dedan.FIAPIDCIAU = " + compra.IdAgencia.ToString() +
                               " AND dedan.FIAPIDCOMP =" + compra.IdCompra.ToString();
                    dtA = dbCnx.GetDataSet(strlSQL).Tables[0];
                    if (dtA.Rows.Count > 0)
                    {
                        _coleccionaccesorio = new List<Accesorio>();
                        foreach (DataRow drA in dtA.Rows)
                        {
                            _accesorio = new Accesorio();
                            _accesorio.Concepto = drA["FSAPCONCEP"].ToString().Trim();
                            _accesorio.SubTotal = drA["FDAPSUBTOT"].ToString().Trim();
                            _accesorio.Iva = drA["FDAPIVA"].ToString().Trim();
                            _accesorio.Total = drA["FDAPTOTAL"].ToString().Trim();
                            _accesorio.Ruta = drA["FSAPRUTFOT"].ToString().Trim();
                            _coleccionaccesorio.Add(_accesorio);
                        }
                    }
                    else
                    {
                        _coleccionaccesorio = new List<Accesorio>();
                    }
                    compra.AccesoriosOtros = _coleccionaccesorio;
                    respuesta.Ok = "SI";
                    respuesta.Mensaje = string.Empty;
                    respuesta.Objeto = compra;
                      
                }
            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = string.Empty;
                respuesta.Objeto = null;
            }

            return respuesta;
        }

        internal OrdenCompraPedido GetChecarEstatusCompra(long aIdCompra, string aIdApps)
        {
            OrdenCompraPedido compra = new OrdenCompraPedido();
            List<Accesorio> _coleccionaccesorio = new List<Accesorio>();
            Accesorio _accesorio = new Accesorio();
            DataTable dtA;
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();

                string strlSQL = string.Empty;
                strlSQL = @"SELECT ape.FIAPIDCOMP IDCOMPRA, ape.FIAPIDAPPS ,ape.FIAPIDMARC, ape.FIAPFOLCOM, ape.FIAPIDCUEN IDCUENTA, ape.FFAPFECCOM, ape.FHAPHORCOM, ape.FDAPSUBTOT SUBTOTAL, ape.FDAPDESCUE DESCUENTO, ape.FDAPIVA IVA, ape.FDAPTOTAL TOTAL, ape.FSAPRUTRFB, ape.FIAPIDESTA,  est.FSAPESTADO, ape.FIAPIDPROC, ape.FIAPIDPASO PASO, dan.FIAPIDCIAU, dan.FIAPIDPEDI, dan.FFAPFECPED, dan.FHAPHORPED, dan.FIAPIDPERS, dan.FIAPIDVEHI, dan.FIAPIDINVE, dan.FSAPMODELO, dan.FSAPVERSIO, dan.FSAPTRANSM ,dan.FSAPCOLEXT, dan.FSAPNUMINV,dan.FSAPNUMSER, dan.FDAPSUBTOT SUBTOTALPEDIDO, dan.FDAPDESCUE, dan.FDAPIVA IVAPEDIDO, dan.FDAPTOTAL TOTALPEDIDO, dan.FIAPCOTSEG, dan.FSAPRUTFOT FROM PRODapps.APECMPVW ape  
                          INNER JOIN PRODapps.APCESCVW est ON ape.FIAPIDESTA = est.FIAPIDESTA 
                          INNER JOIN PRODapps.APEPANVW dan ON dan.FIAPIDCOMP = ape.FIAPIDCOMP
                          AND  dan.FIAPIDAPPS = ape.FIAPIDAPPS 
                          WHERE 1 = 1 
                          AND ape.FIAPSTATUS = 1 AND ape.FIAPIDCOMP = " + aIdCompra + " AND ape.FIAPIDAPPS =" + aIdApps + "  ORDER BY ape.FIAPIDCOMP ASC";
                DataTable dt = dbCnx.GetDataSet(strlSQL).Tables[0];
                if (dt.Rows.Count == 0)
                    throw new Exception();

                foreach (DataRow dr in dt.Rows)
                {
                    compra = new OrdenCompraPedido();
                    compra.IdApp = dr["FIAPIDAPPS"].ToString().Trim();
                    compra.IdCompra = dr["IDCOMPRA"].ToString().Trim();
                    compra.IdMarca = dr["FIAPIDMARC"].ToString().Trim();
                    compra.CuentaUsuario = new Cuenta
                    {
                        IdCuenta = dr["IDCUENTA"].ToString().Trim(),
                        IdPersona = dr["FIAPIDPERS"].ToString().Trim()
                    };
                    compra.Subtotal = dr["SUBTOTAL"].ToString().Trim();
                    compra.Descuento = dr["DESCUENTO"].ToString().Trim();
                    compra.IVA = dr["IVA"].ToString().Trim();
                    compra.Total = dr["TOTAL"].ToString().Trim();
                    compra.IdPaso = dr["PASO"].ToString().Trim();
                    compra.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                    compra.IdVehiculo = dr["FIAPIDVEHI"].ToString().Trim();
                    compra.IdInventario = dr["FIAPIDINVE"].ToString().Trim();
                    compra.NumeroDeSerie = dr["FSAPNUMSER"].ToString().Trim();
                    compra.SubtotalPedido = dr["SUBTOTALPEDIDO"].ToString().Trim();
                    compra.IVAPedido = dr["IVAPEDIDO"].ToString().Trim();
                    compra.TotalPedido = dr["TOTALPEDIDO"].ToString().Trim();
                    compra.Modelo = dr["FSAPMODELO"].ToString().Trim();
                    compra.Version = dr["FSAPVERSIO"].ToString().Trim();
                    compra.ColorExterior = dr["FSAPCOLEXT"].ToString().Trim();
                    compra.NumeroInventario = dr["FSAPNUMINV"].ToString().Trim();
                    compra.RutaFoto = dr["FSAPRUTFOT"].ToString().Trim();
                    compra.Transmision = dr["FSAPTRANSM"].ToString().Trim();
                    compra.Descripcion = dr["FSAPESTADO"].ToString().Trim();

                    strlSQL = string.Empty;
                    strlSQL = $@"SELECT dedan.FIAPIDCIAU, dedan.FIAPIDPEDI, dedan.FIAPIDCONS, dedan.FSAPCONCEP, dedan.FDAPSUBTOT, dedan.FDAPDESCUE, dedan.FDAPIVA, dedan.FDAPTOTAL, dedan.FSAPRUTFOT 
                               FROM Prodapps.APDPANVW dedan 
                               WHERE dedan.FIAPSTATUS=1
                               AND dedan.FIAPIDCIAU =  {compra.IdAgencia.ToString()}
                               AND dedan.FIAPIDCOMP = {compra.IdCompra.ToString()}
                               AND dedan.FIAPIDAPPS = " + aIdApps;
                    dtA = dbCnx.GetDataSet(strlSQL).Tables[0];
                    if (dtA.Rows.Count > 0)
                    {
                        _coleccionaccesorio = new List<Accesorio>();
                        foreach (DataRow drA in dtA.Rows)
                        {
                            _accesorio = new Accesorio();
                            _accesorio.Concepto = drA["FSAPCONCEP"].ToString().Trim();
                            _accesorio.SubTotal = drA["FDAPSUBTOT"].ToString().Trim();
                            _accesorio.Iva = drA["FDAPIVA"].ToString().Trim();
                            _accesorio.Total = drA["FDAPTOTAL"].ToString().Trim();
                            _accesorio.Ruta = drA["FSAPRUTFOT"].ToString().Trim();
                            _coleccionaccesorio.Add(_accesorio);
                        }
                    }
                    else
                    {
                        _coleccionaccesorio = new List<Accesorio>();
                    }
                    compra.AccesoriosOtros = _coleccionaccesorio;
                }

            }
            catch (Exception ex)
            {
                compra = new OrdenCompraPedido();
            }
            return compra;
        }
    }
}