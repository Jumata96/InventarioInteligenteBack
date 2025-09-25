namespace InventarioInteligenteBack.Api.DTOs
{
    public record PaisCreateDto(
        string Codigo,
        string Nombre
    );

    public record PaisReadDto(
        int PaisId,
        string Codigo,
        string Nombre,
        int Estado
    );
}
