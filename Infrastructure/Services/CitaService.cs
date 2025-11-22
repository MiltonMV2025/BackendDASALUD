using Application.DTOs;
using Application.Interfaces;

namespace Infrastructure.Services;

public class CitaService : ICitaService
{
    private readonly ICitaRepository _citaRepository;

    public CitaService(ICitaRepository citaRepository)
    {
        _citaRepository = citaRepository;
    }

    public Task<int> CreateAsync(CreateCitaDto dto, CancellationToken cancellationToken = default)
    {
        return _citaRepository.CreateAsync(dto, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return _citaRepository.SoftDeleteAsync(id, cancellationToken);
    }

    public Task<AppointmentDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _citaRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<PagedResult<AppointmentRowDto>> GetPagedAsync(int page, int pageSize, string? q, CancellationToken cancellationToken = default)
    {
        return _citaRepository.GetPagedAsync(page, pageSize, q, cancellationToken);
    }

    public Task<bool> UpdateAsync(EditCitaDto dto, CancellationToken cancellationToken = default)
    {
        return _citaRepository.UpdateAsync(dto, cancellationToken);
    }
}
