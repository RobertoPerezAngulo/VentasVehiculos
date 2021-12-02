using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.AutoModels
{
    public class Inventario
    {
        public string IdAgencia { get; set; }
        public string IdInventario { get; set; }
        public string IdVehiculo { get; set; }
        public string IdVersion { get; set; }
        public string IdColor { get; set; }
        public string NumeroSerie { get; set; }
        public string Color { get; set; }
        public string NumeroInventario { get; set; }
        public string Ruta { get; set; }
    }

    public class Politicas
    {
        public string RutaTerminosyCondiciones { get; set; }
        public string RutaPrivacidad { get; set; }
        public string PoliticasAgencia { get; set; }
    }

    public class InventarioZona
    {
        public string IdZona { get; set; }
        public string NombreZona { get; set; }
        public List<AgenciasPorZona> Agencias { get; set; }

    }



    public class AgenciasPorZona
    {
        public string IdAgencia { get; set; }
        public string Nombre { get; set; }

    }


    public class Zona
    {

        public string IdZona { get; set; }
        public string NombreZona { get; set; }

    }
}