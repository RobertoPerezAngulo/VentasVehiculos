using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.User
{
    public class Cuenta
    {
        public string IdCuenta { get; set; }
        public string IdPersona { get; set; }
        public string Nombre { get; set; } = "";
        public string ApellidoPaterno { get; set; } = "";
        public string ApellidoMaterno { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Clave { get; set; } = "";
        public string Token { get; set; } = "";
        public string LadaMovil { get; set; } = "";
        public string TelefonoMovil { get; set; } = "";
    }

    public class RegistraCuenta
    {
        public string IdApp { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Correo { get; set; }
        public string Token { get; set; }
        public string LadaMovil { get; set; }
        public string TelefonoMovil { get; set; }
    }


}