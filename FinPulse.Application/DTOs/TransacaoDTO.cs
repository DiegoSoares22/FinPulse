namespace FinPulse.DTOs;

public class TransacaoDTO
{
    public int TransacaoId { get; set; }
    public string? Descricao { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public string? Tipo { get; set; } // "Receita" ou "Despesa"
    public string? Observacao { get; set; }

    // Dados simplificados dos relacionamentos (sem objeto inteiro!)
    public int CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }  // Ex: "Alimentação"
    public string? CategoriaCor { get; set; }   // Ex: "#EF4444"

    public int ContaBancariaId { get; set; }
    public string? ContaBancariaNome { get; set; } // Ex: "Nubank"
}