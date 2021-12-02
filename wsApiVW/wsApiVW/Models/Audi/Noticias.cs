using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.Audi
{
    public class Noticia
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public string PDF { get; set; }
        public string IdApps { get; set; }
    }

    public class SubirNoticia 
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public string PDF { get; set; }
        public string IdUser { get; set; }
        public string IdApps { get; set; }
    }
}