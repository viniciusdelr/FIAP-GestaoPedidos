using FluentAssertions;
using Pedidos.Domain.Exceptions;
using Pedidos.Domain.ValueObjects;
using Xunit;

namespace Pedidos.Domain.Tests.ValueObjects;

public class DinheiroTests
{
    [Fact]
    public void Construtor_ValorNegativo_LancaDomainException()
    {
        var acao = () => new Dinheiro(-1);

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Somar_DoisValores_RetornaSoma()
    {
        var a = new Dinheiro(10);
        var b = new Dinheiro(5);

        var resultado = a.Somar(b);

        resultado.Valor.Should().Be(15);
    }

    [Fact]
    public void AplicarDesconto_PercentualValido_RetornaValorComDesconto()
    {
        var valor = new Dinheiro(100);

        var resultado = valor.AplicarDesconto(10);

        resultado.Valor.Should().Be(90);
    }

    [Fact]
    public void Igualdade_MesmoValor_SaoIguais()
    {
        var a = new Dinheiro(10);
        var b = new Dinheiro(10);

        a.Should().Be(b);
    }
}
