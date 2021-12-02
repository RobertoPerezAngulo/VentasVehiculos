using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.PagoElectronico
{
    public class RequestOpenPay
    {
        public string Cantidad { get; set; }

        public string Descripction { get; set; }

        public string OrderId { get; set; }

        public DatosCliente InformacionCliente { get; set; }
    }
}