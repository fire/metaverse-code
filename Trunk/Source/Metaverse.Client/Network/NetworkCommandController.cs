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
using System.Text;
using System.Xml.Serialization;

namespace OSMP
{
    class NetworkCommandController
    {
        INetworkController network;    
        ArrayList types = new ArrayList();
        
        static NetworkCommandController instance = new NetworkCommandController();
        public static NetworkCommandController GetInstance(){ return instance; }
        
        class CommandHandlerInfo
        {
            public Type CommandType;
            public CommandHandler Handler;
            public CommandHandlerInfo( Type CommandType, CommandHandler Handler )
            {
                this.CommandType = CommandType; this.Handler = Handler;
            }
        }
        ArrayList handlers = new ArrayList();
        
        XmlSerializer xmlserializer = null;
        
        public NetworkCommandController( bool IsServer )
        {
            if( IsServer )
            {
                network = NetworkControllerFactory.GetServerNetworkController();
            }
            else
            {
                network = NetworkControllerFactory.GetClientNetworkController();
            }
            network.ReceivedPacket += new ReceivedPacketHandler( ReceivedPacket ); 
        }
        
        public void RegisterCommand( Type type, CommandHandler handler )
        {
            types.Add( type );
            handlers.Add( new CommandHandlerInfo( type, handler ) );
            xmlserializer = new XmlSerializer( typeof( Command ), (Type[])types.ToArray( typeof( Type ) ) );
        }
        
        // use to send to server, if this is client
        public void SendCommand( Command command )
        {
            Byte[] data = CommandToBytes( command );
            Console.WriteLine("sending " + Encoding.UTF8.GetString( data, 0, data.Length ) );
            network.Send( data );
        }
        
        // destinationconnectionobject is actually an IPEndPoint
        public void SendCommand( object destinationconnectionobject, Command command )
        {
            Byte[] data = CommandToBytes( command );
            network.Send( destinationconnectionobject, data );
        }
        
        Byte[] CommandToBytes( Command command )
        {
            //XmlSerializer xmlserializer = new XmlSerializer( command.GetType() );
            StringWriter stringwriter = new StringWriter();
            xmlserializer.Serialize( stringwriter, command );
            string xmlstring = stringwriter.ToString();
            return Encoding.UTF8.GetBytes( xmlstring );
        }
        void ReceivedPacket( object connection, ReceivedPacketArgs e )
        {
            string packetstring = Encoding.UTF8.GetString( e.Data, 0, e.Data.Length );
            Console.WriteLine("packetstring: " + packetstring );
            Command thiscommand = (Command)xmlserializer.Deserialize( new StringReader( packetstring ) );
            ProcessIncomingCommand( connection, thiscommand );
        }
        void ProcessIncomingCommand( object endpoint, Command command )
        {
            Console.WriteLine("received: " + command.GetType().ToString() );
            for( int i = 0; i < handlers.Count; i++ )
            {
                CommandHandlerInfo commandhandlerinfo = (CommandHandlerInfo)handlers[i];
                if( commandhandlerinfo.CommandType == command.GetType() )
                {
                    commandhandlerinfo.Handler( endpoint, new CommandHandlerArgs( command ) );
                }
            }
        }    
    }
}    
