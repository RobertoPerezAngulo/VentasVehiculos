using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.PagoElectronico
{
    public class OpenPayBad
    {
        public string Categoria { get; set; }
        public string description { get; set; }
        public string http_code { get; set; }
        public string error_code { get; set; }
        public string request_id { get; set; }
        public Fraude fraude { get; set; }
        public string IdPagoElectronico { get; set; }
        public string FechaCreacion { get; set; }
        public string HoraCreacion { get; set; }
    }
    public class Fraude
    {
        public string fraude { get; set; }
    }
}