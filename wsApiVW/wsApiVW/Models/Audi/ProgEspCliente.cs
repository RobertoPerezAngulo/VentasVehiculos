using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.Audi
{
    public class ProgramasEspeciales
    {
        public string IdProgramaEspecial { get; set; }
        public string IdConsecutivo { get; set; } = "";
        public string IdApps { get; set; }
        public string RutaPDF { get; set; }
        public string IdEstado { get; set; }
        public string EstadoProgramaEspecial { get; set; }
        public string IdCompra { get; set; }
        public string Titulo { get; set; }
        public string IdDescuento { get; set; }
        public string Descuento { get; set; }
        public string MotivoRechazo { get; set; } = "";
        public List<ProgramaEspecialcliente> Documentos { get; set; }
    }

    public class ProgramaEspecialcliente
    {
        public string IdDocumento { get; set; }
        public string RutaDoc { get; set; }
    }

    public class ConsutaProgramaespecial
    {
        public string IdCuenta { get; set; }
        public string IdApps { get; set; }
        public string IdCompra { get; set; }
    }
}