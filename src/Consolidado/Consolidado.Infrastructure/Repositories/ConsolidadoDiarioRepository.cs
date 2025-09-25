using Microsoft.EntityFrameworkCore;
using Consolidado.Domain.Entities;
using Consolidado.Domain.Repositories;
using Consolidado.Infrastructure.Data;

namespace Consolidado.Infrastructure.Repositories;

public class ConsolidadoDiarioRepository : IConsolidadoDiarioRepository
{
    private readonly ConsolidadoDbContext _context;

    public ConsolidadoDiarioRepository(ConsolidadoDbContext context)
    {
        _context = context;
    }

    public async Task<ConsolidadoDiario?> ObterPorDataAsync(DateTime data)
    {
        return await _context.ConsolidadosDiarios
            .FirstOrDefaultAsync(c => c.Data.Date == data.Date);
    }

    public async Task<ConsolidadoDiario?> ObterPorDataComLockAsync(DateTime data)
    {
        return await _context.ConsolidadosDiarios
            .FirstOrDefaultAsync(c => c.Data.Date == data.Date);
    }

    public async Task<IEnumerable<ConsolidadoDiario>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _context.ConsolidadosDiarios
            .Where(c => c.Data.Date >= dataInicio.Date && c.Data.Date <= dataFim.Date)
            .OrderBy(c => c.Data)
            .ToListAsync();
    }

    public async Task<ConsolidadoDiario?> ObterUltimoConsolidadoAsync()
    {
        return await _context.ConsolidadosDiarios
            .OrderByDescending(c => c.Data)
            .FirstOrDefaultAsync();
    }

    public async Task AdicionarAsync(ConsolidadoDiario consolidado)
    {
        await _context.ConsolidadosDiarios.AddAsync(consolidado);
    }

    public Task AtualizarAsync(ConsolidadoDiario consolidado)
    {
        _context.ConsolidadosDiarios.Update(consolidado);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}