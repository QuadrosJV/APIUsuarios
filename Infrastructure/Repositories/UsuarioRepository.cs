using APIUsuarios.Domain.Entities;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace APIUsuarios.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> Add(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario?> GetById(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<IEnumerable<Usuario>> GetAll()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> Update(Usuario usuario)
        {
            var existingUsuario = await _context.Usuarios.FindAsync(usuario.Id);
            if (existingUsuario == null) return null;

            existingUsuario.Name = usuario.Name;
            existingUsuario.Email = usuario.Email;

            await _context.SaveChangesAsync();
            return existingUsuario;
        }

        public async Task<bool> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}