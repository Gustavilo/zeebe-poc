using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Zeebe.Client;

namespace Zeebe.Common
{
  /// <summary>
  /// 
  /// </summary>
  public class ZeebeContext
  {
    #region :: Properties ::

    /// <summary>
    /// 
    /// </summary>
    public IZeebeClient Client { get; }

    #endregion

    #region :: Constructor ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="zeebeUrl"></param>
    public ZeebeContext(string zeebeUrl)
    {
      //var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

      //Console.WriteLine(path);

      Client = ZeebeClient.Builder()
      .UseGatewayAddress(zeebeUrl)
      .UsePlainText()
      //.UseTransportEncryption($"{path}/admin.pem")
      .Build();
    }

    #endregion

    #region :: Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task GetTopology()
    {
      var topology = await Client.TopologyRequest()
        .Send();

      Utils.WriteMessage(topology.ToString(), ConsoleColor.Green);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public async Task DeployProcess(string path)
    {
      var response = await Client.NewDeployCommand()
        .AddResourceFile(path)
        .Send();

      Utils.WriteMessage($"Workflow {path} added version - {response.Workflows.FirstOrDefault().Version}",
        ConsoleColor.Yellow);
    }

    #endregion
  }
}
