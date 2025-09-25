using InventarioInteligenteBack.Api.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace InventarioInteligenteBack.Application.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteReadDto>> GetAllAsync();
        Task<ClienteReadDto?> GetByIdAsync(int id);
        Task<ClienteReadDto> CreateAsync(ClienteCreateDto dto);
        Task<bool> DeleteAsync(int id);
         Task<ClientePagedDto<ClienteReadDto>> GetPagedAsync(int page, int pageSize, string? query = null);
       
       // Task<(IEnumerable<ClienteReadDto> Data, int TotalCount)> GetPagedAsync(int page, int pageSize, string? query = null);
        Task<bool> EnableAsync(int id);
        Task<bool> DisableAsync(int id);
        Task<bool> UpdateAsync(int id, ClienteUpdateDto dto);
        
        
    }
}
