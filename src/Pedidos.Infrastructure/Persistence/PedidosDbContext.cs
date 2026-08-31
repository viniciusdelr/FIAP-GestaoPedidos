using Microsoft.EntityFrameworkCore;
using Pedidos.Domain.Entities;

namespace Pedidos.Infrastructure.Persistence;

public class PedidosDbContext : DbContext
{
    public PedidosDbContext(DbContextOptions<PedidosDbContext> options) : base(options)
    {
    }

    public DbSet<Pedido> Pedidos => Set<Pedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pedido>(pedido =>
        {
            pedido.HasKey(p => p.Id);

            pedido.OwnsOne(p => p.Cpf, cpf =>
            {
                cpf.Property(c => c.Numero).IsRequired();
            });

            pedido.OwnsOne(p => p.Email, email =>
            {
                email.Property(e => e.Endereco).IsRequired();
            });

            pedido.OwnsMany(p => p.Itens, item =>
            {
                item.WithOwner().HasForeignKey("PedidoId");
                item.HasKey(i => i.Id);
                item.Property(i => i.Id).ValueGeneratedNever();

                item.OwnsOne(i => i.PrecoUnitario, preco =>
                {
                    preco.Property(d => d.Valor).IsRequired();
                });

                item.Property(i => i.NomeProduto).IsRequired();
                item.Property(i => i.Quantidade).IsRequired();
            });

            pedido.Navigation(p => p.Itens).UsePropertyAccessMode(PropertyAccessMode.Field);
            pedido.Property(p => p.NomeCliente).IsRequired();
            pedido.Property(p => p.StatusPedido).IsRequired();
            pedido.Property(p => p.PercentualDesconto).IsRequired();
            pedido.Property(p => p.DataCriacao).IsRequired();
        });
    }
}
