using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IEmpleadoRepository empleadoRepository, IConfiguration configuration)
    {
        _empleadoRepository = empleadoRepository;
        _configuration = configuration;
    }

    public async Task<AuthResultDto?> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var hashed = PasswordHelper.HashPassword(password);

        var empleado = await _empleadoRepository.GetByUsernameAsync(username, cancellationToken);
        if (empleado == null) return null;

        if (empleado.Contrasena != hashed || !empleado.Activo) return null;

        var secret = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var minutesValue = _configuration["Jwt:ExpiresMinutes"];
        var minutes = 60;
        if (!string.IsNullOrEmpty(minutesValue) && int.TryParse(minutesValue, out var m)) minutes = m;

        if (string.IsNullOrEmpty(secret)) return null;

        var claims = new List<Claim>
        {
            new Claim("userId", empleado.IdEmpleado.ToString()),
            new Claim(ClaimTypes.Name, empleado.Usuario),
            new Claim(ClaimTypes.Email, empleado.Persona.Correo ?? string.Empty),
            new Claim(ClaimTypes.Role, empleado.Rol.NombreRol),
            new Claim("IdRol", empleado.IdRol.ToString()),
            new Claim("IdEspecialidad", empleado.IdEspecialidad.ToString()),
            new Claim("Especialidad", empleado.Especialidad?.NombreEspecialidad ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(minutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var fullName = $"{empleado.Persona.Nombres} {empleado.Persona.Apellidos}";

        return new AuthResultDto(
            Token: tokenString,
            UserId: empleado.IdEmpleado,
            Username: empleado.Usuario,
            Nombres: empleado.Persona.Nombres,
            Apellidos: empleado.Persona.Apellidos,
            FullName: fullName,
            Especialidad: empleado.Especialidad?.NombreEspecialidad ?? string.Empty,
            IdRol: empleado.IdRol,
            IdEspecialidad: empleado.IdEspecialidad,
            RoleName: empleado.Rol.NombreRol
        );
    }
}
