using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Artesanias.Application.DTOs;

namespace Artesanias.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "juan.perez@example.com",
            Password = "Password123!",
            Telefono = "987654321"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<TestResult<AuthResponseDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.Email.Should().Be(request.Email);
        result.Data.Nombre.Should().Be("Juan");
        result.Data.Rol.Should().Be("Cliente");
        result.Data.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        // admin@artesanias.com already exists in seeded data
        var request = new RegisterRequestDto
        {
            Nombre = "Admin",
            Apellido = "Duplicate",
            Email = "admin@artesanias.com",
            Password = "Password123!",
            Telefono = "0000"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<TestResult<AuthResponseDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("ya está registrado");
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "admin@artesanias.com",
            Password = "AdminPassword123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<TestResult<AuthResponseDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.Rol.Should().Be("Administrador");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "admin@artesanias.com",
            Password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<TestResult<AuthResponseDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("Credenciales inválidas");
    }
}

public class TestResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = [];
}
