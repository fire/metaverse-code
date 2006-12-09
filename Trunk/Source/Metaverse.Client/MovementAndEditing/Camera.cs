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
//! \brief Handles camera movement on client, such as pans, orbits etc

using System;
using System.Collections;
using System.Windows.Forms;
using MathGl;

namespace OSMP
{
    public class Camera
    {
        public enum CameraMoveType
        {
             None,
             AltZoom,
             Orbit,
             Pan
        }
        
        public enum Viewpoint
        {
            MouseLook,
            BehindPlayer,
            ThirdParty
        }
        
        public Viewpoint viewpoint = Viewpoint.MouseLook;
    
        CameraMoveType CurrentMove; //!< current movetype, eg PAN or ORBIT
        
        public bool bRoamingCameraEnabled;  //!< if camera is enabled (otherwise, just normal avatar view)
        public Vector3 RoamingCameraPos = new Vector3();  //!< position of camera
        public Rot RoamingCameraRot = new Rot();  //!< rotation of camera
            
        public Vector3 CameraPos;
        public Rot CameraRot;
            
        int iDragStartx;
        int iDragStarty;
        
        Vector3 AltZoomCentrePos = new Vector3();
    
        double fAltZoomStartRotateZAxis;
        double fAltZoomStartRotateYAxis;
        double fAltZoomStartZoom;
    
        double fAltZoomRotateZAxis;
        double fAltZoomRotateYAxis;
        double fAltZoomZoom;
        
        public double fThirdPartyViewZoom;  //!< used in third party view (f9 twice).  Distance from avatar
        public double fThirdPartyViewRotate;   //!< used in third party view (f9 twice).  Rotation around avatar        
        
        static Camera instance = new Camera();
        public static Camera GetInstance()
        {
            return instance;
        }
        
        public Camera()
        {
            IMouseFilterMouseCache mousefiltermousecache = MouseFilterMouseCacheFactory.GetInstance();
            KeyFilterComboKeys.GetInstance().RegisterCombo(
                new string[]{"cameramode"}, null, new KeyComboHandler( CameraModeZoomHandler ) );
            KeyFilterComboKeys.GetInstance().RegisterCombo(
                new string[]{"cameramode","cameraorbit"}, null, new KeyComboHandler( CameraModeOrbitHandler ) );
            KeyFilterComboKeys.GetInstance().RegisterCombo(
                new string[]{"cameramode","camerapan"}, null, new KeyComboHandler( CamerModePanHandler ) );

            KeyFilterComboKeys.GetInstance().RegisterCombo(
                new string[]{"toggleviewpoint"}, null, new KeyComboHandler( ToggleViewpointHandler ) );
                
            mousefiltermousecache.MouseDown += new MouseEventHandler( MouseDown );
            mousefiltermousecache.MouseMove += new MouseEventHandler( MouseMove );
            mousefiltermousecache.MouseUp += new MouseEventHandler( MouseUp );
        }
        
        public void CameraModeZoomHandler( object source, ComboKeyEventArgs e )
        {
        }
        public void CameraModeOrbitHandler( object source, ComboKeyEventArgs e )
        {
        }
        public void CamerModePanHandler( object source, ComboKeyEventArgs e )
        {
        }
        public void ToggleViewpointHandler( object source, ComboKeyEventArgs e )
        {
            if( e.IsComboDown )
            {
                Test.Debug(  "toggling viewpoint..." ); // Test.Debug
                // viewpoint = (Viewpoint)(( (int)viewpoint + 1 ) % 3 );
                viewpoint = (Viewpoint)(( (int)viewpoint + 1 ) % 2 );  // disactivating third viewpoint for now (since we cant see avatar at moment...)
            }
        }
        
        public void MouseDown( object source, MouseEventArgs e )
        {
        }
        public void MouseMove( object source, MouseEventArgs e )
        {
        }
        public void MouseUp( object source, MouseEventArgs e )
        {
        }
    
        public void InitiateOrbitSlashAltZoom( int imousex, int imousey, CameraMoveType eMoveType )
        {
        }
        
        public void GetCurrentCameraFromAltZoomCamera()
        {
            Rot RotationAboutZAxis =  mvMath.AxisAngle2Rot( mvMath.ZAxis, mvMath.Pi - fAltZoomRotateZAxis );
            Rot RotationAboutYAxis = mvMath.AxisAngle2Rot( mvMath.YAxis, fAltZoomRotateYAxis );
        
            Rot CombinedZYRotation = RotationAboutZAxis * RotationAboutYAxis;
        
            double DeltaZ = fAltZoomZoom * Math.Sin( fAltZoomRotateYAxis );
            RoamingCameraPos.z = AltZoomCentrePos.z + DeltaZ;
        
            double DistanceInXYPlane = fAltZoomZoom * Math.Cos( fAltZoomRotateYAxis );
            RoamingCameraPos.x = AltZoomCentrePos.x + DistanceInXYPlane * Math.Cos( fAltZoomRotateZAxis );
            RoamingCameraPos.y = AltZoomCentrePos.y - DistanceInXYPlane * Math.Sin( fAltZoomRotateZAxis );
        
            RoamingCameraRot = CombinedZYRotation;
        }
        
        public void UpdatePanCamera( int imousex, int imousey )
        {
        }
        
