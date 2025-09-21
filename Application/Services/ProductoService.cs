using InventarioInteligente.Application.Interfaces;
using InventarioInteligente.Domain.Entities;
using InventarioInteligente.DTOs;
using InventarioInteligente.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioInteligente.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly AppDbContext _db;
        public ProductoService(AppDbContext db) => _db = db;

        public async Task<List<ProductoReadDto>> GetAllAsync()
        {
            return await _db.Productos
                .Where(p => p.Activo)
                .Select(p => new ProductoReadDto(p.ProductoId, p.Nombre, p.Descripcion, p.Precio, p.Stock, p.Activo))
                .ToListAsync();
        }

        public async Task<ProductoReadDto?> GetByIdAsync(int id)
        {
            var p = await _db.Productos.AsNoTracking().FirstOrDefaultAsync(x => x.ProductoId == id && x.Activo);
            return p is null ? null : new ProductoReadDto(p.ProductoId, p.Nombre, p.Descripcion, p.Precio, p.Stock, p.Activo);
        }

        public async Task<ProductoReadDto> CreateAsync(ProductoCreateDto dto)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("Nombre es obligatorio.");
            if (dto.Precio <= 0) throw new ArgumentException("El precio debe ser mayor a 0.");
            if (dto.Stock < 0) throw new ArgumentException("El stock no puede ser negativo.");

            // Verificar duplicado por índice único
            var exists = await _db.Productos.AnyAsync(x => x.Nombre == dto.Nombre && x.Activo);
            if (exists) throw new InvalidOperationException("Ya existe un producto activo con ese nombre.");

            var entity = new Producto
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Stock = dto.Stock,
                Activo = true
                // FechaCreacion la pone SQL Server
            };

            _db.Productos.Add(entity);
            await _db.SaveChangesAsync();

            return new ProductoReadDto(entity.ProductoId, entity.Nombre, entity.Descripcion, entity.Precio, entity.Stock, entity.Activo);
        }

        public async Task UpdateAsync(int id, ProductoUpdateDto dto)
        {
            var entity = await _db.Productos.FirstOrDefaultAsync(x => x.ProductoId == id && x.Activo);
            if (entity is null) throw new KeyNotFoundException("Producto no encontrado.");

            if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("Nombre es obligatorio.");
            if (dto.Precio <= 0) throw new ArgumentException("El precio debe ser mayor a 0.");
            if (dto.Stock < 0) throw new ArgumentException("El stock no puede ser negativo.");

            // Validar duplicado de nombre (excluyéndome)
            var dup = await _db.Productos.AnyAsync(x => x.ProductoId != id && x.Nombre == dto.Nombre && x.Activo);
            if (dup) throw new InvalidOperationException("Ya existe otro producto activo con ese nombre.");

            entity.Nombre = dto.Nombre.Trim();
            entity.Descripcion = dto.Descripcion;
            entity.Precio = dto.Precio;
            entity.Stock = dto.Stock;
            entity.FechaEdicion = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Productos.FirstOrDefaultAsync(x => x.ProductoId == id && x.Activo);
            if (entity is null) throw new KeyNotFoundException("Producto no encontrado.");

            // Eliminación lógica del Producto
            entity.Activo = false;
            entity.FechaEliminacion = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }
    }
}
