using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class RespuestaTest<T>
    {
        public string Ok { get; set; }
        public string Mensaje { get; set; }
        public T Objeto { get; set; }
    }

    public class Respuesta
    {
        public string Ok { get; set; }
        public string Mensaje { get; set; }
        public string Objeto { get; set; }
    }

}