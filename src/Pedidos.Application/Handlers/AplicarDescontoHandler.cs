using Pedidos.Application.DTOs;
using Pedidos.Application.Exceptions;
using Pedidos.Application.Mapping;
using Pedidos.Domain.Repositories;

namespace Pedidos.Application.Handlers;

public class AplicarDescontoHandler
{
    private readonly IPedidoRepository _repositorio;

    public AplicarDescontoHandler(IPedidoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<PedidoDto> Handle(Guid pedidoId, AplicarDescontoDto dto)
    {
        var pedido = await _repositorio.ObterPorIdAsync(pedidoId)
            ?? throw new NotFoundException($"Pedido com id '{pedidoId}' não encontrado.");

        pedido.AplicarDesconto(dto.Percentual);

        await _repositorio.AtualizarAsync(pedido);

        return PedidoMapper.ParaDto(pedido);
    }
}
