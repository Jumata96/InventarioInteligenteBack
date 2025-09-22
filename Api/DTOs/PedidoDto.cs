namespace InventarioInteligenteBack.Api.DTOs
{
    public record DetallePedidoCreateDto(
        int ProductoId,
        int Cantidad
    );

    public record PedidoCreateDto(
        int ClienteId,
        int PaisId,
        List<DetallePedidoCreateDto> Detalles
    );

    public record DetallePedidoReadDto(
        int ProductoId,
        string ProductoNombre,
        int Cantidad,
        decimal PrecioUnitario,
        decimal Subtotal
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
        List<DetallePedidoReadDto> Detalles
    );
}
