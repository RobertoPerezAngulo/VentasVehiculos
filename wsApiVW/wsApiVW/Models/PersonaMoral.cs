using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class PersonaMoral
    {
        public string IdPersona { get; set; } //FDPMIDPERS,
        public string RazonSocial { get; set; } //FSPMRAZON,
        public string IdRepresentanteLegal { get; set; } //FICTIDRPLG,
        public string IdGiroComercial { get; set; } //FICTIDGRCM,
    }
}