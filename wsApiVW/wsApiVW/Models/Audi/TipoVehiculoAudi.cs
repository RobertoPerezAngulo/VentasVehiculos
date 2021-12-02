using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using wsApiVW.Models.AutoModels;

namespace wsApiVW.Models.Audi
{
    public class TipoVehiculoAudi
    {
        public string Id { get; set; }
        public string Tipo { get; set; }
    }


    public class InventarioAudi : Inventario
    {
        public List<AccesorioAudi> Accesorios { get; set; } = new List<AccesorioAudi>();
        public string PrecioBase { get; set; }
        public string PrecioEspecial { get; set; }
        public string IdModelo { get; set; }
        public string Titulo { get; set; }
    }

    public class AccesorioAudi
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
    }
}