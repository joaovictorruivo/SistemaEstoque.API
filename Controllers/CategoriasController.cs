using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.API.Data;
using SistemaEstoque.API.Models;

namespace SistemaEstoque.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Categorias
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categorias = await _context.Categorias.ToListAsync();
            return Ok(categorias);
        }

        // POST: api/Categorias
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            // Retorna 201 Created
            return CreatedAtAction(nameof(Get), new { id = categoria.Id }, categoria);
        }
    }
}