﻿#region Copyright (C) 2005-2015 Team MediaPortal

// Copyright (C) 2005-2015 Team MediaPortal
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

namespace IRNet
{
    /// <summary>
    /// Represent an IRSS Remote event call generated by a receiver device on the IRSS server.
    /// </summary>
    public class RemoteEventArgs : EventArgs
    {

        /// <summary>
        /// Default Sender Value
        /// </summary>
        public const string IrServer = "IR Server";

        /// <summary>
        /// Sender of this event. It can be either "IR Server" or an input plugin name.
        /// </summary>
        public string Sender;

        /// <summary>
        /// key identifier (representing mainly a key-press or actions)
        /// </summary>
        public string Key;

        /// <summary>
        /// Complementary data tied to the event.
        /// </summary>
        public string Data;

    }
}
