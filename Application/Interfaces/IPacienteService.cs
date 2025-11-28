using Application.DTOs;

namespace Application.Interfaces;

public interface IPacienteService
{
    Task<PagedResult<PacienteDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? q,
        CancellationToken cancellationToken = default
    );

    Task<PacienteDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    );

    Task<int> CreateAsync(
        CreatePacienteDto dto,
        CancellationToken cancellationToken = default
    );

    Task<bool> UpdateAsync(
        EditPacienteDto dto,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default
    );
}
