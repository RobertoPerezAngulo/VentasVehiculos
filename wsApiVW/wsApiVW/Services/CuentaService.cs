using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using wsApiVW.Models;

namespace wsApiVW.Services
{
    public class CuentaService
    {
        internal Respuesta RegistraCuenta(CuentaService cuentaJson)
        {
            Respuesta respuesta = new Respuesta();
            return respuesta;
        }

        public static string GenerarClaveActivacion()
        {
            string clave = "";
            string[] caracteres = new string[] { "a", "b", "c", "d", "e", "f", "g", "h",
            "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            "A", "B", "C", "D", "E", "F",
            "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
            Random ran = new Random();
            while (clave.Length != 8)
            {
                int aleatorio = ran.Next(61);
                clave += caracteres[aleatorio];
            }
            return clave;
        }
    }
}