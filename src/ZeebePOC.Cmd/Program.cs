using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zeebe.Client;

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

        var client = ZeebeClient.Builder()
          .UseGatewayAddress(_zeebeUrl)
          .UsePlainText()
          .Build();

        await GetTopology(client);

        await DeployOrderProcess(client);

      }
      catch (Exception ex)
      {
        WriteMessage(ex.Message, ConsoleColor.Red);
      }
    }

    #endregion

    #region :: Private Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    private static async Task GetTopology(IZeebeClient client)
    {
      var topology = await client.TopologyRequest()
        .Send();

      WriteMessage(topology.ToString(), ConsoleColor.Green);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    private static async Task DeployOrderProcess(IZeebeClient client)
    {
      var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workflows", "order-process.bpmn");

      var response = await client.NewDeployCommand()
        .AddResourceFile(path)
        .Send();

      WriteMessage($"Workflow added version-{response.Workflows.FirstOrDefault().Version}", ConsoleColor.Yellow);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    private static void WriteMessage(string message, ConsoleColor color = ConsoleColor.White)
    {
      Console.ForegroundColor = color;
      Console.WriteLine(message);
      Console.ResetColor();
    }

    #endregion
  }
}
