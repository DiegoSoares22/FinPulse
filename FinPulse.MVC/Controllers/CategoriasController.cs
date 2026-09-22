using Microsoft.AspNetCore.Mvc;
using FinPulse.DTOs;
using FinPulse.MVC.Services;

namespace FinPulse.MVC.Controllers;

public class CategoriasController : Controller
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("JWTToken");

        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var categorias = await _categoriaService.ObterTodasAsync(token);
        return View(categorias);
    }
}