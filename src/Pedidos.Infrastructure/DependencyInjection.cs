using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pedidos.Application.Handlers;
using Pedidos.Domain.Repositories;
using Pedidos.Infrastructure.Persistence;
using Pedidos.Infrastructure.Repositories;

namespace Pedidos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AdicionarInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<PedidosDbContext>(options => options.UseInMemoryDatabase("PedidosDb"));

        services.AddScoped<IPedidoRepository, PedidoRepository>();

        services.AddScoped<CriarPedidoHandler>();
        services.AddScoped<AdicionarItemHandler>();
        services.AddScoped<AplicarDescontoHandler>();
        services.AddScoped<FecharPedidoHandler>();
        services.AddScoped<CancelarPedidoHandler>();
        services.AddScoped<ObterPedidoHandler>();
        services.AddScoped<ListarPedidosHandler>();

        return services;
    }
}
