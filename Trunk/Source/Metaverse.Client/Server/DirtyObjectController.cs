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
using System.Collections.Generic;

namespace OSMP
{
    // runs on server
    public class DirtyObjectController
    {
        Dictionary<object,DirtyObjectQueueSingleClient> remoteclientdirtyqueues = new Dictionary<object,DirtyObjectQueueSingleClient>(); // queue by connection
        
        //static RemoteClientController instance = new RemoteClientController();
        //public static RemoteClientController GetInstance(){ return instance; }

        INetworkImplementation network;
        RpcController rpc;

        public DirtyObjectController( INetworkImplementation network, RpcController rpc )
        {
            this.rpc = rpc;
            this.network = network;
            //network = NetworkControllerFactory.GetInstance();
            network.NewConnection += new NewConnectionHandler(NewConnection);
            network.Disconnection += new DisconnectionHandler( Disconnection );
        }
        public void MarkDirty( Entity targetobject )
        {
            foreach (DirtyObjectQueueSingleClient remoteclientdirtyqueue in remoteclientdirtyqueues.Values)
            {
                //RemoteClient remoteclient = (RemoteClient)remoteclientobject;
                remoteclientdirtyqueue.MarkDirty(targetobject);
            }
        }
        public void NewConnection( object connection, NewConnectionArgs e )
        {
            remoteclientdirtyqueues.Add(connection, new DirtyObjectQueueSingleClient(connection));
        }
        public void Disconnection( object connection, DisconnectionArgs e )
        {
            remoteclientdirtyqueues.Remove(connection);
        }
        public void Tick()
        {
            foreach (DirtyObjectQueueSingleClient remoteclient in remoteclientdirtyqueues.Values)
            {
                //RemoteClient remoteclient = (RemoteClient)remoteclientobject;
                remoteclient.Tick();
            }
        }
    }
}
