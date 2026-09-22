namespace FinPulse.DTOs;

public class ContaBancariaDTO
{
    public int ContaBancariaId { get; set; }
    public string? Nome { get; set; }
    public string? Tipo { get; set; } // "Corrente", "Poupanca", etc.
    public decimal Saldo { get; set; }
    public string? Banco { get; set; }
    public string? Cor { get; set; }
    public string? Icone { get; set; }
    public bool Ativa { get; set; }
}