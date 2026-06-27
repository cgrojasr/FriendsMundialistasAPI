using System.Data;
using FriendsMundialistasAPI.Contracts.Responses;

namespace FriendsMundialistasAPI.Repositories;

public interface IEquipoRepository
{
    Task<int> CountAsync(IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(IDbConnection connection, IDbTransaction transaction, string nombre, CancellationToken cancellationToken);
    Task<int> InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        string nombre,
        int? grupoId,
        DateTime fechaCreacion,
        CancellationToken cancellationToken
    );
    Task<EquipoResponse?> GetByIdAsync(IDbConnection connection, IDbTransaction transaction, int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<EquipoResponse>> SearchAsync(IDbConnection connection, string? search, CancellationToken cancellationToken);
}
