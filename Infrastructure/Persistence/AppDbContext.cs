// AppDbContext.cs
using APIUsuarios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace APIUsuarios.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; } // Representa a nossa tabela no banco

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração para garantir que o Email é único
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
            // Requisito: Normalização - Força o armazenamento do Email em lowercase
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Email)
                .HasConversion(
                    v => v.ToLower(), // Ao salvar, converte para minúsculas
                    v => v           // Ao ler, não faz nada
                );
            
            // Limitação do Nome para garantir o requisito de 100 caracteres
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Nome)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}