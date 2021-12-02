using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.SubirDocumentos
{
    public class SubirArchivoSmartIT
    {
        public string IdApps { get; set; }
        public string IdCompra { get; set; }
        public string IdConsecutivo { get; set; }
        public string IdCuenta { get; set; }
        public string NombreAseguradora { get; set; }
        public string Total { get; set; }
        public string Cobertura { get; set; }
        public string Tipo { get; set; }
        public string NombreDocumento { get; set; }
        public string Base64 { get; set; }
        public string ExtensionArch { get; set; }
    }
}