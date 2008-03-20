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

using System;

namespace FractalSpline
{
    public class Vector2{
        public double x;
        public double y;
            
        public Vector2()
        {
        }
        public Vector2( Vector2 orig )
        {
            x = orig.x;
            y = orig.y;
        }
        public Vector2( double x, double y )
        {
            this.x = x;
            this.y = y;
        }                
        public Vector2( double[]array )
        {
            this.x = array[0];
            this.y = array[1];
        }         
        public double[] ToArray()
        {
            return new double[]{ x,y};
        }
        public override String ToString()
        {
            return "<" + x.ToString() + "," + y.ToString() + ">";
        }
    }
}
