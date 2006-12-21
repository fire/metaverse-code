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
using System.Threading;
using System.Net;
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
            if (targettoload != "")
            {
                 WorldPersistToXml.GetInstance().LoadFromUrl( targettoload );
                 targettoload = "";
            }
            ProcessWorld();
            if (Tick != null)
            {
                Tick();
            }
            network.Tick();
        }
    
        //! Gets world state from server
        void InitializePlayermovement()
        {
            playermovement.avatarpos = new Vector3( -5, 0, 0 );
            playermovement.avatarzrot = 0;
            playermovement.avataryrot = 0;
        }

        public ChatController chatcontroller = null;
        void LoadChat()
        {
            chatcontroller = new ChatController();
        }

        string targettoload = ""; // incoming url from osmp:// or possibly commandline

        public bool waitingforserverconnection = true;
        public int Go(string[] args)
        {
            Arguments arguments = new Arguments(args);

            config = Config.GetInstance();

            string serverip = config.ServerIPAddress;
            int port = config.ServerPort;

            if (arguments.Named.ContainsKey("serverip"))
            {
                serverip = arguments.Named["serverip"];
            }
            if( arguments.Named.ContainsKey("serverport") )
            {
                port = Convert.ToInt32(arguments.Named["serverport"]);
            }

            network = new NetworkLevel2Controller();
            network.NewConnection += new Level2NewConnectionHandler(network_NewConnection);

            network.ConnectAsClient(serverip, port);

            rpc = new RpcController(network);
            netreplicationcontroller = new NetReplicationController(rpc);

            playermovement = PlayerMovement.GetInstance();
            worldstorage = new WorldModel(netreplicationcontroller);

            InitializePlayermovement();

            myavatar = new Avatar();
            worldstorage.AddEntity(myavatar);

            PluginsLoader.GetInstance().LoadClientPlugins(arguments);
            if (!arguments.Unnamed.Contains("nochat"))
            {
                LoadChat();
            }

            foreach (string argument in args)
            {
                Console.WriteLine( argument );
                if (argument.StartsWith( "osmp://" ))
                {
                    targettoload = "http://" + argument.Substring( "osmp://".Length );
                    Console.WriteLine( "target: " + targettoload );
                    // WorldPersistToXml.GetInstance().LoadFromUrl( target );
                }
            }

            renderer = RendererFactory.GetInstance();
            renderer.Tick += new OSMP.TickHandler(MainLoop);
            renderer.Init();
            renderer.StartMainLoop();

            return 0;
        }

        public void ConnectToServer(string ipaddressstring, int port)
        {
            IPAddress[] addresses = System.Net.Dns.GetHostAddresses(ipaddressstring);
            if (addresses.GetLength(0) == 0)
            {
                return;
            }
            IPAddress ipaddress = addresses[0];
            Console.WriteLine("Resolved server address to : " + ipaddressstring);

            try
            {
                network.ConnectAsClient(ipaddress.ToString(), port);
            }
            catch (Exception e)
            {
                DialogHelpers.ShowErrorMessage( null, "Failed to connect to server");
                Console.WriteLine(e.ToString());
            }

            SelectionModel.GetInstance().Clear();
            worldstorage.Clear();
        }

        void network_NewConnection(NetworkLevel2Connection net2con, ConnectionInfo connectioninfo)
        {
            Console.WriteLine("client connected to server");
            waitingforserverconnection = false;

            InitializePlayermovement();

            myavatar = new Avatar();
            worldstorage.AddEntity(myavatar);

            new NetworkInterfaces.WorldControl_ClientProxy(rpc, connectioninfo.Connection)
                .RequestResendWorld();
        }
    }
}
   
