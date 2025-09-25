namespace InventarioInteligenteBack.Api.DTOs
{
    public record ProductoCreateDto(string Nombre, string? Descripcion, decimal Precio, int Stock);
    public record ProductoUpdateDto(string Nombre, string? Descripcion, decimal Precio, int Stock);
    public record ProductoReadDto(int ProductoId, string Nombre, string? Descripcion, decimal Precio, int Stock, int Estado);
    public record ProductoPagedDto<T>(
        IEnumerable<T> Items,
        int TotalCount
    );
}
