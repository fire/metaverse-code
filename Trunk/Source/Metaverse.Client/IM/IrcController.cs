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
using Meebey.SmartIrc4net;

namespace OSMP
{
    public class IrcController : IImImplementation
    {
        //public string[] serverlist = new string[] {"irc.freenode.org"};
        public string[] serverlist = new string[] { "irc.gamernet.org" };
        public int port = 6667;
        public string channel = "#osmp";
        
        public event MessageReceivedHandler MessageReceived;
            
        IrcClient ircclient;
        bool IsConnected;
        
        //static IrcController instance = new IrcController();
        //public static IrcController GetInstance(){ return instance; }
        
        public IrcController()
        {
            LogFile.WriteLine( this.GetType().ToString() + " IrcController()" );
            ircclient = new IrcClient();

            ircclient.SendDelay = 200;
            ircclient.ActiveChannelSyncing = true; // we use channel sync, means we can use ircclient.GetChannel() and so on
            
            ircclient.OnQueryMessage += new IrcEventHandler(OnQueryMessage);
            ircclient.OnQueryNotice += new IrcEventHandler(OnQueryNotice);
            ircclient.OnQueryAction += new ActionEventHandler(OnQueryAction);
            
            ircclient.OnChannelMessage += new IrcEventHandler(OnChannelMessage);
            ircclient.OnChannelNotice += new IrcEventHandler(OnChannelNotice);
            ircclient.OnChannelAction += new ActionEventHandler(OnChannelAction);
            
            ircclient.OnRawMessage += new IrcEventHandler(OnRawMessage);
            //ircclient.OnNames += new NamesEventHandler( OnNames );
    
            ircclient.OnError += new ErrorEventHandler(OnError);
        }
        
        string mylogin;
        
        public bool Login( string username, string password )
        {
            mylogin = username;
            LogFile.WriteLine( this.GetType().ToString() + " Login()" );
            InformClient( "test inform" );
            try
            {
                ircclient.Connect(serverlist, port);
                ircclient.Login(username, username);
                ircclient.RfcJoin(channel);                
                if( password != "" )
                {
                    ircclient.SendMessage(SendType.Message, "nickserv", "identify " + password );
                }
                IsConnected = true;
            }
            catch (ConnectionException e)
            {
                InformClient("IRC Error: "+e.Message + ". Irc chat will not be available in this session" );
            }
            return IsConnected;
        }
        
        public void CheckMessages()
        {
            // LogFile.WriteLine("listen once...");
            ircclient.ListenOnce( false );
        }
        
        public void SendMessage( string message )
        {
            if( IsConnected && message != "" )
            {
                string[] splitmessage = message.Split( new char[]{' '} );
                if( splitmessage[0].ToLower() == "/msg" )
                {
                    if( splitmessage.GetUpperBound(0) >= 2 )
                    {
                        string target = splitmessage[1];
                        string messagetosend = message.Substring( ( splitmessage[0] + " " + splitmessage[1] + " " ).Length );
                        ircclient.SendMessage(SendType.Message, target, messagetosend );
                        InformClient( "-> " + target + " " + messagetosend );
                    }
                }
                else if( splitmessage[0].ToLower() == "/who" )
                {
                    ircclient.WriteLine( "WHO *" );
                }
                else if( splitmessage[0].Substring( 0, 1 ) == "/" )
                {
                    InformClient( "Unknown command: " + splitmessage[0] );
                }
                else
                {
                    ircclient.SendMessage(SendType.Message, channel, message );
                    InformClient( "<" + mylogin + ">" + message );
                }
            }
        }
        
        void InformClient( string message )
        {
            LogFile.WriteLine( "informclient: " + message );
            if( MessageReceived != null )
            {
                MessageReceived( this, new MessageReceivedArgs( message ) );
            }
        }

        public void OnRawMessage(object sender, IrcEventArgs e)
        {
            LogFile.WriteLine("OnRawMessage Replycode " + e.Data.ReplyCode.ToString() + " Received: "+e.Data.RawMessage);
            //InformClient( "*" + e.Data.Nick + "* " + e.Data.Message );
        }
        
        public void OnQueryMessage(object sender, IrcEventArgs e)
        {
            InformClient( "*" + e.Data.Nick + "* " + e.Data.Message );
        }
        public void OnQueryNotice(object sender, IrcEventArgs e)
        {
            InformClient( "-" + e.Data.Nick + "- " + e.Data.Message );
        }
        public void OnQueryAction(object sender, ActionEventArgs e)
        {
            InformClient( "*" + e.Data.Nick + " " + e.Data.Message );
        }
        
        public void OnChannelMessage(object sender, IrcEventArgs e)
        {
            InformClient( "<" + e.Data.Nick + "> " + e.Data.Message );
        }
        public void OnChannelNotice(object sender, IrcEventArgs e)
        {
            InformClient( "-" + e.Data.Nick + "- " + e.Data.Message );
        }
        public void OnChannelAction(object sender, ActionEventArgs e)
        {
            InformClient( "*" + e.Data.Nick + " " + e.Data.Message );
        }
        public void OnError(object sender, ErrorEventArgs e)
        {
            LogFile.WriteLine( "Error: "+e.ErrorMessage);
            InformClient("Error: "+e.ErrorMessage);
            IsConnected = false;
        }
    }
}
