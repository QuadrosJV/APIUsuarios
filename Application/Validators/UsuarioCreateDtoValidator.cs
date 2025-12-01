using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using FluentValidation;

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

            // Email: Formato válido e, o mais importante, ÚNICO!
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("O Email é obrigatório.")
                .EmailAddress().WithMessage("O Email está em um formato inválido.")
                // Validação Assíncrona: checamos no banco se o e-mail já existe.
                .MustAsync(async (email, ct) => !await _repository.EmailExistsAsync(email, ct))
                .WithMessage("O Email informado já está cadastrado."); // Requisito 409

            // Senha: Mínimo 6 caracteres.
            RuleFor(u => u.Senha)
                .NotEmpty().WithMessage("A Senha é obrigatória.")
                .MinimumLength(6).WithMessage("A Senha deve ter no mínimo 6 caracteres.");

            // Data de Nascimento: Checa se tem pelo menos 18 anos.
            RuleFor(u => u.DataNascimento)
                .NotEmpty().WithMessage("A Data de Nascimento é obrigatória.")
                .Must(BeAtLeast18).WithMessage("O Usuário deve ter no mínimo 18 anos.");

            // Telefone: Opcional, mas se preenchido, deve ser um formato válido.
            RuleFor(u => u.Telefone)
                .Matches(@"^\(?(?:[14689][1-9]|2[1-9]|3[1-5]|5[1-9]|7[134579])\)? ?(?:[2-8]|9[1-9])[0-9]{3}\-?[0-9]{4}$")
                .When(u => !string.IsNullOrEmpty(u.Telefone))
                .WithMessage("O Telefone deve estar no formato (XX) XXXXX-XXXX ou (XX) XXXX-XXXX.");
        }

        private bool BeAtLeast18(DateTime dataNascimento)
        {
            var today = DateTime.Today;
            var age = today.Year - dataNascimento.Year;
            // Ajusta se o aniversário ainda não chegou este ano
            if (dataNascimento.Date > today.AddYears(-age)) age--; 
            return age >= 18; // Requisito de Idade Mínima
        }
    }
}
