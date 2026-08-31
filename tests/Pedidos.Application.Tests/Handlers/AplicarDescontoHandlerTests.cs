using FluentAssertions;
using Moq;
using Pedidos.Application.DTOs;
using Pedidos.Application.Exceptions;
using Pedidos.Application.Handlers;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Exceptions;
using Pedidos.Domain.Repositories;
using Pedidos.Domain.ValueObjects;
using Xunit;

namespace Pedidos.Application.Tests.Handlers;

public class AplicarDescontoHandlerTests
{
    [Fact]
    public async Task Handle_DescontoInvalido_PropagaDomainException()
    {
        var pedido = Pedido.Criar("João da Silva", new Cpf("529.982.247-25"), new Email("joao@dominio.com"));

        var repositorioMock = new Mock<IPedidoRepository>();
        repositorioMock.Setup(r => r.ObterPorIdAsync(pedido.Id)).ReturnsAsync(pedido);
        var handler = new AplicarDescontoHandler(repositorioMock.Object);
        var dto = new AplicarDescontoDto(50);

        var acao = async () => await handler.Handle(pedido.Id, dto);

        await acao.Should().ThrowAsync<DomainException>();
        repositorioMock.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PedidoInexistente_LancaNotFoundException()
    {
        var repositorioMock = new Mock<IPedidoRepository>();
        repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Pedido?)null);
        var handler = new AplicarDescontoHandler(repositorioMock.Object);

        var acao = async () => await handler.Handle(Guid.NewGuid(), new AplicarDescontoDto(10));

        await acao.Should().ThrowAsync<NotFoundException>();
    }
}
