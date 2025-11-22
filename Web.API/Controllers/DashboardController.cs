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
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardController(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var pacientesAtendidos = await _dashboardRepository.GetPacientesAtendidosAsync();
        var consultasPendientes = await _dashboardRepository.GetConsultasPendientesAsync();
        var totalConsultas = await _dashboardRepository.GetTotalConsultasAsync();
        var consultasPorMes = await _dashboardRepository.GetConsultasPorMesAsync(6);
        var consultasPorEspecialidad = await _dashboardRepository.GetConsultasPorEspecialidadAsync(6);

        var data = new
        {
            PacientesAtendidos = pacientesAtendidos,
            ConsultasPendientes = consultasPendientes,
            TotalConsultas = totalConsultas,
            ConsultasPorMes = consultasPorMes,
            ConsultasPorEspecialidad = consultasPorEspecialidad
        };

        var response = new ResponseDto<object>("Resumen obtenido correctamente", true, data);
        return Ok(response);
    }
}
