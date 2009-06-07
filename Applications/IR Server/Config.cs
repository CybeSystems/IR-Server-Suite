using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using InputService.Plugin;
using IrssUtils;
using SourceGrid;
using SourceGrid.Cells;
using SourceGrid.Cells.Controllers;
using Button = SourceGrid.Cells.Button;
using CheckBox = SourceGrid.Cells.CheckBox;
using ColumnHeader = SourceGrid.Cells.ColumnHeader;

namespace IRServer
{
  internal partial class Config : Form
  {
    #region Constants

    private const int ColConfigure = 4;
    private const int ColIcon = 0;
    private const int ColName = 1;
    private const int ColReceive = 2;
    private const int ColTransmit = 3;

    #endregion Constants

    #region Variables

    private readonly PluginBase[] _transceivers;

    private bool _abstractRemoteMode;
    private string _hostComputer = String.Empty;
    private IRServerMode _mode = IRServerMode.ServerMode;
    private string _processPriority = String.Empty;

    #endregion Variables

    #region Properties

    public bool AbstractRemoteMode
    {
      get { return _abstractRemoteMode; }
      set { _abstractRemoteMode = value; }
    }

    public IRServerMode Mode
    {
      get { return _mode; }
      set { _mode = value; }
    }

    public string HostComputer
    {
      get { return _hostComputer; }
      set { _hostComputer = value; }
    }

    public string ProcessPriority
    {
      get { return _processPriority; }
      set { _processPriority = value; }
    }

    public string[] PluginReceive
    {
      get
      {
        List<string> receivers = new List<string>();

        CheckBox checkBox;
        for (int row = 1; row < gridPlugins.RowsCount; row++)
        {
          checkBox = gridPlugins[row, ColReceive] as CheckBox;
          if (checkBox != null && checkBox.Checked == true)
            receivers.Add(gridPlugins[row, ColName].DisplayText);
        }

        if (receivers.Count == 0)
          return null;
        else
          return receivers.ToArray();
      }
      set
      {
        CheckBox checkBox;
        for (int row = 1; row < gridPlugins.RowsCount; row++)
        {
          checkBox = gridPlugins[row, ColReceive] as CheckBox;
          if (checkBox == null)
            continue;

          if (value == null)
            checkBox.Checked = false;
          else if (Array.IndexOf(value, gridPlugins[row, ColName].DisplayText) != -1)
            checkBox.Checked = true;
          else
            checkBox.Checked = false;
        }
      }
    }

