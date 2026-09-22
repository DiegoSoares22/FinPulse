using System.ComponentModel.DataAnnotations;
using FinPulse.Enums;

namespace FinPulse.DTOs;

public class ContaBancariaCreateDTO
{
    [Required(ErrorMessage = "O nome da conta é obrigatório")]
    [StringLength(100)]
    public string? Nome { get; set; }

    [Required]
    public TipoConta Tipo { get; set; }

    [Required]
    public decimal Saldo { get; set; }

    [StringLength(50)]
    public string? Banco { get; set; }

    [StringLength(10)]
    public string? Cor { get; set; }

    [StringLength(50)]
    public string? Icone { get; set; }
}