namespace Application.DTOs
{
    public record ConsultasPorMesDto(string Mes, int Cantidad);
    public record ConsultasPorEspecialidadDto(string Especialidad, string Codigo, int Cantidad);
}
