using Grpc.Core;
using Grpc.Core.Interceptors;
using LinkTracker.Scrapper.Contracts.Dto;
using System.Text.Json;

namespace LinkTracker.Scrapper.ExceptionInterceptor;

public class GrpcExceptionInterceptor : Interceptor
{
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
                ExceptionMessage = ex.Message
            };

            var metadata = new Metadata
            {
                { "error", JsonSerializer.Serialize(error) }
            };

            throw new RpcException(new Status(StatusCode.Internal, ex.Message), metadata);
        }
    }
}
