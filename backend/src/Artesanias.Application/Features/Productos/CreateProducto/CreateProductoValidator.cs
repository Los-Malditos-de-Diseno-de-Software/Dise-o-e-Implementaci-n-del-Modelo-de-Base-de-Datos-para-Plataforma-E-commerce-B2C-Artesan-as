using Artesanias.Application.Features.Productos.CreateProducto;
using FluentValidation;

namespace Artesanias.Application.Features.Productos.CreateProducto;

public class CreateProductoValidator : AbstractValidator<CreateProductoCommand>
{
    private static readonly string[] ContentTypesPermitidos = ["image/jpeg", "image/png", "image/webp"];

    public CreateProductoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del producto es requerido.")
            .MaximumLength(200).WithMessage("El nombre no puede superar 200 caracteres.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es requerida.")
            .MaximumLength(2000).WithMessage("La descripción no puede superar 2000 caracteres.");

        RuleFor(x => x.Precio)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.")
            .LessThan(100_000).WithMessage("El precio no puede superar 100,000.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");

        RuleFor(x => x.ArtesanoId)
            .NotEmpty().WithMessage("El ArtesanoId es requerido.");

        When(x => x.ImagenData is not null, () =>
        {
            RuleFor(x => x.ImagenData!)
                .Must(d => d.Length <= 5 * 1024 * 1024)
                .WithMessage("La imagen no puede superar 5 MB.");

            RuleFor(x => x.ImagenContentType!)
                .Must(ct => ContentTypesPermitidos.Contains(ct))
                .WithMessage("El tipo de imagen debe ser JPEG, PNG o WebP.");
        });
    }
}
