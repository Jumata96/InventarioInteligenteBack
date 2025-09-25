using InventarioInteligenteBack.Api.DTOs;
using InventarioInteligenteBack.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventarioInteligenteBack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _service;
        private readonly IDescuentoService _descuentoService;

        public PedidosController(IPedidoService service, IDescuentoService descuentoService)
        {
            _service = service;
            _descuentoService = descuentoService;
        }

        // GET: api/Pedidos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pedidos = await _service.GetAllAsync();
            return Ok(pedidos);
        }

        // GET: api/Pedidos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pedido = await _service.GetByIdAsync(id);
            if (pedido == null) return NotFound();

            return Ok(pedido);
        }

        // POST: api/Pedidos
        [HttpPost]
        public async Task<IActionResult> Create(PedidoCreateDto dto)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId == null) return Unauthorized();

            var created = await _service.CreateAsync(dto, usuarioId);
            if (created == null) return BadRequest("No se pudo crear el pedido");

            return CreatedAtAction(nameof(GetById), new { id = created.PedidoId }, created);
        }

        // POST: api/Pedidos/calcular-descuento
        [HttpPost("calcular-descuento")]
        public async Task<IActionResult> CalcularDescuento([FromBody] PedidoCreateDto dto)
        {
            if (dto.Detalles == null || !dto.Detalles.Any())
                return BadRequest("Debe enviar al menos un producto");

            var result = await _service.CalcularDescuentoAsync(dto);

            return Ok(new
            {
                Subtotal = result.Subtotal,
                Descuento = result.Descuento,
                Total = result.Total
            });
        }


    }
}
