using FinPulse.Enums;

namespace FinPulse.Pagination;

public class CategoriasParameters : QueryStringParameters
{
    public TipoTransacao? Tipo { get; set; } // Filtrar por Receita ou Despesa
}