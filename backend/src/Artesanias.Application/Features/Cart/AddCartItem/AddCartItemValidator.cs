using Artesanias.Application.Features.Cart.AddCartItem;
using FluentValidation;

namespace Artesanias.Application.Features.Cart.AddCartItem;

public class AddCartItemValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("El SessionId es requerido.");

        RuleFor(x => x.ProductoId)
            .NotEmpty().WithMessage("El ProductoId es requerido.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.")
            .LessThanOrEqualTo(100).WithMessage("No se pueden agregar más de 100 unidades del mismo producto.");
    }
}
