using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Models;
using wsApiVW.Models.User;
using wsApiVW.Services;
using static wsApiVW.Services.CorreoService;

namespace wsApiVW.Bussine
{
    public class ProcesoCompra
    {
        static string RutaUbicaciones = ConfigurationManager.AppSettings["RutaUbicaciones"];

        internal EncuestaOrden GetEncuesta(long aIdCompra, string aIdApps)
        {
            EncuestaOrden Encuesta = new EncuestaOrden();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            try
            {
                string Query = string.Empty;
                Query = "SELECT FIAPIDCOMP, FIAPIDENCU, FSAPENCUES ";
                Query += "FROM    " + constantes.Ambiente + "APPS.APDENCVW ";
                Query += "WHERE FIAPIDCOMP = " + aIdCompra + " AND FIAPSTATUS = 1" + " AND FIAPIDAPPS= " + aIdApps;
                DataTable DT = dbCnx.GetDataSet(Query).Tables[0];
                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow dr in DT.Rows)
                    {
                        Encuesta = new EncuestaOrden();
                        Encuesta.IdEncuesta = dr["FIAPIDENCU"].ToString().Trim();
                        Encuesta.Encuesta = dr["FSAPENCUES"].ToString().Trim();
                    }
                }
                else
                {
                    Encuesta = new EncuestaOrden();
                    Encuesta.IdEncuesta = "0";
                    Encuesta.Encuesta = "No se encontró encuesta para la compra seleccionada.";
                }
            }
            catch (Exception)
            {
                Encuesta = new EncuestaOrden();
                Encuesta.IdEncuesta = "0";
                Encuesta.Encuesta = "Ocurrió un error al buscar la encuesta.";
            }
            return Encuesta;
        }
        
        internal Task<Respuesta> PostEncuesta(string aIdCompra,[FromBody]Encuesta Encuesta, string aIdApps)
        {
            return Task.Run(() =>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new
                DVAConstants.Constants();
                Respuesta respuesta = new Respuesta();
                respuesta.Ok = "NO";
                respuesta.Mensaje = string.Empty;
                try
                {
                    dbCnx.AbrirConexion();
                    dbCnx.BeginTransaccion();
                    string JsonEncuesta = JsonConvert.SerializeObject(Encuesta);
                    string strInsCabComp = string.Empty;
                    strInsCabComp += "INSERT INTO " + constantes.Ambiente + "APPS.APDENCVW ";
                    strInsCabComp += "(FIAPIDCOMP, FIAPIDENCU, FSAPENCUES, FIAPIDESTA, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT,FIAPIDAPPS)";
                    strInsCabComp += "VALUES ";
                    strInsCabComp += "(";
                    strInsCabComp += aIdCompra + ", ";
                    strInsCabComp += "1, ";
                    strInsCabComp += "'" + JsonEncuesta + "'" + ", ";
                    strInsCabComp += "1" + ",";
                    strInsCabComp += "1, 'APPS' ,CURRENT DATE, CURRENT TIME, 'APPS'," + aIdApps;
                    strInsCabComp += ")";
                    dbCnx.SetQuery(strInsCabComp);
                    dbCnx.CommitTransaccion();
                    dbCnx.CerrarConexion();
                    respuesta.Objeto = JsonEncuesta;
                    respuesta.Ok = "SI";
                    respuesta.Mensaje = "Envio exitoso.";
                }
                catch (Exception)
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "Fallo el envio.";
                }
                return respuesta;
            });
        }
        internal Task<Respuesta> PutEstatusOrdenCompra(long aIdCompra, int PasoSiguiente, string aIdApps)
        {
            return Task.Run(() =>
            {
                string nuevoPaso = string.Empty;
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                Respuesta respuesta = new Respuesta();
                string strSqlSeg = string.Empty;
                string IdAgencia;
                try
                {
                    strSqlSeg = $@"SELECT B.FIAPIDCIAU FROM PRODAPPS.APECMPVW A
                                    INNER JOIN PRODapps.APEPANVW B
                                    ON A.FIAPIDCOMP = B.FIAPIDCOMP
                                    AND A.FIAPIDMARC = B.FIAPIDMARC
                                    AND A.FIAPIDAPPS = B.FIAPIDAPPS
                                    WHERE A.FIAPIDCOMP = {aIdCompra}
                                    AND A.FIAPIDAPPS = {aIdApps}";
                    IdAgencia = dbCnx.GetDataSet(strSqlSeg).Tables[0].Rows[0]["FIAPIDCIAU"].ToString();

                    if (string.IsNullOrEmpty(IdAgencia))
                        throw new Exception();

                    strSqlSeg = string.Empty;
                    strSqlSeg += "UPDATE " + constantes.Ambiente + "APPS.APECMPVW ";
                    strSqlSeg += "SET FIAPIDPASO = " + PasoSiguiente + ", ";
                    strSqlSeg += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' ";
                    strSqlSeg += "WHERE FIAPIDCOMP = " + aIdCompra + " AND FIAPIDAPPS = " + aIdApps + " ";
                    strSqlSeg += "AND FIAPSTATUS = 1";

                    dbCnx.AbrirConexion();
                    dbCnx.BeginTransaccion();
                    dbCnx.SetQuery(strSqlSeg);
                    dbCnx.CommitTransaccion();
                    dbCnx.CerrarConexion();
                    respuesta.Ok = "SI";
                    respuesta.Mensaje = "Se actualizó el proceso de manera satisfactoria";
                    respuesta.Objeto = null;
                }
                catch (Exception)
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "No se pudo actualizar el estado de la cuenta";
                    respuesta.Objeto = null;
                    return respuesta;
                }
                if (respuesta.Ok.Equals("SI"))
                {
                    switch (PasoSiguiente)
                    {
                        #region paso siguiente
                        case 1:
                            nuevoPaso = "<p> Se actualizo el paso a <strong>Apártalo</strong></p>";
                            enviaCorreo(aIdCompra, nuevoPaso,IdAgencia,aIdApps);
                            break;
                        case 2:
                            nuevoPaso = "<p>Se actualizo el paso a <strong>Carga tus datos</strong></p>";
                            enviaCorreo(aIdCompra, nuevoPaso,IdAgencia, aIdApps);
                            break;
                        case 3:
                            nuevoPaso = "<p>Se actualizo el paso a <strong>Liquida</strong></p>";
                            enviaCorreo(aIdCompra, nuevoPaso,IdAgencia, aIdApps);
                            break;
                        case 4:
                            nuevoPaso = "<p>Se actualizo el paso a <strong>Asegura</strong></p>";
                            enviaCorreo(aIdCompra, nuevoPaso,IdAgencia, aIdApps);
                            break;
                        case 5:
                            nuevoPaso = "<p>Se actualizo el paso a <strong>Emplaca</strong></p>";
                            enviaCorreo(aIdCompra, nuevoPaso,IdAgencia, aIdApps);
                            break;
                        case 6:
                            nuevoPaso = "<p>Se actualizo el paso a <strong>Programa tu entrega</strong></p>";
                            enviaCorreo(aIdCompra, nuevoPaso,IdAgencia, aIdApps);
                            break;
                        case 7:
                            nuevoPaso = "<p>Se actualizo el paso a <strong>Recibe tu auto</strong></p>";
                            enviaCorreo(aIdCompra, nuevoPaso,IdAgencia, aIdApps);
                            break;
                        case 8:
                            nuevoPaso = "<p>Se actualizo el paso a <strong>Factura disponible</strong></p>";
                            enviaCorreo(aIdCompra, nuevoPaso,IdAgencia, aIdApps);
                            break;
                        case 9:
                            nuevoPaso = "<p>Se actualizo el paso a <strong>Recibe tu factura</strong></p>";
                            enviaCorreo(aIdCompra, nuevoPaso,IdAgencia, aIdApps);
                            break;
                        default:
                            nuevoPaso = "";
                            break;
                            #endregion
                    }
                }
                return respuesta;
            });
        }
        internal Respuesta PostRegistraCitas(CitaAppGenerica cita)
        {
            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            CatalogoCitas citas = new CatalogoCitas();
            string Pasoproceso = string.Empty;

            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                string strCatCitas = string.Empty;
                strCatCitas =  @"SELECT FIAPIDTCIT, FSAPDESCIT FROM " +
                                "PRODAPPS.APCTPCVW  " +
                                "WHERE FIAPSTATUS = 1 AND FIAPIDTCIT=" + cita.IdTipoCita;

                DataTable dtCat = dbCnx.GetDataSet(strCatCitas).Tables[0];

                if (dtCat.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtCat.Rows)
                    {
                        citas = new CatalogoCitas();
                        citas.IdTipo = dr["FIAPIDTCIT"].ToString().Trim();
                        citas.Descripcion = dr["FSAPDESCIT"].ToString().Trim();
                    }
                }


                string strCita = string.Empty;
                strCita += "INSERT INTO PRODAPPS.APDDTCVW \t";
                strCita += "( ";
                strCita += "FIAPIDCONS, FIAPIDTCIT, FIAPIDTURN,";
                strCita += "FIAPIDCOMP, FIAPIDHORA, FIAPDESCCI,";
                strCita += "FFAPFECHA,  FHAPHORINI, FHAPHORFIN,";
                strCita += "FSAPUBICAC, FIAPSTATUS, USERCREAT,";
                strCita += "DATECREAT,TIMECREAT, PROGCREAT,FIAPIDAPPS)";
                strCita += "VALUES (";
                strCita += "(SELECT COALESCE(MAX(FIAPIDCONS),0) + 1 FROM prodapps.APDDTCVW )," + cita.IdTipoCita.ToString().Trim() + "," + cita.IdTurnoMatVes.ToString().Trim();
                strCita += "," + cita.IdCompra.ToString().Trim() + "," + "0" + "," + "'" + citas.Descripcion.ToString().Trim() + "'";
                strCita += "," + "'" + cita.Fecha + "'" + "," + "'" + cita.HoraInicial + "'" + "," + (string.IsNullOrEmpty(cita.HoraFinal) ? "Default" : "'" + cita.HoraFinal.ToString().Trim() + "'");
                strCita += "," + "'" + cita.Ubicacion.ToString().Trim().ToUpper() + "'" + "," + "1" + "," + "'APP'";
                strCita += "," + "CURRENT DATE, CURRENT TIME, 'APP', " + Convert.ToInt32(cita.IdApps)+ ")";

                dbCnx.SetQuery(strCita);

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Datos registrados exitosamente";
                respuesta.Objeto = null;
                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();
                

                if (respuesta.Ok.Equals("SI"))
                {
                    string compraCliente = string.Empty;
                    compraCliente += "SELECT FIAPIDCUEN, FIAPIDMARC FROM ";
                    compraCliente += "PRODAPPS.APECMPVW ";
                    compraCliente += "WHERE FIAPIDCOMP = " + cita.IdCompra.ToString().Trim() + " AND FIAPIDAPPS=" + cita.IdApps;
                    DataTable cuen = dbCnx.GetDataSet(compraCliente).Tables[0];
                    if (cuen.Rows.Count == 0)
                        throw new Exception();
                    string idCuenta = string.Empty;
                    string idMarca = string.Empty;
                    idCuenta = cuen.Rows[0]["FIAPIDCUEN"].ToString().Trim();
                    idMarca = cuen.Rows[0]["FIAPIDMARC"].ToString().Trim();

                    string strSqlCu = string.Empty;
                    strSqlCu = "SELECT  *  ";
                    strSqlCu += "FROM	PRODAPPS.APCCTAVW ";
                    strSqlCu += "WHERE FIAPSTATUS = 1 ";
                    strSqlCu += "AND FIAPIDCUEN = " + idCuenta + " AND FIAPIDAPPS=" + cita.IdApps;

                    DataTable dtCu = dbCnx.GetDataSet(strSqlCu).Tables[0];

                    string correoCliente = string.Empty;
                    if (dtCu.Rows.Count > 0)
                    {
                        correoCliente = dtCu.Rows[0]["FSAPCORREO"].ToString().Trim();
                    }
                    
                    if (!string.IsNullOrEmpty(correoCliente))
                    {
                        string strHtml = string.Empty;

                        try
                        {
                            HttpRequest request = HttpContext.Current.Request;
                            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";
                            string rutaTipoCita = string.Empty;
                            switch (Convert.ToInt32(cita.IdTipoCita))
                            {

                                case 1:
                                    rutaTipoCita = baseUrl + "Resources/Emailing/TestDrive/index.html";
                                    break;
                                case 2:
                                    rutaTipoCita = baseUrl + "Resources/Emailing/EntregaAuto/index.html";
                                    Pasoproceso = "<strong>PP: </strong> Programa entrega";
                                    break;
                                case 3:
                                    rutaTipoCita = baseUrl + "Resources/Emailing/EntregaFactura/index.html";
                                    Pasoproceso = "<strong>PP: </strong> Programa factura";
                                    break;
                                default:
                                    break;
                            }

                            Uri uri = new Uri(rutaTipoCita);
                            HttpWebRequest requestweb = (HttpWebRequest)HttpWebRequest.Create(uri);
                            requestweb.Method = WebRequestMethods.Http.Get;
                            HttpWebResponse response = (HttpWebResponse)requestweb.GetResponse();
                            StreamReader reader = new StreamReader(response.GetResponseStream());
                            string output = reader.ReadToEnd();
                            response.Close();
                            strHtml = output;

                            string dia = string.Empty;
                            string mes = string.Empty;
                            string anio = string.Empty;
                            string fecha = string.Empty;

                            mes = cita.Fecha.Substring(5, 2).ToString().Trim();
                            switch (mes)
                            {
                                case "01":
                                    mes = "enero";
                                    break;
                                case "02":
                                    mes = "febrero";
                                    break;
                                case "03":
                                    mes = "marzo";
                                    break;
                                case "04":
                                    mes = "abril";
                                    break;
                                case "05":
                                    mes = "mayo";
                                    break;
                                case "06":
                                    mes = "junio";
                                    break;
                                case "07":
                                    mes = "julio";
                                    break;
                                case "08":
                                    mes = "agosto";
                                    break;
                                case "09":
                                    mes = "septiembre";
                                    break;
                                case "10":
                                    mes = "octubre";
                                    break;
                                case "11":
                                    mes = "noviembre";
                                    break;
                                case "12":
                                    mes = "diciembre";
                                    break;
                            }

                            dia = cita.Fecha.Substring(8, 2).ToString().Trim();
                            anio = cita.Fecha.Substring(0, 4).ToString().Trim();
                            fecha = dia + " de " + mes + " de " + anio;
                            strHtml = strHtml.Replace("[{Ubicacion}]", cita.Ubicacion.ToString().Trim().ToUpper());
                            strHtml = strHtml.Replace("[{Fecha}]", fecha);
                            strHtml = strHtml.Replace("[{Hora}]", cita.HoraInicial.ToString().Trim());
                        }
                        catch (Exception ex)
                        {
                        }



                        string consulta = string.Empty;
                        int idAgencia;
                        string strSql = "select FIAPIDCIAU from prodapps.APEPANVW where FIAPIDCOMP = " + cita.IdCompra.ToString().Trim() + " AND FIAPIDAPPS=" + cita.IdApps;
                        DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
                        if(dt.Rows.Count == 0)
                            throw new Exception();
                        idAgencia = Convert.ToInt32(dt.Rows[0]["FIAPIDCIAU"].ToString().Trim());
                        string subjectCliente = "VW";
                        EnvioCorreoCliente hiloEnvioCorreoCliente = new EnvioCorreoCliente(subjectCliente, correoCliente, "Se agendó la cita " + citas.Descripcion + " con exito", strHtml, idAgencia, idMarca);
                        Thread hiloCliente = new Thread(new ThreadStart(hiloEnvioCorreoCliente.EnviarCorreoCliente));
                        hiloCliente.Start();
                    }

                }
            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo guardar la cita";
                respuesta.Objeto = null;
                return respuesta;
            }
            return respuesta;
        }

        internal HorariosCitas GetFiltroDeCitas(string TipoCita, string aIdAgencia, string aIdMarca)
        {
            #region VARIABLES
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            List<UbicacionAgencias> _ubicaciones = new List<UbicacionAgencias>();
            List<Direccione> ListaUbicaciones = new List<Direccione>();
            UbicacionAgencias aux = new UbicacionAgencias();
            List<CitasExistentes> Citas = new List<CitasExistentes>();
            List<Citas> CitasPorOrden = new List<Citas>();
            Citas Cita;
            List<Citas> CitasExistentes = new List<Citas>();
            TimeSpan HoraPasadas48Horas = new TimeSpan();
            DateTime FechaLimite = new DateTime();
            string FechaInicial = string.Empty;
            string FechaFinal = string.Empty;
            List<string> PeriodoDeFechas = new List<string>();
            List<string> Horarios = new List<string>();
            Rangos Horario = new Rangos();
            HorariosCitas Objeto = new HorariosCitas();
            List<Intervalos> ListaDeIntervalos = new List<Intervalos>();
            List<Intervalos> HorariosCitasSeguroFactura = new List<Intervalos>();
            List<Rangos> HorariosFinales = new List<Rangos>();
            List<Fechas> Fechas = new List<Fechas>();
            List<TimeSpan> Horas = new List<TimeSpan>();
            #endregion
            #region LLENA CITAS EXISTENTES
            DateTime hoy = DateTime.Today;
            DateTime x = hoy.AddHours(48);
            HoraPasadas48Horas = x.TimeOfDay;
            FechaLimite = x.AddDays(10).Date;

            FechaInicial = x.ToString("yyyy-MM-dd");
            FechaFinal = FechaLimite.ToString("yyyy-MM-dd");

            string strSQL = string.Empty;
            strSQL = "SELECT FIAPIDTCIT, FIAPIDCOMP, FIAPIDTURN, FIAPDESCCI, FFAPFECHA, FHAPHORINI, FHAPHORFIN, FSAPUBICAC " +
                     "FROM	" + constantes.Ambiente + "APPS.APDDTCVW " +
                     "WHERE FFAPFECHA >= " + "'" + FechaInicial + "'" + " AND FFAPFECHA <= " + "'" + FechaFinal + "'" + " AND FIAPSTATUS = 1";
            DataTable DT = dbCnx.GetDataSet(strSQL).Tables[0];

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    Cita = new Citas();
                    Cita.Descripcion = dr["FIAPDESCCI"].ToString().Trim();
                    Cita.Fecha = dr["FFAPFECHA"].ToString().Trim();
                    Cita.HoraFinal = dr["FHAPHORFIN"].ToString().Trim();
                    Cita.HoraInicial = dr["FHAPHORINI"].ToString().Trim();
                    Cita.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    Cita.IdTipo = dr["FIAPIDTCIT"].ToString().Trim();
                    Cita.IdTurno = dr["FIAPIDTURN"].ToString().Trim();
                    Cita.Ubicacion = dr["FSAPUBICAC"].ToString().Trim();
                    CitasExistentes.Add(Cita);
                }
            }
            else
            {
                Cita = new Citas();
                Cita.Descripcion = null;
                Cita.Fecha = null;
                Cita.HoraFinal = null;
                Cita.HoraInicial = null;
                Cita.IdCompra = null;
                Cita.IdTipo = null;
                Cita.Ubicacion = null;
                Cita.IdTurno = null;
                CitasExistentes.Add(Cita);
            }


            foreach (Citas cita in CitasExistentes)
            {

                string HoraInicia = cita.HoraInicial;
                string HoraFinal = cita.HoraFinal;

                if (HoraInicia != null)
                {
                    if (!string.IsNullOrEmpty(HoraInicia))
                        HoraInicia = HoraInicia.Substring(10, HoraInicia.Length - 19);

                }
                if (!string.IsNullOrEmpty(HoraFinal))
                {
                    if (!string.IsNullOrEmpty(HoraFinal))
                        HoraFinal = HoraFinal.Substring(10, HoraFinal.Length - 19);
                }


                string IntervaloCadena = string.Empty;
                if (HoraFinal != null)
                {
                    if (!string.IsNullOrEmpty(HoraFinal))
                    {
                        IntervaloCadena = HoraInicia + "-" + HoraFinal;

                    }
                    else
                    {
                        DateTime xs = Convert.ToDateTime(cita.HoraInicial);
                        IntervaloCadena = xs.ToString("HH:ss");
                    }

                }

                CitasExistentes nc = new CitasExistentes();

                if (cita.Fecha != null)
                {
                    nc.Fecha = cita.Fecha.Remove(cita.Fecha.Length - 14);
                    nc.Intervalo = IntervaloCadena;
                    nc.Tipo = cita.IdTipo;
                    nc.Turno = cita.IdTurno;
                }


                Citas.Add(nc);
            }

            #endregion
            #region LLENA HORARIOS
            while (x <= FechaLimite)
            {
                Fechas fecha = new Fechas();
                string Fecha = x.ToString("dd/MM/yyyy");

                fecha.Fecha = Fecha;
                fecha.Dia = x.ToString("dddd", new CultureInfo("es-ES")).ToUpper();
                fecha.Horarios = null;
                fecha.Intervalos = new List<Intervalos>();
                Fechas.Add(fecha);

                x = x.AddDays(1);
            }


            strSQL = string.Empty;
            strSQL = @"SELECT FIAPDESCDI ,FIAPIDTURN , FHAPHORINI, FHAPHORFIN " + 
                     "FROM	" + constantes.Ambiente + "APPS.APCHORVW " +
                     "WHERE FIAPSTATUS = 1";
            DataTable DTRangos = dbCnx.GetDataSet(strSQL).Tables[0];

            if (DTRangos.Rows.Count > 0)
            {
                foreach (DataRow dr in DTRangos.Rows)
                {
                    DateTime HoraInicial = Convert.ToDateTime(dr["FHAPHORINI"].ToString().Trim());
                    DateTime HoraFinald = Convert.ToDateTime(dr["FHAPHORFIN"].ToString().Trim());
                    string HoraFinal = HoraFinald.ToString("HH:mm:ss");
                    string HoraInicia = HoraInicial.ToString("HH:mm:ss");
                    string IntervaloCadena = HoraInicia + "-" + HoraFinal;


                    Intervalos Intervalo = new Intervalos();
                    Intervalo.Dia = dr["FIAPDESCDI"].ToString().Trim();
                    Intervalo.IdTurno = dr["FIAPIDTURN"].ToString().Trim();
                    Intervalo.Intervalo = IntervaloCadena;
                    ListaDeIntervalos.Add(Intervalo);
                }

            }
            else
            {
                ListaDeIntervalos = new List<Intervalos>();
            }
            #endregion
            #region Obtiene Horarios
            HorariosCitasSeguroFactura = ListaDeIntervalos.FindAll(o => o.IdTurno == "0");
            #endregion
            #region Realiza filtro Horarios 
            foreach (Intervalos inter in ListaDeIntervalos)
            {
                foreach (Fechas f in Fechas)
                {
                    if (f.Dia == inter.Dia)
                    {
                        f.Intervalos.Add(inter);
                    }
                }

            }


            foreach (Fechas F in Fechas)
            {
                List<Intervalos> IntervaloFinal = new List<Intervalos>();

                foreach (Intervalos i in F.Intervalos)
                {
                    CitasExistentes ce = new CitasExistentes();
                    if (Citas[0].Fecha != null)
                    {
                        ce = Citas.Find(o => o.Fecha.Trim() == F.Fecha);
                    }

                    if (ce != null)
                    {
                        List<Intervalos> NoAgendados = new List<Intervalos>();
                        NoAgendados = F.Intervalos.FindAll(O => O.IdTurno != ce.Turno && O.IdTurno != "0");
                        List<int> aIds = new List<int>();
                        F.Intervalos = NoAgendados;
                        break;

                    }

                }
            }


            #endregion
            #region LLENA UBICACIONES
            string strJSON = File.ReadAllText(RutaUbicaciones);
            _ubicaciones = JsonConvert.DeserializeObject<List<UbicacionAgencias>>(strJSON);
            aux = _ubicaciones.Where(o => o.IdMarca == aIdMarca).First();
            ListaUbicaciones = aux.Direcciones.Where(o => o.IdAgencia == aIdAgencia).ToList();
            #endregion
            #region LLENA OBJETO
            Objeto.TipoDeCita = TipoCita;
            Objeto.Ubicaciones = ListaUbicaciones;
            Objeto.Fechas = Fechas;
            #region Realiza filtro de tipo de cita 
            if (TipoCita != "1")
            {
                #region Llena Horarios 
                foreach (Fechas r in Objeto.Fechas)
                {
                    DateTime Fecha = Convert.ToDateTime(r.Fecha);

                    if (r.Dia == "SÁBADO")
                    {
                        string HoraInicial = "09:00";
                        string HoraFinal = "14:00";

                        DateTime Inicio = Convert.ToDateTime(HoraInicial);
                        DateTime Final = Convert.ToDateTime(HoraFinal);
                        List<DateTime> HorasSabado = new List<DateTime>();
                        List<Horarios> HorariosSabado = new List<Horarios>();

                        while (Inicio <= Final)
                        {
                            HorasSabado.Add(Inicio);
                            Inicio = Inicio.AddHours(1);
                        }

                        foreach (DateTime w in HorasSabado)
                        {
                            Horarios hr = new Horarios();
                            string DiaName = Fecha.ToString("dddd", new CultureInfo("es-ES"));
                            string Hora = w.ToString("HH:mm:ss");

                            hr.Dia = DiaName.ToUpper();
                            hr.Hora = Hora;
                            HorariosSabado.Add(hr);
                        }

                        r.Horarios = HorariosSabado;
                    }
                    else
                    {

                        string HoraInicial = "09:00";
                        string HoraFinal = "18:00";

                        DateTime Inicio = Convert.ToDateTime(HoraInicial);
                        DateTime Final = Convert.ToDateTime(HoraFinal);
                        List<DateTime> HorasSemana = new List<DateTime>();
                        List<Horarios> HorariosSemana = new List<Horarios>();
                        //List<Intervalos> _Intervalos = new List<Intervalos>();

                        while (Inicio <= Final)
                        {
                            HorasSemana.Add(Inicio);
                            Inicio = Inicio.AddHours(1);
                        }

                        foreach (DateTime w in HorasSemana)
                        {
                            Horarios hr = new Horarios();
                            string DiaName = Fecha.ToString("dddd", new CultureInfo("es-ES"));
                            string Hora = w.ToString("HH:mm:ss");

                            hr.Dia = DiaName.ToUpper();
                            hr.Hora = Hora;
                            HorariosSemana.Add(hr);
                        }
                        //_Intervalos = HorariosSemana.ConvertAll(h => new Intervalos() { Dia = h.Dia, Intervalo = h.Hora+ "-" + h.Hora }   );
                        //r.Intervalos = _Intervalos;
                        r.Horarios = HorariosSemana;

                    }
                }

                #endregion

                //foreach (Fechas Fech in Objeto.Fechas)
                //{
                //    Fech.Intervalos = null;
                //}

            }
            else
            {
                #region Elimina intervalos para el caso de citas tipo 2 o 3
                foreach (Fechas F in Fechas)
                {
                    foreach (Intervalos I in F.Intervalos)
                    {
                        F.Intervalos = F.Intervalos.FindAll(O => O.IdTurno != "0");
                    }
                }
                #endregion

            }
            #endregion

            #region Elimina Horarios no disponibles
            if (TipoCita != "1")
            {
                Objeto.Fechas = Objeto.Fechas.FindAll(o => o.Dia != "DOMINGO");


                foreach (CitasExistentes C in Citas)
                {

                    if (C.Fecha != null)
                    {
                        string Fecha = C.Fecha.Trim();
                        string Hora = C.Intervalo.Trim();
                        List<Horarios> HorariosDisponibles = new List<Horarios>();

                        foreach (Fechas fecha in Objeto.Fechas)
                        {
                            HorariosDisponibles = new List<Horarios>();
                            if (fecha.Fecha == Fecha)
                            {
                                foreach (Horarios hora in fecha.Horarios)
                                {
                                    string Hora2 = hora.Hora.Substring(0, hora.Hora.Length - 3);

                                    if (Hora2 != Hora)
                                    {
                                        HorariosDisponibles.Add(hora);
                                    }

                                }
                            }
                            else
                            {
                                HorariosDisponibles = fecha.Horarios;
                            }
                            fecha.Intervalos = HorariosDisponibles.ConvertAll(h => new Intervalos() { Dia = h.Dia, Intervalo = h.Hora + "-" + h.Hora ,IdTurno = ""});
                            fecha.Horarios = HorariosDisponibles;
                        }
                    }
                }
            }

            #endregion

            #endregion
            return Objeto;
        }

        
        internal List<Citas> GetCitas(long aIdCompra, string aIApps)
        {
            List<Citas> _Coleccioncitas = new List<Citas>();
            Citas _cita = new Citas();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string strSQL = string.Empty;
            strSQL = @"SELECT FIAPIDTCIT, FIAPIDCOMP, FIAPDESCCI, FFAPFECHA, FHAPHORINI, FHAPHORFIN, FSAPUBICAC " +
                      "FROM " + constantes.Ambiente + "APPS.APDDTCVW " +
                      "WHERE FIAPIDCOMP = " + aIdCompra + " AND FIAPSTATUS = 1" + " AND FIAPIDAPPS= " + aIApps;
            DataTable DT = dbCnx.GetDataSet(strSQL).Tables[0];
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    _cita = new Citas();
                    _cita.Descripcion = dr["FIAPDESCCI"].ToString().Trim();
                    _cita.Fecha = Convert.ToDateTime(dr["FFAPFECHA"]).ToString("dd/MM/yyyy");
                    _cita.HoraFinal = dr["FHAPHORFIN"].ToString() != "" ? Convert.ToDateTime(dr["FHAPHORFIN"]).ToString("hh:mm tt") : dr["FHAPHORFIN"].ToString();
                    _cita.HoraInicial = dr["FHAPHORINI"].ToString() != "" ? Convert.ToDateTime(dr["FHAPHORINI"]).ToString("hh:mm tt") : dr["FHAPHORINI"].ToString();
                    _cita.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    _cita.IdTipo = dr["FIAPIDTCIT"].ToString().Trim();
                    _cita.Ubicacion = dr["FSAPUBICAC"].ToString().Trim();
                    _Coleccioncitas.Add(_cita);
                }
            }
            else
            {
                _Coleccioncitas = new List<Citas>();
            }
            return _Coleccioncitas;
        }

        internal List<Checklist> CheckListOrden(long aIdCompra, string aIdApps)
        {
            List<Checklist> CheckList = new List<Checklist>();
            Checklist Check;
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string Query = string.Empty;
            Query = "SELECT FIAPIDCOMP,FIAPIDAPPS, FIAPIDPROC, FIAPIDPCKL, FSAPDESCCK, FIAPSMARTI, FIAPAPPVIS, FIAPSISTEM, FIAPREALIZ ";
            Query += "FROM    " + constantes.Ambiente + "APPS.APDCKLVW ";
            Query += "WHERE FIAPIDCOMP = " + aIdCompra + " AND FIAPSTATUS = 1 " + "AND FIAPIDAPPS =" + aIdApps;

            DataTable DT = dbCnx.GetDataSet(Query).Tables[0];
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    Check = new Checklist();
                    Check.IdApps = dr["FIAPIDAPPS"].ToString().Trim();
                    Check.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    Check.IdCheck = dr["FIAPIDPCKL"].ToString().Trim();
                    Check.IdProceso = dr["FIAPIDPROC"].ToString().Trim();
                    Check.DescripcionCheck = dr["FSAPDESCCK"].ToString().Trim();
                    Check.SmartItControl = dr["FIAPSMARTI"].ToString().Trim();
                    Check.AppControl = dr["FIAPAPPVIS"].ToString().Trim();
                    Check.SistemaContol = dr["FIAPSISTEM"].ToString().Trim();
                    Check.Realizado = dr["FIAPREALIZ"].ToString().Trim();
                    CheckList.Add(Check);
                }
                CheckList = CheckList.OrderBy(o => Convert.ToInt32(o.IdCheck)).ToList();
            }
            else
            {
                CheckList = new List<Checklist>();
            }
            return CheckList;
        }


        public void enviaCorreo(long IdCompra, string nuevoPaso,string IdAgencia, string IdApp)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            string SqlCorreos = string.Empty;
            List<string> ob = new List<string>();

            SqlCorreos = $@"SELECT FSAPCORREO FROM PRODAPPS.APDCRSVW
                           WHERE 1=1
                           AND FIAPIDTIPO = 2
                           AND FIAPIDAPPS = {IdApp}
                           AND FIAPIDCIAU=" + IdAgencia;

            DataTable _correo = dbCnx.GetDataSet(SqlCorreos).Tables[0];
            foreach (DataRow item in _correo.Rows)
            {
                ob.Add(item["FSAPCORREO"].ToString().Trim());
            }
            string subject = "APP";
            HiloEnvioCorreoSoporte hiloEnvioCorreo = new HiloEnvioCorreoSoporte(subject, ob, nuevoPaso, IdCompra.ToString());
            hiloEnvioCorreo.EnvioCorreoSoporte();
        }

        internal async Task<Respuesta> GetActualizaCheckOrdenCompra(long IdCompra, int IdCheck, string aIdApps)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            BPedido _updateEstado = new BPedido();
            Respuesta respuesta = new Respuesta();
            string Sql = string.Empty;
            string idAgencia = string.Empty;
            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                string strSqlSeg = string.Empty;
                strSqlSeg += "UPDATE " + constantes.Ambiente + "APPS.APDCKLVW ";
                strSqlSeg += "SET FIAPREALIZ = 1" + ", ";
                strSqlSeg += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' ";
                strSqlSeg += "WHERE FIAPIDCOMP = " + IdCompra + " AND FIAPIDAPPS= " + aIdApps + " ";
                strSqlSeg += "AND FIAPIDPCKL = " + IdCheck + " ";
                strSqlSeg += "AND FIAPSTATUS = 1";
                dbCnx.SetQuery(strSqlSeg);
                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se actualizó el proceso de manera satisfactoria";
                respuesta.Objeto = null;

                #region Actualiza estado
                switch (IdCheck)
                {
                    case 2:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 2, aIdApps);
                        break;
                    case 3:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 3, aIdApps);
                        break;
                    case 4:
                        await PutEstatusOrdenCompra(IdCompra, 2, aIdApps);
                        break;
                    case 5:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 4, aIdApps);
                        break;
                    case 6:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 5, aIdApps);
                        break;
                    case 7:
                        await PutEstatusOrdenCompra(IdCompra, 3, aIdApps);
                        break;
                    case 8:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 6, aIdApps);
                        break;
                    case 9:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 7, aIdApps);
                        break;
                    case 10:
                        await PutEstatusOrdenCompra(IdCompra, 4, aIdApps);
                        break;
                    case 11:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 8, aIdApps);
                        break;
                    case 12:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 9, aIdApps);
                        break;
                    case 13:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 10, aIdApps);
                        break;
                    case 15:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 11, aIdApps);
                        break;
                    case 16:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 12, aIdApps);
                        break;
                    case 17:
                        await PutEstatusOrdenCompra(IdCompra, 5, aIdApps);
                        break;
                    case 18:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 13, aIdApps);
                        break;
                    case 19:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 14, aIdApps);
                        break;
                    case 20:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 16, aIdApps);
                        break;
                    case 21:
                        await PutEstatusOrdenCompra(IdCompra, 6, aIdApps);
                        break;
                    case 23:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 17, aIdApps);
                        break;
                    case 24:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 18, aIdApps);
                        break;
                    case 25:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 19, aIdApps);
                        break;
                    case 27:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 20, aIdApps);
                        break;
                    case 29:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 21, aIdApps);
                        break;
                    case 28:
                        await PutEstatusOrdenCompra(IdCompra, 7, aIdApps);
                        break;
                    case 30:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 22, aIdApps);
                        break;
                    case 31:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 23, aIdApps);
                        break;
                    case 32:
                        await PutEstatusOrdenCompra(IdCompra, 8, aIdApps);
                        break;
                    case 33:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 24, aIdApps);
                        break;
                    default:
                        break;
                }
                #endregion

            }
            catch (Exception)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo actualizar el proceso";
                respuesta.Objeto = null;
                return respuesta;
            }

            if (respuesta.Ok.Equals("SI"))
            {
                Sql = $@"SELECT B.FIAPIDCIAU FROM PRODAPPS.APECMPVW A
                            INNER JOIN PRODapps.APEPANVW B
                                ON A.FIAPIDCOMP = B.FIAPIDCOMP
                                AND A.FIAPIDMARC = B.FIAPIDMARC
                                AND A.FIAPIDAPPS = B.FIAPIDAPPS
                            WHERE A.FIAPIDCOMP = {IdCompra}
                            AND A.FIAPIDAPPS =  {aIdApps}";

                idAgencia = dbCnx.GetDataSet(Sql).Tables[0].Rows[0]["FIAPIDCIAU"].ToString();
                string nuevoPaso = string.Empty;
                switch (IdCheck)
                {
                    #region check
                    case 2:
                        nuevoPaso = "<p><strong>EC: </strong> Carga del apartado</p><p><strong>PP: </strong>Apártalo</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 3:
                        nuevoPaso = "<p><strong>EC: </strong> Apartado validado</p><p><strong>PP: </strong>Apártalo</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 5:
                        nuevoPaso = "<p><strong>EC: </strong> Pedido</p><p><strong>PP: </strong>Carga tus datos</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 6:
                        nuevoPaso = "<p><strong>EC: </strong> Factura generada</p><p><strong>PP: </strong>Carga tus datos</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 8:
                        nuevoPaso = "<p><strong>EC: </strong> Carga de liquidación</p><p><strong>PP: </strong>Liquida</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 9:
                        nuevoPaso = "<p><strong>EC: </strong> Total validado</p><p><strong>PP: </strong>Liquida</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 11:
                        nuevoPaso = "<p><strong>EC: </strong> Carga de póliza</p><p><strong>PP: </strong>Asegura</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 12:
                        nuevoPaso = "<p><strong>EC: </strong> Solicitud de póliza</p><p><strong>PP: </strong>Asegura</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 13:
                        nuevoPaso = "<p><strong>EC: </strong> Cotización de póliza</p><p><strong>PP: </strong>Asegura</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 15:
                        nuevoPaso = "<p><strong>EC: </strong> Pago de póliza</p><p><strong>PP: </strong>Asegura</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 16:
                        nuevoPaso = "<p><strong>EC: </strong> Póliza emitida</p><p><strong>PP: </strong>Asegura</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 18:
                        nuevoPaso = "<p><strong>EC: </strong> Placas si</p><p><strong>PP: </strong>Emplaca</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 19:
                        nuevoPaso = "<p><strong>EC: </strong> Placas no</p><p><strong>PP: </strong>Emplaca</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 20:
                        nuevoPaso = "<p><strong>EC: </strong> Llegada</p><p><strong>PP: </strong>Programa entrega</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 23:
                        nuevoPaso = "<p><strong>EC: </strong> Cita agendada</p><p><strong>PP: </strong>Programa entrega</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 24:
                        nuevoPaso = "<p><strong>EC: </strong> Cita fuera de zona</p><p><strong>PP: </strong>Programa entrega</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 25:
                        nuevoPaso = "<p><strong>EC: </strong> Auto listo para entrega</p><p><strong>PP: </strong>Programa entrega</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 27:
                        nuevoPaso = "<p><strong>EC: </strong> Auto recibido</p><p><strong>PP: </strong>Recibe tu auto</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 29:
                        nuevoPaso = "<p><strong>EC: </strong> Cita factura agendada</p><p><strong>PP: </strong>Programa factura</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 30:
                        nuevoPaso = "<p><strong>EC: </strong> Factura fuera de zona</p><p><strong>PP: </strong>Programa factura</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 31:
                        nuevoPaso = "<p><strong>EC: </strong> Factura lista para entrega</p><p><strong>PP: </strong>Programa factura</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 33:
                        nuevoPaso = "<p><strong>EC: </strong> Factura recibida</p><p><strong>PP: </strong>Programa factura</p>";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    case 34:
                        nuevoPaso = "Prueba solicitada.";
                        enviaCorreo(IdCompra, nuevoPaso, idAgencia, aIdApps);
                        break;
                    default:
                        nuevoPaso = "";
                        break;
                        #endregion
                }
            }
            return respuesta;
        }
    }
}