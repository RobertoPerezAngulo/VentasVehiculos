using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Bussine;
using wsApiVW.Models;
using static wsApiVW.Models.Audi.AutosAudi;

namespace wsApiVW.Controllers
{
    public class AutosController : ApiController
    {
        private AutosBussine _bussine;
        public AutosController()
        {
            _bussine = new AutosBussine();
        }
           
        [Route("api/Autos/GetModelosXMarca", Name = "GetModelosXMarca")]
        public Vehiculos GetModelosXMarca(string aIdMarca)
        {
            return  _bussine.ModelosXMarca(aIdMarca);
        }

        [Route("api/Autos/GetMaquetasXMarca", Name = "GetMaquetasXMarca")]
        public Ruta360Maquetas GetMaquetasXMarca(string aIdMarca)
        {
            return _bussine.MaquetasXMarca(aIdMarca);
        }

        [Route("api/Autos/GetModelosXMarcaAudi", Name = "GetModelosXMarcaAudi")]
        public List<ModeloAudi> GetModelosXMarcaAudi()
        {
            return _bussine.ModelosXMarcaAudi();
        }

        [Route("api/Autos/Valorconfigurador", Name = "Valorconfigurador")]
        public string Valorconfigurador()
        {
            //return _bussine.TotalDescuentos("269711.50","40","4","43","7");
            return _bussine.Valorconfigurador("1","4", "269711.50");
        }

    }
}