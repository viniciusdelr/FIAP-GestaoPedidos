using FluentAssertions;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Enums;
using Pedidos.Domain.Exceptions;
using Pedidos.Domain.ValueObjects;
using Xunit;

namespace Pedidos.Domain.Tests.Entities;

public class PedidoTests
{
    private static Pedido CriarPedidoValido()
    {
        return Pedido.Criar("João da Silva", new Cpf("529.982.247-25"), new Email("joao@dominio.com"));
    }

    [Fact]
    public void Criar_DadosValidos_CriaPedidoEmRascunho()
    {
        var pedido = CriarPedidoValido();

        pedido.StatusPedido.Should().Be(StatusPedido.Rascunho);
        pedido.Itens.Should().BeEmpty();
        pedido.PercentualDesconto.Should().Be(0);
    }

    [Fact]
    public void AdicionarItem_QuantidadeValida_AdicionaItemAoPedido()
    {
        var pedido = CriarPedidoValido();

        pedido.AdicionarItem("Produto A", 2, 10);

        pedido.Itens.Should().ContainSingle(i => i.NomeProduto == "Produto A");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AdicionarItem_QuantidadeZeroOuNegativa_LancaDomainException(int quantidadeInvalida)
    {
        var pedido = CriarPedidoValido();

        var acao = () => pedido.AdicionarItem("Produto A", quantidadeInvalida, 10);

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void AdicionarItem_PedidoNaoRascunho_LancaDomainException()
    {
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem("Produto A", 1, 10);
        pedido.Fechar();

        var acao = () => pedido.AdicionarItem("Produto B", 1, 10);

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void RemoverItem_ItemExistente_RemoveItemDoPedido()
    {
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem("Produto A", 1, 10);
        var itemId = pedido.Itens.Single().Id;

        pedido.RemoverItem(itemId);

        pedido.Itens.Should().BeEmpty();
    }

    [Fact]
    public void RemoverItem_ItemInexistente_LancaDomainException()
    {
        var pedido = CriarPedidoValido();

        var acao = () => pedido.RemoverItem(Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void CalcularTotal_SemDesconto_RetornaSomaDosItens()
    {
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem("Produto A", 2, 10);
        pedido.AdicionarItem("Produto B", 1, 30);

        var total = pedido.CalcularTotal();

        total.Valor.Should().Be(50);
    }

    [Fact]
    public void CalcularTotal_ComDesconto_RetornaSomaComDescontoAplicado()
    {
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem("Produto A", 1, 100);
        pedido.AplicarDesconto(10);

        var total = pedido.CalcularTotal();

        total.Valor.Should().Be(90);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(30)]
    public void AplicarDesconto_PercentualNoLimite_AplicaDesconto(decimal percentual)
    {
        var pedido = CriarPedidoValido();

        pedido.AplicarDesconto(percentual);

        pedido.PercentualDesconto.Should().Be(percentual);
    }

    [Theory]
    [InlineData(30.01)]
    [InlineData(50)]
    [InlineData(-1)]
    public void AplicarDesconto_PercentualAcimaDoLimite_LancaDomainException(decimal percentualInvalido)
    {
        var pedido = CriarPedidoValido();

        var acao = () => pedido.AplicarDesconto(percentualInvalido);

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Fechar_SemItens_LancaDomainException()
    {
        var pedido = CriarPedidoValido();

        var acao = () => pedido.Fechar();

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Fechar_ComItens_MudaStatusParaFechado()
    {
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem("Produto A", 1, 10);

        pedido.Fechar();

        pedido.StatusPedido.Should().Be(StatusPedido.Fechado);
    }

    [Fact]
    public void Enviar_PedidoFechado_MudaStatusParaEnviado()
    {
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem("Produto A", 1, 10);
        pedido.Fechar();

        pedido.Enviar();

        pedido.StatusPedido.Should().Be(StatusPedido.Enviado);
    }

    [Fact]
    public void Enviar_PedidoEmRascunho_LancaDomainException()
    {
        var pedido = CriarPedidoValido();

        var acao = () => pedido.Enviar();

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Entregar_PedidoEnviado_MudaStatusParaEntregue()
    {
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem("Produto A", 1, 10);
        pedido.Fechar();
        pedido.Enviar();

        pedido.Entregar();

        pedido.StatusPedido.Should().Be(StatusPedido.Entregue);
    }

    [Fact]
    public void Entregar_PedidoFechado_LancaDomainException()
    {
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem("Produto A", 1, 10);
        pedido.Fechar();

        var acao = () => pedido.Entregar();

        acao.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(StatusPedido.Rascunho)]
    [InlineData(StatusPedido.Fechado)]
    public void Cancelar_StatusPermitido_MudaStatusParaCancelado(StatusPedido status)
    {
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem("Produto A", 1, 10);
        if (status == StatusPedido.Fechado)
            pedido.Fechar();

        pedido.Cancelar();

        pedido.StatusPedido.Should().Be(StatusPedido.Cancelado);
    }

    [Fact]
    public void Cancelar_PedidoEnviado_LancaDomainException()
    {
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem("Produto A", 1, 10);
        pedido.Fechar();
        pedido.Enviar();

        var acao = () => pedido.Cancelar();

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancelar_PedidoEntregue_LancaDomainException()
    {
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem("Produto A", 1, 10);
        pedido.Fechar();
        pedido.Enviar();
        pedido.Entregar();

        var acao = () => pedido.Cancelar();

        acao.Should().Throw<DomainException>();
    }
}
