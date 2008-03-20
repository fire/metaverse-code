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
using MathGl;

namespace FractalSpline
{
    public class LinearExtrusionPath : ExtrusionPath
    {
        public override void UpdatePath()
        {
            iNumberOfTransforms = iLevelOfDetail;
            int i;
            double fRatio;
            for( i = 0; i < iNumberOfTransforms; i++ )
            {
                fRatio = (double)i / (double)( iNumberOfTransforms - 1 );
                transforms[i] = GLMatrix4D.Identity();
                transforms[i].ApplyTranslation( fRatio * fShear, 0, fRatio - 0.5 );
                transforms[i].ApplyScale( 1 + fRatio * (fTopSizeX - 1), 1 + fRatio * ( fTopSizeY - 1 ), 1 );
                transforms[i].applyRotate( fRatio * (double)iTwist, 0, 0, 1 );
            }
        }
    }
}
