using System;

namespace Zeebe.Common
{
  /// <summary>
  /// 
  /// </summary>
  public static class Utils
  {
    #region :: Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public static void WriteMessage(string message, ConsoleColor color = ConsoleColor.White)
    {
      Console.ForegroundColor = color;
      Console.WriteLine(message);
      Console.ResetColor();
    }

    #endregion
  }
}
