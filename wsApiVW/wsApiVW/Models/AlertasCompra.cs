using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Models.SmartIT;

namespace wsApiVW.Models
{
    public class AlertasCompra
    {
        static string llavesAutorizacionNotificaciones = ConfigurationManager.AppSettings["llavesAutorizacionNotificaciones"];

        internal string KeyPush(string aIdApps)
        {
            List<NotificacionXApp> notifica = new List<NotificacionXApp>();
            string strJSON = File.ReadAllText(llavesAutorizacionNotificaciones);
            notifica = JsonConvert.DeserializeObject<List<NotificacionXApp>>(strJSON);
            return  notifica.Where(x => x.IdApps == aIdApps).Select(x => x.Key).First().ToString();
        }
        internal string EnviarConGlobo(string token, string asunto, string mensaje, string aIdApps)
        {
            Alerta alerta = new Alerta();
            alerta.to = token.Trim();
            notification notification = new notification();
            notification.title = asunto;
            notification.body = mensaje;
            notification.badge = "1";
            alerta.notification = notification;

            var client = new RestClient("https://fcm.googleapis.com/fcm/send");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Key={KeyPush(aIdApps)}");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(alerta), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
        internal Task<Respuesta> PostEnviaNotificacion([FromBody] NotificacionCompra NotificacionCompra)
        {
            return Task.Run(() =>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                Respuesta respuesta = new Respuesta();
                string notificacionesSinLeer = "0";
                string instruccion = string.Empty;
                string strSql = string.Empty;

                strSql = @"INSERT INTO " + constantes.Ambiente + "APPS.APDNOTVW (FIAPIDCUEN, FIAPIDNOTI, FFAPNOTIFI, FHAPNOTIFI, FSAPASUNTO, FSAPNOTIFI, FIAPAPLSEG, FIAPIDPREO, FIAPAPLENC, FIAPIDENPE, FSAPINSTRU, FIAPSTATUS, USERCREAT, PROGCREAT,FIAPIDAPPS) "
                            + "VALUES("
                            + NotificacionCompra.IdCuenta + ","
                            + $"(SELECT coalesce(MAX(FIAPIDNOTI),0)+1 ID FROM PRODAPPS.APDNOTVW WHERE FIAPIDAPPS = {NotificacionCompra.IdApps} ),"
                            + "CURRENT DATE" + ","
                            + "CURRENT TIME" + ","
                            + "'" + NotificacionCompra.Asunto + "',"
                            + "'" + NotificacionCompra.DescripcionNotificacion + "',"
                            + "0,"
                            + "default,"
                            + "1,"
                            + "default,"
                            + "'" + instruccion + "'" + ","
                            + "1,'APPS','APPS'," + NotificacionCompra.IdApps  
                            + ")";

                string strSql_a = string.Empty;
                strSql_a = @"SELECT COUNT(*) NOTIFICACIONES FROM " + 
                                        "PRODAPPS.APDNOTVW " + 
                                        "WHERE FIAPIDCUEN = " + NotificacionCompra.IdCuenta + " " +
                                        "AND FIAPIDAPPS = " + NotificacionCompra.IdApps + " "+ 
                                        "AND FIAPVISTO = 0 AND FIAPSTATUS = 1";
                try
                {
                    dbCnx.AbrirConexion();
                    dbCnx.BeginTransaccion();
                    dbCnx.SetQuery(strSql);
                    DataTable notificacionesNL = dbCnx.GetDataSet(strSql_a).Tables[0];
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

                    if (dt.Rows.Count == 0)
                        throw new Exception();

                    token = dt.Rows[0]["FSAPTOKEN"].ToString().Trim();

                    Alerta alerta = new Alerta();
                    alerta.to = token;
                    notification notification = new notification();
                    notification.title = NotificacionCompra.Asunto;
                    notification.body = NotificacionCompra.DescripcionNotificacion;
                    notification.badge = notificacionesSinLeer.Trim();
                    alerta.notification = notification;

                    var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", $"Key={KeyPush(NotificacionCompra.IdApps)}");
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("application/json", JsonConvert.SerializeObject(alerta), ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    Console.WriteLine(response.Content);
                    #endregion
                }
                catch (Exception)
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "No se pudo enviar la notificación";
                    respuesta.Objeto = null;
                }
                return respuesta;
            });
        }

        public class Alerta
        {
            public string to { get; set; }
            public notification notification { get; set; }
        }
        public class notification
        {
            public string title { get; set; }
            public string body { get; set; }
            public string badge { get; set; }
        }

    }
}