using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.Agregar_Autos
{
    public class VersionMxAuto
    {
        public string IdMarca { get; set; }
        public string IdVersion { get; set; }
        public string IdModelo { get; set; }
        public string Descripcion { get; set; }
        public int Estatus { get; set; }
    }
}