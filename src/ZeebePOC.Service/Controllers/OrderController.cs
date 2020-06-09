using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zeebe.Common;
using ZeebePOC.Order.Service;

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

    #endregion
  }
}