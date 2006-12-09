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

namespace OSMP
{
    // runs on server
    public class RemoteClient
    {
        public object connection;
        
        Hashtable DirtyEntities = new Hashtable();
        
        public RemoteClient( object connection )
        {
            this.connection = connection; 
        }
        
        public void MarkDirty( object targetobject, int bitmask )
        {
            if( !DirtyEntities.Contains( targetobject )
            {
                DirtyEntities.Add( targetobject, bitmask );
            }
            else
            {
                DirtyEntities[ targetobject ] = ( (int)DirtyEntities[ targetobject ] ) | bitmask;
            }
        }
        
        public void Tick()
        {
            Queue queue = new Queue();
            foreach( object dirtyentityobject in DirtyEntries )
            {
                Entity entity = (Entity)dirtyentityobject;
                // can add logic here to decide what objects are priority
                queue.Enqueue( entity );
            }
            for( int i = 0; i < Math.Min( 5, queue.Count ); i++ )
            {
                Entity entity = (entity)Queue.Dequeue();
                {
                    entity.SendUpdate( connection, DirtyEntries[ entity ] );
                    DirtyEntities.Remove( entity );
                }
            }
        }
    }
}
