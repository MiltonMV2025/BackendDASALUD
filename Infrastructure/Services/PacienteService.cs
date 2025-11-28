using Application.DTOs;
using Application.Interfaces;

namespace Infrastructure.Services;

public class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _pacienteRepository;

    public PacienteService(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public Task<PagedResult<PacienteDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? q,
        CancellationToken cancellationToken = default
    )
    {
        return _pacienteRepository.GetPagedAsync(page, pageSize, q, cancellationToken);
    }

    public Task<PacienteDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return _pacienteRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<int> CreateAsync(
        CreatePacienteDto dto,
        CancellationToken cancellationToken = default
    )
    {
        return _pacienteRepository.CreateAsync(dto, cancellationToken);
    }

    public Task<bool> UpdateAsync(
        EditPacienteDto dto,
        CancellationToken cancellationToken = default
    )
    {
        return _pacienteRepository.UpdateAsync(dto, cancellationToken);
    }

    public Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return _pacienteRepository.SoftDeleteAsync(id, cancellationToken);
    }
}
