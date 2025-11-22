namespace Application.DTOs;

public record ResponseDto<T>(string Message, bool Result, T? Data);

public record ResponseDto(string Message, bool Result, object? Data = null);
