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
using System.Collections.Generic;
using System.Text;
using Meebey.SmartIrc4net;
using System.Threading;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace OSMP
{
    public class ServerRegistration
    {
        //public string[] serverlist = new string[] { "irc.gamernet.org" };
        //public int port = 6667;
        //public string channel = "#osmp";

        static ServerRegistration instance = new ServerRegistration();
        public static ServerRegistration GetInstance(){ return instance; }

        IrcClient ircclient = null;
        public string ircname;

        XmlSerializer xmlserializer;

        public class Command
        {
        }

        public class ServerInfo : Command
        {
            public byte[] IPAddress;
            public int port;
            public ServerInfo(){}
            public ServerInfo( IPAddress ipaddress, int port )
            {
                this.IPAddress = ipaddress.GetAddressBytes();
                this.port = port;
            }
        }

        ServerRegistration() // protected to enforce singleton
        {
            xmlserializer = new XmlSerializer( typeof( Command ), new Type[]{ typeof( ServerInfo ) } );

            MetaverseServer.GetInstance().Tick += new MetaverseServer.TickHandler(ServerRegistration_Tick);

            ircclient = new IrcClient();

            ircclient.SendDelay = 200;
            ircclient.ActiveChannelSyncing = true; // we use channel sync, means we can use ircclient.GetChannel() and so on

            ircclient.OnQueryMessage += new IrcEventHandler( OnQueryMessage );

            //ircclient.OnRawMessage += new IrcEventHandler( OnRawMessage );
            //ircclient.OnNames += new NamesEventHandler( OnNames );

            ircclient.OnError += new Meebey.SmartIrc4net.ErrorEventHandler( OnError );

            new InputBox( "Please enter a worldname to publish your server to gamenet irc #osmp", new InputBox.Callback( ServernameCallback ) );
        }

        void ServerRegistration_Tick()
        {
            if (ircclient != null)
            {
                ircclient.ListenOnce(false);
            }
        }

        void ServernameCallback( string servername )
        {
            if (servername == "")
            {
                return;
            }

            ircname = "srv_" + servername;

            LogFile.WriteLine( this.GetType() + " ircname will be: " + ircname + " calling STUN..." );
            STUN stun = new STUN( MetaverseServer.GetInstance().network.networkimplementation, new STUN.GotExternalAddress( GotExternalAddress ) );
        }

        IPAddress externaladdress;
        int externalport;

        void GotExternalAddress( IPAddress ipaddres, int port )
        {
            LogFile.WriteLine( "serverregistration using stun info: " + ipaddres + " " + port );
            this.externaladdress = ipaddres;
            this.externalport = port;
            Connect();
        }

        void Connect()
        {
            Config.Coordination coordinationconfig = Config.GetInstance().coordination;
            string[] serverlist = new string[] { coordinationconfig.ircserver };
            int port = coordinationconfig.ircport;
            string channel = coordinationconfig.ircchannel;

            LogFile.WriteLine( "serverregistration connecting..." );
            ircclient.Connect( serverlist, port );
            LogFile.WriteLine( "serverregistration login as " + ircname );
            ircclient.Login( ircname, ircname );
            LogFile.WriteLine( "serverregistration join channel " + channel );
            ircclient.RfcJoin( channel );
            LogFile.WriteLine( "serverregistration connected" );

        }
        //public void OnRawMessage( object sender, IrcEventArgs e )
        //{
          //  LogFile.WriteLine( "OnRawMessage Replycode " + e.Data.ReplyCode.ToString() + " Received: " + e.Data.RawMessage );
            //InformClient( "*" + e.Data.Nick + "* " + e.Data.Message );
        //}

        public void OnQueryMessage( object sender, IrcEventArgs e )
        {
            LogFile.WriteLine( "serverregistration received from " + e.Data.Nick + ": " + e.Data.Message );
            if (e.Data.Message.StartsWith( "QUERY" ) )
            {
                SendCommand( e.Data.Nick, new ServerInfo( 
                    externaladdress, externalport ) );
            }
        }

        void SendCommand( string targetnick, Command command )
        {
            StringWriter stringwriter = new StringWriter();
            xmlserializer.Serialize( stringwriter, command );
            ircclient.SendMessage(SendType.Message, targetnick, stringwriter.ToString().Replace("\n", "").Replace("\r", "") );
            stringwriter.Close();
        }

        public void OnError( object sender, Meebey.SmartIrc4net.ErrorEventArgs e )
        {
            LogFile.WriteLine( "Error: " + e.ErrorMessage );
            Thread.Sleep( 10000 );
            Connect();
        }
    }
}
