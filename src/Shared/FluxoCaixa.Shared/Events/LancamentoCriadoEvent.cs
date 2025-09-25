namespace FluxoCaixa.Shared.Events;

public record LancamentoCriadoEvent
{
    public Guid Id { get; init; }
    public DateTime Data { get; init; }
    public decimal Valor { get; init; }
    public TipoLancamento Tipo { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public DateTime DataCriacao { get; init; } = DateTime.UtcNow;
}

public enum TipoLancamento
{
    Debito = 0,
    Credito = 1
}