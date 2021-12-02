using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Models;
using wsApiVW.Models.PagoElectronico;

namespace wsApiVW.Controllers
{
    public class PagoElectronicoController : ApiController
    {
        [Route("api/PagoElectronico/PostCargoOpenPay", Name = "PostCargoOpenPay")]
        public async Task<RespuestaTest<OpenPay>> PostCargoOpenPay([FromBody] VentaOpenPay  venta)
        {
            InternalPagoElectronicos _Pago = new InternalPagoElectronicos();
            return await _Pago.CargoOpenPay(venta);
        }
    }
}