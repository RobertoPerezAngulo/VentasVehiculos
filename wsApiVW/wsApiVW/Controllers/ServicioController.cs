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
    public class ServicioController : ApiController
    {
        [Route("api/Servicio/PostEnviarCorreoContactoDeCliente", Name = "PostEnviarCorreoContactoDeCliente")]
        public async Task<Respuesta> PostEnviarCorreoContactoDeCliente([FromBody] EnviarCorreoContactoCliente CorreoContactoCliente)
        {
            ContactoCliente _cliente = new ContactoCliente();
            return await _cliente.EnviarCorreoContactoDeCliente(CorreoContactoCliente);
        }
    }
}