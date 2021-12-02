using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Services;
using static wsApiVW.Services.CorreoService;

namespace wsApiVW.Models.User
{
    public class CuentasUsuarios
    {
        private ApuntadorDeServicio _ob;
        public CuentasUsuarios()
        {
            _ob = new ApuntadorDeServicio();
        }

        internal Respuesta PutToken(string aIdCuenta, string aToken, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Respuesta _respuesta = new Respuesta();
            SQLTransaction _sql = new SQLTransaction();
            string strSql = string.Empty;
            try
            {
                strSql = @"SELECT A.FIAPIDCUEN, A.FIAPSTATUS, A.FIAPIDAPPS,A.FSAPCORREO,B.FSAPTOKEN FROM PRODAPPS.APCCTAVW A " +
                        "INNER JOIN PRODAPPS.APCTOKEN B ON A.FIAPIDCUEN = B.FIAPIDCUEN AND A.FIAPIDAPPS = B.FIAPIDAPPS " +
                        "WHERE A.FIAPIDAPPS = " + aIdApps + " AND A.FIAPIDCUEN = " + aIdCuenta;
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
                if (dt.Rows.Count == 0)
                    throw new Exception("No existe la cuenta.");

                strSql = string.Empty;
                strSql = @"UPDATE PRODAPPS.APCTOKEN A " +
                          "SET A.FSAPTOKEN = '" + aToken + "' ,A.USERUPDAT = 'APPS' ,A.DATEUPDAT = CURRENT_DATE ,A.TIMEUPDAT= CURRENT_TIME " +
                          "WHERE A.FIAPIDCUEN = '" + aIdCuenta + "' AND A.FIAPIDAPPS= '" + aIdApps + "'";

                if (!_sql.SQLGuardaTabla(strSql))
                    throw new Exception("No fue posible actulizar el token.");

                _respuesta.Ok = "SI";
                _respuesta.Mensaje = "Se ha actualizado con éxito.";
                _respuesta.Objeto = "";
            }
            catch (Exception ex)
            {
                _respuesta.Ok = "No";
                _respuesta.Mensaje = ex.Message.ToString();
                _respuesta.Objeto = "";
            }
            return _respuesta;
        }
        internal List<Notificacion> GetNotificacionesCompra(string aIdCompra, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = string.Empty;
            strSql = @"select noti.* " +
                    "from	PRODAPPS.APECMPVW compr " +
                    "inner join PRODAPPS.APDNOTVW noti " +
                        "on compr.FIAPIDCUEN = noti.FIAPIDCUEN and noti.FIAPSTATUS = 1 " +
                        "and compr.FIAPIDAPPS = noti.FIAPIDAPPS " + 
                    "left join " + constantes.Ambiente + "GRAL.GECCIAUN agen " +
                         "on noti.FIAPIDCIAU = agen.FIGEIDCIAU " +
                    "where compr.FIAPIDCOMP = " + aIdCompra + " " +
                    "and compr.FIAPIDAPPS = " + aIdApps + " " + 
                    " order by FFAPNOTIFI, FHAPNOTIFI DESC";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            DataView dtX = dt.DefaultView;
            dtX.Sort = "FFAPNOTIFI DESC";
            dt = dtX.ToTable();
            Notificacion notificacion;
            List<Notificacion> coleccionNotificaciones = new List<Notificacion>();
            foreach (DataRow dr in dt.Rows)
            {
                notificacion = new Notificacion();
                notificacion.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                notificacion.IdNotificacion = dr["FIAPIDNOTI"].ToString().Trim();
                notificacion.FechaNotificacion = DateTime.Parse(dr["FFAPNOTIFI"].ToString().Trim()).ToString("dd-MM-yyyy");
                notificacion.HoraNotificacion = DateTime.Parse(dr["FHAPNOTIFI"].ToString().Trim()).ToString("HH:mm:ss");
                notificacion.Asunto = dr["FSAPASUNTO"].ToString().Trim();
                notificacion.DescripcionNotificacion = dr["FSAPNOTIFI"].ToString().Trim();
                notificacion.Instrucciones = dr["FSAPINSTRU"].ToString().Trim();
                notificacion.Visto = dr["FIAPVISTO"].ToString().Trim();
                coleccionNotificaciones.Add(notificacion);
            }
            return coleccionNotificaciones;
        }
        internal RespuestaTest<Cuenta> PutActualizaNotificacionVisto(long IdCuenta, int IdNotificacion, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            RespuestaTest<Cuenta> respuesta = new RespuestaTest<Cuenta>();
            SQLTransaction _save = new SQLTransaction();

            string strSqlSeg = string.Empty;
            strSqlSeg = @"SELECT * FROM PRODAPPS.APDNOTVW " +
                        "WHERE  FIAPIDCUEN = " + IdCuenta + " " +
                        "AND FIAPIDAPPS = " + aIdApps + " " + 
                        "AND FIAPIDNOTI = " + IdNotificacion;
            DataTable dt = dbCnx.GetDataSet(strSqlSeg).Tables[0];

            try
            {
                if (dt.Rows.Count == 0)
                    throw new Exception();

                strSqlSeg = @"UPDATE " + constantes.Ambiente + "APPS.APDNOTVW "
                        + "SET FIAPVISTO = " + 1 + ", "
                        + "USERUPDAT = 'APPS', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APPS' "
                        + "WHERE FIAPIDCUEN = " + IdCuenta + " "
                        + "AND FIAPIDAPPS = " + aIdApps + " "
                        + "AND FIAPSTATUS = 1 "
                        + "AND FIAPIDNOTI = " + IdNotificacion;

                if (!_save.SQLGuardaTabla(strSqlSeg))
                    throw new Exception();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se cambió el estado a visto de la notificación";
                respuesta.Objeto = null;
            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo cambiar el estado a visto la notificación";
                respuesta.Objeto = null;
            }
            return respuesta;
        }

