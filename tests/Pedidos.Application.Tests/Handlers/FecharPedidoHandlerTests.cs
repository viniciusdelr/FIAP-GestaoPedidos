using FluentAssertions;
using Moq;
using Pedidos.Application.Exceptions;
using Pedidos.Application.Handlers;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Repositories;
using Pedidos.Domain.ValueObjects;
using Xunit;

namespace Pedidos.Application.Tests.Handlers;

public class FecharPedidoHandlerTests
{
    [Fact]
    public async Task Handle_PedidoInexistente_LancaNotFoundException()
    {
        var repositorioMock = new Mock<IPedidoRepository>();
        repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Pedido?)null);
        var handler = new FecharPedidoHandler(repositorioMock.Object);

        var acao = async () => await handler.Handle(Guid.NewGuid());

        await acao.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_FluxoFeliz_PersisteViaAtualizarAsync()
    {
        var pedido = Pedido.Criar("João da Silva", new Cpf("529.982.247-25"), new Email("joao@dominio.com"));
        pedido.AdicionarItem("Produto A", 1, 10);

        var repositorioMock = new Mock<IPedidoRepository>();
        repositorioMock.Setup(r => r.ObterPorIdAsync(pedido.Id)).ReturnsAsync(pedido);
        var handler = new FecharPedidoHandler(repositorioMock.Object);

        var resultado = await handler.Handle(pedido.Id);

        resultado.StatusPedido.Should().Be("Fechado");
        repositorioMock.Verify(r => r.AtualizarAsync(It.Is<Pedido>(p => p.Id == pedido.Id)), Times.Once);
    }
}
