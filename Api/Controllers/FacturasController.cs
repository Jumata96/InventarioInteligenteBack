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
            var factura = await _facturaService.GetByPedidoAsync(pedidoId);
            if (factura == null) return NotFound();
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
