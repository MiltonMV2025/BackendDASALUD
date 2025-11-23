using Application.DTOs;

namespace Application.Interfaces;

public interface ICitaRepository
{
    Task<PagedResult<AppointmentRowDto>> GetPagedAsync(int page, int pageSize, string? q, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateCitaDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(EditCitaDto dto, CancellationToken cancellationToken = default);
    Task<AppointmentDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorCatalogDto>> GetDoctorsCatalogAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientCatalogDto>> GetPatientsCatalogAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentStatusDto>> GetStatusesCatalogAsync(CancellationToken cancellationToken = default);
}
