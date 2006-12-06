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

namespace OSMP
{
    // runs on server
    public class RemoteClientController
    {
        INetworkController network;
        
        Hashtable remoteclients = new Hashtable();
        
        static RemoteClientController instance = new RemoteClientController();
        public static RemoteClientController GetInstance(){ return instance; }
        
        public RemoteClientController()
        {
            network = NetworkControllerFactory.GetInstance();
            network += new NewConnectionHandler( NewConnection );
            network += new DisconnectionHandler( Disconnection );
        }
        public void MarkDirty( object targetobject, int bitmask )
        {
            foreach( object remoteclientobject in remoteclients )
            {
                RemoteClient remoteclient = (RemoteClient)remoteclientobject;
                remoteclient.MarkDirty( targetobject, bitmask );
            }
        }
        public void NewConnection( object connection, NewConnectionArgs e )
        {
            remoteclients.Add( connection, new RemoteClient( connection ) );
        }
        public void Disconnection( object connection, DisconnectionArgs e )
        {
            remoteclients.Remove( connection );
        }
        public void Tick()
        {
            foreach( object remoteclientobject in remoteclients )
            {
                RemoteClient remoteclient = (RemoteClient)remoteclientobject;
                remoteclient.Tick();
            }
        }
    }
}
