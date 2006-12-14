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
using System.Net;

namespace OSMP
{
    // runs on server
    public class DirtyObjectQueueSingleClient
    {
        DirtyObjectController parent;
        public IPEndPoint connection;

        Dictionary<IHasReference, List<Type>> DirtyAttributesByEntity = new Dictionary<IHasReference, List<Type>>();

        public DirtyObjectQueueSingleClient( DirtyObjectController parent, IPEndPoint connection)
        {
            this.parent = parent;
            this.connection = connection;

            // note to self: this is a hack
            foreach (IHasReference entity in MetaverseServer.GetInstance().World.entities)
            {
                DirtyAttributesByEntity.Add(entity, new List<Type>());
                DirtyAttributesByEntity[entity].Add( typeof(Replicate) );
            }
        }

        public void MarkDirty(IHasReference targetentity, Type[] dirtytypes)
        {
            if (!DirtyAttributesByEntity.ContainsKey(targetentity))
            {
                DirtyAttributesByEntity.Add(targetentity, new List<Type>() );
                Console.WriteLine( "dirtyobjectqueuesingleclient " + connection + " marking object " + targetentity + " dirty" );
            }
            foreach( Type type in dirtytypes )
            {
                if( !DirtyAttributesByEntity[ targetentity ].Contains( type ) )
                {
                    DirtyAttributesByEntity[ targetentity ].Add( type );
                }
            }
            //else
            //{
              //  DirtyEntities[ targetobject ] = ( (int)DirtyEntities[ targetobject ] ) | bitmask;
            //}
        }
        
        public void Tick()
        {
            //Console.WriteLine("dirtyobjectqueuesingleclient.Tick()");
            Queue<IHasReference> queue = new Queue<IHasReference>();
            foreach (IHasReference entity in DirtyAttributesByEntity.Keys)
            {
            //    Entity entity = (Entity)dirtyentityobject;
                // can add logic here to decide what objects are priority
                queue.Enqueue( entity );
            }
            for( int i = 0; i < Math.Min( 1, queue.Count ); i++ )
            {
                IHasReference entity = queue.Dequeue();
                {
                   // entity.SendUpdate( connection, DirtyEntries[ entity ] );
                    parent.netreplicationcontroller.ReplicateSingleEntityToSingleClient(
                        connection, entity, DirtyAttributesByEntity[ entity ].ToArray() );
                    DirtyAttributesByEntity.Remove(entity);
                }
            }
        }
    }
}
