using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class PersonaCliente
    {
        public string IdPersona { get; set; }  //FDPEIDPERS,
        public string IdLegacy { get; set; }  //FSPECTEUNI,
        public string IdAgencia { get; set; }  //FIPEIDCIAU, 
        public string Rfc { get; set; }  //FSPERFC,
        public string IdTipo { get; set; }  //FDPEIDTIPO, 
        public string TipoCliente { get; set; }  //FDPEIDTCTE, 
        public string ClasePersona { get; set; }  //FDPEIDCLAS,
        public string FechaAlta { get; set; }  //FFPEALTA, 
        public string HoraAlta { get; set; }  //FHPEALTA,
        public string FechaEstatus { get; set; }  //FFPEESTATU,
        public string HoraEstatus { get; set; }  //FHPEESTATU,
    }
}