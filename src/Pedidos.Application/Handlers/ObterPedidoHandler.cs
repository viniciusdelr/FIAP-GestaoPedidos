using Pedidos.Application.DTOs;
using Pedidos.Application.Exceptions;
using Pedidos.Application.Mapping;
using Pedidos.Domain.Repositories;

namespace Pedidos.Application.Handlers;

public class ObterPedidoHandler
{
    private readonly IPedidoRepository _repositorio;

    public ObterPedidoHandler(IPedidoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<PedidoDto> Handle(Guid pedidoId)
    {
        var pedido = await _repositorio.ObterPorIdAsync(pedidoId)
            ?? throw new NotFoundException($"Pedido com id '{pedidoId}' não encontrado.");

        return PedidoMapper.ParaDto(pedido);
    }
}
