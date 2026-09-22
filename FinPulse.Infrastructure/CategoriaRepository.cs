using FinPulse.Context;
using FinPulse.Models;

namespace FinPulse.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }
}