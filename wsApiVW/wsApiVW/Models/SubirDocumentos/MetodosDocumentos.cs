using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace wsApiVW.Models.SubirDocumentos
{
    public class MetodosDocumentos 
    {
        public Task<string> creaImagen(string filePath, string objBase64)
        {
            return Task.Run(() =>
            {
                string cadena = string.Empty;

                try
                {
                    Bitmap bmpReturn = null;
                    byte[] byteBuffer = Convert.FromBase64String(objBase64.Split(',')[1]);
                    MemoryStream memoryStream = new MemoryStream(byteBuffer);
                    memoryStream.Position = 0;
                    bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);
                    memoryStream.Close();
                    memoryStream = null;
                    byteBuffer = null;

                    using (Bitmap bmp = new Bitmap(bmpReturn))
                    {
                        using (Bitmap newImage = new Bitmap(bmp))
                        {
                            newImage.Save(filePath, ImageFormat.Jpeg);
                        }
                    }

                    cadena = "SI";

                    return cadena;

                }
                catch (Exception)
                {

                    cadena = "NO";
                    return cadena;
                }
            });
        }
        internal Task<Respuesta> UpdateRegisterSmartIT(long IdCompra, int consecutivo, int TipoPoliza, string NombreArchivo, long cuenta, string NombreAseguradora, string path, string Total, string Cobertura, string aIdApps)
        {
            return Task.Run(() =>
            {
                ApuntadorDeServicio _prod = new ApuntadorDeServicio();
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                string strSql = string.Empty;
                strSql = @"UPDATE PRODAPPS.APEDVPOL SET FSAPNOASEG= '" + NombreAseguradora + "',FSAPCOBERT= '" + Cobertura + "',FDAPCANTID= " + Math.Round(Convert.ToDecimal(Total), 2) + ",FSAPRUTDOC= '" + _prod.Respuestservicio() + "/Resources/Polizas/" + path + "',FIAPSTATUS= " + 1 + ", USERUPDAT= " + "'APPS'" + ",DATEUPDAT= " + "CURRENT_DATE" + ",TIMEUPDAT=" + "CURRENT_TIME"
                        + " WHERE FIAPIDCOMP= " + IdCompra + " AND FIAPIDCUEN= " + cuenta + " AND FIAPIDTIPO =" + TipoPoliza + " AND  FIAPIDSPOL=" + consecutivo + " AND FIAPIDAPPS =" + aIdApps;
                dbCnx.SetQuery(strSql);
                return new Respuesta() { Ok = "SI", Mensaje = "La poliza se reemplazó correctamente" };
            });
        }
        internal Task<Respuesta> PostSubirPolizaSmartIT(long IdCompra, int consecutivo, int TipoPoliza, string NombreArchivo, long cuenta, string NombreAseguradora, string path, string Total, string Cobertura, string aIdApps)
        {
            return Task.Run(() =>
            {
                ApuntadorDeServicio _prod = new ApuntadorDeServicio();
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                string strSql = string.Empty;
                strSql = @"INSERT INTO PRODAPPS.APEDVPOL
                    (
                       FIAPIDAPPS
                      ,FIAPIDSPOL   /*CONSECUTIVO*/
                      ,FIAPIDCOMP   /*ID COMPRA*/
                      ,FIAPIDCUEN   /*ID CUENTA*/
                      ,FSAPNOASEG   /*NOMBRE DE ASEGURADORA*/
                      ,FSAPCOBERT   /*COBERTURA*/
                      ,FDAPCANTID   /*TOTAL*/
                      ,FIAPIDTIPO   /*TIPO*/
                      ,FSAPDOCUME   /*NOMBRE DOCUMENTO*/
                      ,FSAPRUTDOC   /*RUTA*/
                      ,FIAPSELECT   /*SELECCIONADO*/
                      ,FIAPSTATUS   /*ESTATUS*/
                      ,USERCREAT    /*USUARIO DE CREACION*/
                      ,DATECREAT    /*FECHA DE CREACION*/
                      ,TIMECREAT    /*HORA DE CREACION*/
                      ,PROGCREAT    /*PROGRAMA DE CREACION*/

                   ) VALUES(" +  aIdApps + ", "+ consecutivo + ", " +
                       IdCompra + " , " +
                       cuenta + ",'" +
                       NombreAseguradora + "','" +
                       Cobertura + "'," +
                       Math.Round(Convert.ToDecimal(Total), 2) + "," +
                       "" + TipoPoliza + " , " +
                       "'" + NombreArchivo + " '," +
                       "'" + _prod.Respuestservicio() + "/Resources/Polizas/" + path + "'" + " , " +
                         0 + " , " +
                         1 + " , " +
                    "'APP' , " +
                    "CURRENT DATE , " +
                    "CURRENT TIME , " +
                    "'APP'" + ")";
                dbCnx.SetQuery(strSql);
                return new Respuesta() { Ok = "SI", Mensaje = "Su poliza subió con éxito." };

            });
        }
        internal Task<string> creaPDF(string filePath, string objBase64)
        {
            return Task.Run(() =>
            {
                string cadena = string.Empty;
                byte[] bytes = Convert.FromBase64String(objBase64.Split(',')[0]);
                System.IO.FileStream stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
                System.IO.BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(bytes, 0, bytes.Length);
                writer.Close();
                cadena = "SI";
                return cadena;
            });
        }
        internal Task<Respuesta> MetodoUpdateRegisterComprobantePagos(long aIdCompra, int consecutivo, int aIdTipoDocumento, string Documento, string path, string aIdApps)
        {
            return Task.Run(() =>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                ApuntadorDeServicio _prod = new ApuntadorDeServicio();
                string strSql = string.Empty;
                strSql = @"UPDATE PRODAPPS.APDCMPVW SET FSAPDOCUME = '" + Documento.Trim() + "', FSAPRUTDOC ='" + _prod.Respuestservicio() + "/Compras/ComprobantePagos/" + path + "', FIAPSTATUS = 1, USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE , TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP'" +
                            "WHERE FIAPIDCOMP = " + aIdCompra +
                            " AND FIAPIDTIPO = " + aIdTipoDocumento +
                            " AND  FIAPIDCONS=" + consecutivo +
                            " AND FIAPIDAPPS=" + aIdApps;
                dbCnx.SetQuery(strSql);
                return new Respuesta() { Ok = "SI", Mensaje = "El comprobante de pago se reemplazó correctamente" };
            });
        }

        internal Task<Respuesta> MetodoSubirComprobante(long aIdCompra, int consecutivo, int aIdTipoDocumento, string nombreDoc, string path, string aIdApps)
        {
            return Task.Run(() =>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                ApuntadorDeServicio _prod = new ApuntadorDeServicio();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                try
                {
                    string strSql = string.Empty;
                    strSql = @"INSERT INTO PRODAPPS.APDCMPVW
                    (
                       FIAPIDAPPS
                      ,FIAPIDCONS   /*CONSECUTIVO*/
                      ,FIAPIDCOMP   /*ID COMPRA*/
                      ,FIAPIDTIPO   /*ID TIPO*/
                      ,FSAPDOCUME   /*DOCUMENTO*/
                      ,FSAPRUTDOC   /*RUTA DOCUMENTO*/
                      ,FIAPSTATUS   /*ESTATUS*/
                      ,USERCREAT    /*USUARIO DE CREACION*/
                      ,DATECREAT    /*FECHA DE CREACION*/
                      ,TIMECREAT    /*HORA DE CREACION*/
                      ,PROGCREAT    /*PROGRAMA DE CREACION*/

                   ) VALUES(" +
                       aIdApps + "," + 
                       consecutivo + " , " +
                       aIdCompra + " , " +
                       aIdTipoDocumento + " , " +
                       "'" + nombreDoc + "'" + " , " +
                       "'" + _prod.Respuestservicio() + "/Compras/ComprobantePagos/" + path + "'" + " , " +
                         1 + " , " +
                    "'APP' , " +
                    "CURRENT DATE , " +
                    "CURRENT TIME , " +
                    "'APP'" + ")";
                    dbCnx.SetQuery(strSql);
                    return new Respuesta() { Ok = "SI", Mensaje = "Su pago se subió correctamente." };
                }
                catch (Exception)
                {
                    return new Respuesta() { Ok = "NO", Mensaje = "Intente de nuevo." };
                }
            });
        }
    }
}