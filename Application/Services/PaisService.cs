using InventarioInteligenteBack.Api.DTOs;
using InventarioInteligenteBack.Application.Interfaces;
using InventarioInteligenteBack.Domain.Entities;
using InventarioInteligenteBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioInteligenteBack.Application.Services
{
    public class PaisService : IPaisService
    {
        private readonly AppDbContext _db;
        public PaisService(AppDbContext db) => _db = db;

        public async Task<IEnumerable<PaisReadDto>> GetAllAsync()
        {
            return await _db.Paises
                .Select(p => new PaisReadDto(p.PaisId, p.Codigo, p.Nombre, p.Activo))
                .ToListAsync();
        }

        public async Task<PaisReadDto?> GetByIdAsync(int id)
        {
            var p = await _db.Paises.FindAsync(id);
            return p == null ? null : new PaisReadDto(p.PaisId, p.Codigo, p.Nombre, p.Activo);
        }

        public async Task<PaisReadDto> CreateAsync(PaisCreateDto dto)
        {
            var pais = new Pais
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _db.Paises.Add(pais);
            await _db.SaveChangesAsync();

            return new PaisReadDto(pais.PaisId, pais.Codigo, pais.Nombre, pais.Activo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pais = await _db.Paises.FindAsync(id);
            if (pais == null) return false;

            pais.Activo = false;
            pais.FechaEliminacion = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
