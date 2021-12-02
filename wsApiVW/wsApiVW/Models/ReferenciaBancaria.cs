using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class ReferenciaBancaria
    {
        public string RazonSocial { get; set; }
        public string Ubicacion { get; set; }
        public string IdReferencia { get; set; }
        public string Nombre { get; set; }
        public string NombreCliente { get; set; }
        public string NumeroCliente { get; set; }
        public string NumeroPedido { get; set; }
        public string LineaCaptura { get; set; }
        public List<Banco> Bancos { get; set; }
    }
    public class Banco
    {

        public string Nombre { get; set; }
        public string Convenio { get; set; }
        public string ClabeInterbancaria { get; set; }
    }

    public class ConceptoPago
    {

        public string IdConceptoPago { get; set; }
        public string DescripcionConceptoPago { get; set; }

    }
}