using FinPulse.Models;
using FinPulse.Pagination;

namespace FinPulse.Repositories;

public interface ITransacaoRepository : IRepository<Transacao>
{
    Task<PagedList<Transacao>> GetTransacoesPaginadasAsync(TransacoesParameters parameters);
    Task<Transacao?> GetTransacaoComDetalhesAsync(int id);
}