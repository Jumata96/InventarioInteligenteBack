using InventarioInteligenteBack.Application.Interfaces;
using InventarioInteligenteBack.Domain.Entities;
using InventarioInteligenteBack.Api.DTOs;
using InventarioInteligenteBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioInteligenteBack.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly AppDbContext _db;
        public ProductoService(AppDbContext db) => _db = db;

        public async Task<List<ProductoReadDto>> GetAllAsync()
        {
            return await _db.Productos
                .Where(p => p.Estado == 1)
                .Select(p => new ProductoReadDto(p.ProductoId, p.Nombre, p.Descripcion, p.Precio, p.Stock, p.Estado))
                .ToListAsync();
        }

        public async Task<ProductoReadDto?> GetByIdAsync(int id)
        {
            var p = await _db.Productos.AsNoTracking().FirstOrDefaultAsync(x => x.ProductoId == id && x.Estado != 0);
            return p is null ? null : new ProductoReadDto(p.ProductoId, p.Nombre, p.Descripcion, p.Precio, p.Stock, p.Estado);
        }

        public async Task<ProductoReadDto> CreateAsync(ProductoCreateDto dto)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("Nombre es obligatorio.");
            if (dto.Precio <= 0) throw new ArgumentException("El precio debe ser mayor a 0.");
            if (dto.Stock < 0) throw new ArgumentException("El stock no puede ser negativo.");

            // Verificar duplicado por índice único
            var exists = await _db.Productos.AnyAsync(x => x.Nombre == dto.Nombre && x.Estado != 0);
            if (exists) throw new InvalidOperationException("Ya existe un producto activo con ese nombre.");

            var entity = new Producto
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Stock = dto.Stock,
                Estado = 1
                // FechaCreacion la pone SQL Server
            };

            _db.Productos.Add(entity);
            await _db.SaveChangesAsync();

            return new ProductoReadDto(entity.ProductoId, entity.Nombre, entity.Descripcion, entity.Precio, entity.Stock, entity.Estado);
        }

        public async Task UpdateAsync(int id, ProductoUpdateDto dto)
        {
            var entity = await _db.Productos.FirstOrDefaultAsync(x => x.ProductoId == id && x.Estado != 0);
            if (entity is null) throw new KeyNotFoundException("Producto no encontrado.");

            if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("Nombre es obligatorio.");
            if (dto.Precio <= 0) throw new ArgumentException("El precio debe ser mayor a 0.");
            if (dto.Stock < 0) throw new ArgumentException("El stock no puede ser negativo.");

            // Validar duplicado de nombre (excluyéndome)
            var dup = await _db.Productos.AnyAsync(x => x.ProductoId != id && x.Nombre == dto.Nombre && x.Estado != 0);
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
            var entity = await _db.Productos.FirstOrDefaultAsync(x => x.ProductoId == id && x.Estado != 0);
            if (entity is null) throw new KeyNotFoundException("Producto no encontrado.");

            // Eliminación lógica del Producto
            entity.Estado = 0;
            entity.FechaEliminacion = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }
        public async Task<bool> EnableAsync(int id)
        {
            var product = await _db.Productos.FindAsync(id);
            if (product == null) return false;

            product.Estado = 1; // enabled
            product.FechaEdicion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DisableAsync(int id)
        {
            var product = await _db.Productos.FindAsync(id);
            if (product == null) return false;

            product.Estado = 2; // disabled
            product.FechaEdicion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<ProductoPagedDto<ProductoReadDto>> GetPagedAsync(int page, int pageSize, string? query = null)

        //public async Task<ProductoPagedDto<ProductoReadDto>> GetPagedAsync(int page, int pageSize, string? query = null)
        {
            if (page < 0) page = 0;

            var q = _db.Productos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var lower = query.ToLower();
                q = q.Where(p => p.Nombre.ToLower().Contains(lower));
            }

            q = q.Where(p => p.Estado != 0);

            var totalCount = await q.CountAsync();

            var data = await q
                .OrderBy(p => p.Nombre)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(p => new ProductoReadDto(
                    p.ProductoId,
                    p.Nombre,
                    p.Descripcion,
                    p.Precio,
                    p.Stock,
                    p.Estado
                ))
                .ToListAsync();

            return new ProductoPagedDto<ProductoReadDto>(data, totalCount);
        }



    }

}
