using InventarioInteligenteBack.Api.DTOs;
using InventarioInteligenteBack.Application.Interfaces;
using InventarioInteligenteBack.Domain.Entities;
using InventarioInteligenteBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioInteligenteBack.Application.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly AppDbContext _db;
        private readonly IFacturaPdfService _pdfService;

        public FacturaService(AppDbContext db, IFacturaPdfService pdfService)
        {
            _db = db;
            _pdfService = pdfService;
        }

        // Emitir factura desde un pedido
        public async Task<FacturaReadDto?> EmitirFacturaAsync(int pedidoId)
        {
            var pedido = await _db.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);

            if (pedido == null)
                throw new KeyNotFoundException("Pedido no encontrado");

            if (await _db.Facturas.AnyAsync(f => f.PedidoId == pedidoId))
                throw new InvalidOperationException("El pedido ya tiene factura");

            //ObtenerDatosFacturaPdf(int pedidoId)

            var numeroFactura = $"F{DateTime.UtcNow:yyyyMMddHHmmss}";

            var factura = new Factura
            {
                PedidoId = pedidoId,
                NumeroFactura = numeroFactura,
                FechaEmision = DateTime.UtcNow,
                Subtotal = pedido.Subtotal,
                Descuento = pedido.Descuento,
                Impuesto = pedido.Impuesto,
                Total = pedido.Total,
                Estado = "Emitida",
                UrlPdf = $"/storage/facturas/{numeroFactura}.pdf",
                FechaCreacion = DateTime.UtcNow
            };

            _db.Facturas.Add(factura);
            await _db.SaveChangesAsync();

            // Console.WriteLine($"⚠️ No había factura para PedidoId={factura}, generando nueva...");
            // Console.WriteLine($"⚠️ No había factura para PedidoId={pedido}, generando nueva...");
            // Generar PDF real

            //ObtenerDatosFacturaPdf(factura.FacturaId);
            var datosFactura = await ObtenerDatosFacturaPdf(factura.FacturaId);
            var pdfBytes = _pdfService.GenerarPdf2(datosFactura);

            // Guardar en wwwroot/storage/facturas
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "storage", "facturas");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, $"{numeroFactura}.pdf");
            await File.WriteAllBytesAsync(filePath, pdfBytes);

            // Actualizar URL pública
            factura.UrlPdf = $"/storage/facturas/{numeroFactura}.pdf";
            await _db.SaveChangesAsync();

            return MapToDto(factura);
        }

        public async Task<FacturaReadDto?> GetByPedidoAsync(int pedidoId)
        {
            // Console.WriteLine($"🔍 [FacturaService] Buscando factura para PedidoId={pedidoId}");

            var factura = await _db.Facturas.FirstOrDefaultAsync(f => f.PedidoId == pedidoId);

            if (factura == null)
            {
                // Console.WriteLine($"⚠️ [FacturaService] No se encontró factura para PedidoId={pedidoId}");
                return null;
            }

            // Console.WriteLine($"✅ [FacturaService] Factura encontrada: {factura.NumeroFactura}, UrlPdf={factura.UrlPdf}");
            return MapToDto(factura);
        }

        public async Task<FacturaReadDto?> GetByIdAsync(int id)
        {
            var factura = await _db.Facturas
                .FirstOrDefaultAsync(f => f.FacturaId == id);

            return factura == null ? null : MapToDto(factura);
        }

        public async Task<IEnumerable<FacturaReadDto>> GetAllAsync()
        {
            var facturas = await _db.Facturas
                .OrderByDescending(f => f.FechaEmision)
                .ToListAsync();

            return facturas.Select(MapToDto);
        }

        private static FacturaReadDto MapToDto(Factura f)
        {
            return new FacturaReadDto(
                f.FacturaId,
                f.PedidoId,
                f.NumeroFactura,
                f.FechaEmision,
                f.Subtotal,
                f.Descuento,
                f.Impuesto,
                f.Total,
                f.Estado,
                f.UrlPdf
            );
        }
        public async Task<FacturaReadPdfDto> ObtenerDatosFacturaPdf(int facturaId)
        {
            var factura = await _db.Facturas
                .Include(f => f.Pedido)
                    .ThenInclude(p => p.Cliente)
                .Include(f => f.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(f => f.FacturaId == facturaId);

            if (factura == null)
                throw new Exception($"No se encontró la factura con ID {facturaId}");

            var pedido = factura.Pedido;

            if (pedido == null)
                throw new Exception($"No se encontró el pedido asociado a la factura con ID {facturaId}");

            return new FacturaReadPdfDto
            {
                NumeroFactura = factura.NumeroFactura,
                FechaEmision = factura.FechaEmision,
                Subtotal = factura.Subtotal,
                Descuento = factura.Descuento,
                Impuesto = factura.Impuesto,
                Total = factura.Total,
                Cliente = new ClientePdfDto
                {
                    Nombre = pedido.Cliente?.Nombre ?? "Cliente desconocido",
                    Documento = pedido.Cliente?.Ruc ?? "00000000000",
                    Direccion = pedido.Cliente?.Direccion ?? "Sin dirección"
                },
                Detalles = pedido.Detalles.Select(d => new DetalleFacturaPdfDto
                {
                    Nombre = d.Producto?.Nombre ?? "Producto genérico",
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            };
        }

    }
}
