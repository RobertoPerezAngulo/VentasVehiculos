using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.Aplicaciones
{
    public class CatalogoApps
    {
        public string IdApps { get; set; }
        public string Apps { get; set; }
        public string Version { get; set; }
        public string FechaLiberacion { get; set; }
    }
}