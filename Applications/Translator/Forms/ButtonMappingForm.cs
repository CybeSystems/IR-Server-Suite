using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

using NamedPipes;
using IrssUtils;

namespace Translator
{

  public partial class ButtonMappingForm : Form
  {

    #region Variables

    string _keyCode;
    string _description;
    string _command;

    #endregion Variables

    #region Properties

    internal string KeyCode
    {
      get { return _keyCode; }
    }
    internal string Description
    {
      get { return _description; }
    }
    internal string Command
    {
      get { return _command; }
    }

    #endregion Properties

    #region Constructors

    public ButtonMappingForm(string keyCode, string description, string command)
    {
      InitializeComponent();
      
      _keyCode = keyCode;
      _description = description;
      _command = command;
    }

    #endregion Constructors

    void SetupIRList()
    {
      comboBoxIRCode.Items.Clear();

      string[] irList = Common.GetIRList(false);
      if (irList != null && irList.Length > 0)
      {
        comboBoxIRCode.Items.AddRange(irList);
        comboBoxIRCode.SelectedIndex = 0;
      }
    }
    void SetupMacroList()
    {
      comboBoxMacro.Items.Clear();

      string[] macroList = Program.GetMacroList(false);
      if (macroList != null && macroList.Length > 0)
      {
        comboBoxMacro.Items.AddRange(macroList);
        comboBoxMacro.SelectedIndex = 0;
      }
    }

    private void ButtonMappingForm_Load(object sender, EventArgs e)
    {
      textBoxKeyCode.Text = _keyCode;
      textBoxButtonDesc.Text = _description;
      textBoxCommand.Text = _command;

      // Setup IR Blast tab
      SetupIRList();
      
      // Setup macro tab
      SetupMacroList();
      
      comboBoxPort.Items.Clear();
      comboBoxPort.Items.AddRange(Program.TransceiverInformation.Ports);
      if (comboBoxPort.Items.Count > 0)
        comboBoxPort.SelectedIndex = 0;

      comboBoxSpeed.Items.Clear();
      comboBoxSpeed.Items.AddRange(Program.TransceiverInformation.Speeds);
      if (comboBoxSpeed.Items.Count > 0)
        comboBoxSpeed.SelectedIndex = 0;

      // Setup Serial tab
      comboBoxComPort.Items.Clear();
      comboBoxComPort.Items.AddRange(SerialPort.GetPortNames());
      if (comboBoxComPort.Items.Count > 0)
        comboBoxComPort.SelectedIndex = 0;

      comboBoxParity.Items.Clear();
      comboBoxParity.Items.AddRange(Enum.GetNames(typeof(Parity)));
      comboBoxParity.SelectedIndex = 0;

      comboBoxStopBits.Items.Clear();
      comboBoxStopBits.Items.AddRange(Enum.GetNames(typeof(StopBits)));
      comboBoxStopBits.SelectedIndex = 0;

      // Setup Run tab
      comboBoxWindowStyle.Items.Clear();
      comboBoxWindowStyle.Items.AddRange(Enum.GetNames(typeof(ProcessWindowStyle)));
      comboBoxWindowStyle.SelectedIndex = 0;

      if (!String.IsNullOrEmpty(_command))
      {
        int find = _command.IndexOf(": ");

        string prefix = _command.Substring(0, find + 2);
        string suffix = _command.Substring(find + 2);
        
        switch (prefix)
        {
          case Common.CmdPrefixBlast:
            {
              string[] commands = Common.SplitBlastCommand(suffix);
              
              tabControl.SelectTab(tabPageBlastIR);
              comboBoxIRCode.SelectedItem = commands[0];
              comboBoxPort.SelectedItem = commands[1];
              comboBoxSpeed.SelectedItem = commands[2];
              break;
            }

          case Common.CmdPrefixMacro:
            {
              tabControl.SelectTab(tabPageMacro);
              comboBoxMacro.SelectedItem = suffix;
              break;
            }

          case Common.CmdPrefixRun:
            {
              string[] commands = Common.SplitRunCommand(suffix);

              tabControl.SelectTab(tabPageProgram);
              textBoxApp.Text = commands[0];
              textBoxAppStartFolder.Text = commands[1];
              textBoxApplicationParameters.Text = commands[2];
              comboBoxWindowStyle.SelectedItem = commands[3];
              checkBoxNoWindow.Checked = bool.Parse(commands[4]);
              checkBoxShellExecute.Checked = bool.Parse(commands[5]);
              break;
            }

          case Common.CmdPrefixSerial:
            {
              string[] commands = Common.SplitSerialCommand(suffix);

              tabControl.SelectTab(tabPageSerial);
              textBoxSerialCommand.Text = commands[0];
              comboBoxComPort.SelectedItem = commands[1];
              comboBoxParity.SelectedItem = commands[2];
              comboBoxStopBits.SelectedItem = commands[3];
              numericUpDownBaudRate.Value = decimal.Parse(commands[4]);
              numericUpDownDataBits.Value = decimal.Parse(commands[5]);
              break;
            }

          case Common.CmdPrefixMessage:
            {
              string[] commands = Common.SplitMessageCommand(suffix);

              tabControl.SelectTab(tabPageMessage);
              textBoxMsgApp.Text = (checkBoxMsgCurrApp.Checked ? "*" : commands[0]);
              numericUpDownMsg.Value = decimal.Parse(commands[1]);
              numericUpDownWParam.Value = decimal.Parse(commands[2]);
              numericUpDownLParam.Value = decimal.Parse(commands[3]);
              break;
            }

          case Common.CmdPrefixKeys:
            {
              tabControl.SelectTab(tabPageKeystrokes);
              textBoxKeys.Text = suffix;
              break;
            }
        }
      }

    }

