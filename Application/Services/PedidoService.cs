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
                        new DetallePedidoDto(d.ProductoId, d.Cantidad, d.PrecioUnitario)
                    ).ToList()
                ))
                .ToListAsync();
        }

        public async Task<PedidoReadDto?> GetByIdAsync(int id)
        {
            var p = await _db.Pedidos
                .Include(p => p.Detalles)
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
                    new DetallePedidoDto(d.ProductoId, d.Cantidad, d.PrecioUnitario)
                ).ToList()
            );
        }

        public async Task<PedidoReadDto> CreateAsync(PedidoCreateDto dto, string usuarioId)
        {
            // Calcular Subtotal
            decimal subtotal = 0;
            var detalles = new List<DetallePedido>();
            foreach (var d in dto.Detalles)
            {
                var sub = d.Cantidad * d.PrecioUnitario;
                subtotal += sub;

                detalles.Add(new DetallePedido
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = sub,
                    FechaCreacion = DateTime.UtcNow
                });
            }

            // Impuesto: tomar el primero activo del país
            var impuesto = await _db.Impuestos
                .Where(i => i.PaisId == dto.PaisId && i.Activo)
                .Select(i => i.Porcentaje)
                .FirstOrDefaultAsync();

            var impuestoValor = subtotal * (impuesto / 100);
            var total = subtotal + impuestoValor;

            var pedido = new Pedido
            {
                ClienteId = dto.ClienteId,
                PaisId = dto.PaisId,
                UsuarioId = usuarioId,
                Fecha = DateTime.UtcNow,
                Subtotal = subtotal,
                Descuento = 0, // por ahora fijo
                Impuesto = impuestoValor,
                Total = total,
                Estado = "Emitido",
                FechaCreacion = DateTime.UtcNow,
                Detalles = detalles
            };

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
                    new DetallePedidoDto(d.ProductoId, d.Cantidad, d.PrecioUnitario)
                ).ToList()
            );
        }
    }
}
