using Artesanias.Application.DTOs;
using Artesanias.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Artesanias.Api.Controllers;

[AllowAnonymous]
public class AuthController : BaseApiController
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        return HandleResult(await _authService.RegisterAsync(request));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        return HandleResult(await _authService.LoginAsync(request));
    }
}
