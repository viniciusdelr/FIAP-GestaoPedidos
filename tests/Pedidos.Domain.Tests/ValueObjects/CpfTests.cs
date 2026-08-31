using FluentAssertions;
using Pedidos.Domain.Exceptions;
using Pedidos.Domain.ValueObjects;
using Xunit;

namespace Pedidos.Domain.Tests.ValueObjects;

public class CpfTests
{
    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    public void Construtor_CpfValido_CriaInstancia(string cpfValido)
    {
        var cpf = new Cpf(cpfValido);

        cpf.Numero.Should().Be("52998224725");
    }

    [Theory]
    [InlineData("111.111.111-11")]
    [InlineData("123.456.789-00")]
    [InlineData("123")]
    [InlineData("")]
    public void Construtor_CpfInvalido_LancaDomainException(string cpfInvalido)
    {
        var acao = () => new Cpf(cpfInvalido);

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Igualdade_MesmoNumero_SaoIguais()
    {
        var cpf1 = new Cpf("529.982.247-25");
        var cpf2 = new Cpf("52998224725");

        cpf1.Should().Be(cpf2);
    }
}
