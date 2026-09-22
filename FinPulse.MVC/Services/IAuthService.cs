using FinPulse.DTOs;

namespace FinPulse.MVC.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(LoginDTO loginDto);
    Task<bool> RegisterAsync(RegisterDTO registerDto);
}