using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.Agregar_Autos
{
    public class VersionAuto
    {
        public string Anio { get; set; }
        public string IdVersion { get; set; }
        public string NombreVersion { get; set; }
        public string IdModelo { get; set; }
        public string IdMarca { get; set; }
        public string PrecioContado { get; set; }
        public string PrecioLista { get; set; }
        public int Estatus { get; set; }
    }
}