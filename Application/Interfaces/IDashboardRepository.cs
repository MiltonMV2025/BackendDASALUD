using Application.DTOs;

namespace Application.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetPacientesAtendidosAsync(CancellationToken cancellationToken = default);
        Task<int> GetConsultasPendientesAsync(CancellationToken cancellationToken = default);
        Task<int> GetTotalConsultasAsync(CancellationToken cancellationToken = default);
        Task<List<ConsultasPorMesDto>> GetConsultasPorMesAsync(int months, CancellationToken cancellationToken = default);
        Task<List<ConsultasPorEspecialidadDto>> GetConsultasPorEspecialidadAsync(int top, CancellationToken cancellationToken = default);
    }
}
