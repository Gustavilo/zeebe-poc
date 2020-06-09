using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Newtonsoft.Json;
using Zeebe.Client;
using Zeebe.Client.Api.Worker;
using Zeebe.Common;
using ZeebePOC.Order.Service;
using ZeebePOC.Payment.Service;
using static ZeebePOC.Payment.Service.PaymentService;

namespace ZeebePOC.JobManager.Jobs
{
  /// <summary>
  /// 
  /// </summary>
  public class PaymentJob
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
    public PaymentJob(IZeebeClient zeebeClient) => _zeebeClient = zeebeClient;

    #endregion

    #region :: Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobType"></param>
    /// <param name="workerName"></param>
    public void StartWorker(string jobType, string workerName)
    {
      Utils.WriteMessage("Payment Job starting...", ConsoleColor.Magenta);

      _jobWorker = _zeebeClient.NewWorker()
        .JobType(jobType)
        .Handler((jobClient, job) =>
        {
          var orderRequest = JsonConvert.DeserializeObject<OrderRequest>(job.Variables);

          var jobKey = job.Key;
          Utils.WriteMessage($"---> Collect the money!!! (JobKey {jobKey})", ConsoleColor.Green);

          Utils.WriteMessage($"***> Sendig OrderId {orderRequest.OrderId} to payment service...", ConsoleColor.Green);

          var response = SendToProcess(new PaymentRequest { OrderId = orderRequest.OrderId }).Result;

          Utils.WriteMessage($":::> PaymentId {response.PaymentId} created.", ConsoleColor.Green);

          jobClient.NewCompleteJobCommand(jobKey)
            .Variables(JsonConvert.SerializeObject(response))
            .Send()
            .GetAwaiter()
            .GetResult();

        })
        .MaxJobsActive(5)
        .Name(workerName)
        .AutoCompletion()
        .PollInterval(TimeSpan.FromMilliseconds(250))
        .Timeout(TimeSpan.FromSeconds(10))
        .Open();

      Utils.WriteMessage("-> Payment Job started.", ConsoleColor.Magenta);
    }

    /// <summary>
    /// 
    /// </summary>
    public void StopCurrentWorker()
    {
      if (_jobWorker != null)
      {
        _jobWorker.Dispose();

        Utils.WriteMessage("-> Payment Job disposed.", ConsoleColor.Magenta);
      }
    }

    #endregion

    #region :: Private Fields ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paymentRequest"></param>
    /// <returns></returns>
    private async Task<PaymentResponse> SendToProcess(PaymentRequest paymentRequest)
    {
      AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

      using var channel = GrpcChannel.ForAddress("http://localhost:6060");

      var client = new PaymentServiceClient(channel);

      return await client.ProcessPaymentAsync(paymentRequest);
    }

    #endregion
  }
}
