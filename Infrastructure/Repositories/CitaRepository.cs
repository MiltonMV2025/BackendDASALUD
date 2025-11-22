using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CitaRepository : ICitaRepository
{
    private readonly AppDbContext _context;

    public CitaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AppointmentRowDto>> GetPagedAsync(int page, int pageSize, string? q, CancellationToken cancellationToken = default)
    {
        var query = _context.Citas
            .Include(c => c.Paciente).ThenInclude(p => p.Persona)
            .Include(c => c.Empleado).ThenInclude(e => e.Persona)
            .Include(c => c.Estado)
            .Where(c => c.Activo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(c =>
                c.Paciente.Persona.Nombres.Contains(q) ||
                c.Paciente.Persona.Apellidos.Contains(q) ||
                c.Empleado.Persona.Nombres.Contains(q) ||
                c.Empleado.Persona.Apellidos.Contains(q));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .OrderByDescending(c => c.FechaCita)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new AppointmentRowDto(
                c.IdCita,
                $"#{c.IdCita:D4}",
                c.Empleado.Persona.Nombres + " " + c.Empleado.Persona.Apellidos,
                c.Paciente.Persona.Nombres + " " + c.Paciente.Persona.Apellidos,
                c.FechaCita,
                c.Costo,
                c.Estado.NombreEstado
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<AppointmentRowDto>(items, page, pageSize, totalItems, totalPages);
    }

    public async Task<int> CreateAsync(CreateCitaDto dto, CancellationToken cancellationToken = default)
    {
        var estadoPendiente = await _context.EstadosCitas.FirstOrDefaultAsync(e => e.NombreEstado == "Pendiente", cancellationToken);
        if (estadoPendiente == null) throw new InvalidOperationException("Estado 'Pendiente' no encontrado");

        var cita = new Domain.Models.Cita
        {
            IdPaciente = dto.IdPaciente,
            IdEmpleado = dto.IdEmpleado,
            IdEstado = estadoPendiente.IdEstado,
            FechaCita = dto.FechaCita,
            Costo = dto.Costo,
            Activo = true
        };

        _context.Citas.Add(cita);
        await _context.SaveChangesAsync(cancellationToken);
        return cita.IdCita;
    }

    public async Task<bool> UpdateAsync(EditCitaDto dto, CancellationToken cancellationToken = default)
    {
        var cita = await _context.Citas.FindAsync(new object?[] { dto.IdCita }, cancellationToken);
        if (cita == null || !cita.Activo) return false;

        cita.IdPaciente = dto.IdPaciente;
        cita.IdEmpleado = dto.IdEmpleado;
        cita.IdEstado = dto.IdEstado;
        cita.FechaCita = dto.FechaCita;
        cita.Costo = dto.Costo;

        _context.Citas.Update(cita);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<AppointmentDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cita = await _context.Citas
            .Include(c => c.Paciente).ThenInclude(p => p.Persona)
            .Include(c => c.Empleado).ThenInclude(e => e.Persona)
            .Include(c => c.Empleado).ThenInclude(e => e.Especialidad)
            .Include(c => c.Estado)
            .FirstOrDefaultAsync(c => c.IdCita == id, cancellationToken);

        if (cita == null) return null;

        return new AppointmentDetailsDto(
            cita.IdCita,
            cita.IdPaciente,
            cita.Paciente.Persona.Nombres + " " + cita.Paciente.Persona.Apellidos,
            cita.IdEmpleado,
            cita.Empleado.Persona.Nombres + " " + cita.Empleado.Persona.Apellidos,
            cita.FechaCita,
            cita.Costo,
            cita.IdEstado,
            cita.Estado.NombreEstado,
            cita.Activo
        );
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var cita = await _context.Citas.FindAsync(new object?[] { id }, cancellationToken);
        if (cita == null || !cita.Activo) return false;

        cita.Activo = false;
        _context.Citas.Update(cita);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
