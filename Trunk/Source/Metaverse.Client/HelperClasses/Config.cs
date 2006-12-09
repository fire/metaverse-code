// Copyright Hugh Perkins 2004,2005,2006
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

//! \file
//! \brief Used to read config from config.xml

using System;
using System.Collections;
using System.Xml;

namespace OSMP
{

    //! Database connection info for one database. Contained by mvConfig class
    public class DatabaseConnectionInfo
    {
        public string host;
        public string databasename;
        public string username;
        public string password;
        public DatabaseConnectionInfo( string host, string databasename, string username, string password )
        {
            this.host = host; this.databasename = databasename; this.username = username; this.password = password;
        }
    };
    
    //! Connection info for the authentication server. Contained by mvConfig class
    public class AuthServerConnectionInfo
    {
        public string ipaddress;
        public int port;
        public string password;
        public AuthServerConnectionInfo( string ipaddress, int port, string password )
        {
            this.ipaddress= ipaddress; this.port = port; this.password = password;
        }
    }
    
    public class Config
    {
        string sFilePath = "config.xml";
        
        public int iDebugLevel;
        public int iOSMPServerPort = 2501;
            
        public Hashtable CommandsByKeycode = new Hashtable();
        public Hashtable KeycodesByCommand = new Hashtable();
        
        XmlDocument configdoc;
        public XmlElement clientconfig;
        
        static Config instance = new Config();
        public static Config GetInstance()
        {
            return instance;
        }
        
        public Config()
        {
            RefreshConfig();
        }
        
        public void RefreshConfig()
        {
            configdoc = XmlHelper.OpenDom( sFilePath );
            Test.Debug("reading config.xml ...");
        
            XmlElement systemnode = (XmlElement)configdoc.DocumentElement.SelectSingleNode( "config");
            iDebugLevel = Convert.ToInt32( systemnode.GetAttribute("debuglevel") );
            Test.Debug("DebugLevel " + iDebugLevel.ToString() );
        
            clientconfig = (XmlElement)configdoc.DocumentElement.SelectSingleNode( "client");
            foreach( XmlElement mappingnode in clientconfig.SelectNodes("keymappings/key") )
            {
                string sCommand = mappingnode.GetAttribute("command");
                string sKeyCode = mappingnode.GetAttribute("keycode");
                if( !CommandsByKeycode.Contains( sKeyCode ) )
                {
                    CommandsByKeycode.Add( sKeyCode, new StringArrayList() );
                }
                ((StringArrayList)CommandsByKeycode[ sKeyCode ]).Add( sCommand );
                if( !KeycodesByCommand.Contains( sCommand ) )
                {
                    KeycodesByCommand.Add( sCommand, new StringArrayList() );
                }
                ((StringArrayList)KeycodesByCommand[ sCommand ]).Add( sKeyCode );
            }
        
            XmlElement servernode = (XmlElement)configdoc.DocumentElement.SelectSingleNode( "server" );
            iOSMPServerPort = Convert.ToInt32( servernode.GetAttribute("listenport"));
                    
            Test.Debug("... config.xml read");
        }
        
        public string GetFactoryTargetClassname( string sfactoryname )
        {
            try
            {
                XmlElement factorynode = (XmlElement)configdoc.SelectSingleNode("root/factories/factory[name='" + sfactoryname.ToLower() + "']" );
                return factorynode.GetAttribute("select");
            }
            catch( Exception e )
            {
                Console.WriteLine( e.ToString() );
                return "";
            }
        }

/*        
        public string GetCommandForKeycode( string sKeycode )
        {
            return (string)CommandsByKeycode[ sKeycode ];
        }
        
        public string GetKeycodeForCommand( string sCommand )
        {
            return (string)KeycodesByCommand[ sCommand ];
        }
        */
    }
}