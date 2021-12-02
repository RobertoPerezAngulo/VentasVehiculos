using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.User
{
    public class SegurosCliente
    {
        public string IdConsecutivo { get; set; }
        public string IdCompra { get; set; }
        public string IdCuenta { get; set; }
        public string Nombre { get; set; }
        public string Cobertura { get; set; }
        public string Cantidad { get; set; }
        public string Tipo { get; set; }
        public string NombreDocumento { get; set; }
        public string Ruta { get; set; }
        public string selecciono { get; set; } = "";
    }
}