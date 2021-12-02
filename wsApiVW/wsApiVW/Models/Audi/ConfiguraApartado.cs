using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.Audi
{
    public class PostConfiguraApartado
    {
        public string IdTipoDescuento { get; set; }
        public string Valor { get; set; }
        public string IdTipoVehiculo { get; set; }
        public string IdUser { get; set; }
        public string IdApps { get; set; }
    }

    public class ConfiguraApartado
    {
        public string Id { get; set; }
        public string IdTipoDescuento { get; set; }
        public string Valor { get; set; }
        public string IdTipoVehiculo { get; set; }
        public string IdUser { get; set; }
        public string IdApps { get; set; }
    }

    public class RestConfiguraApartado : ConfiguraApartado
    {
        public string TipoDescuento { get; set; }
        public string TipoVehiculo { get; set; }
    }
}