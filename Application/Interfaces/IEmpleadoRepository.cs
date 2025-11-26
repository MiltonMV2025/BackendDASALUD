using Domain.Models;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<Empleado?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default
        );

        Task<PagedResult<EmpleadoRowDto>> GetPagedAsync(
            int page,
            int pageSize,
            string? q,
            CancellationToken cancellationToken = default
        );

        // Crear Persona + Empleado en la misma transacción
        Task<int> CreateAsync(
            Persona persona,
            Empleado empleado,
            CancellationToken cancellationToken = default
        );

        Task<bool> UpdateAsync(
            EditEmpleadoDto dto,
            CancellationToken cancellationToken = default
        );

        Task<EmpleadoDetailsDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        );

        // Solo cambia estado "Activo"
        Task<bool> SetActivoAsync(
            int id,
            bool activo,
            CancellationToken cancellationToken = default
        );
    }
}