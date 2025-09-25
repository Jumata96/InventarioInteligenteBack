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
}
