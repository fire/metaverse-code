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
using System.Threading;
using System.Text;

namespace OSMP
{
    public class Server
    {
        NetworkModel net;
        public void Go()
        {
            net = NetworkModel.GetInstance();
            net.ListenAsServer( 3456 );
            // net.ListenAsServer( 3457 );
            while( true )
            {
                net.Tick();
                Thread.Sleep( 50 );
            }
        }
    }
    
    public class Client
    {
        NetworkModel net;
        public void Go()
        {
            net = NetworkModel.GetInstance();
            net.ConnectAsClient( "127.0.0.1", 3456 );
            
            while( !net.ConnectionToServer.sharedsecretexchange.Validated )
            {
                net.Tick();
                Thread.Sleep(50);
            }
            
            int i = 0;
            while( true )
            {
                net.Tick();
                //net.ConnectionToServer.Send( 'P', Encoding.ASCII.GetBytes( "sample data " + i.ToString() ) );
                Thread.Sleep( 300 );
                i++;
            }
        }
    }
    
    public class Entrypoint
    {
        public static void Main( string[] args )
        {
            try
            {
                bool IsClient = false;
                if( args.GetUpperBound(0) + 1 > 0 && args[0] == "client" )
                {
                    IsClient = true;
                }
                if( IsClient )
                {
                    new Client().Go();
                }
                else
                {
                    new Server().Go();
                }
            }
            catch( Exception e )
            {
                Console.WriteLine( e );
            }
        }
    }
}
