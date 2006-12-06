// Copyright Hugh Perkins 2004,2005,2006
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
//! \brief mgraphics contains certain wrapper routines around OpenGL/GLUT

using System;
using Tao.OpenGl;
using Tao.FreeGlut;
using MathGl;

namespace OSMP
{
    class GraphicsHelperGl : IGraphicsHelper
    {
        int iWindowWidth = 1024;
        int iWindowHeight = 812;
        
        bool bCubeDefined = false;
        bool bConeDefined = false;
        bool bCylinderDefined = false;
        bool bSphereDefined = false;
        
        int LISTCONE = 1;
        int LISTCUBE = 2;
        int LISTSPHERE = 3;
        int LISTCYLINDER = 4;
        
        //! Feedback point buffer for OpenGL feedback, used by mvgraphics.cpp
        /*
        class FeedbackPointBufferItem
        {
            public double type;
            public double x;
            public double y;
            public override string ToString()
            {
                return "<FeedbackPointBufferItem type=\"" + type.ToString() + "\" x=\"" + x.ToString() + "\" y=\"" + y.ToString() + " />";
            }
        }
        */
        
        Vector2 GetFeedbackPointBufferItem( float[]buffer, int index )
        {
            // each point takes 3 bytes:
            // type, x, y
            int vertexoffset = 3 * index + 1;
            return new Vector2( buffer[ vertexoffset ], buffer[ vertexoffset + 1 ] );
        }
        
        Vector2 GetFeedbackLineBufferItem( float[]buffer, int lineindex, int vertexindex )
        {
            // each line takes 5 bytes:
            // type, x1, y1, x2, y2
            int vertexoffset = 5 * lineindex + 1 + vertexindex * 2;
            return new Vector2( buffer[ vertexoffset ], buffer[ vertexoffset + 1 ] );
        }
        
        //! Feedback line buffer for OpenGL feedback, used by mvgraphics.cpp
        /*
        class FeedbackLineBufferItem
        {
            public double type;
            public Vector2[] vertices = new Vector2[2];
        }
        */
        
        public Vector3 GetMouseVector( Vector3 OurPos, Rot OurRot, int imousex, int imousey )
        {
            IRenderer renderer = RendererFactory.GetInstance();
            
            //double FeedbackBuffer[9];
            float[] feedbackbuffer = new float[ 2 * 3 ];
            Gl.glFeedbackBuffer( 2 * 3, Gl.GL_2D, feedbackbuffer );
        
            int[] viewport = new int[4];
            // This Sets The Array <viewport> To The Size And Location Of The Screen Relative To The Window
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);
        
            Gl.glPushMatrix();
        
            Gl.glRenderMode( Gl.GL_FEEDBACK );
        
            Gl.glLoadIdentity();
        
            // rotate so z axis is up, and x axis is forward
            Gl.glRotatef( 90f, 0.0f, 0.0f, 1.0f );
            Gl.glRotatef( 90f, 0.0f, 1.0f, 0.0f );
        
            Gl.glTranslated( 10.0, 0.0, 0.0 );
        
            Gl.glBegin( Gl.GL_POINTS );
            Gl.glVertex3d( 0.0, 0.0, 0.0 );
            Gl.glVertex3d( 0.0, -1.0, 0.0 );
            Gl.glEnd();
        
            Test.Debug(  "number of points: " + Gl.glRenderMode( Gl.GL_RENDER ) ); // Test.Debug
        
            //Test.Debug(  feedbackbuffer[0].ToString() ); // Test.Debug
        
            Gl.glPopMatrix();
            
            Vector2 screenpointone = GetFeedbackPointBufferItem( feedbackbuffer, 0 );
            Vector2 screenpointtwo = GetFeedbackPointBufferItem( feedbackbuffer, 1 );
        
            Test.Debug(  "Screencoords of input vertex: " + screenpointone.ToString() ); // Test.Debug
            Test.Debug(  "Screencoords of input vertex: " + screenpointtwo.ToString() ); // Test.Debug
        
            Vector3 ScreenPosPointOne = new Vector3( 0, renderer.WindowWidth - screenpointone.x, screenpointone.y );
            Vector3 ScreenPosPointTwo = new Vector3( 0, renderer.WindowWidth - screenpointtwo.x, screenpointtwo.y );
        
