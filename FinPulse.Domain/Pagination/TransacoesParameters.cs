using FinPulse.Enums;

namespace FinPulse.Pagination;

public class TransacoesParameters : QueryStringParameters
{
    // Filtros específicos de Transações
    public TipoTransacao? Tipo { get; set; }        // Filtrar por Receita ou Despesa
    public int? CategoriaId { get; set; }            // Filtrar por categoria
    public int? ContaBancariaId { get; set; }        // Filtrar por conta
    public DateTime? DataInicio { get; set; }        // Filtrar a partir de uma data
    public DateTime? DataFim { get; set; }           // Filtrar até uma data
    public decimal? ValorMinimo { get; set; }        // Valor mínimo
    public decimal? ValorMaximo { get; set; }        // Valor máximo
}