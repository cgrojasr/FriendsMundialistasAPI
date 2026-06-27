using System.Data;
using FriendsMundialistasAPI.Contracts.Responses;

namespace FriendsMundialistasAPI.Repositories;

public interface IGrupoRepository
{
    Task<IReadOnlyList<GrupoResponse>> GetAllAsync(IDbConnection connection, CancellationToken cancellationToken);
    Task<bool> ExistsByIdAsync(IDbConnection connection, IDbTransaction transaction, int id, CancellationToken cancellationToken);
}
