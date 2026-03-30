using Grpc.Core;
using Grpc.Core.Interceptors;
using LinkTracker.Bot.Contracts.Dto;
using System.Text.Json;

namespace LinkTracker.Bot.ExceptionInterceptor;

public class GrpcExceptionInterceptor : Interceptor
{
    private readonly ILogger<GrpcExceptionInterceptor> _logger;

    public GrpcExceptionInterceptor(ILogger<GrpcExceptionInterceptor> logger)
    {
        _logger = logger;
    }
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {

            var error = new ApiErrorResponse
            {
                Code = ex.GetType().Name,
                ExceptionName = ex.GetType().Name,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace?.Split('\n').ToList()
            };

            var metadata = new Metadata
            {
                { "error", JsonSerializer.Serialize(error) }
            };

            _logger.LogError("Grpc interceptor bot error {message}", ex.Message);

            throw new RpcException(new Status(StatusCode.Internal, ex.Message), metadata);
        }
    }
}
