using FriendsMundialistasAPI.Contracts.Requests;
using FriendsMundialistasAPI.Contracts.Responses;

namespace FriendsMundialistasAPI.Services;

public interface IAuthService
{
    /// <summary>
    /// Valida las credenciales recibidas y genera un token JWT si son correctas.
    /// </summary>
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