        internal RespuestaTest<Cuenta> IniciarSesionRedesSociales(string Correo, string aIdApps)
        {
            RespuestaTest<Cuenta> respuesta = new RespuestaTest<Cuenta>();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Cuenta cuenta;
            try
            {
                string strSql = string.Empty;
                strSql = @"SELECT A.FIAPIDCUEN, A.FIAPIDPERS,  A.FIAPSTATUS, A.FIAPIDAPPS,A.FSAPNOMBRE,A.FSAPAPEPAT, A.FSAPAPEMAT,A.FSAPCORREO,A.FIAPLADMOV, A.FIAPNUMMOV, A.FSAPRUTFOT, B.FSAPTOKEN, B.FSAPCVEACT  FROM PRODAPPS.APCCTAVW A 
                            INNER JOIN PRODAPPS.APCTOKEN B ON 
                            A.FIAPIDCUEN = B.FIAPIDCUEN 
                            AND A.FIAPIDAPPS = B.FIAPIDAPPS " +
                            "WHERE LOWER(TRIM(A.FSAPCORREO)) = '" + Correo.Trim().ToLower() + "' " +
                            "AND A.FIAPIDAPPS = " + aIdApps;
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count == 0)
                    throw new Exception("No existe la cuenta.");

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["FIAPSTATUS"].ToString().Trim() == "0")
                        throw new Exception("La cuenta existe pero está inactiva, por favor actualice la clave.");
                    cuenta = new Cuenta();
                    cuenta.IdPersona = dr["FIAPIDPERS"].ToString().Trim();
                    cuenta.Clave = dr["FSAPCVEACT"].ToString().Trim();
                    cuenta.Token = dr["FSAPTOKEN"].ToString().Trim();
                    cuenta.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                    cuenta.Nombre = dr["FSAPNOMBRE"].ToString().Trim();
                    cuenta.ApellidoPaterno = dr["FSAPAPEPAT"].ToString().Trim();
                    cuenta.ApellidoMaterno = dr["FSAPAPEMAT"].ToString().Trim();
                    cuenta.Correo = dr["FSAPCORREO"].ToString().Trim();
                    cuenta.LadaMovil = dr["FIAPLADMOV"].ToString().Trim();
                    cuenta.TelefonoMovil = dr["FIAPNUMMOV"].ToString().Trim();
                    respuesta.Ok = "SI";
                    respuesta.Mensaje = "Inicio de sesión con éxito.";
                    respuesta.Objeto = cuenta;
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = ex.Message.ToString();
                respuesta.Objeto = null;
            }
            return respuesta;
        }
        internal RespuestaTest<Cuenta> GetObtieneRutaFotoPerfil(long aIdCuenta, string aIdApps)
        {
            RespuestaTest<Cuenta> respuesta = new RespuestaTest<Cuenta>();
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                string strSql = string.Empty;
                strSql = @"SELECT FSAPRUTFOT " +
                        "FROM    " + constantes.Ambiente + "APPS.APCCTAVW " +
                        "WHERE FIAPIDCUEN = " + aIdCuenta + " AND FIAPSTATUS = 1 " +
                        "AND FIAPIDAPPS = " + aIdApps;
                DataTable DT = dbCnx.GetDataSet(strSql).Tables[0];
                if (DT.Rows.Count == 0)
                    throw new Exception();

                foreach (DataRow dr in DT.Rows)
                {
                    if (dr["FSAPRUTFOT"].ToString().Trim() == "" || dr["FSAPRUTFOT"].ToString().Trim() == null)
                        throw new Exception();
                    respuesta.Ok = "SI";
                    respuesta.Mensaje = dr["FSAPRUTFOT"].ToString().Trim();
                    respuesta.Objeto = null;
                }
            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "ocurrió un error al tratar de obtener la foto de perfil";
                respuesta.Objeto = null;
            }
            return respuesta;
        }

        internal Task<Respuesta> PostActualizaDatosCuenta([FromBody]CuentaActualizar datosPerfil)
        {
            return Task.Run(() =>
            {
                Respuesta respuesta = new Respuesta();
                DVADB.DB2 dbCnx = new DVADB.DB2();
                SQLTransaction _save = new SQLTransaction();
                try
                {
                    string strSqlCuenta = string.Empty;
                    strSqlCuenta = @"UPDATE PRODAPPS.APCCTAVW " +
                                    "SET FSAPNOMBRE = " + "'" + datosPerfil.Nombre.ToString().Trim().ToUpper() + "'" + "," +
                                    "FSAPAPEPAT = " + "'" + datosPerfil.ApellidoPaterno.ToString().Trim().ToUpper() + "'" + "," +
                                    "FSAPAPEMAT = " + "'" + datosPerfil.ApellidoMaterno.ToString().Trim().ToUpper() + "'" + "," +
                                    "FIAPNUMMOV = " + (string.IsNullOrEmpty(datosPerfil.TelefonoMovil.ToString().Trim()) ? "null" : datosPerfil.TelefonoMovil.ToString().Trim()) + ", " +
                                    "USERUPDAT = 'APP' , DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT= 'APP' " +
                                        "WHERE FIAPIDCUEN = " + datosPerfil.IdCuenta + " " +
                                        "AND lower(TRIM(FSAPCORREO)) =" + "'" + datosPerfil.Correo.ToString().Trim().ToLower() + "'";
                    if (!_save.SQLGuardaTabla(strSqlCuenta))
                        throw new Exception();
                    respuesta.Ok = "SI";
                    respuesta.Mensaje = "Datos actualizados correctamente.";
                    respuesta.Objeto = null;
                }
                catch (Exception)
                {
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "No se pudo actualizar la información";
                    respuesta.Objeto = null;
                }
                return respuesta;
            });
        }
        internal Task<Respuesta> UpdateRegisterFotoPerfil(long aIdCuenta, string path, string aIdApps)
        {
            return Task.Run(() =>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                string strSql = string.Empty;
                strSql = @"UPDATE PRODAPPS.APCCTAVW " +
                            "SET FSAPRUTFOT = " + "'" + _ob.Respuestservicio() + "/Cuentas/FotoPerfil/" + path + "'" +
                            ",PROGCREAT = 'APP' " +
                            ",USERUPDAT='APP' " +
                            ",DATEUPDAT = CURRENT DATE " +
                            ",TIMEUPDAT = CURRENT TIME " +
                            ",PROGUPDAT = 'APP' " +
                            " WHERE FIAPIDCUEN = " + aIdCuenta +
                            " AND FIAPIDAPPS = " + aIdApps + 
                            " AND FIAPSTATUS = 1";
                dbCnx.SetQuery(strSql);
                return new Respuesta() { Ok = "SI", Mensaje = "Se remplazó la foto de perfil correctamente." };
            });
        }
        internal Task<Respuesta> PostSubirFoto(long aIdCuenta, string path, string aIdApps)
        {
            return Task.Run(() =>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                string strSql = string.Empty;
                strSql = @"UPDATE PRODAPPS.APCCTAVW " +
                            "SET FSAPRUTFOT = " + "'" + _ob.Respuestservicio() + "/Cuentas/FotoPerfil/" + path  + "'" +
                            ",PROGCREAT = 'APP' " +
                            ",USERUPDAT='APP' " +
                            ",DATEUPDAT = CURRENT DATE " +
                            ",TIMEUPDAT = CURRENT TIME " +
                            ",PROGUPDAT = 'APP' " +
                            " WHERE FIAPIDCUEN = " + aIdCuenta +
                            " AND FIAPIDAPPS = " + aIdApps +
                            " AND FIAPSTATUS = 1";

                dbCnx.SetQuery(strSql);
                return new Respuesta() { Ok = "SI", Mensaje = "Se guardó correctamente la foto de perfil." };
            });
        }
        internal RespuestaTest<Cuenta> IniciarSesion(string Correo, string Clave, string aIdApps)
        {
            RespuestaTest<Cuenta> respuesta = new RespuestaTest<Cuenta>();
            Cuenta _cuenta = new Cuenta();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = string.Empty;
            strSql = @"SELECT A.FIAPIDCUEN, A.FIAPIDPERS,  A.FIAPSTATUS, A.FIAPIDAPPS,A.FSAPNOMBRE,A.FSAPAPEPAT, A.FSAPAPEMAT,A.FSAPCORREO,A.FIAPLADMOV, A.FIAPNUMMOV, A.FSAPRUTFOT, B.FSAPTOKEN, B.FSAPCVEACT  FROM PRODAPPS.APCCTAVW A " +
                        "INNER JOIN PRODAPPS.APCTOKEN B ON " +
                            "A.FIAPIDCUEN = B.FIAPIDCUEN " +
                            "AND A.FIAPIDAPPS = B.FIAPIDAPPS " +
                        "WHERE LOWER(TRIM(A.FSAPCORREO)) = '" + Correo.Trim().ToLower() + "' " +
                        "AND A.FIAPIDAPPS = " + aIdApps + " " +
                        "AND B.FSAPCVEACT = '" + Clave + "'";
            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["FIAPSTATUS"].ToString().Trim() == "0")
                    {
                        respuesta.Ok = "NO";
                        respuesta.Mensaje = "La cuenta existe pero está inactiva, por favor actualice la clave.";
                        respuesta.Objeto = null;
                        return respuesta;
                    }
                    _cuenta.Clave = Clave.Trim();
                    _cuenta.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                    _cuenta.IdPersona = dr["FIAPIDPERS"].ToString().Trim(); 
                    _cuenta.Nombre = dr["FSAPNOMBRE"].ToString().Trim();
                    _cuenta.Token = dr["FSAPTOKEN"].ToString().Trim();
                    _cuenta.ApellidoPaterno = dr["FSAPAPEPAT"].ToString().Trim();
                    _cuenta.ApellidoMaterno = dr["FSAPAPEMAT"].ToString().Trim();
                    _cuenta.Correo = dr["FSAPCORREO"].ToString().Trim();
                    _cuenta.LadaMovil = dr["FIAPLADMOV"].ToString().Trim();
                    _cuenta.TelefonoMovil = dr["FIAPNUMMOV"].ToString().Trim();
                    respuesta.Ok = "SI";
                    respuesta.Mensaje = "Inicio de sesión con éxito.";
                    respuesta.Objeto = _cuenta;
                }
            }
            else
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No es posible iniciar sesión.";
                respuesta.Objeto = null;
            }
            return respuesta;
        }

        internal RespuestaTest<Cuenta> RegistraCuenta([FromBody] RegistraCuenta cuentaJson)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Cuenta _cuenta = new Cuenta();
            CorreoService enviar = new CorreoService();
            RespuestaTest<Cuenta> respuesta = new RespuestaTest<Cuenta>
            {
                Ok = "NO",
                Mensaje = "No es posible registrar su cuenta, intente más tarde.",
                Objeto = null
            };

            string clave = CuentaService.GenerarClaveActivacion();
            string strSql = string.Empty;
            strSql = @"SELECT * FROM " + constantes.Ambiente + "APPS.APCCTAVW CTECV " +
                    "WHERE LOWER(TRIM(CTECV.FSAPCORREO)) = '" + cuentaJson.Correo.Trim().ToLower() + "' " +
                    " AND FIAPSTATUS = 1 " +
                    " AND FIAPIDAPPS = " + cuentaJson.IdApp.Trim();
            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                respuesta.Mensaje = "Ya existe una cuenta con su correo.";
                return respuesta;
            }


            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                strSql = string.Empty;
                strSql = @"SELECT FIAPIDCUEN FROM NEW TABLE ( INSERT INTO PRODAPPS.APCCTAVW " +
                           "(FIAPIDCUEN," +
                            "FIAPIDAPPS," +
                            "FSAPNOMBRE," +
                            "FSAPAPEPAT," +
                            "FSAPAPEMAT," +
                            "FSAPCORREO," +
                            "FIAPLADMOV," +
                            "FIAPNUMMOV," +
                            "FIAPSTATUS," +
                            "USERCREAT," +
                            "PROGCREAT) VALUES(" +
                                $"(SELECT COALESCE(MAX(FIAPIDCUEN), 0) + 1 FROM PRODAPPS.APCCTAVW WHERE FIAPIDAPPS = {cuentaJson.IdApp.Trim()})," +
                                cuentaJson.IdApp.Trim() + "," +
                                "'" + cuentaJson.Nombre.Trim().ToUpper() + "','" +
                                       cuentaJson.ApellidoPaterno.Trim().ToUpper() + "','" +
                                       cuentaJson.ApellidoMaterno.Trim().ToUpper() + "','" +
                                       cuentaJson.Correo.Trim().ToLower() + "'," +
                                       (string.IsNullOrEmpty(cuentaJson.LadaMovil.ToString().Trim()) ? "null" : cuentaJson.LadaMovil.Trim()) + "," +
                                       (string.IsNullOrEmpty(cuentaJson.TelefonoMovil.ToString().Trim()) ? "null" : cuentaJson.TelefonoMovil.ToString().Trim()) + "," +
                                       "1,7244,'APP'))";
                _cuenta.IdCuenta = dbCnx.GetDataSet(strSql).Tables[0].Rows[0]["FIAPIDCUEN"].ToString();

                if (string.IsNullOrEmpty(_cuenta.IdCuenta))
                    throw new Exception();

                strSql = string.Empty;
                strSql = @"INSERT INTO PRODAPPS.APCTOKEN (" +
                          "FIAPIDCUEN," +
                          "FIAPIDAPPS," +
                          "FSAPTOKEN," +
                          "FSAPCVEACT," +
                          "FIAPSTATUS," +
                          "USERCREAT," +
                          "PROGCREAT) VALUES(" +
                            _cuenta.IdCuenta + ",'" +
                            cuentaJson.IdApp.Trim() + "','" +
                            cuentaJson.Token.ToString().Trim() + "','" +
                            clave + "'," +
                            "1,'7244','APPS')";
                dbCnx.SetQuery(strSql);

                _cuenta.IdPersona = "";
                _cuenta.Nombre = cuentaJson.Nombre;
                _cuenta.ApellidoPaterno = cuentaJson.ApellidoPaterno;
                _cuenta.ApellidoMaterno = cuentaJson.ApellidoMaterno;
                _cuenta.Correo = cuentaJson.Correo;
                _cuenta.Clave = clave;
                _cuenta.Token = cuentaJson.Token;
                _cuenta.LadaMovil = cuentaJson.LadaMovil;
                _cuenta.TelefonoMovil = cuentaJson.TelefonoMovil;
                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Registro de cuenta con éxito.";
                respuesta.Objeto = _cuenta;

                if (respuesta.Ok.Equals("SI"))
                {
                    if (!string.IsNullOrEmpty(cuentaJson.Correo.Trim().ToLower()))
                    {
                        string subject = "-Clave de activación APP";
                        string strHtml = "";
                        HttpRequest request = HttpContext.Current.Request;
                        string rutaArchivo = obtenerURLServidor() + "Resources/Email/" + "claveActivacion.html";
                        strHtml = leerArchivoWeb(rutaArchivo);
                        strHtml = strHtml.Replace("[Clave]", clave);
                        enviar.EnviarHtml(subject, cuentaJson.Correo.Trim().ToLower(), strHtml);
                        //HiloEnvioCorreo hiloEnvioCorreo = new HiloEnvioCorreo(subject, cuentaJson.Correo.Trim().ToLower(), strHtml);
                        //hiloEnvioCorreo.EnvioCorreo();
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = "No es posible registrar su cuenta, intente más tarde.";
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
            }
            return respuesta;
        }

        internal RespuestaTest<Cuenta> ActualizarClave(string Correo, string aIdApp)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            SQLTransaction _save = new SQLTransaction();
            RespuestaTest<Cuenta> respuesta = new RespuestaTest<Cuenta>();
            CorreoService enviar = new CorreoService();
            respuesta.Ok = "NO";
            respuesta.Mensaje = "No es posible actualizar su clave.";
            respuesta.Objeto = null;
            string _IdCuenta = string.Empty;

            try
            {

                string clave = CuentaService.GenerarClaveActivacion();
                string strSql = string.Empty;
                strSql = @"SELECT A.FIAPIDCUEN, A.FIAPIDPERS, A.FIAPIDAPPS,A.FSAPNOMBRE,A.FSAPAPEPAT, A.FSAPAPEMAT,A.FSAPCORREO,A.FIAPLADMOV, A.FIAPNUMMOV, A.FSAPRUTFOT, B.FSAPTOKEN, B.FSAPCVEACT  FROM PRODAPPS.APCCTAVW A " +
                            "INNER JOIN PRODAPPS.APCTOKEN B ON " +
                                "A.FIAPIDCUEN = B.FIAPIDCUEN " +
                                "AND A.FIAPIDAPPS = B.FIAPIDAPPS " +
                                    "WHERE	LOWER(TRIM(A.FSAPCORREO)) = '" + Correo.Trim().ToLower() + "' " +
                                    "AND A.FIAPIDAPPS = " + aIdApp + " " +
                                    "AND B.FIAPSTATUS = 1";
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count == 0)
                {
                    respuesta.Mensaje = "No existe una cuenta.";
                    throw new Exception();
                }
                else
                {
                    _IdCuenta = dt.Rows[0]["FIAPIDCUEN"].ToString();
                    strSql = string.Empty;
                    strSql = @"UPDATE PRODAPPS.APCTOKEN A " +
                              "SET A.FSAPCVEACT= '" + clave + "' ,A.USERUPDAT = 'APPS' ,A.DATEUPDAT = CURRENT_DATE ,A.TIMEUPDAT= CURRENT_TIME " +
                              "WHERE A.FIAPIDCUEN= " + _IdCuenta + " " +
                              "AND A.FIAPIDAPPS = " + aIdApp + " " +
                              "AND A.FIAPSTATUS = 1";
                    if (!(_save.SQLGuardaTabla(strSql)))
                        throw new Exception();

                    respuesta.Ok = "SI";
                    respuesta.Mensaje = Correo + " se generó clave con éxito.";
                }
                if (respuesta.Ok.Equals("SI"))
                {
                    if (!string.IsNullOrEmpty(Correo.Trim().ToLower()))
                    {
                        string subject = "-Clave de activación APP";
                        string strHtml = "";
                        HttpRequest request = HttpContext.Current.Request;
                        string rutaArchivo = obtenerURLServidor() + "Resources/Email/" + "claveActivacion.html";
                        strHtml = leerArchivoWeb(rutaArchivo);
                        strHtml = strHtml.Replace("[Clave]", clave);
                        enviar.EnviarHtml(subject, Correo.Trim().ToLower(), strHtml);
                        //HiloEnvioCorreo hiloEnvioCorreo = new HiloEnvioCorreo(subject, Correo.Trim().ToLower(), strHtml);
                        //hiloEnvioCorreo.EnvioCorreo();
                    }
                }
                return respuesta;
            }
            catch (Exception)
            {
                return respuesta;
            }
        }

        public string obtenerURLServidor()
        {
            HttpRequest request = HttpContext.Current.Request;
            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";
            return baseUrl;
        }
        public string leerArchivoWeb(string url)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string output = reader.ReadToEnd();
            response.Close();

            return output;
        }
    }
}