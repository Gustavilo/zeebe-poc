using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Newtonsoft.Json;
using Zeebe.Client;
using Zeebe.Client.Api.Worker;
using Zeebe.Common;
using ZeebePOC.Shipment.Service;
using static ZeebePOC.Shipment.Service.ShipmentService;

namespace ZeebePOC.JobManager.Jobs
{
  public class ShipmentJob
  {
    #region :: Private Fields ::

    /// <summary>
    /// 
    /// </summary>
    private readonly IZeebeClient _zeebeClient;

    /// <summary>
    /// 
    /// </summary>
    private IJobWorker _jobWorker;

    #endregion

    #region :: Constructor ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="zeebeContext"></param>
    public ShipmentJob(IZeebeClient zeebeClient) => _zeebeClient = zeebeClient;

    #endregion

    #region :: Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobType"></param>
    /// <param name="workerName"></param>
    public void StartWorker(string jobType, string workerName)
    {
      Utils.WriteMessage("Shipment Job starting...", ConsoleColor.Yellow);

      _jobWorker = _zeebeClient.NewWorker()
        .JobType(jobType)
        .Handler((jobClient, job) =>
        {
          var shipmentRequest = JsonConvert.DeserializeObject<ShipmentRequest>(job.Variables);

          var jobKey = job.Key;
          Utils.WriteMessage($"---> Shipping order!!! (JobKey {jobKey})", ConsoleColor.Cyan);

          Utils.WriteMessage($"***> Sendig PaymentId {shipmentRequest.PaymentId} to shipping service...", ConsoleColor.Cyan);

          var response = SendToProcess(shipmentRequest).Result;

          Utils.WriteMessage($":::> ShipmentId {response.ShipmentId} created.", ConsoleColor.Cyan);

          jobClient.NewCompleteJobCommand(jobKey)
            .Variables(JsonConvert.SerializeObject(response))
            .Send()
            .GetAwaiter()
            .GetResult();

        })
        .MaxJobsActive(5)
        .Name(workerName)
        .AutoCompletion()
        .PollInterval(TimeSpan.FromSeconds(1))
        .Timeout(TimeSpan.FromSeconds(10))
        .Open();

      Utils.WriteMessage("-> Shipment Job started.", ConsoleColor.Yellow);
    }

    /// <summary>
    /// 
    /// </summary>
    public void StopCurrentWorker()
    {
      if (_jobWorker != null)
      {
        _jobWorker.Dispose();

        Utils.WriteMessage("-> Shipment Job disposed.", ConsoleColor.Yellow);
      }
    }

    #endregion

    #region :: Private Fields ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paymentRequest"></param>
    /// <returns></returns>
    private async Task<ShipmentResponse> SendToProcess(ShipmentRequest shipmentRequest)
    {
      AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

      using var channel = GrpcChannel.ForAddress("http://localhost:7070");

      var client = new ShipmentServiceClient(channel);

      return await client.ProcessShipmentAsync(shipmentRequest);
    }

    #endregion
  }
}
