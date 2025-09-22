using InventarioInteligenteBack.Api.DTOs;
using InventarioInteligenteBack.Application.Interfaces;
using InventarioInteligenteBack.Domain.Entities;
using InventarioInteligenteBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioInteligenteBack.Application.Services
{
    public class ImpuestoService : IImpuestoService
    {
        private readonly AppDbContext _db;
        public ImpuestoService(AppDbContext db) => _db = db;

        public async Task<IEnumerable<ImpuestoReadDto>> GetAllAsync()
        {
            return await _db.Impuestos
                .Select(i => new ImpuestoReadDto(i.ImpuestoId, i.PaisId, i.Nombre, i.Porcentaje, i.Activo))
                .ToListAsync();
        }

        public async Task<IEnumerable<ImpuestoReadDto>> GetByPaisAsync(int paisId)
        {
            return await _db.Impuestos
                .Where(i => i.PaisId == paisId && i.Activo)
                .Select(i => new ImpuestoReadDto(i.ImpuestoId, i.PaisId, i.Nombre, i.Porcentaje, i.Activo))
                .ToListAsync();
        }

        public async Task<ImpuestoReadDto?> GetByIdAsync(int id)
        {
            var i = await _db.Impuestos.FindAsync(id);
            return i == null ? null : new ImpuestoReadDto(i.ImpuestoId, i.PaisId, i.Nombre, i.Porcentaje, i.Activo);
        }

        public async Task<ImpuestoReadDto> CreateAsync(ImpuestoCreateDto dto)
        {
            var impuesto = new Impuesto
            {
                PaisId = dto.PaisId,
                Nombre = dto.Nombre,
                Porcentaje = dto.Porcentaje,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _db.Impuestos.Add(impuesto);
            await _db.SaveChangesAsync();

            return new ImpuestoReadDto(impuesto.ImpuestoId, impuesto.PaisId, impuesto.Nombre, impuesto.Porcentaje, impuesto.Activo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var impuesto = await _db.Impuestos.FindAsync(id);
            if (impuesto == null) return false;

            impuesto.Activo = false;
            impuesto.FechaEliminacion = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
