using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetPacientesAtendidosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Citas
            .Where(c => c.Estado.NombreEstado == "Completada" && c.Activo)
            .Select(c => c.IdPaciente)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetConsultasPendientesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Citas
            .Where(c => (c.Estado.NombreEstado == "Pendiente" || c.Estado.NombreEstado == "Confirmada") && c.Activo)
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetTotalConsultasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Citas
            .Where(c => c.Activo)
            .CountAsync(cancellationToken);
    }

    public async Task<List<ConsultasPorMesDto>> GetConsultasPorMesAsync(int months, CancellationToken cancellationToken = default)
    {
        var sixMonthsAgo = DateTime.Now.AddMonths(-months);
        var consultasPorMes = await _context.Citas
            .Where(c => c.FechaCita >= sixMonthsAgo && c.Activo)
            .GroupBy(c => new { c.FechaCita.Year, c.FechaCita.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count()
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync(cancellationToken);

        string[] meses = { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };

        return consultasPorMes.Select(c => new ConsultasPorMesDto(meses[c.Month - 1], c.Count)).ToList();
    }

    public async Task<List<ConsultasPorEspecialidadDto>> GetConsultasPorEspecialidadAsync(int top, CancellationToken cancellationToken = default)
    {
        // Group by primitive values to ensure EF Core can translate the query to SQL
        var query = _context.Citas
            .Include(c => c.Empleado).ThenInclude(e => e.Especialidad)
            .Where(c => c.Activo)
            .GroupBy(c => new { IdEspecialidad = c.Empleado.IdEspecialidad, Nombre = c.Empleado.Especialidad.NombreEspecialidad })
            .Select(g => new { g.Key.IdEspecialidad, g.Key.Nombre, Cantidad = g.Count() })
            .OrderByDescending(x => x.Cantidad)
            .Take(top);

        var list = await query.ToListAsync(cancellationToken);

        return list.Select(x =>
            new ConsultasPorEspecialidadDto(
                x.Nombre,
                x.Nombre != null && x.Nombre.Length >= 3 ? x.Nombre.Substring(0, 3).ToUpper() : (x.Nombre ?? string.Empty).ToUpper(),
                x.Cantidad
            )).ToList();
    }
}