            Vector3 ScreenMousePos = new Vector3 ( 0, renderer.WindowWidth - imousex, renderer.WindowHeight - imousey );
            Test.Debug(  "posone " + ScreenPosPointOne.ToString() + " postwo " + ScreenPosPointTwo.ToString() ); // Test.Debug
        
            Vector3 MouseVectorOnScreenScreenScreenCoords = ScreenMousePos - ScreenPosPointOne;
            Test.Debug(  " MouseVectorOnScreenScreenScreenCoords " + MouseVectorOnScreenScreenScreenCoords.ToString() ); // Test.Debug
        
            double fScalingFromWorldToScreen = ( (double)ScreenPosPointOne.y -  (double)ScreenPosPointTwo.y );
            Test.Debug(  " GetScalingFrom3DToScreen " + GetScalingFrom3DToScreen( 10.0f ).ToString() ); // Test.Debug
        
            Vector3 MouseVectorRayWorldCoordsAvatarAxes = new Vector3(
                10.0f,
                MouseVectorOnScreenScreenScreenCoords.y * ( 1.0f / fScalingFromWorldToScreen ),
                MouseVectorOnScreenScreenScreenCoords.z * ( 1.0f / fScalingFromWorldToScreen )
            );
            Test.Debug(  "MouseVectorRayWorldCoordsAvatarAxes " + MouseVectorRayWorldCoordsAvatarAxes.ToString() ); // Test.Debug
        
            Rot InverseOurRot = OurRot.Inverse();
        
            Vector3 MouseVectorRayWorldCoordsWorldAxes = MouseVectorRayWorldCoordsAvatarAxes * InverseOurRot;
            Test.Debug(  " MouseVectorRayWorldCoordsWorldAxes " + MouseVectorRayWorldCoordsWorldAxes.ToString() ); // Test.Debug
        
            return MouseVectorRayWorldCoordsWorldAxes;
        }

        // we want to find out the length something of 1 world unit takes up in screen pixels when it is fDepth away from us
        // into the screen (along our x axis)
        // we draw a lines in the world, at depth fDepth (in world axes)
        // and we use an OpenGl FeedbackBuffer to find out how long that line is on the screen
        // ( see http://msdn.microsoft.com/library/en-us/opengl/glfunc02_3m42.asp )
        public double GetScalingFrom3DToScreen( double fDepth )
        {
            int iNumLines = 1;
            // each line uses five floats in the buffer:
            float[] feedbackbuffer = new float[ iNumLines * 5 ];
        
            int[] viewport = new int[4];
            // This Sets The Array <viewport> To The Size And Location Of The Screen Relative To The Window
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);
        
            Gl.glLoadIdentity();
            
            RendererFactory.GetInstance().ApplyViewingMatrixes();
        
            // rotate so z axis is up, and x axis is forward
            Gl.glRotatef( 90f, 0.0f, 0.0f, 1.0f );
            Gl.glRotatef( 90f, 0.0f, 1.0f, 0.0f );
        
            Gl.glFeedbackBuffer( iNumLines * 5, Gl.GL_2D, feedbackbuffer );
            Gl.glRenderMode( Gl.GL_FEEDBACK );
            
            Gl.glBegin( Gl.GL_LINES );
            Gl.glVertex3d( fDepth, -0.5, 0 );
            Gl.glVertex3d( fDepth, 0.5, 0 );
            Gl.glEnd();
                    
            int iNumValues = Gl.glRenderMode( Gl.GL_RENDER );
        
            // A is distance of vector, at depth fDepth in world, in pixels
            // Test.WriteOut( feedbackbuffer );
            Vector2 VectorA = GetFeedbackLineBufferItem( feedbackbuffer, 0, 1 ) - GetFeedbackLineBufferItem( feedbackbuffer, 0, 0 );
            double A = VectorA.Det();
            
