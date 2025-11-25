using Application.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;

namespace Infrastructure.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly AppDbContext _context;

        public EmpleadoRepository(AppDbContext context)
        {
            _context = context;
        }

        // ----------------- GET BY USERNAME -----------------
        public async Task<Empleado?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default
        )
        {
            return await _context.Empleados
                .Include(e => e.Persona)
                .Include(e => e.Rol)
                .Include(e => e.Especialidad)
                .FirstOrDefaultAsync(e => e.Usuario == username, cancellationToken);
        }

        // ----------------- PAGINADO -----------------
        public async Task<PagedResult<EmpleadoRowDto>> GetPagedAsync(
            int page,
            int pageSize,
            string? q,
            CancellationToken cancellationToken = default
        )
        {
            var query = _context.Empleados
                .Include(e => e.Persona)
                .Include(e => e.Rol)
                .Include(e => e.Especialidad)
                .Where(e => e.Activo);

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(e =>
                    (e.Persona != null &&
                        (e.Persona.Nombres.Contains(q) || e.Persona.Apellidos.Contains(q))) ||
                    e.Usuario.Contains(q) ||
                    (e.Rol != null && e.Rol.NombreRol.Contains(q)) ||
                    (e.Especialidad != null && e.Especialidad.NombreEspecialidad.Contains(q))
                );
            }

            var totalItems = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .OrderBy(e => e.Persona != null ? e.Persona.Nombres : string.Empty)
                .ThenBy(e => e.Persona != null ? e.Persona.Apellidos : string.Empty)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmpleadoRowDto(
                    e.IdEmpleado,
                    e.Usuario,
                    e.Persona != null ? e.Persona.Nombres + " " + e.Persona.Apellidos : string.Empty,
                    e.Persona != null ? e.Persona.DUI ?? string.Empty : string.Empty,
                    e.Rol != null ? e.Rol.NombreRol : string.Empty,
                    e.Especialidad != null ? e.Especialidad.NombreEspecialidad : string.Empty,
                    e.Activo
                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<EmpleadoRowDto>(items, page, pageSize, totalItems, totalPages);
        }

        // ----------------- CREAR PERSONA + EMPLEADO -----------------
        public async Task<int> CreateAsync(
            Persona persona,
            Empleado empleado,
            CancellationToken cancellationToken = default
        )
        {
            using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                _context.Personas.Add(persona);
                await _context.SaveChangesAsync(cancellationToken);

                empleado.IdEmpleado = persona.IdPersona; // PK compartida
                _context.Empleados.Add(empleado);
                await _context.SaveChangesAsync(cancellationToken);

                await tx.CommitAsync(cancellationToken);

                return empleado.IdEmpleado;
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        // ----------------- UPDATE -----------------
        public async Task<bool> UpdateAsync(
            EditEmpleadoDto dto,
            CancellationToken cancellationToken = default
        )
        {
            var empleado = await _context.Empleados
                .Include(e => e.Persona)
                .FirstOrDefaultAsync(e => e.IdEmpleado == dto.IdEmpleado, cancellationToken);

            if (empleado == null) return false;

            empleado.IdRol = dto.IdRol;
            empleado.IdEspecialidad = dto.IdEspecialidad ?? 0;

            if (empleado.Persona != null)
            {
                empleado.Persona.Nombres = dto.Nombres;
                empleado.Persona.Apellidos = dto.Apellidos;
                empleado.Persona.Telefono = dto.Telefono;
                empleado.Persona.Correo = dto.Correo;
                empleado.Persona.Direccion = dto.Direccion;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        // ----------------- GET BY ID (DEVUELVE DTO) -----------------
        public async Task<EmpleadoDetailsDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            var e = await _context.Empleados
                .Include(x => x.Persona)
                .Include(x => x.Rol)
                .Include(x => x.Especialidad)
                .FirstOrDefaultAsync(x => x.IdEmpleado == id, cancellationToken);

            if (e == null) return null;

            return new EmpleadoDetailsDto(
                e.IdEmpleado,
                e.Usuario,
                e.IdRol,
                e.Rol != null ? e.Rol.NombreRol : string.Empty,
                e.IdEspecialidad == 0 ? (int?)null : e.IdEspecialidad,
                e.Especialidad != null ? e.Especialidad.NombreEspecialidad : null,
                e.Persona != null ? e.Persona.Nombres + " " + e.Persona.Apellidos : string.Empty,
                e.Persona?.Correo,
                e.Persona?.DUI,
                e.Persona?.Telefono,
                e.Persona?.Direccion,
                e.Activo
            );
        }

        // ----------------- SET ACTIVO -----------------
        public async Task<bool> SetActivoAsync(
            int id,
            bool activo,
            CancellationToken cancellationToken = default
        )
        {
            var empleado = await _context.Empleados.FindAsync(new object?[] { id }, cancellationToken);

            if (empleado == null) return false;

            empleado.Activo = activo;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
