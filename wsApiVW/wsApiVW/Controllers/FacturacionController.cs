using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Bussine;
using wsApiVW.Models;
using wsApiVW.Models.Factura;

namespace wsApiVW.Controllers
{
    public class FacturacionController : ApiController
    {
        private FClienteAuto _factura;
        public FacturacionController()
        {
            _factura = new FClienteAuto();
        }

        [Route("api/Facturacion/GetOpcionesDeFormularios", Name = "GetOpcionesDeFormularios")]
        public async Task<Documento> GetOpcionesDeFormularios()
        {
            return await _factura.GetOpcionesDeFormularios();
        }

        [Route("api/Facturacion/PostRegistraDatosFiscales", Name = "PostRegistraDatosFiscales")]
        public async Task<Respuesta> PostRegistraDatosFiscales(DatosFiscales datosFiscales, int IdTipoPersona, string aIdApps)
        {
            return await _factura.RegistraDatosFiscales(datosFiscales, IdTipoPersona, aIdApps);
        }

        [Route("api/Facturacion/GetObtenerFactura", Name = "GetObtenerFactura")]
        public Respuesta GetObtenerFactura(long aIdCompra, string aIdApps)
        {
            return _factura.GetObtenerFactura(aIdCompra, aIdApps);
        }


        [Route("api/Facturacion/PostRegistraDatosFiscalesAudi", Name = "PostRegistraDatosFiscalesAudi")]
        public async Task<Respuesta> PostRegistraDatosFiscalesAudi(DatosFiscales datosFiscales, int IdTipoPersona, string aIdApps)
        {
            return await _factura.RegistraDatosFiscalesAudi(datosFiscales, IdTipoPersona, aIdApps);
        }
    }
}