using Artesanias.Application.Features.Cart.AddCartItem;
using Artesanias.Application.Features.Productos.CreateProducto;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Artesanias.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationServiceExtensions).Assembly;

        // Registra todos los handlers de MediatR en este ensamblado
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Registra todos los validadores de FluentValidation en este ensamblado
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
