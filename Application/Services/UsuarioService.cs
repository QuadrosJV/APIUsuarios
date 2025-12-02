// UsuarioService.cs
using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Application.Services
{
    // Implementa a lógica de negócio (Service Pattern)
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        // Injeção de Dependência do Repositório (IoC)
        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        // UsuarioService.cs

public async Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct)
{
    // NOVO: 1. CHECAGEM DE DUPLICIDADE (LÓGICA DE NEGÓCIO para 409)
    // Usamos o método auxiliar que chama o repositório
    var emailExiste = await EmailJaCadastradoAsync(dto.Email, ct);
    
    if (emailExiste)
    {
        // Lança a exceção. O middleware no Program.cs a transforma em 409.
        throw new ApplicationException($"O Email informado '{dto.Email}' já está cadastrado.");
    }
    
    // 2. Mapeamento de DTO para a Entidade de Domínio
    var usuario = new Usuario
    {
        Nome = dto.Nome,
        Email = dto.Email.ToLower(),
        Senha = dto.Senha, 
        DataNascimento = dto.DataNascimento,
        Telefone = dto.Telefone,
        Ativo = true, // Garantindo que o novo usuário é ativo
        // DataCriacao e Ativo são defaults na Entidade
    };

    await _repository.AddAsync(usuario, ct);
    await _repository.SaveChangesAsync(ct);

    // 3. Retorna o DTO de Leitura, sem a senha
    return new UsuarioReadDto(
        usuario.Id, usuario.Nome, usuario.Email, usuario.DataNascimento,
        usuario.Telefone, usuario.Ativo, usuario.DataCriacao
    );
}
        
        public async Task<UsuarioReadDto?> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct)
        {
            var usuario = await _repository.GetByIdAsync(id, ct);
            
            if (usuario == null) return null; // Não achou, retorna nulo para o endpoint retornar 404

            // Validação de Negócio: Checa se o novo email já pertence a OUTRO usuário.
            var existingUser = await _repository.GetByEmailAsync(dto.Email, ct);
            if (existingUser != null && existingUser.Id != id)
            {
                // Lança uma exceção de Application/Negócio. O Program.cs vai capturar e retornar 409 Conflict.
                throw new ApplicationException("O Email já está sendo usado por outro usuário.");
            }

            // Atualiza os campos
            usuario.Nome = dto.Nome;
            usuario.Email = dto.Email;
            usuario.DataNascimento = dto.DataNascimento;
            usuario.Telefone = dto.Telefone;
            usuario.Ativo = dto.Ativo;
            usuario.DataAtualizacao = DateTime.UtcNow;

            await _repository.UpdateAsync(usuario, ct);
            await _repository.SaveChangesAsync(ct);
            
            // Mapeamento para DTO
            return new UsuarioReadDto(
                usuario.Id, usuario.Nome, usuario.Email, usuario.DataNascimento,
                usuario.Telefone, usuario.Ativo, usuario.DataCriacao
            );
        }

        public async Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct)
        {
            var usuarios = await _repository.GetAllAsync(ct);

            // Mapeia a lista de Entidades para a lista de DTOs, seguindo o padrão
            return usuarios
                .Select(usuario => new UsuarioReadDto(
                    usuario.Id, usuario.Nome, usuario.Email, usuario.DataNascimento,
                    usuario.Telefone, usuario.Ativo, usuario.DataCriacao
                ))
                .ToList();
        }

        public async Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct)
        {
            var usuario = await _repository.GetByIdAsync(id, ct);
            
            if (usuario == null) return null;

            // Mapeamento para DTO
            return new UsuarioReadDto(
                usuario.Id, usuario.Nome, usuario.Email, usuario.DataNascimento,
                usuario.Telefone, usuario.Ativo, usuario.DataCriacao
            );
        }

        public async Task<bool> RemoverAsync(int id, CancellationToken ct)
        {
            var usuario = await _repository.GetByIdAsync(id, ct);
            
            if (usuario == null) return false;

            // Requisito 6: Soft Delete - Apenas marca como inativo
            usuario.Ativo = false;
            usuario.DataAtualizacao = DateTime.UtcNow;

            // O Update no Repositório salva a alteração de estado
            await _repository.UpdateAsync(usuario, ct);
            await _repository.SaveChangesAsync(ct);
            
            return true;
        }

        public async Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct)
        {
            return await _repository.EmailExistsAsync(email, ct);
        }
    }
}