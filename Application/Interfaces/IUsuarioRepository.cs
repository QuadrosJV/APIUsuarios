namespace APIUsuarios.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        void Add(Usuario usuario);
        Usuario GetById(int id);
        void Update(Usuario usuario);
        void Delete(int id);
        IEnumerable<Usuario> GetAll();
    }
}