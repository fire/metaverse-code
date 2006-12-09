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
    //
    // sequencenumber
    // ==============
    // - we are using a sharedkey to reduce spoofing, so we just use a sequential number, starting from 1, for the sequencenumber
    // - the sequencenumber for each direction is independent.  Each will start from 1, and they can each use the same numbers as each other.
    //
    // sharedkey, spoofing protection
    // ============================
    //
    // sharedkey, int32, is used to make spoofing harder.  No attempt to eliminate man-in-the-middle
    // if the sharedkey on an incoming packet doesnt match that for the connection, it is rejected
    //
    // Protocol for key exchange:
    //   client sends R packet to server.  This requests the server to send it the shared key, and contains a temporary key to authenticate the server reply
    //   server sends A packet to server, which contains the shared key, and also the temporary client key the client sent
    //   client sends K keepalive packet to the server.  Client now considers connection "open"
    //   server receives K keepalive packet and considers connection open
    //
    // what we have achieved is to avoid the client pretending to be someone else, since the client can only find out the sharedkey if it provides
    // a valid return ip address to the server
    // server spoofing is prevented in most cases because it is the client making the request, and because the server has to receive the temporary,
    // client key sent to its address by the client, in order to create a valid A packet
    // we can mitigate this later using an IM (IRC, Jabber...) authentication scheme.
    //
    // The packets are then forwarded to the appropriate packethandler for higher-level processing, such as streaming etc
    // Packet format:
    // [int32 sharedkey][short sequence][char packetcode][data .... ]
    // packetcodes can be registered by specific packethandlers. The following packetcodes are native to the class itself:
    //
    // R   -> request a shared key.  Sent by client to server to request that the server send it the shared key
    //    Packet format:   [int32 xxx][short xxx][char 'R'][temporary client key]
    //
    // A   -> reply by server to R packet.
    //    Packet format:   [int32 xxx][short xxx][char 'R'][temporary client key][shared key]
    //
    // K   -> reply by client to A packet
    //
    // ... where xxx means undefined, can be anything 
    //
    // client only accepts A packets with the correct tempclientkey
    // server replies to any R packet, generating the shared key if the connection didnt exist before
    // client marks connection verified on receipt of A packet
    // server marks connection verified on receipt of any valid non-R packet
    //
    // Acking
    // ======
    // one ack packet is sent every AckPacketIntervalSeconds, to each connection
    // the ack packet code is 'C'
    // as packets are received they are added to the ReceivedPacketsNotAcked,
    // ack packet format:
    // [int32 key][short ref][char 'C'][first packet ref][second packet ref][third packet ref] ...
    
    // note to self: this class is horribly bloated; could be good to fission for readability
    // also, ConnectionInfo is horribly bloated, maybe it should become a class in its own right, rather than just a data storage structure?
    public class NetworkModel
    {
        public event NewConnectionHandler NewConnection;
        public event DisconnectionHandler Disconnection;
        public event ReceivedPacketHandler ReceivedPacket; 

        public int RPacketIntervalSeconds = 1;
        public int AckPacketIntervalSeconds = 1;
            
        INetworkImplementation networkimplementation;
        MyRand rand = new MyRand( (int)System.DateTime.Now.Ticks );
        bool isserver;
        
        Hashtable packethandlers = new Hashtable();     
        
        Hashtable connections = new Hashtable(); // used by server
        ConnectionInfo connectiontoserver; // used by client
                
        static NetworkModel instance = new NetworkModel();
        public static NetworkModel GetInstance(){ return instance; }

        class ConnectionInfo
        {
            public object Connection;
            public DateTime LastTimestamp;
            public short NextPacketReference;
            public DateTime LastRPacket; // for client
            public DateTime LastAckPacket;
            public int TempClientKey; // used by client to store temporary key that is used to verify server's response comes from server
            public int Key; // shared secret for connection; transmitted in clear for now; doesnt prevent man in middle, but does reduce casual spoofing
            public bool KeyVerified;
                
            // received packet hashtable format:
            // ( packetref, datetime )
            public Hashtable RecentReceivedPackets = new Hashtable();
                
            // just contains list of packet references, no timestamp
            public Queue ReceivedPacketsNotAcked = new Queue();
                
            // sent packet awaiting hashtable format:
            // ( packetref, new object[]{ datetime, byte[]packet ) )
            public Hashtable SentPacketsAwaitingAck = new Hashtable();
            
            public ConnectionInfo()
            {
                NextPacketReference = 1;
                LastTimestamp = DateTime.Now;
                LastRPacket = DateTime.Now;
                LastAckpacket = DateTime.Now;
                KeyVerified = false;
                Connection = null;
            }
            public ConnectionInfo( object connection, int key, short NextPacketReference )
            {
                ConnectionInfo();
                this.Connection = connection;
                this.Key = key;
                LastTimestamp = DateTime.Now;
                this.NextPacketReference = NextPacketReference;
            }
            public short GetNextPacketReference()
            {
                short nextpacketreference = NextPacketReference;
                NextPacketReference++;
                return nextpacketreference;
            }
        }
        
        public NetworkModel()
        {
            networkimplementation = NetworkImplementationFactory.GetInstance();
            networkimplementation.ReceivedPacket += new ReceivedPacketHandler( ReceivedPacketHandler );
            //networkimplementation.NewConnection += new NewConnectionHandler( NewConnectionHandler );  // we only consider a client connected
            // once we have confirmed its shared key
            networkimplementation.Disconnection += new DisconnectionHandler( DisconnectionHandler );
        }

        public void ConnectAsClient( string ipaddress, int port )
        {
            connectiontoserver = new ConnectionInfo();
            connectiontoserver.NextPacketReference = 1;
            connectiontoserver.Connection = null;
            isserver = false;

            networkimplementation.ConnectAsClient( ipaddress, port );
            
            connectiontoserver.TempClientKey = rand.GetRandomInt( 0, int.MaxValue - 1 );
            SendRPacketToServer();
        }
        
        void SendRPacketToServer()
        {
            Console.WriteLine( "Sending R packet to server...");
            byte[] packet = new byte[4];
            int tempnextposition = 0;
            BinaryPacker.WriteValueToBuffer( packet, ref tempnextposition, typeof( int ), connectiontoserver.TempClientKey );
            Send( 'R', packet );
        }
        
        public void ListenAsServer( int port )
        {
            isserver = true;
            networkimplementation.ListenAsServer( port );
        }
        
        public void ReceivedPacketHandler( object source, ReceivedPacketArgs e )
        {
            object connection = e.Connection;
            byte[] packet = e.Data;
            int nextposition = e.DataStartIndex;
            
            if( packet.Length >= 4 + 2 + 1 )
            {
                int packetkey = (int)BinaryPacker.ReadValueFromBuffer( packet, ref nextposition, typeof( int ) );
                int packetref = (short)BinaryPacker.ReadValueFromBuffer( packet, ref nextposition, typeof( short ) );
                char packetcode = (char)BinaryPacker.ReadValueFromBuffer( packet, ref nextposition, typeof( char ) );
                
                Console.WriteLine( "Packet key: " + packetkey.ToString() + " packetref: " + packetref.ToString() );
                
                if( packetcode != 'A' && packetcode != 'R' )
                {
                    ConnectionInfo connectioninfo = null;
                    if( isserver )
                    {
                        connectioninfo = connections[connection] as ConnectionInfo;
                    }
                    else
                    {
                        connectioninfo = connectiontoserver;
                    }
                    
                    if( isserver && !connectioninfo.KeyVerified )
                    {
                        if( packetkey == connectioninfo.Key )
                        {
                            connectioninfo.KeyVerified = true;
                            Console.WriteLine("Shared key confirmed; sending NewConnection event" );
                            // notify observers of new connection
                            if( NewConnection != null )
                            {
                                NewConnection( this, new NewConnectionArgs( connection ) );
                            }
                        }
                    }
                    
                    if( !connectioninfo.RecentReceivedPackets.Contains( packetref ) )
                    {
                        connectioninfo.RecentReceivedPackets.Add( packetref, DateTime.Now );
                        connectioninfo.ReceivedPacketsNotAcked.Enqueue( packetref );
                        if( isserver && packetkey == connectioninfo.Key && connectioninfo.KeyVerified ||
                            !isserver && packetkey == connectioninfo.Key && connectioninfo.KeyVerified )
                        {
                            if( packethandlers.Contains( packetcode ) )
                            {
                                ((INetPacketHandler)packethandlers[ packetcode ]).ReceivedPacket( connection, packet, nextposition );
                            }
                        }
                        else
                        {
                            Console.WriteLine("WARNING: received potentially spoofed packet allegedly from " + connection.ToString() + " " + Encoding.ASCII.GetString( packet, 0, packet.Length ) );
                        }
                    }
                }
                else
                {
                    if( isserver )
                    {
                        if( !( connections.Contains( connection ) ) )
                        {
                            int key = GenerateSharedKey();
                            connections.Add( connection, new ConnectionInfo( connection, key, 1 ) );
                        }
                        ConnectionInfo connectioninfo = connections[ connection ] as ConnectionInfo;
                        if( packetcode == 'R' )
                        {          
                            int tempnextposition = nextposition;
                            int tempclientkey = (int)BinaryPacker.ReadValueFromBuffer( packet, ref nextposition, typeof( int ) );
    
                            byte[] newpacket = new byte[8];
                            tempnextposition = 0;
                            
                            Console.WriteLine( "R packet received, sending A packet, tempclientkey: " + tempclientkey.ToString() + " sharedkey: " + connectioninfo.Key.ToString() );
                            
                            BinaryPacker.WriteValueToBuffer( newpacket, ref tempnextposition, typeof( int ), tempclientkey );
                            BinaryPacker.WriteValueToBuffer( newpacket, ref tempnextposition, typeof( int ), connectioninfo.Key );
                            Send( connection, 'A', newpacket );
                        }
                    }
                    else
                    {
                        if( packetcode == 'A' )
                        {
                            int tempnextposition = nextposition;
                            int tempclientkey = (int)BinaryPacker.ReadValueFromBuffer( packet, ref tempnextposition, typeof( int ) );
                            Console.WriteLine( "A packet received, tempclientkey: " + tempclientkey.ToString() );
                            if( tempclientkey == connectiontoserver.TempClientKey )
                            {
                                connectiontoserver.Key = packetkey;
                                Console.WriteLine( "Connection to server confirmed, sharedkey: " + packetkey.ToString() );
                                connectiontoserver.KeyVerified = true;
                                Send( 'K', new byte[]{} );
                            }
                            else
                            {
                                Console.WriteLine("WARNING: potential spoof packet detected, allegedly from server " + connection.ToString() + " " + Encoding.ASCII.GetString( packet, 0, packet.Length ) );
                            }
                        }
                    }
                }
            }
        }
        
        int GenerateSharedKey()
        {
            return rand.GetRandomInt( 0, int.MaxValue - 1 );
        }

        byte[] GenerateOutgoingPacket( ConnectionInfo connectioninfo, char packettype, byte[] data )
        {
            int key = connectioninfo.Key;
            byte[]packet = new byte[ data.Length + 4 + 2 + 1 ];
            int nextposition = 0;
            short nextreference = connectioninfo.GetNextPacketReference();
            
            
            BinaryPacker.WriteValueToBuffer( packet, ref nextposition, typeof( int ), key );
            //BinaryPacker.WriteValueToBuffer( packet, ref nextposition, typeof( int ), 2345 );
            BinaryPacker.WriteValueToBuffer( packet, ref nextposition, typeof( short ), nextreference );
            BinaryPacker.WriteValueToBuffer( packet, ref nextposition, typeof( char ), packettype );
            Buffer.BlockCopy( data, 0, packet, nextposition, data.Length );
            
            connectioninfo.SentPacketsAwaitingAck.Add( nextreference, new object[]{ DateTime.Now, packet } );
                
            return packet;
        }
        
        public void Send( object connection, char packettype, byte[] data )
        {
            ConnectionInfo connectioninfo = connections[ connection ] as ConnectionInfo;
            
            if( !connectioninfo.KeyVerified && packettype != 'R' && packettype != 'A' )
            {
                Console.WriteLine("Error: tried to send on connection with unverified shared key" );
                return;
            }
            
            byte[]outgoingpacket = GenerateOutgoingPacket( connectioninfo, packettype, data );
            networkimplementation.Send( connection, outgoingpacket );
        }
        
        public void Send( char packettype, byte[] data )
        {
            byte[]outgoingpacket = GenerateOutgoingPacket( connectiontoserver, packettype, data );
            networkimplementation.Send( outgoingpacket );
        }
        
        void SendRPacketToServerIfNecessary()
        {
            if( !isserver )
            {
                if( !connectiontoserver.KeyVerified )
                {
                    if( (int)DateTime.Now.Subtract( connectiontoserver.LastRPacket ).TotalMilliseconds > RPacketIntervalSeconds * 1000 )
                    {
                        SendRPacketToServer();
                        connectiontoserver.LastRPacket = DateTime.Now;
                    }
                }
            }
        }
        
        void SendAckPacketsForConnection( ConnectionInfo connectioninfo )
        {
            Console.WriteLine("Checking Last ack for " + connectioninfo.ToString() + " " + ((int)DateTime.Now.Subtract( connectioninfo.LastAckPacket ).TotalMilliseconds).ToString() );
            if( (int)DateTime.Now.Subtract( connectioninfo.LastAckPacket ).TotalMilliseconds > AckPacketIntervalSeconds * 1000 )
            {
                connectioninfo.LastAckPacket = DateTime.Now;
                byte[] ackpacketdata = null;
                lock( connectioninfo.ReceivedPacketsNotAcked )
                {
                    int numpacketstoack = connectioninfo.ReceivedPacketsNotAcked.Count;
                    ackpacketdata = new byte[ numpacketstoack * 2 ];
                    int nextposition = 0;
                    for( int i = 0; i < numpacketstoack; i++ )
                    {
                        BinaryPacker.WriteValueToBuffer( ackpacketdata, ref nextposition, typeof( short ), (short)connectioninfo.ReceivedPacketsNotAcked.Dequeue() );
                    }
                }
                Console.WriteLine("Sending ack packet " + Encoding.ASCII.GetString( ackpacketdata, 0, ackpacketdata.Length ) );
                if( connectioninfo.Connection == null )
                {
                    Send( 'C', ackpacketdata );
                }
                else
                {
                    Send( connectioninfo.Connection, 'C', ackpacketdata );
                }
            }
        }
        
        void SendAckPackets()
        {
            if( isserver )
            {
                foreach( DictionaryEntry dictionaryentry in connections )
                {
                    ConnectionInfo connectioninfo = dictionaryentry.Value as ConnectionInfo;
                    SendAckPacketsForConnection( connectioninfo );
                }
            }
            else
            {
                SendAckPacketsForConnection( connectiontoserver );
            }
        }
        
        public void Tick()
        {
            networkimplementation.Tick();
            SendRPacketToServerIfNecessary();
            SendAckPackets();
        }
        
        public void RegisterPacketHandler( char packetcode, INetPacketHandler packethandler )
        {
            if( packethandlers.Contains( packetcode ) && packethandlers[ packetcode ] != packethandler )
            {
                throw new Exception( "Trying to register duplicate packetcode " + packetcode.ToString() + " by handler " + packethandler.ToString() + " conflicting handler: " + packethandlers[ packetcode ].ToString() );
            }
            if( !packethandlers.Contains( packetcode ) )
            {
                packethandlers.Add( packetcode, packethandler );
            }
        }
        
        public void DisconnectionHandler( object source, DisconnectionArgs e )
        {
            if( connections.Contains( e.Connection ) )
            {
                connections.Remove( e.Connection );
            }
            if( Disconnection != null )
            {
                Disconnection( this, e );
            }
        }        
    }
}
