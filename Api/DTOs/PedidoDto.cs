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
        string ClienteNombre,
        int PaisId,
        string PaisNombre,
        decimal Subtotal,
        decimal Descuento,
        decimal Impuesto,
        decimal Total,
        decimal TotalFinal,
        string Estado,
        List<DetallePedidoReadDto> Detalles
    );
}
