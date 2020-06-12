using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zeebe.Common;
using ZeebePOC.Order.Service;
using ZeebePOC.Service.Models;
using ZeebePOC.ShipmentLink.Service;

namespace ZeebePOC.Service.Controllers
{
  /// <summary>
  /// 
  /// </summary>
  [Produces("application/json")]
  [Route("v1/order")]
  [ApiController]
  public class OrderController : ControllerBase
  {
    #region :: Private Methods ::

    /// <summary>
    /// 
    /// </summary>
    private static readonly string _zeebeUrl = "127.0.0.1:26500";

    /// <summary>
    /// 
    /// </summary>
    private static ZeebeContext _zeebeContext;

    #endregion

    #region :: Constructor ::

    /// <summary>
    /// 
    /// </summary>
    public OrderController() => _zeebeContext = new ZeebeContext(_zeebeUrl);

    #endregion

    #region :: Methods ::

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="body">Order data.</param>
    /// <returns>Order response</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] OrderRequest body)
    {
      var response = await CreateOrder(body);

      return StatusCode((int)HttpStatusCode.OK, response);
    }

    /// <summary>
    /// Creates a payment link.
    /// </summary>
    /// <param name="body">PaymentLink data.</param>
    /// <returns>PaymentLink response</returns>
    [HttpPost("create-paymentlink")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreatePaymentLink([FromBody] PaymentLinkRequest body)
    {
      await CreatePaymentLinkInternal(body);

      return StatusCode((int)HttpStatusCode.OK);
    }

    /// <summary>
    /// Creates a payment link.
    /// </summary>
    /// <param name="body">PaymentLink data.</param>
    /// <returns>PaymentLink response</returns>
    [HttpPost("send-paymentlink-message")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SendPaymentLinkMessage([FromBody] PaymentLinkMessage body)
    {
      await _zeebeContext.Client.NewPublishMessageCommand()
        .MessageName("payment-link-status-changed")
        .CorrelationKey(body.PaymentLinkId.ToString())
        .Variables(JObject.FromObject(new { body.Status }).ToString())
        .Send();

      return StatusCode((int)HttpStatusCode.OK);
    }

    #endregion

    #region :: Private Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="orderRequest"></param>
    /// <returns></returns>
    private async Task<OrderResponse> CreateOrder(OrderRequest orderRequest)
    {
      var workflowInstanceResult = await _zeebeContext.Client
        .NewCreateWorkflowInstanceCommand()
        .BpmnProcessId("order-process")
        .LatestVersion()
        .Variables(JsonConvert.SerializeObject(orderRequest))
        .WithResult()
        .Send();

      dynamic response = JObject.Parse(workflowInstanceResult.Variables);

      return new OrderResponse
      {
        StatusId = 1,
        ShipmentId = response.ShipmentId
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paymentLinkRequest"></param>
    /// <returns></returns>
    private async Task CreatePaymentLinkInternal(PaymentLinkRequest paymentLinkRequest)
    {
      await _zeebeContext.Client
        .NewCreateWorkflowInstanceCommand()
        .BpmnProcessId("payment-link-process")
        .LatestVersion()
        .Variables(JsonConvert.SerializeObject(paymentLinkRequest))
        .Send();
    }

    #endregion
  }
}