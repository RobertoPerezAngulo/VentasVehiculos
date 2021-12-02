using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace wsApiVW.Models.PagoElectronico
{
    public class InternalPagoElectronicos
    {
        static string RutaServicio = ConfigurationManager.AppSettings["RutaServicioPagoElectronico"];
        internal async Task<RespuestaTest<OpenPay>> CargoOpenPay([FromBody] VentaOpenPay venta)
        {
            RespuestaTest<OpenPay> _respuesta = new RespuestaTest<OpenPay>();
            SQLTransaction _transaction = new SQLTransaction();
            string strSqlSeg = string.Empty;
            _respuesta.Objeto = null;
            _respuesta.Ok = "NO";
            try
            {
                RequestOpenPay _EnviOpenPay = new RequestOpenPay()
                {
                    Cantidad = Math.Round(Convert.ToDecimal(venta.Cantidad), 2).ToString(),
                    Descripction = venta.Descripcion,
                    OrderId = "",
                    InformacionCliente = new DatosCliente()
                    {
                        Nombre = venta.InformacionCliente.Nombre,
                        ApellidoPaterno = venta.InformacionCliente.ApellidoPaterno,
                        Correo = venta.InformacionCliente.Correo,
                        Telefono = venta.InformacionCliente.Telefono
                    }
                };

                string json = JsonConvert.SerializeObject(_EnviOpenPay);
                string url = RutaServicio + "/wsApiEPayment/api/openpay/PostCargoId?aIdEmpresa=" + venta.IdEmpresa + "&aIdT=" + venta.IdToken + "&aDispositivoId=" + venta.DispositivoId;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var obj = streamReader.ReadToEnd();
                    _respuesta.Objeto = JsonConvert.DeserializeObject<OpenPay>(obj);

                    if (string.IsNullOrEmpty(_respuesta.Objeto.IdCargo))
                    {
                        OpenPayBad badResultObject = JsonConvert.DeserializeObject<OpenPayBad>(obj);
                        _respuesta.Mensaje = badResultObject.description;
                        _respuesta.Objeto = null;

                        strSqlSeg = @"INSERT INTO PRODCAJA.CADPAEVW (
                                    FICAIDPAGO,
                                    FIAPIDCOMP,
                                    FIAPIDAPPS,
                                    FICAIDPAGE,
                                    FICAIDCIAU,
                                    FICAIDCUEN,
                                    FICAUTORIZ,
                                    FDCACANTID,
                                    FSCAIDAUTO,
                                    FSCADESCRV,
                                    FIAPIDTIPO,
                                    FFCATRANPE,
                                    FHCATRANPE,
                                    FSCASOJSON,
                                    FSCAREJSON,
                                    FICASTATUS,
                                    USERCREAT,
                                    DATECREAT, 
                                    TIMECREAT,
                                    PROGCREAT ) VALUES(" +
                                    "(SELECT coalesce(MAX(FICAIDPAGO),0)+1 ID FROM PRODCAJA.CADPAEVW)" + "," +
                                    venta.IdCompra + "," +
                                    venta.IdApps + "," +
                                    badResultObject.IdPagoElectronico + "," +
                                    venta.IdEmpresa + "," +
                                    venta.IdCuenta + "," +
                                    0 + "," +
                                    venta.Cantidad + "," +
                                    0 + ",'" +
                                    badResultObject.description + "'," +
                                    venta.TipoPago + ",'" +
                                    Convert.ToDateTime(badResultObject.FechaCreacion).ToString("yyyy-MM-dd") + "','" +
                                    badResultObject.HoraCreacion + "','" +
                                    Newtonsoft.Json.JsonConvert.SerializeObject(venta) + "','" +
                                    Newtonsoft.Json.JsonConvert.SerializeObject(badResultObject) + "'," +
                                    1 + "," +
                                    "'APPS'" + "," +
                                    "CURRENT_DATE" + "," +
                                    "CURRENT_TIME" + "," +
                                    "'APPS'" + ")";
                        if (!_transaction.SQLGuardaTabla(strSqlSeg))
                            throw new System.ArgumentException();
                    }
                    else
                    {
                        _respuesta.Ok = "SI";
                        _respuesta.Mensaje = "Se Cargó Exitosamente";

                        strSqlSeg = @"INSERT INTO PRODCAJA.CADPAEVW (
                                    FICAIDPAGO,
                                    FIAPIDCOMP,
                                    FIAPIDAPPS,
                                    FICAIDPAGE,
                                    FICAIDCIAU,
                                    FICAIDCUEN,
                                    FICAUTORIZ,
                                    FDCACANTID,
                                    FSCAIDAUTO,
                                    FSCADESCRV,
                                    FIAPIDTIPO,
                                    FFCATRANPE,
                                    FHCATRANPE,
                                    FSCASOJSON,
                                    FSCAREJSON,
                                    FICASTATUS,
                                    USERCREAT,
                                    DATECREAT, 
                                    TIMECREAT,
                                    PROGCREAT ) VALUES(" +
                                    "(SELECT coalesce(MAX(FICAIDPAGO),0)+1 ID FROM PRODCAJA.CADPAEVW)" + "," +
                                    venta.IdCompra + "," +
                                    venta.IdApps + "," +
                                     _respuesta.Objeto.IdPagoElectronico + "," +
                                    venta.IdEmpresa + "," +
                                    venta.IdCuenta + "," +
                                    1 + "," +
                                    venta.Cantidad + "," +
                                    _respuesta.Objeto.Autorizacion + ",'" +
                                    venta.Descripcion + "'," +
                                    venta.TipoPago + ",'" +
                                    Convert.ToDateTime(_respuesta.Objeto.FechaCreacion).ToString("yyyy-MM-dd") + "','" +
                                    _respuesta.Objeto.HoraCreacion + "','" +
                                    Newtonsoft.Json.JsonConvert.SerializeObject(venta) + "','" +
                                    Newtonsoft.Json.JsonConvert.SerializeObject(_respuesta.Objeto) + "'," +
                                    1 + "," +
                                    "'APPS'" + "," +
                                    "CURRENT_DATE" + "," +
                                    "CURRENT_TIME" + "," +
                                    "'APPS'" + ")";
                        if (!_transaction.SQLGuardaTabla(strSqlSeg))
                            throw new System.ArgumentException();
                    }
                }
            }
            catch (Exception)
            {
                _respuesta.Mensaje = "Error de conexión, Intente de nuevo";
            }

            return _respuesta;
        }
    }
}