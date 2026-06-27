using FriendsMundialistasAPI.Contracts.Requests;
using FriendsMundialistasAPI.Domain.Services;
using FriendsMundialistasAPI.Services;

namespace FriendsMundialistasAPI.Endpoints;

public static class AdminEndpoints
{
    /// <summary>
    /// Registra en la aplicacion las rutas administrativas de grupos y equipos.
    /// Su responsabilidad es solo conectar HTTP con los servicios de negocio.
    /// </summary>
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var admin = app.MapGroup("/api/admin")
            .RequireAuthorization("AdminOnly")
            .WithTags("Administracion");

        admin.MapGet("/grupos", async (IGrupoService grupoService, CancellationToken cancellationToken) =>
        {
            var items = await grupoService.ListarAsync(cancellationToken);
            return Results.Ok(new { items });
        });

        admin.MapGet("/equipos", async (string? search, IEquipoService equipoService, CancellationToken cancellationToken) =>
        {
            var items = await equipoService.ListarAsync(search, cancellationToken);
            return Results.Ok(new { items, total = items.Count });
        });

        admin.MapPost("/equipos", async (
            RegistrarEquipoRequest request,
            IEquipoService equipoService,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await equipoService.RegistrarAsync(request, cancellationToken);
            return ToHttpResult(result);
        });

        return app;
    }

    /// <summary>
    /// Traduce el resultado del servicio a una respuesta HTTP concreta.
    /// Centraliza el manejo de codigo de estado y mensajes de error.
    /// </summary>
    private static IResult ToHttpResult(ServiceResult<Contracts.Responses.EquipoResponse> result)
    {
        if (result.IsSuccess && result.Data is not null)
        {
            if (result.StatusCode == 201)
            {
                return Results.Created(
                    $"/api/admin/equipos/{result.Data.IdEquipo}",
                    new { message = result.Message, data = result.Data }
                );
            }

            return Results.Ok(result.Data);
        }

        return result.StatusCode switch
        {
            400 => Results.BadRequest(result.Error),
            404 => Results.NotFound(result.Error),
            409 => Results.Conflict(result.Error),
            _ => Results.Problem(result.Error?.Message ?? "Error interno")
        };
    }
}
