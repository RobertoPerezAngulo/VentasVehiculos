using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using wsApiVW.Bussine;
using wsApiVW.Models;
using wsApiVW.Models.AutoModels;
using wsApiVW.Models.User;

namespace wsApiVW.Controllers
{
    public class OrdenCompraController : ApiController
    {
        
        [Route("api/OrdenCompra/GetEstadosOrdenCompra", Name = "GetObtenerEstadosOrdenCompra")]
        public List<EstadoOrdenCompra> GetObtenerEstadosOrdenCompra()
        {
            AutosBussine _bussine = new AutosBussine();
            return _bussine.GetObtenerEstadosOrdenCompra();
        }

        [Route("api/OrdenCompra/PostSeleccionaSeguro", Name = "PostSeleccionaSeguro")]
        public async Task<Respuesta> PostSeleccionaSeguro([FromBody]SeleccionaSeguroCliente seguro)
        {
            AutosBussine _bussine = new AutosBussine();
            return await _bussine.UpdateSeleccionaSeguro(seguro);
        }

        [Route("api/OrdenCompra/GetObtenerSeguros", Name = "GetObtenerSeguros")]
        public List<SegurosCliente> GetObtenerSeguros(long aIdCompra, long aIdCuenta, string aIdApps)
        {
            AutosBussine _bussine = new AutosBussine();
            return _bussine.GetObtenerSeguros(aIdCompra, aIdCuenta, aIdApps);
        }

        [Route("api/OrdenCompra/PutCancelaOrdenCompra", Name = "PutCancelaOrdenCompra")]
        public async Task<Respuesta> PutCancelaOrdenCompra(long aIdCompra, string aIdApps)
        {
            AutosBussine _bussine = new AutosBussine();
            return await _bussine.CancelaOrdenCompra(aIdCompra, aIdApps);
        }

        [Route("api/OrdenCompra/GetOrdenCompraXIdMarca", Name = "GetOrdenCompraXIdMarca")]
        public RespuestaTest<OrdenCompraPedido> GetOrdenCompraXIdMarca(long aIdCuenta, string aIdMarca , string aIdApps)
        {
            AutosBussine _bussine = new AutosBussine();
            return _bussine.GetOrdenCompraXIdMarca(aIdCuenta, aIdMarca, aIdApps);
        }

        [Route("api/OrdenCompra/GetCompraVehiculoXIdMarca", Name = "GetCompraVehiculoXIdMarca")]
        public RespuestaTest<CompraVehiculo> GetCompraVehiculoXIdMarca(long aIdCuenta, string aIdMarca, string aIdApps)
        {
            AutosBussine _bussine = new AutosBussine();
            return _bussine.GetCompraVehiculoXIdMarca(aIdCuenta, aIdMarca, aIdApps);
        }

        [Route("api/OrdenCompra/GetChecarEstatusCompra", Name = "GetChecarEstatusCompra")]
        public OrdenCompraPedido GetChecarEstatusCompra(long aIdCompra, string aIdApps)
        {
            AutosBussine _bussine = new AutosBussine();
            return _bussine.GetChecarEstatusCompra(aIdCompra, aIdApps);
        }

        [Route("api/OrdenCompra/GetCheckListOrden", Name = "GetCheckListOrden")]
        public List<Checklist> GetCheckListOrden(long aIdCompra, string aIdApps)
        {
            ProcesoCompra _compra = new ProcesoCompra();
            return _compra.CheckListOrden(aIdCompra, aIdApps);
        }

        [Route("api/OrdenCompra/GetActualizaCheckOrdenCompra", Name = "GetActualizaCheckOrdenCompra")]
        public Task<Respuesta> GetActualizaCheckOrdenCompra(long aIdCompra, int aIdCheck, string aIdApps)
        {
            ProcesoCompra _compra = new ProcesoCompra();
            return _compra.GetActualizaCheckOrdenCompra(aIdCompra, aIdCheck, aIdApps);
        }

        [Route("api/OrdenCompra/GetCitas", Name = "GetCitas")]
        public List<Citas> GetCitas(long aIdCompra, string aIdApps)
        {
            ProcesoCompra _compra = new ProcesoCompra();
            return _compra.GetCitas(aIdCompra, aIdApps);
        }

        [Route("api/OrdenCompra/GetFiltroDeCitas", Name = "GetFiltroDeCitas")]
        public HorariosCitas GetFiltroDeCitas(string TipoCita, string aIdAgencia, string aIdMarca)
        {
            ProcesoCompra _compra = new ProcesoCompra();
            return _compra.GetFiltroDeCitas(TipoCita, aIdAgencia, aIdMarca);
        }

        [Route("api/OrdenCompra/PostRegistraCitas", Name = "PostRegistraCitas")]
        public Respuesta PostRegistraCitas([FromBody]CitaAppGenerica cita)
        {
            ProcesoCompra _compra = new ProcesoCompra();
            return _compra.PostRegistraCitas(cita);
        }

        [Route("api/OrdenCompra/PutEstatusOrdenCompra", Name = "PutEstatusOrdenCompra")]
        public async Task<Respuesta> PutEstatusOrdenCompra(int aIdCompra, int PasoSiguiente, string aIdApps)
        {
            ProcesoCompra _compra = new ProcesoCompra();
            return await _compra.PutEstatusOrdenCompra(aIdCompra,PasoSiguiente, aIdApps);
        }

    }
}