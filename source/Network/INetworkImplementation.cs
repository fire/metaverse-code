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

namespace OSMP
{
    public class NetworkChangeStateArgs : EventArgs
    {
        public bool IsServer;
        public NetworkChangeStateArgs( bool isserver )
        {
            IsServer = isserver;
        }
    }
    
    public class NewConnectionArgs : EventArgs
    {
        public object Connection;
        public NewConnectionArgs( object connection )
        {
            this.Connection = connection;
        }
    }
    
    public class DisconnectionArgs : EventArgs
    {
        public object Connection;
        public DisconnectionArgs( object connection )
        {
            this.Connection = connection;
        }
    }
    
    public class ReceivedPacketArgs : EventArgs
    {
        public object Connection;
        public byte[] Data;
        public int DataStartIndex;
        public ReceivedPacketArgs( object Connection, byte[] data, int datastartindex )
        {
            this.Connection = Connection;
            this.Data = data;
            this.DataStartIndex = datastartindex;
        }
    }
    
    public delegate void NetworkChangeStateHandler( object source, NetworkChangeStateArgs e );
    public delegate void NewConnectionHandler( object source, NewConnectionArgs e );
    public delegate void DisconnectionHandler( object source, DisconnectionArgs e );
    public delegate void ReceivedPacketHandler( object source, ReceivedPacketArgs e );

    // network implementation is responsible for sending pre-formatted/serialized data across the underlying network    
    // it could be udp or tcp, or somethign else, so we use a factory to select the specific type of INetworkImplementation that we want
    public interface INetworkImplementation
    {
        event NetworkChangeStateHandler NetworkChangeState;
        
        event NewConnectionHandler NewConnection;
        event DisconnectionHandler Disconnection;
        event ReceivedPacketHandler ReceivedPacket; 
    
        bool IsServer{get;}
        int ServerPort{get;}
        string ServerAddress{get;}

        void ConnectAsClient( string ipaddress, int port );
        void ListenAsServer( int port );
                
        void Start();
        void Shutdown();
        
        void Tick();
        void Send( byte[] data );
        void Send( object connection, byte[] data );
    }
}
