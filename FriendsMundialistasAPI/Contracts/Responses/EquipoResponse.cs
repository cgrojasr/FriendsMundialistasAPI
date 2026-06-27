namespace FriendsMundialistasAPI.Contracts.Responses;

public sealed record EquipoResponse(
    int IdEquipo,
    string Nombre,
    int? IdGrupo,
    string? NombreGrupo,
    DateTime FechaCreacion
);
