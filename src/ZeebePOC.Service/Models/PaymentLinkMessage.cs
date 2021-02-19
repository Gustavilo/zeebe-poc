using System;

namespace ZeebePOC.Service.Models
{
  /// <summary>
  /// 
  /// </summary>
  public class PaymentLinkMessage
  {
    #region :: Properties ::

    /// <summary>
    /// 
    /// </summary>
    public Guid PaymentLinkId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Status { get; set; }

    #endregion
  }
}
