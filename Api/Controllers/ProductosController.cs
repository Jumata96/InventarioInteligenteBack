using InventarioInteligenteBack.Application.Interfaces;
using InventarioInteligenteBack.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace InventarioInteligenteBack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _service;
        public ProductosController(IProductoService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoReadDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoReadDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<ProductoReadDto>> Create([FromBody] ProductoCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ProductoId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoUpdateDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPatch("{id}/enable")]
        public async Task<IActionResult> Enable(int id)
        {
            var ok = await _service.EnableAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpPatch("{id}/disable")]
        public async Task<IActionResult> Disable(int id)
        {
            var ok = await _service.DisableAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
