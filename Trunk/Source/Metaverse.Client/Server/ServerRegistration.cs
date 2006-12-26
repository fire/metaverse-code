﻿// Copyright Hugh Perkins 2006
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

        Config.Coordination coordinationconfig;

        ServerRegistration() // protected to enforce singleton
        {
            coordinationconfig = Config.GetInstance().coordination;

            MetaverseServer.GetInstance().Tick += new MetaverseServer.TickHandler(ServerRegistration_Tick);

            ircclient = new IrcClient();

            ircclient.SendDelay = 200;
            ircclient.ActiveChannelSyncing = true; // we use channel sync, means we can use ircclient.GetChannel() and so on

            ircclient.OnQueryMessage += new IrcEventHandler(OnQueryMessage);

            ircclient.OnError += new Meebey.SmartIrc4net.ErrorEventHandler( OnError );

            new InputBox( "Please enter a worldname to publish your server to " + coordinationconfig.ircserver + 
                 " irc " + coordinationconfig.ircchannel, new InputBox.Callback( ServernameCallback ) );
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
            string[] serverlist = new string[] { coordinationconfig.ircserver };
            int port = coordinationconfig.ircport;
            string channel = coordinationconfig.ircchannel;

            LogFile.WriteLine( "serverregistration connecting to " + coordinationconfig.ircserver + " ..." );
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
            LogFile.WriteLine( "serverregistration. received from " + e.Data.Nick + ": " + e.Data.Message );
            if (e.Data.Message.StartsWith( "QUERY" ))
            {
                SendCommand( e.Data.Nick, new XmlCommands.ServerInfo(
                    externaladdress, externalport ) );
            }
            else
            {
                try
                {
                    XmlCommands.Command command = XmlCommands.GetInstance().Decode( e.Data.Message );
                    if( command.GetType() == typeof( XmlCommands.PingMe ) )
                    {
                        XmlCommands.PingMe pingmecommand = command as XmlCommands.PingMe;
                        LogFile.WriteLine( "serverregistration received pingme command: " + new IPAddress( pingmecommand.MyIPAddress ) +
                            " " + pingmecommand.Myport );
                        IPEndPoint endpoint = new IPEndPoint( new IPAddress( pingmecommand.MyIPAddress ), pingmecommand.Myport );
                        MetaverseServer.GetInstance().network.networkimplementation
                            .Send( endpoint, new byte[] { 0 } );
                    }
                }
                catch (Exception ex)
                {
                    LogFile.WriteLine( ex );
                }
            }
        }
        
        void SendCommand( string targetnick, XmlCommands.Command command )
        {
            string message = XmlCommands.GetInstance().Encode( command );
            LogFile.WriteLine( message );
            ircclient.SendMessage( SendType.Message, targetnick, message );
        }

        public void OnError( object sender, Meebey.SmartIrc4net.ErrorEventArgs e )
        {
            LogFile.WriteLine( "Error: " + e.ErrorMessage );
            Thread.Sleep( 10000 );
            Connect();
        }
    }
}
