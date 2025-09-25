using MediatR;
using Lancamentos.Application.Commands;
using Lancamentos.Domain.Repositories;

namespace Lancamentos.Application.Handlers;

public class RemoverLancamentoCommandHandler : IRequestHandler<RemoverLancamentoCommand, bool>
{
    private readonly ILancamentoRepository _repository;

    public RemoverLancamentoCommandHandler(ILancamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(RemoverLancamentoCommand request, CancellationToken cancellationToken)
    {
        var lancamento = await _repository.ObterPorIdAsync(request.Id);
        
        if (lancamento == null)
        {
            return false;
        }

        await _repository.RemoverAsync(request.Id);
        await _repository.SaveChangesAsync();

        return true;
    }
}