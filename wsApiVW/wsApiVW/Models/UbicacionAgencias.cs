using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class UbicacionAgencias
    {
        public string IdMarca { get; set; }
        public string Nombre { get; set; }
        public List<Direccione> Direcciones { get; set; }
    }
    public partial class Direccione
    {
        public string IdAgencia { get; set; }
        public string Ubicacion { get; set; }
        public string Direccion { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string Icono { get; set; }
    }
}