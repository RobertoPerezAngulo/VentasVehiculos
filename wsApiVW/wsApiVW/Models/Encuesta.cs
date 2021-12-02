using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class Encuesta
    {
        public string IdEncuesta { get; set; }
        public List<Preguntas> Preguntas { get; set; }
    }
    public class Preguntas
    {
        public string IdPregunta { get; set; }
        public string Pregunta { get; set; }
        public List<Respuestas> Respuestas { get; set; }
        public string ValorRespuestaSeleccionada { get; set; }
    }
    public class Respuestas
    {
        public string ValorRespuesta { get; set; }
    }
    public class EncuestaOrden
    {
        public string IdEncuesta { get; set; }
        public string Encuesta { get; set; }

    }
}