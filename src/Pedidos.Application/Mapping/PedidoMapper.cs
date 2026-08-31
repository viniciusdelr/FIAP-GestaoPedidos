using Pedidos.Application.DTOs;
using Pedidos.Domain.Entities;

namespace Pedidos.Application.Mapping;

public static class PedidoMapper
{
    public static PedidoDto ParaDto(Pedido pedido)
    {
        var itens = pedido.Itens
            .Select(item => new ItemPedidoDto(
                item.Id,
                item.NomeProduto,
                item.Quantidade,
                item.PrecoUnitario.Valor,
                item.Subtotal.Valor))
            .ToList();

        return new PedidoDto(
            pedido.Id,
            pedido.NomeCliente,
            pedido.Cpf.Numero,
            pedido.Email.Endereco,
            pedido.StatusPedido.ToString(),
            pedido.PercentualDesconto,
            pedido.DataCriacao,
            itens,
            pedido.CalcularTotal().Valor);
    }
}
