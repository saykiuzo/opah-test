using Xunit;
using FluentAssertions;
using Lancamentos.Domain.Entities;

namespace Lancamentos.Tests.Unit.Domain;

public class LancamentoTests
{
    [Fact]
    public void CriarLancamento_ComDadosValidos_DeveRetornarLancamentoValido()
    {
        var data = DateTime.Today;
        var valor = 100.50m;
        var tipo = TipoLancamento.Credito;
        var descricao = "Recebimento de cliente";

        var lancamento = new Lancamento(data, valor, tipo, descricao);

        lancamento.Id.Should().NotBeEmpty();
        lancamento.Data.Should().Be(data.Date);
        lancamento.Valor.Should().Be(valor);
        lancamento.Tipo.Should().Be(tipo);
        lancamento.Descricao.Should().Be(descricao);
        lancamento.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10.50)]
    public void CriarLancamento_ComValorInvalido_DeveLancarExcecao(decimal valorInvalido)
    {
        var data = DateTime.Today;
        var tipo = TipoLancamento.Debito;
        var descricao = "Teste";

        var action = () => new Lancamento(data, valorInvalido, tipo, descricao);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Valor deve ser maior que zero*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void CriarLancamento_ComDescricaoInvalida_DeveLancarExcecao(string? descricaoInvalida)
    {
        var data = DateTime.Today;
        var valor = 50.00m;
        var tipo = TipoLancamento.Credito;

        var action = () => new Lancamento(data, valor, tipo, descricaoInvalida!);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CriarLancamento_ComDataFutura_DeveLancarExcecao()
    {
        var dataFutura = DateTime.Today.AddDays(1);
        var valor = 100.00m;
        var tipo = TipoLancamento.Debito;
        var descricao = "Teste";

        var action = () => new Lancamento(dataFutura, valor, tipo, descricao);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Data não pode ser futura*");
    }

    [Fact]
    public void AtualizarDescricao_ComDescricaoValida_DeveAtualizarCorretamente()
    {
        var lancamento = new Lancamento(DateTime.Today, 100m, TipoLancamento.Credito, "Descrição original");
        var novaDescricao = "Nova descrição";

        lancamento.AtualizarDescricao(novaDescricao);

        lancamento.Descricao.Should().Be(novaDescricao);
        lancamento.DataAtualizacao.Should().NotBeNull();
        lancamento.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AtualizarDescricao_ComDescricaoInvalida_DeveLancarExcecao(string descricaoInvalida)
    {
        var lancamento = new Lancamento(DateTime.Today, 100m, TipoLancamento.Credito, "Descrição original");

        var action = () => lancamento.AtualizarDescricao(descricaoInvalida);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Descrição não pode ser vazia*");
    }
}