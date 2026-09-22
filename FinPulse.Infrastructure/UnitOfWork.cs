using FinPulse.Context;
using FinPulse.Repositories;

namespace FinPulse.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ICategoriaRepository? _categoriaRepo;
    private IContaBancariaRepository? _contaRepo;
    private ITransacaoRepository? _transacaoRepo;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ICategoriaRepository CategoriaRepository =>
        _categoriaRepo ??= new CategoriaRepository(_context);

    public IContaBancariaRepository ContaBancariaRepository =>
        _contaRepo ??= new ContaBancariaRepository(_context);

    public ITransacaoRepository TransacaoRepository =>
        _transacaoRepo ??= new TransacaoRepository(_context);

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}