            return A;
        }

        public Vector3 GetScreenPos( Vector3 ObserverPos, Rot ObserverRot, Vector3 TargetPos3D )
        {
            float[] feedbackbuffer = new float[ 1 * 3 ];
            
            Gl.glFeedbackBuffer( 3, Gl.GL_2D, feedbackbuffer );
        
            int[] viewport = new int[4];
            // This Sets The Array <viewport> To The Size And Location Of The Screen Relative To The Window
            Gl.glGetIntegerv( Gl.GL_VIEWPORT, viewport );
        
            Gl.glPushMatrix();
        
            Gl.glRenderMode( Gl.GL_FEEDBACK );
            Gl.glLoadIdentity();
        
            // rotate so z axis is up, and x axis is forward
            Gl.glRotatef( 90f, 0.0f, 0.0f, 1.0f );
            Gl.glRotatef( 90f, 0.0f, 1.0f, 0.0f );
        
            Rotate( ObserverRot.Inverse() );        
            Translate( - ObserverPos );
        
            Gl.glBegin( Gl.GL_POINTS );
            Gl.glVertex3f( (float)TargetPos3D.x, (float)TargetPos3D.y, (float)TargetPos3D.z );
            Gl.glEnd();
        
            Gl.glRenderMode( Gl.GL_RENDER );
        
            Gl.glPopMatrix();
        
            //DEBUG(  "Screencoords of input vertex: " << FeedbackBuffer.Vertices[0].x << " " << FeedbackBuffer.Vertices[0].y ); // DEBUG
            Vector2 screenpoint = GetFeedbackPointBufferItem( feedbackbuffer, 0 );
            Vector3 ScreenPos = new Vector3(
                0,
                RendererFactory.GetInstance().WindowWidth - screenpoint.x,
                screenpoint.y
            );
        
            //DEBUG(  "screenpos: " << ScreenPos ); // DEBUG
        
            return ScreenPos;
        }
        
        public void SetColor( double r, double g, double b )
        {
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE, new float[]{ (float)r, (float)g, (float)b, 1.0f } );
        }
        
        public void SetColor( Color color )
        {
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE, new float[]{ (float)color.r, (float)color.g, (float)color.b, 1.0f } );
        }
        
        public void PrintText( string text )
        {
            foreach( char p in text )
            {
                Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_TIMES_ROMAN_24, p );
            }
        }
        
        public void ScreenPrintText(int x, int y, string text )
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            Gl.glOrtho(0, iWindowWidth, 0, iWindowHeight, -1.0f, 1.0f);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            Gl.glPushAttrib(Gl.GL_DEPTH_TEST);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glRasterPos2i(x,y);
            PrintText( text );
            Gl.glPopAttrib();
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();
        }
        
        public void Rotate( Rot rot )
        {
            double fRotAngle = 0;
            Vector3 vAxis = new Vector3();
            mvMath.Rot2AxisAngle( ref vAxis, ref fRotAngle, rot );
            Gl.glRotatef( (float)( fRotAngle / mvMath.PiOver180 ), (float)vAxis.x, (float)vAxis.y, (float)vAxis.z );
        }
        
        public void DrawSquareXYPlane()
        {
            // Test.Debug(  "DrawSquareXYPlanes" ); // Test.Debug
            Gl.glBegin( Gl.GL_LINE_LOOP );
            Gl.glVertex3f( -0.5f, -0.5f, 0f);
            Gl.glVertex3f(-0.5f, 0.5f,0f);
            Gl.glVertex3f( 0.5f,0.5f, 0f);
            Gl.glVertex3f( 0.5f,-0.5f, 0f);
            Gl.glEnd();
        }
        
        public void DrawParallelSquares( int iNumSlices )
        {
            // Test.Debug(  "DrawParallelSquares" ); // Test.Debug
            Gl.glPushMatrix();
            Gl.glTranslated( 0,0, -0.5 );
            double fSpacing = 1.0 / (double)iNumSlices;
            for( int i = 0; i <= iNumSlices; i++ )
            {
                DrawSquareXYPlane();
                Gl.glTranslated( 0, 0, fSpacing );
            }
            Gl.glPopMatrix();
        }
        
        public void DrawWireframeBox( int iNumSlices )
        {
            // Test.Debug(  "DrawWireframeBox" ); // Test.Debug
            Gl.glPushMatrix();
            DrawParallelSquares( iNumSlices );
            Gl.glPushMatrix();
            Gl.glRotatef( 90f, 1f,0f,0f);
            DrawParallelSquares( iNumSlices );
        
            Gl.glPopMatrix();
        
            Gl.glRotatef( 90f, 0f,1f,0f);
            DrawParallelSquares( iNumSlices );
        
            Gl.glPopMatrix();
        }
        
        public void DrawCone()
        {
            if( !bConeDefined )
            {
                Gl.glNewList(LISTCONE, Gl.GL_COMPILE);
        
                Gl.glPushMatrix();
                Glu.GLUquadric quadratic = Glu.gluNewQuadric();   // Create A Pointer To The Quadric Object
                Glu.gluQuadricNormals( quadratic, Glu.GLU_SMOOTH); // Create Smooth Normals
                Glu.gluQuadricTexture( quadratic, 32 );  // Create Texture Coords
                Gl.glTranslated(0.0f,0.0f,-0.5f);   // Center The Cone
                Glu.gluQuadricOrientation( quadratic, Glu.GLU_OUTSIDE );
                Glu.gluCylinder(quadratic,0.5f,0.0f,1.0f,32,32);
                //glTranslatef(0.0f,0.0f,-0.5f);
                Gl.glRotatef( 180.0f, 1.0f, 0.0f, 0.0f );
                Glu.gluDisk(quadratic,0.0f,0.5f,32,32);
                //glRotatef( 180.0f, 1.0f, 0.0f, 0.0f );
                //glTranslatef(0.0f,0.0f,1.0f);
                //gluDisk(quadratic,0.0f,1.0f,32,32);
                Gl.glPopMatrix();
        
                Gl.glEndList();
                bConeDefined = true;
            }
        
            Gl.glCallList(LISTCONE);
        }
        
        public void DrawCube()
        {
        
            if( !bCubeDefined )
            {
                Gl.glNewList(LISTCUBE, Gl.GL_COMPILE);
        
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glNormal3f( 0.0f, 0.0f, 1.0f);
                Gl.glTexCoord2f(0.0f, 0.0f);
                Gl.glVertex3f( 1f, 1f, 1f);
                Gl.glTexCoord2f(0.0f, 1.0f);
                Gl.glVertex3f(0f, 1f,1f);
                Gl.glTexCoord2f(1.0f, 1.0f);
                Gl.glVertex3f( 0f,0f, 1f);
                Gl.glTexCoord2f(1.0f, 0.0f);
                Gl.glVertex3f( 1f,0f, 1f);
        
                Gl.glNormal3f( 0.0F, 0.0F,-1.0F);
                Gl.glTexCoord2f(1.0f, 0.0f);
                Gl.glVertex3f( 1f, 1f, 0f);
                Gl.glTexCoord2f(0.0f, 0.0f);
                Gl.glVertex3f( 1f,0f, 0f);
                Gl.glTexCoord2f(0.0f, 1.0f);
                Gl.glVertex3f( 0f,0f, 0f);
                Gl.glTexCoord2f(1.0f, 1.0f);
                Gl.glVertex3f(0f, 1f, 0f);
        
                Gl.glNormal3f( 0.0F, 1.0F, 0.0F);
                Gl.glTexCoord2f(0.0f, 0.0f);
                Gl.glVertex3f( 1f, 1f, 1f);
                Gl.glTexCoord2f(0.0f, 1.0f);
                Gl.glVertex3f( 1f, 1f,0f);
                Gl.glTexCoord2f(1.0f, 1.0f);
                Gl.glVertex3f(0f, 1f,0f);
                Gl.glTexCoord2f(1.0f, 0.0f);
                Gl.glVertex3f(0f, 1f, 1f);
        
                Gl.glNormal3f( 0.0F,-1.0F, 0.0F);
                Gl.glTexCoord2f(0.0f, 1.0f);
                Gl.glVertex3f(0f,0f,0f);
                Gl.glTexCoord2f(1.0f, 1.0f);
                Gl.glVertex3f( 1f,0f,0f);
                Gl.glTexCoord2f(1.0f, 0.0f);
                Gl.glVertex3f( 1f,0f, 1f);
                Gl.glTexCoord2f(0.0f, 0.0f);
                Gl.glVertex3f(0f,0f, 1f);
        
                Gl.glNormal3f( 1.0F, 0.0F, 0.0F);
                Gl.glTexCoord2f(1.0f, 0.0f);
                Gl.glVertex3f( 1f, 1f, 1f);
                Gl.glTexCoord2f(0.0f, 0.0f);
                Gl.glVertex3f( 1f,0f, 1f);
                Gl.glTexCoord2f(0.0f, 1.0f);
                Gl.glVertex3f( 1f,0f,0f);
                Gl.glTexCoord2f(1.0f, 1.0f);
                Gl.glVertex3f( 1f, 1f,0f);
        
                Gl.glNormal3f(-1.0F, 0.0F, 0.0F);
                Gl.glTexCoord2f(1.0f, 1.0f);
                Gl.glVertex3f(0f,0f,0f);
                Gl.glTexCoord2f(1.0f, 0.0f);
                Gl.glVertex3f(0f,0f, 1f);
                Gl.glTexCoord2f(0.0f, 0.0f);
                Gl.glVertex3f(0f, 1f, 1f);
                Gl.glTexCoord2f(0.0f, 1.0f);
                Gl.glVertex3f(0f, 1f,0f);
                Gl.glEnd();
                Gl.glEndList();
        
                //    Test.Debug(  "cube list stored" ); // Test.Debug
                bCubeDefined = true;
            }
        
            Gl.glPushMatrix();
            Gl.glTranslated( -0.5 , -0.5, -0.5 );
        
            // if( sTextureChecksum != "" )
            // {
            // }
        
            Gl.glCallList(LISTCUBE);
            Gl.glPopMatrix();
        }
        
        public void DrawSphere()
        {
            if( !bSphereDefined )
            {
                Gl.glNewList(LISTSPHERE, Gl.GL_COMPILE);
        
                Glu.GLUquadric quadratic = Glu.gluNewQuadric();   // Create A Pointer To The Quadric Object
                Glu.gluQuadricNormals(quadratic, Glu.GLU_SMOOTH); // Create Smooth Normals
                Glu.gluQuadricTexture(quadratic, 32 );  // Create Texture Coords
                Glu.gluSphere(quadratic,0.5f,32,32);
                Gl.glEndList();
                bSphereDefined = true;
            }
        
            Gl.glCallList(LISTSPHERE);
        }
        
        public void DrawCylinder()
        {
            if( !bCylinderDefined )
            {
                Gl.glNewList(LISTCYLINDER, Gl.GL_COMPILE);
        
                Gl.glPushMatrix();
                Glu.GLUquadric quadratic = Glu.gluNewQuadric();   // Create A Pointer To The Quadric Object
                Glu.gluQuadricNormals(quadratic, Glu.GLU_SMOOTH); // Create Smooth Normals
                Glu.gluQuadricTexture(quadratic, 32 );  // Create Texture Coords
                Gl.glTranslatef(0.0f,0.0f,-0.5f);   // Center The Cylinder
                Glu.gluQuadricOrientation( quadratic, Glu.GLU_OUTSIDE );
                Glu.gluCylinder(quadratic,0.5f,0.5f,1.0f,32,32);
                //glTranslatef(0.0f,0.0f,-0.5f);
                Gl.glRotatef( 180.0f, 1.0f, 0.0f, 0.0f );
                Glu.gluDisk(quadratic,0.0f,0.5f,32,32);
                Gl.glRotatef( 180.0f, 1.0f, 0.0f, 0.0f );
                Gl.glTranslatef(0.0f,0.0f,1.0f);
                Glu.gluDisk(quadratic,0.0f,0.5f,32,32);
                Gl.glPopMatrix();
        
                Gl.glEndList();
                bCylinderDefined = true;
            }
        
            Gl.glCallList(LISTCYLINDER);
        }
        
        // based on http://nehe.gamedev.net
        public void RenderHeightMap(  int[,] HeightMap, int iMapSize )     // This Renders The Height Map As Quads
        {
            int X = 0, Y = 0;
            int x, y, z;
        
            if( HeightMap == null )
            {
                Test.Debug(  "Error: no height map data available" ); // Test.Debug
                return;
            }
        
            Gl.glBegin( Gl.GL_QUADS );
        
            // Test.Debug(  "drawing quads..." ); // Test.Debug
            for ( X = 2; X < (iMapSize - 3); X += 1 )
            {
                // Test.Debug(  "X " << X ); // Test.Debug
                for ( Y = 2; Y < (iMapSize - 3); Y += 1 )
                {
                    Vector3 Normal = new Vector3();; // = VectorNormals[ X + 128 * Y ];
                    Normal.z = 1;
                    Normal.x = ( HeightMap[ X + 10, Y ] - HeightMap[ X, Y ] ) / 10.0f;
                    Normal.y = ( HeightMap[ X, Y + 10 ] - HeightMap[ X, Y ] ) / 10.0f;
        
                    x = X;
                    y = Y;
                    z = HeightMap[ X, Y ];
                    Gl.glNormal3d( Normal.x, Normal.y, Normal.z);
                    Gl.glTexCoord2d((double)X / (double)iMapSize, (double)Y / (double)iMapSize);
                    Gl.glVertex3i(x, y, z);
        
                    x = X + 1;
                    y = Y;
                    z = HeightMap[ X + 1, Y ];
                    Gl.glNormal3d( Normal.x, Normal.y, Normal.z);
                    Gl.glTexCoord2d((double)(X + 1) / (double)iMapSize, (double)(Y ) / (double)iMapSize);
                    Gl.glVertex3i(x, y, z);
        
                    x = X + 1;
                    y = Y + 1 ;
                    z = HeightMap[ X + 1, Y + 1 ];
                    Gl.glNormal3d( Normal.x, Normal.y, Normal.z);
                    Gl.glTexCoord2d((double)(X + 1) / (double)iMapSize, (double)(Y + 1 ) / (double)iMapSize);
                    Gl.glVertex3i(x, y, z);
        
                    x = X;
                    y = Y + 1 ;
                    z = HeightMap[ X, Y + 1 ];
                    Gl.glNormal3d( Normal.x, Normal.y, Normal.z);
                    Gl.glTexCoord2d((double)(X) / (double)iMapSize, (double)(Y + 1 ) / (double)iMapSize);
                    Gl.glVertex3i(x, y, z);
                }
            }
            //Test.Debug(  "Quads done" << X ); // Test.Debug
            Gl.glEnd();
        }
        
        //int iNextTerrainListNumber = 1001;
        
        public void RenderTerrain( int[,] HeightMap, int iMapSize )
        {
            // Test.Debug(  "RenderTerrain() start..." ); // Test.Debug
            Gl.glPushMatrix();
            Gl.glTranslated( -0.5,-0.5,-0.5 );
            Gl.glScaled( 1 / (double)iMapSize, 1 / (double)iMapSize, 1.0f / 256.0f );
        
            //if( !bTerrainListInitialized )
            // {
            //  iOurTerrainListNumber = iNextTerrainListNumber;
            // iNextTerrainListNumber++;
        
            //   glNewList(iOurTerrainListNumber, GL_COMPILE);
            // Test.Debug(  "rendering height map..." ); // Test.Debug
        
            RenderHeightMap( HeightMap, iMapSize );
            // Test.Debug(  "rendering done" ); // Test.Debug
            //   glEndList();
        
            //  bTerrainListInitialized = true;
            //}
        
            //glCallList(iOurTerrainListNumber);
        
            Gl.glPopMatrix();
        
            //  Test.Debug(  "RenderTerrain() ... done" ); // Test.Debug
        }
        
        public void Translate( double x, double y, double z )
        {
            Gl.glTranslated( x, y, z );
        }
        
        public void Translate( Vector3 pos )
        {
            Gl.glTranslated( pos.x, pos.y, pos.z );
        }
        
        public void Scale( double x, double y, double z )
        {
            Gl.glScaled( x, y, z );
        }
        
        public void Scale( Vector3 scale )
        {
            Gl.glScaled( scale.x, scale.y, scale.z );
        }
        
        public void Bind2DTexture(int iTextureID )
        {
            Gl.glBindTexture( Gl.GL_TEXTURE_2D, iTextureID );
        }
        
        public void PopMatrix()
        {
            Gl.glPopMatrix();
        }
        
        public void PushMatrix()
        {
            Gl.glPushMatrix();
        }
        
        public void LoadMatrix( double[]matrix )
        {
            Gl.glLoadMatrixd( matrix );
        }
        
        public void SetMaterialColor( double[] mcolor)
        {
            Gl.glMaterialfv( Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE,
                new float[]{ (float)mcolor[0],(float)mcolor[1],(float)mcolor[2],(float)mcolor[3] } );
        }
        
        public void SetMaterialColor( Color color )
        {
            Gl.glMaterialfv( Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE,
                new float[]{ (float)color.r,(float)color.g,(float)color.b,(float)color.a } );
        }
        
        public void DrawWireSphere()
        {
            //glutWireSphere(0.5, 32, 32 );
        }
        
        public void RasterPos3f(double x, double y, double z )
        {
            Gl.glRasterPos3f((float)x, (float)y, (float)z );
        }
        
        public void Rotate( double fAngleDegrees, double fX, double fY, double fZ)
        {
            Gl.glRotatef( (float)fAngleDegrees, (float)fX, (float)fY, (float)fZ );
        }
    }
}    
