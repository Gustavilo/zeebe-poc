using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zeebe.Common;
using ZeebePOC.Order.Service;

namespace ZeebePOC.Cmd
{
  /// <summary>
  /// 
  /// </summary>
  class Program
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

    #region :: Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static async Task Main(string[] args)
    {
      try
      {
        _zeebeContext = new ZeebeContext(_zeebeUrl);

        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workflows", "order-process.bpmn");

        await _zeebeContext.GetTopology();

        await _zeebeContext.DeployProcess(path);

        await CreateInstance("order-process", new OrderRequest
        {
          OrderId = Guid.NewGuid().ToString(),
          Amount = 23,
          TotalItems = 2
        });

      }
      catch (Exception ex)
      {
        Utils.WriteMessage(ex.Message, ConsoleColor.Red);
      }
    }

    #endregion

    #region :: Private Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="processId"></param>
    /// <param name="orderRequest"></param>
    /// <returns></returns>
    private static async Task CreateInstance(string processId, OrderRequest orderRequest)
    {
      var workflowInstance = await _zeebeContext.Client
        .NewCreateWorkflowInstanceCommand()
        .BpmnProcessId(processId)
        .LatestVersion()
        .Variables(JsonConvert.SerializeObject(orderRequest))
        .Send();


      Utils.WriteMessage(workflowInstance.WorkflowInstanceKey.ToString(), ConsoleColor.Green);
    }

    #endregion
  }
}
