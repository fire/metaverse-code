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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
//using System.Threading;

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
        public event Level1NetworkChangeStateHandler NetworkChangeState;
        
        public event Level1NewConnectionHandler NewConnection;
        public event Level1DisconnectionHandler Disconnection;
        public event Level1ReceivedPacketHandler ReceivedPacket; 
        
        Dictionary<IPEndPoint,Level1ConnectionInfo> connections = new Dictionary<IPEndPoint,Level1ConnectionInfo>(); // used by server
        Level1ConnectionInfo connectiontoserver = new Level1ConnectionInfo( null, new ConnectionInfo( null, null, 0 ) ); // used by client
        
        public int ConnectionTimeOutSeconds = 10;
        public int KeepaliveIntervalSeconds = 2;
        
        class Level1ConnectionInfo
        {
            public IPEndPoint EndPoint;
            public ConnectionInfo connectioninfo;
            public DateTime LastTimestamp;
            public DateTime LastOutgoingPacketTime;
                
            public Level1ConnectionInfo( IPEndPoint EndPoint, ConnectionInfo connectioninfo )
            {
                this.EndPoint = EndPoint;
                this.connectioninfo = connectioninfo;
                LastTimestamp = DateTime.Now;
                LastOutgoingPacketTime = DateTime.Now;
            }
            public void UpdateLastOutgoingPacketTime()
            {
                LastOutgoingPacketTime = DateTime.Now;
            }
        }

        public IPAddress GetIPAddressForConnection(object connection)
        {
            return ((IPEndPoint)connection).Address;
        }

        public int GetPortForConnection(object connection)
        {
            return ((IPEndPoint)connection).Port;
        }
        
        bool isserver;
        int serverport = 3456;
        string serveraddress = "127.0.0.1";
        
        UdpClient udpclient;
        //Thread receivethread;
        
        //Queue ReceivedPackets = new Queue();  // be sure to lock this whilst accessing, because items are enqueued from a separate thread (class Receive)
        
        public NetworkImplementationUdp()
        {
        }
        
        public void ConnectAsClient( string ipaddress, int port )
        {
            //Shutdown();
            serveraddress = ipaddress;
            serverport = port;
            isserver = false;
            Init();
            if( NetworkChangeState != null )
            {
                NetworkChangeState( this, isserver );
            }
        }
        
        public void ListenAsServer( int port )
        {
            //Shutdown();            
            serverport = port;
            isserver = true;
            Init();
            if( NetworkChangeState != null )
            {
                NetworkChangeState( this, isserver );
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
                while (true)
                {
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
                    try
                    {
                        Byte[] receiveddata = udpclient.Receive(ref endpoint);
                     //   Console.WriteLine("received: " + Encoding.UTF8.GetString(receiveddata, 0, receiveddata.Length));
                        lock (receivedpackets)
                        {
                            receivedpackets.Enqueue(new object[] { endpoint, receiveddata });
                        }
                    }
                    catch //(Exception e)
                    {
                        //Console.WriteLine(e);
                    }
                }
            }
        }
        
        void Init()
        {
            if( isserver )
            {
                udpclient = new UdpClient( ServerPort );
            }
            else
            {
                udpclient = new UdpClient( serveraddress, ServerPort );
            }

            Console.WriteLine("our port: " + ((IPEndPoint)udpclient.Client.LocalEndPoint).Port);
            
            //Receive receive = new Receive( udpclient, ReceivedPackets );
            //receivethread = new Thread( new ThreadStart( receive.Go ) );
            //receivethread.IsBackground = true;
            //receivethread.Start();
        }

        // process any received packets        
        public void Tick()
        {
            ProcessReceivedPackets();
            CheckDisconnections();
            SendKeepalives();
        }

        public void ProcessReceivedPackets()
        {
            while (udpclient.Available > 0)
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
                Byte[] receiveddata = null;
                try
                {
                    receiveddata = udpclient.Receive(ref endpoint);
                    //   Console.WriteLine("received: " + Encoding.UTF8.GetString(receiveddata, 0, receiveddata.Length));
                }
                catch //(Exception e)
                {
                  //  Console.WriteLine(e);
                }
                ProcessReceivedPacket(endpoint, receiveddata);
            }
        }

        // sends received packets up stack
        void ProcessReceivedPacket(IPEndPoint endpoint, byte[] packetdata)
        {
            if (!connections.ContainsKey(endpoint))
            {
                Console.WriteLine("connection: " + endpoint.ToString());
                connections.Add(endpoint, new Level1ConnectionInfo(endpoint, new ConnectionInfo(endpoint, endpoint.Address, endpoint.Port)));
            }
            Level1ConnectionInfo level1connectioninfo = connections[endpoint] as Level1ConnectionInfo;
            level1connectioninfo.LastTimestamp = DateTime.Now;

            if (ReceivedPacket != null && packetdata.Length > 0)
            {
                ReceivedPacket(this, level1connectioninfo.connectioninfo, packetdata, 0, packetdata.GetLength(0));
            }
        }

        void SendKeepalive()
        {
            Send( new byte[]{} );
        }
        
        // for server
        public void Send( object connection, byte[] data, int length )
        {
            if (isserver)
            {
                IPEndPoint connectionendpoint = (IPEndPoint)connection;
                connections[connectionendpoint].UpdateLastOutgoingPacketTime();
                udpclient.Send(data, data.Length, connectionendpoint );
            }
            else
            {
                Send(data, length);
            }
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

        void SendKeepalives()
        {
            if( isserver )
            {
                foreach( KeyValuePair<IPEndPoint,Level1ConnectionInfo> kvp in connections )
                {
                    object connection = kvp.Key;
                    Level1ConnectionInfo connectioninfo = kvp.Value;
                    if( (int)DateTime.Now.Subtract( connectioninfo.LastOutgoingPacketTime ).TotalMilliseconds > ( KeepaliveIntervalSeconds * 1000 ) )
                    {
                       // Console.WriteLine("sending keepalive to " + connection.ToString() );
                        Send( connection, new byte[]{} );
                        connectioninfo.UpdateLastOutgoingPacketTime();
                    }
                }
            }
            else
            {
                Level1ConnectionInfo connectioninfo = connectiontoserver;
                if( (int)DateTime.Now.Subtract( connectioninfo.LastOutgoingPacketTime ).TotalMilliseconds > ( KeepaliveIntervalSeconds * 1000 ) )
                {
                   // Console.WriteLine("sending keepalive to server" );
                    Send( new byte[]{} );
                    connectioninfo.UpdateLastOutgoingPacketTime();
                }
            }
        }
        
        void CheckDisconnections()
        {
            List<IPEndPoint> disconnected = new List<IPEndPoint>();
            foreach( KeyValuePair<IPEndPoint,Level1ConnectionInfo> entry in connections )
            {
                IPEndPoint connection = entry.Key;
                Level1ConnectionInfo connectioninfo = (Level1ConnectionInfo)entry.Value;
                if( (int)DateTime.Now.Subtract( connectioninfo.LastTimestamp ).TotalMilliseconds > ( ConnectionTimeOutSeconds * 1000 ) )
                {
                    disconnected.Add( connection );
                }
            }
            for( int i = 0; i < disconnected.Count; i++ )
            {
                IPEndPoint connection = disconnected[i];
                Level1ConnectionInfo connectioninfo = (Level1ConnectionInfo)connections[ connection ];
                if( Disconnection != null )
                {
                    Disconnection( this, connectioninfo.connectioninfo );
                }
                Console.WriteLine( "disconnection: " + connectioninfo.EndPoint.ToString() );
                connections.Remove( connection );
            }
        }
    }
}
