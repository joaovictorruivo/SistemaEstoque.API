using global::SistemaEstoque.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace SistemaEstoque.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private static List<Produto> produtos = new List<Produto>();

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var produto = produtos.FirstOrDefault(p => p.Id == id);

        if (produto == null)
            return NotFound();

        return Ok(produto);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Produto produto)
    {
        produto.Id = produtos.Count + 1;
        produtos.Add(produto);

        return CreatedAtAction(nameof(Get), new { id = produto.Id }, produto);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] Produto produtoAtualizado)
    {
        var produto = produtos.FirstOrDefault(p => p.Id == id);

        if (produto == null)
            return NotFound();

        produto.Nome = produtoAtualizado.Nome;
        produto.Preco = produtoAtualizado.Preco;
        produto.Quantidade = produtoAtualizado.Quantidade;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var produto = produtos.FirstOrDefault(p => p.Id == id);

        if (produto == null)
            return NotFound();

        produtos.Remove(produto);

        return NoContent();
    }
    
}