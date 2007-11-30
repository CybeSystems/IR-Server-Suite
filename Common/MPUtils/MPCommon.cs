using System;
using System.Collections.Generic;
using System.Text;

using MediaPortal.Configuration;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Util;

namespace MPUtils
{

  /// <summary>
  /// Contains common MediaPortal code and data.
  /// </summary>
  public static class MPCommon
  {

    #region Paths
    
    /// <summary>
    /// Folder for Custom Input Device data files.
    /// </summary>
    public static readonly string CustomInputDevice = Config.GetFolder(Config.Dir.CustomInputDevice) + "\\";
    
    /// <summary>
    /// Folder for Input Device data default files.
    /// </summary>
    public static readonly string CustomInputDefault = Config.GetFolder(Config.Dir.CustomInputDefault) + "\\";

    /// <summary>
    /// Path to the MediaPortal configuration file.
    /// </summary>
    public static readonly string MPConfigFile = Config.GetFolder(Config.Dir.Config) + "\\MediaPortal.xml";

    #endregion Paths

    #region Methods

    /// <summary>
    /// Pop up a dialog in MediaPortal.
    /// </summary>
    /// <param name="heading">Dialog heading text.</param>
    /// <param name="text">Dialog body text.</param>
    /// <param name="timeout">Dialog timeout in seconds, zero for no timeout.</param>
    public static void ShowNotifyDialog(string heading, string text, int timeout)
    {
      GUIDialogNotify dlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
      if (dlgNotify == null)
        throw new ApplicationException("Failed to create GUIDialogNotify");

      dlgNotify.Reset();
      dlgNotify.ClearAll();
      dlgNotify.SetHeading(heading);
      dlgNotify.SetText(text);
      dlgNotify.TimeOut = timeout;
      dlgNotify.DoModal(GUIWindowManager.ActiveWindow);
    }

    /// <summary>
    /// Takes a MediaPortal window name or window number and activates it.
    /// </summary>
    /// <param name="screen">MediaPortal window name or number.</param>
    /// <param name="useBasicHome">Use the basic home screen when home is requested.</param>
    public static void ProcessGoTo(string screen, bool useBasicHome)
    {
      if (String.IsNullOrEmpty(screen))
        throw new ArgumentNullException("screen");

      int window = (int)GUIWindow.Window.WINDOW_INVALID;

      try
      {
        window = (int)Enum.Parse(typeof(GUIWindow.Window), "WINDOW_" + screen, true);
      }
      catch (ArgumentException)
      {
        // Parsing the window id as a GUIWindow.Window failed, so parse it as an int
      }

      if (window == (int)GUIWindow.Window.WINDOW_INVALID)
        int.TryParse(screen, out window);

      if (window == (int)GUIWindow.Window.WINDOW_INVALID)
        throw new ArgumentException(String.Format("Failed to parse Goto command window id \"{0}\"", screen), "screen");

      if (window == (int)GUIWindow.Window.WINDOW_HOME && useBasicHome)
        window = (int)GUIWindow.Window.WINDOW_SECOND_HOME;

      GUIGraphicsContext.ResetLastActivity();
      GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_GOTO_WINDOW, 0, 0, 0, window, 0, null));
    }

    /// <summary>
    /// Put the computer into Hibernate in a MediaPortal friendly way.
    /// </summary>
    public static void Hibernate()
    {
      bool mpBasicHome = false;
      using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(MPConfigFile))
        mpBasicHome = xmlreader.GetValueAsBool("general", "startbasichome", false);

      GUIGraphicsContext.ResetLastActivity();
      // Stop all media before hibernating
      g_Player.Stop();

      GUIMessage msg;

      if (mpBasicHome)
        msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GOTO_WINDOW, 0, 0, 0, (int)GUIWindow.Window.WINDOW_SECOND_HOME, 0, null);
      else
        msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GOTO_WINDOW, 0, 0, 0, (int)GUIWindow.Window.WINDOW_HOME, 0, null);

      GUIWindowManager.SendThreadMessage(msg);

      WindowsController.ExitWindows(RestartOptions.Hibernate, false);
    }

    /// <summary>
    /// Put the computer into Standby in a MediaPortal friendly way.
    /// </summary>
    public static void Standby()
    {
      bool mpBasicHome = false;
      using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(MPConfigFile))
        mpBasicHome = xmlreader.GetValueAsBool("general", "startbasichome", false);

      GUIGraphicsContext.ResetLastActivity();
      // Stop all media before suspending
      g_Player.Stop();

      GUIMessage msg;

      if (mpBasicHome)
        msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GOTO_WINDOW, 0, 0, 0, (int)GUIWindow.Window.WINDOW_SECOND_HOME, 0, null);
      else
        msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GOTO_WINDOW, 0, 0, 0, (int)GUIWindow.Window.WINDOW_HOME, 0, null);

      GUIWindowManager.SendThreadMessage(msg);

      WindowsController.ExitWindows(RestartOptions.Suspend, false);
    }

    /// <summary>
    /// Reboot the computer in a MediaPortal friendly way.
    /// </summary>
    public static void Reboot()
    {
      GUIGraphicsContext.OnAction(new Action(Action.ActionType.ACTION_REBOOT, 0, 0));
    }

    /// <summary>
    /// Shut Down the computer in a MediaPortal friendly way.
    /// </summary>
    public static void ShutDown()
    {
      GUIGraphicsContext.OnAction(new Action(Action.ActionType.ACTION_SHUTDOWN, 0, 0));
    }
    
    #endregion Methods

  }

}