using FluentAssertions;
using Moq;
using Pedidos.Application.Handlers;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Repositories;
using Pedidos.Domain.ValueObjects;
using Xunit;

namespace Pedidos.Application.Tests.Handlers;

public class ListarPedidosHandlerTests
{
    [Fact]
    public async Task Handle_PedidosCadastrados_RetornaListaDeDtos()
    {
        var pedido1 = Pedido.Criar("João da Silva", new Cpf("529.982.247-25"), new Email("joao@dominio.com"));
        var pedido2 = Pedido.Criar("Maria Souza", new Cpf("111.444.777-35"), new Email("maria@dominio.com"));

        var repositorioMock = new Mock<IPedidoRepository>();
        repositorioMock.Setup(r => r.ListarAsync()).ReturnsAsync(new[] { pedido1, pedido2 });
        var handler = new ListarPedidosHandler(repositorioMock.Object);

        var resultado = await handler.Handle();

        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_SemPedidosCadastrados_RetornaListaVazia()
    {
        var repositorioMock = new Mock<IPedidoRepository>();
        repositorioMock.Setup(r => r.ListarAsync()).ReturnsAsync(Array.Empty<Pedido>());
        var handler = new ListarPedidosHandler(repositorioMock.Object);

        var resultado = await handler.Handle();

        resultado.Should().BeEmpty();
    }
}
