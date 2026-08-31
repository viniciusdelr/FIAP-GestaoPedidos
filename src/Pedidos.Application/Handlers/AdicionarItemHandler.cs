using Pedidos.Application.DTOs;
using Pedidos.Application.Exceptions;
using Pedidos.Application.Mapping;
using Pedidos.Domain.Repositories;

namespace Pedidos.Application.Handlers;

public class AdicionarItemHandler
{
    private readonly IPedidoRepository _repositorio;

    public AdicionarItemHandler(IPedidoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<PedidoDto> Handle(Guid pedidoId, AdicionarItemDto dto)
    {
        var pedido = await _repositorio.ObterPorIdAsync(pedidoId)
            ?? throw new NotFoundException($"Pedido com id '{pedidoId}' não encontrado.");

        pedido.AdicionarItem(dto.NomeProduto, dto.Quantidade, dto.PrecoUnitario);

        await _repositorio.AtualizarAsync(pedido);

        return PedidoMapper.ParaDto(pedido);
    }
}
