using Domain.Models;

namespace Application.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<Empleado?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    }
}
