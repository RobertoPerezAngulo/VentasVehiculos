using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class referenciasBancarias
    {
        public string IdMarca { get; set; }
        public string RutaLogo { get; set; }
        public List<DatosBancariosXIdAgencia> Datos { get; set; }
    }
    public class DatosBancariosXIdAgencia
    {
        public string IdAgencia { get; set; }
        public List<CuentasBancariasPorAgencia> DatosBancarios { get; set; }
    }
    public class CuentasBancariasPorAgencia
    {
        public string RazonSocial { get; set; }
        public string Ubicacion { get; set; }
        public List<CuentaBancaria> CuentasBancarias { get; set; }
    }
    public class CuentaBancaria
    {
        public string Banco { get; set; }
        public string NombreLogo { get; set; }
        public string NumeroCuenta { get; set; }
        public string ClabeInterbancaria { get; set; }

    }
}