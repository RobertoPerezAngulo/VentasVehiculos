using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.SmartIT
{
    public class DatoCheckAgencia
    {
        public string IdAgencia { get; set; }
        public string IdCompra { get; set; } 
        public string IdCheck { get; set; } 
        public string DescripcionCheck { get; set; } 
        public string DescripcionEstado { get; set; } 
        public string IdEstado { get; set; }
    }
}