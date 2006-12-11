// Copyright Hugh Perkins 2006
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
using System.Collections;
using Tao.OpenGl;
using System.Runtime.InteropServices;

namespace OSMP
{
    public delegate void DrawWorldHandler();
    
    class AdditionalGlWrap
    {
        [DllImport ("glselectwrap.dll", CharSet=CharSet.Auto )]
        public static extern void glCreateSelectBuffer();
            
        [DllImport ("glselectwrap.dll", CharSet=CharSet.Auto )]
        public static extern int glGetNearestBufferName( int numhits );
    }
    
    // responsible for picking, which in OpenGl means essentially using a glSelect buffer to decide what you clicked on.
    // This is the OpenGl specific class; you can derive others from IPicker3dModel
    // You can get an instance of this class at runtime by doing RendererFactory().GetInstance().GetPicker3dModel();
    class Picker3dModelGl : IPicker3dModel
    {
        ArrayList hittargets = new ArrayList();  // ArrayList is not really fast; might consider using a normal array
        
        bool bAddingNames; // we set this to true if we are adding names to hittargets, otherwise to false to gain speed
        
        public void AddHitTarget( HitTarget hittarget )
        {
            if( bAddingNames )
            {
                Test.Debug("adding name " + hittarget.ToString() );
                hittargets.Add( hittarget );
                Gl.glLoadName( hittargets.Count );  // note: this isnt quite the index; it is index + 1
            }
        }
        
        public class WorldDrawer : IRenderable
        {
            public void Render()
            {
                RendererFactory.GetInstance().DrawWorld();
            }
        }
        
        public HitTarget GetClickedHitTarget( int MouseX, int MouseY )
        {
            return GetClickedHitTarget( new WorldDrawer(), MouseX, MouseY );
        }

        // dependencies:
        // - RendererFactory.GetInstance()
        // - Tao.OpenGl        
        public HitTarget GetClickedHitTarget( IRenderable renderable, int MouseX, int MouseY )
        {
            IRenderer renderer = RendererFactory.GetInstance();
            ArrayList results = new ArrayList();
            
            int[] viewport = new int[ 4 ];
            Gl.glGetIntegerv( Gl.GL_VIEWPORT, viewport );
            AdditionalGlWrap.glCreateSelectBuffer();
        
            // This Creates A Matrix That Will Zoom Up To A Small Portion Of The Screen, Where The Mouse Is.
            Gl.glMatrixMode( Gl.GL_PROJECTION );        
            Gl.glPushMatrix();   // save old matrix, we restore it at end         
            Gl.glLoadIdentity();        
            Glu.gluPickMatrix( (float)MouseX, (float) (renderer.WindowHeight - MouseY ), 1.0f, 1.0f, viewport);        
            Glu.gluPerspective(45.0f, (float)renderer.WindowWidth / (float)renderer.WindowHeight, 0.5f, 100.0f);
            
            Gl.glMatrixMode(Gl.GL_MODELVIEW);   
            
            Gl.glRenderMode( Gl.GL_SELECT );
            Gl.glInitNames();
            Gl.glPushName(0);            // Push one entry onto the stack; we will use LoadName to change this value throughout the rendering
            
            bAddingNames = true;
            hittargets = new ArrayList();
            renderable.Render();            
            bAddingNames = false;
            
            // return projection matrix to normal
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();        
            Gl.glMatrixMode(Gl.GL_MODELVIEW);                    
            
            int iNumHits = Gl.glRenderMode( Gl.GL_RENDER );    
            
            if( iNumHits == 0 )
            {
                return null;
            }
            int hitname = AdditionalGlWrap.glGetNearestBufferName( iNumHits );
            Console.WriteLine( "hitname: " + hitname.ToString() );
            if( hitname == -1 )
            {
                return null;
            }
            
            return (HitTarget)hittargets[ hitname - 1 ];
        }        
    }
}