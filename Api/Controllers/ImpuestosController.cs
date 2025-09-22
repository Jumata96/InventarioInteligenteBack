using InventarioInteligenteBack.Api.DTOs;
using InventarioInteligenteBack.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventarioInteligenteBack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImpuestosController : ControllerBase
    {
        private readonly IImpuestoService _service;

        public ImpuestosController(IImpuestoService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var impuesto = await _service.GetByIdAsync(id);
            return impuesto == null ? NotFound() : Ok(impuesto);
        }

        [HttpGet("pais/{paisId}")]
        public async Task<IActionResult> GetByPais(int paisId) =>
            Ok(await _service.GetByPaisAsync(paisId));

        [HttpPost]
        public async Task<IActionResult> Create(ImpuestoCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ImpuestoId }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
