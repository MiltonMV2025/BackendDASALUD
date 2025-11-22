using Application.DTOs;

namespace Application.Interfaces;

public interface ICitaService
{
    Task<PagedResult<AppointmentRowDto>> GetPagedAsync(int page, int pageSize, string? q, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateCitaDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(EditCitaDto dto, CancellationToken cancellationToken = default);
    Task<AppointmentDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
