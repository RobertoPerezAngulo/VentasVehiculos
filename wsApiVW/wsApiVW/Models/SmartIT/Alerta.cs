using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.SmartIT
{
    public class Alerta
    {
        public string to { get; set; }
        public notification notification { get; set; }
    }
    public class notification
    {
        public string title { get; set; }
        public string body { get; set; }
        public string badge { get; set; }
    }
}