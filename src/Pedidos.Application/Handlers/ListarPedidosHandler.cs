using Pedidos.Application.DTOs;
using Pedidos.Application.Mapping;
using Pedidos.Domain.Repositories;

namespace Pedidos.Application.Handlers;

public class ListarPedidosHandler
{
    private readonly IPedidoRepository _repositorio;

    public ListarPedidosHandler(IPedidoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IReadOnlyCollection<PedidoDto>> Handle()
    {
        var pedidos = await _repositorio.ListarAsync();

        return pedidos.Select(PedidoMapper.ParaDto).ToList();
    }
}
