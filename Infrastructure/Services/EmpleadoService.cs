using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Helpers; // <- para PasswordHelper

namespace Infrastructure.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly IEmpleadoRepository _empleadoRepository;

    public EmpleadoService(IEmpleadoRepository empleadoRepository)
    {
        _empleadoRepository = empleadoRepository;
    }

    // ======================================================
    // LISTADO PARA GRILLA / TABLAS
    // ======================================================
    public Task<PagedResult<EmpleadoRowDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? q,
        CancellationToken cancellationToken = default
    )
    {
        return _empleadoRepository.GetPagedAsync(page, pageSize, q, cancellationToken);
    }

    // ======================================================
    // DETALLES POR ID
    // ======================================================
    public Task<EmpleadoDetailsDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return _empleadoRepository.GetByIdAsync(id, cancellationToken);
    }

    // ======================================================
    // DETALLES POR USUARIO
    // ======================================================
    public async Task<EmpleadoDetailsDto?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default
    )
    {
        var e = await _empleadoRepository.GetByUsernameAsync(username, cancellationToken);
        if (e == null) return null;

        return new EmpleadoDetailsDto(
            e.IdEmpleado,
            e.Usuario,
            e.IdRol,
            e.Rol?.NombreRol ?? string.Empty,
            e.IdEspecialidad == 0 ? (int?)null : e.IdEspecialidad,
            e.Especialidad?.NombreEspecialidad,
            e.Persona?.Nombres ?? string.Empty,
            e.Persona?.Apellidos ?? string.Empty,
            e.Persona?.Correo,
            e.Persona?.DUI,
            e.Persona?.Telefono,
            e.Persona?.Direccion,
            e.Activo
        );
    }

    // ======================================================
    // CREAR EMPLEADO + PERSONA (transacción dentro del repo)
    // ======================================================
    public Task<int> CreateAsync(
        CreateEmpleadoDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var persona = new Persona
        {
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            Correo = dto.Correo,
            DUI = dto.DUI,
            Telefono = dto.Telefono,
            Direccion = dto.Direccion
        };

        // Determinar la contraseña en texto plano (para luego hashearla)
        var rawPassword = string.IsNullOrWhiteSpace(dto.Password)
            ? GeneratePassword()
            : dto.Password!;

        var empleado = new Empleado
        {
            Usuario = string.IsNullOrWhiteSpace(dto.Usuario)
                ? GenerateUsername(dto.Nombres, dto.Apellidos)
                : dto.Usuario,
            // Guardar SIEMPRE el hash en la BD
            Contrasena = PasswordHelper.HashPassword(rawPassword),
            IdRol = dto.IdRol,
            IdEspecialidad = dto.IdEspecialidad ?? 0,
            Activo = dto.Activo
        };

        return _empleadoRepository.CreateAsync(persona, empleado, cancellationToken);
    }

    // ======================================================
    // ACTUALIZAR EMPLEADO + PERSONA
    // ======================================================
    public Task<bool> UpdateAsync(
        EditEmpleadoDto dto,
        CancellationToken cancellationToken = default
    )
    {
        return _empleadoRepository.UpdateAsync(dto, cancellationToken);
    }

    // ======================================================
    // CAMBIAR ESTADO ACTIVO / INACTIVO
    // ======================================================
    public Task<bool> SetActivoAsync(
        int id,
        bool activo,
        CancellationToken cancellationToken = default
    )
    {
        return _empleadoRepository.SetActivoAsync(id, activo, cancellationToken);
    }

    // ======================================================
    // FUNCIONES AUXILIARES
    // ======================================================
    private string GenerateUsername(string nombres, string apellidos)
    {
        if (string.IsNullOrWhiteSpace(nombres) || string.IsNullOrWhiteSpace(apellidos))
            return Guid.NewGuid().ToString("N")[..8].ToLower();

        // Ejemplo simple: primera letra nombre + apellido, todo en minúsculas
        return $"{nombres[0]}{apellidos}".ToLower();
    }

    private string GeneratePassword()
    {
        // Ejemplo simple: generar password aleatorio de 8 caracteres
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}