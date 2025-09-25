using MediatR;
using Lancamentos.Application.DTOs;

namespace Lancamentos.Application.Commands;

public record CriarLancamentoCommand : IRequest<LancamentoResponse>
{
    public DateTime Data { get; init; }
    public decimal Valor { get; init; }
    public int Tipo { get; init; }
    public string Descricao { get; init; } = string.Empty;
}