namespace InventarioInteligenteBack.Api.DTOs
{
    public record ClienteCreateDto(
        string Ruc,
        string Nombre,
        string? Email,
        string? Telefono,
        string? Direccion,
        int PaisId
    );

    public record ClienteReadDto(
        int ClienteId,
        string Ruc,
        string Nombre,
        string? Email,
        string? Telefono,
        string? Direccion,
        string PaisNombre,
        int PaisId,
        int Estado
    );
    public record ClienteUpdateDto(
        string Ruc,
        string Nombre,
        string? Email,
        string? Telefono,
        string? Direccion,
        int PaisId
    );
    public record ClientePagedDto<T>(
        IEnumerable<T> Items,
        int TotalCount
    );
}
