using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class CuentaActualizar
    {
        public string IdApps { get; set; }
        public string IdCuenta { get; set; }
        public string Nombre { get; set; } = "";
        public string ApellidoPaterno { get; set; } = "";
        public string ApellidoMaterno { get; set; } = "";
        public string Correo { get; set; } = "";
        public string LadaMovil { get; set; } = "";
        public string TelefonoMovil { get; set; } = "";
    }
}