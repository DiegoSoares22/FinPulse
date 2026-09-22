using System.ComponentModel.DataAnnotations;
using FinPulse.Enums;

namespace FinPulse.DTOs;

public class CategoriaCreateDTO
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(80, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
    public string? Nome { get; set; }

    [Required]
    public TipoTransacao Tipo { get; set; }

    [StringLength(50)]
    public string? Icone { get; set; }

    [StringLength(10)]
    public string? Cor { get; set; }
}