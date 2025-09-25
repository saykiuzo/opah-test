using MediatR;
using Lancamentos.Application.DTOs;
using Lancamentos.Domain.Entities;

namespace Lancamentos.Application.Commands;

public class AtualizarLancamentoCommand : IRequest<LancamentoResponse>
{
    public Guid Id { get; set; }
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public TipoLancamento Tipo { get; set; }
    public string Descricao { get; set; } = string.Empty;
}