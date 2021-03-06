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
//! \brief This module is responsible for moving one's own avatar around

using System;
using System.Collections;
//using System.Windows.Forms;
using System.Xml;

namespace OSMP
{
    public class PlayerMovement
    {
        Vector3 currentvelocity = new Vector3();
        TimeKeeper timekeeper;
            
        Camera camera;
        
        Vector3 WorldBoundingBoxMin;
        Vector3 WorldBoundingBoxMax;

        public Vector3 avatarpos = new Vector3( 0, 0, 0.5 );
        public Rot avatarrot = new Rot();

        public bool bAvatarMoved;  //! If avatar has moved (so it should be synchronized to server)
        
        double fAvatarAcceleration;
        double fAvatarTurnSpeed;  //! turnspeed of avatar (constant)
        double fAvatarMoveSpeed;   //! movespeed of avatar (constant)
        double fVerticalMoveSpeed;
        double fDeceleration;
        
        public bool kMovingLeft;    //!< set by keyboardandmouse, and used by playermovement to set current player object velocity
        public bool kMovingRight;//!< set by keyboardandmouse, and used by playermovement to set current player object velocity
        public bool kMovingForwards;//!< set by keyboardandmouse, and used by playermovement to set current player object velocity
        public bool kMovingBackwards;//!< set by keyboardandmouse, and used by playermovement to set current player object velocity
        public bool kMovingUpZAxis;//!< set by keyboardandmouse, and used by playermovement to set current player object velocity
        public bool kMovingDownZAxis;//!< set by keyboardandmouse, and used by playermovement to set current player object velocity
        
        public bool bJumping;//!< set by keyboardandmouse, and used by playermovement to set current player object velocity
        
        public double avatarzrot,avataryrot;    //!< avatar z and y rot, used to get avatar rotation
        
        static PlayerMovement instance = new PlayerMovement();
        public static PlayerMovement GetInstance()
        {
            return instance;
        }
        
        public void MoveLeft( string command, bool down )
        {
            Test.WriteOut("playermovement.moveleft");
            kMovingLeft = down;
        }
        public void MoveRight(string command, bool down)
        {
            kMovingRight = down;
        }
        public void MoveForwards(string command, bool down)
        {
            kMovingForwards = down;
        }
        public void MoveBackwards(string command, bool down)
        {
            kMovingBackwards = down;
        }
        public void MoveUp(string command, bool down)
        {
            kMovingUpZAxis = down;
        }
        public void MoveDown(string command, bool down)
        {
            kMovingDownZAxis = down;
        }
        public PlayerMovement()
        {
            Test.Debug("instantiating PlayerMovement()");
            
            Config config = Config.GetInstance();
            XmlElement minnode = (XmlElement)config.clientconfig.SelectSingleNode("worldboundingboxmin");
            XmlElement maxnode =(XmlElement) config.clientconfig.SelectSingleNode("worldboundingboxmax");
            WorldBoundingBoxMin = new Vector3( minnode );
            WorldBoundingBoxMax = new Vector3( maxnode );
            Test.WriteOut( WorldBoundingBoxMin );
            Test.WriteOut( WorldBoundingBoxMax );
            
            XmlElement movementnode = (XmlElement)config.clientconfig.SelectSingleNode("movement");
            fAvatarAcceleration = Convert.ToDouble( movementnode.GetAttribute("acceleration") );
            fAvatarTurnSpeed = Convert.ToDouble( movementnode.GetAttribute("turnspeed") );
            fAvatarMoveSpeed = Convert.ToDouble( movementnode.GetAttribute("movespeed") );
            fVerticalMoveSpeed = Convert.ToDouble( movementnode.GetAttribute("verticalmovespeed") );
            fDeceleration = Convert.ToDouble( movementnode.GetAttribute("deceleration") );

            camera = Camera.GetInstance();
            //KeyFilterComboKeys keyfiltercombokeys = KeyFilterComboKeys.GetInstance();

            CommandCombos.GetInstance().RegisterCommand(
                "moveleft", new KeyCommandHandler(MoveLeft));
            CommandCombos.GetInstance().RegisterCommand(
                "moveright", new KeyCommandHandler(MoveRight));
            CommandCombos.GetInstance().RegisterCommand(
                "movebackwards", new KeyCommandHandler(MoveBackwards));
            CommandCombos.GetInstance().RegisterCommand(
                "moveforwards", new KeyCommandHandler(MoveForwards));
            CommandCombos.GetInstance().RegisterCommand(
                "moveup", new KeyCommandHandler(MoveUp));
            CommandCombos.GetInstance().RegisterCommand(
                "movedown", new KeyCommandHandler(MoveDown));

            CommandCombos.GetInstance().RegisterCommand(
              "mouselook", new KeyCommandHandler(ActivateMouseLook));

            CommandCombos.GetInstance().RegisterCommand(
              "leftmousebutton", new KeyCommandHandler(MouseDown));

            MouseCache.GetInstance().MouseMove += new MouseMoveHandler(PlayerMovement_MouseMove);

            //keyfiltercombokeys.RegisterCombo( new string[]{"moveleft"},new string[]{"ANY"}, new KeyComboHandler( this.MoveLeft ) );
            //keyfiltercombokeys.RegisterCombo( new string[]{"moveright"},new string[]{"ANY"}, new KeyComboHandler( this.MoveRight ) );
            //keyfiltercombokeys.RegisterCombo( new string[]{"movebackwards"},new string[]{"ANY"}, new KeyComboHandler( this.MoveBackwards ) );
            //keyfiltercombokeys.RegisterCombo( new string[]{"moveforwards"},new string[]{"ANY"}, new KeyComboHandler( this.MoveForwards ) );
            //keyfiltercombokeys.RegisterCombo( new string[]{"moveup"},new string[]{"ANY"},new KeyComboHandler( this.MoveUp ) );
            //keyfiltercombokeys.RegisterCombo( new string[]{"movedown"},new string[]{"ANY"}, new KeyComboHandler( this.MoveDown ) );

            //keyfiltercombokeys.RegisterCombo( new string[]{"none"},new string[]{"moveleft","moveright","movebackwards","moveforwards","moveup","movedown"}, new KeyComboHandler( this.ActivateMouseLook ) );
            
            // note to self: to do:
            //RendererFactory.GetInstance().MouseDown += new MouseEventHandler( MouseDown );
            //RendererFactory.GetInstance().MouseMove += new MouseEventHandler( MouseMove );
            //RendererFactory.GetInstance().MouseUp += new MouseEventHandler( MouseUp );
            
            timekeeper = new TimeKeeper();
            
            Test.Debug("PlayerMovement instantiated");
        }

