using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CitasController : ControllerBase
{
    private readonly ICitaService _citaService;

    public CitasController(ICitaService citaService)
    {
        _citaService = citaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? q = null)
    {
        var result = await _citaService.GetPagedAsync(page, pageSize, q);
        var response = new ResponseDto<PagedResult<AppointmentRowDto>>("Citas obtenidas", true, result);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _citaService.GetByIdAsync(id);
        if (dto == null) return NotFound(new ResponseDto<object>("Cita no encontrada", false, null));
        return Ok(new ResponseDto<AppointmentDetailsDto>("Cita encontrada", true, dto));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCitaDto dto)
    {
        var id = await _citaService.CreateAsync(dto);
        return Ok(new ResponseDto<int>("Cita creada", true, id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EditCitaDto dto)
    {
        if (id != dto.IdCita) return BadRequest(new ResponseDto<object>("Id no coincide", false, null));
        var ok = await _citaService.UpdateAsync(dto);
        if (!ok) return NotFound(new ResponseDto<object>("Cita no encontrada", false, null));
        return Ok(new ResponseDto<object>("Cita actualizada", true, null));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _citaService.DeleteAsync(id);
        if (!ok) return NotFound(new ResponseDto<object>("Cita no encontrada", false, null));
        return Ok(new ResponseDto<object>("Cita eliminada", true, null));
    }
}
