using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using wsApiVW.Models;

namespace wsApiVW.Bussine
{
    public class BPedido
    {
        internal Task<Respuesta> UpdateDocContactoAgencia(long aIdCompra, int IdConsecutivo, int IdTipoDocumento, string Documento, string path, string aIdApps)
        {
            return Task.Run(() =>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                ApuntadorDeServicio _prod = new ApuntadorDeServicio();
                try
                {
                    DVAConstants.Constants constantes = new DVAConstants.Constants();
                    dbCnx.AbrirConexion();
                    dbCnx.BeginTransaccion();
                    string strSql = string.Empty;
                    strSql = @"UPDATE PRODAPPS.APDDCSVW SET FSAPDOCUME = '" + Documento.Trim() + "', FSAPRUTDOC ='" + _prod.Respuestservicio() + "/Compras/ComunicacionAgencia/" + path + "', USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE , TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FIAPIDCOMP = " + aIdCompra + " AND FIAPIDTIPO = " + IdTipoDocumento + " AND FIAPIDCONS = " + IdConsecutivo + " AND FIAPSTATUS = 1 " + "AND FIAPIDAPPS = " + aIdApps;
                    dbCnx.SetQuery(strSql);
                    dbCnx.CommitTransaccion();
                    dbCnx.CerrarConexion();
                    return new Respuesta() { Ok = "SI", Mensaje = "Los documentos se remplazaron correctamente." };
                }
                catch (Exception)
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();
                    return new Respuesta() { Ok = "NO", Mensaje = "No se pudieron subir los archivo." };
                }
            });
        }

        internal Task<Respuesta> PostSubirDocumentoComunicacionAgencia(long aIdCompra, int IdConsecutivo, int IdTipoDocumento, string nombreDoc, string path, string aIdApps)
        {
            return Task.Run(() =>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                try
                {
                    ApuntadorDeServicio _prod = new ApuntadorDeServicio();
                    DVAConstants.Constants constantes = new DVAConstants.Constants();
                    dbCnx.AbrirConexion();
                    dbCnx.BeginTransaccion();
                    string strSql = "";
                    strSql = @"INSERT INTO PRODAPPS.APDDCSVW
                    (
                       FIAPIDAPPS
                      ,FIAPIDCOMP   /*ID COMPRA*/
                      ,FIAPIDCONS  /*ID CONSECUTIVO*/
                      ,FIAPIDTIPO   /*ID TIPO*/
                      ,FSAPDOCUME   /*DOCUMENTO*/
                      ,FSAPRUTDOC   /*RUTA DOCUMENTO*/
                      ,FIAPSTATUS   /*ESTATUS*/
                      ,USERCREAT    /*USUARIO DE CREACION*/
                      ,DATECREAT    /*FECHA DE CREACION*/
                      ,TIMECREAT    /*HORA DE CREACION*/
                      ,PROGCREAT    /*PROGRAMA DE CREACION*/

                   ) VALUES(" +
                       aIdApps + " , " +
                       aIdCompra + " , " +
                       IdConsecutivo + " , " +
                       IdTipoDocumento + " , " +
                       "'" + nombreDoc + "'" + " , " +
                       "'" + _prod.Respuestservicio() + "/Compras/ComunicacionAgencia/" + path + "'" + " , " +
                         1 + " , " +
                    "'APP' , " +
                    "CURRENT DATE , " +
                    "CURRENT TIME , " +
                    "'APP'" + ")";
                    dbCnx.SetQuery(strSql);
                    dbCnx.CommitTransaccion();
                    dbCnx.CerrarConexion();
                    return new Respuesta() { Ok = "SI", Mensaje = "Se subió su documento de forma satisfactoria." };
                }
                catch (Exception _exc)
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();
                    return new Respuesta() { Ok = "No", Mensaje = _exc.Message };
                }
            });
        }

        internal Task<Respuesta> UpdateRegister(long idCompra, int idTipoDocumento, string Documento, string path, string aIdApss)
        {
            return Task.Run(() =>
            {
                ApuntadorDeServicio _prod = new ApuntadorDeServicio();
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                string strSql = string.Empty;
                strSql = @"UPDATE PRODAPPS.APDDCPVW SET FSAPDOCUME = '" + Documento.Trim() + "', FSAPRUTDOC ='" + _prod.Respuestservicio() + "/Compras/Documentacion/" + path + "', USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE , TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FIAPIDCOMP = " + idCompra + " AND FIAPIDTIPO = " + idTipoDocumento + " AND FIAPIDAPPS=" + aIdApss;
                dbCnx.SetQuery(strSql);

                return new Respuesta() { Ok = "SI", Mensaje = "Los documentos se remplazaron correctamente" };
            });
        }
        internal Task<Respuesta> PostSubirDocumentos(long idCompra, int idTipoDocumento, string nombreDoc, string path, string aIdApps)
        {
            return Task.Run(() =>
            {
                try
                {
                    ApuntadorDeServicio _prod = new ApuntadorDeServicio();
                    DVADB.DB2 dbCnx = new DVADB.DB2();
                    string strSql = string.Empty;
                    strSql = @"INSERT INTO PRODAPPS.APDDCPVW
                    (
                    FIAPIDAPPS,
                    FIAPIDCOMP,      /*ID COMPRA   */
                    FIAPIDTIPO,      /*ID TIPO DOC*/
                    FSAPDOCUME,      /*NOM. DOCUMENTO*/	
                    FSAPRUTDOC,      /*RUTA DOCUMENTO*/
                    FIAPSTATUS,      /*ESTATUS       */  
                    USERCREAT,       /*USUARIO CREACION*/
                    DATECREAT,       /*FECHA CREACION   */
                    TIMECREAT,       /*HORA CREACION    */
                    PROGCREAT       /*PROGRAMA CREACION*/
                   ) VALUES(" +
                       aIdApps + "," +
                       idCompra + " , " +
                       idTipoDocumento + " , " +
                       "'" + nombreDoc + "'" + " , " +
                       "'" + _prod.Respuestservicio() + "/Compras/Documentacion/" + path + "'" + " , " +
                         1 + " , " +
                    "'APP' , " +
                    "CURRENT DATE , " +
                    "CURRENT TIME , " +
                    "'APP'" + ")";
                    dbCnx.SetQuery(strSql);
                    return new Respuesta() { Ok = "SI", Mensaje = "Su documentación se subió correctamente." };
                }
                catch (Exception _exc)
                {
                    throw _exc;
                }
            });
        }
        public static string FormatoRfc(string rfc)
        {

            string rfcG = "";

            for (int c = 0; c < rfc.Length; c++)
            {

                if (rfcG.Length <= 5)
                {
                    if (Char.IsDigit(rfc[c]))
                    {
                        rfcG += rfc[c];
                    }
                }
            }

            return rfc.Replace(rfcG, "-*-").Replace("*", rfcG);
        }
        internal Task<Respuesta> UpdateEstadoCompraSmart(long aIdCompra, int aIdEstado, string aIdApps)
        {
            return Task.Run(() =>
            {
                Respuesta respuesta = new Respuesta();
                DVADB.DB2 dbCnx = new DVADB.DB2();
                respuesta.Ok = "No";
                try
                {
                    string strSql = string.Empty;
                    strSql = @"UPDATE PRODAPPS.APECMPVW SET FIAPIDESTA = " + aIdEstado +
                                " WHERE FIAPIDCOMP =" + aIdCompra + " AND FIAPIDAPPS =" + aIdApps;
                    dbCnx.SetQuery(strSql);
                    respuesta.Ok = "Si";
                    respuesta.Mensaje = "Se actualizo exitosamente";
                    respuesta.Objeto = "";
                }
                catch (Exception)
                {
                    respuesta.Mensaje = "Hubo un error al actualizar el estado";
                    respuesta.Objeto = "";
                }

                return respuesta;
            });

        }
    }
}