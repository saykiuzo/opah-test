using MediatR;
using Lancamentos.Application.Commands;
using Lancamentos.Application.DTOs;
using Lancamentos.Domain.Entities;
using Lancamentos.Domain.Repositories;
using FluxoCaixa.Shared.Events;
using FluxoCaixa.Shared.Messaging;

namespace Lancamentos.Application.Handlers;

public class CriarLancamentoCommandHandler : IRequestHandler<CriarLancamentoCommand, LancamentoResponse>
{
    private readonly ILancamentoRepository _repository;
    private readonly IMessageBus _messageBus;

    public CriarLancamentoCommandHandler(ILancamentoRepository repository, IMessageBus messageBus)
    {
        _repository = repository;
        _messageBus = messageBus;
    }

    public async Task<LancamentoResponse> Handle(CriarLancamentoCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[CriarLancamentoHandler] Iniciando criação do lançamento - Data: {request.Data}, Valor: {request.Valor}");
        
        var lancamento = new Lancamento(
            request.Data,
            request.Valor,
            (Lancamentos.Domain.Entities.TipoLancamento)request.Tipo,
            request.Descricao
        );

        await _repository.AdicionarAsync(lancamento);
        await _repository.SaveChangesAsync();

        Console.WriteLine($"[CriarLancamentoHandler] Lançamento salvo no banco - ID: {lancamento.Id}");

        var evento = new LancamentoCriadoEvent
        {
            Id = lancamento.Id,
            Data = lancamento.Data,
            Valor = lancamento.Valor,
            Tipo = (FluxoCaixa.Shared.Events.TipoLancamento)lancamento.Tipo,
            Descricao = lancamento.Descricao
        };

        Console.WriteLine($"[CriarLancamentoHandler] Publicando evento - ID: {evento.Id}, Routing Key: 'lancamento-criado'");
        _messageBus.Publish(evento, "lancamento-criado");
        Console.WriteLine($"[CriarLancamentoHandler] Evento publicado com sucesso!");

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