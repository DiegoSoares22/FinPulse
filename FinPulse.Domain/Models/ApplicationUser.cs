using Microsoft.AspNetCore.Identity;

namespace FinPulse.Models;

public class ApplicationUser : IdentityUser
{
    public string? NomeCompleto { get; set; }

    // Campos necessários para o Refresh Token (Aulas 135-136)
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}