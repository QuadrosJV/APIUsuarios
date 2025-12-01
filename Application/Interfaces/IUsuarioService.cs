using APIUsuarios.Application.DTOs;

namespace APIUsuarios.Application.Interfaces
{
    public interface IUsuarioService
    {
        // Contrato do Service Pattern: define as operações de negócio.
        Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct);
        Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct);
        Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct);
        Task<UsuarioReadDto?> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct);
        Task<bool> RemoverAsync(int id, CancellationToken ct); // Vamos implementar o Soft Delete aqui!
        Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct);
    }
}