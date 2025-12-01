namespace APIUsuarios.Domain.Entities
{
    // A Entidade define o formato dos dados que o nosso negócio gerencia.
    public class Usuario
    {
        public int Id { get; set; } // Chave primária (PK), auto-incremento do banco.
        public string Nome { get; set; } = string.Empty; // Obrigatório, nome do usuário.
        public string Email { get; set; } = string.Empty; // Obrigatório, único.
        public string Senha { get; set; } = string.Empty; // Obrigatório, guardamos a senha (no mundo real, faríamos HASH).
        public DateTime DataNascimento { get; set; } // Obrigatório, para checar a idade mínima.
        public string? Telefone { get; set; } // Opcional, o '?' indica que pode ser nulo.
        public bool Ativo { get; set; } = true; // Obrigatório, usamos para o Soft Delete.
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow; // Preenchido automaticamente na criação.
        public DateTime? DataAtualizacao { get; set; } // Atualizado em cada modificação.
    }
}