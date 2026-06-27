using System.Data;
using Dapper;
using FriendsMundialistasAPI.Contracts.Responses;

namespace FriendsMundialistasAPI.Repositories;

public sealed class EquipoRepository : IEquipoRepository
{
    /// <summary>
    /// Cuenta los equipos existentes para validar el limite permitido.
    /// </summary>
    public Task<int> CountAsync(IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        const string sql = "SELECT COUNT(1) FROM Equipos;";
        var command = new CommandDefinition(sql, transaction: transaction, cancellationToken: cancellationToken);
        return connection.ExecuteScalarAsync<int>(command);
    }

    /// <summary>
    /// Verifica si ya existe un equipo con el nombre recibido.
    /// </summary>
    public Task<bool> ExistsByNameAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        string nombre,
        CancellationToken cancellationToken
    )
    {
        const string sql = "SELECT COUNT(1) FROM Equipos WHERE UPPER(Nombre) = UPPER(@Nombre);";
        var command = new CommandDefinition(
            sql,
            new { Nombre = nombre },
            transaction,
            cancellationToken: cancellationToken
        );

        return ExecuteExistsAsync(connection, command);
    }

    /// <summary>
    /// Inserta un equipo nuevo en la base de datos.
    /// </summary>
    public Task<int> InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        string nombre,
        int? grupoId,
        DateTime fechaCreacion,
        CancellationToken cancellationToken
    )
    {
        const string sql = """
            INSERT INTO Equipos (Nombre, IdGrupo, FechaCreacion)
            OUTPUT INSERTED.IdEquipo
            VALUES (@Nombre, @GrupoId, @FechaCreacion);
            """;

        var command = new CommandDefinition(
            sql,
            new { Nombre = nombre, GrupoId = grupoId, FechaCreacion = fechaCreacion },
            transaction,
            cancellationToken: cancellationToken
        );

        return connection.ExecuteScalarAsync<int>(command);
    }

    /// <summary>
    /// Recupera el equipo recien creado con su nombre de grupo para construir la respuesta.
    /// </summary>
    public Task<EquipoResponse?> GetByIdAsync(IDbConnection connection, IDbTransaction transaction, int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                e.IdEquipo AS IdEquipo,
                e.Nombre,
                e.IdGrupo AS IdGrupo,
                g.CodigoGrupo AS NombreGrupo,
                e.FechaCreacion AS FechaCreacion
            FROM Equipos e
            LEFT JOIN Grupos g ON g.IdGrupo = e.IdGrupo
            WHERE e.IdEquipo = @Id;
            """;

        var command = new CommandDefinition(sql, new { Id = id }, transaction, cancellationToken: cancellationToken);
        return connection.QueryFirstOrDefaultAsync<EquipoResponse>(command);
    }

    /// <summary>
    /// Busca equipos por texto opcional para alimentar listados y filtros.
    /// </summary>
    public async Task<IReadOnlyList<EquipoResponse>> SearchAsync(IDbConnection connection, string? search, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                e.IdEquipo AS IdEquipo,
                e.Nombre,
                e.IdGrupo AS IdGrupo,
                g.CodigoGrupo AS NombreGrupo,
                e.FechaCreacion AS FechaCreacion
            FROM Equipos e
            LEFT JOIN Grupos g ON g.IdGrupo = e.IdGrupo
            WHERE (@Search IS NULL OR @Search = '' OR e.Nombre LIKE '%' + @Search + '%')
            ORDER BY e.Nombre;
            """;

        var command = new CommandDefinition(sql, new { Search = search?.Trim() }, cancellationToken: cancellationToken);
        var rows = await connection.QueryAsync<EquipoResponse>(command);
        return rows.ToList();
    }

    /// <summary>
    /// Reutiliza la lectura de conteo para convertirla en una respuesta booleana.
    /// </summary>
    private static async Task<bool> ExecuteExistsAsync(IDbConnection connection, CommandDefinition command)
    {
        var count = await connection.ExecuteScalarAsync<int>(command);
        return count > 0;
    }
}
