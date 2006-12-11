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
        public double fHeight = -1.0;    //!< height of hardcoded land
        //public int iMyReference = 0;  //!< iReference of user's avatar
        public Avatar myavatar;
    
        Config config;
        IRenderer renderer;
        PlayerMovement playermovement;
        WorldModel worldstorage;
        
        static MetaverseClient instance = new MetaverseClient();
        public static MetaverseClient GetInstance()
        {
            return instance;
        }
    
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
    
        //! Main loop, called by SDL once a frame
        void MainLoop()
        {
            ProcessWorld();
            ChatController.GetInstance().CheckMessages();
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
            
            PluginsLoader.GetInstance().LoadPlugins();
            
            myavatar = new Avatar();
            worldstorage.AddEntity( myavatar );
            
            renderer = RendererFactory.GetInstance();
            renderer.RegisterMainLoopCallback( new MainLoopDelegate( this.MainLoop ) );
            renderer.StartMainLoop();
            
            return 0;
        }
    }
}
   
