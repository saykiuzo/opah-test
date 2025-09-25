using MediatR;
using Consolidado.Application.DTOs;

namespace Consolidado.Application.Queries;

public record ObterConsolidadoPorDataQuery : IRequest<ConsolidadoDiarioResponse?>
{
    public DateTime Data { get; init; }
}

public record ObterConsolidadoPorPeriodoQuery : IRequest<IEnumerable<ConsolidadoDiarioResponse>>
{
    public DateTime DataInicio { get; init; }
    public DateTime DataFim { get; init; }
}

public record GerarRelatorioQuery : IRequest<byte[]>
{
    public DateTime DataInicio { get; init; }
    public DateTime DataFim { get; init; }
    public string Formato { get; init; } = "xlsx";
}