namespace Consolidado.Domain.Entities;

public class ConsolidadoDiario
{
    public Guid Id { get; private set; }
    public DateTime Data { get; private set; }
    public decimal SaldoInicial { get; private set; }
    public decimal TotalDebitos { get; private set; }
    public decimal TotalCreditos { get; private set; }
    public decimal SaldoFinal { get; private set; }
    public int QuantidadeTransacoes { get; private set; }
    public DateTime DataAtualizacao { get; private set; }

    protected ConsolidadoDiario() { }

    public ConsolidadoDiario(DateTime data, decimal saldoInicial = 0)
    {
        Id = Guid.NewGuid();
        Data = data.Date;
        SaldoInicial = saldoInicial;
        TotalDebitos = 0;
        TotalCreditos = 0;
        QuantidadeTransacoes = 0;
        DataAtualizacao = DateTime.UtcNow;
        CalcularSaldoFinal();
    }

    public void AdicionarDebito(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("Valor do débito deve ser maior que zero", nameof(valor));

        TotalDebitos += valor;
        QuantidadeTransacoes++;
        DataAtualizacao = DateTime.UtcNow;
        CalcularSaldoFinal();
    }

    public void AdicionarCredito(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("Valor do crédito deve ser maior que zero", nameof(valor));

        TotalCreditos += valor;
        QuantidadeTransacoes++;
        DataAtualizacao = DateTime.UtcNow;
        CalcularSaldoFinal();
    }

    public void RemoverDebito(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("Valor do débito deve ser maior que zero", nameof(valor));

        TotalDebitos = Math.Max(0, TotalDebitos - valor);
        QuantidadeTransacoes = Math.Max(0, QuantidadeTransacoes - 1);
        DataAtualizacao = DateTime.UtcNow;
        CalcularSaldoFinal();
    }

    public void RemoverCredito(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("Valor do crédito deve ser maior que zero", nameof(valor));

        TotalCreditos = Math.Max(0, TotalCreditos - valor);
        QuantidadeTransacoes = Math.Max(0, QuantidadeTransacoes - 1);
        DataAtualizacao = DateTime.UtcNow;
        CalcularSaldoFinal();
    }

    private void CalcularSaldoFinal()
    {
        SaldoFinal = SaldoInicial + TotalCreditos - TotalDebitos;
    }

    public void AtualizarSaldoInicial(decimal novoSaldoInicial)
    {
        SaldoInicial = novoSaldoInicial;
        DataAtualizacao = DateTime.UtcNow;
        CalcularSaldoFinal();
    }
}