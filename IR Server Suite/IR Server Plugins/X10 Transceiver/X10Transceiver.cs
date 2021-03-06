#region Copyright (C) 2005-2009 Team MediaPortal

// Copyright (C) 2005-2009 Team MediaPortal
// http://www.team-mediaportal.com
// 
// This Program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.
// 
// This Program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with GNU Make; see the file COPYING.  If not, write to
// the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
// http://www.gnu.org/copyleft/gpl.html

#endregion

using System;
#if TRACE
using System.Diagnostics;
#endif
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using IRServer.Plugin.Properties;
using IrssUtils;
using X10;

namespace IRServer.Plugin
{
  /// <summary>
  /// IR Server Plugin for X10 Transceiver devices.
  /// </summary>
  public class X10Transceiver : PluginBase, IRemoteReceiver, _DIX10InterfaceEvents, IConfigure
  {
    #region Constants

    private static readonly string ConfigurationFile = Path.Combine(ConfigurationPath, "X10 Transceiver.xml");

    #endregion Constants

    #region Variables

    private static RemoteHandler _remoteButtonHandler;

    private int cookie;
    private IConnectionPoint icp;
    private IConnectionPointContainer icpc;
    private X10Interface X10Inter;
    private bool useChannelControl;
    private int channelNumber;
    private bool getChannelNumber;

    #endregion Variables

    /// <summary>
    /// Name of the IR Server plugin.
    /// </summary>
    /// <value>The name.</value>
    public override string Name
    {
      get { return "X10"; }
    }

    /// <summary>
    /// IR Server plugin version.
    /// </summary>
    /// <value>The version.</value>
    public override string Version
    {
      get { return "1.4.2.0"; }
    }

    /// <summary>
    /// The IR Server plugin's author.
    /// </summary>
    /// <value>The author.</value>
    public override string Author
    {
      get { return "and-81, with original MediaPortal code by CoolHammer, mPod and diehard2"; }
    }

    /// <summary>
    /// A description of the IR Server plugin.
    /// </summary>
    /// <value>The description.</value>
    public override string Description
    {
      get { return "X10 Transceiver"; }
    }

    /// <summary>
    /// Gets a display icon for the plugin.
    /// </summary>
    /// <value>The icon.</value>
    public override Icon DeviceIcon
    {
      get { return Resources.Icon; }
    }

    #region _DIX10InterfaceEvents Members

    /// <summary>
    /// X10 command.
    /// </summary>
    /// <param name="bszCommand">The command.</param>
    /// <param name="eCommand">The command type.</param>
    /// <param name="lAddress">The address.</param>
    /// <param name="EKeyState">State of the key.</param>
    /// <param name="lSequence">The sequence.</param>
    /// <param name="eCommandType">Type of the command.</param>
    /// <param name="varTimestamp">The timestamp.</param>
    [CLSCompliant(false)]
    public void X10Command(string bszCommand, EX10Command eCommand, int lAddress, EX10Key EKeyState, int lSequence,
                           EX10Comm eCommandType, object varTimestamp)
    {
      if ((EKeyState == EX10Key.X10KEY_ON || EKeyState == EX10Key.X10KEY_REPEAT) && lSequence != 2)
      {
        try
        {
          if (getChannelNumber)
          {
            channelNumber = lAddress;
            getChannelNumber = false;
            return;
          }
          if (useChannelControl && (lAddress != channelNumber))
          {
            return;
          }

          string keyCode = Enum.GetName(typeof(EX10Command), eCommand);

          if (RemoteCallback != null)
            RemoteCallback(Name, keyCode);
        }
#if TRACE
        catch (Exception ex)
        {
          Trace.WriteLine(ex.ToString());
        }
#else
        catch
        {
        }
#endif
      }
    }

    /// <summary>
    /// X10 help event.
    /// </summary>
    /// <param name="hwndDialog">The HWND of the dialog.</param>
    /// <param name="lHelpID">The help ID.</param>
    public void X10HelpEvent(int hwndDialog, int lHelpID)
    {
    }

    #endregion

    #region IRemoteReceiver Members

    /// <summary>
    /// Callback for remote button presses.
    /// </summary>
    /// <value>The remote callback.</value>
    public RemoteHandler RemoteCallback
    {
      get { return _remoteButtonHandler; }
      set { _remoteButtonHandler = value; }
    }

