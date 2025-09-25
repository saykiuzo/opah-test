using Lancamentos.Domain.Entities;

namespace Lancamentos.Domain.Repositories;

public interface ILancamentoRepository
{
    Task<Lancamento?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Lancamento>> ObterTodosAsync();
    Task<IEnumerable<Lancamento>> ObterPorDataAsync(DateTime data);
    Task<IEnumerable<Lancamento>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task AdicionarAsync(Lancamento lancamento);
    Task AtualizarAsync(Lancamento lancamento);
    Task RemoverAsync(Guid id);
    Task<int> SaveChangesAsync();
}