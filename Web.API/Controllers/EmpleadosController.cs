using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoService _empleadoService;

    public EmpleadosController(IEmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseDto<PagedResult<EmpleadoRowDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseDto<PagedResult<EmpleadoRowDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? q = null)
    {
        var result = await _empleadoService.GetPagedAsync(page, pageSize, q);
        return Ok(new ResponseDto<PagedResult<EmpleadoRowDto>>("Empleados obtenidos", true, result));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDto<EmpleadoDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<EmpleadoDetailsDto>>> GetById(int id)
    {
        var dto = await _empleadoService.GetByIdAsync(id);
        if (dto == null)
            return NotFound(new ResponseDto<object>("Empleado no encontrado", false, null));

        return Ok(new ResponseDto<EmpleadoDetailsDto>("Empleado encontrado", true, dto));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseDto<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseDto<int>>> Create([FromBody] CreateEmpleadoDto dto)
    {
        var id = await _empleadoService.CreateAsync(dto);
        return Ok(new ResponseDto<int>("Empleado creado", true, id));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<object>>> Update(int id, [FromBody] EditEmpleadoDto dto)
    {
        if (id != dto.IdEmpleado)
            return BadRequest(new ResponseDto<object>("Id no coincide", false, null));

        var ok = await _empleadoService.UpdateAsync(dto);

        if (!ok)
            return NotFound(new ResponseDto<object>("Empleado no encontrado", false, null));

        return Ok(new ResponseDto<object>("Empleado actualizado", true, null));
    }

    // Cambiar activo/inactivo SOLO cambia la propiedad Activo
    // Ej: PUT api/empleados/5/activo?value=false
    [HttpPut("{id}/activo")]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<object>>> SetActivo(int id, [FromQuery] bool value)
    {
        var ok = await _empleadoService.SetActivoAsync(id, value);

        if (!ok)
            return NotFound(new ResponseDto<object>("Empleado no encontrado", false, null));

        return Ok(new ResponseDto<object>(
            value ? "Empleado activado" : "Empleado desactivado",
            true,
            null
        ));
    }

    [HttpGet("by-username/{username}")]
    [ProducesResponseType(typeof(ResponseDto<EmpleadoDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<EmpleadoDetailsDto>>> GetByUsername(string username)
    {
        var dto = await _empleadoService.GetByUsernameAsync(username);

        if (dto == null)
            return NotFound(new ResponseDto<object>("Empleado no encontrado", false, null));

        return Ok(new ResponseDto<EmpleadoDetailsDto>("Empleado encontrado", true, dto));
    }
}
