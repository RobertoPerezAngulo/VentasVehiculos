using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using wsApiVW.Bussine;
using wsApiVW.Models;
using wsApiVW.Models.Audi;
using wsApiVW.Models.AutoModels;

namespace wsApiVW.Controllers
{
    public class PedidoController : ApiController
    {
        private AutosBussine _bussine;
        public PedidoController()
        {
            _bussine = new AutosBussine();
        }

        [Route("api/Pedido/PutProgramaEspecialDocumentos", Name = "PutProgramaEspecialDocumentos")]
        public Respuesta PutProgramaEspecialDocumentos(string IdCompra, string IdCuenta, string IdApps, string IdProgramaEspecial, string IdEstado, string MotivoRechazo, string IdConsecutivo)
        {
            return _bussine.PutProgramaEspecialDocumentos(IdCompra,IdCuenta,IdApps,IdProgramaEspecial, IdEstado, MotivoRechazo, IdConsecutivo);
        }

        [Route("api/Pedido/GetConsultaEstadosProgramasEspeciales", Name = "GetConsultaEstadosProgramasEspeciales")]
        public List<EstadoProgramasEspeciales> GetConsultaEstadosProgramasEspeciales()
        {
            return _bussine.GetConsultaEstadosProgramasEspeciales();
        }

        [Route("api/Pedido/GetPorgramaEspecial", Name = "GetPorgramaEspecial")]
        public List<ProgramasEspeciales> GetPorgramaEspecial(string IdCuenta,string IdApps, string IdCompra)
        {
            return _bussine.GetPorgramaEspecial(IdCuenta,IdApps,IdCompra);
        }

        [Route("api/Pedido/PostSubirDocumentosProgramasEspeciales", Name = "PostSubirDocumentosProgramasEspeciales")]
        [HttpPost]
        public async Task<Respuesta> PostSubirDocumentosProgramasEspeciales(string IdProgramaEspecial, string IdCompra, string IdCuenta, string IdApps, string consecutivo)
        {
            Respuesta respuesta = new Respuesta();
            ApuntadorDeServicio _prod = new ApuntadorDeServicio();
            SQLTransaction _save = new SQLTransaction();
            string paths = string.Empty;
            string sql = string.Empty;
            try
            {
                var ctx = HttpContext.Current;
                var root = HostingEnvironment.MapPath("~" + "/ProgramasEspeciales/Documentos");
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

                    name =  consecutivo + "_" + IdProgramaEspecial + "_" + IdCompra + "_" + IdCuenta + "_" +IdApps + name.Substring(index);
                    var filePath = Path.Combine(root, name);
                    path = filePath;

                    if (!File.Exists(filePath))
                    {
                        File.Move(localFileName, filePath);
                        sql = $@"INSERT INTO PRODAPPS.APCCVSRT(
                                FIAPIDRTDC,
                                FIAPIDPRO, 
                                FIAPIDCOMP,
                                FIAPIDCUEN,
                                FIAPIDAPPS,
                                FSAPRUTDOC,
                                FIAPSTATUS,
                                USERCREAT,
                                DATECREAT,
                                TIMECREAT,
                                PROGCREAT
                            )VALUES(
                            (SELECT coalesce(MAX(FIAPIDRTDC),0)+1 ID FROM PRODAPPS.APCCVSRT),
                            {IdProgramaEspecial},
                            {IdCompra},
                            {IdCuenta},
                            {IdApps},
                            '{_prod.Respuestservicio() + "/ProgramasEspeciales/Documentos/" + name}',
                            1,
                            'APPS',
                            CURRENT DATE,
                            CURRENT TIME,
                            'APPS')";
                        if (!_save.SQLGuardaTabla(sql))
                            throw new Exception();
                        respuesta = new Respuesta() { Ok = "SI", Mensaje = "Se subió su documento de forma satisfactoria." };
                    }
                    else
                    {
                        File.Delete(filePath);
                        File.Move(localFileName, filePath);
                        sql = $@"UPDATE PRODAPPS.APCCVSRT SET 
                                    FSAPRUTDOC = '{_prod.Respuestservicio() + "/ProgramasEspeciales/Documentos/" + name}',
                                         USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE , TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP'
                                WHERE 
                                FIAPIDPRO = {IdProgramaEspecial} 
                                AND FIAPIDCOMP = {IdCompra} 
                                AND FIAPIDCUEN = {IdCuenta} 
                                AND FIAPIDAPPS= {IdApps}";
                        if (!_save.SQLGuardaTabla(sql))
                            throw new Exception();
                        respuesta = new Respuesta() { Ok = "SI", Mensaje = "Los documentos se remplazaron correctamente" };
                    }

                    sql = $@"UPDATE PRODAPPS.APCPRERC SET FIAPIDESTA = 3 
                                WHERE FIAPIDPRO = {IdProgramaEspecial} 
                                AND FIAPIDCOMP = {IdCompra} 
                                AND FIAPIDCUEN = {IdCuenta} 
                                AND FIAPIDAPPS= {IdApps}";
                    _save.SQLGuardaTabla(sql);

                }
            }
            catch (Exception e)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = e.Message + "ruta->:" + paths;
            }
            return respuesta;
        }

        [Route("api/Pedido/PostDeshacePasoRealizado", Name = "PostDeshacePasoRealizado")]
        public Respuesta PostDeshacePasoRealizado(long aIdCompra, int IdPaso, string aIdApps)
        {
            return _bussine.PostDeshacePasoRealizado(aIdCompra, IdPaso, aIdApps);
        }

        [Route("api/Pedido/GetObtenerInventarioXAgencia", Name = "GetObtenerInventarioXAgencia")]
        public List<Inventario> GetObtenerInventarioXAgencia(string aIdAgencia, string aIdVersion, string aIdMarca)
        {
            return _bussine.GetObtenerInventarioXAgencia(aIdAgencia,aIdVersion, aIdMarca);
        }

        [Route("api/Pedido/GetObtenerInventario", Name = "GetObtenerInventario")]
        public List<Inventario> GetObtenerInventario(string aIdMarca, string aIdVersion)
        {
            return _bussine.ObtenerInventario(aIdMarca, aIdVersion);
        }

        [Route("api/Pedido/GetObtenerInventarioAudi", Name = "GetObtenerInventarioAudi")]
        public List<InventarioAudi> GetObtenerInventarioAudi(string IdModelo)
        {
            return _bussine.GetObtenerInventarioAudi(IdModelo);
        }

        [Route("api/Pedido/PostRegistraApartado", Name = "PostRegistraApartado")]
        public async Task<RespuestaTest<OrdenCompraPedido>> PostRegistraApartado([FromBody] OrdenCompraPedido compra)
        {
            return await _bussine.RegistraApartado(compra);
        }

        [Route("api/Pedido/RegistraApartadoAudi", Name = "RegistraApartadoAudi")]
        public async Task<RespuestaTest<OrdenCompraPedido>> RegistraApartadoAudi([FromBody] OrdenCompraPedido compra)
        {
            return await _bussine.RegistraApartadoAudi(compra);
        }
        

    }
}