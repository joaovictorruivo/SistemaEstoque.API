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

    // GET: api/Produtos?numeroPagina=1&tamanhoPagina=5
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int numeroPagina = 1, [FromQuery] int tamanhoPagina = 10)
    {
        // Travas de segurança: não existe página 0 e ninguém pode pedir 1 milhão de itens de uma vez
        if (numeroPagina < 1) numeroPagina = 1;
        if (tamanhoPagina < 1) tamanhoPagina = 1;
        if (tamanhoPagina > 50) tamanhoPagina = 50; // Limite máximo por página

        // Conta o total de produtos para avisar o Front-end
        var totalItens = await _context.Produtos.CountAsync();
        var totalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina);

        // Faz a busca usando Skip e Take
        var produtos = await _context.Produtos
                                     .Include(p => p.Categoria)
                                     .Skip((numeroPagina - 1) * tamanhoPagina)
                                     .Take(tamanhoPagina)
                                     .ToListAsync();

        // Retorna os dados empacotados com as informações da paginação (Recrutador ama isso!)
        return Ok(new
        {
            TotalItens = totalItens,
            TotalPaginas = totalPaginas,
            PaginaAtual = numeroPagina,
            TamanhoPagina = tamanhoPagina,
            Dados = produtos
        });
    }

    // GET: api/Produtos/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // Procura o produto pelo ID e já traz a Categoria associada
        var produto = await _context.Produtos
                                    .Include(p => p.Categoria)
                                    .FirstOrDefaultAsync(p => p.Id == id);

        // Se o ID não existir, devolve um erro 404
        if (produto == null)
        {
            return NotFound("Produto não encontrado.");
        }

        return Ok(produto);
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

    // GET: api/Produtos/Categoria/1
    [HttpGet("Categoria/{categoriaId}")]
    public async Task<IActionResult> GetByCategoria(int categoriaId)
    {
        // Vamos buscar na tabela de Produtos onde o CategoriaId seja igual ao que o usuário pediu
        var produtos = await _context.Produtos
                                     .Include(p => p.Categoria)
                                     .Where(p => p.CategoriaId == categoriaId)
                                     .ToListAsync();

        // Se a lista vier vazia, retornamos um 404 amigável
        if (!produtos.Any())
        {
            return NotFound("Nenhum produto encontrado para esta categoria.");
        }

        return Ok(produtos);
    }

    // PUT: api/Produtos/5/BaixaEstoque
    [HttpPut("{id}/BaixaEstoque")]
    public async Task<IActionResult> BaixaEstoque(int id, [FromBody] int quantidadeVendida)
    {
        var produto = await _context.Produtos.FindAsync(id);

        // Se o produto não existir, retorna erro 404
        if (produto == null)
        {
            return NotFound("Produto não encontrado.");
        }

        // Regra de Negócio: Não pode vender mais do que tem no estoque!
        if (produto.Quantidade < quantidadeVendida)
        {
            return BadRequest($"Estoque insuficiente. Temos apenas {produto.Quantidade} unidades.");
        }

        // Subtrai a quantidade vendida do estoque atual
        produto.Quantidade -= quantidadeVendida;

        await _context.SaveChangesAsync();

        // Retorna uma mensagem de sucesso bacana
        return Ok(new { mensagem = "Venda realizada com sucesso!", estoqueAtual = produto.Quantidade });
    }
    // GET: api/Produtos/Busca?nome=mouse
    [HttpGet("Busca")]
    public async Task<IActionResult> BuscarPorNome([FromQuery] string nome)
    {
        // Verifica se o usuário mandou o texto vazio
        if (string.IsNullOrWhiteSpace(nome))
        {
            return BadRequest("O termo de busca não pode ser vazio.");
        }

        // Busca no banco produtos que contenham aquele texto no nome
        var produtos = await _context.Produtos
                                     .Include(p => p.Categoria)
                                     .Where(p => p.Nome.Contains(nome))
                                     .ToListAsync();

        if (!produtos.Any())
        {
            return NotFound("Nenhum produto encontrado com esse nome.");
        }

        return Ok(produtos);
    }

    // GET: api/Produtos/FiltroPreco?min=50&max=200
    [HttpGet("FiltroPreco")]
    public async Task<IActionResult> FiltrarPorPreco([FromQuery] decimal min, [FromQuery] decimal max)
    {
        // Regra de negócio: O mínimo não pode ser maior que o máximo
        if (min > max)
        {
            return BadRequest("O preço mínimo não pode ser maior que o preço máximo.");
        }

        // Busca no banco os produtos que estão dentro dessa faixa de preço
        var produtos = await _context.Produtos
                                     .Include(p => p.Categoria)
                                     .Where(p => p.Preco >= min && p.Preco <= max)
                                     .ToListAsync();

        if (!produtos.Any())
        {
            return NotFound("Nenhum produto encontrado nessa faixa de preço.");
        }

        return Ok(produtos);
    }

    // GET: api/Produtos/Estatisticas
    [HttpGet("Estatisticas")]
    public async Task<IActionResult> GetEstatisticas()
    {
        // Conta quantos produtos existem no banco
        var totalProdutos = await _context.Produtos.CountAsync();

        // Calcula o valor total do estoque (Preço x Quantidade de cada produto)
        var valorTotalEstoque = await _context.Produtos.SumAsync(p => p.Preco * p.Quantidade);

        // Retorna um objeto anônimo formatado bonitinho
        return Ok(new
        {
            TotalProdutosCadastrados = totalProdutos,
            ValorTotalDoEstoque = valorTotalEstoque
        });
    }
}