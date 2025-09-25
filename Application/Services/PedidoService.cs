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
        private readonly IDescuentoService _descuentoService;

        public PedidoService(AppDbContext db, IDescuentoService descuentoService)
        {
            _db = db;
            _descuentoService = descuentoService;
        }

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
                    p.TotalFinal,
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
                p.TotalFinal,
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
                int cantidadTotal = 0;

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
                    cantidadTotal += d.Cantidad;
                    pedido.Detalles.Add(detalle);

                    // Descontar stock
                    producto.Stock -= d.Cantidad;
                }

                pedido.Subtotal = subtotal;

                // ✅ Calcular descuento con el servicio inyectado
                pedido.Descuento = _descuentoService.CalcularDescuento(subtotal, cantidadTotal);

                // ✅ Impuesto
                var impuestoPorcentaje = await _db.Impuestos
                    .Where(i => i.PaisId == dto.PaisId && i.Estado == 1)
                    .Select(i => i.Porcentaje)
                    .FirstOrDefaultAsync();

                var baseImponible = subtotal - pedido.Descuento;
                pedido.Impuesto = baseImponible * (impuestoPorcentaje / 100m);

                // ✅ Totales
                pedido.Total = subtotal; // referencia al bruto
                pedido.TotalFinal = baseImponible + pedido.Impuesto;

                if (pedido.TotalFinal < 0)
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
                    pedido.TotalFinal,
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

        // ✅ Método para la vista: calcular descuento dinámicamente sin crear pedido
        public async Task<(decimal Subtotal, decimal Descuento, decimal Total)> CalcularDescuentoAsync(PedidoCreateDto dto)
        {
            decimal subtotal = 0;
            int totalUnidades = 0;

            foreach (var d in dto.Detalles)
            {
                var producto = await _db.Productos.FirstOrDefaultAsync(p => p.ProductoId == d.ProductoId);
                if (producto == null)
                    throw new KeyNotFoundException($"Producto {d.ProductoId} no existe");

                subtotal += producto.Precio * d.Cantidad;
                totalUnidades += d.Cantidad;
            }

            var descuento = _descuentoService.CalcularDescuento(subtotal, totalUnidades);
            var total = subtotal - descuento;

            return (subtotal, descuento, total);
        }
    }
}
