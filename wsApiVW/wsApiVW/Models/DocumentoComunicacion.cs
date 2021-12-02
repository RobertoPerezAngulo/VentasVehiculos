using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class DocumentoComunicacion
    {
        public string IdCompra { get; set; } // FIAPIDCOMP   /*ID COMPRA*/
        public string IdConsecutivo { get; set; } //FIAPIDCONS
        public string IdTipo { get; set; } //,FIAPIDTIPO   /*ID TIPO*/
        public string NombreDocumento { get; set; } //,FSAPDOCUME   /*DOCUMENTO*/
        public string RutaDocumento { get; set; }
    }
}