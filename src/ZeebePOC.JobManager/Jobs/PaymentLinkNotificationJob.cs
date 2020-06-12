using System;
using Zeebe.Client;
using Zeebe.Client.Api.Worker;
using Zeebe.Common;

namespace ZeebePOC.JobManager.Jobs
{
  /// <summary>
  /// 
  /// </summary>
  public class PaymentLinkNotificationJob
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
    public PaymentLinkNotificationJob(IZeebeClient zeebeClient) => _zeebeClient = zeebeClient;

    #endregion

    #region :: Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobType"></param>
    /// <param name="workerName"></param>
    public void StartWorker(string jobType, string workerName)
    {
      Utils.WriteMessage("Send PaymnetLink Notification Job starting...", ConsoleColor.DarkGreen);

      _jobWorker = _zeebeClient.NewWorker()
        .JobType(jobType)
        .Handler((jobClient, job) =>
        {
          var jobKey = job.Key;
          Utils.WriteMessage($"---> PaymnetLink Notification Sent!!! (JobKey {jobKey})", ConsoleColor.DarkGreen);

          jobClient.NewCompleteJobCommand(jobKey)
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

      Utils.WriteMessage("-> Send PaymnetLink Notification started.", ConsoleColor.DarkGreen);
    }

    /// <summary>
    /// 
    /// </summary>
    public void StopCurrentWorker()
    {
      if (_jobWorker != null)
      {
        _jobWorker.Dispose();

        Utils.WriteMessage("-> Send PaymnetLink Notification Job disposed.", ConsoleColor.DarkGreen);
      }
    }

    #endregion
  }
}
