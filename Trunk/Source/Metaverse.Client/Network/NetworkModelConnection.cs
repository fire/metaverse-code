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
    public class PacketHandlerArgs
    {
        public int PacketKey;
        public char PacketCode;
        public short PacketRef;
        public byte[] Data;
        public int NextPosition;
        public PacketHandlerArgs( int PacketKey, char PacketCode, short PacketRef, byte[] packet, int nextposition )
        {
            this.PacketKey = PacketKey;
            this.PacketCode = PacketCode;
            this.PacketRef = PacketRef;
            this.Data = packet;
            this.NextPosition = nextposition;
        }
    }
    
    public delegate void PacketHandler( object source, PacketHandlerArgs e );
    
    // This is going to handle a single network connection
    public class NetworkModelConnection
    {
        INetworkImplementation networkimplementation;
        NetworkModel parent;
        bool isserver;
        
        object connection;
        
        DateTime lasttimestamp;
        
        NetPacketReferenceController packetreferencecontroller;
        public NetSharedSecretExchange sharedsecretexchange;  // responsible for exchanging and validating shared secret
        
        Hashtable unsafepackethandlers = new Hashtable();     // packets which dont have correct sharedsecret, or sharedsecret not yet validated
        Hashtable packethandlers = new Hashtable();     
        
        public NetworkModelConnection( NetworkModel parent, object connection, bool isserver )
        {
            networkimplementation = NetworkImplementationFactory.GetInstance();
            
            this.parent = parent;
            this.connection = connection;
            this.isserver = isserver;
            
            lasttimestamp = DateTime.Now;
            
            packetreferencecontroller = new NetPacketReferenceController( this, isserver );
            sharedsecretexchange = new NetSharedSecretExchange( this, isserver );
            //sharedsecretexchange.Tick();
        }
        
        public void Tick()
        {
            sharedsecretexchange.Tick();
            packetreferencecontroller.Tick();
        }
        
        public void ReceivedPacketHandler( object source, ReceivedPacketArgs e )
        {
            object connection = e.Connection;
            byte[] packet = e.Data;
            int nextposition = e.DataStartIndex;
            
            lasttimestamp = DateTime.Now;
            
            if( packet.Length >= 4 + 2 + 1 )
            {
                int packetkey = (int)BinaryPacker.ReadValueFromBuffer( packet, ref nextposition, typeof( int ) );
                short packetref = (short)BinaryPacker.ReadValueFromBuffer( packet, ref nextposition, typeof( short ) );
                char packetcode = (char)BinaryPacker.ReadValueFromBuffer( packet, ref nextposition, typeof( char ) );
                
                Console.WriteLine( "Packet key: " + packetkey.ToString() + " packetref: " + packetref.ToString() );

                if( unsafepackethandlers.Contains( packetcode ) )
                {
                    if( packetreferencecontroller.ValidateIncomingReference( packetref ) )
                    {
                        ((PacketHandler)unsafepackethandlers[ packetcode ])( this, new PacketHandlerArgs(
                            packetkey, packetcode, packetref, packet, nextposition ) );
                    }
                }
                else
                {
                    if( sharedsecretexchange.ValidateIncomingPacketKey( packetkey ) )
                    {
                        if( packetreferencecontroller.ValidateIncomingReference( packetref ) )
                        {
                            if( packethandlers.Contains( packetcode ) )
                            {
                                ((PacketHandler)packethandlers[ packetcode ])( this, new PacketHandlerArgs(
                                    packetkey, packetcode, packetref, packet, nextposition ) );
                            }
                            else
                            {
                                Console.WriteLine("Warning: unknown packet code " + packetcode.ToString() + " " + Encoding.ASCII.GetString( packet, 0, packet.Length ) );
                            }
                        }// else silently ignore duplicate packet
                    }
                    else
                    {
                        Console.WriteLine("WARNING: received potentially spoofed packet allegedly from " + connection.ToString() + " " + Encoding.ASCII.GetString( packet, 0, packet.Length ) );
                    }
                }
            }
        }
        
        byte[] GenerateOutgoingPacket( char packettype, byte[] data )
        {
            byte[]packet = new byte[ data.Length + 4 + 2 + 1 ];
            int nextposition = 0;
            
            short packetreference = packetreferencecontroller.NextReference;
            
            BinaryPacker.WriteValueToBuffer( packet, ref nextposition, typeof( int ), sharedsecretexchange.SharedSecretKey );
            BinaryPacker.WriteValueToBuffer( packet, ref nextposition, typeof( short ), packetreference );
            BinaryPacker.WriteValueToBuffer( packet, ref nextposition, typeof( char ), packettype );
            Buffer.BlockCopy( data, 0, packet, nextposition, data.Length );
            
            packetreferencecontroller.RegisterSentPacket( packetreference, packet );
                
            return packet;
        }
        
        public void Send( char packettype, byte[] data )
        {
            byte[]outgoingpacket = GenerateOutgoingPacket( packettype, data );
            RawSend( outgoingpacket );
        }
        
        public void SendNonAckable( char packettype, byte[] data )
        {
            byte[]outgoingpacket = new byte[ data.Length + 4 + 2 + 1 ];
            int nextposition = 0;
            
            BinaryPacker.WriteValueToBuffer( outgoingpacket, ref nextposition, typeof( int ), sharedsecretexchange.SharedSecretKey );
            BinaryPacker.WriteValueToBuffer( outgoingpacket, ref nextposition, typeof( short ), (short)0 );
            BinaryPacker.WriteValueToBuffer( outgoingpacket, ref nextposition, typeof( char ), packettype );
            Buffer.BlockCopy( data, 0, outgoingpacket, nextposition, data.Length );
            
            RawSend( outgoingpacket );
        }
        
        // primarily for use of NetPacketReferenceController, or equivalent, to resent non-acked sent packets
        public void RawSend( byte[]packet)
        {
            if( isserver )
            {
                networkimplementation.Send( connection, packet );
            }
            else
            {
                networkimplementation.Send( packet );
            }
        }
        
        public void RegisterUnsafePacketHandler( char packetcode, PacketHandler packethandler )
        {
            if( unsafepackethandlers.Contains( packetcode ) && (PacketHandler)unsafepackethandlers[ packetcode ] != packethandler )
            {
                throw new Exception( "Trying to register duplicate packetcode " + packetcode.ToString() + " by handler " + packethandler.ToString() + " conflicting handler: " + unsafepackethandlers[ packetcode ].ToString() );
            }
            if( !unsafepackethandlers.Contains( packetcode ) )
            {
                Console.WriteLine("Registering unsafe-packet handler " + packetcode.ToString() + " " + packethandler.ToString() );
                unsafepackethandlers.Add( packetcode, packethandler );
            }
        }
        
        public void RegisterPacketHandler( char packetcode, PacketHandler packethandler )
        {
            if( packethandlers.Contains( packetcode ) && (PacketHandler)packethandlers[ packetcode ] != packethandler )
            {
                throw new Exception( "Trying to register duplicate packetcode " + packetcode.ToString() + " by handler " + packethandler.ToString() + " conflicting handler: " + packethandlers[ packetcode ].ToString() );
            }
            if( !packethandlers.Contains( packetcode ) )
            {
                packethandlers.Add( packetcode, packethandler );
            }
        }
        
        public void _ConnectionValidated()  // generated by NetSharedSecretExchange once key validated
        {
            
        }
        
        public void _Disconnect()
        {
        }
        
        void OnNewConnection( NewConnectionArgs e )
        {
            // notify observers of new connection
          //  if( NewConnection != null )
            //{
               // NewConnection( this, e );
           // }
        }
    }
}