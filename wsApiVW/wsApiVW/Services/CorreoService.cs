using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using wsApiVW.Bussine;
using wsApiVW.Models;
using wsApiVW.Models.AutoModels;

namespace wsApiVW.Services
{
    public class CorreoService
    {
        static string RutaResources = ConfigurationManager.AppSettings["RutaResources"];
        public bool EnviarCorreoAutorizacion(List<string> Destinatarios, AutorizacionDocumento doc)
        {
            bool retorno = false;
            try
            {
                string mailFrom = "notificaciones@grupoautofin.com";
                string password = "Z0cW3aiXoJwyMl";
                string smtpServidor = "smtp.office365.com";
                string rutaArchivo;
                SmtpClient client = new SmtpClient(smtpServidor, 587);
                MailAddress from = new MailAddress(mailFrom);
                MailMessage message = new MailMessage();
                foreach (string correoDestino in Destinatarios)
                {
                    message.To.Add(new MailAddress(correoDestino));
                }
                message.From = from;
                message.Subject = "¡Autorizacion de Documento!";
                rutaArchivo = RutaResources + "AutorizaDocumento.html";
                string Leer = File.ReadAllText(rutaArchivo);
                Leer = Leer.Replace("[[MARCA]]", doc.Marca);
                Leer = Leer.Replace("[[FOLIO]]", doc.Folio);
                Leer = Leer.Replace("[[ESTADO]]", doc.Estado);
                Leer = Leer.Replace("[[MOTIVO]]", doc.Motivo);
                message.Body = Leer;
                message.IsBodyHtml = true;
                client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
                client.EnableSsl = true;
                client.Send(message);
                retorno = true;
            }
            catch (Exception)
            {
                retorno = false;
            }
            return retorno;
        }
        private void EnviarCorreoHtml(string subject, string correo, string html)
        {
            string mailFrom = "notificaciones@grupoautofin.com";
            string password = "Z0cW3aiXoJwyMl";
            string smtpServidor = "smtp.office365.com";
            SmtpClient client = new SmtpClient(smtpServidor, 587);
            MailAddress from = new MailAddress(mailFrom);
            MailAddress to = new MailAddress(correo);
            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            string mensaje = "<!doctype html>" + html;
            message.Body = mensaje;
            message.IsBodyHtml = true;
            client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
            client.EnableSsl = true;
            client.Send(message);
        }
        public void EnviarHtml(string Subject, string Correo, string Html)
        {
            EnviarCorreoHtml(Subject, Correo, Html);
        }
        public class HiloEnvioCorreo
        {
            protected string subject;
            protected string correo;
            protected string strHtml;
            public HiloEnvioCorreo(string Subject, string Correo, string StrHtml)
            {
                this.subject = Subject;
                this.correo = Correo;
                this.strHtml = StrHtml;
            }

            public void EnvioCorreo()
            {
                try
                {
                    CorreoService servicioCorreo = new CorreoService();
                    servicioCorreo.EnviarHtml(subject, correo, strHtml);
                }
                catch (Exception e)
                {
                }
            }
        }

