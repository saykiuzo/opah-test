namespace Lancamentos.Application.DTOs;

public record CriarLancamentoRequest
{
    public DateTime Data { get; init; }
    public decimal Valor { get; init; }
    public int Tipo { get; init; } // 0 = Débito, 1 = Crédito
    public string Descricao { get; init; } = string.Empty;
}

public record AtualizarLancamentoRequest
{
    public DateTime Data { get; init; }
    public decimal Valor { get; init; }
    public int Tipo { get; init; } // 0 = Débito, 1 = Crédito
    public string Descricao { get; init; } = string.Empty;
}

public record LancamentoResponse
{
    public Guid Id { get; init; }
    public DateTime Data { get; init; }
    public decimal Valor { get; init; }
    public string Tipo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public DateTime DataCriacao { get; init; }
}