    public string PluginTransmit
    {
      get
      {
        CheckBox checkBox;
        for (int row = 1; row < gridPlugins.RowsCount; row++)
        {
          checkBox = gridPlugins[row, ColTransmit] as CheckBox;
          if (checkBox != null && checkBox.Checked == true)
            return gridPlugins[row, ColName].DisplayText;
        }

        return String.Empty;
      }
      set
      {
        if (String.IsNullOrEmpty(value))
          return;

        CheckBox checkBox;
        for (int row = 1; row < gridPlugins.RowsCount; row++)
        {
          checkBox = gridPlugins[row, ColTransmit] as CheckBox;
          if (checkBox == null)
            continue;

          if (gridPlugins[row, ColName].DisplayText.Equals(value, StringComparison.OrdinalIgnoreCase))
            checkBox.Checked = true;
          else
            checkBox.Checked = false;
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

      // Add transceivers to list ...
      _transceivers = Program.AvailablePlugins();
      if (_transceivers == null || _transceivers.Length == 0)
        MessageBox.Show(this, "No IR Server Plugins found!", "IR Server Configuration", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
      else
        CreateGrid();

      try
      {
        checkBoxRunAtBoot.Checked = SystemRegistry.GetAutoRun("IR Server");
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex);

        checkBoxRunAtBoot.Checked = false;
      }
    }

    #endregion Constructor

    #region Controls

    private void buttonOK_Click(object sender, EventArgs e)
    {
      try
      {
        if (checkBoxRunAtBoot.Checked)
          SystemRegistry.SetAutoRun("IR Server", Application.ExecutablePath);
        else
          SystemRegistry.RemoveAutoRun("IR Server");
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex);
      }

      DialogResult = DialogResult.OK;
      Close();
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void buttonClickEvent_Executed(object sender, EventArgs e)
    {
      CellContext context = (CellContext)sender;
      Button cell = (Button)context.Cell;

      try
      {
        IConfigure plugin = cell.Row.Tag as IConfigure;
        if (plugin != null)
          plugin.Configure(this);
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex);

        MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void TransmitChanged(object sender, EventArgs e)
    {
      CellContext context = (CellContext)sender;
      CheckBox cell = (CheckBox)context.Cell;

      if (cell.Checked != true)
        return;

      PluginBase plugin = cell.Row.Tag as PluginBase;

      for (int row = 1; row < gridPlugins.RowsCount; row++)
      {
        CheckBox checkBox = gridPlugins[row, ColTransmit] as CheckBox;

        if (checkBox != null && checkBox.Checked == true &&
            !gridPlugins[row, ColName].DisplayText.Equals(plugin.Name, StringComparison.OrdinalIgnoreCase))
          checkBox.Checked = false;
      }
    }

    private void PluginDoubleClick(object sender, EventArgs e)
    {
      CellContext context = (CellContext)sender;
      Cell cell = (Cell)context.Cell;

      CheckBox checkBoxReceive = gridPlugins[cell.Row.Index, ColReceive] as CheckBox;
      if (checkBoxReceive != null)
        checkBoxReceive.Checked = true;

      CheckBox checkBoxTransmit = gridPlugins[cell.Row.Index, ColTransmit] as CheckBox;
      if (checkBoxTransmit != null)
        checkBoxTransmit.Checked = true;
    }

    private void toolStripButtonDetect_Click(object sender, EventArgs e)
    {
      // TODO: Place on a seperate thread?
      Detect();
    }

    private void toolStripButtonAdvancedSettings_Click(object sender, EventArgs e)
    {
      Advanced();
    }

    private void toolStripButtonHelp_Click(object sender, EventArgs e)
    {
      ShowHelp();
    }

    #endregion Controls

    private void CreateGrid()
    {
      IrssLog.Info("Creating configuration grid ...");

      try
      {
        int row = 0;

        gridPlugins.Rows.Clear();
        gridPlugins.Columns.SetCount(5);

        // Setup Column Headers
        gridPlugins.Rows.Insert(row);

        ColumnHeader header = new ColumnHeader(" ");
        header.AutomaticSortEnabled = false;
        gridPlugins[row, ColIcon] = header;

        gridPlugins[row, ColName] = new ColumnHeader("Name");
        gridPlugins[row, ColReceive] = new ColumnHeader("Receive");
        gridPlugins[row, ColTransmit] = new ColumnHeader("Transmit");
        gridPlugins[row, ColConfigure] = new ColumnHeader("Configure");
        gridPlugins.FixedRows = 1;

        foreach (PluginBase transceiver in _transceivers)
        {
          gridPlugins.Rows.Insert(++row);
          gridPlugins.Rows[row].Tag = transceiver;

          // Icon Cell
          if (transceiver.DeviceIcon != null)
          {
            Image iconCell = new Image(transceiver.DeviceIcon);
            iconCell.Editor.EnableEdit = false;

            gridPlugins[row, ColIcon] = iconCell;
          }
          else
          {
            gridPlugins[row, ColIcon] = new Cell();
          }

          // Name Cell
          Cell nameCell = new Cell(transceiver.Name);

          CustomEvents nameCellController = new CustomEvents();
          nameCellController.DoubleClick += PluginDoubleClick;
          nameCell.AddController(nameCellController);

          nameCell.AddController(new ToolTipText());
          nameCell.ToolTipText = String.Format("{0}\nVersion: {1}\nAuthor: {2}\n{3}", transceiver.Name,
                                               transceiver.Version, transceiver.Author, transceiver.Description);

          gridPlugins[row, ColName] = nameCell;

          // Receive Cell
          if (transceiver is IRemoteReceiver || transceiver is IMouseReceiver || transceiver is IKeyboardReceiver)
          {
            gridPlugins[row, ColReceive] = new CheckBox();
          }
          else
          {
            gridPlugins[row, ColReceive] = new Cell();
          }

          // Transmit Cell
          if (transceiver is ITransmitIR)
          {
            CheckBox checkbox = new CheckBox();

            CustomEvents checkboxcontroller = new CustomEvents();
            checkboxcontroller.ValueChanged += TransmitChanged;
            checkbox.Controller.AddController(checkboxcontroller);

            gridPlugins[row, ColTransmit] = checkbox;
          }
          else
          {
            gridPlugins[row, ColTransmit] = new Cell();
          }

          // Configure Cell
          if (transceiver is IConfigure)
          {
            Button button = new Button("Configure");

            SourceGrid.Cells.Controllers.Button buttonClickEvent = new SourceGrid.Cells.Controllers.Button();
            buttonClickEvent.Executed += buttonClickEvent_Executed;
            button.Controller.AddController(buttonClickEvent);

            gridPlugins[row, ColConfigure] = button;
          }
          else
          {
            gridPlugins[row, ColConfigure] = new Cell();
          }
        }

        gridPlugins.Columns[ColIcon].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
        gridPlugins.Columns[ColName].AutoSizeMode = SourceGrid.AutoSizeMode.Default;
        gridPlugins.Columns[ColReceive].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
        gridPlugins.Columns[ColTransmit].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
        gridPlugins.Columns[ColConfigure].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
        gridPlugins.AutoStretchColumnsToFitWidth = true;
        gridPlugins.AutoSizeCells();
      }
      catch (Exception ex)
      {
        IrssLog.Error(ex);
        MessageBox.Show(this, ex.ToString(), "Error setting up plugin grid", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void Detect()
    {
      IrssLog.Info("Attempting to detect Input Plugins ...");

      CheckBox checkBox;
      for (int row = 1; row < gridPlugins.RowsCount; row++)
      {
        try
        {
          PluginBase plugin = gridPlugins.Rows[row].Tag as PluginBase;

          IrssLog.Info("Checking: {0}", plugin.Name);
          bool detected = plugin.Detect();

          if (detected)
            IrssLog.Info("Found: {0}", plugin.Name);

          // Receive
          checkBox = gridPlugins[row, ColReceive] as CheckBox;
          if (checkBox != null)
            checkBox.Checked = detected;

          // Transmit
          checkBox = gridPlugins[row, ColTransmit] as CheckBox;
          if (checkBox != null)
            checkBox.Checked = detected;
        }
        catch (Exception ex)
        {
          IrssLog.Error(ex);
        }
      }
    }

    private void Advanced()
    {
      IrssLog.Info("Entering advanced configuration ...");

      Advanced advanced = new Advanced();

      advanced.AbstractRemoteMode = _abstractRemoteMode;
      advanced.Mode = _mode;
      advanced.HostComputer = _hostComputer;
      advanced.ProcessPriority = _processPriority;

      if (advanced.ShowDialog(this) == DialogResult.OK)
      {
        _abstractRemoteMode = advanced.AbstractRemoteMode;
        _mode = advanced.Mode;
        _hostComputer = advanced.HostComputer;
        _processPriority = advanced.ProcessPriority;
      }
    }

    private void ShowHelp()
    {
      try
      {
        string file = Path.Combine(SystemRegistry.GetInstallFolder(), "IR Server Suite.chm");
        Help.ShowHelp(this, file, HelpNavigator.Topic, "Input Service\\index.html");
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, ex.Message, "Failed to load help", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }
}