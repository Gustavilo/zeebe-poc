using System;
using System.Threading.Tasks;
using Grpc.Core;
using Serilog;
using static ZeebePOC.Shipment.Service.ShipmentService;

namespace ZeebePOC.Shipment.Service.Services
{
  /// <summary>
  /// 
  /// </summary>
  public class ShipmentServiceContext : ShipmentServiceBase
  {
    #region :: Private Fields ::

    /// <summary>
    /// 
    /// </summary>
    private readonly ILogger _logger = Log.Logger;

    #endregion

    #region :: Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<ShipmentResponse> ProcessShipment(ShipmentRequest request, ServerCallContext context)
    {
      _logger.Information($"Shipment with PaymentId {request.PaymentId} processed.");

      return Task.FromResult(new ShipmentResponse
      {
        ShipmentId = Guid.NewGuid().ToString()
      });
    }

    #endregion
  }
}
