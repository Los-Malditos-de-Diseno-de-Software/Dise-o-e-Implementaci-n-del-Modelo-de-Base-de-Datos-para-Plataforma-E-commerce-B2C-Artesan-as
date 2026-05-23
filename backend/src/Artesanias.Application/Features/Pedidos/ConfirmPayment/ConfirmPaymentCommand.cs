using Artesanias.Application.Common;
using Artesanias.Domain.Entities;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Pedidos.ConfirmPayment;

/// <summary>
/// Llamado desde PagosController al recibir el webhook de Stripe 'checkout.session.completed'.
/// Ejecuta la transacción ACID completa:
///   1. Verificar StripeSessionId en PaymentTransactions
///   2. Marcar PaymentTransaction como Pagado
///   3. Marcar Order como Pagado
///   4. Descontar stock de cada Producto
///   5. Eliminar ShoppingCart + CartItems del SessionId
/// Todo o nada dentro de una transacción SQL.
/// </summary>
public record ConfirmPaymentCommand(
    string StripeSessionId,
    string StripePaymentIntentId,
    string RawPayload
) : IRequest<Result>;

public class ConfirmPaymentCommandHandler(IUnitOfWork uow)
    : IRequestHandler<ConfirmPaymentCommand, Result>
{
    public async Task<Result> Handle(
        ConfirmPaymentCommand request,
        CancellationToken cancellationToken)
    {
        await uow.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Buscar PaymentTransaction por StripeSessionId
            var paymentTx = await uow.PaymentTransactions
                .GetByStripeSessionIdAsync(request.StripeSessionId, cancellationToken);

            if (paymentTx is null)
                return Result.Fail($"PaymentTransaction no encontrada para session {request.StripeSessionId}.");

            // Idempotencia: si ya fue confirmado, retornar OK sin reprocessar
            if (paymentTx.EstadoPago == EstadosPago.Pagado)
                return Result.Ok("Pago ya confirmado previamente (idempotente).");

            // 2. Obtener la Order con sus Items
            var order = await uow.Orders.GetByIdWithItemsAsync(paymentTx.OrderId, cancellationToken);
            if (order is null)
                return Result.Fail($"Order {paymentTx.OrderId} no encontrada.");

            // 3. Actualizar PaymentTransaction → Pagado
            paymentTx.EstadoPago = EstadosPago.Pagado;
            paymentTx.StripePaymentIntentId = request.StripePaymentIntentId;
            paymentTx.ReferenciaPasarela = request.StripePaymentIntentId;
            paymentTx.PayloadPasarela = request.RawPayload;
            paymentTx.UpdatedAt = DateTime.UtcNow;
            uow.PaymentTransactions.Update(paymentTx);

            // 4. Actualizar Order → Pagado
            order.EstadoPedido = EstadosPedido.Pagado;
            order.UpdatedAt = DateTime.UtcNow;
            uow.Orders.Update(order);

            // 5. Descontar stock de cada producto
            foreach (var orderItem in order.Items)
            {
                var producto = await uow.Productos.GetByIdAsync(orderItem.ProductoId, cancellationToken);
                if (producto is null)
                    throw new InvalidOperationException($"Producto {orderItem.ProductoId} no encontrado al confirmar pago.");

                if (producto.Stock < orderItem.Cantidad)
                    throw new InvalidOperationException(
                        $"Stock insuficiente para '{producto.Nombre}' al confirmar. Stock actual: {producto.Stock}.");

                producto.Stock -= orderItem.Cantidad;
                producto.UpdatedAt = DateTime.UtcNow;
                uow.Productos.Update(producto);
            }

            // 6. Eliminar el carrito del usuario (por OrderId → UsuarioId → SessionId)
            await uow.Cart.DeleteByUsuarioIdAsync(order.UsuarioId, cancellationToken);

            // 7. Commit — todo o nada
            await uow.SaveChangesAsync(cancellationToken);
            await uow.CommitTransactionAsync(cancellationToken);

            return Result.Ok("Pago confirmado. Stock descontado y carrito eliminado.");
        }
        catch (Exception)
        {
            await uow.RollbackTransactionAsync(cancellationToken);
            throw; // el ExceptionMiddleware lo captura y devuelve 500
        }
    }
}
