﻿using System;
using System.Threading.Tasks;
using Zeebe.Common;
using ZeebePOC.JobManager.Jobs;

namespace ZeebePOC.JobManager
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
    static void Main(string[] args)
    {
      try
      {
        var zeebeContext = new ZeebeContext(_zeebeUrl);

        var paymentJob = new PaymentJob(zeebeContext.Client);
        var shipmentJob = new ShipmentJob(zeebeContext.Client);

        var emailJob = new EmailJob(zeebeContext.Client);
        var databaseCleanerJob = new CleanDataBaseJob(zeebeContext.Client);

        var createPaymentLinkJob = new CreatePaymentLinkJob(zeebeContext.Client);
        var paymentLinNotificationkJob = new PaymentLinkNotificationJob(zeebeContext.Client);

        Task.Factory.StartNew(() => paymentJob.StartWorker("payment-service-op", Environment.MachineName));
        Task.Factory.StartNew(() => shipmentJob.StartWorker("shipment-service-op", Environment.MachineName));

        Task.Factory.StartNew(() => emailJob.StartWorker("email-service", Environment.MachineName));
        Task.Factory.StartNew(() => databaseCleanerJob.StartWorker("cleaner-service", Environment.MachineName));

        Task.Factory.StartNew(() => createPaymentLinkJob.StartWorker("link-service", Environment.MachineName));
        Task.Factory.StartNew(() => paymentLinNotificationkJob.StartWorker("link-status-service", Environment.MachineName));

        Console.ReadLine();

        paymentJob.StopCurrentWorker();
        shipmentJob.StopCurrentWorker();

        emailJob.StopCurrentWorker();
        databaseCleanerJob.StopCurrentWorker();

        createPaymentLinkJob.StopCurrentWorker();
        paymentLinNotificationkJob.StopCurrentWorker();
      }
      catch (Exception ex)
      {
        Utils.WriteMessage(ex.Message, ConsoleColor.Red);
      }
    }

    #endregion
  }
}
