using Pedidos.Domain.Exceptions;

namespace Pedidos.Domain.ValueObjects;

public sealed record Dinheiro
{
    public decimal Valor { get; }

    public Dinheiro(decimal valor)
    {
        if (valor < 0)
            throw new DomainException("O valor em dinheiro não pode ser negativo.");

        Valor = valor;
    }

    public static Dinheiro Zero => new(0);

    public Dinheiro Somar(Dinheiro outro) => new(Valor + outro.Valor);

    public Dinheiro AplicarDesconto(decimal percentual)
    {
        if (percentual < 0 || percentual > 100)
            throw new DomainException("Percentual de desconto deve estar entre 0 e 100.");

        var valorComDesconto = Valor - (Valor * percentual / 100m);
        return new Dinheiro(valorComDesconto);
    }

    public static Dinheiro operator +(Dinheiro a, Dinheiro b) => a.Somar(b);

    public override string ToString() => Valor.ToString("C2");
}
