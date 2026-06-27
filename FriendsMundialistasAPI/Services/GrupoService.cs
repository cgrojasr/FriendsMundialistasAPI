using FriendsMundialistasAPI.Contracts.Responses;
using FriendsMundialistasAPI.Data;
using FriendsMundialistasAPI.Repositories;

namespace FriendsMundialistasAPI.Services;

public sealed class GrupoService(IDbConnectionFactory connectionFactory, IGrupoRepository grupoRepository) : IGrupoService
{
    /// <summary>
    /// Recupera todos los grupos desde la base de datos para mostrarlos en pantalla.
    /// Su unico trabajo es orquestar conexion y repositorio.
    /// </summary>
    public async Task<IReadOnlyList<GrupoResponse>> ListarAsync(CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await grupoRepository.GetAllAsync(connection, cancellationToken);
    }
}
