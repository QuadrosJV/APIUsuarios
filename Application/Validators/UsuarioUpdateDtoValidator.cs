using APIUsuarios.Application.DTOs;
using FluentValidation;

namespace APIUsuarios.Application.Validators
{
    // Validador de Update é similar, mas não valida a senha (que não está no DTO) e 
    // a checagem de e-mail é mais complexa (será feita no Service).
    public class UsuarioUpdateDtoValidator : AbstractValidator<UsuarioUpdateDto>
    {
        public UsuarioUpdateDtoValidator()
        {
            RuleFor(u => u.Nome)
                .NotEmpty().WithMessage("O Nome é obrigatório.")
                .Length(3, 100).WithMessage("O Nome deve ter entre 3 e 100 caracteres.");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("O Email é obrigatório.")
                .EmailAddress().WithMessage("O Email está em um formato inválido.");
                // A checagem de unicidade, excluindo o próprio ID, será feita na camada de Serviço.

            RuleFor(u => u.DataNascimento)
                .NotEmpty().WithMessage("A Data de Nascimento é obrigatória.")
                .Must(BeAtLeast18).WithMessage("O Usuário deve ter no mínimo 18 anos.");

            RuleFor(u => u.Telefone)
                .Matches(@"^\(?(?:[14689][1-9]|2[1-9]|3[1-5]|5[1-9]|7[134579])\)? ?(?:[2-8]|9[1-9])[0-9]{3}\-?[0-9]{4}$")
                .When(u => !string.IsNullOrEmpty(u.Telefone))
                .WithMessage("O Telefone deve estar no formato (XX) XXXXX-XXXX ou (XX) XXXX-XXXX.");
        }

        private bool BeAtLeast18(DateTime dataNascimento)
        {
            var today = DateTime.Today;
            var age = today.Year - dataNascimento.Year;
            if (dataNascimento.Date > today.AddYears(-age)) age--;
            return age >= 18;
        }
    }
}