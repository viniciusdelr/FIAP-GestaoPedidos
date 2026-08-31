using Pedidos.Domain.Enums;
using Pedidos.Domain.Exceptions;
using Pedidos.Domain.ValueObjects;

namespace Pedidos.Domain.Entities;

public class Pedido
{
    private readonly List<ItemPedido> _itens = new();

    public Guid Id { get; private set; }
    public string NomeCliente { get; private set; } = string.Empty;
    public Cpf Cpf { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public StatusPedido StatusPedido { get; private set; }
    public decimal PercentualDesconto { get; private set; }
    public DateTime DataCriacao { get; private set; }

    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

    private Pedido()
    {
    }

    private Pedido(string nomeCliente, Cpf cpf, Email email)
    {
        if (string.IsNullOrWhiteSpace(nomeCliente))
            throw new DomainException("O nome do cliente é obrigatório.");

        Id = Guid.NewGuid();
        NomeCliente = nomeCliente;
        Cpf = cpf;
        Email = email;
        StatusPedido = StatusPedido.Rascunho;
        PercentualDesconto = 0;
        DataCriacao = DateTime.UtcNow;
    }

    public static Pedido Criar(string nomeCliente, Cpf cpf, Email email) => new(nomeCliente, cpf, email);

    public void AdicionarItem(string nomeProduto, int quantidade, decimal precoUnitario)
    {
        GarantirStatus(StatusPedido.Rascunho, "Só é possível adicionar itens a um pedido em Rascunho.");

        var item = new ItemPedido(nomeProduto, quantidade, new Dinheiro(precoUnitario));
        _itens.Add(item);
    }

    public void RemoverItem(Guid itemId)
    {
        GarantirStatus(StatusPedido.Rascunho, "Só é possível remover itens de um pedido em Rascunho.");

        var item = _itens.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
            throw new DomainException("Item não encontrado no pedido.");

        _itens.Remove(item);
    }

    public void AplicarDesconto(decimal percentual)
    {
        GarantirStatus(StatusPedido.Rascunho, "Só é possível aplicar desconto a um pedido em Rascunho.");

        if (percentual < 0 || percentual > 30)
            throw new DomainException("O percentual de desconto deve estar entre 0 e 30.");

        PercentualDesconto = percentual;
    }

    public void Fechar()
    {
        GarantirStatus(StatusPedido.Rascunho, "Só é possível fechar um pedido em Rascunho.");

        if (_itens.Count == 0)
            throw new DomainException("Não é possível fechar um pedido sem itens.");

        StatusPedido = StatusPedido.Fechado;
    }

    public void Enviar()
    {
        GarantirStatus(StatusPedido.Fechado, "Só é possível enviar um pedido Fechado.");

        StatusPedido = StatusPedido.Enviado;
    }

    public void Entregar()
    {
        GarantirStatus(StatusPedido.Enviado, "Só é possível entregar um pedido Enviado.");

        StatusPedido = StatusPedido.Entregue;
    }

    public void Cancelar()
    {
        if (StatusPedido != StatusPedido.Rascunho && StatusPedido != StatusPedido.Fechado)
            throw new DomainException("Só é possível cancelar um pedido em Rascunho ou Fechado.");

        StatusPedido = StatusPedido.Cancelado;
    }

    public Dinheiro CalcularTotal()
    {
        var total = _itens.Aggregate(Dinheiro.Zero, (acumulado, item) => acumulado.Somar(item.Subtotal));
        return total.AplicarDesconto(PercentualDesconto);
    }

    private void GarantirStatus(StatusPedido statusEsperado, string mensagemErro)
    {
        if (StatusPedido != statusEsperado)
            throw new DomainException(mensagemErro);
    }
}