    #region Controls

    private void buttonOK_Click(object sender, EventArgs e)
    {
      if (String.IsNullOrEmpty(_keyCode) || String.IsNullOrEmpty(_command))
      {
        MessageBox.Show(this, "You must set a valid button mapping to press OK", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
      MessageBox.Show(this, 
@"\a = Alert (ascii 7)
\b = Backspace (ascii 8)
\f = Form Feed (ascii 12)
\n = Line Feed (ascii 10)
\r = Carriage Return (ascii 13)
\t = Tab (ascii 9)
\v = Vertical Tab (ascii 11)
\x = Hex Value (\x0Fh = ascii char 15, \x8h = ascii char 8)
\0 = Null (ascii 0)

", "Parameters", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void buttonSet_Click(object sender, EventArgs e)
    {
      switch (tabControl.SelectedTab.Name)
      {
        case "tabPageBlastIR":
          {
            textBoxCommand.Text = _command =
              String.Format("{0}{1}|{2}|{3}",
                Common.CmdPrefixBlast,
                comboBoxIRCode.SelectedItem as string,
                comboBoxPort.SelectedItem as string,
                comboBoxSpeed.SelectedItem as string);
            break;
          }

        case "tabPageMacro":
          {
            textBoxCommand.Text = _command = Common.CmdPrefixMacro + comboBoxMacro.SelectedItem as string;
            break;
          }

        case "tabPageSerial":
          {
            textBoxCommand.Text = _command =
              String.Format("{0}{1}|{2}|{3}|{4}|{5}|{6}",
                Common.CmdPrefixSerial,
                textBoxSerialCommand.Text,
                comboBoxComPort.SelectedItem as string,
                numericUpDownBaudRate.Value.ToString(),
                comboBoxParity.SelectedItem as string,
                numericUpDownDataBits.Value.ToString(),
                comboBoxStopBits.SelectedItem as string);
            break;
          }

        case "tabPageProgram":
          {
            textBoxCommand.Text = _command =
              String.Format("{0}{1}|{2}|{3}|{4}|{5}|{6}|False",
                Common.CmdPrefixRun,
                textBoxApp.Text,
                textBoxAppStartFolder.Text,
                textBoxApplicationParameters.Text,
                comboBoxWindowStyle.SelectedItem as string,
                checkBoxNoWindow.Checked.ToString(),
                checkBoxShellExecute.Checked.ToString());
            break;
          }

        case "tabPageMessage":
          {
            textBoxCommand.Text = _command =
              String.Format("{0}{1}|{2}|{3}|{4}",
                Common.CmdPrefixMessage,
                checkBoxMsgCurrApp.Checked ? "*" : textBoxMsgApp.Text,
                numericUpDownMsg.Value.ToString(),
                numericUpDownWParam.Value.ToString(),
                numericUpDownLParam.Value.ToString());
            break;
          }

        case "tabPageKeystrokes":
          {
            textBoxCommand.Text = _command = Common.CmdPrefixKeys + textBoxKeys.Text;
            break;
          }
      }
    }

    private void buttonTest_Click(object sender, EventArgs e)
    {
      if (_command.StartsWith(Common.CmdPrefixKeys))
        MessageBox.Show(this, "Keystroke commands cannot be tested here", "Cannot test Keystroke command", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      else
      {
        try
        {
          Program.ProcessCommand(_command);
        }
        catch (Exception ex)
        {
          MessageBox.Show(this, ex.Message, "Test failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void buttonFindMsgApp_Click(object sender, EventArgs e)
    {
      OpenFileDialog find = new OpenFileDialog();
      find.Filter = "All files|*.*";
      find.Multiselect = false;
      find.Title = "Application to send message to";

      if (find.ShowDialog(this) == DialogResult.OK)
        textBoxMsgApp.Text = find.FileName;
    }

    private void buttonLocate_Click(object sender, EventArgs e)
    {
      OpenFileDialog find = new OpenFileDialog();
      find.Filter = "All files|*.*";
      find.Multiselect = false;
      find.Title = "Application to launch";

      if (find.ShowDialog(this) == DialogResult.OK)
      {
        textBoxApp.Text = find.FileName;
        if (String.IsNullOrEmpty(textBoxAppStartFolder.Text))
          textBoxAppStartFolder.Text = Path.GetDirectoryName(find.FileName);
      }
    }

    private void buttonStartupFolder_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog find = new FolderBrowserDialog();
      find.Description = "Please specify the starting folder for the application";
      find.ShowNewFolderButton = true;
      if (find.ShowDialog(this) == DialogResult.OK)
        textBoxAppStartFolder.Text = find.SelectedPath;
    }

    private void buttonLearnIR_Click(object sender, EventArgs e)
    {
      LearnIR learnIR = new LearnIR(true, String.Empty);
      learnIR.ShowDialog(this);
      
      SetupIRList();
    }

    private void buttonNewMacro_Click(object sender, EventArgs e)
    {
      MacroEditor macroEditor = new MacroEditor(true, String.Empty);
      macroEditor.ShowDialog(this);

      SetupMacroList();
    }

    private void buttonKeyHelp_Click(object sender, EventArgs e)
    {
      MessageBox.Show(this,
@"Place special keys inside {}, for example: {F4} for the F4 key.
Use + to apply the SHIFT key to the following character,
Use ^ to apply the CONTROL key to the following character,
Use % to apply the ALT key to the following character.

For more information refer to 'Simulating Keystrokes.txt'
", "Advanced keystroke input", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void checkBoxMsgCurrApp_CheckedChanged(object sender, EventArgs e)
    {
      textBoxMsgApp.Enabled = !checkBoxMsgCurrApp.Checked;
    }

    private void textBoxButtonDesc_TextChanged(object sender, EventArgs e)
    {
      _description = textBoxButtonDesc.Text;
    }

    #endregion Controls

  }

}