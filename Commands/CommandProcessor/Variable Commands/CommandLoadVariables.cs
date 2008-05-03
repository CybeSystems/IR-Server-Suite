using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Commands
{

  /// <summary>
  /// Load Variables macro command.
  /// </summary>
  public class CommandLoadVariables : Command
  {

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLoadVariables"/> class.
    /// </summary>
    public CommandLoadVariables() { InitParameters(1); }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLoadVariables"/> class.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    public CommandLoadVariables(string[] parameters) : base(parameters) { }

    #endregion Constructors

    #region Implementation

    /// <summary>
    /// Gets the category of this command.
    /// </summary>
    /// <returns>The category of this command.</returns>
    public override string GetCategory() { return Processor.CategoryVariable; }

    /// <summary>
    /// Gets the user interface text.
    /// </summary>
    /// <returns>User interface text.</returns>
    public override string GetUserInterfaceText() { return "Load Variables"; }

    /// <summary>
    /// Edit this command.
    /// </summary>
    /// <param name="parent">The parent window.</param>
    /// <returns><c>true</c> if the command was modified; otherwise <c>false</c>.</returns>
    public override bool Edit(IWin32Window parent)
    {
      EditVariablesFile edit = new EditVariablesFile(Parameters[0]);
      if (edit.ShowDialog(parent) == DialogResult.OK)
      {
        Parameters[0] = edit.FileName;
        return true;
      }

      return false;
    }

    /// <summary>
    /// Execute this command.
    /// </summary>
    /// <param name="variables">The variable list of the calling code.</param>
    public override void Execute(VariableList variables)
    {
      string[] processed = ProcessParameters(variables, Parameters);
      variables.VariableLoad(processed[0]);
    }

    #endregion Implementation

  }

}
