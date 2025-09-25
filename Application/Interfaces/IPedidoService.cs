using InventarioInteligenteBack.Api.DTOs;

namespace InventarioInteligenteBack.Application.Interfaces
{
    public interface IPedidoService
    {
        // Listar todos los pedidos
        Task<IEnumerable<PedidoReadDto>> GetAllAsync();

        // Obtener pedidos paginados con filtro
        Task<(IEnumerable<PedidoReadDto> Data, int TotalCount)> GetPagedAsync(int page, int pageSize, string? query);

        // Obtener un pedido específico por ID
        Task<PedidoReadDto?> GetByIdAsync(int id);

        // Crear un pedido nuevo
        Task<PedidoReadDto> CreateAsync(PedidoCreateDto dto, string usuarioId);

        // Calcular descuento sin crear pedido
        Task<(decimal Subtotal, decimal Descuento, decimal Total)> CalcularDescuentoAsync(PedidoCreateDto dto);
    }
}
