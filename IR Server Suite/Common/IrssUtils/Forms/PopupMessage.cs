using System;
using System.Windows.Forms;

namespace IrssUtils.Forms
{
  /// <summary>
  /// Popup Message Command form.
  /// </summary>
  public partial class PopupMessage : Form
  {
    #region Properties

    /// <summary>
    /// Gets the command string.
    /// </summary>
    /// <value>The command string.</value>
    public string CommandString
    {
      get
      {
        return String.Format("{0}|{1}|{2}",
                             textBoxHeading.Text,
                             textBoxText.Text,
                             numericUpDownTimeout.Value);
      }
    }

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PopupMessage"/> class.
    /// </summary>
    public PopupMessage() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PopupMessage"/> class.
    /// </summary>
    /// <param name="commands">The command elements.</param>
    public PopupMessage(string[] commands)
    {
      InitializeComponent();

      if (commands != null)
      {
        textBoxHeading.Text = commands[0];
        textBoxText.Text = commands[1];
        numericUpDownTimeout.Value = Convert.ToDecimal(commands[2]);
      }
    }

    #endregion Constructors

    #region Buttons

    private void buttonOK_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    #endregion Buttons
  }
}