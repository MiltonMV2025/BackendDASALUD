using Application.DTOs;

namespace Application.Interfaces
{
    public interface IEmpleadoService
    {
        Task<PagedResult<EmpleadoRowDto>> GetPagedAsync(
            int page,
            int pageSize,
            string? q,
            CancellationToken cancellationToken = default
        );

        // Crear Persona + Empleado (flujo opción A sin hashing)
        Task<int> CreateAsync(
            CreateEmpleadoDto dto,
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

        // Activar / desactivar (SoftDelete mejorado)
        Task<bool> SetActivoAsync(
            int id,
            bool activo,
            CancellationToken cancellationToken = default
        );

        // Obtener empleado por su username exacto
        Task<EmpleadoDetailsDto?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default
        );
    }
}
