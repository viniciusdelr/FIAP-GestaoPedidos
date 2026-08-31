using Pedidos.Application.DTOs;
using Pedidos.Application.Mapping;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Repositories;
using Pedidos.Domain.ValueObjects;

namespace Pedidos.Application.Handlers;

public class CriarPedidoHandler
{
    private readonly IPedidoRepository _repositorio;

    public CriarPedidoHandler(IPedidoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<PedidoDto> Handle(CriarPedidoDto dto)
    {
        var pedido = Pedido.Criar(dto.NomeCliente, new Cpf(dto.Cpf), new Email(dto.Email));

        await _repositorio.AdicionarAsync(pedido);

        return PedidoMapper.ParaDto(pedido);
    }
}
