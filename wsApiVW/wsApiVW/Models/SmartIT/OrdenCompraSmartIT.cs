using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Bussine;
using wsApiVW.Controllers;
using wsApiVW.Models.Aplicaciones;
using wsApiVW.Models.Audi;
using wsApiVW.Models.AutoModels;
using wsApiVW.Models.User;
using wsApiVW.Services;

namespace wsApiVW.Models.SmartIT
{
    public class OrdenCompraSmartIT
    {
        public Respuesta PostConfiguraApartado(PostConfiguraApartado con)
        {
            try
            {
                string sql = string.Empty;
                SQLTransaction save = new SQLTransaction();
                sql = $@"INSERT INTO PRODAPPS.APDAPART(
                        FIAPIDCAPA,
                        FIATIPODES,
                        FDAPVALOR, 
                        FIAPTIPOAU,
                        FIAPSTATUS,
                        USERCREAT,
                        FIAPIDAPPS)
                            VALUES(
                            (SELECT COALESCE(MAX(FIAPIDCAPA), 0) + 1 ID FROM PRODAPPS.APDAPART),
                           '{con.IdTipoDescuento}',
                            {con.Valor},
                            {con.IdTipoVehiculo},
                            1,
                            '{con.IdUser}',
                            {con.IdApps})";
                if (!save.SQLGuardaTabla(sql))
                    throw new Exception();
                return new Respuesta() { Ok = "SI", Mensaje = "Se registro éxitosamente.", Objeto = "" };
            }
            catch (Exception)
            {
                return new Respuesta() { Ok = "NO", Mensaje = "Intente nuevamente.", Objeto = "" };
            }
        }
        public Respuesta PutConfiguracionApartado(ConfiguraApartado put)
        {
            try
            {
                string sql = string.Empty;
                SQLTransaction save = new SQLTransaction();
                sql = $@"UPDATE PRODAPPS.APDAPART
                            SET FIATIPODES = {put.IdTipoDescuento},
                            FDAPVALOR = {put.Valor}, 
                            FIAPTIPOAU = {put.IdTipoVehiculo},
                            USERUPDAT = {put.IdUser},
                            DATEUPDAT = CURRENT_DATE,
                            TIMEUPDAT = CURRENT_TIME
                        WHERE FIAPIDCAPA = {put.Id} AND FIAPIDAPPS = {put.IdApps}";
                if (!save.SQLGuardaTabla(sql))
                    throw new Exception();
                return new Respuesta() { Ok = "SI", Mensaje = "Se Actualizo correctamente.", Objeto = "" };
            }
            catch (Exception)
            {
                return new Respuesta() { Ok = "NO", Mensaje = "Intente nuevamente.", Objeto = "" };
            }
        }



