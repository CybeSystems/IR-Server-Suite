using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Xml;

using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

using IRServerPluginInterface;
using IrssComms;
using IrssUtils;

namespace InputService
{

  #region Enumerations

  /// <summary>
  /// Describes the operation mode of the IR Server.
  /// </summary>
  public enum IRServerMode
  {
    /// <summary>
    /// Acts as a standard IR Server (Default).
    /// </summary>
    ServerMode = 0,
    /// <summary>
    /// Relays button presses to another IR Server.
    /// </summary>
    RelayMode = 1,
    /// <summary>
    /// Acts as a repeater for another IR Server's IR blasting.
    /// </summary>
    RepeaterMode = 2,
  }

  #endregion Enumerations

  public class InputService : ServiceBase
  {

    #region Constants

    static readonly string ConfigurationFile = Common.FolderAppData + "Input Service\\Input Service.xml";

    #endregion Constants

    #region Variables

    List<ClientManager> _registeredClients;
    List<ClientManager> _registeredRepeaters;

    Server _server = null;
    Client _client = null;

    int _serverPort = 24000;

    IRServerMode _mode;
    string _hostComputer;

    bool _registered = false; // Used for relay and repeater modes.

    string _pluginNameReceive = String.Empty;
    IRServerPlugin _pluginReceive = null;

    string _pluginNameTransmit = String.Empty;
    IRServerPlugin _pluginTransmit = null;
    
    #endregion Variables

    #region Constructor

    public InputService()
    {
      this.ServiceName = Program.ServiceName;
      
      //this.EventLog.Log = "Application";
      //this.AutoLog = true;

      this.CanHandlePowerEvent          = true;
      this.CanHandleSessionChangeEvent  = false;
      this.CanPauseAndContinue          = false;
      this.CanShutdown                  = true;
      this.CanStop                      = true;
    }

    #endregion Constructor

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (disposing)
        {
          if (_server != null)
            _server.Dispose();

          if (_client != null)
            _client.Dispose();
        }
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    #endregion IDisposable

    #region Service Methods

    protected override void OnStart(string[] args)
    {
      // TODO: Change log level to info for release.
      IrssLog.LogLevel = IrssLog.Level.Debug;
      IrssLog.Open(Common.FolderIrssLogs + "Input Service.log");

      try
      {
        IrssLog.Info("Starting IR Server ...");

        LoadSettings();

        // Load IR Plugins ...
        _pluginReceive  = null;
        _pluginTransmit = null;

        if (String.IsNullOrEmpty(_pluginNameReceive) && String.IsNullOrEmpty(_pluginNameTransmit))
        {
          IrssLog.Warn("No transmit or receive plugin loaded");
        }
        else
        {
          if (String.IsNullOrEmpty(_pluginNameReceive))
          {
            IrssLog.Warn("No receiver plugin loaded");
          }
          else
          {
            _pluginReceive = Program.GetPlugin(_pluginNameReceive);
          }

          if (_pluginNameTransmit.Equals(_pluginNameReceive, StringComparison.InvariantCultureIgnoreCase))
          {
            _pluginTransmit = _pluginReceive;
            IrssLog.Info("Using the same plugin for transmit and receive");
          }
          else if (String.IsNullOrEmpty(_pluginNameTransmit))
          {
            IrssLog.Warn("No transmit plugin loaded");
          }
          else
          {
            _pluginTransmit = Program.GetPlugin(_pluginNameTransmit);
          }            
        }

        switch (_mode)
        {
          case IRServerMode.ServerMode:
            {
              StartServer();

              IrssLog.Info("Started in Server Mode");
              break;
            }

          case IRServerMode.RelayMode:
            {
              if (StartRelay())
                IrssLog.Info("Started in Relay Mode");
              else
                IrssLog.Error("Failed to start in Relay Mode");
              break;
            }

          case IRServerMode.RepeaterMode:
            {
              if (StartRepeater())
                IrssLog.Info("Started in Repeater Mode");
              else
                IrssLog.Error("Failed to start in Repeater Mode");
              break;
            }
        }

        // Start plugin(s) ...
        if (_pluginReceive != null)
        {
          try
          {
            if (_pluginReceive.Start())
              IrssLog.Info("Receiver plugin started: \"{0}\"", _pluginNameReceive);
            else
              IrssLog.Error("Failed to start receive plugin: \"{0}\"", _pluginNameReceive);
          }
          catch (Exception ex)
          {
            IrssLog.Error("Failed to start receive plugin: \"{0}\"", _pluginNameReceive);
            IrssLog.Error(ex.ToString());
          }
        }

        if (!_pluginNameTransmit.Equals(_pluginNameReceive, StringComparison.InvariantCultureIgnoreCase))
        {
          if (_pluginTransmit != null)
          {
            try
            {
              if (_pluginTransmit.Start())
                IrssLog.Info("Transmit plugin started: \"{0}\"", _pluginNameTransmit);
              else
                IrssLog.Error("Failed to start transmit plugin: \"{0}\"", _pluginNameTransmit);
            }
            catch (Exception ex)
            {
              IrssLog.Error("Failed to start transmit plugin: \"{0}\"", _pluginNameTransmit);
              IrssLog.Error(ex.ToString());
            }
          }
        }

        if (_pluginReceive != null)
        {
          if (_pluginReceive is IRemoteReceiver)
            (_pluginReceive as IRemoteReceiver).RemoteCallback += new RemoteHandler(RemoteHandlerCallback);

          if (_pluginReceive is IKeyboardReceiver)
            (_pluginReceive as IKeyboardReceiver).KeyboardCallback += new KeyboardHandler(KeyboardHandlerCallback);
          
          if (_pluginReceive is IMouseReceiver)
            (_pluginReceive as IMouseReceiver).MouseCallback += new MouseHandler(MouseHandlerCallback);
        }

        IrssLog.Info("IR Server started");
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex.ToString());
      }
    }

