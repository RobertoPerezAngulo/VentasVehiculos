using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using wsApiVW.Bussine;
using wsApiVW.Models;

namespace wsApiVW.Controllers
{
    public class MedioPagoController : ApiController
    {
        private AutosBussine _auto;
        public MedioPagoController()
        {
            _auto = new AutosBussine();
        }

        [Route("api/MedioPago/GetObtenerCuentasBancariasPorAgencia", Name = "GetObtenerCuentasBancariasPorAgencia")]
        public CuentasBancariasPorAgencia GetObtenerCuentasBancariasPorAgencia(int aIdAgencia)
        {
            return _auto.GetDatosBancario(aIdAgencia);
        }

        [Route("api/MedioPago/GetObtenerReferenciaDePago", Name = "GetObtenerReferenciaDePago")]
        public Respuesta GetObtenerReferenciaDePago(long aIdCompra, string aIdApps)
        {
            return _auto.GetObtenerReferenciaDePago(aIdCompra, aIdApps);
        }
    }
}