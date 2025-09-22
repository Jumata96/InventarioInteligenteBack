using InventarioInteligenteBack.Api.DTOs;

namespace InventarioInteligenteBack.Application.Interfaces
{
    public interface IPedidoService
    {
        Task<IEnumerable<PedidoReadDto>> GetAllAsync();
        Task<PedidoReadDto?> GetByIdAsync(int id);
        Task<PedidoReadDto> CreateAsync(PedidoCreateDto dto, string usuarioId);
    }
}
