using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public partial class Ruta360Maquetas
    {
        public string IdMarca { get; set; }
        public string Nombre { get; set; }
        public List<Maqueta> Maquetas { get; set; }
    }

    public partial class Maqueta
    {
        public string Version { get; set; }
        public string Nombre { get; set; }
        public string Anio { get; set; }
        public string NombreApp { get; set; }
        public string Ruta { get; set; }
    }

}