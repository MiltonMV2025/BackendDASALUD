using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
    }
}
