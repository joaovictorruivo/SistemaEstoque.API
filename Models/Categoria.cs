using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SistemaEstoque.API.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;

        // O [JsonIgnore] corta o loop infinito!
        [JsonIgnore]
        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
