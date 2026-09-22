using FinPulse.Context;
using FinPulse.Models;

namespace FinPulse.Repositories;

public class ContaBancariaRepository : Repository<ContaBancaria>, IContaBancariaRepository
{
    public ContaBancariaRepository(AppDbContext context) : base(context)
    {
    }
}