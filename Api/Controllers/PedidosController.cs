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
        public PedidosController(IPedidoService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pedido = await _service.GetByIdAsync(id);
            return pedido == null ? NotFound() : Ok(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PedidoCreateDto dto)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
            var created = await _service.CreateAsync(dto, usuarioId);
            return CreatedAtAction(nameof(GetById), new { id = created.PedidoId }, created);
        }
    }
}
