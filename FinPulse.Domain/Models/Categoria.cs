using FinPulse.Models;
using FinPulse.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinPulse.Models;

public class Categoria
{
    // Construtor: inicializa a coleção para evitar NullReferenceException
    public Categoria()
    {
        Transacoes = new Collection<Transacao>();
        Orcamentos = new Collection<Orcamento>();
    }

    [Key] // Define como chave primária (PK)
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(80, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
    public string? Nome { get; set; }

    [Required]
    public TipoTransacao Tipo { get; set; } // Receita ou Despesa

    [StringLength(50)]
    public string? Icone { get; set; } // Ex: "fa-utensils", "fa-car"

    [StringLength(10)]
    public string? Cor { get; set; } // Ex: "#FF5733"

    // Propriedades de navegação (relacionamentos)
    [JsonIgnore] // Evita referência cíclica na serialização JSON
    public ICollection<Transacao>? Transacoes { get; set; }

    [JsonIgnore]
    public ICollection<Orcamento>? Orcamentos { get; set; }
}