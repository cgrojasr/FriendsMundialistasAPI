using System.Data;
using Microsoft.Data.SqlClient;

namespace FriendsMundialistasAPI.Data;

public sealed class SqlConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    private readonly string _connectionString =
        !string.IsNullOrWhiteSpace(configuration.GetConnectionString("FriendsMundialistasDb"))
            ? configuration.GetConnectionString("FriendsMundialistasDb")!
            : throw new InvalidOperationException("La cadena de conexion FriendsMundialistasDb no esta configurada o esta vacia.");

    /// <summary>
    /// Crea y abre una conexion SQL Server usando la cadena de conexion configurada.
    /// </summary>
    public async Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
