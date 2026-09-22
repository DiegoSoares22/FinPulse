using FinPulse.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinPulse.Models;

public class Orcamento
{
    [Key]
    public int OrcamentoId { get; set; }

    [Required(ErrorMessage = "O valor limite é obrigatório")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorLimite { get; set; }

    [Required]
    [Range(1, 12, ErrorMessage = "O mês deve estar entre {1} e {2}")]
    public int Mes { get; set; }

    [Required]
    [Range(2020, 2100, ErrorMessage = "Ano inválido")]
    public int Ano { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorGasto { get; set; } = 0;

    // Chave estrangeira
    [Required]
    public int CategoriaId { get; set; }

    // Propriedade de navegação
    [JsonIgnore]
    public Categoria? Categoria { get; set; }
}