using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using wsApiVW.Models.User;

namespace wsApiVW.Models.SmartIT
{
    public class OrdenCompraPedidoSmartIT
    {
        public string IdApps { get; set; }
        public string IdMarca { get; set; }
        public string IdCompra { get; set; }
        public string IdEstado { get; set; }
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
        public string RutaReferenciaBancaria { get; set; }
        public string IdProceso { get; set; }
        public Cuenta CuentaUsuario { get; set; }
        public List<Accesorio> AccesoriosOtros { get; set; }

    }

    public class OrdenCompraSmartIt : OrdenCompraPedidoSmartIT
    {
        public string IdPedido { get; set; }
        public string FechaCompra { get; set; }
        public string HoraCompra { get; set; }
    }
}