        //bool bcapturing = false; // if someone else filtered our MouseDown (ie SelectionModel, Camera,etc...), we shouldnt be processing MouseMove
        //bool _bcapturing;

        int istartmousex;
        int istartmousey;
        double startavatarzrot;
        double startavataryrot;

        public void ActivateMouseLook(string command, bool down)
        {
            Console.WriteLine("PlayerMovement.ActivateMouseLook " + down);
            //bcapturing = down;

            if (down)
            {
                istartmousex = MouseCache.GetInstance().MouseX;
                istartmousey = MouseCache.GetInstance().MouseY;
                startavatarzrot = avatarzrot;
                startavataryrot = avataryrot;

                //ViewerState.GetInstance().CurrentViewState = ViewerState.ViewerStateEnum.Mouselook;
            }
            else
            {
                //ViewerState.GetInstance().CurrentViewState = ViewerState.ViewerStateEnum.None;
                InMouseMoveDrag = false;
            }
        }

        bool InMouseMoveDrag = false;

        void PlayerMovement_MouseMove()
        {
            //Console.WriteLine("PlayerMovement.MouseMove " + bcapturing);
            if ( InMouseMoveDrag &&
                ViewerState.GetInstance().CurrentViewState == ViewerState.ViewerStateEnum.None)
            {
                avatarzrot = startavatarzrot - (double)(MouseCache.GetInstance().MouseX - istartmousex) * fAvatarTurnSpeed;
                avataryrot = Math.Min(Math.Max(startavataryrot + (double)(MouseCache.GetInstance().MouseY - istartmousey) * fAvatarTurnSpeed, -90), 90);
                UpdateAvatarObjectRotAndPos();
            }
        }

        public void MouseDown(string command, bool down)
        {
            //Test.Debug("Playermovement MouseDown " + e.ToString() );
            //Console.WriteLine("PlayerMovement.MouseDown " + down + " " + bcapturing);
            if (ViewerState.GetInstance().CurrentViewState == ViewerState.ViewerStateEnum.None)
            {
                InMouseMoveDrag = down;
            }
            else
            {
                InMouseMoveDrag = false;
            }
        }

