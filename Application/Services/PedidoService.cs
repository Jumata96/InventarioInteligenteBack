using InventarioInteligenteBack.Api.DTOs;
using InventarioInteligenteBack.Application.Interfaces;
using InventarioInteligenteBack.Domain.Entities;
using InventarioInteligenteBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioInteligenteBack.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly AppDbContext _db;
        public PedidoService(AppDbContext db) => _db = db;

        public async Task<IEnumerable<PedidoReadDto>> GetAllAsync()
        {
            return await _db.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .Select(p => new PedidoReadDto(
                    p.PedidoId,
                    p.ClienteId,
                    p.PaisId,
                    p.Subtotal,
                    p.Descuento,
                    p.Impuesto,
                    p.Total,
                    p.Estado,
                    p.Detalles.Select(d =>
                        new DetallePedidoReadDto(
                            d.ProductoId,
                            d.Producto.Nombre,
                            d.Cantidad,
                            d.PrecioUnitario,
                            d.Subtotal
                        )
                    ).ToList()
                ))
                .ToListAsync();
        }

        public async Task<PedidoReadDto?> GetByIdAsync(int id)
        {
            var p = await _db.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(x => x.PedidoId == id);

            if (p == null) return null;

            return new PedidoReadDto(
                p.PedidoId,
                p.ClienteId,
                p.PaisId,
                p.Subtotal,
                p.Descuento,
                p.Impuesto,
                p.Total,
                p.Estado,
                p.Detalles.Select(d =>
                    new DetallePedidoReadDto(
                        d.ProductoId,
                        d.Producto.Nombre,
                        d.Cantidad,
                        d.PrecioUnitario,
                        d.Subtotal
                    )
                ).ToList()
            );
        }

        public async Task<PedidoReadDto> CreateAsync(PedidoCreateDto dto, string usuarioId)
        {
            var cliente = await _db.Clientes.FindAsync(dto.ClienteId);
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Detalles == null || !dto.Detalles.Any())
                throw new ArgumentException("El pedido debe contener al menos un detalle.");

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // Validar Cliente
                var cliente = await _db.Clientes.FindAsync(dto.ClienteId);
                if (cliente == null)
                    throw new KeyNotFoundException("Cliente no existe");

                // Validar País
                var pais = await _db.Paises.FindAsync(dto.PaisId);
                if (pais == null)
                    throw new KeyNotFoundException("País no existe");

                var pedido = new Pedido
                {
                    ClienteId = dto.ClienteId,
                    PaisId = dto.PaisId,
                    UsuarioId = usuarioId,
                    Fecha = DateTime.UtcNow,
                    Estado = "Emitido", 
                    FechaCreacion = DateTime.UtcNow
                };

                decimal subtotal = 0;

                foreach (var d in dto.Detalles)
                {
                    if (d.Cantidad <= 0)
                        throw new InvalidOperationException($"Cantidad inválida para el producto {d.ProductoId}");

                    var producto = await _db.Productos.FindAsync(d.ProductoId);
                    if (producto == null)
                        throw new KeyNotFoundException($"Producto {d.ProductoId} no existe");

                    if (producto.Stock < d.Cantidad)
                        throw new InvalidOperationException($"Stock insuficiente para {producto.Nombre}. Disponible: {producto.Stock}");

                    var detalle = new DetallePedido
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = producto.Precio,
                        Subtotal = producto.Precio * d.Cantidad,
                        FechaCreacion = DateTime.UtcNow
                    };

                    subtotal += detalle.Subtotal;
                    pedido.Detalles.Add(detalle);

                    // Descontar stock
                    producto.Stock -= d.Cantidad;
                }

                pedido.Subtotal = subtotal;

                // Impuesto: usar el primero activo del país
                var impuesto = await _db.Impuestos
                    .Where(i => i.PaisId == dto.PaisId && i.Estado == 1)
                    .Select(i => i.Porcentaje)
                    .FirstOrDefaultAsync();

                pedido.Impuesto = subtotal * (impuesto / 100);
                pedido.Total = pedido.Subtotal - pedido.Descuento + pedido.Impuesto;

                if (pedido.Total < 0)
                    throw new InvalidOperationException("El total del pedido no puede ser negativo.");

                _db.Pedidos.Add(pedido);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return new PedidoReadDto(
                    pedido.PedidoId,
                    pedido.ClienteId,
                    pedido.PaisId,
                    pedido.Subtotal,
                    pedido.Descuento,
                    pedido.Impuesto,
                    pedido.Total,
                    pedido.Estado,
                    pedido.Detalles.Select(d =>
                        new DetallePedidoReadDto(
                            d.ProductoId,
                            d.Producto.Nombre,
                            d.Cantidad,
                            d.PrecioUnitario,
                            d.Subtotal
                        )
                    ).ToList()
                );
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
