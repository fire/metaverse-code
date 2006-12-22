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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace OSMP
{
    public class MetaverseServer
    {
        static MetaverseServer instance = new MetaverseServer();
        public static MetaverseServer GetInstance() { return instance; }

        MetaverseServer() // protected constructor, enforce singleton
        {
        }

        // Access these via server singleton:

        public NetworkLevel2Controller network;
        public RpcController rpc;
        public NetReplicationController netreplicationcontroller;
        public WorldModel World;                    //!< The World and container for all objects in it (except the hardcoded land)

        public WorldReplication worldreplication = new WorldReplication();

        public int ServerPort;

        public bool Running = false;

        //public const int iTicksPerFrame = 17;         //!< we assume the server is running at around 75fps, and if the server hasnothing better to do, it'll sleep this number of milliseconds
        //public int LastTickCount = 0;   //!< tickcount of last frame

        //public double SayDistance = 10.0;            //!< How far Says travel

        //public List<object> ClientConnections = new List<object>();

        //TimeKeeper timekeeper = new TimeKeeper();

        public Config config;

        //ArrayList DirtyCache = new ArrayList();            //!< set of all objects which have been changed but not yet written to db (mostly for objectmove stuff)
        //int iDirtyCacheWriteDelaySeconds = 10;   //!< Interval between writing objects that have moved to db
        //int iLastDirtyCacheWriteTickCount = 0;   //!< Last dirty cache write tickcount (careful, tickcount is in milliseconds)

        //! Returns true or false according to whether rConnection is a local client or not. Used for privilege assignment to local scripting engines
        /*
        bool IsLocalClient( Connection connection )
        {
        }
        */

        //! Sends out updates to clients for all objects that have changed in the world
        //void ManageDirtyCache()   // could maybe use a separate thread for this???
        //{
          //  int iArrayNum;
            //int iReference;
            //if ( timekeeper.GetTickCount() - iLastDirtyCacheWriteTickCount > 1000 * iDirtyCacheWriteDelaySeconds)
            //{
              //  for (int i = 0; i < DirtyCache.Count; i++)
//                {
  //                  Entity dirtyentity = DirtyCache[i];
    //            }
      //          DirtyCache.Clear();
        //        iLastDirtyCacheWriteTickCount = TimeKeeper.GetTickCount();
          //  }
        //}

        //! Sends Message to all connected metaverse clients
        void BroadcastToAllClients(string message)
        {
            // MetaverseServerConnectionManager.Broadcast( Message );
        }

        //! Sends message to all local connected clients; this will be primarily scripting engines

        //! Sends message to all local connected clients; this will be primarily scripting engines
        //! These messages will tend to be higher security than the general internet client messages
        //! and / or higher bandwidth.  For example, information about scripts is generally
        //! only sent to local clients
        void BroadcastToLocalClients(string message)
        {
        }

        //! Sends Message to all non-local connected client; ie users out on the Internet
        void BroadcastToInternetClients(string message)
        {
        }

        public void Tick()
        {
            network.Tick();
            netreplicationcontroller.Tick();
            //ManageDirtyCache();    // objects that have moved and not been written to db        
        }

        //! metaverseserver entry point.  Processes commandline arguments; starts dbinterface and serverfileagent components; handles initialization
        public void Init( string[] args )
        {
            Arguments arguments = new Arguments(args);

            config = Config.GetInstance();

            Test.Info("*** Server starting ***");

            Running = true;

            network = new NetworkLevel2Controller();
            Test.Debug("Creating Metaverse Client listener on port " + config.ServerPort);
            ServerPort = config.ServerPort;

            if (arguments.Named.Contains("serverport"))
            {
                ServerPort = Convert.ToInt32(arguments.Named["serverport"]);
            }
            network.ListenAsServer(ServerPort);

            network.NewConnection += new Level2NewConnectionHandler(network_NewConnection);
            network.Disconnection += new Level2DisconnectionHandler(network_Disconnection);

            rpc = new RpcController(network);
            netreplicationcontroller = new NetReplicationController( rpc );
            World = new WorldModel(netreplicationcontroller);

            PluginsLoader.GetInstance().LoadServerPlugins();

            Test.Info("*** Server initialization complete ***");
        }

        void network_Disconnection(NetworkLevel2Connection net2con, ConnectionInfo connectioninfo)
        {
            Console.WriteLine("Server: client connected: " + net2con.connectioninfo);
        }

        void network_NewConnection(NetworkLevel2Connection net2con, ConnectionInfo connectioninfo)
        {
            Console.WriteLine("Server: client disconnected: " + net2con.connectioninfo);
        }
    }
}
