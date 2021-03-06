#region Copyright (C) 2005-2009 Team MediaPortal

// Copyright (C) 2005-2009 Team MediaPortal
// http://www.team-mediaportal.com
// 
// This Program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.
// 
// This Program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with GNU Make; see the file COPYING.  If not, write to
// the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
// http://www.gnu.org/copyleft/gpl.html

#endregion

using System.Windows.Forms;

namespace WebRemote
{
  internal struct RemoteButton
  {
    #region Variables

    private string _code;
    private int _height;
    private int _left;
    private string _name;
    private Keys _shortcut;
    private int _top;
    private int _width;

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
  }
}