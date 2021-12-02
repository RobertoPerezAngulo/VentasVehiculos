using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using wsApiVW.Models.User;

namespace wsApiVW.Models.AutoModels
{
    public class CompraAutoNuevo
    {
        public string  IdMarca { get; set; }
        public string Descripcion { get; set; }
        public string IdCompra { get; set; }
        public string IdPaso { get; set; }
        public string Folio { get; set; }
        public string IdCuenta { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Subtotal { get; set; }
        public string Iva { get; set; }
        public string Total { get; set; }
        public string IdEstado { get; set; }
        public string RutaReferenciaBancaria { get; set; }
        public PedidoAutoNuevo Pedido { get; set; }
        public Cuenta CuentaUsuario { get; set; }
        public List<AccesoriosUOtros> AccesoriosOtros { get; set; }
    }

    public class PedidoAutoNuevo
    {


        public string IdCompra { get; set; }
        public string IdAgencia { get; set; }

        public string IdPedido { get; set; }
        public string Iva { get; set; }
        public string Subtotal { get; set; }
        public string Total { get; set; }
        public string FechaPedido { get; set; }
        public string HoraPedido { get; set; }
        public string IdPersona { get; set; }
        public string IdVehiculo { get; set; }
        public string IdInventario { get; set; }
        public string Transmision { get; set; }
        public string Modelo { get; set; }
        public string Version { get; set; }
        public string ColorExterior { get; set; }
        public string NumeroInventario { get; set; }
        public string NumeroDeSerie { get; set; }
        public string CotizarSeguro { get; set; }
        public string RutaFoto { get; set; }

    }

    public class AccesoriosUOtros
    {
        public string IdAgencia { get; set; }
        public string IdPedido { get; set; }
        public string Id { get; set; }
        public string Concepto { get; set; }
        public string Subtotal { get; set; }
        public string Iva { get; set; }
        public string Total { get; set; }
        public string RutaFoto { get; set; }
    }
    public class Compra
    {


        public string IdCompra { get; set; }
        public string FolioCompra { get; set; }
        public string IdCuenta { get; set; }
        public string FechaCompra { get; set; }
        public string HoraCompra { get; set; }
        public string Subtotal { get; set; }
        public string Descuento { get; set; }
        public string IVA { get; set; }
        public string Total { get; set; }
        public string IdEstado { get; set; }
        public string DescripcionEstado { get; set; }

        public string IdProceso { get; set; }
        public string IdPaso { get; set; }

        public string RutaReferenciaBancaria { get; set; }
        public Cuenta CuentaUsuario { get; set; }
        public PedidoVehiculo Pedido { get; set; }

        public List<AccesorioDescuento> Accesorios { get; set; }

    }

    public class OrdenCompraPedido
    {
        public string IdApp { get; set; }
        public string IdMarca { get; set; }
        public string IdCompra { get; set; }
        public string Descripcion { get; set; }
        public string TotalPedido { get; set; }
        public string Total { get; set; }
        public string SubtotalPedido { get; set; }
        public string Subtotal { get; set; }
        public string Descuento { get; set; }
        public string IVAPedido { get; set; }
        public string IVA { get; set; }
        public string Transmision { get; set; }
        public string ColorExterior { get; set; }
        public string Version { get; set; }
        public string Modelo { get; set; }
        public string NumeroInventario { get; set; }
        public string RutaFoto { get; set; }
        public string IdAgencia { get; set; }
        public string IdInventario { get; set; }
        public string IdVehiculo { get; set; }
        public string NumeroDeSerie { get; set; }
        public string IdPaso { get; set; }
        public Cuenta CuentaUsuario { get; set; }
        public List<Accesorio> AccesoriosOtros { get; set; }
    }

    public class CompraVehiculo
    {
        public string IdApp { get; set; }
        public string IdMarca { get; set; }
        public string IdCompra { get; set; }
        public string Descripcion { get; set; }
        public string TotalPedido { get; set; }
        public string Total { get; set; }
        public string SubtotalPedido { get; set; }
        public string Subtotal { get; set; }
        public string Descuento { get; set; }
        public string IVAPedido { get; set; }
        public string IVA { get; set; }
        public string Transmision { get; set; }
        public string ColorExterior { get; set; }
        public string Version { get; set; }
        public string Modelo { get; set; }
        public string NumeroInventario { get; set; }
        public string RutaFoto { get; set; }
        public string IdAgencia { get; set; }
        public string IdInventario { get; set; }
        public string IdVehiculo { get; set; }
        public string NumeroDeSerie { get; set; }
        public string IdPaso { get; set; }
        public Cuenta CuentaUsuario { get; set; }
        public string TipoVehiculo { get; set; }
        public string TotalApartado { get; set; }
        public string TotalDescuentos { get; set; }
        public string PrecioInicial { get; set; }
        public string PrecioALiquidar { get; set; }
    }
}