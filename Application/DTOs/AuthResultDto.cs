namespace Application.DTOs;

public record AuthResultDto(
    string Token,
    int UserId,
    string Username,
    string Nombres,
    string Apellidos,
    string FullName,
    string Especialidad,
    int IdRol,
    int IdEspecialidad,
    string RoleName
);
