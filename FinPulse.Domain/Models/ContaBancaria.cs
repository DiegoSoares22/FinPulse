using FinPulse.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinPulse.Models;

public class ContaBancaria
{
    public ContaBancaria()
    {
        Transacoes = new Collection<Transacao>();
    }

    [Key]
    public int ContaBancariaId { get; set; }

    [Required(ErrorMessage = "O nome da conta é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
    public string? Nome { get; set; } // Ex: "Nubank", "Itaú Corrente"

    [Required]
    public TipoConta Tipo { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Saldo { get; set; }

    [StringLength(50)]
    public string? Banco { get; set; } // Ex: "Nubank", "Itaú"

    [StringLength(10)]
    public string? Cor { get; set; }

    [StringLength(50)]
    public string? Icone { get; set; }

    public bool Ativa { get; set; } = true; // Valor padrão

    // Propriedade de navegação
    [JsonIgnore]
    public ICollection<Transacao>? Transacoes { get; set; }
}