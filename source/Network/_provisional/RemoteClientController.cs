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
using System.Collections;
using System.Windows.Forms;

namespace OSMP
{
    // RemoteClientController is used by a server to manage connecting and connected remote clients
    // Other objects can register with RemoteClientController to receive incoming commands,
    // and can use this object to send outgoing commands, either broadcast, or to specific clients
    public class RemoteClientController
    {
        public event NewRemoteClientHandler NewRemoteClient;
        public event RemoteClientAuthenticateHandler RemoteClientAuthenticate;
        public event RemoteClientDisconnectHandler RemoteClientDisconnect;
        
        ArrayList registeredcommands = new ArrayList();
        
        class ServerCommandInfo
        {
            public Type CommandType;
            CommandHandler CommandHandler;
            public ServerCommandInfo( Type commandtype, CommandHandler commandhandler )
            {
                this.CommandType = commandtype;
                this.CommandHandler = commandhandler;
            }
            public override string ToString(){ return "ServerCommandInfo " + CommandType.ToString() );
        }
        
        static instance RemoteClientController = new RemoteClientController();
        public static RemoteClientController GetInstance(){ return instance; }
        
        // starts remoteclientcontroller
        public void Activate()
        {
        }
        
        // call this function frequently while remoteclientcontroller is activated
        public void ProcessMessages()
        {
        }
        
        // shuts remoteclientcontroller down
        public void Shutdown()
        {
        }
        
        // other objects can register commands that they would like to process
        public void RegisterCommand( Type commandtype, CommandHandler commandhandler )
        {
                registeredcommands.Add( new CommandInfo( commandtype, commandhandler ) );
        }
        
        // objects can use this to broadcast a command to all remote clients
        public void BroadcastCommand( Command command )
        {
        }
        
        // objects can use this to send a command to a specific remote client
        public void SendCommandToSingleRemoteClient( object RemoteClient, Command command )
        {
        }
    }
}