        private void EnviarCorreo(string subject, string folio, string mensaje, List<string> correo)
        {
            string mailFrom = "notificaciones@grupoautofin.com";
            string password = "Z0cW3aiXoJwyMl";
            string smtpServidor = "smtp.office365.com";
            SmtpClient client = new SmtpClient(smtpServidor, 587);
            MailAddress from = new MailAddress(mailFrom);
            MailMessage message = new MailMessage();

            foreach (string correoDestino in correo)
            {
                message.To.Add(new MailAddress(correoDestino));
            }

            message.From = from;
            message.Subject = subject;
            message.Body = @"<!DOCTYPE html> <html lang='en'> <head> <meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'> <title>APP</title> 
                            </head> <body> <table style='width:80%; margin:auto;'> <thead> <tr> 
                            <th data-ogsb='rgb(79, 129, 199)' style='text-align: center; background-color:
                            rgb(52, 52, 65) !important; color: white; font-size: 25px; font-weight: bold; padding: 10px; border-radius: 30px 30px 0px 0px;'>
                            <span style='font-family: Arial, Helvetica, sans-serif;'>ACTUALIZACI&Oacute;N DE LA COMPRA</span></th> </tr> </thead> <tbody style='text-align: center;'> 
                            <tr> <td colspan='2' style='min-height:100px; font-size:16px; text-align:justify; padding:15px; width:100%;'> <table style='width:100%;'> 
                            <tbody style='text-align: center;'> <tr> <td><span style='font-weight: bold; font-family: Arial, Helvetica, sans-serif;'>N&uacute;mero de folio:
                            </span><span style='font-family: Arial, Helvetica, sans-serif;'>" + folio + "</span></td> </tr> <tr> <td><span style='font-family: Arial, Helvetica, sans-serif;'>" + mensaje + "</span></td> </tr> </tbody> </table> </td> </tr> <tr> <td> <div style='text-align: center; font-size:12px;'><br></div> </td> </tr> </tbody> <thead> <tr> <th colspan='2' data-ogsb='rgb(79, 129, 199)' style='background-color: rgb(52, 52, 65) !important; color: white; font-size: 18px; font-weight: bold; padding: 10px; border-radius: 0px 0px 30px 30px;'><span style='font-family: Arial, Helvetica, sans-serif;'><u>Grupo Autofin M&eacute;xico</u></span><br> <div style='text-align: center; font-size:12px;'><span style='font-family: Arial, Helvetica, sans-serif; font-size: 8px;'>Por favor no responda este mensaje dado que fue generado de manera autom&aacute;tica y es solo para su informaci&oacute;n</span></div> </th> </tr> </thead> </table> </body> </html>";
            message.IsBodyHtml = true;
            client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
            client.EnableSsl = true;
            client.Send(message);
        }

        public void Enviar(string Subject, string folio, string Mensaje, List<string> Correo)
        {
            EnviarCorreo(Subject, folio, Mensaje, Correo);
        }
        public class HiloEnvioCorreoSoporte
        {
            protected string subject;
            protected List<string> correo;
            protected string mensaje;
            protected string folio;

            public HiloEnvioCorreoSoporte(string Subject, List<string> Correo, string Mensaje, string folio)
            {
                this.subject = Subject;
                this.correo = Correo;
                this.mensaje = Mensaje;
                this.folio = folio;
            }

            public void EnvioCorreoSoporte()
            {
                try
                {
                    CorreoService servicioCorreo = new CorreoService();
                    servicioCorreo.Enviar(subject, folio, mensaje, correo);

                }
                catch (Exception e)
                {

                }
            }
        }

        public class EnvioCorreoSoporte
        {
            protected string subject;
            protected string folio;
            protected string EC;
            protected string PP;
            protected List<string> Destinatarios;
            public EnvioCorreoSoporte(string Subject, string folio, string EC, string PP, List<string> Destinatarios)
            {
                this.subject = Subject;
                this.folio = folio;
                this.EC = EC;
                this.PP = PP;
                this.Destinatarios = Destinatarios;
            }

            public void EnvioCorreoGerentes()
            {
                try
                {
                    CorreoService servicioCorreo = new CorreoService();
                    servicioCorreo.EnviarCorreoGerentes(subject, folio, EC, PP, Destinatarios);
                }
                catch (Exception e)
                {

                }
            }
        }

