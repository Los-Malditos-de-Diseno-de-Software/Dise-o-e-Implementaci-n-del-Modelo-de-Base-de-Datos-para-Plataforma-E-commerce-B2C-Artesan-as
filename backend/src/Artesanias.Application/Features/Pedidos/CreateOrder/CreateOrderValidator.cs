using Artesanias.Application.Features.Pedidos.CreateOrder;
using FluentValidation;

namespace Artesanias.Application.Features.Pedidos.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("El SessionId es requerido.");

        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("Debes iniciar sesión para crear una orden.");

        RuleFor(x => x.DireccionEnvio)
            .NotEmpty().WithMessage("La dirección de envío es requerida.")
            .MaximumLength(500).WithMessage("La dirección no puede superar 500 caracteres.");
    }
}