        public List<RestConfiguraApartado> GetValoresConfigurados(string IdApps)
        {
            try
            {
                SmartITController controller = new SmartITController();
                List<TipoDescuento> desc = controller.GetDescuentos();
                List<TipoVehiculoAudi> TipoVehiculo = controller.GetTipoVehciuloAudi();
                DVADB.DB2 dbCnx = new DVADB.DB2();
                string sql = string.Empty;
                List<RestConfiguraApartado> Ocon = new List<RestConfiguraApartado>();
                RestConfiguraApartado con;
                sql = @"SELECT * FROM PRODAPPS.APDAPART WHERE FIAPIDAPPS = " + IdApps;
                DataTable dt = dbCnx.GetDataSet(sql).Tables[0];
                if (dt.Rows.Count == 0)
                    throw new Exception();
                foreach (DataRow dr in dt.Rows)
                {
                    con = new RestConfiguraApartado();
                    con.Id = dr["FIAPIDCAPA"].ToString().Trim();
                    con.IdTipoDescuento = dr["FIATIPODES"].ToString().Trim();
                    con.TipoDescuento = desc.Where(x => x.Id == dr["FIATIPODES"].ToString().Trim()).FirstOrDefault().Nombre;
                    con.Valor = dr["FDAPVALOR"].ToString().Trim();
                    con.IdTipoVehiculo = dr["FIAPTIPOAU"].ToString().Trim();
                    con.IdUser = dr["USERCREAT"].ToString().Trim();
                    con.IdApps = dr["FIAPIDAPPS"].ToString().Trim();
                    con.TipoVehiculo = TipoVehiculo.Where(x => x.Id == dr["FIAPTIPOAU"].ToString().Trim()).FirstOrDefault().Tipo;
                    Ocon.Add(con);
                }
                return Ocon;
            }
            catch (Exception)
            {
                return new List<RestConfiguraApartado>();
            }
        }
        public List<Noticia> Obtnernoticias(int IdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            string sql = string.Empty;
            List<Noticia> onoti = new List<Noticia>();
            Noticia noti;
            sql = @"SELECT FIAPIDNOTI,
                            FSAPNOMBNT,
                            FSAPDESCNT,
                            FSAPRUTDOC,
                            FSRUTAIMNT,
                            FIAPIDAPPS
                    FROM PRODAPPS.APCNOTIC
                    WHERE FIAPIDAPPS = " + IdApps;
            DataTable dt = dbCnx.GetDataSet(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    noti = new Noticia();
                    noti.Id = dr["FIAPIDNOTI"].ToString().Trim();
                    noti.Nombre = dr["FSAPNOMBNT"].ToString().Trim();
                    noti.Descripcion = dr["FSAPDESCNT"].ToString().Trim();
                    noti.PDF = dr["FSAPRUTDOC"].ToString().Trim();
                    noti.Imagen = dr["FSRUTAIMNT"].ToString().Trim();
                    noti.IdApps = dr["FIAPIDAPPS"].ToString().Trim();
                    onoti.Add(noti);
                }
            }
            return dt.Rows.Count == 0 ? new List<Noticia>() : onoti;
        }
        public Respuesta ActualizarProgramaEspecial(int Id,int status, int IdApps)
        {
            SQLTransaction _save = new SQLTransaction();
            string sql = string.Empty;
            try
            {
                sql = $@"UPDATE PRODAPPS.APCPROGR
                            SET FIAPSTATUS = {status}
                         WHERE FIAPIDPRO = {Id} AND FIAPIDAPPS = {IdApps}";
                if (!_save.SQLGuardaTabla(sql))
                    throw new Exception();
                return new Respuesta { Ok = "SI", Mensaje = "Actualizado con éxito.", Objeto = "" };
            }
            catch (Exception)
            {
                return new Respuesta { Ok = "No", Mensaje = "No se pudo actualizar.", Objeto = "" };
            } 
        }
        public List<ResponsePromo> ObtnerProgramasEspeciales(int IdApps)
        {
            SmartITController _smart = new SmartITController();
            List<ResponsePromo> opromo = new List<ResponsePromo>();
            ResponsePromo promo;
            DVADB.DB2 dbCnx = new DVADB.DB2();
            string sql = string.Empty;
            sql = $@"SELECT * FROM PRODAPPS.APCPROGR
                    WHERE 1=1
                    AND FFAPFECHAF >= CURRENT_DATE 
                    AND FIAPSTATUS = 1
                    AND FIAPIDAPPS = {IdApps}";
            DataTable dt = dbCnx.GetDataSet(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    promo = new ResponsePromo();
                    promo.Id = dr["FIAPIDPRO"].ToString().Trim();
                    promo.Nombre = dr["FSAPTITUGR"].ToString().Trim();
                    promo.FechaInicio = Convert.ToDateTime(dr["FFAPFECHAI"].ToString().Trim()).ToString("yyyy-MM-dd").ToString();
                    promo.FechaFinal = Convert.ToDateTime(dr["FFAPFECHAF"].ToString().Trim()).ToString("yyyy-MM-dd").ToString();
                    promo.Ruta = dr["FSAPRUTDOC"].ToString().Trim();
                    promo.Status = dr["FIAPSTATUS"].ToString().Trim();
                    promo.Descripcion = dr["FSAPDESCGR"].ToString().Trim();
                    promo.IdTipoDescuento = dr["FIAPTPDESC"].ToString().Trim();
                    promo.Valor = dr["FIAVALORDC"].ToString().Trim();
                    promo.IdApps = dr["FIAPIDAPPS"].ToString().Trim();
                    opromo.Add(promo);
                }
            }
            return dt.Rows.Count == 0 ? new List<ResponsePromo>() : opromo;
        }

        internal List<TipoSeguro> GetTipoSeguro()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            List<TipoSeguro> _list = new List<TipoSeguro>();
            TipoSeguro _seguro;
            string strSQL = @"SELECT FIAPIDTIPO,FSAPTPPDES FROM PRODAPPS.APECTPVW
                                WHERE FIAPSTATUS = 1";
            DataTable dtCorreos = dbCnx.GetDataSet(strSQL).Tables[0];
            if (dtCorreos.Rows.Count > 0)
            {
                foreach (DataRow dr in dtCorreos.Rows)
                {
                    _seguro = new TipoSeguro();
                    _seguro.Id = dr["FIAPIDTIPO"].ToString().Trim();
                    _seguro.Tipo = dr["FSAPTPPDES"].ToString().Trim();
                    _list.Add(_seguro);
                }
            }
            return _list.Count > 0 ? _list : new List<TipoSeguro>();
        }

        internal string GetEnviaMotivoAutorizacion(AutorizacionDocumento doc)
        {
            string salida = string.Empty;
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                List<string> lstCorreos = new List<string>();
                CorreoService correoService = new CorreoService();
                string strSQL = @"SELECT FSAPCORREO  FROM PRODAPPS.APDCRSVW
                             WHERE FIAPIDCIAU = " + doc.IdAgencia;
                DataTable dtCorreos = dbCnx.GetDataSet(strSQL).Tables[0];

                if (dtCorreos.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtCorreos.Rows)
                    {
                        string correo = string.Empty;
                        correo = dr["FSAPCORREO"].ToString().Trim();
                        lstCorreos.Add(correo);
                    }
                }
                if (!correoService.EnviarCorreoAutorizacion(lstCorreos, doc))
                    throw new Exception();
                salida = "Correo enviado";
            }
            catch (Exception)
            {
                salida = "No fue posible mandar el correo";
            }
            return salida;
        }
        internal Respuesta PutCita(Citas _cita)
        {
            Respuesta _respuesta = new Respuesta();
            SQLTransaction SQL = new SQLTransaction();
            try
            {
                string strSQL = string.Empty;
                strSQL = $@"UPDATE PRODAPPS.APDDTCVW
                            SET FIAPDESCCI = '{_cita.Descripcion}' , FFAPFECHA = '{_cita.Fecha}' , FHAPHORINI = '{_cita.HoraInicial}' , FHAPHORFIN = '{_cita.HoraFinal}', FSAPUBICAC = '{_cita.Ubicacion}' , DATEUPDAT = CURRENT_DATE, TIMEUPDAT = CURRENT_TIME
                            WHERE FIAPIDCOMP = {_cita.IdCompra}
                                AND FIAPIDAPPS = {_cita.IdApps}
                                AND FIAPIDTCIT = {_cita.IdTipo}";
                if (!SQL.SQLGuardaTabla(strSQL))
                    throw new Exception();
                _respuesta.Ok = "SI";
                _respuesta.Mensaje = "Se ha actualizo con éxito la cita";
                _respuesta.Objeto = "";
            }
            catch (Exception)
            {
                _respuesta.Ok = "NO";
                _respuesta.Mensaje = "Ocurrio un error al actualizar la cita";
                _respuesta.Objeto = "";
            }
            return _respuesta;
        }
        internal List<DocumentoComunicacion> GetObtenerDocumentoComunicacion(string aIdCompra, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            DocumentoComunicacion Comprobante;
            List<DocumentoComunicacion> ComprobantesDePago = new List<DocumentoComunicacion>();
            try
            {
                string Query = string.Empty;
                Query = @"SELECT FIAPIDCOMP, FIAPIDCONS, FIAPIDTIPO, FSAPDOCUME, FSAPRUTDOC 
                    FROM PRODAPPS.APDDCSVW
                    WHERE FIAPIDCOMP = " + aIdCompra + " AND FIAPSTATUS = 1" + " AND FIAPIDAPPS = " + aIdApps;
                DataTable DT = dbCnx.GetDataSet(Query).Tables[0];
                if (DT.Rows.Count == 0)
                    throw new Exception();
                foreach (DataRow dr in DT.Rows)
                {
                    Comprobante = new DocumentoComunicacion();
                    Comprobante.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    Comprobante.IdConsecutivo = dr["FIAPIDCONS"].ToString().Trim();
                    Comprobante.IdTipo = dr["FIAPIDTIPO"].ToString().Trim();
                    Comprobante.NombreDocumento = dr["FSAPDOCUME"].ToString().Trim();
                    Comprobante.RutaDocumento = dr["FSAPRUTDOC"].ToString().Trim();
                    ComprobantesDePago.Add(Comprobante);
                }
            }
            catch (Exception)
            {
                ComprobantesDePago = new List<DocumentoComunicacion>();
            }
            return ComprobantesDePago;
        }
        internal List<HistorialMovimientosContactoAgencia> GetObtieneHistorialMovimientosConAgencia(string aIdCompra, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            List<HistorialMovimientosContactoAgencia> Historial = new List<HistorialMovimientosContactoAgencia>();
            HistorialMovimientosContactoAgencia Reg = new HistorialMovimientosContactoAgencia();
            string strSql = string.Empty;
            try
            {
                strSql = @"SELECT  FIAPIDCIAU, FSAPDESCCK, FFAPFECHA, FHAPHORA, FSAPDESEST, FSAPNOTIFI  FROM PRODAPPS.APDHSMVW
                        WHERE FIAPSTATUS = 1 AND FIAPIDCOMP = " + aIdCompra + " AND FIAPIDAPPS=" + aIdApps;
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
                if (dt.Rows.Count == 0)
                    throw new Exception();
                foreach (DataRow dr in dt.Rows)
                {
                    Reg = new HistorialMovimientosContactoAgencia();
                    Reg.Agencia = dr["FIAPIDCIAU"].ToString().Trim();
                    Reg.Estado = dr["FSAPDESEST"].ToString().Trim();
                    Reg.Fecha = dr["FFAPFECHA"].ToString().Trim();
                    Reg.Hora = dr["FHAPHORA"].ToString().Trim();
                    Reg.Mensaje = dr["FSAPNOTIFI"].ToString().Trim();
                    Reg.Paso = dr["FSAPDESCCK"].ToString().Trim();

                    Historial.Add(Reg);
                }
            }
            catch (Exception)
            {
                Historial =  new List<HistorialMovimientosContactoAgencia>();
            }
            return Historial;
        }
        internal Respuesta PostRegistraHistoricoContactoAgencia([FromBody] HistoricoComunicacionAgencia historico)
        {
            Respuesta respuesta = new Respuesta();
            SQLTransaction _save = new SQLTransaction();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            try
            {
                string strHist = string.Empty;
                respuesta.Ok = "SI";
                strHist = @"INSERT INTO PRODAPPS.APDHSMVW (
                            FIAPIDCIAU, 
                            FIAPIDCOMP,
                            FFAPFECHA, 
                            FHAPHORA,
                            FIAPIDESTA, 
                            FSAPDESEST,
                            FIAPIDPCKL, 
                            FSAPDESCCK,
                            FSAPNOTIFI, 
                            FIAPSTATUS,
                            USERCREAT,
                            PROGCREAT,
                            FIAPIDAPPS)
                            VALUES (" +
                            (string.IsNullOrEmpty(historico.IdAgencia) ? "Default" : historico.IdAgencia.Trim().ToUpper()) + "," + (string.IsNullOrEmpty(historico.IdCompra) ? "Default" : historico.IdCompra.Trim().ToUpper()) + "," +
                            "CURRENT DATE, CURRENT TIME," +
                            (string.IsNullOrEmpty(historico.IdEstadoMovimiento) ? "Default" : historico.IdEstadoMovimiento.Trim().ToUpper()) + " ," + "'" + (string.IsNullOrEmpty(historico.DescripcionEstadoMovimiento) ? "Default" : historico.DescripcionEstadoMovimiento.Trim().ToUpper()) + "'," +
                            (string.IsNullOrEmpty(historico.IdCheck) ? "Default" : historico.IdCheck.Trim().ToUpper()) + "," + "'" + (string.IsNullOrEmpty(historico.DescripcionCheck) ? "Default" : historico.DescripcionCheck.Trim().ToUpper()) + "','" +
                            (string.IsNullOrEmpty(historico.Mensaje) ? "Default" : historico.Mensaje.Trim().ToUpper()) + "'" + "," + "1" + "," +
                            "'APP', 'APP'," + historico.IdApps + ")";
                if (!_save.SQLGuardaTabla(strHist))
                    throw new Exception();
                respuesta.Objeto = null;
                respuesta.Mensaje = "Mensaje guardado con éxito";
            }
            catch (Exception ex)
            {
                respuesta.Ok = "No";
                respuesta.Mensaje = ex.Message;
                respuesta.Objeto = null; 
            }
            return respuesta;
        }

        internal List<CatalogoMarca> GetCatalogoMarca()
        {
            List<CatalogoMarca> OCatalogoMarca = new List<CatalogoMarca>();
            try
            {
                CatalogoMarca _catalogo = new CatalogoMarca();
                DVADB.DB2 dbCnx = new DVADB.DB2();
                string Sql = string.Empty;
                Sql = @"SELECT FIAPIDAPPS IDAPPS /*ID DE APLICACION*/ ,
                        FIAPIDMARC  MARCA /*ID MARCA*/,
                        FSAPDESCRI  /*DESCRIPCION*/
                        FROM PRODAPPS.APCAPMAR WHERE FIAPSTATUS = 1";
                DataTable dt = dbCnx.GetDataSet(Sql).Tables[0];
                if (dt.Rows.Count == 0)
                    throw new Exception();
                foreach (DataRow dr in dt.Rows)
                {
                    _catalogo = new CatalogoMarca();
                    _catalogo.IdApps = dr["IDAPPS"].ToString().Trim();
                    _catalogo.IdMarca = dr["MARCA"].ToString().Trim();
                    _catalogo.Marca = dr["FSAPDESCRI"].ToString().Trim();
                    OCatalogoMarca.Add(_catalogo);
                }
            }
            catch (Exception)
            {
                OCatalogoMarca = new List<CatalogoMarca>();
            }
            return OCatalogoMarca;
        }
        internal List<CatalogoApps> GetCatalogoIdApps()
        {
            List<CatalogoApps> OCatalogoApps = new List<CatalogoApps>();
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                CatalogoApps _catalogo;
                string Sql = string.Empty;
                Sql = @"SELECT FIAPIDAPPS IDAPPS,
                        FSAPVERLIB  VERSION,
                        FSAPDESCRI  DESCRIPCION,
                        FFAPFECHA  FECHA 
                    FROM PRODAPPS.APCAPPMB WHERE FIAPSTATUS = 1";
                DataTable dt = dbCnx.GetDataSet(Sql).Tables[0];

                if (dt.Rows.Count == 0)
                    throw new Exception();

                foreach (DataRow dr in dt.Rows)
                {
                    _catalogo = new CatalogoApps();
                    _catalogo.IdApps = dr["IDAPPS"].ToString().Trim();
                    _catalogo.Version = dr["VERSION"].ToString().Trim();
                    _catalogo.Apps = dr["DESCRIPCION"].ToString().Trim();
                    _catalogo.FechaLiberacion = Convert.ToDateTime(dr["FECHA"].ToString().Trim()).ToString("dd/MM/yyyy");
                    OCatalogoApps.Add(_catalogo);
                }
            }
            catch (Exception)
            {
                OCatalogoApps = new List<CatalogoApps>();
            }
            return OCatalogoApps;
        }
        internal Respuesta PostActualizaCheckContactoAgencia([FromBody] ActualizaCheckAgencia checkAgencia)
        {
            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            SQLTransaction _Obj = new SQLTransaction();
            string descEstado = string.Empty;
            string strCCatEsta = string.Empty;
            try
            {
                strCCatEsta += "select FIAPIDESTA, FSAPDESEST FROM " +
                                "PRODAPPS.APCESSVW " +
                                "where FIAPSTATUS = 1 " +
                                "AND FIAPIDESTA = " + checkAgencia.IdEstado;
                DataTable dtCaE = dbCnx.GetDataSet(strCCatEsta).Tables[0];
                if (dtCaE.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtCaE.Rows)
                    {
                        descEstado = dr["FSAPDESEST"].ToString().Trim();
                    }
                }

                string strUpCh = string.Empty;
                strUpCh = "UPDATE PRODAPPS.APDCLSVW "
                + "SET FSAPDESEST = " + "'" + descEstado + "'" + "," + "FIAPIDESTA = " + checkAgencia.IdEstado + ","
                + "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' "
                + "WHERE "
                + "FIAPIDCOMP = " + checkAgencia.IdCompra
                + " AND FIAPIDPCKL = " + checkAgencia.IdCheck
                + " AND FIAPSTATUS = 1" +
                 " AND FIAPIDAPPS = " + checkAgencia.IdApps;

                if (!_Obj.SQLGuardaTabla(strUpCh))
                    throw new Exception();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Registro actualizado correctamente.";
                respuesta.Objeto = "";
            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo guardar el registro.";
                respuesta.Objeto = "";
            }
            return respuesta;
        }
        internal List<DatoCheckAgencia> GetCheckContactoAgenciaPorIdCompra(long aIdCompra, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DatoCheckAgencia dato = new DatoCheckAgencia();
            List<DatoCheckAgencia> lstDatos = new List<DatoCheckAgencia>();
            string strCC = string.Empty;
            try
            {
                strCC += " select FIAPIDCIAU, FIAPIDCOMP, FIAPIDPCKL, FSAPDESCCK, FSAPDESEST, FIAPIDESTA "+
                "from PRODAPPS.APDCLSVW " +
                "WHERE " +
                " FIAPIDCOMP = " + aIdCompra +
                " AND FIAPIDAPPS = " + aIdApps + 
                " AND FIAPSTATUS = 1";
                DataTable dtE = dbCnx.GetDataSet(strCC).Tables[0];
                if (dtE.Rows.Count == 0)
                    throw new Exception();
                foreach (DataRow dr in dtE.Rows)
                {
                    dato = new DatoCheckAgencia();
                    dato.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                    dato.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    dato.IdCheck = dr["FIAPIDPCKL"].ToString().Trim();
                    dato.DescripcionCheck = dr["FSAPDESCCK"].ToString().Trim();
                    dato.DescripcionEstado = dr["FSAPDESEST"].ToString().Trim();
                    dato.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
                    lstDatos.Add(dato);
                }
            }
            catch (Exception)
            {
                lstDatos = new List<DatoCheckAgencia>();
            }
            return lstDatos;
        }

        internal Respuesta RegistraCheckContactoAgencia([FromBody] CheckAgencia check)
        {
            Respuesta respuesta = new Respuesta();
            SQLTransaction _sql = new SQLTransaction();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            string descEstado = string.Empty;
            string strCCatEsta = string.Empty;
            try
            {
                strCCatEsta += "select FIAPIDESTA, FSAPDESEST FROM " +
                                "PRODAPPS.APCESTAG " +
                                "where FIAPSTATUS = 1 " +
                                "AND FIAPIDESTA = " + check.IdEstado;
                DataTable dtCaE = dbCnx.GetDataSet(strCCatEsta).Tables[0];
                if (dtCaE.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtCaE.Rows)
                    {
                        descEstado = dr["FSAPDESEST"].ToString().Trim();
                    }
                }
                string strRCh = string.Empty;
                strRCh += "INSERT INTO PRODAPPS.APDCLSVW ( "
                        + "FIAPIDCIAU, FIAPIDCOMP, FIAPIDPCKL, "
                        + "FSAPDESCCK, FSAPDESEST, FIAPIDESTA, "
                        + "FIAPSTATUS, USERCREAT, PROGCREAT,FIAPIDAPPS)"
                        + "VALUES ("
                        + (string.IsNullOrEmpty(check.IdAgencia) ? "Default" : check.IdAgencia.Trim().ToUpper()) + "," + (string.IsNullOrEmpty(check.IdCompra) ? "Default" : check.IdCompra.Trim().ToUpper()) + "," + (string.IsNullOrEmpty(check.IdCheck) ? "Default" : check.IdCheck.Trim().ToUpper()) + ","
                        + "'" + (string.IsNullOrEmpty(check.DescripcionCheck) ? "Default" : check.DescripcionCheck.Trim().ToUpper()) + "'" + "," + "'" + (string.IsNullOrEmpty(descEstado) ? "Default" : descEstado.Trim().ToUpper()) + "'" + "," + (string.IsNullOrEmpty(check.IdEstado) ? "Default" : check.IdEstado.Trim())
                        + "," + "1,'APP', 'APP'," + check.IdApps
                        + ")";
                if (!_sql.SQLGuardaTabla(strRCh))
                    throw new Exception();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Registro guardado correctamente.";
                respuesta.Objeto = "";

            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo guardar el registro.";
                respuesta.Objeto = "";
            }
            return respuesta;
        }
        internal Respuesta UpdateDeshacePasoRealizado(long aIdCompra, int aIdPaso, string aIdApps)
        {
            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            SQLTransaction obj = new SQLTransaction();
            string strActualiza = string.Empty;
            try
            {
                strActualiza = "UPDATE PRODAPPS.APDCKLVW " +
                                "SET FIAPREALIZ = 0, " +
                                "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' " +
                                "where FIAPIDCOMP = " + aIdCompra.ToString() + " " +
                                "and FIAPIDPCKL = " + aIdPaso.ToString() + " " +
                                "AND FIAPSTATUS = 1 AND FIAPIDAPPS=" + aIdApps;
                if (!obj.SQLGuardaTabla(strActualiza))
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
        internal List<ProcesoCompraSmartIt> GetCatalogoProcesoCompra()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            List<ProcesoCompraSmartIt> lstElementoProceso = new List<ProcesoCompraSmartIt>();
            ProcesoCompraSmartIt elementoProceso = new ProcesoCompraSmartIt();
            try
            {
                string srtCons = string.Empty;
                srtCons = @"select FIAPIDPASO, FSAPDESCRI  from " +
                            "prodapps.APCPROVW " +
                            "WHERE FIAPIDPROC = 2 " +
                            "AND FIAPSTATUS = 1 " +
                            "AND FIAPIDPASO <> 10 ";
                DataTable dt = dbCnx.GetDataSet(srtCons).Tables[0];
                if (dt.Rows.Count == 0)
                    throw new Exception();
                foreach (DataRow dr in dt.Rows)
                {
                    elementoProceso = new ProcesoCompraSmartIt();
                    elementoProceso.Idpaso = dr["FIAPIDPASO"].ToString().Trim();
                    elementoProceso.Descripcion = dr["FSAPDESCRI"].ToString().Trim();
                    lstElementoProceso.Add(elementoProceso);
                }
                lstElementoProceso = lstElementoProceso.OrderBy(s => Convert.ToUInt32(s.Idpaso)).ToList();
            }
            catch (Exception)
            {
                lstElementoProceso = new List<ProcesoCompraSmartIt>();
            }
            return lstElementoProceso;
        }
        internal Seguro GetDatosSeguro(long IdCompra)
        {
            Seguro Seguro = new Seguro();
            DVADB.DB2 dbCnx = new DVADB.DB2();

            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string Query = "";
            Query = "SELECT FIAPIDCOMP, FIAPSELECT, FIAPIDASEG, FSAPDESASE, FSAPCOBERT, FFAPVIGINI, FFAPVIGFIN, FDAPPRIMAT, FDAPCOMISE, FDAPTOTAL ";
            Query += "FROM	" + constantes.Ambiente + "APPS.APDDTSVW ";
            Query += "WHERE FIAPIDCOMP = " + IdCompra + " AND FIAPSTATUS = 1";

            DataTable DT = dbCnx.GetDataSet(Query).Tables[0];

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    Seguro.Aseguradora = dr["FSAPDESASE"].ToString().Trim();
                    Seguro.Cobertura = dr["FSAPCOBERT"].ToString().Trim();
                    Seguro.Comision = dr["FDAPCOMISE"].ToString().Trim();
                    Seguro.FechaFinal = dr["FFAPVIGFIN"].ToString().Trim();
                    Seguro.FechaInicial = dr["FFAPVIGINI"].ToString().Trim();
                    Seguro.PrimaTotal = dr["FDAPPRIMAT"].ToString().Trim();
                    Seguro.Seleccionado = dr["FIAPSELECT"].ToString().Trim();
                    Seguro.TotalSeguro = dr["FDAPTOTAL"].ToString().Trim();
                }
            }
            else
            {
                Seguro.Aseguradora = null;
                Seguro.Cobertura = null;
                Seguro.Comision = null;
                Seguro.FechaFinal = null;
                Seguro.FechaInicial = null;
                Seguro.PrimaTotal = null;
                Seguro.Seleccionado = null;
                Seguro.TotalSeguro = null;

            }
            return Seguro;
        }
        internal DatosGestoria GetDatosGestoria(long aIdCompra)
        {
            DatosGestoria Gestoria = new DatosGestoria();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string Query = "";
            Query = "SELECT FIAPIDCOMP, FIAPIDTRSE, FIAPSELECT, FDAPVALORC, FDAPPORCEN, FDAPTOTAL ";
            Query += "FROM	" + constantes.Ambiente + "APPS.APDDTGVW ";
            Query += "WHERE FIAPIDCOMP = " + aIdCompra + " AND FIAPSTATUS = 1";
            DataTable DT = dbCnx.GetDataSet(Query).Tables[0];
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    Gestoria.PorcentajeMensual = dr["FDAPPORCEN"].ToString().Trim();
                    Gestoria.ElegioTramite = dr["FIAPSELECT"].ToString().Trim();
                    Gestoria.Total = dr["FDAPTOTAL"].ToString().Trim();
                    Gestoria.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    Gestoria.IdTramiteSeleccionado = dr["FIAPIDTRSE"].ToString().Trim();
                }
            }
            else
            {
                Gestoria.PorcentajeMensual = null;
                Gestoria.ElegioTramite = null;
                Gestoria.Total = null;
            }
            return Gestoria;
        }
        internal List<DocumentoSmartIT> GetObtieneComprobantesPago(long aIdCompra, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string Query = string.Empty;
            Query = @"SELECT FIAPIDAPPS,FIAPIDCONS, FIAPIDCOMP, FIAPIDTIPO, FSAPDOCUME, FSAPRUTDOC " +
                        "FROM	" + constantes.Ambiente + "APPS.APDCMPVW " +
                        "WHERE FIAPIDCOMP = " + aIdCompra + " AND FIAPSTATUS = 1" + " AND FIAPIDAPPS = " + aIdApps;
            DataTable DT = dbCnx.GetDataSet(Query).Tables[0];
            DocumentoSmartIT Comprobante;
            List<DocumentoSmartIT> ComprobantesDePago = new List<DocumentoSmartIT>();
            try
            {
                if (DT.Rows.Count == 0)
                    throw new Exception();

                foreach (DataRow dr in DT.Rows)
                {
                    Comprobante = new DocumentoSmartIT();
                    Comprobante.IdApps = dr["FIAPIDAPPS"].ToString().Trim();
                    Comprobante.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    Comprobante.IdTipo = dr["FIAPIDTIPO"].ToString().Trim();
                    Comprobante.NombreDocumento = dr["FSAPDOCUME"].ToString().Trim();
                    Comprobante.RutaDocumento = dr["FSAPRUTDOC"].ToString().Trim(); 
                    ComprobantesDePago.Add(Comprobante);
                }
            }
            catch (Exception)
            {
                ComprobantesDePago = new List<DocumentoSmartIT>();
            }
            return ComprobantesDePago;
        }

        internal Respuesta PostEnviaNotificaciones([FromBody]NotificacionCompra NotificacionCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Respuesta respuesta = new Respuesta();
            string notificacionesSinLeer = "0";
            string strSqlSeg = string.Empty;
            strSqlSeg = @"UPDATE " + constantes.Ambiente + "APPS.APECMPVW " + 
                            "SET FIAPIDESTA = " + NotificacionCompra.IdEstado + ", " +
                            "USERUPDAT = 'APPS', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APPS' " +
                            "WHERE FIAPIDCOMP = " + NotificacionCompra.IdCompra + " AND FIAPIDAPPS = " + NotificacionCompra.IdApps  + " " +
                            "AND FIAPSTATUS = 1 AND FIAPIDESTA NOT IN(15,24)";

            string strInsCabComp = string.Empty; 
            strInsCabComp += "INSERT INTO " + constantes.Ambiente + "APPS.APDSGCVW ";
            strInsCabComp += "(FIAPIDCOMP, FIAPIDSEGU, FSAPTITSEG, FIAPIDESTA, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT,FIAPIDAPPS)";
            strInsCabComp += "VALUES ";
            strInsCabComp += "(";
            strInsCabComp += NotificacionCompra.IdCompra + ", ";
            strInsCabComp += "(SELECT coalesce(MAX(FIAPIDSEGU),0)+1 ID FROM PRODAPPS.APDSGCVW WHERE FIAPIDCOMP = " + NotificacionCompra.IdCompra + " AND FIAPIDAPPS = "+ NotificacionCompra.IdApps + "),";
            strInsCabComp += "'Movimiento generado en SmartIt' ,";
            strInsCabComp += NotificacionCompra.IdEstado + ",";
            strInsCabComp += "1, 'APPS' ,CURRENT DATE, CURRENT TIME, 'APPS'," + NotificacionCompra.IdApps;
            strInsCabComp += ")";

            string instruccion = "{\"Vista\":\"miCupra\",\"Parametros\":{\"IdCompra\":\"" + NotificacionCompra.IdCompra + "\"}}";

            string strSql = string.Empty; 
            strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDNOTVW ";
            strSql += "(FIAPIDCUEN, FIAPIDNOTI, FFAPNOTIFI, FHAPNOTIFI, FSAPASUNTO, FSAPNOTIFI, FIAPAPLSEG, FIAPIDPREO, FIAPAPLENC, FIAPIDENPE, FSAPINSTRU, FIAPSTATUS, USERCREAT, PROGCREAT,FIAPIDAPPS) ";
            strSql += "VALUES ";
            strSql += "(";
            strSql += NotificacionCompra.IdCuenta + ",";
            strSql += "(SELECT coalesce(MAX(FIAPIDNOTI),0)+1 ID FROM PRODAPPS.APDNOTVW WHERE FIAPIDAPPS = " + NotificacionCompra.IdApps + " AND FIAPIDCUEN = " + NotificacionCompra.IdCuenta + "),";
            strSql += "CURRENT DATE" + ",";
            strSql += "CURRENT TIME" + ",";
            strSql += "'" + NotificacionCompra.Asunto + "',";
            strSql += "'" + NotificacionCompra.DescripcionNotificacion + "',";
            strSql += "0,";
            strSql += "default,";
            strSql += "1,";
            strSql += "default,";
            strSql += "'" + instruccion + "'" + ",";
            strSql += "1,'APPS','APPS'," + NotificacionCompra.IdApps;
            strSql += ")";

            string strSqlNotificaciones = string.Empty;
            strSqlNotificaciones = @"select COUNT(*) NOTIFICACIONES from " +
                                    "prodapps.APDNOTVW " +
                                    "where FIAPIDCUEN = " + NotificacionCompra.IdCuenta + " " +
                                    " AND FIAPVISTO = 0 AND FIAPSTATUS = 1 AND FIAPIDAPPS = " + NotificacionCompra.IdApps;
            try
            {
                
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                string sql = @"SELECT * FROM PRODAPPS.APECMPVW " + " " +
                                "WHERE FIAPIDCOMP = " + NotificacionCompra.IdCompra + " " + "AND FIAPSTATUS = 1 AND FIAPIDESTA NOT IN(15, 24) AND FIAPIDAPPS = " + NotificacionCompra.IdApps;
                DataTable _data = dbCnx.GetDataSet(sql).Tables[0];
                if (_data.Rows.Count == 0)
                    throw new Exception();

                dbCnx.SetQuery(strSqlSeg);
                dbCnx.SetQuery(strSql);
                dbCnx.SetQuery(strInsCabComp);
                if (NotificacionCompra.IdEstado.Equals("15"))
                {
                    strSql = string.Empty;
                    strSql += "select FIAPIDCIAU, FIAPIDPEDI, FIAPIDINVE from prodapps.APEPANVW where FIAPIDCOMP = " + NotificacionCompra.IdCompra + " AND FIAPIDAPPS =" + NotificacionCompra.IdApps;
                    DataTable dtPedido = dbCnx.GetDataSet(strSql).Tables[0];
                    foreach (DataRow drPedido in dtPedido.Rows)
                    {
                        if (Convert.ToInt64(drPedido["FIAPIDPEDI"]) > 0)
                        {
                            strSql = string.Empty;
                            strSql = @"UPDATE PRODAUT.ANEPEDAU SET FIANPASTP = 4, USERUPDAT = 'SmartIt', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'SmartIt' " + 
                                      "WHERE FNANPAAGE = " + drPedido["FIAPIDCIAU"].ToString() + " AND FNANPAIDE = " + drPedido["FIAPIDPEDI"].ToString();
                            dbCnx.SetQuery(strSql);
                        }
                        strSql = string.Empty;
                        strSql = @"UPDATE PRODAUT.ANCAUTOM SET FNAUTOEST = 10, USERUPDAT = 'SmartIt', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'SmartIt' " + 
                                  "WHERE FNAUTOEST in (11,50) AND FNAUTOAGE = " + drPedido["FIAPIDCIAU"].ToString() + " AND FIANIDINVE = " + drPedido["FIAPIDINVE"].ToString();
                        dbCnx.SetQuery(strSql);
                    }
                }
                DataTable notificacionesNL = dbCnx.GetDataSet(strSqlNotificaciones).Tables[0];
                if (notificacionesNL.Rows.Count > 0)
                {
                    foreach (DataRow dr in notificacionesNL.Rows)
                    {
                        notificacionesSinLeer = dr["NOTIFICACIONES"].ToString().Trim();
                    }
                }
                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se envió la notificación de forma satisfactoria";
                respuesta.Objeto = null;
                #region Notificacion fija
                string token = string.Empty;

                strSql = string.Empty;
                strSql = @"SELECT A.FIAPIDCUEN, A.FIAPIDPERS,  A.FIAPSTATUS, A.FIAPIDAPPS,A.FSAPNOMBRE,A.FSAPAPEPAT, A.FSAPAPEMAT,A.FSAPCORREO,A.FIAPLADMOV, A.FIAPNUMMOV, A.FSAPRUTFOT, B.FSAPTOKEN, B.FSAPCVEACT  FROM PRODAPPS.APCCTAVW A 
                            INNER JOIN PRODAPPS.APCTOKEN B ON 
                            A.FIAPIDCUEN = B.FIAPIDCUEN 
                            AND A.FIAPIDAPPS = B.FIAPIDAPPS " +
                        "WHERE A.FIAPIDCUEN = " + NotificacionCompra.IdCuenta + "  " +
                        "AND A.FIAPIDAPPS = " + NotificacionCompra.IdApps;
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    token = dt.Rows[0]["FSAPTOKEN"].ToString().Trim();
                }
                else
                {
                    throw new Exception();
                }
                Alerta alerta = new Alerta();
                alerta.to = token;
                notification notification = new notification();
                notification.title = NotificacionCompra.Asunto;
                notification.body = NotificacionCompra.DescripcionNotificacion;
                notification.badge = notificacionesSinLeer.Trim();
                alerta.notification = notification;
                AlertasCompra alert = new AlertasCompra();

                var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", $"Key={alert.KeyPush(NotificacionCompra.IdApps)}");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(alerta), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                #endregion

            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo enviar la notificación";
                respuesta.Objeto = null;
            }

            return respuesta;

        }
        internal List<OrdenCompraSmartIt> GetOrdenesDeCompraEnProcesoPorAgencia(int aIdAgencia, string aIdApps)
        {
            OrdenCompraSmartIt compra = new OrdenCompraSmartIt();
            List<OrdenCompraSmartIt> coleccionCompra = new List<OrdenCompraSmartIt>();
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();

                string strlSQL = string.Empty;
                strlSQL = $@"SELECT ape.FIAPIDCOMP IDCOMPRA, ape.FIAPIDAPPS, ape.FIAPIDMARC, ape.FIAPFOLCOM, ape.FIAPIDCUEN IDCUENTA, ape.FFAPFECCOM, ape.FHAPHORCOM, ape.FDAPSUBTOT SUBTOTAL, ape.FDAPDESCUE DESCUENTO, ape.FDAPIVA IVA, ape.FDAPTOTAL TOTAL, ape.FSAPRUTRFB, ape.FIAPIDESTA,  est.FSAPESTADO, ape.FIAPIDPROC, ape.FIAPIDPASO PASO, dan.FIAPIDCIAU, dan.FIAPIDPEDI, dan.FFAPFECPED, dan.FHAPHORPED, dan.FIAPIDPERS, dan.FIAPIDVEHI, dan.FIAPIDINVE, dan.FSAPMODELO, dan.FSAPVERSIO, dan.FSAPTRANSM ,dan.FSAPCOLEXT, dan.FSAPNUMINV,dan.FSAPNUMSER, dan.FDAPSUBTOT SUBTOTALPEDIDO, dan.FDAPDESCUE, dan.FDAPIVA IVAPEDIDO, dan.FDAPTOTAL TOTALPEDIDO, dan.FIAPCOTSEG, dan.FSAPRUTFOT,CUENTA.FSAPNOMBRE, CUENTA.FSAPAPEPAT, CUENTA.FSAPAPEMAT, CUENTA.FSAPCORREO, CUENTA.FIAPLADMOV, CUENTA.FIAPNUMMOV FROM PRODapps.APECMPVW ape 
                                INNER JOIN PRODapps.APCESCVW est ON ape.FIAPIDESTA = est.FIAPIDESTA 
                                INNER JOIN PRODapps.APEPANVW dan ON dan.FIAPIDCOMP = ape.FIAPIDCOMP 
                                AND  dan.FIAPIDMARC = ape.FIAPIDMARC 
                                INNER JOIN PRODAPPS.APCCTAVW CUENTA ON 
                                ape.FIAPIDCUEN = CUENTA.FIAPIDCUEN 
                                AND ape.FIAPIDAPPS= CUENTA.FIAPIDAPPS 
                                WHERE 1 = 1 
                                AND ape.FIAPSTATUS = 1 AND dan.FIAPIDCIAU = {aIdAgencia} AND ape.FIAPIDAPPS= {aIdApps}  ORDER BY ape.FIAPIDCOMP ASC";

                DataTable dt = dbCnx.GetDataSet(strlSQL).Tables[0];
                if (dt.Rows.Count == 0)
                    throw new Exception();

                foreach (DataRow dr in dt.Rows)
                {
                    compra = new OrdenCompraSmartIt();
                    compra.IdApps = dr["FIAPIDAPPS"].ToString().Trim();
                    compra.IdCompra = dr["IDCOMPRA"].ToString().Trim();
                    compra.IdMarca = dr["FIAPIDMARC"].ToString().Trim();
                    compra.CuentaUsuario = new Cuenta
                    {
                        IdCuenta = dr["IDCUENTA"].ToString().Trim(),
                        IdPersona = dr["FIAPIDPERS"].ToString().Trim(),
                        Nombre = dr["FSAPNOMBRE"].ToString().Trim(),
                        ApellidoPaterno = dr["FSAPAPEPAT"].ToString().Trim(),
                        ApellidoMaterno = dr["FSAPAPEMAT"].ToString().Trim(),
                        Correo = dr["FSAPCORREO"].ToString().Trim(),
                        LadaMovil = dr["FIAPLADMOV"].ToString().Trim(),
                        TelefonoMovil = dr["FIAPNUMMOV"].ToString().Trim(),
                    };
                    compra.IdProceso = dr["FIAPIDPROC"].ToString().Trim();
                    compra.RutaReferenciaBancaria = dr["FSAPRUTRFB"].ToString().Trim();
                    compra.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
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
                    compra.AccesoriosOtros = null;
                    compra.FechaCompra = !string.IsNullOrEmpty(dr["FFAPFECCOM"].ToString().Trim()) ? Convert.ToDateTime(dr["FFAPFECCOM"].ToString().Trim()).ToString("dd/MM/yyy") : dr["FFAPFECCOM"].ToString().Trim();
                    compra.HoraCompra = !string.IsNullOrEmpty(dr["FHAPHORCOM"].ToString()) ? Convert.ToDateTime(dr["FHAPHORCOM"]).ToString("hh:mm tt") : dr["FHAPHORCOM"].ToString();
                    compra.IdPedido = dr["FIAPIDPEDI"].ToString().Trim();
                    coleccionCompra.Add(compra);
                }
            }
            catch (Exception)
            {
                coleccionCompra = new List<OrdenCompraSmartIt>();
            }
            return coleccionCompra;
        }
        internal Task<List<OrdenCompraSmartIt>> GetOrdenesCompra(string aIdApps)
        {
            return Task.Run(() =>
            {
                OrdenCompraSmartIt compra = new OrdenCompraSmartIt();
                List<OrdenCompraSmartIt> coleccionCompra = new List<OrdenCompraSmartIt>();
                try
                {
                    DVADB.DB2 dbCnx = new DVADB.DB2();
                    DVAConstants.Constants constantes = new DVAConstants.Constants();

                    string strlSQL = string.Empty;
                    strlSQL = $@"SELECT ape.FIAPIDCOMP IDCOMPRA, ape.FIAPIDAPPS, ape.FIAPIDMARC, ape.FIAPFOLCOM, ape.FIAPIDCUEN IDCUENTA, ape.FFAPFECCOM, ape.FHAPHORCOM, ape.FDAPSUBTOT SUBTOTAL, ape.FDAPDESCUE DESCUENTO, ape.FDAPIVA IVA, ape.FDAPTOTAL TOTAL, ape.FSAPRUTRFB, ape.FIAPIDESTA,  est.FSAPESTADO, ape.FIAPIDPROC, ape.FIAPIDPASO PASO, dan.FIAPIDCIAU, dan.FIAPIDPEDI, dan.FFAPFECPED, dan.FHAPHORPED, dan.FIAPIDPERS, dan.FIAPIDVEHI, dan.FIAPIDINVE, dan.FSAPMODELO, dan.FSAPVERSIO, dan.FSAPTRANSM ,dan.FSAPCOLEXT, dan.FSAPNUMINV,dan.FSAPNUMSER, dan.FDAPSUBTOT SUBTOTALPEDIDO, dan.FDAPDESCUE, dan.FDAPIVA IVAPEDIDO, dan.FDAPTOTAL TOTALPEDIDO, dan.FIAPCOTSEG, dan.FSAPRUTFOT,CUENTA.FSAPNOMBRE, CUENTA.FSAPAPEPAT, CUENTA.FSAPAPEMAT, CUENTA.FSAPCORREO, CUENTA.FIAPLADMOV, CUENTA.FIAPNUMMOV FROM PRODapps.APECMPVW ape 
                                INNER JOIN PRODapps.APCESCVW est ON ape.FIAPIDESTA = est.FIAPIDESTA 
                                INNER JOIN PRODapps.APEPANVW dan ON dan.FIAPIDCOMP = ape.FIAPIDCOMP 
                                AND  dan.FIAPIDMARC = ape.FIAPIDMARC 
                                INNER JOIN PRODAPPS.APCCTAVW CUENTA ON 
                                ape.FIAPIDCUEN = CUENTA.FIAPIDCUEN 
                                AND ape.FIAPIDAPPS= CUENTA.FIAPIDAPPS 
                                WHERE 1 = 1 
                                AND ape.FIAPSTATUS = 1  AND ape.FIAPIDAPPS= {aIdApps}  ORDER BY ape.FIAPIDCOMP ASC";

                    DataTable dt = dbCnx.GetDataSet(strlSQL).Tables[0];
                    if (dt.Rows.Count == 0)
                        throw new Exception();

                    foreach (DataRow dr in dt.Rows)
                    {
                        compra = new OrdenCompraSmartIt();
                        compra.IdApps = dr["FIAPIDAPPS"].ToString().Trim();
                        compra.IdCompra = dr["IDCOMPRA"].ToString().Trim();
                        compra.IdPedido = dr["FIAPIDPEDI"].ToString().Trim();
                        compra.IdMarca = dr["FIAPIDMARC"].ToString().Trim();
                        compra.FechaCompra = !string.IsNullOrEmpty(dr["FFAPFECCOM"].ToString().Trim()) ? Convert.ToDateTime(dr["FFAPFECCOM"].ToString().Trim()).ToString("dd/MM/yyy") : dr["FFAPFECCOM"].ToString().Trim();
                        compra.HoraCompra = !string.IsNullOrEmpty(dr["FHAPHORCOM"].ToString()) ? Convert.ToDateTime(dr["FHAPHORCOM"]).ToString("hh:mm tt") : dr["FHAPHORCOM"].ToString();
                        compra.CuentaUsuario = new Cuenta
                        {
                            IdCuenta = dr["IDCUENTA"].ToString().Trim(),
                            IdPersona = dr["FIAPIDPERS"].ToString().Trim(),
                            Nombre = dr["FSAPNOMBRE"].ToString().Trim(),
                            ApellidoPaterno = dr["FSAPAPEPAT"].ToString().Trim(),
                            ApellidoMaterno = dr["FSAPAPEMAT"].ToString().Trim(),
                            Correo = dr["FSAPCORREO"].ToString().Trim(),
                            LadaMovil = dr["FIAPLADMOV"].ToString().Trim(),
                            TelefonoMovil = dr["FIAPNUMMOV"].ToString().Trim(),
                        };
                        compra.IdProceso = dr["FIAPIDPROC"].ToString().Trim();
                        compra.RutaReferenciaBancaria = dr["FSAPRUTRFB"].ToString().Trim();
                        compra.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
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
                        compra.AccesoriosOtros = null;
                        coleccionCompra.Add(compra);
                    }
                }
                catch (Exception)
                {
                    coleccionCompra = new List<OrdenCompraSmartIt>();
                }
                return coleccionCompra;
            });
        }
        internal OrdenCompraSmartIt GetOrdenCompra(long aIdCompra, string aIdApps)
        {
            OrdenCompraSmartIt compra = new OrdenCompraSmartIt();
            List<Accesorio> _coleccionaccesorio = new List<Accesorio>();
            Accesorio _accesorio = new Accesorio();
            DataTable dtA;
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();

                string strlSQL = string.Empty;
                strlSQL = @"SELECT ape.FIAPIDCOMP IDCOMPRA, ape.FIAPIDAPPS, ape.FIAPIDMARC, ape.FIAPFOLCOM, ape.FIAPIDCUEN IDCUENTA, ape.FFAPFECCOM, ape.FHAPHORCOM, ape.FDAPSUBTOT SUBTOTAL, ape.FDAPDESCUE DESCUENTO, ape.FDAPIVA IVA, ape.FDAPTOTAL TOTAL, ape.FSAPRUTRFB, ape.FIAPIDESTA,  est.FSAPESTADO, ape.FIAPIDPROC, ape.FIAPIDPASO PASO, dan.FIAPIDCIAU, dan.FIAPIDPEDI, dan.FFAPFECPED, dan.FHAPHORPED, dan.FIAPIDPERS, dan.FIAPIDVEHI, dan.FIAPIDINVE, dan.FSAPMODELO, dan.FSAPVERSIO, dan.FSAPTRANSM ,dan.FSAPCOLEXT, dan.FSAPNUMINV,dan.FSAPNUMSER, dan.FDAPSUBTOT SUBTOTALPEDIDO, dan.FDAPDESCUE, dan.FDAPIVA IVAPEDIDO, dan.FDAPTOTAL TOTALPEDIDO, dan.FIAPCOTSEG, dan.FSAPRUTFOT,CUENTA.FSAPNOMBRE, CUENTA.FSAPAPEPAT, CUENTA.FSAPAPEMAT, CUENTA.FSAPCORREO, CUENTA.FIAPLADMOV, CUENTA.FIAPNUMMOV FROM PRODapps.APECMPVW ape " +
                          "INNER JOIN PRODapps.APCESCVW est ON ape.FIAPIDESTA = est.FIAPIDESTA " +
                          "INNER JOIN PRODapps.APEPANVW dan ON dan.FIAPIDCOMP = ape.FIAPIDCOMP " +
                          "AND  dan.FIAPIDMARC = ape.FIAPIDMARC " + 
                          "INNER JOIN PRODAPPS.APCCTAVW CUENTA ON " +
                          "ape.FIAPIDCUEN = CUENTA.FIAPIDCUEN " +
                          "AND ape.FIAPIDAPPS= CUENTA.FIAPIDAPPS " + 
                          "WHERE 1 = 1 " +
                          "AND ape.FIAPSTATUS = 1 AND ape.FIAPIDCOMP = " + aIdCompra + " AND ape.FIAPIDAPPS=" + aIdApps + "  ORDER BY ape.FIAPIDCOMP ASC";
                DataTable dt = dbCnx.GetDataSet(strlSQL).Tables[0];
                if (dt.Rows.Count == 0)
                    throw new Exception();

                foreach (DataRow dr in dt.Rows)
                {
                    compra = new OrdenCompraSmartIt();
                    compra.IdApps = dr["FIAPIDAPPS"].ToString().Trim();
                    compra.IdCompra = dr["IDCOMPRA"].ToString().Trim();
                    compra.IdPedido = dr["FIAPIDPEDI"].ToString().Trim();
                    compra.IdMarca = dr["FIAPIDMARC"].ToString().Trim();
                    compra.FechaCompra = !string.IsNullOrEmpty(dr["FFAPFECCOM"].ToString().Trim()) ? Convert.ToDateTime(dr["FFAPFECCOM"].ToString().Trim()).ToString("dd/MM/yyy") : dr["FFAPFECCOM"].ToString().Trim();
                    compra.HoraCompra = !string.IsNullOrEmpty(dr["FHAPHORCOM"].ToString()) ? Convert.ToDateTime(dr["FHAPHORCOM"]).ToString("hh:mm tt") : dr["FHAPHORCOM"].ToString();
                    compra.CuentaUsuario = new Cuenta
                    {
                        IdCuenta = dr["IDCUENTA"].ToString().Trim(),
                        IdPersona = dr["FIAPIDPERS"].ToString().Trim(),
                        Nombre = dr["FSAPNOMBRE"].ToString().Trim(),
                        ApellidoPaterno = dr["FSAPAPEPAT"].ToString().Trim(),
                        ApellidoMaterno = dr["FSAPAPEMAT"].ToString().Trim(),
                        Correo = dr["FSAPCORREO"].ToString().Trim(),
                        LadaMovil = dr["FIAPLADMOV"].ToString().Trim(),
                        TelefonoMovil = dr["FIAPNUMMOV"].ToString().Trim(),
                    };
                    
                    compra.IdProceso = dr["FIAPIDPROC"].ToString().Trim();
                    compra.RutaReferenciaBancaria = dr["FSAPRUTRFB"].ToString().Trim();
                    compra.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
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
                }

            }
            catch (Exception)
            {
                compra = new OrdenCompraSmartIt();
            }
            return compra;
        }
        internal List<DocumentoSmartIT> GetDocumento(long aIdCompra, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string strSql = string.Empty;
            strSql = @"SELECT FIAPIDCOMP, FIAPIDTIPO, FSAPDOCUME, FSAPRUTDOC,FIAPIDAPPS " +
                        "FROM	" + constantes.Ambiente + "APPS.APDDCPVW " +
                        "WHERE FIAPIDCOMP = " + aIdCompra + " AND FIAPSTATUS = 1" + " AND FIAPIDAPPS = " + aIdApps;
            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            DocumentoSmartIT documento;
            List<DocumentoSmartIT> coleccionDocumento = new List<DocumentoSmartIT>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    documento = new DocumentoSmartIT();
                    documento.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    documento.IdTipo = dr["FIAPIDTIPO"].ToString().Trim();
                    documento.NombreDocumento = dr["FSAPDOCUME"].ToString().Trim();
                    documento.RutaDocumento = dr["FSAPRUTDOC"].ToString().Trim(); 
                    coleccionDocumento.Add(documento);
                }
            }
            else
            {
                documento = new DocumentoSmartIT();
                documento.IdCompra = null;
                documento.IdTipo = null;
                documento.NombreDocumento = null;
                documento.RutaDocumento = null;
                coleccionDocumento.Add(documento);
            }
            return coleccionDocumento;
        }
        internal List<SeguimientoCompra> GetSeguimientoCompra(long aIdCompra, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string strSql = string.Empty;
            strSql = @"SELECT seg.FIAPIDCOMP, seg.FIAPIDAPPS, seg.FIAPIDSEGU, seg.FSAPTITSEG, seg.FIAPIDESTA, est.FSAPESTADO, seg.DATECREAT, seg.TIMECREAT " +
                      "FROM	" + constantes.Ambiente + "APPS.APDSGCVW seg " +
                      "inner join prodapps.APCESCVW est " +
                      "ON seg.FIAPIDESTA = est.FIAPIDESTA "+
                      "WHERE seg.FIAPIDCOMP = " + aIdCompra + " AND seg.FIAPIDAPPS=" + aIdApps + " AND seg.FIAPSTATUS = 1";
            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            SeguimientoCompra seguimiento;
            List<SeguimientoCompra> coleccionSeguimiento = new List<SeguimientoCompra>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    seguimiento = new SeguimientoCompra();
                    seguimiento.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    seguimiento.IdSeguimiento = dr["FIAPIDSEGU"].ToString().Trim();
                    seguimiento.TituloSeguimiento = dr["FSAPTITSEG"].ToString().Trim();
                    seguimiento.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
                    seguimiento.DescripcionEstado = dr["FSAPESTADO"].ToString().Trim();
                    seguimiento.Fecha = dr["DATECREAT"].ToString().Trim();
                    seguimiento.Hora = dr["TIMECREAT"].ToString().Trim();
                    coleccionSeguimiento.Add(seguimiento);
                }
            }
            else
            {
                seguimiento = new SeguimientoCompra();
                seguimiento.IdCompra = null;
                seguimiento.IdSeguimiento = null;
                seguimiento.TituloSeguimiento = null;
                seguimiento.IdEstado = null;
                seguimiento.DescripcionEstado = null;
                coleccionSeguimiento.Add(seguimiento);
            }
            return coleccionSeguimiento;
        }
    }
}