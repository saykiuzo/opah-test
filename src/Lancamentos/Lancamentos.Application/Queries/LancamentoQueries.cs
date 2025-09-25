using MediatR;
using Lancamentos.Application.DTOs;

namespace Lancamentos.Application.Queries;

public record ObterLancamentosQuery : IRequest<IEnumerable<LancamentoResponse>>
{
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
}

public record ObterLancamentoPorIdQuery : IRequest<LancamentoResponse?>
{
    public Guid Id { get; init; }
}