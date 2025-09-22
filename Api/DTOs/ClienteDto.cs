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
        int PaisId,
        bool Activo
    );
}
