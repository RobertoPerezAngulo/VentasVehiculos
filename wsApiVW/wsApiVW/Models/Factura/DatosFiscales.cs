using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.Factura
{
    public class DatosFiscales
    {
        public string IdCompra { get; set; } //FIAPIDCOMP, 
        public string RfcFisica { get; set; }//FSAPRFCFIS,
        public string NombreFisica { get; set; } //FSAPNMBFIS, 
        public string ApellidoPaternoFisica { get; set; }//FSAPAPTFIS,
        public string ApellidoMaternoFisica { get; set; }//FSAPAMTFIS,
        public string LadaFisica { get; set; }//FIAPLDTFIS,
        public string NumeroTelefonoFisica { get; set; }//FIAPNMTFIS,
        public string CorreoFisica { get; set; }//FSAPCRRFIS,
        public string RfcRazonSocial { get; set; }//FSAPRFCRSC,
        public string RazonSocial { get; set; }//FSAPRAZSOC,
        public string RfcRepresentanteLegal { get; set; }//FSAPRFCRSC,
        public string NombreRepresentanteLegal { get; set; }//FSAPNMBRLG, 
        public string ApellidoPaternoRepresentanteLegal { get; set; }//FSAPAPTRLG,
        public string ApellidoMaternoRepresentantelegal { get; set; }//FSAPAMTRLG,
        public string LadaRepresentantelegal { get; set; }//FIAPLDTRLG, 
        public string NumeroTelefonoRepresentanteLegal { get; set; }//FIAPNMTRLG
        public string CorreoRepresentantelegal { get; set; }//FSAPCRRRLG
        public string ClaveUsoCfdi { get; set; }// FSAPCUCFDI
        public string DescripcionUsoCfdi { get; set; }// FSAPDESCRI

        // aqui inicia lo nuevo
        public string IdNacionalidadC2 { get; set; } // FIAPIDNACP
        public string DescripcionNacionalidadC2 { get; set; } // FSAPDESNAP
        public string CurpC2 { get; set; } //FSAPCURPPE

        //c2

        public string CallePersonaC2 { get; set; } //-FSAPCALLEP    -CALLE PERSONA
        public string NumeroExteriorPersonaC2 { get; set; } //-FSAPNUMEXP    -NUMERO EXTERIOR PERSONA 
        public string NumeroInteriorPersonaC2 { get; set; } //-FSAPNUMINP    -NUMERO INTERIOR PERSONA 
        public string ColoniaPersonaC2 { get; set; }//-FSAPCOLONP    -COLONIA PERSONA
        public string DelegacionPersonaC2 { get; set; } //-FSAPDELEGP    -DELEGACION PERSONA 
        public string CiudadPersonaC2 { get; set; } //-FSAPCIUDAP    -CIUDAD PERSONA
        public string EstadoPersonaC2 { get; set; } //-FSAPESTADP    -ESTADO PERSONA

        public string DireccionC2 { get; set; } // FSAPDIRPER    


        //c2 

        public string IdGeneroPersonaPF { get; set; } //-FIAPIDGENP ID GENERO PERSONA
        public string DescripcionGeneroPF { get; set; } //-FSAPDESGEP DESCRIPCION DE GENERO PERSONA
        public string IdOcupacionPersonaPF { get; set; } //-FIAPIDOCUP ID OCUPACION PERSONA
        public string DescripcionOcupacionPF { get; set; } //-FSAPDESOCP DESCRIPCION DE OCUPACION PERSONA
        // pf


        public string IdTipoSociedadPM { get; set; }  //-FIAPIDTSOP ID TIPO DE SOCIEDAD PERSONA 
        public string DescripcionTipoSociedadPM { get; set; }  //-FSAPDESTSP TIPO DE SOCIEDAD PERSONA
        public string FechaConstitucionEmpresaPM { get; set; }  //-FFAPCREEMP FECHA DE CONSTITUCION DE LA EMPRESA PERSONA
        public string IdGiroDeLaEmpresaPM { get; set; }  //-FIAPIDGIRO ID GIRO DE LA EMPRESA
        public string DescripcionGiroDeLaEmpresaPM { get; set; }  //-FSAPDESCGI GIRO DE LA EMPRESA PERSONA


        public string CalleRepresentanteLegalVPF { get; set; }   //-FSAPCALLER    -CALLE REPRESENTANTE LEGAL
        public string NumeroExteriorRepresentanteLegalVPF { get; set; }   //-FSAPNUMEXR    -NUMERO EXTERIOR REPRESENTANTE LEGAL
        public string NumeroInteriorRepresentanteLegalVPF { get; set; }   //-FSAPNUMINR    -NUMERO INTERIOR REPRESENTANTE LEGAL
        public string ColoniaRepresentanteLegalVPF { get; set; }   //-FSAPCOLONR    -COLONIA REPRESENTANTE LEGAL
        public string DelegacionRepresentanteLegalVPF { get; set; }   //-FSAPDELEGR    -DELEGACION REPRESENTANTE LEGAL
        public string CiudadRepresentanteLegalVPF { get; set; }   //-FSAPCIUDAR    -CIUDAD REPRESENTANTE LEGAL
        public string EstadoRepresentanteLegalVPF { get; set; }   //-FSAPESTADR    -ESTADO REPRESENTANTE LEGAL

        public string DireccionRepresentanteLegalVPF { get; set; } // FSAPDIRREP    

        public string IdNacionalidadRepresentanteLegalVPF { get; set; }   //-FIAPIDNACR ID NACIONALIDAD REPRESENTANTE LEGAL
        public string DescripcionNacionalidadRepresentanteLegalVPF { get; set; }   //-FSAPDESNAR DESCRIPCION NACIONALIDAD REPRESENTANTE LEGAL
        public string IdGeneroRepresentanteLegalVPF { get; set; }   //-FIAPIDGENR ID GENERO REPRESENTANTE LEGAL
        public string DescripcionGeneroRepresentanteLegalVPF { get; set; }   //-FSAPDESGER DESCRIPCION DE GENERO REPRESENTANTE LEGAL
        public string IdOcupacionRepresentanteLegalVPF { get; set; }   //-FIAPIDOCUR ID OCUPACION REPRESENTANTE LEGAL
        public string DescripcionOcupacionRepresentanteLegalVPF { get; set; }   //-FSAPDESOCR DESCRIPCION DE OCUPACION REPRESENTANTE LEGAL



        public string ApellidoPaternoContactoVPF { get; set; }     //  -FSAPAPTCON APELLIDO PATERNO CONTACTO
        public string ApellidoMaternoContactoVPF { get; set; }     //  -FSAPAPMCON APELLIDO MATERNO CONTACTO
        public string NombresContactoVPF { get; set; }     //  -FSAPNOMCON NOMBRES    CONTACTO
        public string LadaContactoVPF { get; set; }     //  -FIAPLADCON LADA CONTACTO
        public string TelefonoContactoVPF { get; set; }     //  -FIAPTELCON TELEFONO CONTACTO
        public string CorreoContactoVPF { get; set; }     //  -FSAPCORCON CORREO CONTACTO
    }
}