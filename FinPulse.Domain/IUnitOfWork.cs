namespace FinPulse.Repositories;

public interface IUnitOfWork : IDisposable
{
    ICategoriaRepository CategoriaRepository { get; }
    IContaBancariaRepository ContaBancariaRepository { get; }
    ITransacaoRepository TransacaoRepository { get; }

    Task CommitAsync(); // Era Commit(), agora é assíncrono
}