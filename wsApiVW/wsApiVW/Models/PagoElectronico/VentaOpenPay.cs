using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.PagoElectronico
{
    public class VentaOpenPay
    {
        public string IdCompra { get; set; }
        public string IdApps { get; set; }
        public string  IdEmpresa { get; set; }
        public string IdToken { get; set; }
        public string DispositivoId { get; set; }
        public string IdCuenta { get; set; }
        public DatosCliente InformacionCliente { get; set; }
        public string Cantidad { get; set; }
        public string Descripcion { get; set; }
        public string TipoPago { get; set; }
    }

    public class DatosCliente
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
    }

    public class DatosTarjeta
    {
        public string NumeroTarjeta { get; set; }
        public string CVV { get; set; }
        public string NombreTarjeta { get; set; }
        public string AnioExpiracion { get; set; }
        public string MesExpiracion { get; set; }
    }
}