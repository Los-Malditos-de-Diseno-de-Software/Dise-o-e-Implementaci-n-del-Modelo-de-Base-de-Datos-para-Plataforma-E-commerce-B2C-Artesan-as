using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Artesanias.Application.DTOs;
using Artesanias.Api.Controllers;

namespace Artesanias.IntegrationTests.Controllers;

public class CarritoControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CarritoControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCart_WithoutSessionIdHeader_ReturnsInternalServerError()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/carrito");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var result = await response.Content.ReadFromJsonAsync<TestResult<object>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("error interno del servidor");
    }

    [Fact]
    public async Task Carrito_FullLifecycle_Succeeds()
    {
        // 1. Arrange
        var sessionId = Guid.NewGuid();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Session-Id", sessionId.ToString());

        var productId = Guid.Parse("22222222-2222-2222-2222-222222222222"); // Product seeded in CustomWebApplicationFactory

        // 2. Act: Get initial empty cart
        var getInitialResponse = await client.GetAsync("/api/v1/carrito");
        getInitialResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var initialCartResult = await getInitialResponse.Content.ReadFromJsonAsync<TestResult<CartDto>>();
        initialCartResult.Should().NotBeNull();
        initialCartResult!.Success.Should().BeTrue();
        initialCartResult.Data.Should().NotBeNull();
        initialCartResult.Data!.Items.Should().BeEmpty();
        initialCartResult.Data.Total.Should().Be(0);

        // 3. Act: Add item to cart
        var addRequest = new AddCartItemRequest(productId, 2);
        var addResponse = await client.PostAsJsonAsync("/api/v1/carrito/items", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var addResult = await addResponse.Content.ReadFromJsonAsync<TestResult<CartDto>>();
        addResult.Should().NotBeNull();
        addResult!.Success.Should().BeTrue();
        addResult.Data.Should().NotBeNull();
        addResult.Data!.Items.Should().ContainSingle(i => i.ProductoId == productId && i.Cantidad == 2);
        addResult.Data.Total.Should().Be(2 * 99.99m); // price seeded is 99.99m

        // Get the cart item ID
        var cartItemId = addResult.Data.Items.First().Id;

        // 4. Act: Get cart again to verify persistence
        var getCartResponse2 = await client.GetAsync("/api/v1/carrito");
        getCartResponse2.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var cartResult2 = await getCartResponse2.Content.ReadFromJsonAsync<TestResult<CartDto>>();
        cartResult2.Should().NotBeNull();
        cartResult2!.Success.Should().BeTrue();
        cartResult2.Data!.Items.Should().ContainSingle(i => i.ProductoId == productId && i.Cantidad == 2);

        // 5. Act: Remove item from cart
        var removeResponse = await client.DeleteAsync($"/api/v1/carrito/items/{cartItemId}");
        removeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var removeResult = await removeResponse.Content.ReadFromJsonAsync<TestResult<object>>();
        removeResult.Should().NotBeNull();
        removeResult!.Success.Should().BeTrue();

        // 6. Act: Get cart again to verify item was removed
        var getCartResponse3 = await client.GetAsync("/api/v1/carrito");
        getCartResponse3.StatusCode.Should().Be(HttpStatusCode.OK);

        var cartResult3 = await getCartResponse3.Content.ReadFromJsonAsync<TestResult<CartDto>>();
        cartResult3.Should().NotBeNull();
        cartResult3!.Success.Should().BeTrue();
        cartResult3.Data.Should().NotBeNull();
        cartResult3.Data!.Items.Should().BeEmpty();
        cartResult3.Data.Total.Should().Be(0);
    }
}
