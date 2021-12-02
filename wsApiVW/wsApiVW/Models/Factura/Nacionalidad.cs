using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.Factura
{
    public class Documento
    {
        public List<Nacionalidad> Nacionalidad { get; set; }
        public List<UsoCfdi> CFDI { get; set; }
        public List<CatalogoSexo> Sexo { get; set; }
        public List<Ocupacion> Ocupacion { get; set; }
        public List<Sociedad> Sociedad { get; set; }
        public List<GiroComercial> GiroComercial { get; set; }
        public List<DocumentoIdentificacion> Identificacion { get; set; }
    }
    public class Nacionalidad
    {
        public string IdNacionalidad { get; set; }
        public string DescripcionNacionalidad { get; set; }
    }
    public class UsoCfdi
    {
        public string ClaveUsoFiscal { get; set; }
        public string Descripcion { get; set; }
    }
    public class CatalogoSexo
    {
        public string IdSexo { get; set; }
        public string DescripcionSexo { get; set; }

    }
    public class Ocupacion
    {

        public string IdOcupacion { get; set; }
        public string DescripcionOcupacion { get; set; }

    }

    public class Sociedad
    {
        public string IdSociedad { get; set; }
        public string DescripcionSociedad { get; set; }
    }

    public class GiroComercial
    {
        public string IdGiroComercial { get; set; }
        public string DescripcionGiroComercial { get; set; }

    }

    public class DocumentoIdentificacion
    {

        public string IdTipoPersona { get; set; }
        public string DescripcionTipoPersona { get; set; }

        public List<Nacionalidades> Nacionalidades { get; set; }

    }
    public class Nacionalidades
    {

        public string Nacionalidad { get; set; }
        public List<DocumentacionRequerida> DocumentacionRequerida { get; set; }
    }
    public class DocumentacionRequerida
    {
        public string IdTipo { get; set; }
        public string DescripcionDocumento { get; set; }
        public string Obligatorio { get; set; }
    }

    public class SolicitudPedido
    {
        public string Serie { get; set; }
        public long IdCliente { get; set; }
        public long IdContacto { get; set; }
        public long IdAgente { get; set; }
        public int IdTipoDeVenta { get; set; }
        public decimal Total { get; set; }
        public string Marca { get; set; }
        public string RutaFisica { get; set; }
        public string IdAgencia { get; set; }
        public string IdUsuario { get; set; }
        public string IdPrograma { get; set; }
    }
}