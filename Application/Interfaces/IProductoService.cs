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

        Task<ProductoPagedDto<ProductoReadDto>> GetPagedAsync(int page, int pageSize, string? query = null);
        //Task<(IEnumerable<ProductoReadDto> Data, int TotalCount)> GetPagedAsync(int page, int pageSize, string? query = null);
         Task<bool> EnableAsync(int id);
        Task<bool> DisableAsync(int id);
    }
}
