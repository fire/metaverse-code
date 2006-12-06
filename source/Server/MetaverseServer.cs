// Copyright Hugh Perkins 2004,2006
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURVector3E. See the GNU General Public License for
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
//! \brief This module is the metaverseserver module which is responsible for managing the server-side parts of OSMP.
//!
//! This module is the metaverseserver module which is responsible for managing the server-side parts of OSMP.
//! It connects to DBInterface in order to read and write to a database
//! ScriptingEngines can connect to it in order to read and write to the world
//! A server console/gui can also connect to this to obtain informatoin on current connections and so on
//!
//! The server has a copy of the world
//! It interfaces with a collision and physics dll
//! Scripting engines link with the metaverseserver via XML sockets IPC.
//! The server handles communicatiosn with the clients, and with the database.
//! we might possibly put a WAn compression module in between the server and the Internet/WAN connection

using System;
using System.Collections;
using System.Xml;
using System.Net;

using OSMP;

public class Connection
{
    
}

public class MetaverseServer
{
	const int iTicksPerFrame = 17;         //!< we assume the server is running at around 75fps, and if the server hasnothing better to do, it'll sleep this number of milliseconds
	int LastTickCount = 0;   //!< tickcount of last frame
	
	float fSayDistance = 10.0;            //!< How far Says travel
	
    TcpListener clientlistener;    
    ArrayList ClientConnections = new ArrayList();
    
    Config config;

	mvWorldStorage World;                    //!< The World and container for all objects in it (except the hardcoded land)
	
	ArrayList DirtyCache = new ArrayList();            //!< set of all objects which have been changed but not yet written to db (mostly for objectmove stuff)
	int iDirtyCacheWriteDelaySeconds = 10;   //!< Interval between writing objects that have moved to db
	int iLastDirtyCacheWriteTickCount = 0;   //!< Last dirty cache write tickcount (careful, tickcount is in milliseconds)
	
	//! Returns true or false according to whether rConnection is a local client or not. Used for privilege assignment to local scripting engines
    /*
	bool IsLocalClient( Connection connection )
	{
	}
    */
    
    public void WriteToDB( Entity dirtyentity )
    {
    }
	
	//! Sends out updates to clients for all objects that have changed in the world
	void ManageDirtyCache()   // could maybe use a separate thread for this???
	{
	    int iArrayNum;
	    int iReference;
	    if( TimeKeeper.GetTickCount() - iLastDirtyCacheWriteTickCount > 1000 * iDirtyCacheWriteDelaySeconds )
	    {
            for( int i = 0 ; i < DirtyCache.Count; i++ )
            {
                Entity dirtyentity = DirtyCache[i];
                WriteToDB( dirtyentity );
            }
            DirtyCache.Clear();
            iLastDirtyCacheWriteTickCount = TimeKeeper.GetTickCount();
	    }
	}
	
	//! Sends Message to all connected metaverse clients
	void BroadcastToAllClients( string message )
	{
	    // MetaverseServerConnectionManager.Broadcast( Message );
	}
	
	//! Sends message to all local connected clients; this will be primarily scripting engines
	
	//! Sends message to all local connected clients; this will be primarily scripting engines
	//! These messages will tend to be higher security than the general internet client messages
	//! and / or higher bandwidth.  For example, information about scripts is generally
	//! only sent to local clients
	void BroadcastToLocalClients( string message )
	{
	}
	
	//! Sends Message to all non-local connected client; ie users out on the Internet
	void BroadcastToInternetClients( string message )
	{
	}
		
    public void CheckForNewClientConnections()
    {
        if( clientlistener.Pending() )
        {
            TcpClient newclient = clientlistener.AcceptTcpClient();
            ClientConnections.Add( newclient );
            Test.Debug( "Got new connection: " + newclient.Client.RemoteEndPoint.ToString() );
        }
    }
    
    public void CheckForClientMessages()
    {
        for( int i = 0; i < ClientConnections.Count; i++ )
        {
            TcpClient thisclient = (TcpClient)ClientConnections[i];
            NetworkStream networkstream = thisclient.GetStream();
            if( networkstream.DataAvailable )
            {
                
            }
        }
    }

	//! Runs the metaverseserver.  Processes incoming messages, animates world, processes physics ...
	void MainLoop()
	{
	    while( true )
	    {
            CheckForNewClientConnections();        
            CheckForClientMessages();
            ManageDirtyCache();    // objects that have moved and not been written to db        
	    }
	}
	
    bool bRunningWithDB = true;
    bool bSpawnNewWindows = false;
    
	//! metaverseserver entry point.  Processes commandline arguments; starts dbinterface and serverfileagent components; handles initialization
	int Go(string[]args)
	{	
	    Console.WriteLine( "Metaverse Server\n" );
	
	    config = Config.GetInstance();
        
        Arguments arguments = new Arguments( args );
        
        if( arguments.Named["nodb"] != null )
        {
		    Console.WriteLine( "Option --nodb selected: we will not be using the database for this session" );
		    Console.WriteLine( "WARNING: objects will not be persistent beyond this session");
		    bRunningWithDB = false;            
        }
        
	    Test.Debug("Initing sockets");
        
	    Console.WriteLine( "Creating Metaverse Client listener on port " + iPortMetaverseServer.ToString() );
	    MetaverseServerConnectionManager.Init( INADDR_ANY, iPortMetaverseServer );
	
	    Console.WriteLine( "Initialization complete" );
	
	    MainLoop();
	
	    return 0;
	}
}	

public class EntryPoint
{
    public static void Main( string[] args )
    {
        return new MetaverseServer().Go( args );
    }
}
