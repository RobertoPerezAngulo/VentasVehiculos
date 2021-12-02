using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class RegistroPersonaFisica
    {
        public string IdPersona { get; set; } //FDPFIDPERS,
        public string Nombre { get; set; } //FSPFNOMBRE,
        public string ApellidoPaterno { get; set; } //FSPFAPATER,
        public string ApellidoMaterno { get; set; } //FSPFAMATER,
        public string IdSexo { get; set; } //FDPFIDSEXO,
        public string IdEstadoCIvil { get; set; } //FDPFIDEDCI,                                                
        public DateTime FechaAlta { get; set; }
    }
}