using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FinPulse.Enums;
using FinPulse.Validations; // <-- Importante para reconhecer a validação customizada

namespace FinPulse.Models;

public class Transacao
{
    [Key]
    public int TransacaoId { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O valor é obrigatório")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Valor { get; set; }

    // Propriedade com a validação customizada [DataNaoFutura]
    [Required(ErrorMessage = "A data é obrigatória")]
    [DataNaoFutura(ErrorMessage = "A data da transação não pode estar no futuro")]
    public DateTime Data { get; set; }

    [Required]
    public TipoTransacao Tipo { get; set; }

    [StringLength(500)]
    public string? Observacao { get; set; }

    // Chaves estrangeiras (FK)
    [Required]
    public int CategoriaId { get; set; }

    [Required]
    public int ContaBancariaId { get; set; }

    // Propriedades de navegação
    [JsonIgnore]
    public Categoria? Categoria { get; set; }

    [JsonIgnore]
    public ContaBancaria? ContaBancaria { get; set; }
}