namespace InventarioInteligenteBack.Api.DTOs
{
    public record ImpuestoCreateDto(
        int PaisId,
        string Nombre,
        decimal Porcentaje
    );

    public record ImpuestoReadDto(
        int ImpuestoId,
        int PaisId,
        string Nombre,
        decimal Porcentaje,
        int Estado
    );
}
