using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class PERSControlDeFolio
    {
        public int IdFolio { get; set; }
        public string Descripcion { get; set; }
        public int FolioActual { get; set; }
        public string Estatus { get; set; }
    }
}