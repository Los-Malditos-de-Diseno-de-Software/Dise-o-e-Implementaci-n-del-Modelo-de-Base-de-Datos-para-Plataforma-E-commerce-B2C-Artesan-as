using Artesanias.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Artesanias.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(ArtesaniasDbContext context)
    {
        // 0. Seed Administrator
        if (!await context.Usuarios.AnyAsync())
        {
            var adminUser = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = "Administrador",
                Apellido = "Cusco",
                Email = "admin@artesanias.com",
                PasswordHash = Identity.PasswordHasher.HashPassword("AdminPassword123"),
                Rol = Roles.Administrador,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = "System"
            };
            await context.Usuarios.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }

        if (await context.Artesanos.AnyAsync())
        {
            return; // Already seeded
        }

        // 1. Seed Artesanos
        var artesanos = new List<Artesano>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Mamerto Sánchez",
                HistoriaBiografia = "Maestro alfarero de la comunidad de Pucará, dedicado a preservar las técnicas de cerámica incaica y colonial por más de 40 años.",
                ComunidadOrigen = "Pucará, Puno - Cusco",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = "System"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Nilda Callañaupa",
                HistoriaBiografia = "Tejedora tradicional de Chinchero y fundadora del Centro de Textiles Tradicionales de Cusco (CTTC), rescatando diseños ancestrales en fibra de alpaca.",
                ComunidadOrigen = "Chinchero, Cusco",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = "System"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Silvia Quispe",
                HistoriaBiografia = "Orfebre cusqueña especializada en joyería fina de plata de 950 con incrustaciones de piedras semipreciosas como turquesas y spondylus.",
                ComunidadOrigen = "San Blas, Cusco",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = "System"
            }
        };

        await context.Artesanos.AddRangeAsync(artesanos);
        await context.SaveChangesAsync();

        // 2. Seed Productos
        // A nice valid blue-colored tiny PNG base64 to render nicely in the browser
        byte[] defaultImageData = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAABmJLR0QA/wD/AP+gvaeTAAAAI0lEQVR42u3BAQ0AAADCoPdPbQ43oAAAAAAAAAAAAAAAAAAAvgwFtgABj+b2OQAAAABJRU5ErkJggg==");

        var productos = new List<Producto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ArtesanoId = artesanos[0].Id,
                Nombre = "Toro de Pucará Tradicional",
                Descripcion = "Cerámica utilitaria pintada a mano con simbología andina de prosperidad y protección para el hogar. Altura: 25cm.",
                Precio = 85.00m,
                Stock = 12,
                EsUnico = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = "System"
            },
            new()
            {
                Id = Guid.NewGuid(),
                ArtesanoId = artesanos[1].Id,
                Nombre = "Manta Andina Chinchero Lujo",
                Descripcion = "Tejido a mano en telar de cintura con lana de alpaca de la más alta calidad teñida naturalmente con cochinilla y plantas locales.",
                Precio = 450.00m,
                Stock = 1,
                EsUnico = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = "System"
            },
            new()
            {
                Id = Guid.NewGuid(),
                ArtesanoId = artesanos[2].Id,
                Nombre = "Collar Chakana en Plata 950",
                Descripcion = "Pendiente Chakana (Cruz Andina) de plata pura con incrustaciones de nácar y piedra de turquesa cusqueña hecha a mano.",
                Precio = 180.00m,
                Stock = 5,
                EsUnico = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = "System"
            },
            new()
            {
                Id = Guid.NewGuid(),
                ArtesanoId = artesanos[1].Id,
                Nombre = "Chuspa de Alpaca para Ofrendas",
                Descripcion = "Pequeño bolso tradicional utilizado para guardar hojas de coca, decorado con pompones y patrones tradicionales.",
                Precio = 65.00m,
                Stock = 15,
                EsUnico = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = "System"
            }
        };

        await context.Productos.AddRangeAsync(productos);
        await context.SaveChangesAsync();

        // 3. Seed Product Images
        var imagenes = productos.Select(p => new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductoId = p.Id,
            ImageData = defaultImageData,
            ContentType = "image/png",
            EsPrincipal = true,
            CreatedAt = DateTime.UtcNow
        });

        await context.ProductImages.AddRangeAsync(imagenes);
        await context.SaveChangesAsync();
    }
}
