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
using System.Windows.Forms;

namespace OSMP
{
    // ServerLiaisonController is used by a remote client to connect to the server and manage commands
    public class ServerLiaisonController
    {
        public bool IsConnected;
            
        TcpClientPublic server = null;
        NetworkStream serverstream;
        StreamWriter serverstreamwriter;
        ArrayList registeredcommands = new ArrayList();
        CommandReceiver commandreceiver;
        Thread commandreceiverthread;
        
        Queue ReceivedCommands = new Queue();
        
        static ServerLiaisonController instance = new ServerLiaisonController();
        public static ServerLiaisonController GetInstance(){ return instance; }
        
        class CommandReceiver
        {
            NetworkStream serverstream;
            public CommandReceiver( Stream stream, ArrayList commands )
            {
                this.serverstream = stream;
            }
            public void Run()
            {
                XmlSerializer mySerializer = new XmlSerializer( typeof( Command ) );
                Command command = (Command)mySerializer.Deserialize(serverstream);
                lock( ReceivedCommands )
                {
                    ReceivedCommands.Enqueue( command );
                }
            }
        }
        
        public ServerLiaisonController()
        {
            public event EventHandler ServerConnect;
            public event EventHandler ServerAuthenticate;
            public event EventHandler ServerDisconnect;

            // starts ServerLiaisonController
            public bool ConnectToServer( string serverdnsname, int port )
            {
                try{
                    lock( server )
                    {
                        lock( serverstream )
                        {
                            lock( ReceivedCommands )
                            {
                                server = new TcpClientPublic();
                                server.Connect( serverdnsname, port );
                                serverstream = server.GetStream();
                                serverstreamwriter = new StreamWriter( serverstream );
                                commandreceiver = new CommandReceiver( serverstream, ReceivedCommands );
                                commandreceiverthread = new Thread( new ThreadStart( commandreceiver.Run ) );
                                IsConnected = true;
                                return true;
                            }
                        }
                    }
                }
                catch( Exception e )
                {
                    Console.WriteLine( e );
                    return false;
                }
            }
            
            // call this function frequently while remoteclientcontroller is activated
            public void ProcessMessages()
            {
                lock( ReceivedCommands )
                {
                    while( ReceivedCommands.Count > 0 )
                    {
                        Command thiscommand = (Command)ReceivedCommands.Dequeue();
                        for( int i = 0; i < registeredcommands.Count; i++ )
                        {
                            CommandInfo thiscommandinfo = (CommandInfo)registeredcommands[i];
                            if( thiscommandinfo.CommandType == thiscommand.GetType() )
                            {
                                thiscommandinfo.CommandHandler( this, new CommandEventArgs( thiscommand, null ) );
                            }
                        }
                    }
                }
            }
            
            // shuts ServerLiaisonController down
            public void Disconnect()
            {
                lock( server )
                {
                    server.Close();
                }
                IsConnected = false;
            }
            
            // other objects can register commands that they would like to process
            public void RegisterCommand( Type commandtype, CommandHandler commandhandler )
            {
                lock( registeredcommands )
                {
                    registeredcommands.Add( new CommandInfo( commandtype, commandhandler ) );
                }
            }
            
            // objects can use this to send a command to the server
            public void SendCommandToServer( Command command )
            {
                XmlSerializer mySerializer = new XmlSerializer( command.GetType() );
                mySerializer.Serialize( serverstream, command );
            }
        }
    }
}
