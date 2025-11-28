using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PacienteRepository : IPacienteRepository
{
    private readonly AppDbContext _context;

    public PacienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PacienteDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? q,
        CancellationToken cancellationToken = default
    )
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _context.Pacientes
            .Include(p => p.Persona)
            .Where(p => p.Activo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();

            query = query.Where(p =>
                p.Persona.Nombres.Contains(q) ||
                p.Persona.Apellidos.Contains(q) ||
                (p.Persona.Correo != null && p.Persona.Correo.Contains(q)) ||
                (p.Persona.Telefono != null && p.Persona.Telefono.Contains(q)) ||
                (p.Persona.DUI != null && p.Persona.DUI.Contains(q)) ||        // DUI de la entidad
                (p.Persona.Direccion != null && p.Persona.Direccion.Contains(q)) ||
                (p.TipoSangre != null && p.TipoSangre.Contains(q))
            );
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .OrderBy(p => p.Persona.Nombres)
            .ThenBy(p => p.Persona.Apellidos)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PacienteDto
            {
                IdPaciente = p.IdPaciente,
                Nombres = p.Persona.Nombres,
                Apellidos = p.Persona.Apellidos,
                Dui = p.Persona.DUI,                 // mapea DUI -> Dui del DTO
                Direccion = p.Persona.Direccion,
                Correo = p.Persona.Correo,
                Telefono = p.Persona.Telefono,
                TipoSangre = p.TipoSangre,
                Peso = p.Peso,
                Altura = p.Altura
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PacienteDto>(items, page, pageSize, totalItems, totalPages);
    }

    public async Task<PacienteDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var p = await _context.Pacientes
            .Include(p => p.Persona)
            .FirstOrDefaultAsync(p => p.IdPaciente == id && p.Activo, cancellationToken);

        if (p == null) return null;

        return new PacienteDto
        {
            IdPaciente = p.IdPaciente,
            Nombres = p.Persona.Nombres,
            Apellidos = p.Persona.Apellidos,
            Dui = p.Persona.DUI,                 // igual aquí
            Direccion = p.Persona.Direccion,
            Correo = p.Persona.Correo,
            Telefono = p.Persona.Telefono,
            TipoSangre = p.TipoSangre,
            Peso = p.Peso,
            Altura = p.Altura
        };
    }

    public async Task<int> CreateAsync(
        CreatePacienteDto dto,
        CancellationToken cancellationToken = default
    )
    {
        // 1) Crear PERSONA (IDENTITY)
        var persona = new Persona
        {
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            Correo = dto.Correo,
            Telefono = dto.Telefono,
            DUI = dto.Dui,                  // DTO.Dui -> entidad.DUI
            Direccion = dto.Direccion
        };

        _context.Personas.Add(persona);
        await _context.SaveChangesAsync(cancellationToken); // genera persona.IdPersona

        // 2) Crear PACIENTE con el mismo id
        var paciente = new Paciente
        {
            IdPaciente = persona.IdPersona, // FK = PK Persona
            TipoSangre = dto.TipoSangre,
            Peso = dto.Peso,
            Altura = dto.Altura,
            Activo = true
        };

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync(cancellationToken);

        return paciente.IdPaciente;
    }

    public async Task<bool> UpdateAsync(
        EditPacienteDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var paciente = await _context.Pacientes
            .Include(p => p.Persona)
            .FirstOrDefaultAsync(p => p.IdPaciente == dto.IdPaciente, cancellationToken);

        if (paciente == null || !paciente.Activo) return false;

        paciente.Persona.Nombres = dto.Nombres;
        paciente.Persona.Apellidos = dto.Apellidos;
        paciente.Persona.Correo = dto.Correo;
        paciente.Persona.Telefono = dto.Telefono;
        paciente.Persona.DUI = dto.Dui;            // DTO.Dui -> entidad.DUI
        paciente.Persona.Direccion = dto.Direccion;

        paciente.TipoSangre = dto.TipoSangre;
        paciente.Peso = dto.Peso;
        paciente.Altura = dto.Altura;

        _context.Pacientes.Update(paciente);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> SoftDeleteAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.IdPaciente == id, cancellationToken);

        if (paciente == null || !paciente.Activo) return false;

        paciente.Activo = false;
        _context.Pacientes.Update(paciente);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
