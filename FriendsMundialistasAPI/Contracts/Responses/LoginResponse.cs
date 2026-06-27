namespace FriendsMundialistasAPI.Contracts.Responses;

public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAtUtc, string Role);
