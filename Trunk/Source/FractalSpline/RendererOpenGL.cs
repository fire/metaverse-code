// Copyright Hugh Perkins 2005,2006,2008
// hughperkins@gmail.com http://manageddreams.com
//
// This library is free software; you can redistribute it and/or modify it 
// under the terms of the GNU Lesser General Public License as published by 
// the Free Software Foundation; either version 2.1 of the License, or 
// (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public 
// License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this library; if not, write to the Free Software 
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA  
using Tao.OpenGl;

namespace FractalSpline
{
    public class RendererOpenGl : IRenderer
    {
        static RendererOpenGl instance = new RendererOpenGl();
        public static RendererOpenGl GetInstance()
        {
            return instance;
        }
        
        public void AddVertex( double x, double y, double z )
        {
            Gl.glVertex3f( (float)x,(float)y,(float)z );
        }
        public void SetNormal( double x, double y, double z )
        {
            Gl.glNormal3f( (float)x,(float)y, (float)z );
        }
        public void SetTextureCoord( double u, double v )
        {
            Gl.glTexCoord2f( (float)u,(float)v );
        }
        public void StartTriangle()
        {
            Gl.glBegin( Gl.GL_TRIANGLES );
        }
        public void EndTriangle()
        {
            Gl.glEnd();
        }
        public void SetTextureId( int iTexture )
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, iTexture );
        }
        public void SetColor( double r, double g, double b )
        {
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE, new float[]{ (float)r,(float)g,(float)b,1.0f} );   
        }
    }
} // namespace FractalSpline

