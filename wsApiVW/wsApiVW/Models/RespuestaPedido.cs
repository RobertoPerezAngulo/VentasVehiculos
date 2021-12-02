using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class RespuestaPedido
    {
        public string IdPedido { get; set; }
        public string Ruta { get; set; }



        public RespuestaPedido()
        {
        }

        public RespuestaPedido(string aIdPedido, string aRuta)
        {
            IdPedido = aIdPedido;
            Ruta = aRuta;
        }
    }
}