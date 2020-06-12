using System;
using Zeebe.Client;
using Zeebe.Client.Api.Worker;
using Zeebe.Common;

namespace ZeebePOC.JobManager.Jobs
{
  /// <summary>
  /// 
  /// </summary>
  public class EmailJob
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
    public EmailJob(IZeebeClient zeebeClient) => _zeebeClient = zeebeClient;

    #endregion

    #region :: Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobType"></param>
    /// <param name="workerName"></param>
    public void StartWorker(string jobType, string workerName)
    {
      Utils.WriteMessage("Email Job starting...", ConsoleColor.Gray);

      _jobWorker = _zeebeClient.NewWorker()
        .JobType(jobType)
        .Handler((jobClient, job) =>
        {
          var jobKey = job.Key;
          Utils.WriteMessage($"---> Processing emails...!!! (JobKey {jobKey})", ConsoleColor.Gray);

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

      Utils.WriteMessage("-> Email Job started.", ConsoleColor.Gray);
    }

    /// <summary>
    /// 
    /// </summary>
    public void StopCurrentWorker()
    {
      if (_jobWorker != null)
      {
        _jobWorker.Dispose();

        Utils.WriteMessage("-> Email Job disposed.", ConsoleColor.Gray);
      }
    }

    #endregion
  }
}
