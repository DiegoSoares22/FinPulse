using FinPulse.DTOs;

namespace FinPulse.MVC.Services;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaDTO>?> ObterTodasAsync(string token);
    Task<CategoriaDTO?> ObterPorIdAsync(int id, string token);
    Task<CategoriaDTO?> CriarAsync(CategoriaCreateDTO categoriaDto, string token);
    Task<bool> DeletarAsync(int id, string token);
}