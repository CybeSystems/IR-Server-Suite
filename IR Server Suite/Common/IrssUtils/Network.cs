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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace IrssUtils
{
  /// <summary>
  /// Provides access to Win32 Network functions.
  /// </summary>
  public static class Network
  {
    #region Constants

    private const int MAX_PREFERRED_LENGTH = -1;

    private const int SV_TYPE_SERVER = 2;
    private const int SV_TYPE_WORKSTATION = 1;

    #endregion Constants

    #region Structures

    [StructLayout(LayoutKind.Sequential)]
    private struct _SERVER_INFO_100
    {
      public int sv100_platform_id;
      [MarshalAs(UnmanagedType.LPWStr)] public string sv100_name;
    }

    #endregion Structures

    #region Interop

    [DllImport("netapi32.dll", CharSet = CharSet.Auto, SetLastError = true), SuppressUnmanagedCodeSecurity]
    private static extern int NetServerEnum(
      string ServerName, // must be null
      int Level,
      ref IntPtr Buf,
      int PrefMaxLen,
      out int EntriesRead,
      out int TotalEntries,
      int ServerType,
      string Domain, // null for login domain
      out int ResumeHandle
      );

    [DllImport("netapi32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
    private static extern int NetApiBufferFree(IntPtr pBuf);

    #endregion Interop

    #region Methods

    /// <summary>
    /// Get a list of all computer names on the LAN, except for the local host.
    /// </summary>
    /// <param name="includeLocalMachine">Include this computer in the list.</param>
    /// <returns>List of LAN computer names.</returns>
    [EnvironmentPermission(SecurityAction.Demand, Read = "COMPUTERNAME")]
    public static List<string> GetComputers(bool includeLocalMachine)
    {
      try
      {
        List<string> networkComputers = new List<string>();

        IntPtr buffer = IntPtr.Zero;
        IntPtr tmpBuffer = IntPtr.Zero;
        int entriesRead = 0;
        int totalEntries = 0;
        int resHandle = 0;
        int sizeofINFO = Marshal.SizeOf(typeof (_SERVER_INFO_100));

        int ret = NetServerEnum(
          null,
          100,
          ref buffer,
          MAX_PREFERRED_LENGTH,
          out entriesRead,
          out totalEntries,
          SV_TYPE_WORKSTATION, //  | SV_TYPE_SERVER
          null,
          out resHandle);

        if (ret == 0)
        {
          for (int i = 0; i < totalEntries; i++)
          {
            tmpBuffer = new IntPtr((int) buffer + (i * sizeofINFO));
            _SERVER_INFO_100 svrInfo = (_SERVER_INFO_100) Marshal.PtrToStructure(tmpBuffer, typeof (_SERVER_INFO_100));

            if (includeLocalMachine ||
                !svrInfo.sv100_name.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
              networkComputers.Add(svrInfo.sv100_name);
          }
        }

        NetApiBufferFree(buffer);

        if (networkComputers.Count > 0)
          return networkComputers;
      }
      catch
      {
      }

      return null;
    }

    /// <summary>
    /// Translates a host name or address into an IPAddress object (IPv4).
    /// </summary>
    /// <param name="name">Host name or IP address.</param>
    /// <returns>IPAddress object.</returns>
    public static IPAddress GetIPFromName(string name)
    {
      // Automatically convert localhost to loopback address, avoiding lookup calls for systems that aren't on a network. 
      if (name.Equals("localhost", StringComparison.OrdinalIgnoreCase))
        return IPAddress.Loopback;

      IPAddress[] addresses = Dns.GetHostAddresses(name);
      foreach (IPAddress address in addresses)
        if (address.AddressFamily == AddressFamily.InterNetwork)
          return address;

      return null;
    }

    #endregion Methods
  }
}