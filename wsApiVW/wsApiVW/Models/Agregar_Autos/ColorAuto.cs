using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.Agregar_Autos
{
    public class ColorAuto
    {
        public string IdColor { get; set; }
        public string RutaMini { get; set; }
        public string Ruta { get; set; }
        public string Nombre { get; set; }
        public string IdVersion { get; set; }
        public int Estatus { get; set; }
    }
}