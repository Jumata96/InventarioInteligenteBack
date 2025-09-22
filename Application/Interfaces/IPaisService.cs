using InventarioInteligenteBack.Api.DTOs;

namespace InventarioInteligenteBack.Application.Interfaces
{
    public interface IPaisService
    {
        Task<IEnumerable<PaisReadDto>> GetAllAsync();
        Task<PaisReadDto?> GetByIdAsync(int id);
        Task<PaisReadDto> CreateAsync(PaisCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
