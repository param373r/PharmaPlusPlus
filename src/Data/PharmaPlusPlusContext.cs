using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PharmaPlusPlus.Models;

namespace PharmaPlusPlus.Data
{
    public class PharmaPlusPlusContext : DbContext
    {
        public PharmaPlusPlusContext(DbContextOptions<PharmaPlusPlusContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Drug> Drugs { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Role)
                    .HasConversion(entity =>
                        entity.ToString(),
                        entity =>
                            (Role)Enum.Parse(typeof(Role), entity)
                    );
            });

            modelBuilder.Entity<Cart>().HasKey(cart => cart.UserCartId);

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.Property(e => e.QuantityByDrugs)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, new JsonSerializerOptions(JsonSerializerDefaults.General)),
                        v => JsonSerializer.Deserialize<Dictionary<Guid, int>>(v, new JsonSerializerOptions(JsonSerializerDefaults.General))
                    );

                entity.Property(e => e.TotalPriceByDrugs)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, new JsonSerializerOptions(JsonSerializerDefaults.General)),
                        v => JsonSerializer.Deserialize<Dictionary<Guid, double>>(v, new JsonSerializerOptions(JsonSerializerDefaults.General))
                    );
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.QuantityByDrugs)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, new JsonSerializerOptions(JsonSerializerDefaults.General)),
                        v => JsonSerializer.Deserialize<Dictionary<Guid, int>>(v, new JsonSerializerOptions(JsonSerializerDefaults.General))
                    );

                entity.Property(e => e.TotalPriceByDrugs)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, new JsonSerializerOptions(JsonSerializerDefaults.General)),
                        v => JsonSerializer.Deserialize<Dictionary<Guid, double>>(v, new JsonSerializerOptions(JsonSerializerDefaults.General))
                    );
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderStatus)
                    .HasConversion(entity =>
                        entity.ToString(),
                        entity =>
                            (OrderStatus)Enum.Parse(typeof(OrderStatus), entity)
                    );
            });

        }
    }
}
