namespace FinPulse.Pagination;

public abstract class QueryStringParameters
{
    // Limite máximo para evitar que alguém peça 1 milhão de registros
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}