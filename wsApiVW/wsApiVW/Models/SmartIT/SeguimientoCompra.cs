using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.SmartIT
{
    public class SeguimientoCompra
    {
        public string IdCompra { get; set; }
        public string IdSeguimiento { get; set; }
        public string TituloSeguimiento { get; set; }
        public string IdEstado { get; set; }
        public string DescripcionEstado { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
    }
}