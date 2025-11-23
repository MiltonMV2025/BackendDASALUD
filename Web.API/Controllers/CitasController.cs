using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [ProducesResponseType(typeof(ResponseDto<PagedResult<AppointmentRowDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseDto<PagedResult<AppointmentRowDto>>>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? q = null)
    {
        var result = await _citaService.GetPagedAsync(page, pageSize, q);
        var response = new ResponseDto<PagedResult<AppointmentRowDto>>("Citas obtenidas", true, result);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDto<AppointmentDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<AppointmentDetailsDto>>> GetById(int id)
    {
        var dto = await _citaService.GetByIdAsync(id);
        if (dto == null) return NotFound(new ResponseDto<object>("Cita no encontrada", false, null));
        return Ok(new ResponseDto<AppointmentDetailsDto>("Cita encontrada", true, dto));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseDto<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseDto<int>>> Create([FromBody] CreateCitaDto dto)
    {
        var id = await _citaService.CreateAsync(dto);
        return Ok(new ResponseDto<int>("Cita creada", true, id));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<object>>> Update(int id, [FromBody] EditCitaDto dto)
    {
        if (id != dto.IdCita) return BadRequest(new ResponseDto<object>("Id no coincide", false, null));
        var ok = await _citaService.UpdateAsync(dto);
        if (!ok) return NotFound(new ResponseDto<object>("Cita no encontrada", false, null));
        return Ok(new ResponseDto<object>("Cita actualizada", true, null));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<object>>> Delete(int id)
    {
        var ok = await _citaService.DeleteAsync(id);
        if (!ok) return NotFound(new ResponseDto<object>("Cita no encontrada", false, null));
        return Ok(new ResponseDto<object>("Cita eliminada", true, null));
    }

    [HttpGet("doctors")]
    [ProducesResponseType(typeof(ResponseDto<IEnumerable<DoctorCatalogDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseDto<IEnumerable<DoctorCatalogDto>>>> GetDoctorsCatalog()
    {
        var doctors = await _citaService.GetDoctorsCatalogAsync();
        var response = new ResponseDto<IEnumerable<DoctorCatalogDto>>("Catálogo de doctores", true, doctors);
        return Ok(response);
    }

    [HttpGet("patients")]
    [ProducesResponseType(typeof(ResponseDto<IEnumerable<PatientCatalogDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseDto<IEnumerable<PatientCatalogDto>>>> GetPatientsCatalog()
    {
        var patients = await _citaService.GetPatientsCatalogAsync();
        var response = new ResponseDto<IEnumerable<PatientCatalogDto>>("Catálogo de pacientes", true, patients);
        return Ok(response);
    }

    [HttpGet("status")]
    [ProducesResponseType(typeof(ResponseDto<IEnumerable<AppointmentStatusDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseDto<IEnumerable<AppointmentStatusDto>>>> GetStatusesCatalog()
    {
        var status = await _citaService.GetStatusesCatalogAsync();
        var response = new ResponseDto<IEnumerable<AppointmentStatusDto>>("Catálogo de estados de citas", true, status);
        return Ok(response);
    }
}