    protected override void OnStop()
    {
      IrssLog.Info("Stopping IR Server ...");

      if (_mode == IRServerMode.ServerMode)
      {
        IrssMessage message = new IrssMessage(MessageType.ServerShutdown, MessageFlags.Notify);
        SendToAll(message);
      }

      if (_pluginReceive != null)
      {
        if (_pluginReceive is IRemoteReceiver)
          (_pluginReceive as IRemoteReceiver).RemoteCallback -= new RemoteHandler(RemoteHandlerCallback);
        
        if (_pluginReceive is IKeyboardReceiver)
          (_pluginReceive as IKeyboardReceiver).KeyboardCallback -= new KeyboardHandler(KeyboardHandlerCallback);
        
        if (_pluginReceive is IMouseReceiver)
          (_pluginReceive as IMouseReceiver).MouseCallback -= new MouseHandler(MouseHandlerCallback);
      }
      
      // Stop Plugin(s)
      try
      {
        if (_pluginReceive != null)
          _pluginReceive.Stop();
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex.ToString());
      }
      try
      {
        if (_pluginTransmit != null && _pluginTransmit != _pluginReceive)
          _pluginTransmit.Stop();
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex.ToString());
      }

      // Stop Server
      try
      {
        switch (_mode)
        {
          case IRServerMode.ServerMode:
            StopServer();
            break;

          case IRServerMode.RelayMode:
            StopRelay();
            break;

          case IRServerMode.RepeaterMode:
            StopRepeater();
            break;
        }
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex.ToString());
      }
      
      IrssLog.Close();
    }

    protected override void OnShutdown()
    {
      OnStop();
    }

    protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
    {
      switch (powerStatus)
      {
        case PowerBroadcastStatus.Suspend:
          IrssLog.Info("Entering standby ...");

          if (_pluginReceive != null)
            _pluginReceive.Suspend();

          if (_pluginTransmit != null && _pluginTransmit != _pluginReceive)
            _pluginTransmit.Suspend();

          // Inform clients ...
          if (_mode == IRServerMode.ServerMode)
          {
            IrssMessage message = new IrssMessage(MessageType.ServerShutdown, MessageFlags.Notify);
            SendToAll(message);
          }
          break;

        case PowerBroadcastStatus.ResumeAutomatic:
        case PowerBroadcastStatus.ResumeCritical:
        case PowerBroadcastStatus.ResumeSuspend:
          IrssLog.Info("Resume from standby ...");

          if (_pluginReceive != null)
            _pluginReceive.Resume();

          if (_pluginTransmit != null && _pluginTransmit != _pluginReceive)
            _pluginTransmit.Resume();

          // TODO: Inform clients ?
          break;
      }

      return true;
    }

    #region Unused

    protected override void OnPause()
    {
    }

    protected override void OnContinue()
    {
    }

    protected override void OnSessionChange(SessionChangeDescription changeDescription)
    {
      switch (changeDescription.Reason)
      {
        case SessionChangeReason.SessionLogon:
          break;

        case SessionChangeReason.SessionLogoff:
          break;
      }
    }

    protected override void OnCustomCommand(int command)
    {
      switch (command)
      {
        case 128:
          break;
      }
    }

    #endregion Unused

    #endregion Service Methods

    void StartServer()
    {
      if (_server != null)
        return;

      // Initialize registered client lists ...
      _registeredClients = new List<ClientManager>();
      _registeredRepeaters = new List<ClientManager>();

      ServerMessageSink sink = new ServerMessageSink(ServerReceivedMessage);
      _server = new Server(_serverPort, sink);

      _server.Start();
    }
    void StopServer()
    {
      if (_server == null)
        return;

      _server.Stop();
      _server = null;

      _registeredClients.Clear();
      _registeredClients = null;

      _registeredRepeaters.Clear();
      _registeredRepeaters = null;
    }

    void CommsFailure(object obj)
    {
      Exception ex = obj as Exception;

      if (ex != null)
        IrssLog.Error("Communications failure: {0}", ex.Message);
      else
        IrssLog.Error("Communications failure");

      StopClient();
    }
    void Connected(object obj)
    {
      IrssLog.Info("Connected to another server");

      if (_mode == IRServerMode.RepeaterMode)
      {
        IrssMessage message = new IrssMessage(MessageType.RegisterRepeater, MessageFlags.Request);
        _client.Send(message);
      }
    }
    void Disconnected(object obj)
    {
      IrssLog.Warn("Communications with other server has been lost");

      Thread.Sleep(1000);
    }

    bool StartClient()
    {
      if (_client != null)
        return false;

      ClientMessageSink sink = new ClientMessageSink(ClientReceivedMessage);

      IPAddress serverAddress = Client.GetIPFromName(_hostComputer);

      _client = new Client(serverAddress, 24000, sink);
      _client.CommsFailureCallback  = new WaitCallback(CommsFailure);
      _client.ConnectCallback       = new WaitCallback(Connected);
      _client.DisconnectCallback    = new WaitCallback(Disconnected);
      
      if (_client.Start())
      {
        return true;
      }
      else
      {
        _client = null;
        return false;
      }
    }
    void StopClient()
    {
      if (_client == null)
        return;

      _client.Stop();
      _client = null;
    }
    
    bool StartRelay()
    {
      try
      {
        StartServer();
        StartClient();

        return true;
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex.ToString());
        return false;
      }
    }
    void StopRelay()
    {
      try
      {
        StopServer();
        StopClient();
      }
      catch { }
    }

    bool StartRepeater()
    {
      try
      {
        StartServer();
        StartClient();

        return true;
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex.ToString());
        return false;
      }
    }
    void StopRepeater()
    {
      try
      {
        if (_registered)
        {
          _registered = false;

          IrssMessage message = new IrssMessage(MessageType.UnregisterRepeater, MessageFlags.Request);
          _client.Send(message);
        }

        StopServer();
        StopClient();
      }
      catch { }
    }

    void RemoteHandlerCallback(string keyCode)
    {
      IrssLog.Debug("Remote Event: {0}", keyCode);

      byte[] bytes = Encoding.ASCII.GetBytes(keyCode);

      switch (_mode)
      {
        case IRServerMode.ServerMode:
          {
            IrssMessage message = new IrssMessage(MessageType.RemoteEvent, MessageFlags.Notify, bytes);
            SendToAll(message);
            break;
          }

        case IRServerMode.RelayMode:
          {
            IrssMessage message = new IrssMessage(MessageType.ForwardRemoteEvent, MessageFlags.Request, bytes);
            _client.Send(message);
            break;
          }

        case IRServerMode.RepeaterMode:
          {
            IrssLog.Debug("Remote event ignored, IR Server is in Repeater Mode.");
            break;
          }
      }
    }
    void KeyboardHandlerCallback(int vKey, bool keyUp)
    {
      IrssLog.Debug("Keyboard Event: {0}, keyUp: {1}", vKey, keyUp);

      byte[] bytes = new byte[8];
      BitConverter.GetBytes(vKey).CopyTo(bytes, 0);
      BitConverter.GetBytes(keyUp).CopyTo(bytes, 4);
      
      switch (_mode)
      {
        case IRServerMode.ServerMode:
          {
            IrssMessage message = new IrssMessage(MessageType.KeyboardEvent, MessageFlags.Notify, bytes);
            SendToAll(message);
            break;
          }

        case IRServerMode.RelayMode:
          {
            IrssMessage message = new IrssMessage(MessageType.ForwardKeyboardEvent, MessageFlags.Request, bytes);
            _client.Send(message);
            break;
          }

        case IRServerMode.RepeaterMode:
          {
            IrssLog.Debug("Keyboard event ignored, IR Server is in Repeater Mode.");
            break;
          }
      }
    }
    void MouseHandlerCallback(int deltaX, int deltaY, int buttons)
    {
      IrssLog.Debug("Mouse Event - deltaX: {0}, deltaY: {1}, buttons: {2}", deltaX, deltaY, buttons);

      byte[] bytes = new byte[12];
      BitConverter.GetBytes(deltaX).CopyTo(bytes, 0);
      BitConverter.GetBytes(deltaY).CopyTo(bytes, 4);
      BitConverter.GetBytes(buttons).CopyTo(bytes, 8);

      switch (_mode)
      {
        case IRServerMode.ServerMode:
          {
            IrssMessage message = new IrssMessage(MessageType.MouseEvent, MessageFlags.Notify, bytes);
            SendToAll(message);
            break;
          }

        case IRServerMode.RelayMode:
          {
            IrssMessage message = new IrssMessage(MessageType.ForwardMouseEvent, MessageFlags.Request, bytes);
            _client.Send(message);
            break;
          }

        case IRServerMode.RepeaterMode:
          {
            IrssLog.Debug("Mouse event ignored, IR Server is in Repeater Mode.");
            break;
          }
      }
    }

    void SendToAll(IrssMessage message)
    {
      IrssLog.Debug("SendToAll({0}, {1})", message.Type, message.Flags);

      List<ClientManager> unregister = new List<ClientManager>();

      lock (_registeredClients)
      {
        foreach (ClientManager client in _registeredClients)
        {
          if (!_server.Send(client, message))
          {
            IrssLog.Warn("Failed to send message to a client, unregistering client");

            // If a message doesn't get through then unregister that client
            unregister.Add(client);
          }
        }

        // Unregistering clients must be done as a two part process because otherwise the
        // foreach statement above would fail if you modified the _registeredClients list
        // while enumerating it.
        foreach (ClientManager client in unregister)
        {
          UnregisterClient(client);
        }
      }
    }
    void SendToAllExcept(ClientManager exceptClient, IrssMessage message)
    {
      IrssLog.Debug("SendToAllExcept({0}, {1})", message.Type, message.Flags);

      List<ClientManager> unregister = new List<ClientManager>();

      lock (_registeredClients)
      {
        foreach (ClientManager client in _registeredClients)
        {
          if (client == exceptClient)
            continue;
          
          if (!_server.Send(client, message))
          {
            IrssLog.Warn("Failed to send message to a client, unregistering client");

            // If a message doesn't get through then unregister that client
            unregister.Add(client);
          }
        }

        // Unregistering clients must be done as a two part process because otherwise the
        // foreach statement above would fail if you modified the _registeredClients list
        // while enumerating it.
        foreach (ClientManager client in unregister)
        {
          UnregisterClient(client);
        }
      }
    }
    void SendTo(ClientManager receiver, IrssMessage message)
    {
      IrssLog.Debug("SendTo({0}, {1})", message.Type, message.Flags);
      
      if (!_server.Send(receiver, message))
      {
        IrssLog.Warn("Failed to send message to a client, unregistering client");

        // If a message doesn't get through then unregister that client
        UnregisterClient(receiver);
      }
    }
    void SendToRepeaters(IrssMessage message)
    {
      IrssLog.Debug("SendToRepeaters({0}, {1})", message.Type, message.Flags);

      List<ClientManager> unregister = new List<ClientManager>();

      lock (_registeredRepeaters)
      {
        foreach (ClientManager client in _registeredRepeaters)
        {
          if (!_server.Send(client, message))
          {
            IrssLog.Warn("Failed to send message to a repeater, unregistering repeater");

            // If a message doesn't get through then unregister that repeater
            unregister.Add(client);
          }
        }

        // Unregistering repeaters must be done as a two part process because otherwise the
        // foreach statement above would fail if you modified the _registeredRepeaters list
        // while enumerating it.
        foreach (ClientManager repeater in unregister)
        {
          UnregisterRepeater(repeater);
        }
      }
    }

    bool RegisterClient(ClientManager addClient)
    {
      lock (_registeredClients)
      {
        if (!_registeredClients.Contains(addClient))
          _registeredClients.Add(addClient);
      }

      IrssLog.Info("Registered a client");
      return true;
    }
    bool UnregisterClient(ClientManager removeClient)
    {
      lock (_registeredClients)
      {
        if (!_registeredClients.Contains(removeClient))
          return false;

        _registeredClients.Remove(removeClient);
      }

      IrssLog.Info("Unregistered a client");
      return true;
    }

    bool RegisterRepeater(ClientManager addRepeater)
    {
      lock (_registeredRepeaters)
      {
        if (!_registeredRepeaters.Contains(addRepeater))
          _registeredRepeaters.Add(addRepeater);
      }

      IrssLog.Info("Registered a repeater");
      return true;
    }
    bool UnregisterRepeater(ClientManager removeRepeater)
    {
      lock (_registeredRepeaters)
      {
        if (!_registeredRepeaters.Contains(removeRepeater))
          return false;

        _registeredRepeaters.Remove(removeRepeater);
      }

      IrssLog.Info("Unregistered a repeater");
      return true;
    }

    bool BlastIR(byte[] data)
    {
      try
      {
        IrssLog.Debug("Blast IR");

        if (_pluginTransmit == null || !(_pluginTransmit is ITransmitIR))
          return false;

        string port = "Default";

        int portLen = BitConverter.ToInt32(data, 0);
        if (portLen > 0)
          port = Encoding.ASCII.GetString(data, 4, portLen);

        byte[] codeData = new byte[data.Length - (4 + portLen)];
        Array.Copy(data, 4 + portLen, codeData, 0, codeData.Length);

        return (_pluginTransmit as ITransmitIR).Transmit(port, codeData);
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex.ToString());
        return false;
      }
    }
    LearnStatus LearnIR(out byte[] data)
    {
      IrssLog.Debug("Learn IR");

      data = null;

      if (_pluginTransmit == null)
      {
        IrssLog.Debug("No transmit plugin loaded, can't learn");
        return LearnStatus.Failure;
      }
      else if (!(_pluginTransmit is ILearnIR))
      {
        IrssLog.Debug("Active transmit plugin doesn't support learn");
        return LearnStatus.Failure;
      }

      Thread.Sleep(250);

      LearnStatus status = LearnStatus.Failure;

      try
      {
        status = (_pluginTransmit as ILearnIR).Learn(out data);
        switch (status)
        {
          case LearnStatus.Success:
            IrssLog.Info("Learn IR success");
            break;

          case LearnStatus.Failure:
            IrssLog.Error("Failed to learn IR Code");
            break;

          case LearnStatus.Timeout:
            IrssLog.Warn("IR Code learn timed out");
            break;
        }
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex.ToString());
      }

      return status;
    }

    void ServerReceivedMessage(MessageManagerCombo combo)
    {
      IrssLog.Debug("Server message received: {0}, {1}", combo.Message.Type, combo.Message.Flags);

      try
      {
        switch (combo.Message.Type)
        {
          case MessageType.ForwardRemoteEvent:
            if (_mode == IRServerMode.RelayMode)
            {
              IrssMessage forward = new IrssMessage(MessageType.ForwardRemoteEvent, MessageFlags.Request, combo.Message.DataAsBytes);
              _client.Send(forward);
            }
            else
            {
              IrssMessage forward = new IrssMessage(MessageType.RemoteEvent, MessageFlags.Notify, combo.Message.DataAsBytes);
              SendToAllExcept(combo.Manager, forward);
            }
            break;

          case MessageType.ForwardKeyboardEvent:
            if (_mode == IRServerMode.RelayMode)
            {
              IrssMessage forward = new IrssMessage(MessageType.ForwardKeyboardEvent, MessageFlags.Request, combo.Message.DataAsBytes);
              _client.Send(forward);
            }
            else
            {
              IrssMessage forward = new IrssMessage(MessageType.KeyboardEvent, MessageFlags.Notify, combo.Message.DataAsBytes);
              SendToAllExcept(combo.Manager, forward);
            }
            break;

          case MessageType.ForwardMouseEvent:
            if (_mode == IRServerMode.RelayMode)
            {
              IrssMessage forward = new IrssMessage(MessageType.ForwardMouseEvent, MessageFlags.Request, combo.Message.DataAsBytes);
              _client.Send(forward);
            }
            else
            {
              IrssMessage forward = new IrssMessage(MessageType.MouseEvent, MessageFlags.Notify, combo.Message.DataAsBytes);
              SendToAllExcept(combo.Manager, forward);
            }
            break;

          case MessageType.BlastIR:
          {
            IrssMessage response = new IrssMessage(MessageType.BlastIR, MessageFlags.Response);

            if (_mode == IRServerMode.RelayMode)
            {
              response.Flags |= MessageFlags.Failure;
            }
            else
            {
              if (_registeredRepeaters.Count > 0)
                SendToRepeaters(combo.Message);

              if (BlastIR(combo.Message.DataAsBytes))
                response.Flags |= MessageFlags.Success;
              else
                response.Flags |= MessageFlags.Failure;
            }

            if ((combo.Message.Flags & MessageFlags.ForceNotRespond) != MessageFlags.ForceNotRespond)
              SendTo(combo.Manager, response);
            
            break;
          }

          case MessageType.LearnIR:
          {
            IrssMessage response = new IrssMessage(MessageType.LearnIR, MessageFlags.Response);

            if (_mode == IRServerMode.RelayMode)
            {
              response.Flags |= MessageFlags.Failure;
            }
            else
            {
              byte[] bytes = null;

              LearnStatus status = LearnIR(out bytes);

              switch (status)
              {
                case LearnStatus.Success:
                  response.Flags |= MessageFlags.Success;
                  response.DataAsBytes = bytes;
                  break;

                case LearnStatus.Failure:
                  response.Flags |= MessageFlags.Failure;
                  break;

                case LearnStatus.Timeout:
                  response.Flags |= MessageFlags.Timeout;
                  break;
              }
            }

            SendTo(combo.Manager, response);
            break;
          }

          case MessageType.ServerShutdown:
            if ((combo.Message.Flags & MessageFlags.Request) == MessageFlags.Request)
            {
              IrssLog.Info("Shutdown command received");
              Stop();
            }

            break;

          case MessageType.RegisterClient:
          {
            IrssMessage response = new IrssMessage(MessageType.RegisterClient, MessageFlags.Response);

            if (RegisterClient(combo.Manager))
            {
              IRServerInfo irServerInfo = new IRServerInfo();

              if (_pluginReceive != null)
                irServerInfo.CanReceive = true;

              if (_pluginTransmit != null)
              {
                irServerInfo.CanLearn = (_pluginTransmit is ILearnIR);
                irServerInfo.CanTransmit = true;
                irServerInfo.Ports = (_pluginTransmit as ITransmitIR).AvailablePorts;
              }

              response.DataAsBytes = irServerInfo.ToBytes();
              response.Flags |= MessageFlags.Success;
            }
            else
            {
              response.Flags |= MessageFlags.Failure;
            }

            SendTo(combo.Manager, response);
            break;
          }

          case MessageType.UnregisterClient:
            UnregisterClient(combo.Manager);
            break;

          case MessageType.RegisterRepeater:
            {
              IrssMessage response = new IrssMessage(MessageType.RegisterRepeater, MessageFlags.Response);

              if (RegisterRepeater(combo.Manager))
                response.Flags |= MessageFlags.Success;
              else
                response.Flags |= MessageFlags.Failure;

              SendTo(combo.Manager, response);
              break;
            }

          case MessageType.UnregisterRepeater:
            UnregisterRepeater(combo.Manager);
            break;
        }
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex.ToString());
        IrssMessage response = new IrssMessage(MessageType.Error, MessageFlags.Notify, ex.Message);
        SendTo(combo.Manager, response);
      }
    }
    void ClientReceivedMessage(IrssMessage received)
    {
      IrssLog.Debug("Client message received: {0}, {1}", received.Type, received.Flags);

      try
      {
        switch (received.Type)
        {
          case MessageType.RegisterClient:
            if ((received.Flags & MessageFlags.Response) == MessageFlags.Response)
            {
              if ((received.Flags & MessageFlags.Success) == MessageFlags.Success)
              {
                IrssLog.Info("Registered with host server");
                _registered = true;
              }
              else
              {
                IrssLog.Warn("Host server refused registration");
                _registered = false;
              }
            }
            break;

          case MessageType.ServerShutdown:
            if ((received.Flags & MessageFlags.Notify) == MessageFlags.Notify)
            {
              IrssLog.Warn("Host server has shut down");
              _registered = false;
            }
            break;
        }
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex.ToString());
        IrssMessage response = new IrssMessage(MessageType.Error, MessageFlags.Notify, ex.Message);
        _client.Send(response);
      }
    }

    void LoadSettings()
    {
      try
      {
        XmlDocument doc = new XmlDocument();
        doc.Load(ConfigurationFile);

        _mode               = (IRServerMode)Enum.Parse(typeof(IRServerMode), doc.DocumentElement.Attributes["Mode"].Value, true);
        _hostComputer       = doc.DocumentElement.Attributes["HostComputer"].Value;
        _pluginNameReceive  = doc.DocumentElement.Attributes["PluginReceive"].Value;
        _pluginNameTransmit = doc.DocumentElement.Attributes["PluginTransmit"].Value;
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex.ToString());

        _mode               = IRServerMode.ServerMode;
        _hostComputer       = String.Empty;
        _pluginNameReceive  = String.Empty;
        _pluginNameTransmit = String.Empty;
      }
    }
    void SaveSettings()
    {
      try
      {
        XmlTextWriter writer = new XmlTextWriter(ConfigurationFile, System.Text.Encoding.UTF8);
        writer.Formatting = Formatting.Indented;
        writer.Indentation = 1;
        writer.IndentChar = (char)9;
        writer.WriteStartDocument(true);
        writer.WriteStartElement("settings"); // <settings>

        writer.WriteAttributeString("Mode", Enum.GetName(typeof(IRServerMode), _mode));
        writer.WriteAttributeString("HostComputer", _hostComputer);
        writer.WriteAttributeString("PluginReceive", _pluginNameReceive);
        writer.WriteAttributeString("PluginTransmit", _pluginNameTransmit);

        writer.WriteEndElement(); // </settings>
        writer.WriteEndDocument();
        writer.Close();
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex.ToString());
      }
    }

  }

}