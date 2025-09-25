using MediatR;

namespace Lancamentos.Application.Commands;

public class RemoverLancamentoCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}