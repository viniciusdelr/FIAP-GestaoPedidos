using Microsoft.EntityFrameworkCore;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Repositories;
using Pedidos.Infrastructure.Persistence;

namespace Pedidos.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly PedidosDbContext _contexto;

    public PedidoRepository(PedidosDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<Pedido?> ObterPorIdAsync(Guid id)
    {
        return await _contexto.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IReadOnlyCollection<Pedido>> ListarAsync()
    {
        return await _contexto.Pedidos
            .Include(p => p.Itens)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Pedido pedido)
    {
        await _contexto.Pedidos.AddAsync(pedido);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Pedido pedido)
    {
        if (_contexto.Entry(pedido).State == EntityState.Detached)
            _contexto.Pedidos.Update(pedido);

        await _contexto.SaveChangesAsync();
    }
}
