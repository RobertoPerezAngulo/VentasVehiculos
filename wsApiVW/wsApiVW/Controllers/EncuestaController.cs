using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Bussine;
using wsApiVW.Models;
using wsApiVW.Models.User;

namespace wsApiVW.Controllers
{
    public class EncuestaController : ApiController
    {
        private ProcesoCompra _compra;
        public EncuestaController()
        {
            _compra = new ProcesoCompra();
        }
        [Route("api/Encuesta/PostEncuesta", Name = "PostEncuesta")]
        public async Task<Respuesta> PostEncuesta(string aIdCompra,[FromBody]Encuesta Encuesta, string aIdApps)
        {
            return await _compra.PostEncuesta(aIdCompra, Encuesta, aIdApps);
        }

        [Route("api/Encuesta/GetEncuesta", Name = "GetEncuesta")]
        public EncuestaOrden GetEncuesta(long aIdCompra, string aIdApps)
        {
            return _compra.GetEncuesta(aIdCompra, aIdApps);
        }
    }
}