using System.Text.RegularExpressions;
using Pedidos.Domain.Exceptions;

namespace Pedidos.Domain.ValueObjects;

public sealed record Cpf
{
    public string Numero { get; }

    public Cpf(string numero)
    {
        var apenasDigitos = LimparFormatacao(numero);

        if (!EhValido(apenasDigitos))
            throw new DomainException("CPF inválido.");

        Numero = apenasDigitos;
    }

    private static string LimparFormatacao(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            return string.Empty;

        return Regex.Replace(numero, "[^0-9]", "");
    }

    private static bool EhValido(string cpf)
    {
        if (cpf.Length != 11)
            return false;

        if (TodosDigitosIguais(cpf))
            return false;

        var primeiroDigito = CalcularDigitoVerificador(cpf, 9);
        var segundoDigito = CalcularDigitoVerificador(cpf, 10);

        return cpf[9] - '0' == primeiroDigito && cpf[10] - '0' == segundoDigito;
    }

    private static bool TodosDigitosIguais(string cpf)
    {
        for (var i = 1; i < cpf.Length; i++)
        {
            if (cpf[i] != cpf[0])
                return false;
        }

        return true;
    }

    private static int CalcularDigitoVerificador(string cpf, int quantidadeDigitos)
    {
        var multiplicador = quantidadeDigitos + 1;
        var soma = 0;

        for (var i = 0; i < quantidadeDigitos; i++)
        {
            soma += (cpf[i] - '0') * multiplicador;
            multiplicador--;
        }

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    public override string ToString() => Numero;
}
