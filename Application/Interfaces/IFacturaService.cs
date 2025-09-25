using InventarioInteligenteBack.Api.DTOs;

namespace InventarioInteligenteBack.Application.Interfaces
{
    public interface IFacturaService
    {
        Task<FacturaReadDto?> EmitirFacturaAsync(int pedidoId);
        Task<FacturaReadDto?> GetByPedidoAsync(int pedidoId);
        Task<FacturaReadDto?> GetByIdAsync(int id);
        Task<IEnumerable<FacturaReadDto>> GetAllAsync();
    }
}
