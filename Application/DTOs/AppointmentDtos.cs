namespace Application.DTOs;

public record AppointmentRowDto(
    int Id,
    string IdConsulta,
    string AtendidoPor,
    string Paciente,
    DateTime Fecha,
    decimal Costo,
    string Estado
);

public record CreateCitaDto(
    int IdPaciente,
    int IdEmpleado,
    DateTime FechaCita,
    decimal Costo
);

public record EditCitaDto(
    int IdCita,
    int IdPaciente,
    int IdEmpleado,
    int IdEstado,
    DateTime FechaCita,
    decimal Costo
);

public record AppointmentDetailsDto(
    int IdCita,
    int IdPaciente,
    string PacienteNombre,
    int IdEmpleado,
    string EmpleadoNombre,
    DateTime FechaCita,
    decimal Costo,
    int IdEstado,
    string Estado,
    bool Activo
);

public record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, int TotalItems, int TotalPages);
