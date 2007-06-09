using System;
using System.Diagnostics;
using System.IO;

namespace IrssUtils
{

  /// <summary>
  /// Log file recording class.
  /// </summary>
  public static class IrssLog
  {

    #region Enumerations

    /// <summary>
    /// Log detail levels.
    /// </summary>
    public enum Level
    {
      Off   = 0,
      Error = 1,
      Warn  = 2,
      Info  = 3,
      Debug = 4,
    }

    #endregion Enumerations

    #region Variables

    static Level _logLevel = Level.Debug;
    static StreamWriter _streamWriter;

    #endregion Variables

    #region Properties

    /// <summary>
    /// Level of detail to record in log file.
    /// </summary>
    public static Level LogLevel
    {
      get { return _logLevel; }
      set { _logLevel = value; }
    }

    #endregion Properties

    #region Implementation

    #region Log file opening and closing

    /// <summary>
    /// Open a log file to record to.
    /// </summary>
    /// <param name="fileName">File path, absolute.</param>
    public static void Open(string fileName)
    {
      if (_streamWriter == null && _logLevel > Level.Off)
      {
        if (File.Exists(fileName))
        {
          try
          {
            string backup = Path.ChangeExtension(fileName, ".bak");

            if (File.Exists(backup))
              File.Delete(backup);

            File.Move(fileName, backup);
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }
        }

        try
        {
          _streamWriter = new StreamWriter(fileName, false);
          _streamWriter.AutoFlush = true;

          string message = DateTime.Now.ToString() + ":\tLog Opened";
          _streamWriter.WriteLine(message);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
    }
    
    /// <summary>
    /// Close the currently open log file.
    /// </summary>
    public static void Close()
    {
      if (_streamWriter != null)
      {
        string message = DateTime.Now.ToString() + ":\tLog Closed";
        _streamWriter.WriteLine(message);

        _streamWriter.Close();
        _streamWriter = null;
      }
    }

    #endregion Log file opening and closing

    #region Log recording methods

    /// <summary>
    /// Log an Error.
    /// </summary>
    /// <param name="format">String format.</param>
    /// <param name="args">String format arguments.</param>
    public static void Error(string format, params object[] args)
    {
      if (_streamWriter != null && _logLevel >= Level.Error)
      {
        string message = DateTime.Now.ToString() + " - Error:\t" + String.Format(format, args);
        _streamWriter.WriteLine(message);
      }
    }
    
    /// <summary>
    /// Log a Warning.
    /// </summary>
    /// <param name="format">String format.</param>
    /// <param name="args">String format arguments.</param>
    public static void Warn(string format, params object[] args)
    {
      if (_streamWriter != null && _logLevel >= Level.Warn)
      {
        string message = DateTime.Now.ToString() + " - Warn: \t" + String.Format(format, args);
        _streamWriter.WriteLine(message);
      }
    }
    
    /// <summary>
    /// Log Information.
    /// </summary>
    /// <param name="format">String format.</param>
    /// <param name="args">String format arguments.</param>
    public static void Info(string format, params object[] args)
    {
      if (_streamWriter != null && _logLevel >= Level.Info)
      {
        string message = DateTime.Now.ToString() + " - Info: \t" + String.Format(format, args);
        _streamWriter.WriteLine(message);
      }
    }
    
    /// <summary>
    /// Log a Debug message.
    /// </summary>
    /// <param name="format">String format.</param>
    /// <param name="args">String format arguments.</param>
    public static void Debug(string format, params object[] args)
    {
      if (_streamWriter != null && _logLevel >= Level.Debug)
      {
        string message = DateTime.Now.ToString() + " - Debug:\t" + String.Format(format, args);
        _streamWriter.WriteLine(message);
      }
    }

    #endregion Log recording methods

    #endregion Implementation

  }

}