        //public void MouseMove( object source, MouseEventArgs e )
        //{
            //Test.Debug("Playermovement MouseMove " + e.ToString() );
          //  if( _bcapturing )
            //{
              //  avatarzrot = startavatarzrot - (double)( e.X - istartmousex ) * fAvatarTurnSpeed;
                //avataryrot = Math.Min( Math.Max( startavataryrot + (double)( e.Y - istartmousey ) * fAvatarTurnSpeed, - 90 ), 90 );
                //UpdateAvatarObjectRotAndPos();
            //}
        //}

        //public void MouseUp( object source, MouseEventArgs e )
        //{
            //Test.Debug("Playermovement MouseUp " + e.ToString() );
            //bcapturing = false;
          //  _bcapturing = false;
        //}

        public void UpdateAvatarObjectRotAndPos()
        {
            avatarrot = mvMath.AxisAngle2Rot( mvMath.ZAxis, ( avatarzrot * Math.PI / 180 ) );
            avatarrot = avatarrot * mvMath.AxisAngle2Rot( mvMath.YAxis, avataryrot * Math.PI / 180 );
            
            Entity avatar = MetaverseClient.GetInstance().myavatar;
        
            if( avatar != null )
            {
                avatar.pos = avatarpos;        
                avatar.rot = avatarrot;
            }
        }
        
        public void MovePlayer()
        {
            double fRight = 0.0;
            double fUp = 0.0;
            double fForward = 0.0;
        
            if( kMovingLeft )
            {
                fRight -= 1.0;
                bAvatarMoved = true;
            }
            if( kMovingRight )
            {
                fRight += 1.0;
                bAvatarMoved = true;
            }
            if( kMovingForwards )
            {
                bAvatarMoved = true;
                fForward += 1.0;
            }
            if( kMovingBackwards )
            {
                fForward -= 1.0;
                bAvatarMoved = true;
            }
            if( kMovingUpZAxis )
            {
                fUp += 1.0;
                bAvatarMoved = true;
            }
            if( kMovingDownZAxis )
            {
                fUp -= 1.0;
                bAvatarMoved = true;
            }
        
            double fTimeSlotMultiplier = (double)timekeeper.ElapsedTime / 100; // PLACEHOLDER
            if( bAvatarMoved )
            {
                switch( camera.viewpoint )
                {
                    case Camera.Viewpoint.MouseLook:
                    //case Camera.Viewpoint.BehindPlayer:
                        Vector3 accelerationavaxes = new Vector3( fForward, - fRight, 0 )  * fTimeSlotMultiplier * fAvatarAcceleration;
                        Vector3 accelerationvectorworldaxes = accelerationavaxes * MetaverseClient.GetInstance().myavatar.rot.Inverse();
                    
                        accelerationvectorworldaxes.z = fUp * fAvatarAcceleration * fTimeSlotMultiplier;
                        
                        currentvelocity = currentvelocity / ( 1 + fTimeSlotMultiplier * fDeceleration ) + accelerationvectorworldaxes;
                        currentvelocity.x = Math.Max( Math.Min( currentvelocity.x, fAvatarMoveSpeed ), -fAvatarMoveSpeed );
                        currentvelocity.y = Math.Max( Math.Min( currentvelocity.y, fAvatarMoveSpeed ), -fAvatarMoveSpeed );
                        currentvelocity.z = Math.Max( Math.Min( currentvelocity.z, fVerticalMoveSpeed ), -fVerticalMoveSpeed );
                    
                        avatarpos = avatarpos + currentvelocity * fTimeSlotMultiplier;
                        
                        avatarpos.x = Math.Max( avatarpos.x, WorldBoundingBoxMin.x );
                        avatarpos.y = Math.Max( avatarpos.y, WorldBoundingBoxMin.y );
                        avatarpos.z = Math.Max( avatarpos.z, WorldBoundingBoxMin.z );
                        
                        avatarpos.x = Math.Min( avatarpos.x, WorldBoundingBoxMax.x );
                        avatarpos.y = Math.Min( avatarpos.y, WorldBoundingBoxMax.y );
                        avatarpos.z = Math.Min( avatarpos.z, WorldBoundingBoxMax.z );
                                
                        break;
        
                    //case Camera.Viewpoint.ThirdParty:
                      //  camera.fThirdPartyViewZoom += fForward;
                        //camera.fThirdPartyViewRotate += fRight * 3.0;
                        //break;
                }
                UpdateAvatarObjectRotAndPos();
            }
        }
    }
}
