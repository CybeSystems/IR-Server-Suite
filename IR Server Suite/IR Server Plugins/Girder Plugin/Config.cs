using System;
using System.IO;
using System.Windows.Forms;

namespace InputService.Plugin
{
  /// <summary>
  /// Config form for selecting and configuring a Girder plugin.
  /// </summary>
  internal partial class Config : Form
  {
    #region Properties

    /// <summary>
    /// Gets or sets the path of the Girder plugin folder.
    /// </summary>
    /// <value>The path of the Girder plugin folder.</value>
    public string PluginFolder
    {
      get { return textBoxPluginFolder.Text; }
      set
      {
        textBoxPluginFolder.Text = value;

        UpdatePluginList();
      }
    }

    /// <summary>
    /// Gets or sets the selected plugin.
    /// </summary>
    /// <value>The plugin file.</value>
    public string PluginFile
    {
      get
      {
        if (listViewPlugins.SelectedItems.Count == 0)
          return null;
        else
          return listViewPlugins.SelectedItems[0].Text;
      }
      set
      {
        foreach (ListViewItem item in listViewPlugins.Items)
        {
          if (item.Text.Equals(value, StringComparison.OrdinalIgnoreCase))
          {
            item.Selected = true;
            return;
          }
        }
      }
    }

    #endregion Properties

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="Config"/> class.
    /// </summary>
    public Config()
    {
      InitializeComponent();
    }

    #endregion Constructor

    private void buttonConfigureGirderPlugin_Click(object sender, EventArgs e)
    {
      if (listViewPlugins.SelectedIndices.Count != 1)
        return;

      try
      {
        string pluginFile = Path.Combine(textBoxPluginFolder.Text, listViewPlugins.SelectedItems[0].Text);

        GirderPluginWrapper pluginWrapper = new GirderPluginWrapper(pluginFile);

        pluginWrapper.GirOpen();

        if (!pluginWrapper.CanConfigure)
        {
          MessageBox.Show(this, "No configuration available", "Girder Plugin Configuration", MessageBoxButtons.OK,
                          MessageBoxIcon.Information);
        }
        else
        {
          pluginWrapper.GirCommandGui();

          MessageBox.Show(this, "Press OK after the Girder plugin configuration is complete",
                          "Girder Plugin Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        pluginWrapper.GirClose();

        pluginWrapper.Dispose();
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void buttonFind_Click(object sender, EventArgs e)
    {
      if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
      {
        textBoxPluginFolder.Text = folderBrowserDialog.SelectedPath;

        UpdatePluginList();
      }
    }

    private void buttonOK_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void UpdatePluginList()
    {
      listViewPlugins.Clear();

      string folder = textBoxPluginFolder.Text;

      if (String.IsNullOrEmpty(folder))
        return;

      string[] files = Directory.GetFiles(folder, "*.dll", SearchOption.TopDirectoryOnly);
      if (files.Length > 0)
        foreach (string file in files)
          listViewPlugins.Items.Add(Path.GetFileName(file));
    }
  }
}