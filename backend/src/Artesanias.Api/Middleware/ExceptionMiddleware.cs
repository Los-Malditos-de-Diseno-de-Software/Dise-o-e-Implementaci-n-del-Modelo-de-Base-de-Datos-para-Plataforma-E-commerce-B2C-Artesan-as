using System.Net;
using System.Text.Json;
using Artesanias.Application.Common;
using FluentValidation;

namespace Artesanias.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió una excepción no controlada.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            ValidationException ve => HandleValidationException(context, ve),
            _ => HandleDefaultException(context, exception)
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });

        await context.Response.WriteAsync(json);
    }

    private static Result HandleValidationException(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
        return Result.Fail(errors);
    }

    private static Result HandleDefaultException(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        // En producción no deberíamos mostrar el detalle del error, pero para desarrollo está bien.
        return Result.Fail("Ha ocurrido un error interno del servidor.");
    }
}
