using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Bussine;
using wsApiVW.Models;
using wsApiVW.Models.SmartIT;

namespace wsApiVW.Controllers
{
    public class AlertasPushController : ApiController
    {
        private AlertasCompra _alerta;
        public AlertasPushController()
        {
            _alerta = new AlertasCompra();
        }

        [Route("api/AlertasPush/PostEnviaNotificacion", Name = "PostEnviaNotificacion")]
        public async Task<Respuesta> PostEnviaNotificacion([FromBody] NotificacionCompra NotificacionCompra)
        {
            return await _alerta.PostEnviaNotificacion(NotificacionCompra);
        }

        [Route("api/AlertasPush/GetEnviarConGlobo", Name = "GetEnviarConGlobo")]
        public string GetEnviarConGlobo(string token, string asunto, string mensaje, string aIdApps)
        {
            return _alerta.EnviarConGlobo(token, asunto, mensaje, aIdApps);
        }

        [Route("api/AlertasPush/DeleteNotificaciones", Name = "DeleteNotificaciones")]
        public Respuesta DeleteNotificaciones(string aIdCuenta, string aIdApps)
        {
            ContactoCliente _user = new ContactoCliente();
            return _user.DeleteNotificaciones(aIdCuenta, aIdApps);
        }

    }
}