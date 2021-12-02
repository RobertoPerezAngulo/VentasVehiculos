using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class AutorizacionDocumento
    {
        public string IdAgencia { get; set; }
        public string Folio { get; set; }
        public string Marca { get; set; }
        public string Estado { get; set; }
        public string Motivo { get; set; }
    }
}