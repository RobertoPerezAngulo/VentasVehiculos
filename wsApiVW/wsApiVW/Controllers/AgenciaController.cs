using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using wsApiVW.Bussine;
using wsApiVW.Models;
using wsApiVW.Models.Aplicaciones;
using wsApiVW.Models.AutoModels;

namespace wsApiVW.Controllers
{
    public class AgenciaController : ApiController
    {
        static string RutaUbicaciones = ConfigurationManager.AppSettings["RutaUbicaciones"];
        static string VideoNosotrosXIdApps = ConfigurationManager.AppSettings["VideoNosotrosXIdApps"];

        [Route("api/Agencia/GetObtenerUbicacionesAgencias", Name = "GetObtenerUbicacionesAgencias")]
        public List<Direccione> GetObtenerUbicacionesAgencias(string aIdMarca)
        {
            List<UbicacionAgencias> _ubicacionAgencias = new List<UbicacionAgencias>();
            UbicacionAgencias _ubicacion = new UbicacionAgencias();
            string strJSON = File.ReadAllText(RutaUbicaciones);
            _ubicacionAgencias = JsonConvert.DeserializeObject<List<UbicacionAgencias>>(strJSON);
            _ubicacion = _ubicacionAgencias.Where(x => x.IdMarca == aIdMarca).Count() == 0 ? new UbicacionAgencias() : _ubicacionAgencias.Where(x => x.IdMarca == aIdMarca).First();
            return _ubicacion.Direcciones == null ? new List<Direccione>() : _ubicacion.Direcciones;
        }

        [Route("api/Agencia/GetObtenerPoliticasXMarca", Name = "GetObtenerPoliticasXMarca")]
        public CatalogoPolizas GetObtenerPoliticasXMarca(string aIdMarca)
        {
            AutosBussine _bussine = new AutosBussine();
            return _bussine.RecuperaPoliticasXMarca(aIdMarca);
        }

        [Route("api/Agencia/GetVideoNosotrosXIdApps", Name = "GetVideoNosotrosXIdApps")]
        public VideoNosotrosXIdApps GetVideoNosotrosXIdApps(string aIdApps)
        {
            List<VideoNosotrosXIdApps> _videos = new List<VideoNosotrosXIdApps>();
            string strJSON = File.ReadAllText(VideoNosotrosXIdApps);
            _videos = JsonConvert.DeserializeObject<List<VideoNosotrosXIdApps>>(strJSON);
            return _videos.Where(x => x.IdApps == aIdApps).FirstOrDefault() == null ? new VideoNosotrosXIdApps() : _videos.Where(x => x.IdApps == aIdApps).First();
        }
    }
}