        public void EnviarCorreoGerentes(string subject, string folio, string EC, string PP, List<string> Destinatarios)
        {
            string mailFrom = "notificaciones@grupoautofin.com";
            string password = "Z0cW3aiXoJwyMl";
            string smtpServidor = "smtp.office365.com";

            SmtpClient client = new SmtpClient(smtpServidor, 587);
            MailAddress from = new MailAddress(mailFrom);
            MailMessage message = new MailMessage();
            foreach (string correoDestino in Destinatarios)
            {
                message.To.Add(new MailAddress(correoDestino));
            }
            message.From = from;
            message.Subject = subject;
            message.Body = @"<!DOCTYPE html> <html lang='en'> <head> <meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'> <title>APP</title> 
                            </head> <body> <table style='width:80%; margin:auto;'> <thead> <tr> <th data-ogsb='rgb(79, 129, 199)' 
                            style='text-align: center; background-color: rgb(52, 52, 65) !important; color: white; font-size: 25px; 
                            font-weight: bold; padding: 10px; border-radius: 30px 30px 0px 0px;'><span style='font-family: Arial, Helvetica, 
                            sans-serif;'>FOLIO NUEVO</span></th> </tr> </thead> <tbody> <tr> <td colspan='2' style='min-height:100px; font-size:16px; 
                            text-align:justify; padding:15px; width:100%;'> <table style='width:100%;'> <tbody style='text-align: center;'> 
                            <tr> <td><span style='font-weight: bold; font-family: Arial, Helvetica, sans-serif;'>N&uacute;mero de folio:</span>
                            <span style='font-family: Arial, Helvetica, sans-serif;'>" + folio + "</span></td> </tr> <tr> <td><span style='font-weight: bold; font-family: Arial, Helvetica, sans-serif;'>Estado de compra:</span><span style='font-family: Arial, Helvetica, sans-serif;'>" + EC + "</span></td> </tr> <tr> <td><span style='font-weight: bold; font-family: Arial, Helvetica, sans-serif;'>Paso del proceso:&nbsp;</span><span style='font-family: Arial, Helvetica, sans-serif;'>" + PP + "</span></td> </tr> </tbody> </table> </td> </tr> <tr> <td><br></td> </tr> </tbody> <thead> <tr> <th colspan='2' data-ogsb='rgb(79, 129, 199)' style='background-color: rgb(52, 52, 65) !important; color: white; font-size: 18px; font-weight: bold; padding: 10px; border-radius: 0px 0px 30px 30px;'><span style='font-family: Arial, Helvetica, sans-serif;'>Grupo Autofin M&eacute;xico</span><br> <div style='text-align: center; font-size:12px;'><span style='font-family: Arial, Helvetica, sans-serif; font-size: 8px;'>Por favor no responda este mensaje dado que fue generado de manera autom&aacute;tica y es solo para su informaci&oacute;n</span></div> </th> </tr> </thead> </table> </body> </html>";
            message.IsBodyHtml = true;
            client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
            client.EnableSsl = true;
            client.Send(message);
        }

        public void EnviarCorreoCliente(string subject, string correo, string mensaje, string StrHtml, int IdAgencia, string IdMarca)
        {
            string mailFrom = "notificaciones@grupoautofin.com";
            string password = "Z0cW3aiXoJwyMl";
            string smtpServidor = "smtp.office365.com";

            string filename = string.Empty;
            Politicas _politicas = new Politicas();
            AutosBussine _bussine = new AutosBussine();
            _politicas = _bussine.RecuperaPoliticas(IdMarca, IdAgencia.ToString());

            Attachment data = new Attachment(_politicas.PoliticasAgencia, MediaTypeNames.Application.Octet);
            SmtpClient client = new SmtpClient(smtpServidor, 587);
            MailAddress from = new MailAddress(mailFrom);
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(correo));
            message.From = from;
            message.Subject = subject;
            string mensajeC = "<!doctype html>" + StrHtml;
            message.Body = mensajeC;
            message.IsBodyHtml = true;
            message.Attachments.Add(data);
            client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
            client.EnableSsl = true;
            client.Send(message);
        }
    }

    public class EnvioCorreoCliente
    {
        protected string subject;
        protected string Correo;
        protected string Mensaje;
        protected string strHtml;
        protected int IdAgencia;
        protected string IdMarca;

        public EnvioCorreoCliente(string Subject, string Correo, string Mensaje, string strHtml, int IdAgencia, string IdMarca)
        {
            this.subject = Subject;
            this.Mensaje = Mensaje;
            this.Correo = Correo;
            this.strHtml = strHtml;
            this.IdAgencia = IdAgencia;
            this.IdMarca = IdMarca;
        }

        public void EnviarCorreoCliente()
        {
            try
            {
                CorreoService servicioCorreo = new CorreoService();
                servicioCorreo.EnviarCorreoCliente(subject, Correo, Mensaje, strHtml, IdAgencia, IdMarca);

            }
            catch (Exception e)
            {

            }
        }
    }
}