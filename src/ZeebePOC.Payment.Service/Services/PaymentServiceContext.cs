using System;
using System.Threading.Tasks;
using Grpc.Core;
using Serilog;
using static ZeebePOC.Payment.Service.PaymentService;

namespace ZeebePOC.Payment.Service.Services
{
  /// <summary>
  /// 
  /// </summary>
  public class PaymentServiceContext : PaymentServiceBase
  {
    #region :: Private Fields ::

    /// <summary>
    /// 
    /// </summary>
    private readonly ILogger _logger = Log.Logger;

    #endregion

    #region :: Constructor ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public PaymentServiceContext() { }

    #endregion

    #region :: Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<PaymentResponse> ProcessPayment(PaymentRequest request, ServerCallContext context)
    {
      _logger.Information($"Payment with OrderId {request.OrderId} processed.");

      return Task.FromResult(new PaymentResponse
      {
        PaymentId = Guid.NewGuid().ToString()
      });
    }

    #endregion

  }
}
