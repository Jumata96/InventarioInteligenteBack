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
            // Validar Cliente
            var cliente = await _db.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null) throw new Exception("Cliente no existe");

            // Validar País
            var pais = await _db.Paises.FindAsync(dto.PaisId);
            if (pais == null) throw new Exception("País no existe");

            var pedido = new Pedido
            {
                ClienteId = dto.ClienteId,
                PaisId = dto.PaisId,
                UsuarioId = usuarioId,
                Fecha = DateTime.UtcNow,
                Estado = "Emitido",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            decimal subtotal = 0;

            foreach (var d in dto.Detalles)
            {
                var producto = await _db.Productos.FindAsync(d.ProductoId);
                if (producto == null) throw new Exception($"Producto {d.ProductoId} no existe");
                if (producto.Stock < d.Cantidad) throw new Exception($"Stock insuficiente para {producto.Nombre}");

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
                .Where(i => i.PaisId == dto.PaisId && i.Activo)
                .Select(i => i.Porcentaje)
                .FirstOrDefaultAsync();

            pedido.Impuesto = subtotal * (impuesto / 100);
            pedido.Total = pedido.Subtotal - pedido.Descuento + pedido.Impuesto;

            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync();

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
    }
}
