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
//! \brief Contains all the prim/object classes that can be rezed in the world/sim
//!
//! Contains all the prim / object classes that can be rezed in the world/sim, except Terrain which has its own
//! headerfile (Terrain.h)
//!
//! Object is the pure virtual base class of all objects
//! there are two main derivative branches currently : Prim and ObjectGrouping
//! Prims is stuff you can see, like cubes and spheres (when we implement spheres)
//! ObjectGroupings are groups of objects - could be prims or other objectgroupings
//! groupings can be hierarhical
//! currently max 10 objects per group, but this is easily extended (probably use a map class for this)
//!
//! This module uses BasicTypes.h which defines Vector3, Rot, and Color
//!
//! Objects are responsible for drawing themselves when called with Draw()
//! similarly, a lot of database stuff is in the objects themselves when its object-specific
//! (data-oriented architecture)
//!
//! Object contains static pointers to certain objects, stored in interface pointers, which helps
//! reduce the dependencies that a data-centric architecture encourages

//! An avatar is a player.  Its a type of objectgrouping

//! An avatar is a player.  Its a type of objectgrouping, though arguably we could make it a type of prim
//! For various reasons it's an objectgrouping currently, because this means it can be part of a group,
//! or have objects added to it.
//

using System;
using System.Collections;

namespace OSMP
{
    public class Avatar : EntityGroup
    {
        public Avatar()
        {
        }
        public override void Draw()
        {
            //        Debug( "Avatar::DrawEx\n" );
            //      glPrint(avatarname); // Print GL Text To The Screen            
            
            IGraphicsHelper graphics = GraphicsHelperFactory.GetInstance();
            if( graphics == null )
            {
                return;
            }
           
/*            
            graphics.PushMatrix();
            graphics.RasterPos3f( pos.x, pos.y, pos.z + 0.8 );
            graphics.PrintText( name );
            
            graphics.PopMatrix();
            */
            
            base.Draw();
        }
    }
}    
