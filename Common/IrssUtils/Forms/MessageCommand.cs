using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace IrssUtils.Forms
{

  public partial class MessageCommand : Form
  {

    #region Properties

    public string CommandString
    {
      get
      {
        string target = "error";

        if (radioButtonActiveWindow.Checked)
        {
          target = "active";
          textBoxMsgTarget.Text = "*";
        }
        else if (radioButtonApplication.Checked)
        {
          target = "application";
        }
        else if (radioButtonClass.Checked)
        {
          target = "class";
        }
        else if (radioButtonWindowTitle.Checked)
        {
          target = "window";
        }

        return String.Format("{0}|{1}|{2}|{3}|{4}",
          target,
          textBoxMsgTarget.Text,
          numericUpDownMsg.Value.ToString(),
          numericUpDownWParam.Value.ToString(),
          numericUpDownLParam.Value.ToString());
      }
    }

    #endregion Properties

    #region Constructors

    public MessageCommand() : this(new string[] { "active", String.Empty, Win32.WM_USER.ToString(), "0", "0" }) { }
    public MessageCommand(string[] commands)
    {
      InitializeComponent();

      if (commands != null)
      {
        switch (commands[0].ToLowerInvariant())
        {
          case "active":
            radioButtonActiveWindow.Checked = true;
            break;
          case "application":
            radioButtonApplication.Checked = true;
            break;
          case "class":
            radioButtonClass.Checked = true;
            break;
          case "window":
            radioButtonWindowTitle.Checked = true;
            break;
        }

        textBoxMsgTarget.Text     = commands[1];
        numericUpDownMsg.Value    = decimal.Parse(commands[2]);
        numericUpDownWParam.Value = decimal.Parse(commands[3]);
        numericUpDownLParam.Value = decimal.Parse(commands[4]);
      }
    }

    #endregion Constructors

    #region Controls

    private void buttonFindMsgApp_Click(object sender, EventArgs e)
    {
      if (radioButtonApplication.Checked)
      {
        OpenFileDialog find = new OpenFileDialog();
        find.Filter = "All files|*.*";
        find.Multiselect = false;
        find.Title = "Application to send message to";

        if (find.ShowDialog(this) == DialogResult.OK)
            textBoxMsgTarget.Text = find.FileName;
      }
      else if (radioButtonClass.Checked)
      {
        // TODO: Locate Class
      }
      else if (radioButtonWindowTitle.Checked)
      {
        // TODO: Locate Window
      }
    }

    private void buttonOK_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void wMAPPToolStripMenuItem_Click(object sender, EventArgs e)
    {
      numericUpDownMsg.Value = new decimal(Win32.WM_APP);
    }
    private void wMUSERToolStripMenuItem_Click(object sender, EventArgs e)
    {
      numericUpDownMsg.Value = new decimal(Win32.WM_USER);
    }

    private void radioButtonActiveWindow_CheckedChanged(object sender, EventArgs e)
    {
      buttonFindMsgTarget.Enabled = false;
      textBoxMsgTarget.Enabled = false;
    }
    private void radioButtonApplication_CheckedChanged(object sender, EventArgs e)
    {
      buttonFindMsgTarget.Enabled = true;
      textBoxMsgTarget.Enabled = true;
    }
    private void radioButtonClass_CheckedChanged(object sender, EventArgs e)
    {
      buttonFindMsgTarget.Enabled = false;
      textBoxMsgTarget.Enabled = true;
    }
    private void radioButtonWindowTitle_CheckedChanged(object sender, EventArgs e)
    {
      buttonFindMsgTarget.Enabled = false;
      textBoxMsgTarget.Enabled = true;
    }

    #endregion Controls

  }

}