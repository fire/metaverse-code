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
    // handles converting face coordinates into texture coordinates
    // primary properties are: Scale, Offset, Rotate
    //
    // PretransformOffsetX and PreTransformScaleX are for use in inner faces, where the texture must span multiple faces
    public class TextureMapping
    {
        public Vector2 Scale;
        public Vector2 Offset;
        public double Rotate; // degrees
            
        public double PreTransformOffsetX = 0;
        public double PreTransformScaleX = 1;
        
        public TextureMapping()
        {
            Offset = new Vector2( 0, 0 );
            Scale = new Vector2( 1, 1 );
            Rotate = 0;
        }
        public TextureMapping( TextureMapping texturemapping )
        {
            Scale = new Vector2( texturemapping.Scale ); Offset = new Vector2( texturemapping.Offset );
            Rotate = texturemapping.Rotate;
            PreTransformOffsetX = texturemapping.PreTransformOffsetX;
            PreTransformScaleX = texturemapping.PreTransformScaleX;
        }

        public TextureMapping( Vector2 Scale, Vector2 Offset, double Rotate )
        {
            this.Scale = Scale; this.Offset = Offset; this.Rotate = Rotate;
        }
        
        // This was created for the inner faces of a hollow object.  In the inner faces of a hollow object, a single texture spans all inner faces
        double XPreTransform( double rawfacex )
        {
            return rawfacex / PreTransformScaleX + PreTransformOffsetX;
        }
        
        public Vector2 GetTextureCoordinate( Vector2 facecoordinate )
        {
            double radianrotate = Rotate * Math.PI / 180;
            Vector2 result = new Vector2();
            result.x = ( ( XPreTransform( facecoordinate.x ) - 0.5 ) * Math.Cos( radianrotate ) + ( facecoordinate.y - 0.5 ) * Math.Sin( radianrotate ) )
                / Scale.x + 0.5 + Offset.x;
            result.y = ( - ( XPreTransform( facecoordinate.x ) - 0.5 ) * Math.Sin( radianrotate ) + ( facecoordinate.y - 0.5 ) * Math.Cos( radianrotate ) )
                / Scale.y + 0.5 + Offset.y;
            
            return result;
        }
        
        public override string ToString()
        {
            return "TextureMapping rotate=" + Rotate.ToString() + " offset=" + Offset.ToString() + " scale=" + Scale.ToString();
        }
    }
}
