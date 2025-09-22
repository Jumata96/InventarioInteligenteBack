namespace InventarioInteligenteBack.Api.DTOs
{
    public record DetallePedidoDto(
        int ProductoId,
        int Cantidad,
        decimal PrecioUnitario
    );

    public record PedidoCreateDto(
        int ClienteId,
        int PaisId,
        List<DetallePedidoDto> Detalles
    );

    public record PedidoReadDto(
        int PedidoId,
        int ClienteId,
        int PaisId,
        decimal Subtotal,
        decimal Descuento,
        decimal Impuesto,
        decimal Total,
        string Estado,
        List<DetallePedidoDto> Detalles
    );
}
