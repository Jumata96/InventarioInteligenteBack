using InventarioInteligenteBack.Api.DTOs;
using InventarioInteligenteBack.Application.Interfaces;
using InventarioInteligenteBack.Domain.Entities;
using InventarioInteligenteBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioInteligenteBack.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly AppDbContext _db;
        public ClienteService(AppDbContext db) => _db = db;

        public async Task<IEnumerable<ClienteReadDto>> GetAllAsync()
        {
            return await _db.Clientes
                .Include(c => c.Pais)
                .Select(c => new ClienteReadDto(
                    c.ClienteId,
                    c.Ruc,
                    c.Nombre,
                    c.Email,
                    c.Telefono,
                    c.Direccion,
                    c.Pais.Nombre,
                    c.PaisId,
                    c.Estado
                ))
                .ToListAsync();
        }

        public async Task<ClienteReadDto?> GetByIdAsync(int id)
        {
            var c = await _db.Clientes
                .Include(x => x.Pais)
                .FirstOrDefaultAsync(x => x.ClienteId == id);

            if (c == null) return null;

            return new ClienteReadDto(
                c.ClienteId,
                c.Ruc,
                c.Nombre,
                c.Email,
                c.Telefono,
                c.Direccion,
                c.Pais.Nombre,
                c.PaisId,
                c.Estado
            );
        }

        public async Task<ClienteReadDto> CreateAsync(ClienteCreateDto dto)
        {
            var cliente = new Cliente
            {
                Ruc = dto.Ruc,
                Nombre = dto.Nombre,
                Email = dto.Email,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                PaisId = dto.PaisId,
                Estado = 1,
                FechaCreacion = DateTime.UtcNow
            };

            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync();

            // cargar país para devolver DTO completo
            var pais = await _db.Paises.FindAsync(cliente.PaisId);

            return new ClienteReadDto(
                cliente.ClienteId,
                cliente.Ruc,
                cliente.Nombre,
                cliente.Email,
                cliente.Telefono,
                cliente.Direccion,
                pais?.Nombre ?? "",
                cliente.PaisId,
                cliente.Estado
            );
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _db.Clientes.FindAsync(id);
            if (cliente == null) return false;

            cliente.Estado = 0;
            cliente.FechaEliminacion = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<ClienteReadDto>, int)> GetPagedAsync(int page, int pageSize)
        {
            var query = _db.Clientes.Include(c => c.Pais).AsQueryable();

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(c => c.ClienteId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClienteReadDto(
                    c.ClienteId,
                    c.Ruc,
                    c.Nombre,
                    c.Email,
                    c.Telefono,
                    c.Direccion,
                    c.Pais.Nombre,   // 👈 aquí estaba el error
                    c.PaisId,
                    c.Estado
                ))
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<bool> EnableAsync(int id)
        {
            var cliente = await _db.Clientes.FindAsync(id);
            if (cliente == null) return false;

            cliente.Estado = 1; // enabled
            cliente.FechaEdicion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DisableAsync(int id)
        {
            var cliente = await _db.Clientes.FindAsync(id);
            if (cliente == null) return false;

            cliente.Estado = 2; // disabled
            cliente.FechaEdicion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, ClienteUpdateDto dto)
        {
            var cliente = await _db.Clientes.FindAsync(id);
            if (cliente == null) return false;

            cliente.Ruc = dto.Ruc;
            cliente.Nombre = dto.Nombre;
            cliente.Email = dto.Email;
            cliente.Telefono = dto.Telefono;
            cliente.Direccion = dto.Direccion;
            cliente.PaisId = dto.PaisId;
            cliente.FechaEdicion = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
