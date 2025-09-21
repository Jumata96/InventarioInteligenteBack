using InventarioInteligenteBack.Api.DTOs;

namespace InventarioInteligenteBack.Application.Interfaces
{
    public interface IProductoService
    {
        Task<List<ProductoReadDto>> GetAllAsync();
        Task<ProductoReadDto?> GetByIdAsync(int id);
        Task<ProductoReadDto> CreateAsync(ProductoCreateDto dto);
        Task UpdateAsync(int id, ProductoUpdateDto dto);
        Task DeleteAsync(int id); // soft delete
    }
}
