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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OSMP
{
    // handles udp underlying layer, including:
    // - receiving packets and generating a ReceivedPacket event
    // - generating concept of connection/disconnection
    // - generating keepalives
    //
    // things like packet reference number, duplicate detection and so on are handled by higher-level classes,
    // because they are not udp specific
    //
    // keepalive, connection setup/tear down
    // ===================================
    //
    // Udp packets have no intrinsic connection, but obviously servers have a concept of client connections, so we need to generate this concept somehow
    // Whenever we've received a packet from an alleged host in the last ConnectionTimeOutSeconds seconds, we consider that host to be connected
    // If no packets in ConnectionTimeOutSeconds seconds, that host is considered disconnected
    // Note that this can be manipulated by spoofing, so this might need to be tweaked in the future.
    // 
    public class NetworkImplementationUdp : INetworkImplementation
    {
        public event NetworkChangeStateHandler NetworkChangeState;
        
        public event NewConnectionHandler NewConnection;
        public event DisconnectionHandler Disconnection;
        public event ReceivedPacketHandler ReceivedPacket; 
        
        Hashtable connections = new Hashtable(); // used by server
        ConnectionInfo connectiontoserver = new ConnectionInfo( null ); // used by client
        
        public int ConnectionTimeOutSeconds = 10;
        public int KeepaliveIntervalSeconds = 2;
        
        class ConnectionInfo
        {
            public IPEndPoint EndPoint;
            public DateTime LastTimestamp;
            public DateTime LastOutgoingPacketTime;
                
            public ConnectionInfo( IPEndPoint EndPoint )
            {
                this.EndPoint = EndPoint;
                LastTimestamp = DateTime.Now;
                LastOutgoingPacketTime = DateTime.Now;
            }
            public void UpdateLastOutgoingPacketTime()
            {
                LastOutgoingPacketTime = DateTime.Now;
            }
        }
        
        bool isserver;
        int serverport = 3456;
        string serveraddress = "127.0.0.1";
        
        UdpClient udpclient;
        Thread receivethread;
        
        Queue ReceivedPackets = new Queue();  // be sure to lock this whilst accessing, because items are enqueued from a separate thread (class Receive)
        
        public NetworkImplementationUdp()
        {
        }
        
        public void ConnectAsClient( string ipaddress, int port )
        {
            Shutdown();
            serveraddress = ipaddress;
            serverport = port;
            isserver = false;
            Start();
            if( NetworkChangeState != null )
            {
                NetworkChangeState( this, new NetworkChangeStateArgs( isserver ) );
            }
        }
        
        public void ListenAsServer( int port )
        {
            Shutdown();            
            serverport = port;
            isserver = true;
            Start();
            if( NetworkChangeState != null )
            {
                NetworkChangeState( this, new NetworkChangeStateArgs( isserver ) );
            }
        }
        
        public int ServerPort
        {
            get{ return serverport;}
        }
        public string ServerAddress
        {
            get{ return serveraddress;}
        }
        public bool IsServer
        {
            get{ return isserver;}
        }
        
        class Receive
        {
            UdpClient udpclient;
            Queue receivedpackets;
            
            public Receive( UdpClient udpclient, Queue receivedpackets )
            {
                this.udpclient = udpclient;
                this.receivedpackets = receivedpackets;
            }
            public void Go()
            {
                while( true )
                {
                    IPEndPoint endpoint = new IPEndPoint( IPAddress.Any, 0 );
                    Byte[]receiveddata = udpclient.Receive( ref endpoint );
                    Console.WriteLine("received: " + Encoding.UTF8.GetString( receiveddata,0, receiveddata.Length ) );
                    lock( receivedpackets )
                    {
                        receivedpackets.Enqueue( new object[]{ endpoint, receiveddata } );
                    }
                }
            }
        }
        
        public void Start()
        {
            if( isserver )
            {
                udpclient = new UdpClient( ServerPort );
            }
            else
            {
                udpclient = new UdpClient( serveraddress, ServerPort );
            }
            
            Receive receive = new Receive( udpclient, ReceivedPackets );
            receivethread = new Thread( new ThreadStart( receive.Go ) );
            receivethread.IsBackground = true;
            receivethread.Start();
        }
        public void Shutdown()
        {
            if( receivethread != null )
            {
                receivethread.Abort();
                receivethread.Join();
                receivethread = null;
            }
        }
        
        void SendKeepalive()
        {
            Send( new byte[]{} );
        }
        
        // for server
        public void Send( object connection, byte[] data, int length )
        {
            ( ( ConnectionInfo )connections[ connection ] ).UpdateLastOutgoingPacketTime();
            udpclient.Send( data, data.Length, (IPEndPoint)connection );
        }

        public void Send(object connection, byte[] data )
        {
            Send(connection, data, data.Length);
        }

        // for client
        public void Send( byte[] data, int length )
        {
            connectiontoserver.UpdateLastOutgoingPacketTime();
            udpclient.Send( data , length );
        }

        public void Send(byte[] data)
        {
            Send(data, data.Length);
        }

        // process any received packets        
        public void Tick()
        {
            ProcessReceivedPackets();
            CheckDisconnections();
            SendKeepalives();
        }
        
        void SendKeepalives()
        {
            if( isserver )
            {
                foreach( DictionaryEntry entry in connections )
                {
                    object connection = entry.Key;
                    ConnectionInfo connectioninfo = (ConnectionInfo)entry.Value;
                    if( (int)DateTime.Now.Subtract( connectioninfo.LastOutgoingPacketTime ).TotalMilliseconds > ( KeepaliveIntervalSeconds * 1000 ) )
                    {
                        Console.WriteLine("sending keepalive to " + connection.ToString() );
                        Send( connection, new byte[]{} );
                        connectioninfo.UpdateLastOutgoingPacketTime();
                    }
                }
            }
            else
            {
                ConnectionInfo connectioninfo = connectiontoserver;
                if( (int)DateTime.Now.Subtract( connectioninfo.LastOutgoingPacketTime ).TotalMilliseconds > ( KeepaliveIntervalSeconds * 1000 ) )
                {
                    Console.WriteLine("sending keepalive to server" );
                    Send( new byte[]{} );
                    connectioninfo.UpdateLastOutgoingPacketTime();
                }
            }
        }
        
        // sends received packets up stack
        void ProcessReceivedPackets()
        {
            lock( ReceivedPackets )
            {
                while( ReceivedPackets.Count > 0 )
                {
                    object[]queueitem = (object[])ReceivedPackets.Dequeue();
                    object endpoint = queueitem[0];
                    Byte[]packetdata = (Byte[])queueitem[1];
                    
                    if( !connections.Contains( endpoint ) )
                    {
                        Console.WriteLine( "connection: " + endpoint.ToString() );
                        connections.Add( endpoint, new ConnectionInfo( (IPEndPoint)endpoint ) );
                    }
                    ConnectionInfo connectioninfo = connections[endpoint] as ConnectionInfo;
                    connectioninfo.LastTimestamp = DateTime.Now;
                                        
                    if( ReceivedPacket != null && packetdata.Length > 0 )
                    {
                        ReceivedPacket( endpoint, new ReceivedPacketArgs( endpoint, packetdata, 0 ) );
                    }
                }
            }
        }
        
        void CheckDisconnections()
        {
            ArrayList disconnected = new ArrayList();
            foreach( DictionaryEntry entry in connections )
            {
                object connection = entry.Key;
                ConnectionInfo connectioninfo = (ConnectionInfo)entry.Value;
                if( (int)DateTime.Now.Subtract( connectioninfo.LastTimestamp ).TotalMilliseconds > ( ConnectionTimeOutSeconds * 1000 ) )
                {
                    disconnected.Add( connection );
                }
            }
            for( int i = 0; i < disconnected.Count; i++ )
            {
                object connection = disconnected[i];
                ConnectionInfo connectioninfo = (ConnectionInfo)connections[ connection ];
                if( Disconnection != null )
                {
                    Disconnection( connectioninfo, new DisconnectionArgs( connection ) );
                }
                Console.WriteLine( "disconnection: " + connectioninfo.EndPoint.ToString() );
                connections.Remove( connection );
            }
        }
    }
}
