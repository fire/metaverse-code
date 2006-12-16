// Copyright Hugh Perkins 2006
// hughperkins@gmail.com http://manageddreams.com
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License version 2 as published by the
// Free Software Foundation;
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
//  more details.
//
// You should have received a copy of the GNU General Public License along
// with this program in the file licence.txt; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-
// 1307 USA
// You can find the licence also on the web at:
// http://www.opensource.org/licenses/gpl-license.php
//

using System;
using System.Collections.Generic;
using System.Text;

namespace OSMP
{
    public class ServerInfo
    {
        static ServerInfo instance = new ServerInfo();
        public static ServerInfo GetInstance() { return instance; }

        ServerInfo()
        {
            if (MetaverseServer.GetInstance().Running)
            {
                ContextMenuController.GetInstance().RegisterPersistentContextMenu(new string[] { "Network", "&Server Info..." }, new ContextMenuHandler(ServerInfoDialog));
            }
        }

        void ServerInfoDialog(object source, ContextMenuArgs e)
        {
            DialogHelpers.ShowInfoMessage( null, "Server listening on port: " + MetaverseServer.GetInstance().ServerPort );
        }
    }
}
