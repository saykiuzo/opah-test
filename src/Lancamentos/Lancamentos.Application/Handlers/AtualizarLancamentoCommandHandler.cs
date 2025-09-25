using MediatR;
using Lancamentos.Application.Commands;
using Lancamentos.Application.DTOs;
using Lancamentos.Domain.Repositories;
using Lancamentos.Domain.Entities;

namespace Lancamentos.Application.Handlers;

public class AtualizarLancamentoCommandHandler : IRequestHandler<AtualizarLancamentoCommand, LancamentoResponse>
{
    private readonly ILancamentoRepository _repository;

    public AtualizarLancamentoCommandHandler(ILancamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<LancamentoResponse> Handle(AtualizarLancamentoCommand request, CancellationToken cancellationToken)
    {
        var lancamento = await _repository.ObterPorIdAsync(request.Id);
        
        if (lancamento == null)
        {
            throw new ArgumentException($"Lançamento com ID {request.Id} não encontrado");
        }

        lancamento.AtualizarDados(request.Data, request.Valor, request.Tipo, request.Descricao);

        await _repository.AtualizarAsync(lancamento);
        await _repository.SaveChangesAsync();

        return new LancamentoResponse
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