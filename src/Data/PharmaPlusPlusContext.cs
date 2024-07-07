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
        }
    }
}
