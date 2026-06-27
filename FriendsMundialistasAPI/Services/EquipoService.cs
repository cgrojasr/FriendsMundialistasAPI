using System.Data;
using FriendsMundialistasAPI.Contracts.Requests;
using FriendsMundialistasAPI.Contracts.Responses;
using FriendsMundialistasAPI.Data;
using FriendsMundialistasAPI.Domain.Services;
using FriendsMundialistasAPI.Repositories;
using Microsoft.Data.SqlClient;

namespace FriendsMundialistasAPI.Services;

public sealed class EquipoService(
    IDbConnectionFactory connectionFactory,
    IEquipoRepository equipoRepository,
    IGrupoRepository grupoRepository
) : IEquipoService
{
    /// <summary>
    /// Obtiene el listado de equipos usando el repositorio y la conexion a base de datos.
    /// Esta funcion solo coordina la consulta, sin mezclar reglas HTTP.
    /// </summary>
    public async Task<IReadOnlyList<EquipoResponse>> ListarAsync(string? search, CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await equipoRepository.SearchAsync(connection, search, cancellationToken);
    }

    /// <summary>
    /// Valida y registra un equipo nuevo aplicando las reglas de negocio.
    /// Decide si el alta es valida, unica y dentro del limite permitido.
    /// </summary>
    public async Task<ServiceResult<EquipoResponse>> RegistrarAsync(RegistrarEquipoRequest request, CancellationToken cancellationToken)
    {
        var nombre = request.Nombre?.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return ServiceResult<EquipoResponse>.Failure(400, "TEAM_NAME_REQUIRED", "El nombre es obligatorio");
        }

        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction(IsolationLevel.Serializable);

        if (request.IdGrupo.HasValue)
        {
            var groupExists = await grupoRepository.ExistsByIdAsync(
                connection,
                transaction,
                request.IdGrupo.Value,
                cancellationToken
            );

            if (!groupExists)
            {
                transaction.Rollback();
                return ServiceResult<EquipoResponse>.Failure(404, "GROUP_NOT_FOUND", "El grupo seleccionado no existe");
            }
        }

        var totalEquipos = await equipoRepository.CountAsync(connection, transaction, cancellationToken);
        if (totalEquipos >= 48)
        {
            transaction.Rollback();
            return ServiceResult<EquipoResponse>.Failure(
                409,
                "TEAM_LIMIT_REACHED",
                "Se alcanzó el máximo de 48 equipos permitidos"
            );
        }

        var exists = await equipoRepository.ExistsByNameAsync(connection, transaction, nombre, cancellationToken);
        if (exists)
        {
            transaction.Rollback();
            return ServiceResult<EquipoResponse>.Failure(409, "TEAM_ALREADY_EXISTS", "El equipo ya existe");
        }

        try
        {
            var createdAt = DateTime.UtcNow;
            var equipoId = await equipoRepository.InsertAsync(
                connection,
                transaction,
                nombre,
                request.IdGrupo,
                createdAt,
                cancellationToken
            );

            var savedTeam = await equipoRepository.GetByIdAsync(connection, transaction, equipoId, cancellationToken);
            if (savedTeam is null)
            {
                transaction.Rollback();
                return ServiceResult<EquipoResponse>.Failure(500, "TEAM_NOT_PERSISTED", "No se pudo recuperar el equipo creado");
            }

            transaction.Commit();
            return ServiceResult<EquipoResponse>.Success(savedTeam, 201, "Equipo registrado correctamente");
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            transaction.Rollback();
            return ServiceResult<EquipoResponse>.Failure(409, "TEAM_ALREADY_EXISTS", "El equipo ya existe");
        }
    }
}
