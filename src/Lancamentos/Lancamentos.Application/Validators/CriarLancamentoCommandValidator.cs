using FluentValidation;
using Lancamentos.Application.Commands;

namespace Lancamentos.Application.Validators;

public class CriarLancamentoCommandValidator : AbstractValidator<CriarLancamentoCommand>
{
    public CriarLancamentoCommandValidator()
    {
        RuleFor(x => x.Valor)
            .GreaterThan(0)
            .WithMessage("Valor deve ser maior que zero");

        RuleFor(x => x.Tipo)
            .Must(tipo => tipo == 0 || tipo == 1)
            .WithMessage("Tipo deve ser 0 (Débito) ou 1 (Crédito)");

        RuleFor(x => x.Descricao)
            .NotEmpty()
            .WithMessage("Descrição é obrigatória")
            .MaximumLength(500)
            .WithMessage("Descrição não pode ter mais de 500 caracteres");

        RuleFor(x => x.Data)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Data não pode ser futura");
    }
}