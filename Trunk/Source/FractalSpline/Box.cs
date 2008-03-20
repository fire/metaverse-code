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
using System.Collections;
using MathGl;

namespace FractalSpline
{

    public class Box : LinearPrimitive
    {
        // properties

        // constructor(s)
        public Box( IRenderer renderer )
        {
            ReferenceVertices = new GLVector3d[4];
            
            ReferenceVertices[0] = new GLVector3d( 0.5, -0.5, 0 );
            ReferenceVertices[1] = new GLVector3d( 0.5, 0.5, 0 );
            ReferenceVertices[2] = new GLVector3d( -0.5, 0.5, 0 );
            ReferenceVertices[3] = new GLVector3d( -0.5, -0.5, 0 );
            
            iFirstOuterFace = 0;
            iLastOuterFace = 3;
            
            bShowCut = false;
            bShowHollow = false;

            iCutStart = 0;
            iCutEnd = MaxCut;
            
            iNumberFaces = 4;
            
            this.renderer = renderer;
            SendRendererCallbacksToCrossSections();
            
            linearextrusionpath.UpdatePath();
            BuildFaces();
        }
                
        // accessors (get/set)
        
        // public methods
        
        protected override void AssignFaces()
        {
            ArrayList FacesAL = new ArrayList();
            
            for( int i = iFirstOuterFace; i <= iLastOuterFace; i++ )
            {
               // Console.WriteLine( i.ToString() + " " + OuterFaces[i].ToString() );
                int iFaceNum = 0;            
                switch( i )
                {
                    case 0:
                        iFaceNum = 2;
                        break;
                    
                    case 1:
                        iFaceNum = 3;
                        break;
                    
                    case 2:
                        iFaceNum = 4;
                        break;
                    
                    case 3:
                        iFaceNum = 1;
                        break;
                    
                }
                FacesAL.Add( new OuterFace( iFaceNum, i ) );
            }
            
            if( bShowHollow )
            {
                for( int i = iFirstOuterFace; i <= iLastOuterFace; i++ )
                {
                    FacesAL.Add( new InnerFace( 5, i ) );
                }
            }
            
            if( bShowCut )
            {
                int iCutStartFace = 6;
                int iCutEndFace = 7;
                if( bShowHollow )
                {
                    iCutStartFace = 7;
                    iCutEndFace = 8;
                }
                
                FacesAL.Add( new CutFace( iCutStartFace, 0 ) );
                FacesAL.Add( new CutFace( iCutEndFace, 1 ) );
            }
            
            if( !bShowHollow )
            {
                if( !bShowCut )
                {
                    FacesAL.Add( new EndCapNoCutNoHollow( 5, false ) );
                    FacesAL.Add( new EndCapNoCutNoHollow( 0, true ) );
                }
                else
                {
                    FacesAL.Add( new EndCapCutNoHollow( 5, false ) );
                    FacesAL.Add( new EndCapCutNoHollow( 0, true ) );
                }
            }
            else
            {
                FacesAL.Add( new EndCapHollow( 6, false ) );
                FacesAL.Add( new EndCapHollow( 0, true ) );
            }    
            
            Faces = (Face[])FacesAL.ToArray( typeof( Face ) );
        }
        
        // abstract protected / private methods
        
        // protected/private methods
        
        protected override int GetCutQuadrant( int iCut )
        {
            return ( ( ( iCut * 4 ) / MaxCut % 4 ) + 4 ) % 4;
        }
        
        // should return angle in radians for a given cut ratio (?)
        protected override double GetAngleWithXAxis( double fCutRatio )
        {
            return ( fCutRatio - 0.125 )  * 2 * Math.PI;
        }
    }
}
