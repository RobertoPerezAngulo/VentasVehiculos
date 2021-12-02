using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class Checklist
    {
        public string IdApps { get; set; }
        public string IdCompra { get; set; }
        public string IdProceso { get; set; }
        public string IdCheck { get; set; }
        public string DescripcionCheck { get; set; }
        public string SmartItControl { get; set; }
        public string AppControl { get; set; }
        public string SistemaContol { get; set; }
        public string Realizado { get; set; }
    }
}