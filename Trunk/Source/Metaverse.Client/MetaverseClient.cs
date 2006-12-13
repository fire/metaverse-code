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

using System;
using System.Collections;
using OSMP;

namespace OSMP
{
    public class MetaverseClient
    {
        static MetaverseClient instance = new MetaverseClient();
        public static MetaverseClient GetInstance() { return instance; }

        MetaverseClient() // protected constructor, enforce singleton
        {
        }

        public double fHeight = -1.0;    //!< height of hardcoded land
        //public int iMyReference = 0;  //!< iReference of user's avatar
        public Avatar myavatar;
    
        public Config config;
        public IRenderer renderer;
        public PlayerMovement playermovement;
        public WorldModel worldstorage; // client's copy of worldmodel

        public NetworkLevel2Controller network;
        public RpcController rpc;
        public NetReplicationController netreplicationcontroller;
        
        //! Manages world processing; things like: moving avatar, managing camera, animation, physics, etc...
        void ProcessWorld()
        {
            //Avatar avatar = (Avatar)worldstorage.GetEntityByReference( iMyReference );
            if( myavatar != null )
            {
                //Test.Debug("x: " << int(playermovement.avatarxpos) << " y: " << int(playermovement.avatarypos) << " z: " << int(playermovement.avatarzpos));
                myavatar.pos = playermovement.avatarpos;
                
                // PLACEHOLDER
                
                playermovement.avatarpos = myavatar.pos;
            }
    
            playermovement.MovePlayer();
          //  Console.WriteLine( playermovement.avatarpos );
        }

        public delegate void TickHandler();
        public event TickHandler Tick;
    
        //! Main loop, called by SDL once a frame
        void MainLoop()
        {
            //Console.WriteLine("Tick");
            ProcessWorld();
            ChatController.GetInstance().CheckMessages();
            if (Tick != null)
            {
                Tick();
            }
            network.Tick();
        }
    
        //! Gets world state from server
        void InitializeWorld()
        {
            playermovement.avatarpos = new Vector3( -5, 0, 0 );
            playermovement.avatarzrot = 0;
            playermovement.avataryrot = 0;
        }
        
        public int Go( string[] args )
        {
            Arguments arguments = new Arguments( args );
            config = Config.GetInstance();
            playermovement = PlayerMovement.GetInstance();
            worldstorage = WorldModel.GetInstance();
    
            InitializeWorld();

            string serverip = "";
            if( arguments.Named.ContainsKey("serverip" ) )
            {
                serverip = arguments.Named[ "serverip" ];
            }
            
            network = new NetworkLevel2Controller();
            network.NewConnection += new Level2NewConnectionHandler(network_NewConnection);
            if (serverip == "")
            {
                network.ConnectAsClient(config.ServerIPAddress, config.ServerPort);
            }
            else
            {
                network.ConnectAsClient(serverip, config.ServerPort);
            }

            PluginsLoader.GetInstance().LoadPlugins();

            myavatar = new Avatar();
            worldstorage.AddEntity(myavatar);

            renderer = RendererFactory.GetInstance();
            renderer.RegisterMainLoopCallback( new MainLoopDelegate( this.MainLoop ) );
            renderer.StartMainLoop();
            
            return 0;
        }

        void network_NewConnection(NetworkLevel2Connection net2con, ConnectionInfo connectioninfo)
        {
            Console.WriteLine("client connected to server");
            rpc = new RpcController(network);
            netreplicationcontroller = new NetReplicationController(rpc);
        }
    }
}
   
