using MediatR;
using Consolidado.Application.Queries;
using Consolidado.Application.DTOs;
using Consolidado.Domain.Repositories;

namespace Consolidado.Application.Handlers;

public class ObterConsolidadoPorDataQueryHandler : IRequestHandler<ObterConsolidadoPorDataQuery, ConsolidadoDiarioResponse?>
{
    private readonly IConsolidadoDiarioRepository _repository;

    public ObterConsolidadoPorDataQueryHandler(IConsolidadoDiarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ConsolidadoDiarioResponse?> Handle(ObterConsolidadoPorDataQuery request, CancellationToken cancellationToken)
    {
        var consolidado = await _repository.ObterPorDataAsync(request.Data);

        return consolidado == null ? null : new ConsolidadoDiarioResponse
        {
            Id = consolidado.Id,
            Data = consolidado.Data,
            SaldoInicial = consolidado.SaldoInicial,
            TotalDebitos = consolidado.TotalDebitos,
            TotalCreditos = consolidado.TotalCreditos,
            SaldoFinal = consolidado.SaldoFinal,
            QuantidadeTransacoes = consolidado.QuantidadeTransacoes,
            DataAtualizacao = consolidado.DataAtualizacao
        };
    }
}

public class ObterConsolidadoPorPeriodoQueryHandler : IRequestHandler<ObterConsolidadoPorPeriodoQuery, IEnumerable<ConsolidadoDiarioResponse>>
{
    private readonly IConsolidadoDiarioRepository _repository;

    public ObterConsolidadoPorPeriodoQueryHandler(IConsolidadoDiarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ConsolidadoDiarioResponse>> Handle(ObterConsolidadoPorPeriodoQuery request, CancellationToken cancellationToken)
    {
        var consolidados = await _repository.ObterPorPeriodoAsync(request.DataInicio, request.DataFim);

        return consolidados.Select(c => new ConsolidadoDiarioResponse
        {
            Id = c.Id,
            Data = c.Data,
            SaldoInicial = c.SaldoInicial,
            TotalDebitos = c.TotalDebitos,
            TotalCreditos = c.TotalCreditos,
            SaldoFinal = c.SaldoFinal,
            QuantidadeTransacoes = c.QuantidadeTransacoes,
            DataAtualizacao = c.DataAtualizacao
        });
    }
}