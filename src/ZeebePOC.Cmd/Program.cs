using System;
using System.IO;
using System.Threading.Tasks;
using Zeebe.Common;

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

        var pathOrderProcess = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workflows", "order-process.bpmn");

        var pathOrderPaymentLinkProcess = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workflows", "payment_link.bpmn");

        //var pathOrderProcessSalesForce = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workflows", "order-process-salesforce.bpmn");

        //var pathTimerEmail = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workflows", "timer-email.bpmn");

        await _zeebeContext.GetTopology();

        await _zeebeContext.DeployProcess(pathOrderProcess);
        await _zeebeContext.DeployProcess(pathOrderPaymentLinkProcess);
        //await _zeebeContext.DeployProcess(pathOrderProcessSalesForce);
        //await _zeebeContext.DeployProcess(pathTimerEmail);
      }
      catch (Exception ex)
      {
        Utils.WriteMessage(ex.Message, ConsoleColor.Red);
        Utils.WriteMessage(ex.StackTrace, ConsoleColor.Red);
      }
    }

    #endregion
  }
}
