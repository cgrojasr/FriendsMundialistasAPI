using FriendsMundialistasAPI.Contracts.Responses;

namespace FriendsMundialistasAPI.Domain.Services;

public sealed class ServiceResult<T>
{
    /// <summary>
    /// Construye un resultado de servicio con el estado, datos y error que correspondan.
    /// Se mantiene privado para obligar a usar los metodos de fabrica.
    /// </summary>
    private ServiceResult(bool isSuccess, int statusCode, T? data, ApiError? error, string? message)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Data = data;
        Error = error;
        Message = message;
    }

    public bool IsSuccess { get; }
    public int StatusCode { get; }
    public T? Data { get; }
    public ApiError? Error { get; }
    public string? Message { get; }

    /// <summary>
    /// Crea una respuesta exitosa para cuando la operacion termina correctamente.
    /// </summary>
    public static ServiceResult<T> Success(T data, int statusCode, string? message = null)
        => new(true, statusCode, data, null, message);

    /// <summary>
    /// Crea una respuesta fallida con codigo, mensaje y estado HTTP asociados.
    /// </summary>
    public static ServiceResult<T> Failure(int statusCode, string code, string message)
        => new(false, statusCode, default, new ApiError(code, message), null);
}
