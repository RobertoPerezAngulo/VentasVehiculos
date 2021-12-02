using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.Aplicaciones
{ 
    public partial class CatalogoPolizas
    {
        public string IdMarca { get; set; }
        public string TerminoYCondiciones { get; set; }
        public string AvisoDePrivacidad { get; set; }
        public string LinkVideo { get; set; } = "";
        public string LinkPaginaOficial { get; set; } = "";
        public string LinkPaginaAppStore { get; set; } = "";
        public string LinkPaginaAppStoreAndroid { get; set; } = "";
        public List<PoliticasAgencia> PoliticasAgencias { get; set; }
    }

    public partial class PoliticasAgencia
    {
        public string IdAgencia { get; set; }
        public string NombreAgencia { get; set; }
        public string TerminoYCondiciones { get; set; }
        public string AvisoDePrivacidad { get; set; }
        public string Politicas { get; set; }
    }
}