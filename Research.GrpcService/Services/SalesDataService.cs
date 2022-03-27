using Grpc.Core;

namespace Research.GrpcService.Services
{
    public class SalesDataService : SalesService.SalesServiceBase
    {
        private readonly ILogger<SalesDataService> logger;

        public SalesDataService(ILogger<SalesDataService> logger)
        {
            this.logger = logger;
        }

        public override async Task RequestSalesData(SalesRequest request, IServerStreamWriter<SalesDataModel> responseStream, ServerCallContext context)
        {
            logger.LogInformation("{Time} Requested from: {Host}", DateTimeOffset.UtcNow, context.GetHttpContext().Connection.RemoteIpAddress);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "5m Sales Records.csv");
            using var reader = new StreamReader(filePath);

            string? line; bool isFirst = true;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }

                var cells = line.Split('\u002C'); // ,
                var model = new SalesDataModel();
                model.Region = cells[0];
                model.Country = cells[1];
                model.OrderID = int.TryParse(cells[6], out int _orderID) ? _orderID : 0;
                model.UnitPrice = float.TryParse(cells[9], out float _unitPrice) ? _unitPrice : 0;
                model.ShipDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime
                ((DateTime.TryParse(cells[7], out DateTime _dateShip) ? _dateShip : DateTime.MinValue).ToUniversalTime());
                model.UnitsSold = int.TryParse(cells[8], out int _unitsSold) ? _unitsSold : 0;
                model.UnitCost = float.TryParse(cells[10], out float _unitCost) ? _unitCost : 0;
                model.TotalRevenue = int.TryParse(cells[11], out int _totalRevenue) ? _totalRevenue : 0;
                model.TotalCost = int.TryParse(cells[13], out int _totalCost) ? _totalCost : 0;

                await responseStream.WriteAsync(model);
            }
        }
    }
}
