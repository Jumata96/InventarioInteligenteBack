using InventarioInteligenteBack.Api.DTOs;
using InventarioInteligenteBack.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventarioInteligenteBack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FacturasController : ControllerBase
    {
        private readonly IFacturaService _facturaService;

        public FacturasController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        // Listar todas las facturas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var facturas = await _facturaService.GetAllAsync();
            return Ok(facturas);
        }

        // Obtener factura por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var factura = await _facturaService.GetByIdAsync(id);
            if (factura == null) return NotFound();
            return Ok(factura);
        }

        // Obtener factura por Pedido
        [HttpGet("pedido/{pedidoId}")]
        public async Task<IActionResult> GetByPedido(int pedidoId)
        {
            Console.WriteLine($"➡️ [FacturasController] GET /api/Facturas/pedido/{pedidoId}");

            var factura = await _facturaService.GetByPedidoAsync(pedidoId);

            if (factura == null)
            {
                Console.WriteLine($"⚠️ No había factura para PedidoId={pedidoId}, generando nueva...");
                factura = await _facturaService.EmitirFacturaAsync(pedidoId);
                if (factura == null)
                {
                    Console.WriteLine($"❌ No se pudo generar factura para PedidoId={pedidoId}");
                    return NotFound();
                }
            }

            Console.WriteLine($"📄 Devolviendo factura {factura.NumeroFactura} con PDF={factura.UrlPdf}");
            return Ok(factura);
        }

        // Emitir factura desde un pedido
        [HttpPost("emitir/{pedidoId}")]
        public async Task<IActionResult> EmitirFactura(int pedidoId)
        {
            try
            {
                var factura = await _facturaService.EmitirFacturaAsync(pedidoId);
                return CreatedAtAction(nameof(GetById), new { id = factura!.FacturaId }, factura);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
