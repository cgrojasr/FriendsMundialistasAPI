using FriendsMundialistasAPI.Contracts.Responses;

namespace FriendsMundialistasAPI.Services;

public interface IGrupoService
{
    /// <summary>
    /// Devuelve la lista de grupos para su consumo por la interfaz.
    /// </summary>
    Task<IReadOnlyList<GrupoResponse>> ListarAsync(CancellationToken cancellationToken);
}
