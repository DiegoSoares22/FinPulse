using System.ComponentModel.DataAnnotations;

namespace FinPulse.DTOs;

public class RegisterDTO
{
    [Required(ErrorMessage = "O nome completo é obrigatório")]
    public string? NomeCompleto { get; set; }

    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    public string? UserName { get; set; }

    [EmailAddress(ErrorMessage = "E-mail em formato inválido")]
    [Required(ErrorMessage = "O e-mail é obrigatório")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
    public string? Password { get; set; }
}