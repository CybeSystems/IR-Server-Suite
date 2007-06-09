using System;
using System.Collections.Generic;
using System.Text;

namespace NamedPipes
{

  // TODO: Switch to typed messages

  public class PipeMessage
  {

    #region Members

    string _fromPipe;
    string _fromServer;
    string _name;
    byte[] _data;

    #endregion Members

    #region Properties

    public string FromPipe
    {
      get { return _fromPipe; }
      set { _fromPipe = value; }
    }
    public string FromServer
    {
      get { return _fromServer; }
      set { _fromServer = value; }
    }
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }
    public byte[] Data
    {
      get { return _data; }
      set { _data = value; }
    }

    #endregion Properties

    #region Constructors

    public PipeMessage(string fromPipe, string fromServer, string name, byte[] data)
    {
      _fromPipe = fromPipe;
      _fromServer = fromServer;
      _name = name;
      _data = data;
    }

    #endregion Constructors

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();

      stringBuilder.Append(_fromPipe);
      stringBuilder.Append(',');

      stringBuilder.Append(_fromServer);
      stringBuilder.Append(',');

      stringBuilder.Append(_name);
      stringBuilder.Append(',');

      if (_data == null)
      {
        stringBuilder.Append("null");
      }
      else
      {
        foreach (byte dataByte in _data)
          stringBuilder.Append(dataByte.ToString("X2"));
      }
      
      return stringBuilder.ToString();
    }

    public static PipeMessage FromString(string from)
    {
      try
      {
        if (String.IsNullOrEmpty(from))
          return null;

        string[] stringItems = from.Split(new char[] { ',' }, StringSplitOptions.None);

        if (stringItems.Length != 4)
          return null;

        byte[] data = null;

        if (!String.IsNullOrEmpty(stringItems[3]) && stringItems[3] != "null")
        {
          data = new byte[stringItems[3].Length / 2];
          for (int index = 0; index < stringItems[3].Length; index += 2)
            data[index / 2] = byte.Parse(stringItems[3].Substring(index, 2), System.Globalization.NumberStyles.HexNumber);
        }
        
        return new PipeMessage(stringItems[0], stringItems[1], stringItems[2], data);
      }
      catch (Exception)
      {
        return null;
      }
    }

  }

}