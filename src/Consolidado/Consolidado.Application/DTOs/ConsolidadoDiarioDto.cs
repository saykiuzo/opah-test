namespace Consolidado.Application.DTOs;

public record ConsolidadoDiarioResponse
{
    public Guid Id { get; init; }
    public DateTime Data { get; init; }
    public decimal SaldoInicial { get; init; }
    public decimal TotalDebitos { get; init; }
    public decimal TotalCreditos { get; init; }
    public decimal SaldoFinal { get; init; }
    public int QuantidadeTransacoes { get; init; }
    public DateTime DataAtualizacao { get; init; }
}

public record RelatorioRequest
{
    public DateTime DataInicio { get; init; }
    public DateTime DataFim { get; init; }
    public string Formato { get; init; } = "xlsx"; // xlsx ou csv
}