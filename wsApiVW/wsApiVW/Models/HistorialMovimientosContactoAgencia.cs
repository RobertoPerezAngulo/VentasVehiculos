using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class HistorialMovimientosContactoAgencia
    {
        public string Agencia { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Paso { get; set; }
        public string Estado { get; set; }
        public string Mensaje { get; set; }
    }
}