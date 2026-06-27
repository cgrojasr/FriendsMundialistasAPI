using System.Data;
using Dapper;
using FriendsMundialistasAPI.Contracts.Responses;

namespace FriendsMundialistasAPI.Repositories;

public sealed class GrupoRepository : IGrupoRepository
{
    /// <summary>
    /// Obtiene todos los grupos ordenados alfabeticamente para poblar combos o listas.
    /// </summary>
    public async Task<IReadOnlyList<GrupoResponse>> GetAllAsync(IDbConnection connection, CancellationToken cancellationToken)
    {
        const string sql = "SELECT IdGrupo AS Id, CodigoGrupo AS Nombre FROM Grupos ORDER BY CodigoGrupo;";
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var rows = await connection.QueryAsync<GrupoResponse>(command);
        return rows.ToList();
    }

    /// <summary>
    /// Valida si el grupo solicitado existe antes de asociarlo a un equipo.
    /// </summary>
    public Task<bool> ExistsByIdAsync(IDbConnection connection, IDbTransaction transaction, int id, CancellationToken cancellationToken)
    {
        const string sql = "SELECT COUNT(1) FROM Grupos WHERE IdGrupo = @Id;";
        var command = new CommandDefinition(sql, new { Id = id }, transaction, cancellationToken: cancellationToken);
        return ExecuteExistsAsync(connection, command);
    }

    /// <summary>
    /// Convierte un conteo de filas en una respuesta booleana simple.
    /// </summary>
    private static async Task<bool> ExecuteExistsAsync(IDbConnection connection, CommandDefinition command)
    {
        var count = await connection.ExecuteScalarAsync<int>(command);
        return count > 0;
    }
}
