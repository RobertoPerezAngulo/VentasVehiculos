using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.AutoModels
{
    public class PedidoVehiculo
    {
        public string IdCompra { get; set; }
        public string IdAgencia { get; set; }
        public string IdPedido { get; set; }
        public string FechaPedido { get; set; }
        public string HoraPedido { get; set; }
        public string IdPersona { get; set; }
        public string IdVehiculo { get; set; }
        public string IdInventario { get; set; }
        public string NumeroDeSerie { get; set; }
        public string Subtotal { get; set; }
        public string Descuento { get; set; }
        public string Iva { get; set; }
        public string Total { get; set; }
        public string CotizarSeguro { get; set; }
        public string Modelo { get; set; }
        public string Version { get; set; }
        public string Transmision { get; set; }
        public string ColorExterior { get; set; }
        public string NumeroInventario { get; set; }
        public string RutaFoto { get; set; }
    }
}