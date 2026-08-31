using FluentAssertions;
using Pedidos.Domain.Exceptions;
using Pedidos.Domain.ValueObjects;
using Xunit;

namespace Pedidos.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("cliente@dominio.com")]
    [InlineData("nome.sobrenome@empresa.com.br")]
    public void Construtor_EmailValido_CriaInstancia(string enderecoValido)
    {
        var email = new Email(enderecoValido);

        email.Endereco.Should().Be(enderecoValido);
    }

    [Theory]
    [InlineData("sem-arroba.com")]
    [InlineData("sem-dominio@")]
    [InlineData("@sem-usuario.com")]
    [InlineData("")]
    [InlineData(" ")]
    public void Construtor_EmailInvalido_LancaDomainException(string enderecoInvalido)
    {
        var acao = () => new Email(enderecoInvalido);

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Igualdade_MesmoEndereco_SaoIguais()
    {
        var email1 = new Email("cliente@dominio.com");
        var email2 = new Email("cliente@dominio.com");

        email1.Should().Be(email2);
    }
}
