using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using FluentValidation;
using System; // Adicionar se ainda não estiver presente

namespace APIUsuarios.Application.Validators
{
    // Valida as regras de negócio ANTES de chamar o Service.
    public class UsuarioCreateDtoValidator : AbstractValidator<UsuarioCreateDto>
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioCreateDtoValidator(IUsuarioRepository repository)
        {
            _repository = repository;

            // Nome: Obrigatório e tamanho entre 3 e 100 caracteres.
            RuleFor(u => u.Nome)
                .NotEmpty().WithMessage("O Nome é obrigatório.")
                .Length(3, 100).WithMessage("O Nome deve ter entre 3 e 100 caracteres.");

            // Email: Formato válido.
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("O Email é obrigatório.")
                .EmailAddress().WithMessage("O Email está em um formato inválido.");
                // A checagem de unicidade (409 Conflict) é feita no Service.

            // Senha: Mínimo 6 caracteres.
            RuleFor(u => u.Senha)
                .NotEmpty().WithMessage("A Senha é obrigatória.")
                .MinimumLength(6).WithMessage("A Senha deve ter no mínimo 6 caracteres.");

            // Data de Nascimento: Checa se tem pelo menos 18 anos (Requisito 400).
            RuleFor(u => u.DataNascimento)
                .NotEmpty().WithMessage("A Data de Nascimento é obrigatória.")
                .Must(BeAtLeast18).WithMessage("O Usuário deve ter no mínimo 18 anos.");

            // Telefone: Opcional, mas se preenchido, deve ser um formato válido (Requisito 400).
            RuleFor(u => u.Telefone)
                .Matches(@"^\(\d{2}\)\s\d{4,5}-\d{4}$")
                .WithMessage("O Telefone deve estar no formato (XX) XXXXX-XXXX ou (XX) XXXX-XXXX.")
                .When(u => !string.IsNullOrEmpty(u.Telefone)); // Só valida o regex se o campo não estiver vazio.
        }

        // Método auxiliar para o cálculo da idade
        private bool BeAtLeast18(DateTime dataNascimento)
        {
            var today = DateTime.Today;
            var age = today.Year - dataNascimento.Year;
            
            // Ajusta se o aniversário ainda não chegou este ano
            if (dataNascimento.Date > today.AddYears(-age))
                age--;
                
            return age >= 18; // Requisito de Idade Mínima
        }
    }
}