    #endregion

    /// <summary>
    /// Detect the presence of this device.
    /// </summary>
    public override DetectionResult Detect()
    {
      try
      {
        new X10Interface();
        return DetectionResult.DevicePresent;
      }
      catch (COMException ex)
      {
        if (ex.ErrorCode == -2147221164)
        {
          IrssLog.Warn("{0,15}: needed interface not found", Name);
        }
        else
        {
          IrssLog.Debug("{0,15}: exception {1}", Name, ex.ErrorCode);
          return DetectionResult.DeviceException;
        }
      }
      catch (Exception ex)
      {
        IrssLog.Error("{0,15}: exception {1} type: {2}", Name, ex.Message, ex.GetType());
        return DetectionResult.DeviceException;
      }

      return DetectionResult.DeviceNotFound;
    }

    /// <summary>
    /// Start the IR Server plugin.
    /// </summary>
    public override void Start()
    {
      LoadSettings();
      X10Inter = new X10Interface();
      if (X10Inter == null)
        throw new InvalidOperationException("Failed to start X10 interface");

      // Bind the interface using a connection point
      icpc = (IConnectionPointContainer)X10Inter;
      Guid IID_InterfaceEvents = typeof(_DIX10InterfaceEvents).GUID;
      icpc.FindConnectionPoint(ref IID_InterfaceEvents, out icp);
      icp.Advise(this, out cookie);
    }

    /// <summary>
    /// Suspend the IR Server plugin when computer enters standby.
    /// </summary>
    public override void Suspend()
    {
      Stop();
    }

    /// <summary>
    /// Resume the IR Server plugin when the computer returns from standby.
    /// </summary>
    public override void Resume()
    {
      Start();
    }

    /// <summary>
    /// Stop the IR Server plugin.
    /// </summary>
    public override void Stop()
    {
      if (X10Inter != null)
      {
        icp.Unadvise(cookie);
        icpc = null;
        X10Inter = null;
      }
    }

    /// <summary>
    /// Starts to wait for the channel number
    /// </summary>
    public void StartGetChannelNumber()
    {
      Start();
      getChannelNumber = true;
      channelNumber = -1;
    }

    /// <summary>
    /// Get the found channel number
    /// </summary>
    public int GetChannelNumber()
    {
      return channelNumber;
    }

    /// <summary>
    /// Stops to wait for the channel number
    /// </summary>
    public void StopGetChannelNumber()
    {
      getChannelNumber = false;
      LoadSettings();
      Stop();
    }

    private void LoadSettings()
    {
      try
      {
        getChannelNumber = false;
        XmlDocument doc = new XmlDocument();
        doc.Load(ConfigurationFile);

        useChannelControl = bool.Parse(doc.DocumentElement.Attributes["useChannelControl"].Value);
        channelNumber = int.Parse(doc.DocumentElement.Attributes["channelNumber"].Value);
      }
#if TRACE
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
#else
      catch
      {
#endif
        useChannelControl = false;
        channelNumber = 0;
      }
    }

    private void SaveSettings()
    {
      try
      {
        XmlTextWriter writer = new XmlTextWriter(ConfigurationFile, Encoding.UTF8);
        writer.Formatting = Formatting.Indented;
        writer.Indentation = 1;
        writer.IndentChar = (char)9;
        writer.WriteStartDocument(true);
        writer.WriteStartElement("settings"); // <settings>

        writer.WriteAttributeString("useChannelControl", useChannelControl.ToString());
        writer.WriteAttributeString("channelNumber", channelNumber.ToString());

        writer.WriteEndElement(); // </settings>
        writer.WriteEndDocument();
        writer.Close();
      }
#if TRACE
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
#else
      catch
      {
      }
#endif
    }

    /// <summary>
    /// Configure the IR Server plugin.
    /// </summary>
    /// <param name="owner">The owner window to use for creating modal dialogs.</param>
    public void Configure(IWin32Window owner)
    {
      LoadSettings();

      Configure config = new Configure();

      config.UseChannelControl = useChannelControl;
      config.ChannelNumber = channelNumber;
      config.X10Transceiver = this;
      if (config.ShowDialog(owner) == DialogResult.OK)
      {
        useChannelControl = config.UseChannelControl;
        channelNumber = config.ChannelNumber;
        SaveSettings();
      }
    }
  }
}