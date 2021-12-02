using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.User
{
    public class Notificacion
    {
        public string IdCuenta { get; set; }
        public string IdNotificacion { get; set; }
        public string FechaNotificacion { get; set; }
        public string HoraNotificacion { get; set; }
        public string Asunto { get; set; }
        public string DescripcionNotificacion { get; set; }
        public string AplicaSeguimiento { get; set; }
        public string IdAgencia { get; set; }
        public string IdPreorden { get; set; }
        public string AplicaEncuesta { get; set; }
        public string NombreLogo { get; set; }
        public string IdEncuesta { get; set; }
        public string Instrucciones { get; set; }
        public string Visto { get; set; }
    }
}