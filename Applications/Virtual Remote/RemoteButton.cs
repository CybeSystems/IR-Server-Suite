using System;
using System.Windows.Forms;

namespace VirtualRemote
{

  public class RemoteButton
  {

    #region Variables

    string _name;
    string _code;
    Keys _shortcut;
    int _top;
    int _left;
    int _width;
    int _height;

    #endregion Variables

    #region Properties

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }
    public string Code
    {
      get { return _code; }
      set { _code = value; }
    }
    public Keys Shortcut
    {
      get { return _shortcut; }
      set { _shortcut = value; }
    }
    public int Top
    {
      get { return _top; }
      set { _top = value; }
    }
    public int Left
    {
      get { return _left; }
      set { _left = value; }
    }
    public int Width
    {
      get { return _width; }
      set { _width = value; }
    }
    public int Height
    {
      get { return _height; }
      set { _height = value; }
    }

    #endregion Properties

    #region Constructor

    public RemoteButton()
    {
      _name     = "New Button";
      _code     = String.Empty;
      _shortcut = Keys.None;
      _top      = 0;
      _left     = 0;
      _width    = 10;
      _height   = 10;
    }

    #endregion Constructor

  }

}