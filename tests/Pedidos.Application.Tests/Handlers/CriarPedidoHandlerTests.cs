using FluentAssertions;
using Moq;
using Pedidos.Application.DTOs;
using Pedidos.Application.Handlers;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Exceptions;
using Pedidos.Domain.Repositories;
using Xunit;

namespace Pedidos.Application.Tests.Handlers;

public class CriarPedidoHandlerTests
{
    [Fact]
    public async Task Handle_DadosValidos_RetornaDtoEChamaAdicionarAsync()
    {
        var repositorioMock = new Mock<IPedidoRepository>();
        var handler = new CriarPedidoHandler(repositorioMock.Object);
        var dto = new CriarPedidoDto("João da Silva", "529.982.247-25", "joao@dominio.com");

        var resultado = await handler.Handle(dto);

        resultado.NomeCliente.Should().Be("João da Silva");
        resultado.StatusPedido.Should().Be("Rascunho");
        repositorioMock.Verify(r => r.AdicionarAsync(It.Is<Pedido>(p => p.NomeCliente == "João da Silva")), Times.Once);
    }

    [Fact]
    public async Task Handle_CpfInvalido_PropagaDomainException()
    {
        var repositorioMock = new Mock<IPedidoRepository>();
        var handler = new CriarPedidoHandler(repositorioMock.Object);
        var dto = new CriarPedidoDto("João da Silva", "111.111.111-11", "joao@dominio.com");

        var acao = async () => await handler.Handle(dto);

        await acao.Should().ThrowAsync<DomainException>();
        repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Pedido>()), Times.Never);
    }
}
