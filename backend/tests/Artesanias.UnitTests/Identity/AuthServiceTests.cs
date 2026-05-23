using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Artesanias.Application.DTOs;
using Artesanias.Domain.Entities;
using Artesanias.Domain.Interfaces;
using Artesanias.Infrastructure.Identity;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Artesanias.UnitTests.Identity;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IUsuarioRepository> _usuariosRepoMock;
    private readonly JwtTokenService _jwtService;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _usuariosRepoMock = new Mock<IUsuarioRepository>();
        _uowMock.Setup(x => x.Usuarios).Returns(_usuariosRepoMock.Object);

        // Setup real in-memory configuration for JWT settings
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:SecretKey", "super_secret_key_that_is_long_enough_to_be_secure_1234567890" },
            { "Jwt:Issuer", "ArtesaniasCusco" },
            { "Jwt:Audience", "ArtesaniasCuscoClient" },
            { "Jwt:ExpirationHours", "24" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _jwtService = new JwtTokenService(configuration);
        _authService = new AuthService(_uowMock.Object, _jwtService);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFail_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Nombre = "Saul",
            Apellido = "Perez",
            Email = "saul@example.com",
            Password = "Password123",
            Telefono = "987654321"
        };
        
        _usuariosRepoMock.Setup(x => x.ExistsByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("El email ya está registrado");
        _usuariosRepoMock.Verify(x => x.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndReturnToken_WhenEmailIsValid()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Nombre = "Saul",
            Apellido = "Perez",
            Email = "saul@example.com",
            Password = "Password123",
            Telefono = "987654321"
        };

        _usuariosRepoMock.Setup(x => x.ExistsByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _authService.RegisterAsync(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("correctamente");
        result.Data.Should().NotBeNull();
        result.Data.Token.Should().NotBeNullOrEmpty();
        result.Data.Email.Should().Be(request.Email);
        result.Data.Rol.Should().Be(Roles.Cliente);

        _usuariosRepoMock.Verify(x => x.AddAsync(It.Is<Usuario>(u => 
            u.Email == request.Email && 
            u.Nombre == request.Nombre && 
            u.Apellido == request.Apellido &&
            u.Rol == Roles.Cliente), It.IsAny<CancellationToken>()), Times.Once);

        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFail_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "saul@example.com",
            Password = "Password123"
        };

        _usuariosRepoMock.Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario)null);

        // Act
        var result = await _authService.LoginAsync(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Credenciales inválidas");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFail_WhenPasswordIncorrect()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "saul@example.com",
            Password = "Password123"
        };
        var hash = PasswordHasher.HashPassword("RealPassword123");
        var usuario = new Usuario { Email = request.Email, PasswordHash = hash };

        _usuariosRepoMock.Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        // Act
        var result = await _authService.LoginAsync(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Credenciales inválidas");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "saul@example.com",
            Password = "Password123"
        };
        var hash = PasswordHasher.HashPassword(request.Password);
        var usuario = new Usuario { Id = Guid.NewGuid(), Email = request.Email, Nombre = "Saul", Apellido = "Perez", PasswordHash = hash, Rol = Roles.Cliente };

        _usuariosRepoMock.Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        // Act
        var result = await _authService.LoginAsync(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("Login exitoso");
        result.Data.Should().NotBeNull();
        result.Data.Token.Should().NotBeNullOrEmpty();
        result.Data.Email.Should().Be(request.Email);
    }
}
