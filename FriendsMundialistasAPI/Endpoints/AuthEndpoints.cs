using FriendsMundialistasAPI.Contracts.Requests;
using FriendsMundialistasAPI.Services;

namespace FriendsMundialistasAPI.Endpoints;

public static class AuthEndpoints
{
    /// <summary>
    /// Registra la ruta publica de autenticacion para obtener el JWT.
    /// Su unica responsabilidad es recibir la peticion y devolver la respuesta HTTP.
    /// </summary>
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth")
            .WithTags("Autenticacion");

        auth.MapPost("/login", async (
            LoginRequest request,
            IAuthService authService,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await authService.LoginAsync(request, cancellationToken);
            return result is null
                ? Results.Unauthorized()
                : Results.Ok(result);
        })
        .AllowAnonymous();

        return app;
    }
}
