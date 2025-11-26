namespace Application.DTOs
{
    public class PacienteDto
    {
        public int IdPaciente { get; set; }

        public string Nombres { get; set; } = null!;

        public string Apellidos { get; set; } = null!;

        public string NombreCompleto => $"{Nombres} {Apellidos}";

        public string? Correo { get; set; }

        public string? Telefono { get; set; }

        public string? TipoSangre { get; set; }

        public decimal? Peso { get; set; }

        public decimal? Altura { get; set; }
    }
}

public record CreatePacienteDto(
    string Nombres,
    string Apellidos,
    string? Correo,
    string? Telefono,
    string? TipoSangre,
    decimal? Peso,
    decimal? Altura
);

public record EditPacienteDto(
    int IdPaciente,
    string Nombres,
    string Apellidos,
    string? Correo,
    string? Telefono,
    string? TipoSangre,
    decimal? Peso,
    decimal? Altura
);
