using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Models;
using wsApiVW.Models.User;

namespace wsApiVW.Bussine
{
    public class ContactoCliente
    {
        static string RutaResources = ConfigurationManager.AppSettings["RutaResources"];
        internal Task<Respuesta> EnviarCorreoContactoDeCliente([FromBody] EnviarCorreoContactoCliente CorreoContactoCliente)
        {
            return Task.Run(() =>
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    List<CorreosSoporte> _list = new List<CorreosSoporte>();
                    string mailFrom = "notificaciones@grupoautofin.com";
                    string password = "Z0cW3aiXoJwyMl";
                    string smtpServidor = "smtp.office365.com";
                    string mensaje = CorreoContactoCliente.Texto;
                    string SqlCorreos = string.Empty;
                    string subject = "Contacto";

                    SmtpClient client = new SmtpClient(smtpServidor, 587);
                    MailAddress from = new MailAddress(mailFrom);
                    MailMessage message = new MailMessage();

                    string strJSON = File.ReadAllText(RutaResources + "CorreosSoporteApp.json");
                    _list = JsonConvert.DeserializeObject<List<CorreosSoporte>>(strJSON);
                    
                    if (_list.Count > 0)
                    {
                        foreach (CorreosSoporte item in _list)
                        {
                            message.To.Add(new MailAddress(item.Correo));
                        }
                    }

                    message.To.Add(new MailAddress(CorreoContactoCliente.CorreoCliente));
                    message.From = from;

                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.Subject = subject;
                    message.Body = @"<!DOCTYPE html> <html lang='en'> <head> <meta charset='UTF-8'> 
                                    <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1.0'> 
                                    <title>SEAT</title> </head> <body> <table> <thead> <tr style='height: 67px;'> <th style='text-align: center; color: white; 
                                    font-size: 25px; font-weight: bold; padding: 10px; border-top-left-radius: 30px; border-top-right-radius: 30px; border-bottom-right-radius: 0px; 
                                    border-bottom-left-radius: 0px; background-color:  rgb(52, 52, 65) !important; height: 67px; width: 436.9375px;'> <span style='font-family: Arial, 
                                    Helvetica, sans-serif;'><br>NOTIFICACIÓN AYUDA CLIENTE </span></th> </tr> </thead> <tbody> <tr> <td colspan='2' style='min-height:100px; font-size:16px; text-align:justify; 
                                    padding:15px; width:100%;'> <table style='width:100%;'> <tbody> <tr style='text-align: right;'> <td><span style='font-family: Arial, Helvetica, sans-serif;'>" + CorreoContactoCliente.NombreCliente + "</span></td> </tr> <tr> <td style='font-weight:bold;'><span style='font-family: Arial, Helvetica, sans-serif;'>Buen d&iacute;a,</span></td> </tr> <tr style='text-align: center;'> <td><span style='font-family: Arial, Helvetica, sans-serif;'>" + mensaje + "</span></td> </tr> </tbody> </table> </td> </tr> </tbody> <thead> <tr> <th colspan='2' style='background-color:  rgb(52, 52, 65)!important; color: white; font-size: 18px; font-weight: bold; padding: 10px; border-radius: 0px 0px 30px 30px;'> <span style='font-family: Arial, Helvetica, sans-serif;'>Grupo Autofin M&eacute;xico.</span> <hr><span style='font-size: 11px;'>Por favor no responda este mensaje dado que fue generado de manera autom&aacute;tica y es solo para su informaci&oacute;n</span> </th> </tr> </thead> </table> <p style='text-align: center;'><br></p> </body> </html>";
                    message.IsBodyHtml = true;
                    client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
                    client.EnableSsl = true;
                    client.Send(message);

                    respuesta.Ok = "SI";
                    respuesta.Mensaje = "Correo enviado satisfactoriamente.";
                    respuesta.Objeto = "";

                    return respuesta;
                }

                catch (Exception ex)
                {
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "No se pudo enviar el correo.";
                    respuesta.Objeto = "";
                    return new Respuesta();
                }
            });
        }

        internal Respuesta DeleteNotificaciones(string aIdCuenta, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            SQLTransaction _sql = new SQLTransaction();
            Respuesta _resp = new Respuesta();
            string sql = string.Empty;
           
            try
            {
                sql = $@"UPDATE PRODAPPS.APDNOTVW 
                        SET FIAPSTATUS = 0, DATEUPDAT = CURRENT_DATE, TIMEUPDAT = CURRENT_TIME
                        WHERE FIAPIDCUEN = {aIdCuenta}
                            AND FIAPIDAPPS = {aIdApps}";
                if (!_sql.SQLGuardaTabla(sql))
                    throw new Exception("No fue posible actulizar el token.");
                _resp.Ok = "SI";
                _resp.Mensaje = "Notificaciones borradas";
                _resp.Objeto = "";
            }
            catch (Exception)
            {
                _resp.Ok = "NO";
                _resp.Mensaje = "Notificaciones no borradas";
                _resp.Objeto = "";
            }
            return _resp;
        }
        internal List<Notificacion> ObtenerNotificacionesCuenta(string aIdCuenta, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = string.Empty;
            strSql = @"SELECT * FROM PRODAPPS.APDNOTVW NOTI " +
                      "LEFT JOIN PRODGRAL.GECCIAUN AGEN " +
                      "ON	NOTI.FIAPIDCIAU = AGEN.FIGEIDCIAU " +
                          "WHERE NOTI.FIAPSTATUS = 1 " +
                              "AND NOTI.FIAPIDCUEN = " + aIdCuenta + " " +
                              "AND NOTI.FIAPIDAPPS = " + aIdApps + " ORDER BY FFAPNOTIFI, FHAPNOTIFI DESC";


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
                notificacion.AplicaSeguimiento = dr["FIAPAPLSEG"].ToString().Trim();
                notificacion.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                notificacion.IdPreorden = dr["FIAPIDPREO"].ToString().Trim();
                notificacion.AplicaEncuesta = dr["FIAPAPLENC"].ToString().Trim();
                notificacion.NombreLogo = dr["FSGENOMLOG"].ToString().Trim();
                notificacion.IdEncuesta = dr["FIAPIDENPE"].ToString().Trim();
                notificacion.Instrucciones = dr["FSAPINSTRU"].ToString().Trim();
                notificacion.Visto = dr["FIAPVISTO"].ToString().Trim();
                coleccionNotificaciones.Add(notificacion);
            }
            return coleccionNotificaciones;
        }
    }
}