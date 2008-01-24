using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Commands
{

  /// <summary>
  /// String To Lower macro command.
  /// </summary>
  public class CommandStringToLower : Command
  {

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandStringToLower"/> class.
    /// </summary>
    public CommandStringToLower() { InitParameters(2); }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandStringToLower"/> class.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    public CommandStringToLower(string[] parameters) : base(parameters) { }

    #endregion Constructors

    #region Implementation

    /// <summary>
    /// Gets the category of this command.
    /// </summary>
    /// <returns>The category of this command.</returns>
    public override string GetCategory() { return Processor.CategoryString; }

    /// <summary>
    /// Gets the user interface text.
    /// </summary>
    /// <returns>User interface text.</returns>
    public override string GetUserInterfaceText() { return "String To Lower"; }

    /// <summary>
    /// Edit this command.
    /// </summary>
    /// <param name="parent">The parent window.</param>
    /// <returns><c>true</c> if the command was modified; otherwise <c>false</c>.</returns>
    public override bool Edit(IWin32Window parent)
    {
      EditStringOperation edit = new EditStringOperation(Parameters);
      if (edit.ShowDialog(parent) == DialogResult.OK)
      {
        Parameters = edit.Parameters;
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
      string input = Parameters[0];
      if (input.StartsWith(VariableList.VariablePrefix, StringComparison.OrdinalIgnoreCase))
        input = variables.VariableGet(input.Substring(VariableList.VariablePrefix.Length));
      input = IrssUtils.Common.ReplaceSpecial(input);

      string output = input.ToLower(System.Globalization.CultureInfo.CurrentCulture);

      variables.VariableSet(Parameters[2], output);
    }

    #endregion Implementation

  }

}
