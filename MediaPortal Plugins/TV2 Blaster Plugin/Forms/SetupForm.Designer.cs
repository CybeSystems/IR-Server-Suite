namespace MediaPortal.Plugins
{
  partial class SetupForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.buttonOK = new System.Windows.Forms.Button();
      this.buttonNewMacro = new System.Windows.Forms.Button();
      this.buttonEditIR = new System.Windows.Forms.Button();
      this.buttonDeleteIR = new System.Windows.Forms.Button();
      this.buttonNewIR = new System.Windows.Forms.Button();
      this.listBoxIR = new System.Windows.Forms.ListBox();
      this.buttonTestMacro = new System.Windows.Forms.Button();
      this.listBoxMacro = new System.Windows.Forms.ListBox();
      this.buttonDeleteMacro = new System.Windows.Forms.Button();
      this.buttonEditMacro = new System.Windows.Forms.Button();
      this.buttonCancel = new System.Windows.Forms.Button();
      this.checkBoxLogVerbose = new System.Windows.Forms.CheckBox();
      this.buttonExtChannels = new System.Windows.Forms.Button();
      this.toolTips = new System.Windows.Forms.ToolTip(this.components);
      this.tabControl = new System.Windows.Forms.TabControl();
      this.tabPageOptions = new System.Windows.Forms.TabPage();
      this.tabPageIR = new System.Windows.Forms.TabPage();
      this.tabPageMacros = new System.Windows.Forms.TabPage();
      this.buttonChangeServer = new System.Windows.Forms.Button();
      this.buttonHelp = new System.Windows.Forms.Button();
      this.tabControl.SuspendLayout();
      this.tabPageOptions.SuspendLayout();
      this.tabPageIR.SuspendLayout();
      this.tabPageMacros.SuspendLayout();
      this.SuspendLayout();
      // 
      // buttonOK
      // 
      this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonOK.Location = new System.Drawing.Point(264, 256);
      this.buttonOK.Name = "buttonOK";
      this.buttonOK.Size = new System.Drawing.Size(56, 24);
      this.buttonOK.TabIndex = 1;
      this.buttonOK.Text = "&OK";
      this.buttonOK.UseVisualStyleBackColor = true;
      this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
      // 
      // buttonNewMacro
      // 
      this.buttonNewMacro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonNewMacro.Location = new System.Drawing.Point(8, 184);
      this.buttonNewMacro.Name = "buttonNewMacro";
      this.buttonNewMacro.Size = new System.Drawing.Size(56, 24);
      this.buttonNewMacro.TabIndex = 1;
      this.buttonNewMacro.Text = "New";
      this.toolTips.SetToolTip(this.buttonNewMacro, "Create a new Macro");
      this.buttonNewMacro.UseVisualStyleBackColor = true;
      this.buttonNewMacro.Click += new System.EventHandler(this.buttonNewMacro_Click);
      // 
      // buttonEditIR
      // 
      this.buttonEditIR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonEditIR.Location = new System.Drawing.Point(72, 184);
      this.buttonEditIR.Name = "buttonEditIR";
      this.buttonEditIR.Size = new System.Drawing.Size(56, 24);
      this.buttonEditIR.TabIndex = 2;
      this.buttonEditIR.Text = "Edit";
      this.toolTips.SetToolTip(this.buttonEditIR, "Re-Learn an existing IR command");
      this.buttonEditIR.UseVisualStyleBackColor = true;
      this.buttonEditIR.Click += new System.EventHandler(this.buttonEditIR_Click);
      // 
      // buttonDeleteIR
      // 
      this.buttonDeleteIR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonDeleteIR.Location = new System.Drawing.Point(136, 184);
      this.buttonDeleteIR.Name = "buttonDeleteIR";
      this.buttonDeleteIR.Size = new System.Drawing.Size(56, 24);
      this.buttonDeleteIR.TabIndex = 3;
      this.buttonDeleteIR.Text = "Delete";
      this.toolTips.SetToolTip(this.buttonDeleteIR, "Delete an IR command file");
      this.buttonDeleteIR.UseVisualStyleBackColor = true;
      this.buttonDeleteIR.Click += new System.EventHandler(this.buttonDeleteIR_Click);
      // 
      // buttonNewIR
      // 
      this.buttonNewIR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonNewIR.Location = new System.Drawing.Point(8, 184);
      this.buttonNewIR.Name = "buttonNewIR";
      this.buttonNewIR.Size = new System.Drawing.Size(56, 24);
      this.buttonNewIR.TabIndex = 1;
      this.buttonNewIR.Text = "New";
      this.toolTips.SetToolTip(this.buttonNewIR, "Learn a new IR command");
      this.buttonNewIR.UseVisualStyleBackColor = true;
      this.buttonNewIR.Click += new System.EventHandler(this.buttonNewIR_Click);
      // 
      // listBoxIR
      // 
      this.listBoxIR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.listBoxIR.ColumnWidth = 260;
      this.listBoxIR.FormattingEnabled = true;
      this.listBoxIR.HorizontalScrollbar = true;
      this.listBoxIR.IntegralHeight = false;
      this.listBoxIR.Location = new System.Drawing.Point(8, 8);
      this.listBoxIR.MultiColumn = true;
      this.listBoxIR.Name = "listBoxIR";
      this.listBoxIR.Size = new System.Drawing.Size(352, 168);
      this.listBoxIR.TabIndex = 0;
      this.listBoxIR.DoubleClick += new System.EventHandler(this.listBoxIR_DoubleClick);
      // 
      // buttonTestMacro
      // 
      this.buttonTestMacro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonTestMacro.Location = new System.Drawing.Point(304, 184);
      this.buttonTestMacro.Name = "buttonTestMacro";
      this.buttonTestMacro.Size = new System.Drawing.Size(56, 24);
      this.buttonTestMacro.TabIndex = 4;
      this.buttonTestMacro.Text = "Test";
      this.toolTips.SetToolTip(this.buttonTestMacro, "Test a Macro");
      this.buttonTestMacro.UseVisualStyleBackColor = true;
      this.buttonTestMacro.Click += new System.EventHandler(this.buttonTestMacro_Click);
      // 
      // listBoxMacro
      // 
      this.listBoxMacro.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.listBoxMacro.ColumnWidth = 260;
      this.listBoxMacro.FormattingEnabled = true;
      this.listBoxMacro.HorizontalScrollbar = true;
      this.listBoxMacro.IntegralHeight = false;
      this.listBoxMacro.Location = new System.Drawing.Point(8, 8);
      this.listBoxMacro.MultiColumn = true;
      this.listBoxMacro.Name = "listBoxMacro";
      this.listBoxMacro.Size = new System.Drawing.Size(352, 168);
      this.listBoxMacro.TabIndex = 0;
      this.listBoxMacro.DoubleClick += new System.EventHandler(this.listBoxMacro_DoubleClick);
      // 
      // buttonDeleteMacro
      // 
      this.buttonDeleteMacro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonDeleteMacro.Location = new System.Drawing.Point(136, 184);
      this.buttonDeleteMacro.Name = "buttonDeleteMacro";
      this.buttonDeleteMacro.Size = new System.Drawing.Size(56, 24);
      this.buttonDeleteMacro.TabIndex = 3;
      this.buttonDeleteMacro.Text = "Delete";
      this.toolTips.SetToolTip(this.buttonDeleteMacro, "Delete a Macro file");
      this.buttonDeleteMacro.UseVisualStyleBackColor = true;
      this.buttonDeleteMacro.Click += new System.EventHandler(this.buttonDeleteMacro_Click);
      // 
      // buttonEditMacro
      // 
      this.buttonEditMacro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonEditMacro.Location = new System.Drawing.Point(72, 184);
      this.buttonEditMacro.Name = "buttonEditMacro";
      this.buttonEditMacro.Size = new System.Drawing.Size(56, 24);
      this.buttonEditMacro.TabIndex = 2;
      this.buttonEditMacro.Text = "Edit";
      this.toolTips.SetToolTip(this.buttonEditMacro, "Edit an existing Macro");
      this.buttonEditMacro.UseVisualStyleBackColor = true;
      this.buttonEditMacro.Click += new System.EventHandler(this.buttonEditMacro_Click);
      // 
      // buttonCancel
      // 
      this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.buttonCancel.Location = new System.Drawing.Point(328, 256);
      this.buttonCancel.Name = "buttonCancel";
      this.buttonCancel.Size = new System.Drawing.Size(56, 24);
      this.buttonCancel.TabIndex = 2;
      this.buttonCancel.Text = "&Cancel";
      this.buttonCancel.UseVisualStyleBackColor = true;
      this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
      // 
      // checkBoxLogVerbose
      // 
      this.checkBoxLogVerbose.Location = new System.Drawing.Point(8, 8);
      this.checkBoxLogVerbose.Name = "checkBoxLogVerbose";
      this.checkBoxLogVerbose.Size = new System.Drawing.Size(128, 20);
      this.checkBoxLogVerbose.TabIndex = 0;
      this.checkBoxLogVerbose.Text = "Extended logging";
      this.toolTips.SetToolTip(this.checkBoxLogVerbose, "Enable more detailed logging of plugin operations");
      this.checkBoxLogVerbose.UseVisualStyleBackColor = true;
      // 
      // buttonExtChannels
      // 
      this.buttonExtChannels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonExtChannels.Location = new System.Drawing.Point(280, 184);
      this.buttonExtChannels.Name = "buttonExtChannels";
      this.buttonExtChannels.Size = new System.Drawing.Size(80, 24);
      this.buttonExtChannels.TabIndex = 3;
      this.buttonExtChannels.Text = "STB Setup";
      this.toolTips.SetToolTip(this.buttonExtChannels, "Setup external channel changing");
      this.buttonExtChannels.UseVisualStyleBackColor = true;
      this.buttonExtChannels.Click += new System.EventHandler(this.buttonExtChannels_Click);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.tabPageOptions);
      this.tabControl.Controls.Add(this.tabPageIR);
      this.tabControl.Controls.Add(this.tabPageMacros);
      this.tabControl.Location = new System.Drawing.Point(8, 8);
      this.tabControl.Multiline = true;
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(376, 240);
      this.tabControl.TabIndex = 0;
      // 
      // tabPageOptions
      // 
      this.tabPageOptions.Controls.Add(this.buttonHelp);
      this.tabPageOptions.Controls.Add(this.checkBoxLogVerbose);
      this.tabPageOptions.Controls.Add(this.buttonChangeServer);
      this.tabPageOptions.Controls.Add(this.buttonExtChannels);
      this.tabPageOptions.Location = new System.Drawing.Point(4, 22);
      this.tabPageOptions.Name = "tabPageOptions";
      this.tabPageOptions.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageOptions.Size = new System.Drawing.Size(368, 214);
      this.tabPageOptions.TabIndex = 0;
      this.tabPageOptions.Text = "Plugin Setup";
      this.tabPageOptions.UseVisualStyleBackColor = true;
      // 
      // tabPageIR
      // 
      this.tabPageIR.Controls.Add(this.listBoxIR);
      this.tabPageIR.Controls.Add(this.buttonNewIR);
      this.tabPageIR.Controls.Add(this.buttonEditIR);
      this.tabPageIR.Controls.Add(this.buttonDeleteIR);
      this.tabPageIR.Location = new System.Drawing.Point(4, 22);
      this.tabPageIR.Name = "tabPageIR";
      this.tabPageIR.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageIR.Size = new System.Drawing.Size(368, 214);
      this.tabPageIR.TabIndex = 1;
      this.tabPageIR.Text = "IR Commands";
      this.tabPageIR.UseVisualStyleBackColor = true;
      // 
      // tabPageMacros
      // 
      this.tabPageMacros.Controls.Add(this.buttonTestMacro);
      this.tabPageMacros.Controls.Add(this.buttonDeleteMacro);
      this.tabPageMacros.Controls.Add(this.listBoxMacro);
      this.tabPageMacros.Controls.Add(this.buttonEditMacro);
      this.tabPageMacros.Controls.Add(this.buttonNewMacro);
      this.tabPageMacros.Location = new System.Drawing.Point(4, 22);
      this.tabPageMacros.Name = "tabPageMacros";
      this.tabPageMacros.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageMacros.Size = new System.Drawing.Size(368, 214);
      this.tabPageMacros.TabIndex = 2;
      this.tabPageMacros.Text = "Macros";
      this.tabPageMacros.UseVisualStyleBackColor = true;
      // 
      // buttonChangeServer
      // 
      this.buttonChangeServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonChangeServer.Location = new System.Drawing.Point(8, 184);
      this.buttonChangeServer.Name = "buttonChangeServer";
      this.buttonChangeServer.Size = new System.Drawing.Size(96, 24);
      this.buttonChangeServer.TabIndex = 1;
      this.buttonChangeServer.Text = "Change Server";
      this.toolTips.SetToolTip(this.buttonChangeServer, "Change the IR Server host");
      this.buttonChangeServer.UseVisualStyleBackColor = true;
      this.buttonChangeServer.Click += new System.EventHandler(this.buttonChangeServer_Click);
      // 
      // buttonHelp
      // 
      this.buttonHelp.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.buttonHelp.Location = new System.Drawing.Point(168, 184);
      this.buttonHelp.Name = "buttonHelp";
      this.buttonHelp.Size = new System.Drawing.Size(56, 24);
      this.buttonHelp.TabIndex = 2;
      this.buttonHelp.Text = "Help";
      this.toolTips.SetToolTip(this.buttonHelp, "Click here for help");
      this.buttonHelp.UseVisualStyleBackColor = true;
      this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
      // 
      // SetupForm
      // 
      this.AcceptButton = this.buttonOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.buttonCancel;
      this.ClientSize = new System.Drawing.Size(392, 289);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.buttonCancel);
      this.Controls.Add(this.buttonOK);
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(400, 323);
      this.Name = "SetupForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "MediaPortal TV2 Blaster Plugin";
      this.Load += new System.EventHandler(this.SetupForm_Load);
      this.tabControl.ResumeLayout(false);
      this.tabPageOptions.ResumeLayout(false);
      this.tabPageIR.ResumeLayout(false);
      this.tabPageMacros.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button buttonOK;
    private System.Windows.Forms.Button buttonNewMacro;
    private System.Windows.Forms.Button buttonEditIR;
    private System.Windows.Forms.Button buttonDeleteIR;
    private System.Windows.Forms.Button buttonNewIR;
    private System.Windows.Forms.ListBox listBoxIR;
    private System.Windows.Forms.Button buttonTestMacro;
    private System.Windows.Forms.ListBox listBoxMacro;
    private System.Windows.Forms.Button buttonDeleteMacro;
    private System.Windows.Forms.Button buttonEditMacro;
    private System.Windows.Forms.Button buttonCancel;
    private System.Windows.Forms.Button buttonExtChannels;
    private System.Windows.Forms.CheckBox checkBoxLogVerbose;
    private System.Windows.Forms.ToolTip toolTips;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage tabPageIR;
    private System.Windows.Forms.TabPage tabPageMacros;
    private System.Windows.Forms.Button buttonChangeServer;
    private System.Windows.Forms.TabPage tabPageOptions;
    private System.Windows.Forms.Button buttonHelp;
  }
}