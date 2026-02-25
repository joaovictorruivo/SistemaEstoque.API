using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.API.Data;
using SistemaEstoque.API.Models;

namespace SistemaEstoque.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var produtos = await _context.Produtos.ToListAsync();
        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null)
            return NotFound();
        return Ok(produto);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Produto produtoAtualizado)
    {
        // PROTEÇÃO EXTRA: Verifica se o ID da URL é o mesmo do objeto enviado
        if (id != produtoAtualizado.Id)
        {
            return BadRequest("O ID da URL não coincide com o ID do produto.");
        }

        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null) return NotFound();

        // Seu código de atualização que já está nos prints...
        produto.Nome = produtoAtualizado.Nome;
        produto.Preco = produtoAtualizado.Preco;
        produto.Quantidade = produtoAtualizado.Quantidade;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null)
            return NotFound();

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}