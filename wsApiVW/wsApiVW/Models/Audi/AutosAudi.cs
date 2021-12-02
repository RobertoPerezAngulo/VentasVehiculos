using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.Audi
{
    public class AutosAudi
    {

        public partial class ModeloAudi
        {
            public string IdModelo { get; set; }
            public string NombreModelo { get; set; }
            public string TipoCarro { get; set; }
            public List<AnioAniosAudi> Anios { get; set; }
        }

        public partial class AnioAniosAudi
        {
            public string Anio { get; set; }
            public string Atributo1 { get; set; }
            public string Atributo2 { get; set; }
            public string Atributo3 { get; set; }
            public string Atributo4 { get; set; }
            public List<VersioneAudi> Versiones { get; set; }
            public List<ColoreAudi> GamaColores { get; set; }
            public string Atributo5 { get; set; }
            public string Atributo6 { get; set; }
        }

        public partial class ColoreAudi
        {
            public string IdColor { get; set; }
            public string Nombre { get; set; }
            public Uri Ruta { get; set; }
            public Uri RutaMini { get; set; }
        }

        public partial class VersioneAudi
        {
            public string Nombre { get; set; }
            public string RutaVideo { get; set; }
            public string RutaFichaTecnica { get; set; }
            public string Ruta360 { get; set; }
            public List<PrecioTransmisionAudi> PrecioTransmision { get; set; }
            public List<ColoreAudi> Colores { get; set; }
            public List<dynamic> Accesorios { get; set; }
        }

        public partial class PrecioTransmisionAudi
        {
            public string IdVersion { get; set; }
            public string Transmision { get; set; }
            public List<MotorAudi> Motor { get; set; }
        }

        public partial class MotorAudi
        {
            public string Nombre { get; set; }
            public string Precio { get; set; }
        }

    }
}