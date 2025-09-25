using MediatR;
using Lancamentos.Application.Queries;
using Lancamentos.Application.DTOs;
using Lancamentos.Domain.Repositories;

namespace Lancamentos.Application.Handlers;

public class ObterLancamentosQueryHandler : IRequestHandler<ObterLancamentosQuery, IEnumerable<LancamentoResponse>>
{
    private readonly ILancamentoRepository _repository;

    public ObterLancamentosQueryHandler(ILancamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LancamentoResponse>> Handle(ObterLancamentosQuery request, CancellationToken cancellationToken)
    {
        var lancamentos = request.DataInicio.HasValue && request.DataFim.HasValue
            ? await _repository.ObterPorPeriodoAsync(request.DataInicio.Value, request.DataFim.Value)
            : await _repository.ObterTodosAsync();

        return lancamentos.Select(l => new LancamentoResponse
        {
            Id = l.Id,
            Data = l.Data,
            Valor = l.Valor,
            Tipo = l.Tipo.ToString(),
            Descricao = l.Descricao,
            DataCriacao = l.DataCriacao
        });
    }
}

public class ObterLancamentoPorIdQueryHandler : IRequestHandler<ObterLancamentoPorIdQuery, LancamentoResponse?>
{
    private readonly ILancamentoRepository _repository;

    public ObterLancamentoPorIdQueryHandler(ILancamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<LancamentoResponse?> Handle(ObterLancamentoPorIdQuery request, CancellationToken cancellationToken)
    {
        var lancamento = await _repository.ObterPorIdAsync(request.Id);

        return lancamento == null ? null : new LancamentoResponse
        {
            Id = lancamento.Id,
            Data = lancamento.Data,
            Valor = lancamento.Valor,
            Tipo = lancamento.Tipo.ToString(),
            Descricao = lancamento.Descricao,
            DataCriacao = lancamento.DataCriacao
        };
    }
}