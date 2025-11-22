using Application.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly AppDbContext _context;

    public EmpleadoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Empleado?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Empleados
            .Include(e => e.Persona)
            .Include(e => e.Rol)
            .Include(e => e.Especialidad)
            .FirstOrDefaultAsync(e => e.Usuario == username && e.Activo, cancellationToken);
    }
}
