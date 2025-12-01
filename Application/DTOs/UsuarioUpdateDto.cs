namespace APIUsuarios.Application.DTOs
{
    public record UsuarioUpdateDto(
        string Nome,
        string Email,
        DateTime DataNascimento,
        string? Telefone,
        bool Ativo
        // Observação: Também não precisamos da senha para a atualização PUT completa.
    );
}