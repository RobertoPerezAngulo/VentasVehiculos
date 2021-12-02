using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace wsApiVW.Models.SmartIT
{
    public class ResponsePromo
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFinal { get; set; }
        public string Ruta { get; set; }
        public string IdTipoDescuento { get; set; }
        public string Valor { get; set; }
        public string Status { get; set; }
        public string Descripcion { get; set; }
        public string IdApps { get; set; }
    }

    public class Subirprograma
    {
        public string Nombre { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFinal { get; set; }
        public string Base64 { get; set; }
        public string IdUser { get; set; }
        public string Descripcion { get; set; }
        public string IdTipoDescuento { get; set; }
        public string Valor { get; set; }
        public string IdApps { get; set; }
    }
}