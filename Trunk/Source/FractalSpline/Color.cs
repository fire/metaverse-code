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
//

using System;


namespace FractalSpline
{
    //! Color contains a color, in r,g,b format; plus helper functions
    public class Color
    {
        public double r;  //!< red value
        public double g;  //!< green value
        public double b;  //!< blue value
        
        public Color()
        {
        }
        public Color( Color color )
        {
            r = color.r;
            g = color.g;
            b = color.b;
        }
        //! constructs from passed in x,y,z
        public Color( double r, double g, double b )
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
        //! constructs from passed in double[]
        public Color( double[] color )
        {
            this.r = color[0];
            this.g = color[1];
            this.b = color[2];
        }
        public double[]ToArray()
        {
            return new double[]{ r,g,b };
        }
        
        //! writes out to ostream
        public override string ToString()
        {
            return "<color r=\"" + r.ToString() + "\" g=\"" + g.ToString() + "\" b=\"" + b.ToString() + "\"/>";
        }
    }
}
