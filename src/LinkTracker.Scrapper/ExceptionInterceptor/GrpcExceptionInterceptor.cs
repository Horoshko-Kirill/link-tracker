using Grpc.Core;
using Grpc.Core.Interceptors;
using LinkTracker.Scrapper.Application.Exceptions;
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

            Console.WriteLine(statusCode);

            var statusMessage = statusCode == 500 ? "Ошибка сервера" : ex.Message;

            throw new RpcException(new Status(StatusCode.Internal, statusMessage), metadata);
        }
    }
}
