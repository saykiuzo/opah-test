namespace Lancamentos.Domain.Entities;

public class Lancamento
{
    public Guid Id { get; private set; }
    public DateTime Data { get; private set; }
    public decimal Valor { get; private set; }
    public TipoLancamento Tipo { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    protected Lancamento() { }

    public Lancamento(DateTime data, decimal valor, TipoLancamento tipo, string descricao)
    {
        Id = Guid.NewGuid();
        Data = data.Date;
        Valor = valor;
        Tipo = tipo;
        Descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
        DataCriacao = DateTime.UtcNow;

        ValidarLancamento();
    }

    public void AtualizarDescricao(string novaDescricao)
    {
        if (string.IsNullOrWhiteSpace(novaDescricao))
            throw new ArgumentException("Descrição não pode ser vazia", nameof(novaDescricao));

        Descricao = novaDescricao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarDados(DateTime novaData, decimal novoValor, TipoLancamento novoTipo, string novaDescricao)
    {
        if (string.IsNullOrWhiteSpace(novaDescricao))
            throw new ArgumentException("Descrição não pode ser vazia", nameof(novaDescricao));

        if (novoValor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero", nameof(novoValor));

        if (novaData.Date > DateTime.Today)
            throw new ArgumentException("Data não pode ser futura", nameof(novaData));

        Data = novaData.Date;
        Valor = novoValor;
        Tipo = novoTipo;
        Descricao = novaDescricao;
        DataAtualizacao = DateTime.UtcNow;
    }

    private void ValidarLancamento()
    {
        if (Valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero", nameof(Valor));

        if (string.IsNullOrWhiteSpace(Descricao))
            throw new ArgumentException("Descrição é obrigatória", nameof(Descricao));

        if (Data > DateTime.Today)
            throw new ArgumentException("Data não pode ser futura", nameof(Data));
    }
}

public enum TipoLancamento
{
    Debito = 0,
    Credito = 1
}