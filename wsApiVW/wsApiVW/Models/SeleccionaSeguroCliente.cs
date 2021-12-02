using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class SeleccionaSeguroCliente
    {
        public string IdApps { get; set; }
        public string IdConsecutivo { get; set; }
        public string IdCompra { get; set; }
        public string IdCuenta { get; set; }
        public string Tipo { get; set; }
    }
}