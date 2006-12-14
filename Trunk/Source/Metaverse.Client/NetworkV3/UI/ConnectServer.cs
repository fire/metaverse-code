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
using System.Windows.Forms;
using ajma.Utils;

namespace OSMP
{
    public class ConnectServer
    {
        static ConnectServer instance = new ConnectServer();
        public static ConnectServer GetInstance() { return instance; }

        ConnectServer()
        {
            ContextMenuController.GetInstance().RegisterPersistentContextMenu(new string[] { "Network", "&Connect to server..." }, new ContextMenuHandler(ConnectToServerDialog));
        }

        void ConnectToServerDialog(object source, ContextMenuArgs e)
        {
            string ipaddress = ajma.Utils.InputBox.Show("Please enter server ip address:", "Connect to server");
            if (ipaddress == "") { return; }
            string port = ajma.Utils.InputBox.Show("Please enter server port number:", "Connect to server");
            if (port == "") { return; }
            int portnumber = 0;
            try
            {
                portnumber = Convert.ToInt32(port);
            }
            catch
            {
                return;
            }
            MetaverseClient.GetInstance().ConnectToServer(ipaddress, portnumber);
        }
    }
}
