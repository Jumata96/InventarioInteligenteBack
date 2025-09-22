using InventarioInteligenteBack.Api.DTOs;

namespace InventarioInteligenteBack.Application.Interfaces
{
    public interface IImpuestoService
    {
        Task<IEnumerable<ImpuestoReadDto>> GetAllAsync();
        Task<IEnumerable<ImpuestoReadDto>> GetByPaisAsync(int paisId);
        Task<ImpuestoReadDto?> GetByIdAsync(int id);
        Task<ImpuestoReadDto> CreateAsync(ImpuestoCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
