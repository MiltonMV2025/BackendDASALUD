using Application.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    public record LoginRequest([Required] string Username, [Required] string Password);

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResponseDto<AuthResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponseDto<AuthResultDto>>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Username, request.Password);
        if (result == null)
        {
            return Unauthorized(new ResponseDto<object>("Usuario o contraseña incorrectos", false, null));
        }

        var response = new ResponseDto<AuthResultDto>("Autenticado correctamente", true, result);
        return Ok(response);
    }
}
