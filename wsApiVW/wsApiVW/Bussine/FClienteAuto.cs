using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using wsApiVW.Bussine;
using wsApiVW.Models;
using wsApiVW.Models.Factura;

namespace wsApiVW.Bussine
{
    public class FClienteAuto
    {
        static string Nacionalidad = ConfigurationManager.AppSettings["Nacionalidad"];
        static string CFDI = ConfigurationManager.AppSettings["CFDI"];
        static string Ocupacion = ConfigurationManager.AppSettings["Ocupacion"];
        static string IdentificacionOficial = ConfigurationManager.AppSettings["IdentificacionOficial"];
        static string Documentos = ConfigurationManager.AppSettings["Documentos"];
        static string Asunto = ConfigurationManager.AppSettings["Asunto"];
        static string Mensaje = ConfigurationManager.AppSettings["Mensaje"];

        internal MensajeNotificacion GetMensajesPorIdAsunto(string aIdAsunto)
        {
            List<MensajeNotificacion> lstMensaje = new List<MensajeNotificacion>();
            string strJSON = File.ReadAllText(Mensaje);
            lstMensaje = JsonConvert.DeserializeObject<List<MensajeNotificacion>>(strJSON);
            return lstMensaje.FindAll(c => c.IdAsunto == aIdAsunto).Count == 0 ? new MensajeNotificacion() : lstMensaje.FindAll(c => c.IdAsunto == aIdAsunto).First();
        }
        internal Asunto GetAsuntos(string aIdEstado)
        {
            List<Asunto> lstAsunto = new List<Asunto>();
            string strJSON = File.ReadAllText(Asunto);
            lstAsunto = JsonConvert.DeserializeObject<List<Asunto>>(strJSON);
            return lstAsunto.FindAll(c => c.IdEstado == aIdEstado).Count == 0 ? new Asunto() : lstAsunto.FindAll(c => c.IdEstado == aIdEstado).First();
        }

        internal DatosFiscales GetDatosFiscales(long aIdCompra, string aIdApps)
        {
            DatosFiscales dato = new DatosFiscales();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            try
            {
                string strSql = string.Empty;
                strSql = @"SELECT FIAPIDAPPS,FIAPIDCOMP, FSAPRFCFIS, FSAPNMBFIS, FSAPAPTFIS," +
                            "FSAPAMTFIS, FIAPLDTFIS, FIAPNMTFIS, FSAPCRRFIS,FSAPRFCRSC, FSAPRAZSOC, FSAPRFCRLG, FSAPNMBRLG, FSAPAPTRLG," +
                            "FSAPAMTRLG, FIAPLDTRLG, FSAPCRRRLG, FIAPNMTRLG, FSAPCUCFDI, FSAPDESCRI,FIAPIDNACP, FSAPDESNAP, FSAPCURPPE," +
                            "FSAPCALLEP, FSAPNUMEXP, FSAPNUMINP, FSAPCOLONP, FSAPDELEGP, FSAPCIUDAP, FSAPESTADP, FSAPDIRPER," +
                            "FIAPIDGENP, FSAPDESGEP, FIAPIDOCUP, FSAPDESOCP,FIAPIDTSOP, FSAPDESTSP, FFAPCREEMP, FIAPIDGIRO, FSAPDESCGI," +
                            "FSAPCALLER, FSAPNUMEXR, FSAPNUMINR, FSAPCOLONR, FSAPDELEGR, FSAPCIUDAR, FSAPESTADR, FSAPDIRREP," +
                            "FIAPIDNACR, FSAPDESNAR, FIAPIDGENR, FSAPDESGER, FIAPIDOCUR, FSAPDESOCR,FSAPAPTCON, FSAPAPMCON, FSAPNOMCON, FIAPLADCON, FIAPTELCON, FSAPCORCON " +
                            "FROM PRODAPPS.APDDTFVW WHERE FIAPSTATUS = 1 AND FIAPIDCOMP = " + aIdCompra + " AND FIAPIDAPPS =" + aIdApps;
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
                if (dt.Rows.Count == 0)
                    throw new Exception();
                dato.IdCompra = dt.Rows[0]["FIAPIDCOMP"].ToString().Trim();
                dato.RfcFisica = dt.Rows[0]["FSAPRFCFIS"].ToString().Trim();
                dato.NombreFisica = dt.Rows[0]["FSAPNMBFIS"].ToString().Trim();
                dato.ApellidoPaternoFisica = dt.Rows[0]["FSAPAPTFIS"].ToString().Trim();
                dato.ApellidoMaternoFisica = dt.Rows[0]["FSAPAMTFIS"].ToString().Trim();
                dato.LadaFisica = dt.Rows[0]["FIAPLDTFIS"].ToString().Trim();
                dato.NumeroTelefonoFisica = dt.Rows[0]["FIAPNMTFIS"].ToString().Trim();
                dato.CorreoFisica = dt.Rows[0]["FSAPCRRFIS"].ToString().Trim();
                dato.RfcRazonSocial = dt.Rows[0]["FSAPRFCRSC"].ToString().Trim();
                dato.RazonSocial = dt.Rows[0]["FSAPRAZSOC"].ToString().Trim();
                dato.RfcRepresentanteLegal = dt.Rows[0]["FSAPRFCRLG"].ToString().Trim();
                dato.NombreRepresentanteLegal = dt.Rows[0]["FSAPNMBRLG"].ToString().Trim();
                dato.ApellidoPaternoRepresentanteLegal = dt.Rows[0]["FSAPAPTRLG"].ToString().Trim();
                dato.ApellidoMaternoRepresentantelegal = dt.Rows[0]["FSAPAMTRLG"].ToString().Trim();
                dato.LadaRepresentantelegal = dt.Rows[0]["FIAPLDTRLG"].ToString().Trim();
                dato.NumeroTelefonoRepresentanteLegal = dt.Rows[0]["FIAPNMTRLG"].ToString().Trim();
                dato.CorreoRepresentantelegal = dt.Rows[0]["FSAPCRRRLG"].ToString().Trim();
                dato.ClaveUsoCfdi = dt.Rows[0]["FSAPCUCFDI"].ToString().Trim();
                dato.DescripcionUsoCfdi = dt.Rows[0]["FSAPDESCRI"].ToString().Trim();
                dato.IdNacionalidadC2 = dt.Rows[0]["FIAPIDNACP"].ToString().Trim();
                dato.DescripcionNacionalidadC2 = dt.Rows[0]["FSAPDESNAP"].ToString().Trim();
                dato.CurpC2 = dt.Rows[0]["FSAPCURPPE"].ToString().Trim();
                dato.CallePersonaC2 = dt.Rows[0]["FSAPCALLEP"].ToString().Trim();
                dato.NumeroExteriorPersonaC2 = dt.Rows[0]["FSAPNUMEXP"].ToString().Trim();
                dato.NumeroInteriorPersonaC2 = dt.Rows[0]["FSAPNUMINP"].ToString().Trim();
                dato.ColoniaPersonaC2 = dt.Rows[0]["FSAPCOLONP"].ToString().Trim();
                dato.DelegacionPersonaC2 = dt.Rows[0]["FSAPDELEGP"].ToString().Trim();
                dato.CiudadPersonaC2 = dt.Rows[0]["FSAPCIUDAP"].ToString().Trim();
                dato.EstadoPersonaC2 = dt.Rows[0]["FSAPESTADP"].ToString().Trim();
                dato.DireccionC2 = dt.Rows[0]["FSAPDIRPER"].ToString().Trim();
                dato.IdGeneroPersonaPF = dt.Rows[0]["FIAPIDGENP"].ToString().Trim();
                dato.DescripcionGeneroPF = dt.Rows[0]["FSAPDESGEP"].ToString().Trim();
                dato.IdOcupacionPersonaPF = dt.Rows[0]["FIAPIDOCUP"].ToString().Trim();
                dato.DescripcionOcupacionPF = dt.Rows[0]["FSAPDESOCP"].ToString().Trim();
                dato.IdTipoSociedadPM = dt.Rows[0]["FIAPIDTSOP"].ToString().Trim();
                dato.DescripcionTipoSociedadPM = dt.Rows[0]["FSAPDESTSP"].ToString().Trim();
                dato.FechaConstitucionEmpresaPM = dt.Rows[0]["FFAPCREEMP"].ToString().Trim();
                dato.IdGiroDeLaEmpresaPM = dt.Rows[0]["FIAPIDGIRO"].ToString().Trim();
                dato.DescripcionGiroDeLaEmpresaPM = dt.Rows[0]["FSAPDESCGI"].ToString().Trim();
                dato.CalleRepresentanteLegalVPF = dt.Rows[0]["FSAPCALLER"].ToString().Trim();
                dato.NumeroExteriorRepresentanteLegalVPF = dt.Rows[0]["FSAPNUMEXR"].ToString().Trim();
                dato.NumeroInteriorRepresentanteLegalVPF = dt.Rows[0]["FSAPNUMINR"].ToString().Trim();
                dato.ColoniaRepresentanteLegalVPF = dt.Rows[0]["FSAPCOLONR"].ToString().Trim();
                dato.DelegacionRepresentanteLegalVPF = dt.Rows[0]["FSAPDELEGR"].ToString().Trim();
                dato.CiudadRepresentanteLegalVPF = dt.Rows[0]["FSAPCIUDAR"].ToString().Trim();
                dato.EstadoRepresentanteLegalVPF = dt.Rows[0]["FSAPESTADR"].ToString().Trim();
                dato.DireccionRepresentanteLegalVPF = dt.Rows[0]["FSAPDIRREP"].ToString().Trim();
                dato.IdNacionalidadRepresentanteLegalVPF = dt.Rows[0]["FIAPIDNACR"].ToString().Trim();
                dato.DescripcionNacionalidadRepresentanteLegalVPF = dt.Rows[0]["FSAPDESNAR"].ToString().Trim();
                dato.IdGeneroRepresentanteLegalVPF = dt.Rows[0]["FIAPIDGENR"].ToString().Trim();
                dato.DescripcionGeneroRepresentanteLegalVPF = dt.Rows[0]["FSAPDESGER"].ToString().Trim();
                dato.IdOcupacionRepresentanteLegalVPF = dt.Rows[0]["FIAPIDOCUR"].ToString().Trim();
                dato.DescripcionOcupacionRepresentanteLegalVPF = dt.Rows[0]["FSAPDESOCR"].ToString().Trim();
                dato.ApellidoPaternoContactoVPF = dt.Rows[0]["FSAPAPTCON"].ToString().Trim();
                dato.ApellidoMaternoContactoVPF = dt.Rows[0]["FSAPAPMCON"].ToString().Trim();
                dato.NombresContactoVPF = dt.Rows[0]["FSAPNOMCON"].ToString().Trim();
                dato.LadaContactoVPF = dt.Rows[0]["FIAPLADCON"].ToString().Trim();
                dato.TelefonoContactoVPF = dt.Rows[0]["FIAPTELCON"].ToString().Trim();
                dato.CorreoContactoVPF = dt.Rows[0]["FSAPCORCON"].ToString().Trim();
            }
            catch (Exception)
            {
                dato = new DatosFiscales();
            }
            return dato;
        }
        internal Respuesta GetObtenerFactura(long IdCompra, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            Respuesta respuesta = new Respuesta();
            respuesta.Ok = "NO";
            respuesta.Mensaje = "Factura no encontrada, intentar nuevamente.";
            try
            {
                string strSql = "";
                strSql += "SELECT factu.FICAIDCIAU, factu.FICAIDCLIE, factu.FCCAPREIN, factu.FICAFOLIN FROM PRODAPPS.APEPANVW pedan ";
                strSql += "INNER JOIN PRODCAJA.CAEFACAN facan ON pedan.FIAPIDCIAU = facan.FICAIDCIAU AND pedan.FIAPIDPEDI = facan.FICAIDPEDI ";
                strSql += "INNER JOIN PRODCAJA.CAEFACTU factu ON facan.FICAIDCIAU = factu.FICAIDCIAU AND facan.FICAIDFACT = factu.FICAIDFACT ";
                strSql += "WHERE pedan.FIAPSTATUS = 1  AND pedan.FIAPIDCOMP = " + IdCompra.ToString() + " AND pedan.FIAPIDAPPS = " + aIdApps;
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    string idCia = "";
                    string idCliente = "";
                    string serie = "";
                    string folio = "";
                    foreach (DataRow dr in dt.Rows)
                    {
                        idCia = dr["FICAIDCIAU"].ToString().Trim();
                        idCliente = dr["FICAIDCLIE"].ToString().Trim();
                        serie = dr["FCCAPREIN"].ToString().Trim();
                        folio = dr["FICAFOLIN"].ToString().Trim();
                    }
                    DVAModelsReflection.DB2Database _db = new DVAModelsReflection.DB2Database();
                    List<String> documentoFiscal = new List<string>();
                    DVAFacturacion.FacturaCFD factura = new DVAFacturacion.FacturaCFD(_db, int.Parse(idCia), int.Parse(idCliente));
                    documentoFiscal = (factura.GetDocumentoFiscal(serie, int.Parse(folio)));
                    if (documentoFiscal.Count > 0)
                    {
                        string respuestaDocumentoFiscal = documentoFiscal[0];
                        var webClient = new WebClient();
                        byte[] binarydata = webClient.DownloadData(respuestaDocumentoFiscal);
                        string facturaBase64 = System.Convert.ToBase64String(binarydata, 0, binarydata.Length);
                        byte[] facturaBytes = Convert.FromBase64String(facturaBase64);
                        Stream facturaStream = new MemoryStream(facturaBytes);
                        string directorioFactura = @"C:\inetpub\wwwroot\wsApiVW\Resources\Facturas\";
                        string facturaNombre = serie + "-" + folio + ".pdf";
                        File.WriteAllBytes(directorioFactura + facturaNombre, facturaBytes);
                        string rutaFactura = "http://ws-smartit.divisionautomotriz.com/wsApiVW/Resources/Facturas/" + facturaNombre;
                        respuesta.Ok = "SI";
                        respuesta.Mensaje = rutaFactura;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = "Factura no encontrada, intentar nuevamente.";
            }
            return respuesta;
        }
        internal Task<Documento> GetOpcionesDeFormularios()
        {
            return Task.Run(() =>
            {
                Documento _documentos = new Documento();
                string strJSON = File.ReadAllText(Documentos);
                _documentos = JsonConvert.DeserializeObject<Documento>(strJSON);
                if (_documentos != null)
                {
                    _documentos.Sexo = GetObtenerSexo();
                    _documentos.Sociedad = GetObtenerCatalogoSociedad();
                    _documentos.GiroComercial = GetObtenerGiroComercial();
                }
                else
                {
                    _documentos = new Documento();
                }
                return _documentos;
            });
        }

        internal List<Nacionalidad> ObtenerNacionalidad()
        {
            List<Nacionalidad> lstNacionalidad = new List<Nacionalidad>();
            string strJSON = File.ReadAllText(Nacionalidad);
            lstNacionalidad = JsonConvert.DeserializeObject<List<Nacionalidad>>(strJSON);
            return lstNacionalidad == null ? new List<Nacionalidad>() : lstNacionalidad;
        }

        internal List<UsoCfdi> GetObtenerUsosCfdi()
        {
            List<UsoCfdi> lstUsos = new List<UsoCfdi>();
            string strJSON = File.ReadAllText(CFDI);
            lstUsos = JsonConvert.DeserializeObject<List<UsoCfdi>>(strJSON);
            return lstUsos == null ? new List<UsoCfdi>() : lstUsos;
        }

        internal List<CatalogoSexo> GetObtenerSexo()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            List<CatalogoSexo> lstSexo = new List<CatalogoSexo>();
            CatalogoSexo sexo = new CatalogoSexo();
            string strSexo = string.Empty;
            strSexo += "select FDSXIDSEXO, FSSXDESCRI from PRODPERS.CTCSEXO";

            DataTable comp = dbCnx.GetDataSet(strSexo).Tables[0];
            if (comp.Rows.Count > 0)
            {
                foreach (DataRow dr in comp.Rows)
                {
                    sexo = new CatalogoSexo();
                    sexo.IdSexo = dr["FDSXIDSEXO"].ToString().Trim();
                    sexo.DescripcionSexo = dr["FSSXDESCRI"].ToString().Trim();
                    lstSexo.Add(sexo);
                }
            }
            else
            {
                lstSexo = new List<CatalogoSexo>();
            }

            return lstSexo;
        }

