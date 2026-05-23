using Artesanias.Application.Common;
using Artesanias.Application.DTOs;
using Artesanias.Domain.Entities;
using Artesanias.Domain.Interfaces;

namespace Artesanias.Infrastructure.Identity;

public class AuthService
{
    private readonly IUnitOfWork _uow;
    private readonly JwtTokenService _jwtService;

    public AuthService(IUnitOfWork uow, JwtTokenService jwtService)
    {
        _uow = uow;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
    {
        if (await _uow.Usuarios.ExistsByEmailAsync(request.Email, ct))
        {
            return Result<AuthResponseDto>.Fail("El email ya está registrado.");
        }

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            Telefono = request.Telefono,
            Rol = Roles.Cliente,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "system",
            UpdatedBy = "system"
        };

        await _uow.Usuarios.AddAsync(usuario, ct);
        await _uow.SaveChangesAsync(ct);

        var token = _jwtService.GenerateToken(usuario);

        return Result<AuthResponseDto>.Ok(new AuthResponseDto
        {
            Id = usuario.Id,
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(24), // Hardcoded here for simplicity, in a real app read from config
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol
        }, "Usuario registrado correctamente.");
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        var usuario = await _uow.Usuarios.GetByEmailAsync(request.Email, ct);
        if (usuario is null)
        {
            return Result<AuthResponseDto>.Fail("Credenciales inválidas.");
        }

        if (!PasswordHasher.VerifyPassword(request.Password, usuario.PasswordHash))
        {
            return Result<AuthResponseDto>.Fail("Credenciales inválidas.");
        }

        var token = _jwtService.GenerateToken(usuario);

        return Result<AuthResponseDto>.Ok(new AuthResponseDto
        {
            Id = usuario.Id,
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(24),
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol
        }, "Login exitoso.");
    }
}
