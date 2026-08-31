namespace Pedidos.Application.DTOs;

public record PedidoDto(
    Guid Id,
    string NomeCliente,
    string Cpf,
    string Email,
    string StatusPedido,
    decimal PercentualDesconto,
    DateTime DataCriacao,
    IReadOnlyCollection<ItemPedidoDto> Itens,
    decimal Total);
