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
                .Select(c => new ClienteReadDto(c.ClienteId, c.Ruc, c.Nombre, c.Email, c.Telefono, c.Direccion, c.PaisId, c.Activo))
                .ToListAsync();
        }

        public async Task<ClienteReadDto?> GetByIdAsync(int id)
        {
            var c = await _db.Clientes.FindAsync(id);
            return c == null ? null : new ClienteReadDto(c.ClienteId, c.Ruc, c.Nombre, c.Email, c.Telefono, c.Direccion, c.PaisId, c.Activo);
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
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync();

            return new ClienteReadDto(cliente.ClienteId, cliente.Ruc, cliente.Nombre, cliente.Email, cliente.Telefono, cliente.Direccion, cliente.PaisId, cliente.Activo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _db.Clientes.FindAsync(id);
            if (cliente == null) return false;

            cliente.Activo = false;
            cliente.FechaEliminacion = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
