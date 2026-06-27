using FriendsMundialistasAPI.Contracts.Requests;
using FriendsMundialistasAPI.Contracts.Responses;
using FriendsMundialistasAPI.Domain.Services;

namespace FriendsMundialistasAPI.Services;

public interface IEquipoService
{
    /// <summary>
    /// Devuelve el listado de equipos con filtro opcional por nombre.
    /// </summary>
    Task<IReadOnlyList<EquipoResponse>> ListarAsync(string? search, CancellationToken cancellationToken);
    /// <summary>
    /// Aplica las reglas de negocio y registra un nuevo equipo.
    /// </summary>
    Task<ServiceResult<EquipoResponse>> RegistrarAsync(RegistrarEquipoRequest request, CancellationToken cancellationToken);
}
