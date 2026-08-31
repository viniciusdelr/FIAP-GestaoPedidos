namespace Pedidos.Application.DTOs;

public record ItemPedidoDto(
    Guid Id,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal);
