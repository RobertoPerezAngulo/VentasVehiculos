using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class ApuntadorDeServicio
    {
        private bool IsProduction;
        public ApuntadorDeServicio()
        {
            IsProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["IsProduction"]);
        }
        //True=production False=Test 
        public string Respuestservicio()
        {
            string regreso = string.Empty;
            switch (IsProduction)
            {
                case true:
                    regreso = "http://ws-smartit.divisionautomotriz.com/wsApiVW";
                    break;
                case false:
                    regreso = "http://10.5.16.17/wsApiVW";
                    break;
            }
            return regreso;
        }

        public string RespuestaFacturacion()
        {
            string regreso = string.Empty;
            switch (IsProduction)
            {
                case true:
                    regreso = "http://10.5.2.21:7070/wsRegistraPersona/api/Pedido/registrarconreferenciaMarca/valor";
                    break;
                case false:
                    regreso = "http://10.5.2.21:7071/wsRegistraPersona_Des/api/Pedido/registrarconreferenciaMarca/valor";
                    break; 
            }
            return regreso;
        }

    }
}