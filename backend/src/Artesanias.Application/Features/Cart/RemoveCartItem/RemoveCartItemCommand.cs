using Artesanias.Application.Common;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Cart.RemoveCartItem;

public record RemoveCartItemCommand(Guid SessionId, Guid CartItemId) : IRequest<Result>;

public class RemoveCartItemCommandHandler(IUnitOfWork uow)
    : IRequestHandler<RemoveCartItemCommand, Result>
{
    public async Task<Result> Handle(
        RemoveCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var cart = await uow.Cart.GetOrCreateBySessionAsync(request.SessionId, cancellationToken);

        var item = cart.Items.FirstOrDefault(i => i.Id == request.CartItemId);
        if (item is null)
            return Result.Fail("El ítem no existe en el carrito.");

        cart.Items.Remove(item);
        cart.UltimaActualizacion = DateTime.UtcNow;
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok("Ítem eliminado del carrito.");
    }
}
