using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class SubirArchivo
    {
        public string IdCompra { get; set; } //IdCompra, 
        public string IdConsecutivo { get; set; }
        public string TipoDocumento { get; set; } //TipoDocumento, s
        public string NombreDocumento { get; set; } //Documento, 
        public string Base64 { get; set; } //base64            
        public string ExtensionArch { get; set; }
        public string IdApps { get; set; }
    }
}