using System.Text.RegularExpressions;
using Pedidos.Domain.Exceptions;

namespace Pedidos.Domain.ValueObjects;

public sealed partial record Email
{
    public string Endereco { get; }

    public Email(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco) || !FormatoRegex().IsMatch(endereco))
            throw new DomainException("E-mail inválido.");

        Endereco = endereco.Trim();
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex FormatoRegex();

    public override string ToString() => Endereco;
}
