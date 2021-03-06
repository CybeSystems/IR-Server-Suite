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
using System.Diagnostics;
using System.Windows.Forms;

namespace Commands.General
{
  /// <summary>
  /// Run command.
  /// </summary>
  public class CommandRun : Command
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandRun"/> class.
    /// </summary>
    public CommandRun()
    {
      InitParameters(8);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandRun"/> class.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    public CommandRun(string[] parameters) : base(parameters)
    {
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Gets the category of this command.
    /// </summary>
    /// <returns>The category of this command.</returns>
    public override string GetCategory()
    {
      return "General Commands";
    }

    /// <summary>
    /// Gets the user interface text.
    /// </summary>
    /// <returns>User interface text.</returns>
    public override string GetUserInterfaceText()
    {
      return "Run Program";
    }

    /// <summary>
    /// Execute this command.
    /// </summary>
    /// <param name="variables">The variable list of the calling code.</param>
    public override void Execute(VariableList variables)
    {
      string[] processed = ProcessParameters(variables, Parameters);

      using (Process process = new Process())
      {
        process.StartInfo.FileName = processed[0];
        process.StartInfo.WorkingDirectory = processed[1];
        process.StartInfo.Arguments = processed[2];
        process.StartInfo.WindowStyle = (ProcessWindowStyle) Enum.Parse(typeof (ProcessWindowStyle), processed[3], true);
        process.StartInfo.CreateNoWindow = bool.Parse(processed[4]);
        process.StartInfo.UseShellExecute = bool.Parse(processed[5]);
        //process.PriorityClass               = ProcessPriorityClass.

        bool waitForExit = bool.Parse(processed[6]);
        bool forceFocus = bool.Parse(processed[7]);

        process.Start();

        // Give new process focus ...
        if (forceFocus && !process.StartInfo.CreateNoWindow &&
            process.StartInfo.WindowStyle != ProcessWindowStyle.Hidden)
        {
          //FocusForcer forcer = new FocusForcer(process.Id);
          //forcer.Start();
          //forcer.Force();
        }

        if (waitForExit)
          process.WaitForExit();
      }
    }

    /// <summary>
    /// Edit this command.
    /// </summary>
    /// <param name="parent">The parent window.</param>
    /// <returns><c>true</c> if the command was modified; otherwise <c>false</c>.</returns>
    public override bool Edit(IWin32Window parent)
    {
      EditPause edit = new EditPause(Parameters);
      if (edit.ShowDialog(parent) == DialogResult.OK)
      {
        Parameters = edit.Parameters;
        return true;
      }

      return false;
    }

    #endregion Public Methods
  }
}