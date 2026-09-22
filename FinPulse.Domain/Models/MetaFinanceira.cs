using FinPulse.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinPulse.Models;

public class MetaFinanceira
{
    [Key]
    public int MetaFinanceiraId { get; set; }

    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(150, ErrorMessage = "O título deve ter no máximo {1} caracteres")]
    public string? Titulo { get; set; }

    [StringLength(500)]
    public string? Descricao { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorAlvo { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorAtual { get; set; } = 0;

    [Required]
    public DateTime DataInicio { get; set; }

    public DateTime? DataLimite { get; set; } // Nullable — nem toda meta tem prazo

    [Required]
    public StatusMeta Status { get; set; } = StatusMeta.EmAndamento;
}