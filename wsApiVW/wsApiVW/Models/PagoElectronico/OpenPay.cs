﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models.PagoElectronico
{
    public class OpenPay
    {
        public string Rembolso { get; set; }
        public string FechaCreacion { get; set; }
        public string HoraCreacion { get; set; }
        public string Cantidad { get; set; }
        public string Estatus { get; set; }
        public string Descripcion { get; set; }
        public string TipoTransacion { get; set; }
        public string TipoOperacion { get; set; }
        public string Metodo { get; set; }
        public RespuestaTarjeta Tarjeta { get; set; }
        public string MensajeError { get; set; }
        public string Autorizacion { get; set; }
        public string IdOrden { get; set; }
        public string Conciliacion { get; set; }
        public string IdCargo { get; set; }
        public CancelarPago CargoCancelado { get; set; }
        public string IdPagoElectronico { get; set; }
        public string IdCompra { get; set; }
        public string TipoPago { get; set; }
    }

    public class CancelarPago
    {
        public string id { get; set; }
        public string Cantidad { get; set; }
        public string Autorizacion { get; set; }
        public string Metodo { get; set; }
        public string TipoOperacion { get; set; }
        public string TipoTransacion { get; set; }
        public string Estatus { get; set; }
        public string Descripcion { get; set; }
    }

    public class RespuestaTarjeta
    {
        public string Fecha { get; set; }
        public string Banco { get; set; }
        public string PagoAprovado { get; set; }
        public string TitularTarjeta { get; set; }
        public string Mes { get; set; }
        public string Anio { get; set; }
        public string Tarjeta { get; set; }
        public string Marca { get; set; }
        public string CargoAprovado { get; set; }
        public string NumeroBanco { get; set; }
        public string TipoTarjeta { get; set; }
        public string Id { get; set; }
    }
}