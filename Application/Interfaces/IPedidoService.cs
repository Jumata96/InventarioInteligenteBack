using InventarioInteligenteBack.Api.DTOs;

namespace InventarioInteligenteBack.Application.Interfaces
{
    public interface IPedidoService
    {
        // Listar todos los pedidos
        Task<IEnumerable<PedidoReadDto>> GetAllAsync();

        // Obtener un pedido específico por ID
        Task<PedidoReadDto?> GetByIdAsync(int id);

        // Crear un pedido nuevo
        Task<PedidoReadDto?> CreateAsync(PedidoCreateDto dto, string usuarioId);
    }
}
