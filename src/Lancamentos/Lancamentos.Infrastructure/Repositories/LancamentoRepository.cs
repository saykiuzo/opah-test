using Microsoft.EntityFrameworkCore;
using Lancamentos.Domain.Entities;
using Lancamentos.Domain.Repositories;
using Lancamentos.Infrastructure.Data;

namespace Lancamentos.Infrastructure.Repositories;

public class LancamentoRepository : ILancamentoRepository
{
    private readonly LancamentosDbContext _context;

    public LancamentoRepository(LancamentosDbContext context)
    {
        _context = context;
    }

    public async Task<Lancamento?> ObterPorIdAsync(Guid id)
    {
        return await _context.Lancamentos.FindAsync(id);
    }

    public async Task<IEnumerable<Lancamento>> ObterTodosAsync()
    {
        return await _context.Lancamentos
            .OrderByDescending(l => l.Data)
            .ThenByDescending(l => l.DataCriacao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Lancamento>> ObterPorDataAsync(DateTime data)
    {
        return await _context.Lancamentos
            .Where(l => l.Data.Date == data.Date)
            .OrderByDescending(l => l.DataCriacao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Lancamento>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _context.Lancamentos
            .Where(l => l.Data.Date >= dataInicio.Date && l.Data.Date <= dataFim.Date)
            .OrderByDescending(l => l.Data)
            .ThenByDescending(l => l.DataCriacao)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Lancamento lancamento)
    {
        await _context.Lancamentos.AddAsync(lancamento);
    }

    public Task AtualizarAsync(Lancamento lancamento)
    {
        _context.Lancamentos.Update(lancamento);
        return Task.CompletedTask;
    }

    public async Task RemoverAsync(Guid id)
    {
        var lancamento = await _context.Lancamentos.FindAsync(id);
        if (lancamento != null)
        {
            _context.Lancamentos.Remove(lancamento);
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}