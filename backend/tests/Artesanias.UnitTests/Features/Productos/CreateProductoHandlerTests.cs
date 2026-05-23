using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Artesanias.Application.Features.Productos.CreateProducto;
using Artesanias.Domain.Entities;
using Artesanias.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Artesanias.UnitTests.Features.Productos;

public class CreateProductoHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IProductoRepository> _productosRepoMock;
    private readonly CreateProductoCommandHandler _handler;

    public CreateProductoHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _productosRepoMock = new Mock<IProductoRepository>();
        _uowMock.Setup(x => x.Productos).Returns(_productosRepoMock.Object);
        
        // Mock default behavior for Artesanos to prevent other null reference issues
        _uowMock.Setup(x => x.Artesanos).Returns(new Mock<IArtesanoRepository>().Object);

        _handler = new CreateProductoCommandHandler(_uowMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenArtesanoDoesNotExist()
    {
        // Arrange
        var command = new CreateProductoCommand(Guid.NewGuid(), "Manta Andina", "Manta de lana", 120.00m, 5, false, null, null);
        var artesanosRepoMock = new Mock<IArtesanoRepository>();
        artesanosRepoMock.Setup(x => x.ExistsAsync(command.ArtesanoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _uowMock.Setup(x => x.Artesanos).Returns(artesanosRepoMock.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Artesano");
        result.Message.Should().Contain("no encontrado");
    }

    [Fact]
    public async Task Handle_ShouldCreateProductWithoutImage_WhenNoImageDataProvided()
    {
        // Arrange
        var command = new CreateProductoCommand(Guid.NewGuid(), "Manta Andina", "Manta de lana", 120.00m, 5, false, null, null);
        var artesanosRepoMock = new Mock<IArtesanoRepository>();
        artesanosRepoMock.Setup(x => x.ExistsAsync(command.ArtesanoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _uowMock.Setup(x => x.Artesanos).Returns(artesanosRepoMock.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("creado correctamente");
        result.Data.Should().NotBeNull();
        result.Data.Nombre.Should().Be(command.Nombre);
        result.Data.ImagenBase64.Should().BeNull();

        _productosRepoMock.Verify(x => x.AddAsync(It.Is<Producto>(p => p.Imagenes.Count == 0), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateProductWithImage_WhenImageIsProvided()
    {
        // Arrange
        var imageData = new byte[] { 1, 2, 3, 4, 5 };
        var contentType = "image/png";
        var command = new CreateProductoCommand(Guid.NewGuid(), "Vasija Cusco", "Vasija de barro", 80.00m, 2, true, imageData, contentType);
        var artesanosRepoMock = new Mock<IArtesanoRepository>();
        artesanosRepoMock.Setup(x => x.ExistsAsync(command.ArtesanoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _uowMock.Setup(x => x.Artesanos).Returns(artesanosRepoMock.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.ImagenBase64.Should().Contain("data:image/png;base64,");

        _productosRepoMock.Verify(x => x.AddAsync(It.Is<Producto>(p => 
            p.Imagenes.Count == 1 && 
            p.Imagenes.First().ImageData == imageData && 
            p.Imagenes.First().ContentType == contentType && 
            p.Imagenes.First().EsPrincipal == true), It.IsAny<CancellationToken>()), Times.Once);
        
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
