using Pedidos.Domain.Entities;

namespace Pedidos.Domain.Repositories;

public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(Guid id);
    Task<IReadOnlyCollection<Pedido>> ListarAsync();
    Task AdicionarAsync(Pedido pedido);
    Task AtualizarAsync(Pedido pedido);
}
