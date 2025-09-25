namespace InventarioInteligenteBack.Domain.Entities
{
    public class Factura
    {
        public int FacturaId { get; set; }
        public int PedidoId { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Emitida";
        public string UrlPdf { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEdicion { get; set; }

        public Pedido Pedido { get; set; } = default!;
    }
}
