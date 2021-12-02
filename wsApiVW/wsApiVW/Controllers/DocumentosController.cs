using System;
using System.Collections.Generic;
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
using wsApiVW.Models.SubirDocumentos;

namespace wsApiVW.Controllers
{
    public class DocumentosController : ApiController
    {
        [Route("api/Documentos/PostSubirDocumentos", Name = "PostSubirDocumentos")]
        [HttpPost]
        public async Task<Respuesta> PostSubirDocumentos(long aIdCompra, int TipoDocumento, string Documento, string aIdApps)
        {
            BPedido service = new BPedido();
            string paths = string.Empty;
            Respuesta respuesta = new Respuesta();
            try
            {
                var ctx = HttpContext.Current;
                var root = HostingEnvironment.MapPath("~" + "/Compras/Documentacion");
                paths = root;
                var provider = new MultipartFormDataStreamProvider(root);
                string path = string.Empty;
                int index = 0;
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.FileData)
                {
                    var name = file.Headers
                        .ContentDisposition
                        .FileName;
                    name = name.Trim('"').ToLower();
                    index = name.IndexOf(".jpg");
                    if (index == -1) { index = name.IndexOf(".pdf"); }
                    if (index == -1) { index = name.IndexOf(".png"); }
                    if (index == -1) { index = name.IndexOf(".jpeg"); }
                    var localFileName = file.LocalFileName;
                    name = aIdApps + "_" + aIdCompra + "_" + TipoDocumento + "_" + DateTime.Now.ToString("yyyyMMdd") + "" + name.Substring(index);
                    var filePath = Path.Combine(root, name);
                    path = filePath;
                    if (!File.Exists(filePath))
                    {
                        File.Move(localFileName, filePath);
                        respuesta = await service.PostSubirDocumentos(aIdCompra, TipoDocumento, Documento, name, aIdApps);
                    }
                    else
                    {
                        File.Delete(filePath);
                        File.Move(localFileName, filePath);
                        respuesta = await service.UpdateRegister(aIdCompra, TipoDocumento, Documento, name, aIdApps);
                    }
                }
            }
            catch (Exception e)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = e.Message + "ruta->:" + paths;
            }
            return respuesta;
        }

        [Route("api/Documentos/PostSubirComprobantePago", Name = "PostSubirComprobantePago")]
        [HttpPost]
        public async Task<Respuesta> PostSubirComprobantePago(long aIdCompra, int Consecutivo, int TipoDocumento, string Documento, string aIdApps)
        {
            MetodosDocumentos doc = new MetodosDocumentos();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string paths = string.Empty;
            Respuesta respuesta = new Respuesta();
            string sqlStr = string.Empty;
            sqlStr = @"SELECT FIAPIDCONS, FIAPIDCOMP, FIAPIDTIPO, FSAPDOCUME, FSAPRUTDOC " +
             "FROM PRODAPPS.APDCMPVW " +
             "WHERE FIAPIDCONS = " + Consecutivo +
                " AND FIAPIDTIPO = " + TipoDocumento +
                " AND FIAPIDCOMP = " + aIdCompra + 
                " AND FIAPIDAPPS = " + aIdApps;

            DataTable comp = dbCnx.GetDataSet(sqlStr).Tables[0];
            if (comp.Rows.Count > 0)
            {
                string strSql = string.Empty;
                strSql = @"UPDATE PRODAPPS.APDCMPVW " +
                            "SET FIAPSTATUS = 0 " +
                            " WHERE FIAPIDTIPO = " + TipoDocumento +
                            " AND FIAPIDCOMP = " + aIdCompra +
                            " AND FIAPIDCONS = " + Consecutivo +
                            " AND FIAPIDAPPS = " + aIdApps;
                dbCnx.SetQuery(strSql);
            }

            #region SubirImagen 
            try
            {
                var ctx = HttpContext.Current;
                var root = HostingEnvironment.MapPath("~" + "/Compras/ComprobantePagos");
                paths = root;
                var provider = new MultipartFormDataStreamProvider(root);
                string path = string.Empty;
                int index = 0;

                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.FileData)
                {
                    var name = file.Headers.ContentDisposition.FileName;
                    name = name.Trim('"').ToLower();
                    index = name.IndexOf(".jpg");
                    if (index == -1) { index = name.IndexOf(".pdf"); }
                    if (index == -1) { index = name.IndexOf(".png"); }
                    if (index == -1) { index = name.IndexOf(".jpeg"); }
                    var localFileName = file.LocalFileName;
                    name = aIdApps + "_" + aIdCompra + "_" + TipoDocumento + "_" + Consecutivo + "_" + DateTime.Now.ToString("yyyyMMdd") + "" + name.Substring(index);
                    var filePath = Path.Combine(root, name);
                    path = filePath;
                    if (!File.Exists(filePath))
                    {
                        File.Move(localFileName, filePath);
                        respuesta = await doc.MetodoSubirComprobante(aIdCompra, Consecutivo, TipoDocumento, Documento, name, aIdApps);
                    }
                    else
                    {
                        File.Delete(filePath);
                        File.Move(localFileName, filePath);
                        respuesta = await doc.MetodoUpdateRegisterComprobantePagos(aIdCompra, Consecutivo, TipoDocumento, Documento, name, aIdApps);
                    }
                }
            }
            catch (Exception e)
            {

                respuesta.Ok = "NO";
                respuesta.Mensaje = e.Message + "ruta->:" + paths;
            }
            #endregion
            return respuesta;
        }

        [Route("api/Documentos/PostSubirPolizasSmartIT", Name = "PostSubirPolizasSmartIT")]
        [System.Web.Http.HttpPost]
        public async Task<Respuesta> SubirPolizasSmartIT([FromBody]SubirArchivoSmartIT archivo)
        {
            Respuesta respuesta = new Respuesta();
            MetodosDocumentos service = new MetodosDocumentos();
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/Resources/Polizas/");
            string salida = string.Empty;
            try
            {
                if (archivo.ExtensionArch.ToLower().Trim() == "pdf")
                {
                    var name =  archivo.IdApps + "_" + archivo.IdCompra + "_" + archivo.IdConsecutivo + "_" + archivo.Tipo + "." + archivo.ExtensionArch;
                    string filePath = root.ToString() + name;
                    if (!File.Exists(filePath))
                    {
                        salida = await service.creaPDF(filePath, archivo.Base64);
                        if (salida == "SI")
                            respuesta = await service.PostSubirPolizaSmartIT(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.Tipo), archivo.NombreDocumento, Convert.ToInt64(archivo.IdCuenta), archivo.NombreAseguradora, name, Math.Round(Convert.ToDecimal(archivo.Total), 2).ToString(), archivo.Cobertura, archivo.IdApps);
                    }
                    else
                    {
                        File.Delete(filePath);
                        salida = await service.creaPDF(filePath, archivo.Base64);
                        if (salida == "SI")
                            respuesta = await service.UpdateRegisterSmartIT(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.Tipo), archivo.NombreDocumento, Convert.ToInt64(archivo.IdCuenta), archivo.NombreAseguradora, name, Math.Round(Convert.ToDecimal(archivo.Total), 2).ToString(), archivo.Cobertura, archivo.IdApps);
                    }
                }
                else if (archivo.ExtensionArch.ToLower().Trim() == "png" || archivo.ExtensionArch.ToLower().Trim() == "jpeg" || archivo.ExtensionArch.ToLower().Trim() == "jpg")
                {
                    var name =  archivo.IdApps + "_" + archivo.IdCompra + "_" + archivo.IdConsecutivo + "_" + archivo.Tipo + "_" + DateTime.Now.ToString("yyyyMMdd") + ".Jpeg";
                    string filePath = root.ToString() + name;
                    if (!File.Exists(filePath))
                    {
                        salida = await service.creaImagen(filePath, archivo.Base64);
                        if (salida == "SI")
                            respuesta = await service.PostSubirPolizaSmartIT(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.Tipo), archivo.NombreDocumento, Convert.ToInt64(archivo.IdCuenta), archivo.NombreAseguradora, name, Math.Round(Convert.ToDecimal(archivo.Total), 2).ToString(), archivo.Cobertura, archivo.IdApps);
                    }
                    else
                    {
                        File.Delete(filePath);
                        salida = await service.creaImagen(filePath, archivo.Base64);
                        if (salida == "SI")
                            respuesta = await service.UpdateRegisterSmartIT(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.Tipo), archivo.NombreDocumento, Convert.ToInt64(archivo.IdCuenta), archivo.NombreAseguradora, name, Math.Round(Convert.ToDecimal(archivo.Total), 2).ToString(), archivo.Cobertura, archivo.IdApps);
                    }
                }
            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo subir el archivo.";
            }
            return respuesta;
        }

        [Route("api/Documentos/PostSubirDocumentosContactoAgencia", Name = "PostSubirDocumentosContactoAgencia")]
        public async Task<Respuesta> PostSubirDocumentosContactoAgencia([FromBody] SubirArchivo archivo)
        {
            Respuesta respuesta = new Respuesta();
            BPedido services = new BPedido();
            MetodosDocumentos service = new MetodosDocumentos();
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/Compras/ComunicacionAgencia/");
            try
            {
                if (archivo.ExtensionArch.ToLower().Trim() == "pdf")
                {


                    var name = archivo.IdApps + "_" + archivo.IdCompra + "_" + archivo.IdConsecutivo + "_" + archivo.TipoDocumento + "_" + DateTime.Now.ToString("yyyyMMdd") + "." + archivo.ExtensionArch;

                    string filePath = root.ToString() + name;


                    if (!File.Exists(filePath))
                    {

                        string salida = await service.creaPDF(filePath, archivo.Base64);

                        if (salida == "SI")
                        {
                            respuesta = await services.PostSubirDocumentoComunicacionAgencia(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.TipoDocumento), archivo.NombreDocumento, name, archivo.IdApps);
                        }

                    }
                    else
                    {
                        File.Delete(filePath);

                        string salida = await service.creaPDF(filePath, archivo.Base64);
                        if (salida == "SI")
                        {
                            respuesta = await services.UpdateDocContactoAgencia(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.TipoDocumento), archivo.NombreDocumento, name, archivo.IdApps);
                        }

                    }

                }
                else if (archivo.ExtensionArch.ToLower().Trim() == "png" || archivo.ExtensionArch.ToLower().Trim() == "jpeg" || archivo.ExtensionArch.ToLower().Trim() == "jpg")
                {

                    var name = archivo.IdApps + "_" + archivo.IdCompra + "_" + archivo.IdConsecutivo + "_" + archivo.TipoDocumento + "_" + DateTime.Now.ToString("yyyyMMdd") + ".Jpeg";
                    string filePath = root.ToString() + name;

                    if (!File.Exists(filePath))
                    {
                        string salida = await service.creaImagen(filePath, archivo.Base64);

                        if (salida == "SI")
                        {
                            respuesta = await services.PostSubirDocumentoComunicacionAgencia(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.TipoDocumento), archivo.NombreDocumento, name, archivo.IdApps);
                        }

                    }
                    else
                    {

                        File.Delete(filePath);
                        string salida = await service.creaImagen(filePath, archivo.Base64);
                        if (salida == "SI")
                        {
                            respuesta = await services.UpdateDocContactoAgencia(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.TipoDocumento), archivo.NombreDocumento, name, archivo.IdApps);
                        }

                    }
                }
            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo subir el archivo.";
            }
            return respuesta;
        }
    }
}