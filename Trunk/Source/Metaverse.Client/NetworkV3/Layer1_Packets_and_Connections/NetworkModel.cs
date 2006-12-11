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
using System.Text; // for Encoding
    
namespace OSMP
{
    public interface INetPacketHandler
    {
        void ReceivedPacket( object connection, byte[] packetdata, int nextposition );
    }
    
    // This is responsible for handling:
    // - packet reference number (DONE)
    // - generate, confirm shared key; spoof rejection (DONE)
    // - duplicate detection (DONE)
    // - request and process packet re-send
    public class NetworkModel
    {
        public event NewConnectionHandler NewConnection;
        public event DisconnectionHandler Disconnection;
        public event ReceivedPacketHandler ReceivedPacket; 

        public INetworkImplementation networkimplementation;
        // MyRand rand = new MyRand( (int)System.DateTime.Now.Ticks );  // initialize seed to ticks
        bool isserver;
        
        Hashtable connections = new Hashtable(); // used by server
        NetworkModelConnection connectiontoserver; // used by client
                
        //static NetworkModel instance = new NetworkModel();
        //public static NetworkModel GetInstance(){ return instance; }

        public NetworkModel()
        {
            this.networkimplementation = NetworkImplementationFactory.CreateNewInstance();
            networkimplementation.ReceivedPacket += new ReceivedPacketHandler( ReceivedPacketHandler );
            //networkimplementation.NewConnection += new NewConnectionHandler( NewConnectionHandler );
            networkimplementation.Disconnection += new DisconnectionHandler( DisconnectionHandler );
        }
        
        public NetworkModelConnection ConnectionToServer{
            get{
                return connectiontoserver;
            }
        }

        public void ConnectAsClient( string ipaddress, int port )
        {
            connections = null;
            connectiontoserver = new NetworkModelConnection( this, null, false );
            networkimplementation.ConnectAsClient( ipaddress, port );
        }
        
        public void ListenAsServer( int port )
        {
            isserver = true;
            connectiontoserver = null;
            connections = new Hashtable();
            networkimplementation.ListenAsServer( port );
        }
        
        public void ReceivedPacketHandler( object source, ReceivedPacketArgs e )
        {
            object connection = e.Connection;
            
            if( isserver && !connections.Contains( connection ) )
            {
                connections.Add( connection, new NetworkModelConnection( this, connection, isserver ) );
            }
            if( isserver && connections.Contains( connection ) )
            {
                ( connections[ connection ] as NetworkModelConnection ).ReceivedPacketHandler( this, e );
            }
            else
            {
                connectiontoserver.ReceivedPacketHandler( this, e );
            }
        }
        
        public void Tick()
        {
            networkimplementation.Tick();
            if( isserver )
            {
                foreach( DictionaryEntry dictionaryentry in connections )
                {
                    NetworkModelConnection networkmodelconnection = dictionaryentry.Value as NetworkModelConnection;
                    networkmodelconnection.Tick();
                }
            }
            else
            {
                connectiontoserver.Tick();
            }
        }
        
        public void DisconnectionHandler( object source, DisconnectionArgs e )
        {
            if( isserver && connections.Contains( e.Connection ) )
            {
                ( connections[ e.Connection ] as NetworkModelConnection )._Disconnect();
                connections.Remove( e.Connection );
            }
            if( Disconnection != null )
            {
                Disconnection( this, e );
            }
        }        
    }
}
