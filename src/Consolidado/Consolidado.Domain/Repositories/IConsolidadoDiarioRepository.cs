using Consolidado.Domain.Entities;

namespace Consolidado.Domain.Repositories;

public interface IConsolidadoDiarioRepository
{
    Task<ConsolidadoDiario?> ObterPorDataAsync(DateTime data);
    Task<IEnumerable<ConsolidadoDiario>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<ConsolidadoDiario?> ObterUltimoConsolidadoAsync();
    Task AdicionarAsync(ConsolidadoDiario consolidado);
    Task AtualizarAsync(ConsolidadoDiario consolidado);
    Task<int> SaveChangesAsync();
}