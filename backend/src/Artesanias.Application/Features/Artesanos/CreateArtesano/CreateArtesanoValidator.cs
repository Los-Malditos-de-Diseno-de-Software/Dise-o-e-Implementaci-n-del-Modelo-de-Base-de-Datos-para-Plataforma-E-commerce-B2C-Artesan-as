using Artesanias.Application.Features.Artesanos.CreateArtesano;
using FluentValidation;

namespace Artesanias.Application.Features.Artesanos.CreateArtesano;

public class CreateArtesanoValidator : AbstractValidator<CreateArtesanoCommand>
{
    public CreateArtesanoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del artesano es requerido.")
            .MaximumLength(150).WithMessage("El nombre no puede superar 150 caracteres.");

        RuleFor(x => x.HistoriaBiografia)
            .NotEmpty().WithMessage("La historia/biografía es requerida.")
            .MaximumLength(5000).WithMessage("La historia no puede superar 5000 caracteres.");

        RuleFor(x => x.ComunidadOrigen)
            .NotEmpty().WithMessage("La comunidad de origen es requerida.")
            .MaximumLength(200).WithMessage("La comunidad no puede superar 200 caracteres.");
    }
}
