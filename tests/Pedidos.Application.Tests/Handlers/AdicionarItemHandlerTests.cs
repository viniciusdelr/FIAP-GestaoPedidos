using FluentAssertions;
using Moq;
using Pedidos.Application.DTOs;
using Pedidos.Application.Exceptions;
using Pedidos.Application.Handlers;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Repositories;
using Pedidos.Domain.ValueObjects;
using Xunit;

namespace Pedidos.Application.Tests.Handlers;

public class AdicionarItemHandlerTests
{
    [Fact]
    public async Task Handle_PedidoInexistente_LancaNotFoundException()
    {
        var repositorioMock = new Mock<IPedidoRepository>();
        repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Pedido?)null);
        var handler = new AdicionarItemHandler(repositorioMock.Object);
        var dto = new AdicionarItemDto("Produto A", 1, 10);

        var acao = async () => await handler.Handle(Guid.NewGuid(), dto);

        await acao.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_DadosValidos_AdicionaItemEPersisteViaAtualizarAsync()
    {
        var pedido = Pedido.Criar("João da Silva", new Cpf("529.982.247-25"), new Email("joao@dominio.com"));

        var repositorioMock = new Mock<IPedidoRepository>();
        repositorioMock.Setup(r => r.ObterPorIdAsync(pedido.Id)).ReturnsAsync(pedido);
        var handler = new AdicionarItemHandler(repositorioMock.Object);
        var dto = new AdicionarItemDto("Produto A", 2, 10);

        var resultado = await handler.Handle(pedido.Id, dto);

        resultado.Itens.Should().ContainSingle(i => i.NomeProduto == "Produto A" && i.Quantidade == 2);
        repositorioMock.Verify(r => r.AtualizarAsync(It.Is<Pedido>(p => p.Id == pedido.Id)), Times.Once);
    }
}
