// UsuarioRepository.cs
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;
using APIUsuarios.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace APIUsuarios.Infrastructure.Repositories
{
    // Implementa o IUsuarioRepository (Repository Pattern)
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context; // Recebe o contexto via Injeção de Dependência
        }

        public async Task AddAsync(Usuario usuario, CancellationToken ct)
        {
            await _context.Usuarios.AddAsync(usuario, ct);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
        {
            // Checa a existência do email, convertendo a entrada para minúsculas para garantir a checagem.
            return await _context.Usuarios
                .AnyAsync(u => u.Email.ToLower() == email.ToLower(), ct);
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Usuarios
                .AsNoTracking() // Boa prática para consultas de listagem (melhora performance)
                .ToListAsync(ct);
        }

        public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), ct);
        }

        public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public Task RemoveAsync(Usuario usuario, CancellationToken ct)
        {
            // Na nossa arquitetura, este método é usado para marcar a Entidade para Soft Delete
            _context.Usuarios.Update(usuario);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            // Salva todas as alterações pendentes no banco de dados
            return await _context.SaveChangesAsync(ct);
        }

        public Task UpdateAsync(Usuario usuario, CancellationToken ct)
        {
            _context.Usuarios.Update(usuario);
            return Task.CompletedTask;
        }
    }
}