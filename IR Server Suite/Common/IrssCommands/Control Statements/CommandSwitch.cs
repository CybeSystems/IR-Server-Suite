using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace IrssCommands
{
  /// <summary>
  /// Switch Statement macro command.
  /// </summary>
  public class CommandSwitch : Command
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandSwitch"/> class.
    /// </summary>
    public CommandSwitch()
    {
      InitParameters(3);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandSwitch"/> class.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    public CommandSwitch(string[] parameters) : base(parameters)
    {
    }

    #endregion Constructors

    #region Implementation

    /// <summary>
    /// Gets the category of this command.
    /// </summary>
    /// <value>The category of this command.</value>
    public override string Category
    {
      get { return Processor.CategoryControl; }
    }

    /// <summary>
    /// Gets the user interface text.
    /// </summary>
    /// <value>User interface text.</value>
    public override string UserInterfaceText
    {
      get { return "Switch Statement"; }
    }

    /// <summary>
    /// Gets the user display text.
    /// </summary>
    /// <value>The user display text.</value>
    public override string UserDisplayText
    {
      get { return String.Format("Switch ({0}) ...", Parameters[0]); }
    }

    /// <summary>
    /// Gets the edit control to be used within a common edit form.
    /// </summary>
    /// <returns>The edit control.</returns>
    public override BaseCommandConfig GetEditControl()
    {
      return new SwitchConfig(Parameters);
    }

    #endregion Implementation

    #region Static Methods

    /// <summary>
    /// This method will determine which (if any) case in a Switch Statement evaluates true.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="casesXml">The cases XML.</param>
    /// <returns>Label to goto (null for default).</returns>
    public static string Evaluate(string value, string casesXml)
    {
      string[] cases;
      using (StringReader stringReader = new StringReader(casesXml))
      {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (string[]));
        cases = (string[]) xmlSerializer.Deserialize(stringReader);
      }

      for (int index = 0; index < cases.Length; index += 2)
        if (value.Equals(cases[index], StringComparison.OrdinalIgnoreCase))
          return cases[index + 1];

      return null;
    }

    #endregion Static Methods
  }
}