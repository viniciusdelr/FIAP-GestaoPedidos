using Pedidos.Domain.Exceptions;
using Pedidos.Domain.ValueObjects;

namespace Pedidos.Domain.Entities;

public class ItemPedido
{
    public Guid Id { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public Dinheiro PrecoUnitario { get; private set; } = Dinheiro.Zero;

    public Dinheiro Subtotal => new(PrecoUnitario.Valor * Quantidade);

    private ItemPedido()
    {
    }

    public ItemPedido(string nomeProduto, int quantidade, Dinheiro precoUnitario)
    {
        if (string.IsNullOrWhiteSpace(nomeProduto))
            throw new DomainException("O nome do produto é obrigatório.");

        if (quantidade <= 0)
            throw new DomainException("A quantidade do item deve ser maior que zero.");

        Id = Guid.NewGuid();
        NomeProduto = nomeProduto;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }
}
