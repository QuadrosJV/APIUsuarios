namespace APIUsuarios.Application.DTOs
{
    public record UsuarioCreateDto(
        string Nome,
        string Email,
        string Senha, // O DTO de criação PRECISA da senha
        DateTime DataNascimento,
        string? Telefone
    );
}