        public void UpdateOrbitCamera( int imousex, int imousey )
        {
            if( CurrentMove == CameraMoveType.Orbit )
            {
                fAltZoomRotateZAxis = (double)( imousex - iDragStartx ) / 20.0f / 10.0f + fAltZoomStartRotateZAxis;
                fAltZoomRotateYAxis = (double)( imousey - iDragStarty ) / 20.0f / 10.0f + fAltZoomStartRotateYAxis;
            }
            else if( CurrentMove == CameraMoveType.AltZoom )
            {
                fAltZoomStartRotateZAxis = fAltZoomRotateZAxis;
                fAltZoomStartRotateYAxis = fAltZoomRotateYAxis;
                fAltZoomStartZoom = fAltZoomZoom;
        
                iDragStartx = imousex;
                iDragStarty = imousey;
                CurrentMove = CameraMoveType.Orbit;
            }
            else if( CurrentMove == CameraMoveType.None )
            {}
        
            GetCurrentCameraFromAltZoomCamera();
        }
        
        public void UpdateAltZoomCamera( int imousex, int imousey )
        {
            if( CurrentMove == CameraMoveType.AltZoom )
            {
                // Test.Debug(  " updatealtzoom " << imousex << " " << imousey ); // Test.Debug
                fAltZoomRotateZAxis = (double)( imousex - iDragStartx ) / 20.0f / 10.0f + fAltZoomStartRotateZAxis;
                fAltZoomZoom = (double)( imousey - iDragStarty ) / 20.0f  + fAltZoomStartZoom;
            }
            else if( CurrentMove == CameraMoveType.Orbit )
            {
                fAltZoomStartRotateZAxis = fAltZoomRotateZAxis;
                fAltZoomStartRotateYAxis = fAltZoomRotateYAxis;
                fAltZoomStartZoom = fAltZoomZoom;
        
                iDragStartx = imousex;
                iDragStarty = imousey;
                CurrentMove = CameraMoveType.AltZoom;
            }
            else if( CurrentMove == CameraMoveType.None )
            {
            }
        
            GetCurrentCameraFromAltZoomCamera();
        }
        
        public void InitiatePanCamera( int imousex, int imousey )
        {
            CurrentMove = CameraMoveType.Pan;
            iDragStartx = imousex;
            iDragStarty = imousey;
        }
        
        public void InitiateOrbitCamera( int imousex, int imousey )
        {
            InitiateOrbitSlashAltZoom( imousex, imousey, CameraMoveType.Orbit );
        }
        
        public void InitiateAltZoomCamera( int imousex, int imousey )
        {
            InitiateOrbitSlashAltZoom( imousex, imousey, CameraMoveType.AltZoom );
        }
        
        public void CancelRoamingCamera()
        {
            bRoamingCameraEnabled = false;
        }
        
        public void CameraMoveDone()
        {
            CurrentMove = CameraMoveType.None;
        }
        
        public void ApplyCamera()
        {
            // rotate so z axis is up, and x axis is forward
            
            PlayerMovement playermovement = PlayerMovement.GetInstance();
            
            GLMatrix4d cameramatrix = GLMatrix4d.identity();
            
            cameramatrix.applyRotate( 90, 0.0, 0.0, 1.0 );
            cameramatrix.applyRotate( 90, 0.0, 1.0, 0.0 );
            
            if( bRoamingCameraEnabled )
            {
                Rot inversecamerarot = RoamingCameraRot.Inverse();
                mvMath.ApplyRotToGLMatrix4d( ref cameramatrix, inversecamerarot  );
                cameramatrix.applyTranslate( - RoamingCameraPos.x, - RoamingCameraPos.y, - RoamingCameraPos.z  );
            }
            else if( viewpoint == Viewpoint.MouseLook )
            {
                cameramatrix.applyRotate( - playermovement.avataryrot, 0f, 1f, 0f );
                cameramatrix.applyRotate( - playermovement.avatarzrot, 0f, 0f, 1f );
                cameramatrix.applyTranslate( - playermovement.avatarpos.x, - playermovement.avatarpos.y, - playermovement.avatarpos.z );
            }
            else if( viewpoint == Viewpoint.BehindPlayer )
            {
                cameramatrix.applyRotate( -18f, 0f, 1f, 0f );

                // Vector3 V = new Vector3( 0, playermovement.avataryrot * mvMath.PiOver180, playermovement.avatarzrot * mvMath.PiOver180 );

                cameramatrix.applyTranslate( 3.0f, 0.0f, -1.0f );

                cameramatrix.applyRotate( - (float)playermovement.avataryrot, 0f, 1f, 0f );
                cameramatrix.applyRotate( - (float)playermovement.avatarzrot, 0f, 0f, 1f );

                cameramatrix.applyTranslate( -playermovement.avatarpos.x, -playermovement.avatarpos.y, -playermovement.avatarpos.z );
            }
            else if( viewpoint == Viewpoint.ThirdParty )
            {
                cameramatrix.applyRotate( -18f, 0f, 1f, 0f );
                cameramatrix.applyRotate( -90f, 0f, 0f, 1f );

                cameramatrix.applyTranslate( 0.0, - fThirdPartyViewZoom, fThirdPartyViewZoom / 3.0 );
                cameramatrix.applyRotate( - fThirdPartyViewRotate, 0f, 0f, 1f );
                cameramatrix.applyTranslate( - playermovement.avatarpos.x, - playermovement.avatarpos.y, - playermovement.avatarpos.z );
            }    
            
            GraphicsHelperFactory.GetInstance().LoadMatrix( cameramatrix.ToArray() );
        }
    }    
}
