namespace Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioReadDto> CreateUsuario(UsuarioCreateDto usuarioCreateDto);
        Task<UsuarioReadDto> GetUsuario(int id);
        Task<IEnumerable<UsuarioReadDto>> GetAllUsuarios();
        Task<UsuarioReadDto> UpdateUsuario(int id, UsuarioUpdateDto usuarioUpdateDto);
        Task<bool> DeleteUsuario(int id);
    }
}