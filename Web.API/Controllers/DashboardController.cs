using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardRepository _repo;

    public DashboardController(IDashboardRepository dashboardRepository)
    {
        _repo = dashboardRepository;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ResponseDto<DashboardSummaryDto>>> GetSummary()
    {
        var summary = new DashboardSummaryDto
        {
            PacientesAtendidos = await _repo.GetPacientesAtendidosAsync(),
            ConsultasPendientes = await _repo.GetConsultasPendientesAsync(),
            TotalConsultas = await _repo.GetTotalConsultasAsync(),

            TotalIngresos = await _repo.GetTotalIngresosAsync(),
            DataConcurrencia = await _repo.GetConcurrenciaAsync(),

            ConsultasPorMes = await _repo.GetConsultasPorMesAsync(6),
            ConsultasPorEspecialidad = await _repo.GetConsultasPorEspecialidadAsync(6)
        };

        return Ok(new ResponseDto<DashboardSummaryDto>("Resumen obtenido correctamente", true, summary));
    }
}