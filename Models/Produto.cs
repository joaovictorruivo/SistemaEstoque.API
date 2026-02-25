using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SistemaEstoque.API.Models
{
    public class Produto
    {
        [Key] // Define explicitamente como chave primária
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Range(0.01, 999999, ErrorMessage = "O preço deve ser maior que zero")]
        [Precision(18, 2)] // 18 dígitos no total, com 2 casas decimais (ex: 1500,50)
        public decimal Preco { get; set; }

        [Range(0, 10000, ErrorMessage = "A quantidade não pode ser negativa")]
        public int Quantidade { get; set; }
        public int CategoriaId { get; set; } // Chave Estrangeira
        public Categoria? Categoria { get; set; } // Propriedade de Navegação
    }
}
