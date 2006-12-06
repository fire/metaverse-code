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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

// The goal of this module/class is to simulate a slow network connection
// parameters:
// - bandwidthbitspersecond: max bandwidth, bits per seconds
// - packetlosspercent: packet loss, in percent
//
// listens on 3456, forwards to 127.0.0.1:3457
// packets received from 127.0.0.1:3457 forwarded to whoever sent to 3456
//

public class MyRand
{
	Random rand;
	
	public MyRand( int seed )
	{
		rand = new Random(seed);
	}
	public double GetRandomFloat( int min, int max )
	{
		return rand.NextDouble() * ( max - min ) + min;
	}
	public double GetRandomFloat( double min, double max )
	{
		return rand.NextDouble() * ( max - min ) + min;
	}
	public int GetRandomInt( int min, int max )
	{
		return rand.Next( min, max + 1 );
	}
}

public class SlowNet
{
    public int bandwidthbitspersecond = 56000;
    public double packetlosspercent = 50.0;
    
    public MyRand rand = new MyRand( 0 );
    int listenport = 3456;
    int forwardport = 3457;
    string forwardipaddress = "127.0.0.1";
    
    class SingleProxyConnection
    {
        public IPEndPoint listenpeer; // port of client who sent packets to listenport
        public UdpClient forwardsocket; // socket for outgoing 3457 port
        public SingleProxyConnection( IPEndPoint listenpeer, UdpClient forwardsocket )
        {
            this.listenpeer = listenpeer;
            this.forwardsocket = forwardsocket;
        }
    }
    
    Hashtable reverseproxies = new Hashtable();

    UdpClient incominglistener;
    
    void Listen()
    {
        while( true )
        {
            IPEndPoint listenpeerendpoint = new IPEndPoint( IPAddress.Any, 0 );
            byte[] incomingpacket = incominglistener.Receive( ref listenpeerendpoint );
            ReverseProxy reverseproxy = null;
            if( reverseproxies.Contains( listenpeerendpoint ) && ( reverseproxies[ listenpeerendpoint ] as ReverseProxy ).IsDead )
            {
                reverseproxies.Remove( listenpeerendpoint );
            }
            if( !reverseproxies.Contains( listenpeerendpoint ) )
            {
                UdpClient forwardsocket = new UdpClient( forwardipaddress, forwardport );
                reverseproxy = new ReverseProxy( this, forwardsocket, incominglistener, listenpeerendpoint );
                reverseproxies.Add( listenpeerendpoint, reverseproxy );
                Thread reverseproxythread = new Thread( new ThreadStart( reverseproxy.Go ) );
                reverseproxythread.IsBackground = true;
                reverseproxythread.Start();
            }
            reverseproxy = reverseproxies[ listenpeerendpoint ] as ReverseProxy;
            if( rand.GetRandomFloat( 0, 1 ) > packetlosspercent / 100 )
            {
                Console.WriteLine("forwarding packet from " + listenpeerendpoint.ToString() + " to " + forwardport.ToString() + " " + Encoding.ASCII.GetString( incomingpacket, 0, incomingpacket.Length ) );
                reverseproxy.forwardingsocket.Send( incomingpacket, incomingpacket.Length );
            }
            else
            {
                Console.WriteLine("Dropping packet from " + listenpeerendpoint.ToString() );
            }
        }
    }
    
    class ReverseProxy
    {
        public SlowNet myparent;
        public UdpClient forwardingsocket;
        public UdpClient listensocket;
        public IPEndPoint listenpeerendpoint;
        public bool IsDead;
            
        public ReverseProxy( SlowNet myparent, UdpClient forwardingsocket, UdpClient listensocket, IPEndPoint listenpeerendpoint )
        {
            this.myparent = myparent;
            this.forwardingsocket = forwardingsocket;
            this.listensocket = listensocket;
            this.listenpeerendpoint = listenpeerendpoint;
        }
        
        public void Go()
        {
            try{
                while( true )
                {
                    IPEndPoint incomingendpoint = new IPEndPoint( IPAddress.Any, 0 );
                    byte[] incomingpacket = forwardingsocket.Receive( ref incomingendpoint );
                    if( myparent.rand.GetRandomFloat( 0, 1 ) > myparent.packetlosspercent / 100 )
                    {
                        Console.WriteLine( "reverseproxy packet from " + incomingendpoint.ToString() + " to " + listenpeerendpoint.ToString() + " " + Encoding.ASCII.GetString( incomingpacket, 0, incomingpacket.Length ) );
                        listensocket.Send( incomingpacket, incomingpacket.Length, listenpeerendpoint );
                    }
                    else
                    {
                        Console.WriteLine( "Dropping reverseproxy packet from " + incomingendpoint.ToString() );
                    }
                }
            }
            catch( Exception e )
            {
                Console.WriteLine( e );
                IsDead = true;
            }
        }
    }
    
    public void Go()
    {
        incominglistener = new UdpClient( listenport );
        while( true )
        {
            try
            {
                Listen();
            }
            catch( Exception e )
            {
                Console.WriteLine( "listener exception: " + e.ToString() );
            }
        }
    }
}

public class Entrypoint
{
    public static void Main()
    {
        try{
            new SlowNet().Go();
        }
        catch( Exception e )
        {
            Console.WriteLine( e );
        }
    }
}
