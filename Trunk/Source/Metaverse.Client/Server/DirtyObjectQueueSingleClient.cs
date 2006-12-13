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
using System.Collections.Generic;

namespace OSMP
{
    // runs on server
    public class DirtyObjectQueueSingleClient
    {
        public object connection;
        
        List<Entity> DirtyEntities = new List<Entity>();

        public DirtyObjectQueueSingleClient(object connection)
        {
            this.connection = connection; 
        }
        
        public void MarkDirty( Entity targetentity )
        {
            if (!DirtyEntities.Contains(targetentity))
            {
                DirtyEntities.Add(targetentity);
            }
            //else
            //{
              //  DirtyEntities[ targetobject ] = ( (int)DirtyEntities[ targetobject ] ) | bitmask;
            //}
        }
        
        public void Tick()
        {
            Queue queue = new Queue();
            foreach (Entity entity in DirtyEntities)
            {
            //    Entity entity = (Entity)dirtyentityobject;
                // can add logic here to decide what objects are priority
                queue.Enqueue( entity );
            }
            for( int i = 0; i < Math.Min( 5, queue.Count ); i++ )
            {
                Entity entity = (Entity)queue.Dequeue();
                {
                   // entity.SendUpdate( connection, DirtyEntries[ entity ] );
                    DirtyEntities.Remove( entity );
                }
            }
        }
    }
}
