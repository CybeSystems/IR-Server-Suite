using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IrssUtils.Forms
{

  public partial class ExternalProgram : Form
  {

    #region Variables

    string _parametersMessage = String.Empty;

    #endregion Variables

    #region Properties

    public string CommandString
    {
      get
      {
        return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
          textBoxProgram.Text,
          textBoxStartup.Text,
          textBoxParameters.Text,
          comboBoxWindowStyle.SelectedItem as string,
          checkBoxNoWindow.Checked.ToString(),
          checkBoxShellExecute.Checked.ToString(),
          checkBoxWaitForExit.Checked.ToString());
      }
    }

    #endregion Properties

    #region Constructors

    public ExternalProgram() : this(null, String.Empty, true) { }
    public ExternalProgram(bool canWait) : this(null, String.Empty, canWait) { }
    public ExternalProgram(string parametersMessage) : this(null, parametersMessage, true) { }
    public ExternalProgram(string[] commands) : this(commands, String.Empty, true) { }
    public ExternalProgram(string[] commands, bool canWait) : this(commands, String.Empty, canWait) { }
    public ExternalProgram(string[] commands, string parametersMessage) : this(commands, String.Empty, true) { }
    public ExternalProgram(string[] commands, string parametersMessage, bool canWait)
    {
      InitializeComponent();

      if (canWait)
      {
        checkBoxWaitForExit.Visible = true;
        checkBoxWaitForExit.Enabled = true;
      }
      else
      {
        checkBoxWaitForExit.Visible = false;
        checkBoxWaitForExit.Enabled = false;
        checkBoxWaitForExit.Checked = false;
      }

      _parametersMessage = parametersMessage;

      comboBoxWindowStyle.Items.Clear();
      comboBoxWindowStyle.Items.AddRange(Enum.GetNames(typeof(ProcessWindowStyle)));

      if (commands != null)
      {
        textBoxProgram.Text           = commands[0];
        textBoxStartup.Text           = commands[1];
        textBoxParameters.Text        = commands[2];

        checkBoxNoWindow.Checked      = bool.Parse(commands[4]);
        checkBoxShellExecute.Checked  = bool.Parse(commands[5]);
        checkBoxWaitForExit.Checked   = bool.Parse(commands[6]);

        comboBoxWindowStyle.SelectedItem  = ((ProcessWindowStyle)Enum.Parse(typeof(ProcessWindowStyle), commands[3])).ToString();
      }
      else
      {
        comboBoxWindowStyle.SelectedIndex = 0;
      }
    }

    #endregion Constructors

    private void ExternalProgram_Load(object sender, EventArgs e)
    {
      if (_parametersMessage.Trim().Length == 0)
        buttonParamQuestion.Visible = false;
    }

    #region Buttons

    private void buttonProgam_Click(object sender, EventArgs e)
    {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK)
      {
        textBoxProgram.Text = openFileDialog.FileName;

        if (textBoxStartup.Text.Trim().Length == 0)
        {
          textBoxStartup.Text = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
        }
      }
    }

    private void buttonStartup_Click(object sender, EventArgs e)
    {
      if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
      {
        textBoxProgram.Text = folderBrowserDialog.SelectedPath;
      }
    }

    private void buttonOK_Click(object sender, EventArgs e)
    {
      if (textBoxProgram.Text.Trim().Length == 0)
      {
        MessageBox.Show(this, "You must specify a program to run", "Missing program name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return;
      }
      
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void buttonParamQuestion_Click(object sender, EventArgs e)
    {
      MessageBox.Show(this, _parametersMessage, "Parameters", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void buttonTest_Click(object sender, EventArgs e)
    {
      if (textBoxProgram.Text.Trim().Length == 0)
      {
        MessageBox.Show(this, "You must specify a program to run", "Missing program name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return;
      }

      try
      {
        Process process = new Process();
        process.StartInfo.FileName = textBoxProgram.Text;
        process.StartInfo.WorkingDirectory = textBoxStartup.Text;
        process.StartInfo.Arguments = textBoxParameters.Text;
        process.StartInfo.WindowStyle = (ProcessWindowStyle)Enum.Parse(typeof(ProcessWindowStyle), comboBoxWindowStyle.SelectedItem as string);
        process.StartInfo.CreateNoWindow = checkBoxNoWindow.Checked;
        process.StartInfo.UseShellExecute = checkBoxShellExecute.Checked;

        process.Start();

        if (checkBoxWaitForExit.Visible && checkBoxWaitForExit.Checked)  // Wait for exit
          process.WaitForExit();
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, ex.Message, "Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    #endregion Buttons

  }

}