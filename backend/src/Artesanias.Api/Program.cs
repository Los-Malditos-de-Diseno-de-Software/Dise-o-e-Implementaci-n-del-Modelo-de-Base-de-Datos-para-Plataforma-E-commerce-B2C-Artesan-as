using Artesanias.Api.Middleware;
using Artesanias.Application;
using Artesanias.Infrastructure;
using Artesanias.Infrastructure.Persistence;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Application Layer (MediatR, FluentValidation)
builder.Services.AddApplication();

// Infrastructure (DbContext, UnitOfWork, StripeService)
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar Stripe API Key globalmente
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? ["http://localhost:5173"];

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Middleware global de excepciones
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Sembrar la base de datos de manera automática
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ArtesaniasDbContext>();
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al sembrar la base de datos.");
    }
}

app.Run();

public partial class Program { }

