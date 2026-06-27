using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FriendsMundialistasAPI.Auth;
using FriendsMundialistasAPI.Contracts.Requests;
using FriendsMundialistasAPI.Contracts.Responses;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FriendsMundialistasAPI.Services;

public sealed class AuthService(IOptions<AuthSettings> options) : IAuthService
{
    private const string AdminRole = "Administrador";
    private readonly AuthSettings _settings = options.Value;

    /// <summary>
    /// Compara usuario y contrasena contra la configuracion y, si coinciden,
    /// genera un JWT firmado para acceder a los endpoints protegidos.
    /// </summary>
    public Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var username = request.Username?.Trim();
        var password = request.Password;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult<LoginResponse?>(null);
        }

        var isValid = string.Equals(username, _settings.AdminUsername, StringComparison.Ordinal) &&
                      string.Equals(password, _settings.AdminPassword, StringComparison.Ordinal);

        if (!isValid)
        {
            return Task.FromResult<LoginResponse?>(null);
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_settings.TokenMinutes);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, AdminRole)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt.UtcDateTime,
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = credentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        return Task.FromResult<LoginResponse?>(new LoginResponse(jwt, expiresAt, AdminRole));
    }
}
