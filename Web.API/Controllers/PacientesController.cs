using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _pacienteService;

    public PacientesController(IPacienteService pacienteService)
    {
        _pacienteService = pacienteService;
    }

    // GET api/pacientes?page=1&pageSize=10&q=juan
    [HttpGet]
    [ProducesResponseType(typeof(ResponseDto<PagedResult<PacienteDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseDto<PagedResult<PacienteDto>>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? q = null
    )
    {
        var result = await _pacienteService.GetPagedAsync(page, pageSize, q);
        var response = new ResponseDto<PagedResult<PacienteDto>>("Pacientes obtenidos", true, result);
        return Ok(response);
    }

    // GET api/pacientes/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDto<PacienteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<PacienteDto>>> GetById(int id)
    {
        var dto = await _pacienteService.GetByIdAsync(id);
        if (dto == null)
            return NotFound(new ResponseDto<object>("Paciente no encontrado", false, null));

        return Ok(new ResponseDto<PacienteDto>("Paciente encontrado", true, dto));
    }

    // POST api/pacientes
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDto<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseDto<int>>> Create([FromBody] CreatePacienteDto dto)
    {
        var id = await _pacienteService.CreateAsync(dto);
        var response = new ResponseDto<int>("Paciente creado correctamente", true, id);
        return Ok(response);
    }

    // PUT api/pacientes/5
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<object>>> Update(int id, [FromBody] EditPacienteDto dto)
    {
        if (id != dto.IdPaciente)
            return BadRequest(new ResponseDto<object>("Id no coincide", false, null));

        var ok = await _pacienteService.UpdateAsync(dto);
        if (!ok)
            return NotFound(new ResponseDto<object>("Paciente no encontrado", false, null));

        return Ok(new ResponseDto<object>("Paciente actualizado", true, null));
    }

    // DELETE api/pacientes/5
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<object>>> Delete(int id)
    {
        var ok = await _pacienteService.DeleteAsync(id);
        if (!ok)
            return NotFound(new ResponseDto<object>("Paciente no encontrado", false, null));

        return Ok(new ResponseDto<object>("Paciente eliminado", true, null));
    }
}
