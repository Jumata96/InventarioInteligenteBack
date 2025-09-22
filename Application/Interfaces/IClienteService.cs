using InventarioInteligenteBack.Api.DTOs;

namespace InventarioInteligenteBack.Application.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteReadDto>> GetAllAsync();
        Task<ClienteReadDto?> GetByIdAsync(int id);
        Task<ClienteReadDto> CreateAsync(ClienteCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
