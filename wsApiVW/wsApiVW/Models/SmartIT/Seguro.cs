using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.SmartIT
{
    public class Seguro
    {
        public string Seleccionado { get; set; }
        public string IdAseguradora { get; set; }
        public string Aseguradora { get; set; }
        public string Cobertura { get; set; }
        public string FechaInicial { get; set; }
        public string FechaFinal { get; set; }
        public string PrimaTotal { get; set; }
        public string Comision { get; set; }
        public string TotalSeguro { get; set; }
    }

    public class TipoSeguro
    {
        public string Id { get; set; }
        public string Tipo { get; set; }
    }
}