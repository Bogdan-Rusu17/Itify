using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Extensions.Logging;
using MobyLabWebProgramming.Infrastructure.Errors;

namespace MobyLabWebProgramming.Infrastructure.Responses;

/// <summary>
/// These classes are used as responses from service methods as either a success responses or as error responses.
/// </summary>
public class ServiceResponse
{
    public ErrorMessage? Error { get; private init; }
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsOk => Error == null;

    public static ServiceResponse FromError(ErrorMessage? error) => new() { Error = error };
    public static ServiceResponse ErrorIf(bool condition, Func<ErrorMessage> getError) => new() { Error = condition ? getError() : null };
    public static ServiceResponse ForSuccess() => new();
    public static ServiceResponse<T> ForSuccess<T>(T data) => new() { Result = data };
    public static ServiceResponse<T> FromError<T>(ErrorMessage? error) => new() { Error = error };
    public ServiceResponse<T> ToResponse<T>(T result) => Error == null ? ForSuccess(result) : FromError<T>(Error);
    
    public void LogError(ILogger logger) => Error?.LogError(logger); 

    internal ServiceResponse() { }
}

public sealed class ServiceResponse<T>
{
    public ErrorMessage? Error { get; internal init; }
    
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Result))]
    public bool IsOk => Error == null;
    
    public T? Result { get; init; }
    public ServiceResponse ToResponse() => Result != null && IsOk ? ServiceResponse.ForSuccess() : ServiceResponse.FromError(Error);

    internal ServiceResponse() { }
    public void LogError(ILogger logger) => Error?.LogError(logger);
}

/// <summary>
/// These are extension methods for the ServiceResponse classes.
/// They can be used to functionally process data within the ServiceResponse object.
/// </summary>
public static class ServiceResponseExtension
{
    public static ServiceResponse<TOut> Map<TIn, TOut>(this ServiceResponse<TIn> response, Func<TIn, TOut> selector) where TIn : class where TOut : class =>
        response.IsOk ? ServiceResponse.ForSuccess(selector(response.Result)) : ServiceResponse.FromError<TOut>(response.Error);

    public static async Task<ServiceResponse<TOut>> MapAsync<TIn, TOut>(this ServiceResponse<TIn> response, Func<TIn, Task<TOut>> selector) where TIn : class where TOut : class =>
        response.IsOk ? ServiceResponse.ForSuccess(await selector(response.Result)) : ServiceResponse.FromError<TOut>(response.Error);

    public static ServiceResponse<PagedResponse<TOut>> Map<TIn, TOut>(this ServiceResponse<PagedResponse<TIn>> response, Func<TIn, TOut> selector) =>
        response.IsOk ? ServiceResponse.ForSuccess(response.Result.Map(selector)) : ServiceResponse.FromError<PagedResponse<TOut>>(response.Error);

    public static ServiceResponse<TOut> FlatMap<TIn, TOut>(this ServiceResponse<TIn> response, Func<TIn, ServiceResponse<TOut>> selector) where TIn : class where TOut : class =>
        response.IsOk ? selector(response.Result) : ServiceResponse.FromError<TOut>(response.Error);

    public static ServiceResponse<TOut> FlatMap<TOut>(this ServiceResponse response, Func<ServiceResponse<TOut>> selector) where TOut : class =>
        response.IsOk ? selector() : ServiceResponse.FromError<TOut>(response.Error);

    public static ServiceResponse<TIn> Flatten<TIn>(this ServiceResponse<ServiceResponse<TIn>> response) where TIn : class =>
        response.IsOk ? response.Result : ServiceResponse.FromError<TIn>(response.Error);

    public static ServiceResponse Flatten(this ServiceResponse<ServiceResponse> response) =>
        response.IsOk ? response.Result : ServiceResponse.FromError(response.Error);

    public static ServiceResponse<T> ToServiceResponse<T>(this T data) => ServiceResponse.ForSuccess(data);
    public static ServiceResponse<T> ToServiceError<T>(this ErrorMessage error) => ServiceResponse.FromError<T>(error);

    public static ServiceResponse ToServiceError(this ErrorMessage error) => ServiceResponse.FromError(error);

    public static ServiceResponse ToServiceResponseFromException(this Exception ex) =>
        ServiceResponse.FromError(ex is ServerException serverException
            ? ErrorMessage.FromException(serverException)
            : new ErrorMessage(HttpStatusCode.InternalServerError, "A unexpected error occurred!"));

    public static ServiceResponse<T> ToServiceResponseFromException<T>(this Exception ex) =>
        ServiceResponse.FromError<T>(ex is ServerException serverException
            ? ErrorMessage.FromException(serverException)
            : new ErrorMessage(HttpStatusCode.InternalServerError, "A unexpected error occurred!"));
}
