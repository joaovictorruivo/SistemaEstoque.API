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

        // PUT: api/Categorias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Categoria categoriaAtualizada)
        {
            // Verifica se o engraçadinho não mandou IDs diferentes
            if (id != categoriaAtualizada.Id)
            {
                return BadRequest("O ID da URL não coincide com o ID da categoria.");
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            // Atualiza o nome
            categoria.Nome = categoriaAtualizada.Nome;

            await _context.SaveChangesAsync();
            return NoContent(); // 204 - Sucesso, mas não tem nada para retornar na tela
        }

        // DELETE: api/Categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}