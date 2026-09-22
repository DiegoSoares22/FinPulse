namespace FinPulse.DTOs;

public class CategoriaDTO
{
    public int CategoriaId { get; set; }
    public string? Nome { get; set; }
    public string? Tipo { get; set; } // Retorna "Receita" ou "Despesa" como texto legível
    public string? Icone { get; set; }
    public string? Cor { get; set; }
}