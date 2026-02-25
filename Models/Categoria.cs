using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.API.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        // Um produto pode ter várias categorias? Não, aqui vamos dizer que 
        // uma categoria pode ter vários produtos (1 para N)
        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
