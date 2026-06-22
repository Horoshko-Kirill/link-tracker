using System.Text.Json;
using Grpc.Core;
using Grpc.Core.Interceptors;
using LinkTracker.Scrapper.Application.Exceptions;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.GrpcInterceptors;

public class GrpcExceptionInterceptor : Interceptor
{
    public readonly ILogger<GrpcExceptionInterceptor> _logger;

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

            var statusCode = ex switch
            {
                BadRequestException => 400,
                ConflictException => 409,
                NotFoundException => 404,
                _ => 500
            };

            var error = new ApiErrorResponse
            {
                Code = ex.GetType().Name,
                ExceptionName = ex.GetType().Name,
                ExceptionMessage = statusCode == 500 ? "Ошибка сервера" : ex.Message,
            };

            var metadata = new Metadata
            {
                { "error", JsonSerializer.Serialize(error) }
            };

            var statusMessage = statusCode == 500 ? "Ошибка сервера" : ex.Message;

            _logger.LogError("Grpc interceptor scrapper error {message}", ex.Message);

            throw new RpcException(new Status(StatusCode.Internal, statusMessage), metadata);
        }
    }
}
