using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Artesanias.Infrastructure.Persistence;
using Artesanias.Domain.Entities;
using System.Linq;
using System;

namespace Artesanias.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove real DbContext
            var dbContextOptions = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ArtesaniasDbContext>));
            if (dbContextOptions != null)
            {
                services.Remove(dbContextOptions);
            }

            var dbContext = services.SingleOrDefault(
                d => d.ServiceType == typeof(ArtesaniasDbContext));
            if (dbContext != null)
            {
                services.Remove(dbContext);
            }

            // Register DbContext with a separate internal service provider to avoid collision
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<ArtesaniasDbContext>(options =>
            {
                options.UseInMemoryDatabase("IntegrationTestDb")
                       .UseInternalServiceProvider(serviceProvider);
            });

            // Build service provider
            var sp = services.BuildServiceProvider();

            // Create scope to seed test data
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ArtesaniasDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            try
            {
                SeedTestData(db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {Message}", ex.Message);
            }
        });
    }

    private void SeedTestData(ArtesaniasDbContext db)
    {
        // Seed default Artesanos
        var artesano = new Artesano
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Nombre = "Artesano de Prueba",
            HistoriaBiografia = "Biografía de prueba",
            ComunidadOrigen = "Cusco",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System",
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "System",
            IsDeleted = false
        };
        db.Artesanos.Add(artesano);

        // Seed default Products
        var producto = new Producto
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ArtesanoId = artesano.Id,
            Nombre = "Producto de Prueba",
            Descripcion = "Descripción de prueba",
            Precio = 99.99m,
            Stock = 10,
            EsUnico = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System",
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "System",
            IsDeleted = false
        };
        db.Productos.Add(producto);

        // Seed product image
        var image = new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductoId = producto.Id,
            ImageData = new byte[] { 1, 2, 3 },
            ContentType = "image/jpeg",
            EsPrincipal = true,
            CreatedAt = DateTime.UtcNow
        };
        db.ProductImages.Add(image);

        // Seed standard administrator
        var admin = new Usuario
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Email = "admin@artesanias.com",
            PasswordHash = Artesanias.Infrastructure.Identity.PasswordHasher.HashPassword("AdminPassword123"),
            Rol = "Administrador",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System",
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "System",
            IsDeleted = false
        };
        db.Usuarios.Add(admin);

        db.SaveChanges();
    }
}
