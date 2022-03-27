// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Net.Client;
using Research.GrpcService;


const string addr = "https://localhost:7088";

var channel = GrpcChannel.ForAddress(
    addr
    , new GrpcChannelOptions { Credentials = Grpc.Core.ChannelCredentials.SecureSsl }
    );

var client = new Greeter.GreeterClient(channel);

var metadata = new Metadata();
metadata.Add("ClientName", "Research.GrpcClient");

int i = 0;
do
{
    var res = await client.SayHelloAsync(
  new HelloRequest { Name = "gRPC client", Message = "Testing" }
  , headers: metadata
  );

    Console.WriteLine(res.Message);
    Thread.Sleep(1000);
}
while (++i < 20);

channel.Dispose();
