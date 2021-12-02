using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class Citas
    {
        public string IdApps { get; set; }
        public string IdTipo { get; set; }
        public string IdCompra { get; set; }
        public string IdTurno { get; set; }
        public string Descripcion { get; set; }
        public string Fecha { get; set; } 
        public string HoraInicial { get; set; } 
        public string HoraFinal { get; set; } 
        public string Ubicacion { get; set; } 
    }

    public class CatalogoCitas
    {
        public string IdTipo { get; set; }
        public string Descripcion { get; set; }
    }

    public class CitaAppGenerica
    {
        public string IdApps { get; set; }
        public string IdTipoCita { get; set; }
        public string IdTurnoMatVes { get; set; }
        public string IdCompra { get; set; }
        public string Fecha { get; set; }
        public string HoraInicial { get; set; }
        public string HoraFinal { get; set; }
        public string Ubicacion { get; set; }
    }

    public class Rangos
    {
        public string Dia { get; set; }
        public string Horario { get; set; }
        public List<Intervalos> Intervalos { get; set; }
    }
    public class Intervalos
    {
        public string Dia { get; set; }
        public string IdTurno { get; set; }
        public string Intervalo { get; set; }
    }
    public class CitasExistentes
    {
        public string Tipo { get; set; }
        public string Intervalo { get; set; }
        public string Fecha { get; set; }
        public string Turno { get; set; }

    }
    public class HorariosCitas
    {
        public string TipoDeCita { get; set; }
        public List<Fechas> Fechas { get; set; }
        public List<Direccione> Ubicaciones { get; set; }
    }

    public class Fechas
    {
        public string Fecha { get; set; }
        public string Dia { get; set; }
        public List<Horarios> Horarios { get; set; }
        public List<Intervalos> Intervalos { get; set; }
    }
    public class Horarios
    {
        public string Hora { get; set; }
        public string Dia { get; set; }
        public string IdTurno { get; set; }

    }
}