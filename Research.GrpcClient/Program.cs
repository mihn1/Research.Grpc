// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Net.Client;
using Research.GrpcService;


const string addr = "https://localhost:7088";
var channel = GrpcChannel.ForAddress(
    addr
    , new GrpcChannelOptions
    {
        Credentials = Grpc.Core.ChannelCredentials.SecureSsl
        //, ServiceConfig = new Grpc.Net.Client.Configuration.ServiceConfig { LoadBalancingConfigs = new RoundRobinConfig() }
    });

int salesCount = 0;
try
{
    #region Greeter
    var greetClient = new Greeter.GreeterClient(channel);
    var metadata = new Metadata();
    metadata.Add("ClientName", "Research.GrpcClient");
    int i = 0;
    do
    {
        var res = await greetClient.SayHelloAsync(
            new HelloRequest { Name = "gRPC client", Message = "Testing" }
            , headers: metadata
        );

        Console.WriteLine("Request " + i + ": " + res.Message);
        Thread.Sleep(1000);
    }
    while (++i < 3);
    #endregion

    #region SalesData
    var saleClient = new SalesService.SalesServiceClient(channel);
    var serverStream = saleClient.RequestSalesData(new SalesRequest { Filters = "" }
        , deadline: DateTime.UtcNow.AddSeconds(60)
        );
    var resMeta = await serverStream.ResponseHeadersAsync;
    Console.WriteLine(resMeta);

    await foreach (var data in serverStream.ResponseStream.ReadAllAsync())
    {
        salesCount++;
        Console.WriteLine(string.Format("Order Receieved from {0}-{1},Order ID = {2}, Unit Price ={3}, Ship Date={4}", data.Country, data.Region, data.OrderID, data.UnitPrice, data.ShipDate));
    }

    //while (await serverStream.ResponseStream.MoveNext())
    //{
    //    salesCount++;
    //    var data = serverStream.ResponseStream.Current;
    //    Console.WriteLine(string.Format("Order Receieved from {0}-{1},Order ID = {2}, Unit Price ={3}, Ship Date={4}", data.Country, data.Region, data.OrderID, data.UnitPrice, data.ShipDate));
    //}

    #endregion
}
catch (RpcException ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    Console.WriteLine("Total: " + salesCount);
    await channel.ShutdownAsync();
}


