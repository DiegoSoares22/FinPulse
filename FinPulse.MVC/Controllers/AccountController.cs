using Microsoft.AspNetCore.Mvc;
using FinPulse.DTOs;
using FinPulse.MVC.Services;

namespace FinPulse.MVC.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var token = await _authService.LoginAsync(model);

        if (string.IsNullOrEmpty(token))
        {
            ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
            return View(model);
        }

        // Guarda o token JWT na Session do usuário
        HttpContext.Session.SetString("JWTToken", token);

        return RedirectToAction("Index", "Categorias");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}