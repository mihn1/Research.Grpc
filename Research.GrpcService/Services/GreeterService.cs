using Grpc.Core;
using Newtonsoft.Json;
using Research.GrpcService;
using System;

namespace Research.GrpcService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            _logger.LogInformation("{Time} Requested from: {Host}", DateTimeOffset.UtcNow, context.Host);
            _logger.LogInformation("Headers: {Header}", JsonConvert.SerializeObject(context.RequestHeaders));
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name + ": " + request.Message
            });
        }
    }
}