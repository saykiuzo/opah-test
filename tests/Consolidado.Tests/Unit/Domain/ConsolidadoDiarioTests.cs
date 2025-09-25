using Xunit;
using FluentAssertions;
using Consolidado.Domain.Entities;

namespace Consolidado.Tests.Unit.Domain;

public class ConsolidadoDiarioTests
{
    [Fact]
    public void CriarConsolidadoDiario_ComDadosValidos_DeveRetornarConsolidadoValido()
    {
        var data = DateTime.Today;
        var saldoInicial = 500.00m;
        var consolidado = new ConsolidadoDiario(data, saldoInicial);

        consolidado.Id.Should().NotBeEmpty();
        consolidado.Data.Should().Be(data.Date);
        consolidado.SaldoInicial.Should().Be(saldoInicial);
        consolidado.TotalDebitos.Should().Be(0);
        consolidado.TotalCreditos.Should().Be(0);
        consolidado.SaldoFinal.Should().Be(saldoInicial);
        consolidado.QuantidadeTransacoes.Should().Be(0);
        consolidado.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AdicionarDebito_ComValorValido_DeveAtualizarConsolidado()
    {
        var consolidado = new ConsolidadoDiario(DateTime.Today, 1000m);
        var valorDebito = 200m;

        consolidado.AdicionarDebito(valorDebito);

        consolidado.TotalDebitos.Should().Be(valorDebito);
        consolidado.SaldoFinal.Should().Be(800m);
        consolidado.QuantidadeTransacoes.Should().Be(1);
    }

    [Fact]
    public void AdicionarCredito_ComValorValido_DeveAtualizarConsolidado()
    {
        var consolidado = new ConsolidadoDiario(DateTime.Today, 1000m);
        var valorCredito = 300m;

        consolidado.AdicionarCredito(valorCredito);

        consolidado.TotalCreditos.Should().Be(valorCredito);
        consolidado.SaldoFinal.Should().Be(1300m);
        consolidado.QuantidadeTransacoes.Should().Be(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void AdicionarDebito_ComValorInvalido_DeveLancarExcecao(decimal valorInvalido)
    {
        var consolidado = new ConsolidadoDiario(DateTime.Today, 1000m);

        var action = () => consolidado.AdicionarDebito(valorInvalido);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Valor do débito deve ser maior que zero*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void AdicionarCredito_ComValorInvalido_DeveLancarExcecao(decimal valorInvalido)
    {
        var consolidado = new ConsolidadoDiario(DateTime.Today, 1000m);

        var action = () => consolidado.AdicionarCredito(valorInvalido);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Valor do crédito deve ser maior que zero*");
    }

    [Fact]
    public void CalculoSaldoFinal_ComMultiplasTrasacoes_DeveCalcularCorretamente()
    {
        var consolidado = new ConsolidadoDiario(DateTime.Today, 1000m);

        consolidado.AdicionarCredito(500m);  // Saldo: 1500
        consolidado.AdicionarDebito(300m);   // Saldo: 1200
        consolidado.AdicionarCredito(200m);  // Saldo: 1400
        consolidado.AdicionarDebito(150m);   // Saldo: 1250

        consolidado.SaldoFinal.Should().Be(1250m);
        consolidado.TotalCreditos.Should().Be(700m);
        consolidado.TotalDebitos.Should().Be(450m);
        consolidado.QuantidadeTransacoes.Should().Be(4);
    }

    [Fact]
    public void AtualizarSaldoInicial_DeveRecalcularSaldoFinal()
    {
        var consolidado = new ConsolidadoDiario(DateTime.Today, 1000m);
        consolidado.AdicionarCredito(300m);
        consolidado.AdicionarDebito(200m);

        consolidado.AtualizarSaldoInicial(1500m);

        consolidado.SaldoInicial.Should().Be(1500m);
        consolidado.SaldoFinal.Should().Be(1600m);
    }

    [Fact]
    public void RemoverDebito_DeveAtualizarConsolidado()
    {
        var consolidado = new ConsolidadoDiario(DateTime.Today, 1000m);
        consolidado.AdicionarDebito(300m);
        consolidado.AdicionarDebito(200m);

        consolidado.RemoverDebito(200m);

        consolidado.TotalDebitos.Should().Be(300m);
        consolidado.SaldoFinal.Should().Be(700m);
        consolidado.QuantidadeTransacoes.Should().Be(1);
    }

    [Fact]
    public void RemoverCredito_DeveAtualizarConsolidado()
    {
        var consolidado = new ConsolidadoDiario(DateTime.Today, 1000m);
        consolidado.AdicionarCredito(300m);
        consolidado.AdicionarCredito(200m);

        consolidado.RemoverCredito(200m);

        consolidado.TotalCreditos.Should().Be(300m);
        consolidado.SaldoFinal.Should().Be(1300m);
        consolidado.QuantidadeTransacoes.Should().Be(1);
    }
}