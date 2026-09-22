using System.ComponentModel.DataAnnotations;
using FinPulse.Enums;
using FinPulse.Validations;

namespace FinPulse.DTOs;

public class TransacaoCreateDTO
{
    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(200)]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O valor é obrigatório")]
    public decimal Valor { get; set; }

    [Required]
    [DataNaoFutura(ErrorMessage = "A data não pode estar no futuro")]
    public DateTime Data { get; set; }

    [Required]
    public TipoTransacao Tipo { get; set; }

    [StringLength(500)]
    public string? Observacao { get; set; }

    [Required]
    public int CategoriaId { get; set; }

    [Required]
    public int ContaBancariaId { get; set; }
}