namespace InventarioInteligenteBack.Api.DTOs
{
    public record FacturaReadDto(
        int FacturaId,
        int PedidoId,
        string NumeroFactura,
        DateTime FechaEmision,
        decimal Subtotal,
        decimal Descuento,
        decimal Impuesto,
        decimal Total,
        string Estado,
        string UrlPdf
    );
    public class FacturaReadPdfDto
    {
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }

        // Cliente
        public ClientePdfDto Cliente { get; set; } = new();

        // Detalles
        public List<DetalleFacturaPdfDto> Detalles { get; set; } = new();
    }
    public class ClientePdfDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
    }

    public class DetalleFacturaPdfDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
