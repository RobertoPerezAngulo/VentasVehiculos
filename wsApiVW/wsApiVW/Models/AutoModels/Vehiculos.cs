using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class OVehiculo
    {
        public string IdMarca { get; set; }
        public string IdModelo { get; set; }
        public string NombreModelo { get; set; }
        public string Anio { get; set; }
        public string Atributo1 { get; set; }
        public string Atributo2 { get; set; }
        public string Atributo3 { get; set; }
        public string Atributo4 { get; set; }
        public string Atributo5 { get; set; }
        public string Atributo6 { get; set; }
        public string NombreVersion { get; set; }
        public string RutaVideo { get; set; }
        public string RutaFichaTecnica { get; set; }
        public string Ruta360 { get; set; }
        public string IdColor { get; set; }
        public string NombreColor { get; set; }
        public string Ruta { get; set; }
        public string RutaMini { get; set; }
        public string IdVersion { get; set; }
        public string Transmision { get; set; }
        public string PrecioContado { get; set; }
        public string PrecioLista { get; set; }
    }
    public class Vehiculos
    {
        public string IdMarca { get; set; }
        public List<Modelo> Modelos { get; set; }
    }

    public partial class Modelo
    {
        public string IdModelo { get; set; }
        public string TipoCarro { get; set; }
        public string NombreModelo { get; set; }
        public List<AnioAnio> Anios { get; set; }
    }

    public partial class AnioAnio
    {
        public string Anio { get; set; }
        public string Atributo1 { get; set; }
        public string Atributo2 { get; set; }
        public string Atributo3 { get; set; }
        public string Atributo4 { get; set; }
        public List<Versione> Versiones { get; set; }
        public List<GamaColore> GamaColores { get; set; }
        public string Atributo5 { get; set; }
        public string Atributo6 { get; set; }
    }


    public partial class GamaColore
    {
        public dynamic IdColor { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string RutaMini { get; set; }

    }

    public partial class Versione
    {
        public string Nombre { get; set; }
        public string RutaVideo { get; set; }
        public string RutaFichaTecnica { get; set; }
        public string Ruta360 { get; set; }
        public List<PrecioTransmision> PrecioTransmision { get; set; }
        public List<Colore> Colores { get; set; }
        public List<Accesorio> Accesorios { get; set; }
    }

    public partial class Accesorio
    {
        public string Id { get; set; } = "";
        public string Concepto { get; set; } = "";
        public string Ruta { get; set; } = "";
        public string SubTotal { get; set; } = "";
        public string Iva { get; set; } = "";
        public string Total { get; set; } = "";
    }

    public partial class AccesorioDescuento
    {
        public string IdAgencia { get; set; }
        public string IdPedido { get; set; }
        public string IdConsecutivo { get; set; }
        public string Concepto { get; set; }
        public string Subtotal { get; set; }
        public string Descuento { get; set; }
        public string Iva { get; set; }
        public string Total { get; set; }
        public string RutaFoto { get; set; }
    }

    public partial class Colore
    {
        public string IdColor { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string RutaMini { get; set; }
    }

    public partial class PrecioTransmision
    {
        public string IdVersion { get; set; }
        public string Transmision { get; set; }
        public string PrecioContado { get; set; }
        public string PrecioLista { get; set; }
        public string Iva { get; set; }
        public string SubTotal { get; set; }
        public string Total { get; set; }
    }
}

