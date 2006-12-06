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
using System.Text;
using System.Threading;

namespace OSMP
{
    public class Server
    {
        public void Go( int port )
        {
            INetworkImplementation net = NetworkImplementationFactory.GetInstance();
            net.ListenAsServer( port );
            while( true )
            {
                net.Tick();
                Thread.Sleep(50);
            }
        }
    }
    
    public class Client
    {
        public void Go( string ipaddress, int port )
        {
            INetworkImplementation net = NetworkImplementationFactory.GetInstance();
            net.ConnectAsClient( ipaddress, port );
            net.Send( Encoding.UTF8.GetBytes( "Hi, this is a test" ) );
            while( true )
            {
                net.Tick();
                Thread.Sleep(50);
            }
        }
    }
    
    class entrypoint
    {
        static string ipaddress = "127.0.0.1";
        static int serverport = 4241;
        
        public static void Main( string[] args )
        {
            Console.WriteLine( Int32.MaxValue );
            Console.WriteLine( (int)DateTime.Now.Ticks );
            
            bool IsClient = false;
            if( args.GetUpperBound(0) + 1 > 0 && args[0] == "client" )
            {
                IsClient = true;
            }
            if( IsClient )
            {
                new Client().Go( ipaddress, serverport );
            }
            else
            {
                new Server().Go( serverport );
            }
        }
    }
}