        internal List<Ocupacion> GetObtenerOcupacion()
        {
            List<Ocupacion> lstOcupacion = new List<Ocupacion>();
            string strJSON = File.ReadAllText(Ocupacion);
            lstOcupacion = JsonConvert.DeserializeObject<List<Ocupacion>>(strJSON);
            return lstOcupacion == null ? new List<Ocupacion>() : lstOcupacion;
        }

        internal List<Sociedad> GetObtenerCatalogoSociedad()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            List<Sociedad> lstSociedad = new List<Sociedad>();
            Sociedad sociedad = new Sociedad();
            string strSoc = string.Empty;
            strSoc += "select FIGEIDSOCI, FSGEDESSOC from PRODGRAL.GECSOCIE WHERE FIGESTATUS = 1";
            DataTable soc = dbCnx.GetDataSet(strSoc).Tables[0];
            if (soc.Rows.Count > 0)
            {
                foreach (DataRow dr in soc.Rows)
                {
                    sociedad = new Sociedad();
                    sociedad.IdSociedad = dr["FIGEIDSOCI"].ToString().Trim();
                    sociedad.DescripcionSociedad = dr["FSGEDESSOC"].ToString().Trim();
                    lstSociedad.Add(sociedad);
                }
            }
            else
            {
                lstSociedad = new List<Sociedad>();
            }
            return lstSociedad;
        }
        internal List<GiroComercial> GetObtenerGiroComercial()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            List<GiroComercial> lstGiro = new List<GiroComercial>();
            GiroComercial giro = new GiroComercial();
            string strGiro = "";
            strGiro += "select FICTIDGRCM, TRIM(FSCTDSGRCM) FSCTDSGRCM from PRODPERS.CTCGRCME WHERE  FICTSTATUS = 1 ";
            DataTable gir = dbCnx.GetDataSet(strGiro).Tables[0];
            if (gir.Rows.Count > 0)
            {
                foreach (DataRow dr in gir.Rows)
                {
                    giro = new GiroComercial();
                    giro.IdGiroComercial = dr["FICTIDGRCM"].ToString().Trim();
                    giro.DescripcionGiroComercial = dr["FSCTDSGRCM"].ToString().Trim();
                    lstGiro.Add(giro);
                }
                lstGiro = lstGiro.OrderBy(s => s.DescripcionGiroComercial).ToList();
            }
            else { lstGiro = new List<GiroComercial>(); }
            return lstGiro;
        }

        internal List<DocumentoIdentificacion> GetObtenerDocumentosIdentificacion()
        {
            List<DocumentoIdentificacion> lstDocumentos = new List<DocumentoIdentificacion>();
            string strJSON = File.ReadAllText(IdentificacionOficial);
            lstDocumentos = JsonConvert.DeserializeObject<List<DocumentoIdentificacion>>(strJSON);
            return lstDocumentos == null ? new List<DocumentoIdentificacion>() : lstDocumentos;
        }

        public Task<Respuesta> RegistraDatosFiscalesAudi(DatosFiscales datosFiscales, int IdTipoPersona, string aIdApps)
        {
            return Task.Run(() =>
            {
                AutosBussine _au = new AutosBussine();
                DVADB.DB2 dbCnx = new DVADB.DB2();
                Respuesta respuesta = new Respuesta();
                DatosFiscales dato = new DatosFiscales();
                string jsonPedido = string.Empty;
                string strSql = string.Empty;
                //Transaccion
                SQLTransaction save = new SQLTransaction();
                //PrecioSinDescuentos
                string PrecioSinDescuento = string.Empty;
                //Tipo del vehiculo dependeiendo del IdVehiculo
                string TipoVehiculo = string.Empty;
                //Cuenta
                string IdCuenta = string.Empty;
                string SubTotal=  string.Empty;


                


                long lngIdPersona = 0;
                long lngIdPedido = 0;
                int idAgencia = 0;
                string strRutaReferenciaBancaria = "";
                string strNumeroSerie = "";
                decimal dcmTotalPedido = 0;
                decimal dcmTotal = 0;
                try
                {
                    //Definicion de compras 
                    strSql = $@"SELECT A.FIAPIDVEHI,
                                   A.FDAPPRECIO, 
                                   B.FIAPIDCUEN FROM PRODAPPS.APEPANVW A
                                INNER JOIN PRODapps.APECMPVW B
                                ON A.FIAPIDAPPS = B.FIAPIDAPPS
                                AND A.FIAPIDCOMP =  B.FIAPIDCOMP  
                           WHERE A.FIAPIDCOMP = {datosFiscales.IdCompra}
                           AND A.FIAPIDAPPS ={aIdApps}";

                    PrecioSinDescuento = dbCnx.GetDataSet(strSql).Tables[0].Rows[0]["FDAPPRECIO"].ToString();
                    TipoVehiculo = string.IsNullOrEmpty(dbCnx.GetDataSet(strSql).Tables[0].Rows[0]["FIAPIDVEHI"].ToString()) ? _au.TipoVehiculo(2) : _au.TipoVehiculo(1); //Id 2 significa vehiculo configurado, por ende 1 representa Existencia
                    IdCuenta = dbCnx.GetDataSet(strSql).Tables[0].Rows[0]["FIAPIDCUEN"].ToString();

                    //Actualizacion de Compras
                    dcmTotal = Math.Round(Convert.ToDecimal(PrecioSinDescuento) - Convert.ToDecimal(_au.TotalDescuentos(PrecioSinDescuento, Convert.ToDecimal(_au.Valorconfigurador(TipoVehiculo, aIdApps, PrecioSinDescuento)).ToString(), aIdApps, datosFiscales.IdCompra, IdCuenta)),2);
                    SubTotal = Math.Round(Convert.ToDecimal(dcmTotal) / Convert.ToDecimal(1.16), 2).ToString();
                    strSql = $@"UPDATE PRODAPPS.APEPANVW
                                   SET   FDAPTOTAL = {dcmTotal} /*TOTAL*/
                                        ,FDAPDESCUE = {_au.TotalDescuentos(PrecioSinDescuento, Convert.ToDecimal(_au.Valorconfigurador(TipoVehiculo, aIdApps, PrecioSinDescuento)).ToString(), aIdApps, datosFiscales.IdCompra, IdCuenta)} /*DESCUENTO*/
                                        ,FDAPSUBTOT = {SubTotal} /*SUBTOTAL*/
                                        ,FDAPIVA = {(Math.Round(Convert.ToDecimal(SubTotal) * Convert.ToDecimal(0.16), 2)).ToString()}/*IVA*/
                                   WHERE FIAPIDCOMP = {datosFiscales.IdCompra}
                                         AND FIAPIDAPPS = {aIdApps}";
                    dbCnx.SetQuery(strSql);

                    strSql = $@"UPDATE PRODapps.APECMPVW
                                   SET   FDAPTOTAL = {dcmTotal} /*TOTAL*/
                                        ,FDAPDESCUE = {_au.TotalDescuentos(PrecioSinDescuento, Convert.ToDecimal(_au.Valorconfigurador(TipoVehiculo, aIdApps, PrecioSinDescuento)).ToString(), aIdApps, datosFiscales.IdCompra, IdCuenta)} /*DESCUENTO*/
                                        ,FDAPSUBTOT = {SubTotal} /*SUBTOTAL*/
                                        ,FDAPIVA = {(Math.Round(Convert.ToDecimal(SubTotal) * Convert.ToDecimal(0.16), 2)).ToString()}/*IVA*/
                                    WHERE FIAPIDCOMP = {datosFiscales.IdCompra}
                                         AND FIAPIDAPPS = {aIdApps}";
                    dbCnx.SetQuery(strSql);

                    dato = datosFiscales;

                    #region RFC
                    if (dato.RfcFisica != null && dato.RfcFisica.Trim() != "")
                    {
                        dato.RfcFisica = BPedido.FormatoRfc(dato.RfcFisica.Trim());
                    }
                    if (dato.RfcRazonSocial != null && dato.RfcRazonSocial.Trim() != "")
                    {
                        dato.RfcRazonSocial = BPedido.FormatoRfc(dato.RfcRazonSocial.Trim());
                    }

                    if (dato.RfcRepresentanteLegal != null && dato.RfcRepresentanteLegal.Trim() != "")
                    {
                        dato.RfcRepresentanteLegal = BPedido.FormatoRfc(dato.RfcRepresentanteLegal.Trim());
                    }
                    #endregion

                    string idInventario = string.Empty;
                    strSql = "select FIAPIDCIAU, trim(FSAPNUMSER) NumeroSerie, FDAPTOTAL Total, FIAPIDINVE from prodapps.APEPANVW where FIAPIDCOMP = " + datosFiscales.IdCompra.ToString() + " AND FIAPIDAPPS = " + aIdApps;
                    DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        strNumeroSerie = dt.Rows[0]["NumeroSerie"].ToString().Trim();
                        decimal.TryParse(dt.Rows[0]["Total"].ToString(), out dcmTotalPedido);
                        idInventario = dt.Rows[0]["FIAPIDINVE"].ToString().Trim();
                        idAgencia = Convert.ToInt32(dt.Rows[0]["FIAPIDCIAU"].ToString().Trim());
                    }
                    else
                    {
                        throw new Exception();
                    }

                    #region APARTADO

                    try
                    {
                        dbCnx.AbrirConexion();
                        dbCnx.BeginTransaccion();

                        strSql = "";
                        strSql += "UPDATE PRODAUT.ANCAUTOM  SET FNAUTOEST = 10, \t";
                        strSql += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' \t";
                        strSql += "WHERE FNAUTOAGE = " + idAgencia + " \t";
                        strSql += "AND FIANIDINVE=" + idInventario + "\t";
                        strSql += "AND FIANSTATU = 1 \t";
                        strSql += "AND FNAUTOEST = 50";

                        dbCnx.SetQuery(strSql);

                        #region Registra CLIENTE

                        DVARegistraPersona.RegistraPersona persona = new DVARegistraPersona.RegistraPersona();

                        if (!string.IsNullOrEmpty(datosFiscales.RazonSocial.Trim()))
                        {
                            persona.RFC = datosFiscales.RfcRazonSocial.Trim();
                            persona.RazonSocial = datosFiscales.RazonSocial.Trim();
                            persona.NumeroCelular = datosFiscales.NumeroTelefonoFisica.Trim();

                            if (IdTipoPersona == 1)
                            {
                                persona.Email = datosFiscales.CorreoFisica.Trim();
                            }
                            else if (IdTipoPersona == 2)
                            {
                                persona.Email = datosFiscales.CorreoRepresentantelegal.ToString().Trim();
                            }


                        }
                        else
                        {
                            persona.Nombre = datosFiscales.NombreFisica.Trim();
                            persona.ApellidoPaterno = datosFiscales.ApellidoPaternoFisica.Trim();
                            persona.ApellidoMaterno = datosFiscales.ApellidoMaternoFisica.Trim();
                            persona.NumeroCelular = datosFiscales.NumeroTelefonoFisica.Trim();
                            persona.Email = datosFiscales.CorreoFisica.Trim();

                            if (datosFiscales.RfcFisica.Trim() != "" && datosFiscales.RfcFisica != null)
                            {

                                persona.RFC = datosFiscales.RfcFisica.Trim().ToUpper();

                            }
                        }

                        string idPersona = ObtieneORegistraPersonaApps(ref dbCnx, idAgencia, "APP", "APP", persona);

                        bool isRegister = long.TryParse(idPersona, out lngIdPersona);

                        if (isRegister == false)
                            throw new Exception();

                        #endregion

                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();

                    }
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();
                        dbCnx.CerrarConexion();
                        throw new Exception();
                    }

                    #endregion

                    try
                    {
                        #region  REGISTRO PEDIDO

                        SolicitudPedido solicitudPedido = new SolicitudPedido();
                        ApuntadorDeServicio _prod = new ApuntadorDeServicio();
                        strSql = string.Empty;
                        strSql = @"SELECT FSAPDESCRI FROM prodapps.APECMPVW" +
                                    " WHERE FIAPIDCOMP = " + datosFiscales.IdCompra + " AND FIAPIDAPPS =" + aIdApps;
                        solicitudPedido.Marca = dbCnx.GetDataSet(strSql).Tables[0].Rows[0]["FSAPDESCRI"].ToString().Trim() == "SINMARCA" ? string.Empty : dbCnx.GetDataSet(strSql).Tables[0].Rows[0]["FSAPDESCRI"].ToString().Trim();
                        solicitudPedido.RutaFisica = @"C:\inetpub\wwwroot\wsApiVW\Resources\Adjuntos";
                        solicitudPedido.IdCliente = lngIdPersona;
                        solicitudPedido.IdAgencia = idAgencia.ToString();
                        solicitudPedido.IdPrograma = "1";
                        solicitudPedido.IdUsuario = "7244";
                        solicitudPedido.Serie = strNumeroSerie;
                        solicitudPedido.IdAgente = 999996;
                        solicitudPedido.IdContacto = 0;
                        solicitudPedido.IdTipoDeVenta = 52;
                        solicitudPedido.Total = dcmTotalPedido;
                        jsonPedido = JsonConvert.SerializeObject(solicitudPedido);

                        string valor = idAgencia + "|7244|1|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        DVAAutosystServerClasses.Seguridad.Seguridad seg = new DVAAutosystServerClasses.Seguridad.Seguridad();
                        string token = seg.EncriptarCadena(valor);

                        string url_ped = _prod.RespuestaFacturacion();
                        url_ped = url_ped.Replace("valor", token);

                        var httpWebRequest_ped = (HttpWebRequest)WebRequest.Create(url_ped);

                        httpWebRequest_ped.Timeout = 40000;

                        httpWebRequest_ped.ContentType = "application/json";
                        httpWebRequest_ped.Method = "POST";

                        using (var streamWriter = new StreamWriter(httpWebRequest_ped.GetRequestStream()))
                        {
                            streamWriter.Write(jsonPedido);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        var httpResponse_ped = (HttpWebResponse)httpWebRequest_ped.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse_ped.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            RespuestaPedido respuestaPedido = new RespuestaPedido();
                            string error = string.Empty;
                            if (result.Contains("IdPedido") && result.Contains("Ruta"))
                                respuestaPedido = JsonConvert.DeserializeObject<RespuestaPedido>(result);
                            else
                                error = JsonConvert.DeserializeObject<string>(result);

                            if (!string.IsNullOrEmpty(error))
                            {
                                throw new Exception();
                            }
                            else
                            {
                                long.TryParse(respuestaPedido.IdPedido, out lngIdPedido);
                                strRutaReferenciaBancaria = respuestaPedido.Ruta.Replace("C:\\inetpub\\wwwroot\\wsApiVW\\Resources\\Adjuntos\\", "http://ws-smartit.divisionautomotriz.com/wsApiVW/Resources/Adjuntos/");
                                strSql = string.Empty;
                                strSql = @"UPDATE PRODAPPS.APECMPVW SET FSAPRUTRFB =  '" + strRutaReferenciaBancaria + "' WHERE FIAPIDCOMP =" + datosFiscales.IdCompra + " AND FIAPIDAPPS= " + aIdApps;
                                dbCnx.SetQuery(strSql);
                            }
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {

                        #region APARTADO

                        try
                        {
                            dbCnx.AbrirConexion();
                            dbCnx.BeginTransaccion();

                            strSql = "";
                            strSql += "UPDATE PRODAUT.ANCAUTOM  SET FNAUTOEST = 50, \t";
                            strSql += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' \t";
                            strSql += "WHERE FNAUTOAGE = " + idAgencia + " \t";
                            strSql += "AND FIANIDINVE=" + idInventario + "\t";
                            strSql += "AND FIANSTATU = 1 \t";
                            strSql += "AND FNAUTOEST = 10";

                            dbCnx.SetQuery(strSql);

                            dbCnx.CommitTransaccion();
                            dbCnx.CerrarConexion();

                        }
                        catch (Exception)
                        {
                            dbCnx.RollbackTransaccion();
                            dbCnx.CerrarConexion();
                        }

                        #endregion

                        throw new Exception();
                    }

                    #region Tablas Apps

                    try
                    {

                        dbCnx.AbrirConexion();
                        dbCnx.BeginTransaccion();

                        string existe = "";
                        existe += "SELECT COUNT(*) COUNT FROM PRODAPPS.APDDTFVW ";
                        existe += "WHERE FIAPIDCOMP = " + dato.IdCompra + " ";
                        existe += "AND FIAPSTATUS = 1" + " AND FIAPIDAPPS= " + aIdApps;

                        DataTable exist = dbCnx.GetDataSet(existe).Tables[0];

                        foreach (DataRow dr in exist.Rows)
                        {
                            int cantidad = 0;

                            cantidad = Convert.ToInt32(dr["COUNT"].ToString().Trim());

                            #region DatosFiscales
                            if (cantidad > 0)
                            {
                                #region Actualizar
                                if (IdTipoPersona == 1)
                                { /*PERSONA FISICA*/

                                    strSql = "";
                                    strSql += "UPDATE PRODAPPS.APDDTFVW ";
                                    strSql += "SET FSAPRFCFIS = " + "'" + dato.RfcFisica.Trim().ToUpper() + "'" + ",";
                                    strSql += "FSAPNMBFIS = " + "'" + dato.NombreFisica.Trim().ToUpper() + "'" + ",FSAPAPTFIS = " + "'" + dato.ApellidoPaternoFisica.Trim().ToUpper() + "'";
                                    strSql += ",FSAPAMTFIS= " + "'" + dato.ApellidoMaternoFisica.Trim().ToUpper() + "'" + ",FIAPLDTFIS = " + 52 + ",FIAPNMTFIS=" + (string.IsNullOrEmpty(dato.NumeroTelefonoFisica) ? "Default" : dato.NumeroTelefonoFisica.Trim().ToUpper()) + ",FSAPCRRFIS=" + "'" + dato.CorreoFisica.Trim() + "'";
                                    strSql += ",FSAPRFCRSC =" + "'" + dato.RfcRazonSocial.Trim().ToUpper() + "'" + ",FSAPRAZSOC=" + "'" + dato.RazonSocial.Trim().ToUpper() +
                                        "',FSAPRFCRLG=" + "'" + dato.RfcRepresentanteLegal.Trim().ToUpper() +
                                        "'" + ",FSAPNMBRLG=" + "'" + dato.NombreRepresentanteLegal.Trim().ToUpper() + "'" + ",FSAPAPTRLG = " + "'" + dato.ApellidoPaternoRepresentanteLegal.Trim().ToUpper() + "'";
                                    strSql += ",FSAPAMTRLG = " + "'" + dato.ApellidoMaternoRepresentantelegal.Trim().ToUpper() + "'" + ",FIAPLDTRLG=" + 52 + ",FSAPCRRRLG=" + "'" + dato.CorreoRepresentantelegal.Trim() + "'" +
                                        ",FIAPNMTRLG=" + (string.IsNullOrEmpty(dato.NumeroTelefonoRepresentanteLegal.Trim()) ? "Default" : dato.NumeroTelefonoRepresentanteLegal.Trim().ToUpper());
                                    strSql += " ,FSAPCUCFDI = " + (string.IsNullOrEmpty(dato.ClaveUsoCfdi.Trim()) ? "Default" : "'" + dato.ClaveUsoCfdi.Trim().ToUpper() + "'") + ", FSAPDESCRI = " + (string.IsNullOrEmpty(dato.DescripcionUsoCfdi.Trim()) ? "Default" : "'" + dato.DescripcionUsoCfdi.Trim().ToUpper() + "'");

                                    strSql += ",FIAPIDNACP = " + dato.IdNacionalidadC2.ToString().Trim() + ", FSAPDESNAP = " + "'" + dato.DescripcionNacionalidadC2.ToString().Trim() + "'" + ", FSAPCURPPE = " + "'" + dato.CurpC2.ToString().Trim() + "'";
                                    strSql += ",FSAPDIRPER = " + "'" + dato.DireccionC2.ToString().Trim().ToUpper() + "'" + ",FIAPIDGENP =" + dato.IdGeneroPersonaPF.ToString().Trim() + ",FSAPDESGEP =" + "'" + dato.DescripcionGeneroPF.ToString().Trim() + "'";
                                    strSql += ",FIAPIDOCUP=" + dato.IdOcupacionPersonaPF.ToString().Trim() + ",FSAPDESOCP=" + "'" + dato.DescripcionOcupacionPF.ToString().Trim() + "'";

                                    strSql += " ,PROGCREAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, USERUPDAT = 'APP', PROGUPDAT = 'APP' ";
                                    strSql += "WHERE FIAPIDCOMP = " + dato.IdCompra + " AND FIAPIDAPPS=" + aIdApps;
                                    dbCnx.SetQuery(strSql);

                                }
                                else if (IdTipoPersona == 2)
                                {

                                    strSql = "";
                                    strSql += "UPDATE PRODAPPS.APDDTFVW ";
                                    strSql += "SET FSAPRFCFIS = " + "'" + dato.RfcFisica.Trim().ToUpper() + "'" + ",";
                                    strSql += "FSAPNMBFIS = " + "'" + dato.NombreFisica.Trim().ToUpper() + "'" + ",FSAPAPTFIS = " + "'" + dato.ApellidoPaternoFisica.Trim().ToUpper() + "'";
                                    strSql += ",FSAPAMTFIS= " + "'" + dato.ApellidoMaternoFisica.Trim().ToUpper() + "'" + ",FIAPLDTFIS = " + 52 + ",FIAPNMTFIS=" + (string.IsNullOrEmpty(dato.NumeroTelefonoFisica) ? "Default" : dato.NumeroTelefonoFisica.Trim().ToUpper()) + ",FSAPCRRFIS=" + "'" + dato.CorreoFisica.Trim() + "'";
                                    strSql += ",FSAPRFCRSC =" + "'" + dato.RfcRazonSocial.Trim().ToUpper() + "'" + ",FSAPRAZSOC=" + "'" + dato.RazonSocial.Trim().ToUpper() +
                                        "',FSAPRFCRLG=" + "'" + dato.RfcRepresentanteLegal.Trim().ToUpper() +
                                        "'" + ",FSAPNMBRLG=" + "'" + dato.NombreRepresentanteLegal.Trim().ToUpper() + "'" + ",FSAPAPTRLG = " + "'" + dato.ApellidoPaternoRepresentanteLegal.Trim().ToUpper() + "'";
                                    strSql += ",FSAPAMTRLG = " + "'" + dato.ApellidoMaternoRepresentantelegal.Trim().ToUpper() + "'" + ",FIAPLDTRLG=" + 52 + ",FSAPCRRRLG=" + "'" + dato.CorreoRepresentantelegal.Trim() + "'" +
                                        ",FIAPNMTRLG=" + (string.IsNullOrEmpty(dato.NumeroTelefonoRepresentanteLegal.Trim()) ? "Default" : dato.NumeroTelefonoRepresentanteLegal.Trim().ToUpper());
                                    strSql += " ,FSAPCUCFDI = " + (string.IsNullOrEmpty(dato.ClaveUsoCfdi.Trim()) ? "Default" : "'" + dato.ClaveUsoCfdi.Trim().ToUpper() + "'") + ", FSAPDESCRI = " + (string.IsNullOrEmpty(dato.DescripcionUsoCfdi.Trim()) ? "Default" : "'" + dato.DescripcionUsoCfdi.Trim().ToUpper() + "'");

                                    strSql += ",FIAPIDNACP = " + dato.IdNacionalidadC2.ToString().Trim() + ", FSAPDESNAP = " + "'" + dato.DescripcionNacionalidadC2.ToString().Trim() + "'" + ", FSAPCURPPE = " + "'" + dato.CurpC2.ToString().Trim() + "'";
                                    strSql += ",FSAPDIRPER =" + "'" + dato.DireccionC2.ToString().Trim().ToUpper() + "'" + ",FIAPIDTSOP = " + dato.IdTipoSociedadPM.ToString().Trim().ToUpper() + ",FSAPDESTSP=" + "'" + dato.DescripcionTipoSociedadPM.ToString().Trim().ToUpper() + "'";
                                    strSql += ",FFAPCREEMP= " + "'" + dato.FechaConstitucionEmpresaPM.ToString().Trim().ToUpper() + "'" + ",FIAPIDGIRO=" + dato.IdGiroDeLaEmpresaPM.ToString().Trim().ToUpper() + ",FSAPDESCGI =" + "'" + dato.DescripcionGiroDeLaEmpresaPM.ToString().Trim().ToUpper() + "'";
                                    strSql += ",FSAPDIRREP =" + "'" + dato.DireccionRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + ",FIAPIDNACR=" + dato.IdNacionalidadRepresentanteLegalVPF.ToString().Trim().ToUpper() + ",FSAPDESNAR=" + "'" + dato.DescripcionNacionalidadRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'";
                                    strSql += ",FIAPIDGENR=" + dato.IdGeneroRepresentanteLegalVPF.ToString().Trim().ToUpper() + ",FSAPDESGER=" + "'" + dato.DescripcionGeneroRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + ",FIAPIDOCUR=" + dato.IdOcupacionRepresentanteLegalVPF.ToString().Trim().ToUpper();
                                    strSql += ",FSAPDESOCR=" + "'" + dato.DescripcionOcupacionRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + ",FSAPAPTCON=" + "'" + dato.ApellidoPaternoContactoVPF.ToString().Trim().ToUpper() + "'" + ",FSAPAPMCON=" + "'" + dato.ApellidoMaternoContactoVPF.ToString().Trim().ToUpper() + "'";
                                    strSql += ",FSAPNOMCON=" + "'" + dato.NombresContactoVPF.ToString().Trim().ToUpper() + "'" + ",FIAPLADCON=" + 52 + ", FIAPTELCON=" + dato.TelefonoContactoVPF.ToString().Trim().ToUpper();
                                    strSql += ",FSAPCORCON=" + "'" + dato.CorreoContactoVPF.ToString().Trim().ToUpper() + "'";

                                    strSql += " ,PROGCREAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, USERUPDAT = 'APP', PROGUPDAT = 'APP' ";
                                    strSql += "WHERE FIAPIDCOMP = " + dato.IdCompra + " AND FIAPIDAPPS=" + aIdApps;
                                    dbCnx.SetQuery(strSql);

                                }
                                #endregion
                            }
                            else
                            {
                                #region Agregar
                                if (IdTipoPersona == 1)
                                { /*PERSONA FISICA*/
                                    strSql = "";
                                    strSql += "INSERT INTO PRODAPPS.APDDTFVW (";
                                    strSql += "FIAPIDCOMP, FSAPRFCFIS, FSAPNMBFIS, FSAPAPTFIS, ";
                                    strSql += "FSAPAMTFIS, FIAPLDTFIS, FIAPNMTFIS, FSAPCRRFIS, ";
                                    strSql += "FSAPRFCRSC, FSAPRAZSOC, FSAPRFCRLG, FSAPNMBRLG, FSAPAPTRLG, ";
                                    strSql += "FSAPAMTRLG, FIAPLDTRLG, FIAPNMTRLG, FSAPCRRRLG, FSAPCUCFDI, FSAPDESCRI, ";

                                    strSql += "FIAPIDNACP, FSAPDESNAP, FSAPCURPPE, ";
                                    strSql += "FSAPDIRPER, FIAPIDGENP, FSAPDESGEP, ";
                                    strSql += "FIAPIDOCUP, FSAPDESOCP, ";

                                    strSql += "FIAPSTATUS, USERCREAT, PROGCREAT,FIAPIDAPPS)";
                                    strSql += "VALUES (";
                                    strSql += dato.IdCompra + "," + "'" + dato.RfcFisica.Trim().ToUpper() + "'" + "," + "'" + dato.NombreFisica.Trim().ToUpper() + "'" + "," + "'" + dato.ApellidoPaternoFisica.Trim().ToUpper() + "'" + ",";
                                    strSql += "'" + dato.ApellidoMaternoFisica.Trim().ToUpper() + "'" + "," + 52 + "," + (string.IsNullOrEmpty(dato.NumeroTelefonoFisica) ? "Default" : dato.NumeroTelefonoFisica.Trim().ToUpper()) + "," + "'" + dato.CorreoFisica.Trim() + "'" + ",";
                                    strSql += "'" + dato.RfcRazonSocial.Trim().ToUpper() + "'" + "," + "'" + dato.RazonSocial.Trim().ToUpper() +
                                        "','" + dato.RfcRepresentanteLegal.Trim().ToUpper() +
                                        "','" + dato.NombreRepresentanteLegal.Trim().ToUpper() + "','" + dato.ApellidoPaternoRepresentanteLegal.Trim().ToUpper() + "',";
                                    strSql += "'" + dato.ApellidoMaternoRepresentantelegal.Trim().ToUpper() + "',52" +
                                        "," + (string.IsNullOrEmpty(dato.NumeroTelefonoRepresentanteLegal.Trim()) ? "Default" : dato.NumeroTelefonoRepresentanteLegal.Trim().ToUpper()) +
                                        ",'" + dato.CorreoRepresentantelegal.Trim() + "'" + "," + (string.IsNullOrEmpty(dato.ClaveUsoCfdi.Trim()) ? "Default" : "'" + dato.ClaveUsoCfdi.Trim().ToUpper() + "'") + "," + (string.IsNullOrEmpty(dato.DescripcionUsoCfdi.Trim()) ? "Default" : "'" + dato.DescripcionUsoCfdi.Trim().ToUpper() + "'");

                                    strSql += "," + dato.IdNacionalidadC2.ToString().Trim() + "," + "'" + dato.DescripcionNacionalidadC2.ToString().Trim() + "'" + "," + "'" + dato.CurpC2.ToString().Trim() + "'";
                                    strSql += "," + "'" + dato.DireccionC2.ToString().Trim().ToUpper() + "'" + "," + dato.IdGeneroPersonaPF.ToString().Trim() + "," + "'" + dato.DescripcionGeneroPF.ToString().Trim() + "'";
                                    strSql += "," + dato.IdOcupacionPersonaPF.ToString().Trim() + "," + "'" + dato.DescripcionOcupacionPF.ToString().Trim() + "'";

                                    strSql += ",1,'APP','APP', " + aIdApps + ")";

                                    dbCnx.SetQuery(strSql);
                                }
                                else if (IdTipoPersona == 2)
                                { /*PERSONA MORAL*/

                                    strSql = "";
                                    strSql += "INSERT INTO PRODAPPS.APDDTFVW (";
                                    strSql += "FIAPIDCOMP, FSAPRFCFIS, FSAPNMBFIS, FSAPAPTFIS, ";
                                    strSql += "FSAPAMTFIS, FIAPLDTFIS, FIAPNMTFIS, FSAPCRRFIS, ";
                                    strSql += "FSAPRFCRSC, FSAPRAZSOC, FSAPRFCRLG, FSAPNMBRLG, FSAPAPTRLG, ";
                                    strSql += "FSAPAMTRLG, FIAPLDTRLG, FIAPNMTRLG, FSAPCRRRLG, FSAPCUCFDI, FSAPDESCRI, ";
                                    // aqui inician los cambios del proceso 2
                                    strSql += "FIAPIDNACP,FSAPDESNAP,FSAPCURPPE,";
                                    strSql += "FSAPDIRPER,FIAPIDTSOP,FSAPDESTSP,";
                                    strSql += "FFAPCREEMP,FIAPIDGIRO,FSAPDESCGI,";
                                    strSql += "FSAPDIRREP,FIAPIDNACR,FSAPDESNAR,";
                                    strSql += "FIAPIDGENR,FSAPDESGER,FIAPIDOCUR,";
                                    strSql += "FSAPDESOCR,FSAPAPTCON,FSAPAPMCON,";
                                    strSql += "FSAPNOMCON,FIAPLADCON,FIAPTELCON,";
                                    strSql += "FSAPCORCON,";

                                    strSql += "FIAPSTATUS, USERCREAT, PROGCREAT,FIAPIDAPPS)";
                                    strSql += "VALUES (";
                                    strSql += dato.IdCompra + "," + "'" + dato.RfcFisica.Trim().ToUpper() + "'" + "," + "'" + dato.NombreFisica.Trim().ToUpper() + "'" + "," + "'" + dato.ApellidoPaternoFisica.Trim().ToUpper() + "'" + ",";
                                    strSql += "'" + dato.ApellidoMaternoFisica.Trim().ToUpper() + "'" + "," + 52 + "," + (string.IsNullOrEmpty(dato.NumeroTelefonoFisica) ? "Default" : dato.NumeroTelefonoFisica.Trim().ToUpper()) + "," + "'" + dato.CorreoFisica.Trim() + "'" + ",";
                                    strSql += "'" + dato.RfcRazonSocial.Trim().ToUpper() + "'" + "," + "'" + dato.RazonSocial.Trim().ToUpper() +
                                        "','" + dato.RfcRepresentanteLegal.Trim().ToUpper() +
                                        "','" + dato.NombreRepresentanteLegal.Trim().ToUpper() + "','" + dato.ApellidoPaternoRepresentanteLegal.Trim().ToUpper() + "',";
                                    strSql += "'" + dato.ApellidoMaternoRepresentantelegal.Trim().ToUpper() + "',52" +
                                        "," + (string.IsNullOrEmpty(dato.NumeroTelefonoRepresentanteLegal.Trim()) ? "Default" : dato.NumeroTelefonoRepresentanteLegal.Trim().ToUpper()) +
                                        ",'" + dato.CorreoRepresentantelegal.Trim() + "'" + "," + (string.IsNullOrEmpty(dato.ClaveUsoCfdi.Trim()) ? "Default" : "'" + dato.ClaveUsoCfdi.Trim().ToUpper() + "'") + "," + (string.IsNullOrEmpty(dato.DescripcionUsoCfdi.Trim()) ? "Default" : "'" + dato.DescripcionUsoCfdi.Trim().ToUpper() + "'");

                                    strSql += "," + dato.IdNacionalidadC2.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionNacionalidadC2.ToString().Trim().ToUpper() + "'" + "," + "'" + dato.CurpC2.ToString().Trim().ToUpper() + "'";
                                    strSql += "," + "'" + dato.DireccionC2.ToString().Trim().ToUpper() + "'" + "," + dato.IdTipoSociedadPM.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionTipoSociedadPM.ToString().Trim().ToUpper() + "'";
                                    strSql += "," + "'" + dato.FechaConstitucionEmpresaPM.ToString().Trim().ToUpper() + "'" + "," + dato.IdGiroDeLaEmpresaPM.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionGiroDeLaEmpresaPM.ToString().Trim().ToUpper() + "'";
                                    strSql += "," + "'" + dato.DireccionRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + "," + dato.IdNacionalidadRepresentanteLegalVPF.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionNacionalidadRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'";
                                    strSql += "," + dato.IdGeneroRepresentanteLegalVPF.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionGeneroRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + "," + dato.IdOcupacionRepresentanteLegalVPF.ToString().Trim().ToUpper();
                                    strSql += "," + "'" + dato.DescripcionOcupacionRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + "," + "'" + dato.ApellidoPaternoContactoVPF.ToString().Trim().ToUpper() + "'" + "," + "'" + dato.ApellidoMaternoContactoVPF.ToString().Trim().ToUpper() + "'";
                                    strSql += "," + "'" + dato.NombresContactoVPF.ToString().Trim().ToUpper() + "'" + "," + 52 + "," + dato.TelefonoContactoVPF.ToString().Trim().ToUpper();
                                    strSql += "," + "'" + dato.CorreoContactoVPF.ToString().Trim().ToUpper() + "'";

                                    strSql += ",1,'APP','APP', " + aIdApps + ")";

                                    dbCnx.SetQuery(strSql);

                                }
                                #endregion
                            }
                            #endregion
                        }


                        #region REGISTRO PEDIDOP APP AUTO              
                        strSql = "";
                        strSql = @"UPDATE PRODAPPS.APEPANVW SET FIAPIDPEDI = " + lngIdPedido.ToString() +
                            ", FFAPFECPED = CURRENT DATE, FHAPHORPED = CURRENT TIME, FIAPIDPERS = " + lngIdPersona.ToString() +
                            " WHERE FIAPIDCOMP = " + datosFiscales.IdCompra.ToString() + " AND FIAPIDAPPS = " + aIdApps;
                        dbCnx.SetQuery(strSql);

                        #endregion

                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();

                        respuesta.Ok = "SI";
                        respuesta.Mensaje = "Registro exitoso.";
                        respuesta.Objeto = null;
                        return respuesta;

                    }
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();
                        dbCnx.CerrarConexion();

                        throw new Exception();
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "No fue posible registrar los datos fiscales.";
                    respuesta.Objeto = null;
                    return respuesta;
                }
            });
        }

        internal Task<Respuesta> RegistraDatosFiscales(DatosFiscales datosFiscales, int IdTipoPersona, string aIdApps)
        {
            return Task.Run(() =>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                Respuesta respuesta = new Respuesta();
                DatosFiscales dato = new DatosFiscales();

                string jsonPedido = string.Empty;
                string strSql = "";

                long lngIdPersona = 0;
                long lngIdPedido = 0;
                int idAgencia = 0;
                string strRutaReferenciaBancaria = "";
                string strNumeroSerie = "";
                decimal dcmTotalPedido = 0;
                try
                {
                    dato = datosFiscales;

                    #region RFC
                    if (dato.RfcFisica != null && dato.RfcFisica.Trim() != "")
                    {
                        dato.RfcFisica = BPedido.FormatoRfc(dato.RfcFisica.Trim());
                    }

                    if (dato.RfcRazonSocial != null && dato.RfcRazonSocial.Trim() != "")
                    {
                        dato.RfcRazonSocial = BPedido.FormatoRfc(dato.RfcRazonSocial.Trim());
                    }

                    if (dato.RfcRepresentanteLegal != null && dato.RfcRepresentanteLegal.Trim() != "")
                    {
                        dato.RfcRepresentanteLegal = BPedido.FormatoRfc(dato.RfcRepresentanteLegal.Trim());
                    }
                    #endregion

                    string idInventario = "";
                    strSql = "select FIAPIDCIAU, trim(FSAPNUMSER) NumeroSerie, FDAPTOTAL Total, FIAPIDINVE from prodapps.APEPANVW where FIAPIDCOMP = " + datosFiscales.IdCompra.ToString() + " AND FIAPIDAPPS = " + aIdApps;
                    DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        strNumeroSerie = dt.Rows[0]["NumeroSerie"].ToString().Trim();
                        decimal.TryParse(dt.Rows[0]["Total"].ToString(), out dcmTotalPedido);
                        idInventario = dt.Rows[0]["FIAPIDINVE"].ToString().Trim();
                        idAgencia = Convert.ToInt32(dt.Rows[0]["FIAPIDCIAU"].ToString().Trim());
                    }
                    else
                    {
                        throw new Exception();
                    }

                    #region APARTADO

                    try
                    {
                        dbCnx.AbrirConexion();
                        dbCnx.BeginTransaccion();

                        strSql = "";
                        strSql += "UPDATE PRODAUT.ANCAUTOM  SET FNAUTOEST = 10, \t";
                        strSql += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' \t";
                        strSql += "WHERE FNAUTOAGE = " + idAgencia + " \t";
                        strSql += "AND FIANIDINVE=" + idInventario + "\t";
                        strSql += "AND FIANSTATU = 1 \t";
                        strSql += "AND FNAUTOEST = 50";

                        dbCnx.SetQuery(strSql);

                        #region Registra CLIENTE

                        DVARegistraPersona.RegistraPersona persona = new DVARegistraPersona.RegistraPersona();

                        if (!string.IsNullOrEmpty(datosFiscales.RazonSocial.Trim()))
                        {
                            persona.RFC = datosFiscales.RfcRazonSocial.Trim();
                            persona.RazonSocial = datosFiscales.RazonSocial.Trim();
                            persona.NumeroCelular = datosFiscales.NumeroTelefonoFisica.Trim();

                            if (IdTipoPersona == 1)
                            {
                                persona.Email = datosFiscales.CorreoFisica.Trim();
                            }
                            else if (IdTipoPersona == 2)
                            {
                                persona.Email = datosFiscales.CorreoRepresentantelegal.ToString().Trim();
                            }


                        }
                        else
                        {
                            persona.Nombre = datosFiscales.NombreFisica.Trim();
                            persona.ApellidoPaterno = datosFiscales.ApellidoPaternoFisica.Trim();
                            persona.ApellidoMaterno = datosFiscales.ApellidoMaternoFisica.Trim();
                            persona.NumeroCelular = datosFiscales.NumeroTelefonoFisica.Trim();
                            persona.Email = datosFiscales.CorreoFisica.Trim();

                            if (datosFiscales.RfcFisica.Trim() != "" && datosFiscales.RfcFisica != null)
                            {

                                persona.RFC = datosFiscales.RfcFisica.Trim().ToUpper();

                            }
                        }

                        string idPersona = ObtieneORegistraPersonaApps(ref dbCnx, idAgencia, "APP", "APP", persona);

                        bool isRegister = long.TryParse(idPersona, out lngIdPersona);

                        if (isRegister == false)
                            throw new Exception();

                        #endregion

                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();

                    }
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();
                        dbCnx.CerrarConexion();
                        throw new Exception();
                    }

                    #endregion

                    try
                    {
                        #region  REGISTRO PEDIDO

                        SolicitudPedido solicitudPedido = new SolicitudPedido();
                        ApuntadorDeServicio _prod = new ApuntadorDeServicio();
                        strSql = string.Empty;
                        strSql = @"SELECT FSAPDESCRI FROM prodapps.APECMPVW" +
                                    " WHERE FIAPIDCOMP = " + datosFiscales.IdCompra + " AND FIAPIDAPPS =" + aIdApps;
                        solicitudPedido.Marca = dbCnx.GetDataSet(strSql).Tables[0].Rows[0]["FSAPDESCRI"].ToString().Trim() == "SINMARCA" ? string.Empty : dbCnx.GetDataSet(strSql).Tables[0].Rows[0]["FSAPDESCRI"].ToString().Trim();
                        solicitudPedido.RutaFisica = @"C:\inetpub\wwwroot\wsApiVW\Resources\Adjuntos";
                        solicitudPedido.IdCliente = lngIdPersona;
                        solicitudPedido.IdAgencia = idAgencia.ToString();
                        solicitudPedido.IdPrograma = "1";
                        solicitudPedido.IdUsuario = "7244";
                        solicitudPedido.Serie = strNumeroSerie;
                        solicitudPedido.IdAgente = 999996;
                        solicitudPedido.IdContacto = 0;
                        solicitudPedido.IdTipoDeVenta = 52;
                        solicitudPedido.Total = dcmTotalPedido;
                        jsonPedido = JsonConvert.SerializeObject(solicitudPedido);

                        string valor = idAgencia + "|7244|1|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        DVAAutosystServerClasses.Seguridad.Seguridad seg = new DVAAutosystServerClasses.Seguridad.Seguridad();
                        string token = seg.EncriptarCadena(valor);

                        string url_ped = _prod.RespuestaFacturacion();
                        url_ped = url_ped.Replace("valor", token);

                        var httpWebRequest_ped = (HttpWebRequest)WebRequest.Create(url_ped);

                        httpWebRequest_ped.Timeout = 40000;

                        httpWebRequest_ped.ContentType = "application/json";
                        httpWebRequest_ped.Method = "POST";

                        using (var streamWriter = new StreamWriter(httpWebRequest_ped.GetRequestStream()))
                        {
                            streamWriter.Write(jsonPedido);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        var httpResponse_ped = (HttpWebResponse)httpWebRequest_ped.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse_ped.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            RespuestaPedido respuestaPedido = new RespuestaPedido();
                            string error = string.Empty;
                            if (result.Contains("IdPedido") && result.Contains("Ruta"))
                                respuestaPedido = JsonConvert.DeserializeObject<RespuestaPedido>(result);
                            else
                                error = JsonConvert.DeserializeObject<string>(result);

                            if (!string.IsNullOrEmpty(error))
                            {
                                throw new Exception();
                            }
                            else
                            {
                                long.TryParse(respuestaPedido.IdPedido, out lngIdPedido);
                                strRutaReferenciaBancaria = respuestaPedido.Ruta.Replace("C:\\inetpub\\wwwroot\\wsApiVW\\Resources\\Adjuntos\\", "http://ws-smartit.divisionautomotriz.com/wsApiVW/Resources/Adjuntos/");
                                strSql = string.Empty;
                                strSql = @"UPDATE PRODAPPS.APECMPVW SET FSAPRUTRFB =  '" + strRutaReferenciaBancaria + "' WHERE FIAPIDCOMP =" + datosFiscales.IdCompra + " AND FIAPIDAPPS= " + aIdApps;
                                dbCnx.SetQuery(strSql);
                            }
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {

                        #region APARTADO

                        try
                        {
                            dbCnx.AbrirConexion();
                            dbCnx.BeginTransaccion();

                            strSql = "";
                            strSql += "UPDATE PRODAUT.ANCAUTOM  SET FNAUTOEST = 50, \t";
                            strSql += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' \t";
                            strSql += "WHERE FNAUTOAGE = " + idAgencia + " \t";
                            strSql += "AND FIANIDINVE=" + idInventario + "\t";
                            strSql += "AND FIANSTATU = 1 \t";
                            strSql += "AND FNAUTOEST = 10";

                            dbCnx.SetQuery(strSql);

                            dbCnx.CommitTransaccion();
                            dbCnx.CerrarConexion();

                        }
                        catch (Exception)
                        {
                            dbCnx.RollbackTransaccion();
                            dbCnx.CerrarConexion();
                        }

                        #endregion

                        throw new Exception();
                    }

                    #region Tablas Apps

                    try
                    {

                        dbCnx.AbrirConexion();
                        dbCnx.BeginTransaccion();

                        string existe = "";
                        existe += "SELECT COUNT(*) COUNT FROM PRODAPPS.APDDTFVW ";
                        existe += "WHERE FIAPIDCOMP = " + dato.IdCompra + " ";
                        existe += "AND FIAPSTATUS = 1" + " AND FIAPIDAPPS= " + aIdApps;

                        DataTable exist = dbCnx.GetDataSet(existe).Tables[0];

                        foreach (DataRow dr in exist.Rows)
                        {
                            int cantidad = 0;

                            cantidad = Convert.ToInt32(dr["COUNT"].ToString().Trim());

                            #region DatosFiscales
                            if (cantidad > 0)
                            {
                                #region Actualizar
                                if (IdTipoPersona == 1)
                                { /*PERSONA FISICA*/

                                    strSql = "";
                                    strSql += "UPDATE PRODAPPS.APDDTFVW ";
                                    strSql += "SET FSAPRFCFIS = " + "'" + dato.RfcFisica.Trim().ToUpper() + "'" + ",";
                                    strSql += "FSAPNMBFIS = " + "'" + dato.NombreFisica.Trim().ToUpper() + "'" + ",FSAPAPTFIS = " + "'" + dato.ApellidoPaternoFisica.Trim().ToUpper() + "'";
                                    strSql += ",FSAPAMTFIS= " + "'" + dato.ApellidoMaternoFisica.Trim().ToUpper() + "'" + ",FIAPLDTFIS = " + 52 + ",FIAPNMTFIS=" + (string.IsNullOrEmpty(dato.NumeroTelefonoFisica) ? "Default" : dato.NumeroTelefonoFisica.Trim().ToUpper()) + ",FSAPCRRFIS=" + "'" + dato.CorreoFisica.Trim() + "'";
                                    strSql += ",FSAPRFCRSC =" + "'" + dato.RfcRazonSocial.Trim().ToUpper() + "'" + ",FSAPRAZSOC=" + "'" + dato.RazonSocial.Trim().ToUpper() +
                                        "',FSAPRFCRLG=" + "'" + dato.RfcRepresentanteLegal.Trim().ToUpper() +
                                        "'" + ",FSAPNMBRLG=" + "'" + dato.NombreRepresentanteLegal.Trim().ToUpper() + "'" + ",FSAPAPTRLG = " + "'" + dato.ApellidoPaternoRepresentanteLegal.Trim().ToUpper() + "'";
                                    strSql += ",FSAPAMTRLG = " + "'" + dato.ApellidoMaternoRepresentantelegal.Trim().ToUpper() + "'" + ",FIAPLDTRLG=" + 52 + ",FSAPCRRRLG=" + "'" + dato.CorreoRepresentantelegal.Trim() + "'" +
                                        ",FIAPNMTRLG=" + (string.IsNullOrEmpty(dato.NumeroTelefonoRepresentanteLegal.Trim()) ? "Default" : dato.NumeroTelefonoRepresentanteLegal.Trim().ToUpper());
                                    strSql += " ,FSAPCUCFDI = " + (string.IsNullOrEmpty(dato.ClaveUsoCfdi.Trim()) ? "Default" : "'" + dato.ClaveUsoCfdi.Trim().ToUpper() + "'") + ", FSAPDESCRI = " + (string.IsNullOrEmpty(dato.DescripcionUsoCfdi.Trim()) ? "Default" : "'" + dato.DescripcionUsoCfdi.Trim().ToUpper() + "'");

                                    strSql += ",FIAPIDNACP = " + dato.IdNacionalidadC2.ToString().Trim() + ", FSAPDESNAP = " + "'" + dato.DescripcionNacionalidadC2.ToString().Trim() + "'" + ", FSAPCURPPE = " + "'" + dato.CurpC2.ToString().Trim() + "'";
                                    strSql += ",FSAPDIRPER = " + "'" + dato.DireccionC2.ToString().Trim().ToUpper() + "'" + ",FIAPIDGENP =" + dato.IdGeneroPersonaPF.ToString().Trim() + ",FSAPDESGEP =" + "'" + dato.DescripcionGeneroPF.ToString().Trim() + "'";
                                    strSql += ",FIAPIDOCUP=" + dato.IdOcupacionPersonaPF.ToString().Trim() + ",FSAPDESOCP=" + "'" + dato.DescripcionOcupacionPF.ToString().Trim() + "'";

                                    strSql += " ,PROGCREAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, USERUPDAT = 'APP', PROGUPDAT = 'APP' ";
                                    strSql += "WHERE FIAPIDCOMP = " + dato.IdCompra + " AND FIAPIDAPPS=" + aIdApps;
                                    dbCnx.SetQuery(strSql);

                                }
                                else if (IdTipoPersona == 2)
                                {

                                    strSql = "";
                                    strSql += "UPDATE PRODAPPS.APDDTFVW ";
                                    strSql += "SET FSAPRFCFIS = " + "'" + dato.RfcFisica.Trim().ToUpper() + "'" + ",";
                                    strSql += "FSAPNMBFIS = " + "'" + dato.NombreFisica.Trim().ToUpper() + "'" + ",FSAPAPTFIS = " + "'" + dato.ApellidoPaternoFisica.Trim().ToUpper() + "'";
                                    strSql += ",FSAPAMTFIS= " + "'" + dato.ApellidoMaternoFisica.Trim().ToUpper() + "'" + ",FIAPLDTFIS = " + 52 + ",FIAPNMTFIS=" + (string.IsNullOrEmpty(dato.NumeroTelefonoFisica) ? "Default" : dato.NumeroTelefonoFisica.Trim().ToUpper()) + ",FSAPCRRFIS=" + "'" + dato.CorreoFisica.Trim() + "'";
                                    strSql += ",FSAPRFCRSC =" + "'" + dato.RfcRazonSocial.Trim().ToUpper() + "'" + ",FSAPRAZSOC=" + "'" + dato.RazonSocial.Trim().ToUpper() +
                                        "',FSAPRFCRLG=" + "'" + dato.RfcRepresentanteLegal.Trim().ToUpper() +
                                        "'" + ",FSAPNMBRLG=" + "'" + dato.NombreRepresentanteLegal.Trim().ToUpper() + "'" + ",FSAPAPTRLG = " + "'" + dato.ApellidoPaternoRepresentanteLegal.Trim().ToUpper() + "'";
                                    strSql += ",FSAPAMTRLG = " + "'" + dato.ApellidoMaternoRepresentantelegal.Trim().ToUpper() + "'" + ",FIAPLDTRLG=" + 52 + ",FSAPCRRRLG=" + "'" + dato.CorreoRepresentantelegal.Trim() + "'" +
                                        ",FIAPNMTRLG=" + (string.IsNullOrEmpty(dato.NumeroTelefonoRepresentanteLegal.Trim()) ? "Default" : dato.NumeroTelefonoRepresentanteLegal.Trim().ToUpper());
                                    strSql += " ,FSAPCUCFDI = " + (string.IsNullOrEmpty(dato.ClaveUsoCfdi.Trim()) ? "Default" : "'" + dato.ClaveUsoCfdi.Trim().ToUpper() + "'") + ", FSAPDESCRI = " + (string.IsNullOrEmpty(dato.DescripcionUsoCfdi.Trim()) ? "Default" : "'" + dato.DescripcionUsoCfdi.Trim().ToUpper() + "'");

                                    strSql += ",FIAPIDNACP = " + dato.IdNacionalidadC2.ToString().Trim() + ", FSAPDESNAP = " + "'" + dato.DescripcionNacionalidadC2.ToString().Trim() + "'" + ", FSAPCURPPE = " + "'" + dato.CurpC2.ToString().Trim() + "'";
                                    strSql += ",FSAPDIRPER =" + "'" + dato.DireccionC2.ToString().Trim().ToUpper() + "'" + ",FIAPIDTSOP = " + dato.IdTipoSociedadPM.ToString().Trim().ToUpper() + ",FSAPDESTSP=" + "'" + dato.DescripcionTipoSociedadPM.ToString().Trim().ToUpper() + "'";
                                    strSql += ",FFAPCREEMP= " + "'" + dato.FechaConstitucionEmpresaPM.ToString().Trim().ToUpper() + "'" + ",FIAPIDGIRO=" + dato.IdGiroDeLaEmpresaPM.ToString().Trim().ToUpper() + ",FSAPDESCGI =" + "'" + dato.DescripcionGiroDeLaEmpresaPM.ToString().Trim().ToUpper() + "'";
                                    strSql += ",FSAPDIRREP =" + "'" + dato.DireccionRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + ",FIAPIDNACR=" + dato.IdNacionalidadRepresentanteLegalVPF.ToString().Trim().ToUpper() + ",FSAPDESNAR=" + "'" + dato.DescripcionNacionalidadRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'";
                                    strSql += ",FIAPIDGENR=" + dato.IdGeneroRepresentanteLegalVPF.ToString().Trim().ToUpper() + ",FSAPDESGER=" + "'" + dato.DescripcionGeneroRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + ",FIAPIDOCUR=" + dato.IdOcupacionRepresentanteLegalVPF.ToString().Trim().ToUpper();
                                    strSql += ",FSAPDESOCR=" + "'" + dato.DescripcionOcupacionRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + ",FSAPAPTCON=" + "'" + dato.ApellidoPaternoContactoVPF.ToString().Trim().ToUpper() + "'" + ",FSAPAPMCON=" + "'" + dato.ApellidoMaternoContactoVPF.ToString().Trim().ToUpper() + "'";
                                    strSql += ",FSAPNOMCON=" + "'" + dato.NombresContactoVPF.ToString().Trim().ToUpper() + "'" + ",FIAPLADCON=" + 52 + ", FIAPTELCON=" + dato.TelefonoContactoVPF.ToString().Trim().ToUpper();
                                    strSql += ",FSAPCORCON=" + "'" + dato.CorreoContactoVPF.ToString().Trim().ToUpper() + "'";

                                    strSql += " ,PROGCREAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, USERUPDAT = 'APP', PROGUPDAT = 'APP' ";
                                    strSql += "WHERE FIAPIDCOMP = " + dato.IdCompra + " AND FIAPIDAPPS=" + aIdApps;
                                    dbCnx.SetQuery(strSql);

                                }
                                #endregion
                            }
                            else
                            {
                                #region Agregar
                                if (IdTipoPersona == 1)
                                { /*PERSONA FISICA*/
                                    strSql = "";
                                    strSql += "INSERT INTO PRODAPPS.APDDTFVW (";
                                    strSql += "FIAPIDCOMP, FSAPRFCFIS, FSAPNMBFIS, FSAPAPTFIS, ";
                                    strSql += "FSAPAMTFIS, FIAPLDTFIS, FIAPNMTFIS, FSAPCRRFIS, ";
                                    strSql += "FSAPRFCRSC, FSAPRAZSOC, FSAPRFCRLG, FSAPNMBRLG, FSAPAPTRLG, ";
                                    strSql += "FSAPAMTRLG, FIAPLDTRLG, FIAPNMTRLG, FSAPCRRRLG, FSAPCUCFDI, FSAPDESCRI, ";

                                    strSql += "FIAPIDNACP, FSAPDESNAP, FSAPCURPPE, ";
                                    strSql += "FSAPDIRPER, FIAPIDGENP, FSAPDESGEP, ";
                                    strSql += "FIAPIDOCUP, FSAPDESOCP, ";

                                    strSql += "FIAPSTATUS, USERCREAT, PROGCREAT,FIAPIDAPPS)";
                                    strSql += "VALUES (";
                                    strSql += dato.IdCompra + "," + "'" + dato.RfcFisica.Trim().ToUpper() + "'" + "," + "'" + dato.NombreFisica.Trim().ToUpper() + "'" + "," + "'" + dato.ApellidoPaternoFisica.Trim().ToUpper() + "'" + ",";
                                    strSql += "'" + dato.ApellidoMaternoFisica.Trim().ToUpper() + "'" + "," + 52 + "," + (string.IsNullOrEmpty(dato.NumeroTelefonoFisica) ? "Default" : dato.NumeroTelefonoFisica.Trim().ToUpper()) + "," + "'" + dato.CorreoFisica.Trim() + "'" + ",";
                                    strSql += "'" + dato.RfcRazonSocial.Trim().ToUpper() + "'" + "," + "'" + dato.RazonSocial.Trim().ToUpper() +
                                        "','" + dato.RfcRepresentanteLegal.Trim().ToUpper() +
                                        "','" + dato.NombreRepresentanteLegal.Trim().ToUpper() + "','" + dato.ApellidoPaternoRepresentanteLegal.Trim().ToUpper() + "',";
                                    strSql += "'" + dato.ApellidoMaternoRepresentantelegal.Trim().ToUpper() + "',52" +
                                        "," + (string.IsNullOrEmpty(dato.NumeroTelefonoRepresentanteLegal.Trim()) ? "Default" : dato.NumeroTelefonoRepresentanteLegal.Trim().ToUpper()) +
                                        ",'" + dato.CorreoRepresentantelegal.Trim() + "'" + "," + (string.IsNullOrEmpty(dato.ClaveUsoCfdi.Trim()) ? "Default" : "'" + dato.ClaveUsoCfdi.Trim().ToUpper() + "'") + "," + (string.IsNullOrEmpty(dato.DescripcionUsoCfdi.Trim()) ? "Default" : "'" + dato.DescripcionUsoCfdi.Trim().ToUpper() + "'");

                                    strSql += "," + dato.IdNacionalidadC2.ToString().Trim() + "," + "'" + dato.DescripcionNacionalidadC2.ToString().Trim() + "'" + "," + "'" + dato.CurpC2.ToString().Trim() + "'";
                                    strSql += "," + "'" + dato.DireccionC2.ToString().Trim().ToUpper() + "'" + "," + dato.IdGeneroPersonaPF.ToString().Trim() + "," + "'" + dato.DescripcionGeneroPF.ToString().Trim() + "'";
                                    strSql += "," + dato.IdOcupacionPersonaPF.ToString().Trim() + "," + "'" + dato.DescripcionOcupacionPF.ToString().Trim() + "'";

                                    strSql += ",1,'APP','APP', " + aIdApps + ")";

                                    dbCnx.SetQuery(strSql);
                                }
                                else if (IdTipoPersona == 2)
                                { /*PERSONA MORAL*/

                                    strSql = "";
                                    strSql += "INSERT INTO PRODAPPS.APDDTFVW (";
                                    strSql += "FIAPIDCOMP, FSAPRFCFIS, FSAPNMBFIS, FSAPAPTFIS, ";
                                    strSql += "FSAPAMTFIS, FIAPLDTFIS, FIAPNMTFIS, FSAPCRRFIS, ";
                                    strSql += "FSAPRFCRSC, FSAPRAZSOC, FSAPRFCRLG, FSAPNMBRLG, FSAPAPTRLG, ";
                                    strSql += "FSAPAMTRLG, FIAPLDTRLG, FIAPNMTRLG, FSAPCRRRLG, FSAPCUCFDI, FSAPDESCRI, ";
                                    // aqui inician los cambios del proceso 2
                                    strSql += "FIAPIDNACP,FSAPDESNAP,FSAPCURPPE,";
                                    strSql += "FSAPDIRPER,FIAPIDTSOP,FSAPDESTSP,";
                                    strSql += "FFAPCREEMP,FIAPIDGIRO,FSAPDESCGI,";
                                    strSql += "FSAPDIRREP,FIAPIDNACR,FSAPDESNAR,";
                                    strSql += "FIAPIDGENR,FSAPDESGER,FIAPIDOCUR,";
                                    strSql += "FSAPDESOCR,FSAPAPTCON,FSAPAPMCON,";
                                    strSql += "FSAPNOMCON,FIAPLADCON,FIAPTELCON,";
                                    strSql += "FSAPCORCON,";

                                    strSql += "FIAPSTATUS, USERCREAT, PROGCREAT,FIAPIDAPPS)";
                                    strSql += "VALUES (";
                                    strSql += dato.IdCompra + "," + "'" + dato.RfcFisica.Trim().ToUpper() + "'" + "," + "'" + dato.NombreFisica.Trim().ToUpper() + "'" + "," + "'" + dato.ApellidoPaternoFisica.Trim().ToUpper() + "'" + ",";
                                    strSql += "'" + dato.ApellidoMaternoFisica.Trim().ToUpper() + "'" + "," + 52 + "," + (string.IsNullOrEmpty(dato.NumeroTelefonoFisica) ? "Default" : dato.NumeroTelefonoFisica.Trim().ToUpper()) + "," + "'" + dato.CorreoFisica.Trim() + "'" + ",";
                                    strSql += "'" + dato.RfcRazonSocial.Trim().ToUpper() + "'" + "," + "'" + dato.RazonSocial.Trim().ToUpper() +
                                        "','" + dato.RfcRepresentanteLegal.Trim().ToUpper() +
                                        "','" + dato.NombreRepresentanteLegal.Trim().ToUpper() + "','" + dato.ApellidoPaternoRepresentanteLegal.Trim().ToUpper() + "',";
                                    strSql += "'" + dato.ApellidoMaternoRepresentantelegal.Trim().ToUpper() + "',52" +
                                        "," + (string.IsNullOrEmpty(dato.NumeroTelefonoRepresentanteLegal.Trim()) ? "Default" : dato.NumeroTelefonoRepresentanteLegal.Trim().ToUpper()) +
                                        ",'" + dato.CorreoRepresentantelegal.Trim() + "'" + "," + (string.IsNullOrEmpty(dato.ClaveUsoCfdi.Trim()) ? "Default" : "'" + dato.ClaveUsoCfdi.Trim().ToUpper() + "'") + "," + (string.IsNullOrEmpty(dato.DescripcionUsoCfdi.Trim()) ? "Default" : "'" + dato.DescripcionUsoCfdi.Trim().ToUpper() + "'");

                                    strSql += "," + dato.IdNacionalidadC2.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionNacionalidadC2.ToString().Trim().ToUpper() + "'" + "," + "'" + dato.CurpC2.ToString().Trim().ToUpper() + "'";
                                    strSql += "," + "'" + dato.DireccionC2.ToString().Trim().ToUpper() + "'" + "," + dato.IdTipoSociedadPM.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionTipoSociedadPM.ToString().Trim().ToUpper() + "'";
                                    strSql += "," + "'" + dato.FechaConstitucionEmpresaPM.ToString().Trim().ToUpper() + "'" + "," + dato.IdGiroDeLaEmpresaPM.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionGiroDeLaEmpresaPM.ToString().Trim().ToUpper() + "'";
                                    strSql += "," + "'" + dato.DireccionRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + "," + dato.IdNacionalidadRepresentanteLegalVPF.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionNacionalidadRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'";
                                    strSql += "," + dato.IdGeneroRepresentanteLegalVPF.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionGeneroRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + "," + dato.IdOcupacionRepresentanteLegalVPF.ToString().Trim().ToUpper();
                                    strSql += "," + "'" + dato.DescripcionOcupacionRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + "," + "'" + dato.ApellidoPaternoContactoVPF.ToString().Trim().ToUpper() + "'" + "," + "'" + dato.ApellidoMaternoContactoVPF.ToString().Trim().ToUpper() + "'";
                                    strSql += "," + "'" + dato.NombresContactoVPF.ToString().Trim().ToUpper() + "'" + "," + 52 + "," + dato.TelefonoContactoVPF.ToString().Trim().ToUpper();
                                    strSql += "," + "'" + dato.CorreoContactoVPF.ToString().Trim().ToUpper() + "'";

                                    strSql += ",1,'APP','APP', " + aIdApps + ")";

                                    dbCnx.SetQuery(strSql);

                                }
                                #endregion
                            }
                            #endregion
                        }


                        #region REGISTRO PEDIDOP APP AUTO              
                        strSql = "";
                        strSql = @"UPDATE PRODAPPS.APEPANVW SET FIAPIDPEDI = " + lngIdPedido.ToString() +
                            ", FFAPFECPED = CURRENT DATE, FHAPHORPED = CURRENT TIME, FIAPIDPERS = " + lngIdPersona.ToString() +
                            " WHERE FIAPIDCOMP = " + datosFiscales.IdCompra.ToString() + " AND FIAPIDAPPS = " + aIdApps;
                        dbCnx.SetQuery(strSql);

                        #endregion

                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();

                        respuesta.Ok = "SI";
                        respuesta.Mensaje = "Registro exitoso.";
                        respuesta.Objeto = null;
                        return respuesta;

                    }
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();
                        dbCnx.CerrarConexion();

                        throw new Exception();
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "No fue posible registrar los datos fiscales.";
                    respuesta.Objeto = null;
                    return respuesta;
                }
            });
        }

        public static string quitaAcentos(string cadena)
        {

            string cadenaA = "[á|à|ä|â]".ToString().ToUpper();
            string cadenaE = "[é|è|ë|ê]".ToString().ToUpper();
            string cadenaI = "[í|ì|ï|î]".ToString().ToUpper();
            string cadenaO = "[ó|ò|ö|ô]".ToString().ToUpper();
            string cadenaU = "[ú|ù|ü|û]".ToString().ToUpper();

            Regex reemplazaAcento_a = new Regex(cadenaA, RegexOptions.Compiled);
            Regex reemplazaAcento_e = new Regex(cadenaE, RegexOptions.Compiled);
            Regex reemplazaAcento_i = new Regex(cadenaI, RegexOptions.Compiled);
            Regex reemplazaAcento_o = new Regex(cadenaO, RegexOptions.Compiled);
            Regex reemplazaAcento_u = new Regex(cadenaU, RegexOptions.Compiled);

            cadena = reemplazaAcento_a.Replace(cadena, "A");
            cadena = reemplazaAcento_e.Replace(cadena, "E");
            cadena = reemplazaAcento_i.Replace(cadena, "I");
            cadena = reemplazaAcento_o.Replace(cadena, "O");
            cadena = reemplazaAcento_u.Replace(cadena, "U");
            return cadena;
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
        public static string ObtieneORegistraPersonaApps(ref DVADB.DB2 conexion, int IdAgencia, string IdUsuario, string IdPrograma, DVARegistraPersona.RegistraPersona registraPersona)
        {

            DVADB.DB2 dbCnx = conexion;

            try
            {

                if (registraPersona == null)
                    throw new Exception("Se debe recibir una objeto persona");

                foreach (PropertyInfo prop in registraPersona.GetType().GetProperties())
                {
                    if (prop.GetValue(registraPersona) == null)
                    {
                        prop.SetValue(registraPersona, "");
                    }
                }


                #region LIMPIADO DE ACENTOS Y PUNTO, GENERACION DE FORMATO DEL RFC
                // QUITAMOS ACENTOS 

                registraPersona.Nombre = quitaAcentos(registraPersona.Nombre.ToString().Trim().ToUpper());
                registraPersona.ApellidoPaterno = quitaAcentos(registraPersona.ApellidoPaterno.ToString().Trim().ToUpper());
                registraPersona.ApellidoMaterno = quitaAcentos(registraPersona.ApellidoMaterno.ToString().Trim().ToUpper());
                registraPersona.RazonSocial = quitaAcentos(registraPersona.RazonSocial.ToString().Trim().ToUpper());

                // QUITAMOS CARACTERES ESPECIALES

                if (registraPersona.RazonSocial.Contains("."))
                {
                    registraPersona.RazonSocial = registraPersona.RazonSocial.Replace(".", "");
                }


                // FORMATEAMOS EL RFC

                if (registraPersona.RFC.Contains("-"))
                {

                    string rfcSinAcentos = registraPersona.RFC.Replace("-", "");
                    registraPersona.RFC = FormatoRfc(rfcSinAcentos.ToString().Trim().ToUpper());

                }
                else
                {

                    if (registraPersona.RFC.Length > 0)
                    {
                        registraPersona.RFC = FormatoRfc(registraPersona.RFC.ToString().Trim().ToUpper());
                    }

                }


                #endregion



                if (registraPersona.RazonSocial == "")
                {

                    // Persona Fisica

                    if (registraPersona.RFC != "")
                    {

                        List<PersonaCliente> lstCliente = new List<PersonaCliente>();
                        PersonaCliente cliente = new PersonaCliente();

                        string strRfcB = "";
                        strRfcB += "SELECT FDPEIDPERS, FSPECTEUNI, FIPEIDCIAU, FSPERFC, FDPEIDTIPO, ";
                        strRfcB += "FDPEIDTCTE, FDPEIDCLAS, FFPEALTA, FHPEALTA, FFPEESTATU, FHPEESTATU FROM PRODPERS.CTEPERSO ";
                        strRfcB += "WHERE FSPERFC = " + "'" + registraPersona.RFC.ToString().ToUpper().Trim() + "'";
                        strRfcB += " AND FDPEESTATU = 1";

                        DataTable DtRFC = dbCnx.GetDataSet(strRfcB).Tables[0];

                        if (DtRFC.Rows.Count > 0)
                        {

                            foreach (DataRow dr in DtRFC.Rows)
                            {
                                cliente = new PersonaCliente();

                                cliente.IdPersona = dr["FDPEIDPERS"].ToString().Trim();
                                cliente.IdLegacy = dr["FSPECTEUNI"].ToString().Trim();
                                cliente.IdAgencia = dr["FIPEIDCIAU"].ToString().Trim();
                                cliente.Rfc = dr["FSPERFC"].ToString().Trim();
                                cliente.IdTipo = dr["FDPEIDTIPO"].ToString().Trim();
                                cliente.TipoCliente = dr["FDPEIDTCTE"].ToString().Trim();
                                cliente.ClasePersona = dr["FDPEIDCLAS"].ToString().Trim();
                                cliente.FechaAlta = dr["FFPEALTA"].ToString().Trim();
                                cliente.HoraAlta = dr["FHPEALTA"].ToString().Trim();
                                cliente.FechaEstatus = dr["FFPEESTATU"].ToString().Trim();
                                cliente.HoraEstatus = dr["FHPEESTATU"].ToString().Trim();

                                lstCliente.Add(cliente);
                            }

                        }


                        if (lstCliente.Count == 0)
                        {

                            long IdPersona = RegistraPersonaFisica(ref conexion, IdAgencia, registraPersona);
                            return IdPersona.ToString();

                        }


                        if (lstCliente.Count == 1)
                        {
                            return cliente.IdPersona;
                        }


                        if (lstCliente.Count >= 2)
                        {


                            RegistroPersonaFisica personaFisica = new RegistroPersonaFisica();
                            List<RegistroPersonaFisica> lstPersonaFisica = new List<RegistroPersonaFisica>();


                            string idsRfcs = string.Empty;

                            foreach (PersonaCliente dato in lstCliente)
                            {

                                idsRfcs += dato.IdPersona + ",";
                            }

                            string strBPF = string.Empty;

                            strBPF += "SELECT CPERFI.*, CTINOB.FSTNDESCRI, CPAISE.FSGECVEPAI, CPAISE.FSGEDEPAIS, COCUPA.FSOCCVEOCU ";
                            strBPF += "FROM PRODPERS.CTCPERFI CPERFI ";
                            strBPF += "LEFT JOIN PRODPERS.CTCTINOB CTINOB ON CTINOB.FDTNIDTINO = CPERFI.FDPFIDTINO ";
                            strBPF += "LEFT JOIN PRODGRAL.GECPAISE CPAISE ON CPAISE.FIGEIDPAIS = CPERFI.FIPEIDPAIS ";
                            strBPF += "LEFT JOIN PRODPERS.CTCOCUPA COCUPA ON CPERFI.FIPEIDOCUP = COCUPA.FDOCIDOCUP ";
                            strBPF += "WHERE CPERFI.FDPFIDPERS IN ( " + idsRfcs.Substring(0, idsRfcs.Length - 1) + " ) ";
                            strBPF += "AND CPERFI.FDPFESTATU = 1";

                            DataTable dtBPF = dbCnx.GetDataSet(strBPF).Tables[0];


                            if (dtBPF.Rows.Count > 0)
                            {

                                foreach (DataRow dr in dtBPF.Rows)
                                {

                                    personaFisica = new RegistroPersonaFisica();

                                    personaFisica.IdPersona = dr["FDPFIDPERS"].ToString().Trim();
                                    personaFisica.Nombre = dr["FSPFNOMBRE"].ToString().Trim();
                                    personaFisica.ApellidoPaterno = dr["FSPFAPATER"].ToString().Trim();
                                    personaFisica.ApellidoMaterno = dr["FSPFAMATER"].ToString().Trim();
                                    personaFisica.IdSexo = dr["FDPFIDSEXO"].ToString().Trim();
                                    personaFisica.IdEstadoCIvil = dr["FDPFIDEDCI"].ToString().Trim();
                                    personaFisica.FechaAlta = Convert.ToDateTime(dr["DATECREAT"].ToString().Trim());

                                    lstPersonaFisica.Add(personaFisica);
                                }

                                List<RegistroPersonaFisica> lstListaPersonasConMismoNombre = new List<RegistroPersonaFisica>();

                                lstListaPersonasConMismoNombre = lstPersonaFisica.FindAll(x => x.Nombre == registraPersona.Nombre.ToString().Trim() && x.ApellidoPaterno == registraPersona.ApellidoPaterno.ToString().Trim() && x.ApellidoMaterno == registraPersona.ApellidoMaterno.ToString().Trim());


                                if (lstListaPersonasConMismoNombre.Count == 0)
                                {

                                    long IdPersona = RegistraPersonaFisica(ref conexion, IdAgencia, registraPersona);
                                    return IdPersona.ToString();
                                }


                                if (lstListaPersonasConMismoNombre.Count == 1)
                                {

                                    return lstListaPersonasConMismoNombre[0].IdPersona;

                                }

                                if (lstListaPersonasConMismoNombre.Count >= 2)
                                {
                                    if (registraPersona.RFC.ToString().Trim() == "" || registraPersona.RFC.ToString().Trim() == null)
                                    {

                                        PersonaTelefono NumTelefonosPer = new PersonaTelefono();
                                        List<PersonaTelefono> lstNumTelefonosPer = new List<PersonaTelefono>();

                                        string idsPMN = string.Empty;
                                        foreach (RegistroPersonaFisica perMN in lstListaPersonasConMismoNombre)
                                        {
                                            idsPMN += perMN.IdPersona + ",";
                                        }

                                        string strTel = string.Empty;

                                        strTel += "SELECT * FROM ";
                                        strTel += "PRODPERS.CTDRPTEL ";
                                        strTel += "WHERE  FDPTIDPERS IN ( " + idsPMN.Substring(0, idsPMN.Length - 1) + ")";
                                        strTel += "AND FDPTESTATU = 1 ";

                                        DataTable dtTele = dbCnx.GetDataSet(strTel).Tables[0];

                                        foreach (DataRow dr in dtTele.Rows)
                                        {

                                            NumTelefonosPer = new PersonaTelefono();
                                            NumTelefonosPer.IdPersona = dr["FDPTIDPERS"].ToString().Trim();
                                            NumTelefonosPer.Lada = dr["FDPTPLADA"].ToString().Trim();
                                            NumTelefonosPer.NumeroTelefono = NumTelefonosPer.Lada + dr["FDPTNUMTEL"].ToString().Trim();

                                            lstNumTelefonosPer.Add(NumTelefonosPer);
                                        }

                                        List<PersonaTelefono> lstTelefonoParaMismaPersona = lstNumTelefonosPer.FindAll(p => p.NumeroTelefono == registraPersona.NumeroCelular);

                                        if (lstTelefonoParaMismaPersona.Count == 0)
                                        {

                                            long IdPersona = RegistraPersonaFisica(ref conexion, IdAgencia, registraPersona);
                                            return IdPersona.ToString();
                                        }

                                        if (lstTelefonoParaMismaPersona.Count == 1 || lstTelefonoParaMismaPersona.Count >= 2)
                                        {

                                            return lstTelefonoParaMismaPersona[0].IdPersona;
                                        }

                                    }
                                    else
                                    {

                                        lstListaPersonasConMismoNombre = lstListaPersonasConMismoNombre.OrderBy(i => i.FechaAlta).ToList();

                                        return lstListaPersonasConMismoNombre[0].IdPersona;

                                    }
                                }

                            }
                            else
                            {

                                long IdPersona = RegistraPersonaFisica(ref conexion, IdAgencia, registraPersona);
                                return IdPersona.ToString();

                            }
                        }

                    }
                    else
                    {

                        // CUANDO EL RFC ESTA VACIO 

                        // se busca la coincidencia por nombre

                        string IdsCoincidencias = string.Empty;
                        string strFisica = "";

                        strFisica += "SELECT CPERFI.*, CTINOB.FSTNDESCRI, CPAISE.FSGECVEPAI, CPAISE.FSGEDEPAIS, COCUPA.FSOCCVEOCU ";
                        strFisica += "FROM PRODPERS.CTCPERFI CPERFI ";
                        strFisica += "LEFT JOIN PRODPERS.CTCTINOB CTINOB ON CTINOB.FDTNIDTINO = CPERFI.FDPFIDTINO ";
                        strFisica += "LEFT JOIN PRODGRAL.GECPAISE CPAISE ON CPAISE.FIGEIDPAIS = CPERFI.FIPEIDPAIS ";
                        strFisica += "LEFT JOIN PRODPERS.CTCOCUPA COCUPA ON CPERFI.FIPEIDOCUP = COCUPA.FDOCIDOCUP ";
                        strFisica += "WHERE CPERFI.FSPFNOMBRE = " + "'" + registraPersona.Nombre.ToString().ToUpper().Trim() + "' ";
                        strFisica += "AND FSPFAPATER = " + "'" + registraPersona.ApellidoPaterno.ToString().ToUpper().Trim() + "' ";
                        strFisica += "AND FSPFAMATER = " + "'" + registraPersona.ApellidoMaterno.ToString().ToUpper().Trim() + "' ";
                        strFisica += "AND FDPFESTATU = 1 ";


                        DataTable dtPF = dbCnx.GetDataSet(strFisica).Tables[0];

                        if (dtPF.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtPF.Rows)
                            {
                                IdsCoincidencias += dr["FDPFIDPERS"].ToString().Trim() + ",";
                            }


                            //mediante el IdPersona recuperados hay que buscar las personas que satisfacen 
                            // la regla que cumplen con el nombre para posteriormente eliminar aquellos que tienen RFC


                            List<PersonaCliente> lstPersonas = new List<PersonaCliente>();
                            PersonaCliente persona = new PersonaCliente();
                            PersonaTelefono NumTelefonosPer = new PersonaTelefono();
                            List<PersonaTelefono> lstNumTelefonosPer = new List<PersonaTelefono>();




                            string strRfcB = "";
                            strRfcB += "SELECT FDPEIDPERS, FSPECTEUNI, FIPEIDCIAU, FSPERFC, FDPEIDTIPO, ";
                            strRfcB += "FDPEIDTCTE, FDPEIDCLAS, FFPEALTA, FHPEALTA, FFPEESTATU, FHPEESTATU FROM PRODPERS.CTEPERSO ";
                            strRfcB += "WHERE FDPEIDPERS IN ( " + IdsCoincidencias.Substring(0, IdsCoincidencias.Length - 1) + ") ";
                            strRfcB += " AND FDPEESTATU = 1";

                            DataTable DtRFC = dbCnx.GetDataSet(strRfcB).Tables[0];

                            if (DtRFC.Rows.Count > 0)
                            {

                                foreach (DataRow dr in DtRFC.Rows)
                                {
                                    persona = new PersonaCliente();

                                    persona.IdPersona = dr["FDPEIDPERS"].ToString().Trim();
                                    persona.IdLegacy = dr["FSPECTEUNI"].ToString().Trim();
                                    persona.IdAgencia = dr["FIPEIDCIAU"].ToString().Trim();
                                    persona.Rfc = dr["FSPERFC"].ToString().Trim();
                                    persona.IdTipo = dr["FDPEIDTIPO"].ToString().Trim();
                                    persona.TipoCliente = dr["FDPEIDTCTE"].ToString().Trim();
                                    persona.ClasePersona = dr["FDPEIDCLAS"].ToString().Trim();
                                    persona.FechaAlta = dr["FFPEALTA"].ToString().Trim();
                                    persona.HoraAlta = dr["FHPEALTA"].ToString().Trim();
                                    persona.FechaEstatus = dr["FFPEESTATU"].ToString().Trim();
                                    persona.HoraEstatus = dr["FHPEESTATU"].ToString().Trim();

                                    lstPersonas.Add(persona);
                                }

                            }

                            // quitamos aquellas personas que tienen RFC

                            string IdsPersonasSinRFC = string.Empty;

                            lstPersonas = lstPersonas.FindAll(c => c.Rfc == "");

                            foreach (PersonaCliente dato in lstPersonas)
                            {

                                IdsPersonasSinRFC += dato.IdPersona + ",";

                            }

                            // buscamos los telefonos por los ids que no tienen RFC                            

                            string strTel = string.Empty;

                            strTel += "SELECT * FROM ";
                            strTel += "PRODPERS.CTDRPTEL ";
                            strTel += "WHERE  FDPTIDPERS IN ( " + IdsPersonasSinRFC.Substring(0, IdsPersonasSinRFC.Length - 1) + ")";
                            strTel += "AND FDPTESTATU = 1 ";

                            DataTable dtTele = dbCnx.GetDataSet(strTel).Tables[0];

                            foreach (DataRow dr in dtTele.Rows)
                            {

                                NumTelefonosPer = new PersonaTelefono();
                                NumTelefonosPer.IdPersona = dr["FDPTIDPERS"].ToString().Trim();
                                NumTelefonosPer.Lada = dr["FDPTPLADA"].ToString().Trim();
                                NumTelefonosPer.NumeroTelefono = NumTelefonosPer.Lada + dr["FDPTNUMTEL"].ToString().Trim();

                                lstNumTelefonosPer.Add(NumTelefonosPer);
                            }

                            List<PersonaTelefono> lstTelefonoParaMismaPersona = lstNumTelefonosPer.FindAll(p => p.NumeroTelefono == registraPersona.NumeroCelular);

                            if (lstTelefonoParaMismaPersona.Count == 0)
                            {

                                long IdPersona = RegistraPersonaFisica(ref conexion, IdAgencia, registraPersona);
                                return IdPersona.ToString();
                            }

                            if (lstTelefonoParaMismaPersona.Count == 1 || lstTelefonoParaMismaPersona.Count >= 2)
                            {

                                return lstTelefonoParaMismaPersona[0].IdPersona;
                            }

                            ///////               

                        }
                        else
                        {

                            // si no hay coincidencia por nombres, entonces se registra el cliente 
                            long IdPersona = RegistraPersonaFisica(ref conexion, IdAgencia, registraPersona);
                            return IdPersona.ToString();
                        }

                    }
                }
                else
                {
                    //Persona Moral

                    // Buscamos el Id de persona mediante el rfc

                    if (registraPersona.RFC == "" && registraPersona.RFC == null)
                    {
                        throw new Exception();
                    }


                    List<PersonaCliente> lstClienteMoral = new List<PersonaCliente>();
                    PersonaCliente clienteMoral = new PersonaCliente();

                    string rfcsCoincidentes = string.Empty;

                    string strRfcM = "";
                    strRfcM += "SELECT FDPEIDPERS, FSPECTEUNI, FIPEIDCIAU, FSPERFC, FDPEIDTIPO, ";
                    strRfcM += "FDPEIDTCTE, FDPEIDCLAS, FFPEALTA, FHPEALTA, FFPEESTATU, FHPEESTATU FROM PRODPERS.CTEPERSO ";
                    strRfcM += "WHERE FSPERFC = " + "'" + registraPersona.RFC.ToString().ToUpper().Trim() + "'";
                    strRfcM += " AND FDPEESTATU = 1";

                    DataTable DtRFM = dbCnx.GetDataSet(strRfcM).Tables[0];

                    if (DtRFM.Rows.Count > 0)
                    {

                        foreach (DataRow dr in DtRFM.Rows)
                        {
                            clienteMoral = new PersonaCliente();

                            clienteMoral.IdPersona = dr["FDPEIDPERS"].ToString().Trim();
                            clienteMoral.IdLegacy = dr["FSPECTEUNI"].ToString().Trim();
                            clienteMoral.IdAgencia = dr["FIPEIDCIAU"].ToString().Trim();
                            clienteMoral.Rfc = dr["FSPERFC"].ToString().Trim();
                            clienteMoral.IdTipo = dr["FDPEIDTIPO"].ToString().Trim();
                            clienteMoral.TipoCliente = dr["FDPEIDTCTE"].ToString().Trim();
                            clienteMoral.ClasePersona = dr["FDPEIDCLAS"].ToString().Trim();
                            clienteMoral.FechaAlta = dr["FFPEALTA"].ToString().Trim();
                            clienteMoral.HoraAlta = dr["FHPEALTA"].ToString().Trim();
                            clienteMoral.FechaEstatus = dr["FFPEESTATU"].ToString().Trim();
                            clienteMoral.HoraEstatus = dr["FHPEESTATU"].ToString().Trim();

                            lstClienteMoral.Add(clienteMoral);
                            rfcsCoincidentes += dr["FDPEIDPERS"].ToString().Trim() + ",";

                        }

                    }


                    if (lstClienteMoral.Count == 0)
                    {
                        long IdPersona = RegistraPersonaMoral(ref conexion, IdAgencia, registraPersona);
                        return IdPersona.ToString();
                    }


                    if (lstClienteMoral.Count == 1)
                    {
                        return lstClienteMoral[0].IdPersona;
                    }

                    if (lstClienteMoral.Count >= 2)
                    {

                        List<PersonaMoral> lstPersonaMoral = new List<PersonaMoral>();
                        PersonaMoral personaMoral = new PersonaMoral();

                        string strBPM = string.Empty;
                        strBPM += "SELECT DPERMO.*, CGRCME.FSCTCVEGRC ";
                        strBPM += "FROM PRODPERS.CTDPERMO DPERMO ";
                        strBPM += "LEFT JOIN PRODPERS.CTCGRCME CGRCME ON CGRCME.FICTIDGRCM = DPERMO.FICTIDGRCM ";
                        strBPM += "WHERE ";
                        strBPM += "FSPMRAZON = " + "'" + registraPersona.RazonSocial.ToString().ToUpper().Trim() + "' ";
                        strBPM += "AND FDPMIDPERS IN (" + rfcsCoincidentes.Substring(0, rfcsCoincidentes.Length - 1) + ") ";
                        strBPM += "AND FDPMESTATU = 1 ";


                        /*  string strBPM = string.Empty;
                          strBPM += "SELECT DPERMO.*, CGRCME.FSCTCVEGRC ";
                          strBPM += "FROM PRODPERS.CTDPERMO DPERMO ";
                          strBPM += "LEFT JOIN PRODPERS.CTCGRCME CGRCME ON CGRCME.FICTIDGRCM = DPERMO.FICTIDGRCM ";
                          strBPM += "WHERE ";
                          strBPM += "FDPMIDPERS IN (" + rfcsCoincidentes.Substring(0, rfcsCoincidentes.Length-1) + ")";
                          strBPM += "AND FDPMESTATU = 1";
                          */


                        DataTable dtPM = dbCnx.GetDataSet(strBPM).Tables[0];

                        if (dtPM.Rows.Count > 0)
                        {

                            foreach (DataRow dr in dtPM.Rows)
                            {

                                personaMoral = new PersonaMoral();
                                personaMoral.IdPersona = dr["FDPMIDPERS"].ToString().Trim();
                                personaMoral.RazonSocial = dr["FSPMRAZON"].ToString().Trim();
                                personaMoral.IdRepresentanteLegal = dr["FICTIDRPLG"].ToString().Trim();
                                personaMoral.IdGiroComercial = dr["FICTIDGRCM"].ToString().Trim();

                                lstPersonaMoral.Add(personaMoral);
                            }

                        }

                        if (lstPersonaMoral.Count == 0)
                        {
                            long IdPersona = RegistraPersonaMoral(ref conexion, IdAgencia, registraPersona);
                            return IdPersona.ToString();
                        }

                        if (lstPersonaMoral.Count == 1)
                        {

                            return lstPersonaMoral[0].IdPersona;
                        }

                        if (lstPersonaMoral.Count >= 2)
                        {

                            return lstPersonaMoral[0].IdPersona;

                        }



                    }

                }
                return ""; // quitar
            }
            catch (Exception ex)
            {
                return ex.Message + ex.StackTrace;

            }
        }

        private static long RegistraPersonaFisica(ref DVADB.DB2 conexion, int IdAgencia, DVARegistraPersona.RegistraPersona persona)
        {
            // EL RFC DEBE IR FORMATEADO CON GUIONES

            DVADB.DB2 dbCnx = conexion;

            string insertaPersona = string.Empty;
            long IdPersona = 0;
            PERSControlDeFolio folio = new PERSControlDeFolio();
            string obtieneFoliadorPersona = string.Empty;
            int max = 1;

            try
            {
                #region FOLIADOR

                obtieneFoliadorPersona += "SELECT FDFOIDFOLI, FSFODESCRI, FDFOULTFOL, FBFOESTATU FROM ";
                obtieneFoliadorPersona += "PRODPERS.CTDFOLIO ";
                obtieneFoliadorPersona += "WHERE FBFOESTATU = 1 ";
                obtieneFoliadorPersona += "AND FDFOIDFOLI = 1";

                DataTable dtFoliador = dbCnx.GetDataSet(obtieneFoliadorPersona).Tables[0];
                if (dtFoliador.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtFoliador.Rows)
                    {
                        folio = new PERSControlDeFolio();
                        folio.IdFolio = Convert.ToInt32(dr["FDFOIDFOLI"].ToString().Trim());
                        folio.Descripcion = dr["FSFODESCRI"].ToString().Trim();
                        folio.FolioActual = Convert.ToInt32(dr["FDFOULTFOL"].ToString().Trim());
                        folio.Estatus = dr["FBFOESTATU"].ToString().Trim();
                    }
                }

                PERSControlDeFolio control = folio;

                int lastIdPersona = folio.FolioActual;

                if (control != null)
                    max = control.FolioActual + 1;

                if (max <= lastIdPersona)
                    max = lastIdPersona + 1;

                if (control == null)
                {
                    control = new PERSControlDeFolio();
                    control.FolioActual = max;
                }
                else
                {
                    control.FolioActual = max;
                    // aDB.Update(aIdUser, aIdProgram, control);

                    string actualizaFoliador = "";

                    actualizaFoliador += "UPDATE PRODPERS.CTDFOLIO ";
                    actualizaFoliador += "SET FDFOULTFOL = " + max;
                    actualizaFoliador += ", USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' ";
                    actualizaFoliador += "WHERE FDFOIDFOLI = 1 ";
                    actualizaFoliador += "AND FBFOESTATU = 1";

                    dbCnx.SetQuery(actualizaFoliador);
                }
                #endregion

                insertaPersona += "select FDPEIDPERS from NEW TABLE ( ";

                insertaPersona += "INSERT INTO PRODPERS.CTEPERSO ( ";
                insertaPersona += "FDPEIDPERS, FSPECTEUNI, FIPEIDCIAU, ";
                insertaPersona += "FSPERFC, FDPEIDTIPO, FDPEIDTCTE,";
                insertaPersona += "FDPEIDCLAS, FFPEALTA, FHPEALTA, ";
                insertaPersona += "FFPEESTATU, FHPEESTATU, FDPEESTATU, ";
                insertaPersona += "USERCREAT, DATECREAT, TIMECREAT, PROGCREAT ";
                insertaPersona += ") VALUES ( ";
                insertaPersona += max + ", " + max + ", " + IdAgencia + " ";
                insertaPersona += "," + "'" + persona.RFC.ToString().ToUpper().Trim() + "'" + "," + 1 + "," + 1 + " ";
                insertaPersona += "," + 1 + "," + "CURRENT DATE" + "," + "CURRENT TIME" + ",";
                insertaPersona += "CURRENT DATE" + "," + "CURRENT TIME" + "," + 1;
                insertaPersona += "," + "'APP', CURRENT DATE, CURRENT TIME, 'APP'";
                insertaPersona += ")";

                insertaPersona += ")";


                DataTable dtPeM = dbCnx.GetDataSet(insertaPersona).Tables[0];

                if (dtPeM.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtPeM.Rows)
                    {
                        IdPersona = Convert.ToInt32(dr["FDPEIDPERS"].ToString().Trim());
                    }
                }
                else
                {
                    throw new Exception();
                }


                string insertaPersFis = string.Empty;

                insertaPersFis += "INSERT INTO PRODPERS.CTCPERFI (";
                insertaPersFis += "FDPFIDPERS, FSPFNOMBRE, FSPFAPATER, ";
                insertaPersFis += "FSPFAMATER, FDPFIDSEXO, FDPFIDEDCI, ";
                insertaPersFis += "FDPFESTATU, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT";
                insertaPersFis += ") VALUES ( ";
                insertaPersFis += IdPersona + "," + "'" + persona.Nombre.ToString().Trim().ToUpper() + "'" + "," + "'" + persona.ApellidoPaterno.ToString().Trim().ToUpper() + "'";
                insertaPersFis += "," + "'" + persona.ApellidoMaterno.ToString().Trim().ToUpper() + "'" + "," + 1 + "," + 2 + " ";
                insertaPersFis += "," + "1, 'APP', CURRENT DATE, CURRENT TIME, 'APP' ";
                insertaPersFis += ")";

                dbCnx.SetQuery(insertaPersFis);


                if (persona.NumeroCelular != "" && persona.NumeroCelular.Length == 10)
                {

                    string Lada = string.Empty;
                    string Telefono = string.Empty;
                    if (persona.NumeroCelular.StartsWith("55") || persona.NumeroCelular.StartsWith("81") || persona.NumeroCelular.StartsWith("33"))
                    {
                        Lada = persona.NumeroCelular.Substring(0, 2);
                        Telefono = persona.NumeroCelular.Substring(2, 8);
                    }
                    else
                    {
                        Lada = persona.NumeroCelular.Substring(0, 3);
                        Telefono = persona.NumeroCelular.Substring(3, 7);
                    }



                    string insertaTelefono = string.Empty;

                    insertaTelefono += "INSERT INTO PRODPERS.CTDRPTEL (";
                    insertaTelefono += "FDPTIDPERS, FDPTCONTEL, FDPTIDTELE,";
                    insertaTelefono += "FDPTPLADA, FDPTNUMTEL, FBCTDEFAUL,";
                    insertaTelefono += "FDPTESTATU, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT";
                    insertaTelefono += ") VALUES (";
                    insertaTelefono += IdPersona + "," + 1 + "," + 2 + ",";
                    insertaTelefono += Lada + "," + Telefono + "," + 1 + ",";
                    insertaTelefono += "1, 'APP', CURRENT DATE, CURRENT TIME, 'APP' ";
                    insertaTelefono += ")";

                    dbCnx.SetQuery(insertaTelefono);

                }

            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                throw new Exception();
            }


            return IdPersona;

        }

        private static long RegistraPersonaMoral(ref DVADB.DB2 conexion, int IdAgencia, DVARegistraPersona.RegistraPersona persona)
        {

            // EL RFC DEBE IR FORMATEADO CON GUIONES

            DVADB.DB2 dbCnx = conexion;

            string insertaPersona = string.Empty;
            long IdPersona = 0;
            PERSControlDeFolio folio = new PERSControlDeFolio();
            string obtieneFoliadorPersona = string.Empty;
            int max = 1;
            try
            {
                #region FOLIADOR



                obtieneFoliadorPersona += "SELECT FDFOIDFOLI, FSFODESCRI, FDFOULTFOL, FBFOESTATU FROM ";
                obtieneFoliadorPersona += "PRODPERS.CTDFOLIO ";
                obtieneFoliadorPersona += "WHERE FBFOESTATU = 1 ";
                obtieneFoliadorPersona += "AND FDFOIDFOLI = 1";




                DataTable dtFoliador = dbCnx.GetDataSet(obtieneFoliadorPersona).Tables[0];



                if (dtFoliador.Rows.Count > 0)
                {



                    foreach (DataRow dr in dtFoliador.Rows)
                    {
                        folio = new PERSControlDeFolio();



                        folio.IdFolio = Convert.ToInt32(dr["FDFOIDFOLI"].ToString().Trim());
                        folio.Descripcion = dr["FSFODESCRI"].ToString().Trim();
                        folio.FolioActual = Convert.ToInt32(dr["FDFOULTFOL"].ToString().Trim());
                        folio.Estatus = dr["FBFOESTATU"].ToString().Trim();



                    }
                }




                PERSControlDeFolio control = folio;

                int lastIdPersona = folio.FolioActual;



                if (control != null)
                    max = control.FolioActual + 1;



                if (max <= lastIdPersona)
                    max = lastIdPersona + 1;



                if (control == null)
                {
                    control = new PERSControlDeFolio();
                    control.FolioActual = max;



                }
                else
                {
                    control.FolioActual = max;
                    // aDB.Update(aIdUser, aIdProgram, control);



                    string actualizaFoliador = "";



                    actualizaFoliador += "UPDATE PRODPERS.CTDFOLIO ";
                    actualizaFoliador += "SET FDFOULTFOL = " + max;
                    actualizaFoliador += ", USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' ";
                    actualizaFoliador += "WHERE FDFOIDFOLI = 1 ";
                    actualizaFoliador += "AND FBFOESTATU = 1";



                    dbCnx.SetQuery(actualizaFoliador);



                }




                #endregion

                insertaPersona += "select FDPEIDPERS from NEW TABLE ( ";

                insertaPersona += "INSERT INTO PRODPERS.CTEPERSO ( ";
                insertaPersona += "FDPEIDPERS, FSPECTEUNI, FIPEIDCIAU, ";
                insertaPersona += "FSPERFC, FDPEIDTIPO, FDPEIDTCTE,";
                insertaPersona += "FDPEIDCLAS, FFPEALTA, FHPEALTA, ";
                insertaPersona += "FFPEESTATU, FHPEESTATU, FDPEESTATU, ";
                insertaPersona += "USERCREAT, DATECREAT, TIMECREAT, PROGCREAT ";
                insertaPersona += ") VALUES ( ";
                insertaPersona += max + ", " + max + ", " + IdAgencia + " , ";
                insertaPersona += "'" + persona.RFC.ToString().ToUpper().Trim() + "'" + "," + 2 + "," + 1 + " ";
                insertaPersona += "," + 1 + "," + "CURRENT DATE" + "," + "CURRENT TIME" + ",";
                insertaPersona += "CURRENT DATE" + "," + "CURRENT TIME" + "," + 1;
                insertaPersona += "," + "'APP', CURRENT DATE, CURRENT TIME, 'APP'";
                insertaPersona += ")";

                insertaPersona += ")";


                DataTable dtPeM = dbCnx.GetDataSet(insertaPersona).Tables[0];

                if (dtPeM.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtPeM.Rows)
                    {
                        IdPersona = Convert.ToInt32(dr["FDPEIDPERS"].ToString().Trim());
                    }
                }
                else
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();

                    throw new Exception();
                }


                string insertaPersMoral = string.Empty;

                insertaPersMoral += "INSERT INTO PRODPERS.CTDPERMO (";
                insertaPersMoral += "FDPMIDPERS, FSPMRAZON, FICTIDRPLG,FICTIDGRCM,";
                insertaPersMoral += "FDPMESTATU, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT";
                insertaPersMoral += ") VALUES ( ";
                insertaPersMoral += IdPersona + "," + "'" + persona.RazonSocial.ToString().ToUpper().Trim() + "'" + "," + 0 + "," + 0 + ",";
                insertaPersMoral += "1, 'APP', CURRENT DATE, CURRENT TIME, 'APP'";
                insertaPersMoral += ")";

                dbCnx.SetQuery(insertaPersMoral);

                if (persona.NumeroCelular != "" && persona.NumeroCelular.Length == 10)
                {

                    string Lada = string.Empty;
                    string Telefono = string.Empty;
                    if (persona.NumeroCelular.StartsWith("55") || persona.NumeroCelular.StartsWith("81") || persona.NumeroCelular.StartsWith("33"))
                    {
                        Lada = persona.NumeroCelular.Substring(0, 2);
                        Telefono = persona.NumeroCelular.Substring(2, 8);
                    }
                    else
                    {
                        Lada = persona.NumeroCelular.Substring(0, 3);
                        Telefono = persona.NumeroCelular.Substring(3, 7);
                    }


                    string insertaTelefono = string.Empty;

                    insertaTelefono += "INSERT INTO PRODPERS.CTDRPTEL (";
                    insertaTelefono += "FDPTIDPERS, FDPTCONTEL, FDPTIDTELE,";
                    insertaTelefono += "FDPTPLADA, FDPTNUMTEL, FBCTDEFAUL,";
                    insertaTelefono += "FDPTESTATU, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT";
                    insertaTelefono += ") VALUES (";
                    insertaTelefono += IdPersona + "," + 1 + "," + 2 + ",";
                    insertaTelefono += Lada + "," + Telefono + "," + 1 + ",";
                    insertaTelefono += "1, 'APP', CURRENT DATE, CURRENT TIME, 'APP' ";
                    insertaTelefono += ")";

                    dbCnx.SetQuery(insertaTelefono);


                }

            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
                throw new Exception();
            }

            return IdPersona;

        }

    }

}
