using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.Agregar_Autos
{
    public class ModeloAuto
    {
        public string IdModelo { get; set; }
        public string NombreModelo { get; set; }
        public string Anio { get; set; }
        public string IdMarca { get; set; }
        public int Estatus { get; set; }
    }
}