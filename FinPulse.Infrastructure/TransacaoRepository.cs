using Microsoft.EntityFrameworkCore;
using FinPulse.Context;
using FinPulse.Models;
using FinPulse.Pagination;

namespace FinPulse.Repositories;

public class TransacaoRepository : Repository<Transacao>, ITransacaoRepository
{
    public TransacaoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedList<Transacao>> GetTransacoesPaginadasAsync(TransacoesParameters parameters)
    {
        var query = _context.Transacoes
            .Include(t => t.Categoria)
            .Include(t => t.ContaBancaria)
            .AsNoTracking()
            .AsQueryable();

        if (parameters.Tipo.HasValue)
            query = query.Where(t => t.Tipo == parameters.Tipo.Value);

        if (parameters.CategoriaId.HasValue)
            query = query.Where(t => t.CategoriaId == parameters.CategoriaId.Value);

        if (parameters.ContaBancariaId.HasValue)
            query = query.Where(t => t.ContaBancariaId == parameters.ContaBancariaId.Value);

        if (parameters.DataInicio.HasValue)
            query = query.Where(t => t.Data >= parameters.DataInicio.Value);

        if (parameters.DataFim.HasValue)
            query = query.Where(t => t.Data <= parameters.DataFim.Value);

        if (parameters.ValorMinimo.HasValue)
            query = query.Where(t => t.Valor >= parameters.ValorMinimo.Value);

        if (parameters.ValorMaximo.HasValue)
            query = query.Where(t => t.Valor <= parameters.ValorMaximo.Value);

        query = query.OrderByDescending(t => t.Data);

        return await PagedList<Transacao>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<Transacao?> GetTransacaoComDetalhesAsync(int id)
    {
        return await _context.Transacoes
            .Include(t => t.Categoria)
            .Include(t => t.ContaBancaria)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TransacaoId == id);
    }
}