using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Models;
using wsApiVW.Models.User;
using System.Web.Hosting;
using System.Net.Http;
using System.IO;
using wsApiVW.Bussine;

namespace wsApiVW.Controllers
{
    public class UsuarioController : ApiController
    {
        private CuentasUsuarios _user;
        public UsuarioController()
        {
            _user = new CuentasUsuarios();
        }

        [Route("api/Usuario/PutToken", Name = "PutToken")]
        public Respuesta PutToken(string aIdCuenta, string aToken, string aIdApps)
        {
            return _user.PutToken(aIdCuenta, aToken, aIdApps);
        }

        [Route("api/Usuario/GetObtieneRutaFotoPerfil", Name = "GetObtieneRutaFotoPerfil")]
        public RespuestaTest<Cuenta> GetObtieneRutaFotoPerfil(long aIdCuenta, string aIdApps)
        {
            return _user.GetObtieneRutaFotoPerfil(aIdCuenta, aIdApps);
        }

        [Route("api/Usuario/PostActualizaDatosCuenta", Name = "PostActualizaDatosCuenta")]
        public async Task<Respuesta> PostActualizaDatosCuenta([FromBody]CuentaActualizar datosPerfil)
        {
            return await _user.PostActualizaDatosCuenta(datosPerfil);
        }

        [Route("api/Usuario/GetIniciarSesion", Name = "GetIniciarSesion")]
        public RespuestaTest<Cuenta> GetIniciarSesion(string Correo, string Clave, string aIdApps)
        {
            return _user.IniciarSesion(Correo, Clave, aIdApps);
        }

        [Route("api/Usuario/GetObtenerNotificacionesCuenta", Name = "GetObtenerNotificacionesCuenta")]
        public List<Notificacion> GetObtenerNotificacionesCuenta(string aIdCuenta, string aIdApps)
        {
            ContactoCliente _cliente = new ContactoCliente();
            return _cliente.ObtenerNotificacionesCuenta(aIdCuenta, aIdApps);
        }

        [Route("api/Usuario/PostRegistraCuenta", Name = "PostRegistraCuenta")]
        public RespuestaTest<Cuenta> PostRegistraCuenta([FromBody] RegistraCuenta cuentaJson)
        {
            return  _user.RegistraCuenta(cuentaJson);
        }

        [Route("api/Usuario/GetActualizarClave", Name = "GetActualizarClave")]
        public RespuestaTest<Cuenta> GetActualizarClave(string Correo, string aIdApp)
        {
            return _user.ActualizarClave(Correo, aIdApp);
        }

        [Route("api/Usuario/PostSubirFotoPerfil", Name = "PostSubirFotoPerfil")]
        [HttpPost]
        public async Task<Respuesta> PostSubirFotoPerfil(long aIdCuenta, string Foto, string aIdApps)
        {
            string paths = string.Empty;
            Respuesta respuesta = new Respuesta();
            try
            {
                #region Subir foto
                var ctx = HttpContext.Current;
                var root = HostingEnvironment.MapPath("~" + "/Cuentas/FotoPerfil");
                paths = root;
                var provider = new MultipartFormDataStreamProvider(root);
                string path = string.Empty;
                int index = 0;
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.FileData)
                {
                    var name = file.Headers
                        .ContentDisposition
                        .FileName;
                    name = name.Trim('"').ToLower();
                    index = name.IndexOf(".jpg");
                    if (index == -1) { index = name.IndexOf(".pdf"); }
                    if (index == -1) { index = name.IndexOf(".png"); }
                    if (index == -1) { index = name.IndexOf(".jpeg"); }
                    var localFileName = file.LocalFileName;
                    name = aIdCuenta + aIdApps + name.Substring(index);
                    var filePath = Path.Combine(root, name);
                    path = filePath;
                    if (!File.Exists(filePath))
                    {
                        File.Move(localFileName, filePath);
                        respuesta = await _user.PostSubirFoto(aIdCuenta, name, aIdApps);
                    }
                    else
                    {
                        File.Delete(filePath);
                        File.Move(localFileName, filePath);
                        respuesta = await _user.UpdateRegisterFotoPerfil(aIdCuenta, name, aIdApps);
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = e.Message + "ruta->:" + paths;
            }
            return respuesta;
        }

        [Route("api/Usuario/GetIniciarSesionRedesSociales", Name = "GetIniciarSesionRedesSociales")]
        public RespuestaTest<Cuenta> GetIniciarSesionRedesSociales(string Correo, string aIdApps)
        {
            return _user.IniciarSesionRedesSociales(Correo, aIdApps);
        }

        [Route("api/Usuario/PutActualizaNotificacionVisto", Name = "PutActualizaNotificacionVisto")]
        public RespuestaTest<Cuenta> PutActualizaNotificacionVisto(long aIdCuenta, int aIdNotificacion, string aIdApps)
        {
            return _user.PutActualizaNotificacionVisto(aIdCuenta, aIdNotificacion, aIdApps);
        }

        [Route("api/Usuario/GetNotificacionesCompra", Name = "GetNotificacionesCompra")]
        public List<Notificacion> GetNotificacionesCompra(string aIdCompra, string aIdApps)
        {
            return _user.GetNotificacionesCompra(aIdCompra, aIdApps);
        }
    }
}