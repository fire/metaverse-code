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
//! \brief mvworldstorage is the class used to store the world associated with one server

//! mvworldstorage is the class used to store the world associated with one server
//! including all the objects in it and the avatars (which are just objects)
//! mvworldstorage is a container for objects of type Entity and their derivatives, such as Prims and Cubes
//! mvworldstorage contains functions for managing the objects in the world: creating, modifying, deleting.
//! It is capable of taking XML documents as input for object creation/update
//!
//! the mvWorldStorage class does the following:
//! - contains an array of EntityS, which is basically eveyting in the sim, including avatars, but not
//! including hte current (temporary) hardcoded platform
//! - contains functions for managing these objects:
//!    - adding them
//!    - updating them
//!     - deleting them
//!    - unlinking them
//!
//! This is a common class used by metaverseserver, rendererglutsplit, and the scirpting engines
//!
//! Note that there are two ways of identifying an object:
//! - the global and unique iReference value generated by the database; this is fixed fo rht life of an object
//! - the position in the entities array (iArrayNum).  IMPORTANT: this can change throughout the life of an object
//! Current best method to access an object is via its iReference by calling GetEntityByReference(iReference) and checking
//! result is not null
//!

//! mvworldstorage is the class used to store the world associated with one server

//! mvworldstorage is the class used to store the world associated with one server
//! including all the objects in it and the avatars (which are just objects)
//! mvworldstorage is a container for objects of type Entity and their derivatives, such as Prims and Cubes
//! mvworldstorage contains functions for managing the objects in the world: creating, modifying, deleting.
//!
//! the mvWorldStorage class does the following:
//! - contains an array of EntityS, which is basically eveyting in the sim, including avatars, but not
//! including hte current (temporary) hardcoded platform
//! - contains functions for managing these objects:
//!    - adding them
//!    - updating them
//!     - deleting them
//!
//! Note that there are two ways of identifying an object:
//! - the global and unique iReference value generated by the database; this is fixed fo rht life of an object
//! - the position in the entities array (iArrayNum).  IMPORTANT: this can change throughout the life of an object
//!

// This class handles firing events for delete/creation. Question: how do we handle events to do with object modification???
// Maybe just have an OnModify event???
// Get Entities to tell us???

using System;
using System.Collections;

namespace OSMP
{ 
    public class CreateEntityEventArgs : EventArgs
    {
        public Entity entity;
        public CreateEntityEventArgs( Entity entity )
        {
            this.entity = entity;
        }
    }

    public class DeleteEntityEventArgs : EventArgs
    {
        public Entity entity;
        public DeleteEntityEventArgs( Entity entity )
        {
            this.entity = entity;
        }
    }

    public class ModifyEntityEventArgs : EventArgs
    {
        public Entity oldEntity;
        public Entity newEntity;
        public ModifyEntityEventArgs( Entity oldentity, Entity newentity )
        {
            this.oldEntity = oldentity;
            this.newEntity = newentity;
        }
    }

    public delegate bool BeforeCreateHandler( object source, CreateEntityEventArgs e );
    public delegate bool BeforeDeleteHandler( object source, DeleteEntityEventArgs e );
    public delegate bool BeforeModifyHandler( object source, ModifyEntityEventArgs e );
    
    public delegate void AfterCreateHandler( object source, CreateEntityEventArgs e );
    public delegate void AfterDeleteHandler( object source, DeleteEntityEventArgs e );
    public delegate void AfterModifyHandler( object source, ModifyEntityEventArgs e );

    public delegate void ClearHandler( object source );
    
    public class WorldModel
    {
        public event BeforeCreateHandler BeforeCreate;
        public event BeforeDeleteHandler BeforeDelete;
        public event BeforeModifyHandler BeforeModify;
            
        public event AfterCreateHandler AfterCreate;
        public event AfterDeleteHandler AfterDelete;
        public event AfterModifyHandler AfterModify;

        public event ClearHandler ClearEvent;
        
        public EntityArrayList entities;
            
        static WorldModel instance = new WorldModel(); // This is for deserialization (eg load a world from xml).  Not for normal use.
        public static WorldModel GetInstance()
        {
            return instance;
        }
        
        public WorldModel()
        {
            entities = new EntityArrayList();
            //NetReplicationController.GetInstance().RegisterReplicatedObjectController( this, typeof( Entity ) );
        }
        
        // incoming event from NetReplicationController:
        //public void ReplicatedObjectCreated( object notifier, ObjectCreatedArgs e )
        //{
        //}
        
        // incoming event from NetReplicationController:
        //public void ReplicatedObjectModified( object notifier, ObjectModifiedArgs e )
        //{
        //}
        
        // incoming event from NetReplicationController:
        //public void ReplicatedObjectDeleted( object notifier, ObjectDeletedArgs e )
        //{
        //}
        
        public Entity GetEntityByReference( int iReference )
        {
            for( int i = 0; i < entities.Count; i++ )
            {
                if( entities[i].iReference == iReference )
                {
                    return entities[i];
                }
            }
            return null;
        }
        
        // fires off events, then does the actual delete
        public bool DeleteEntity( Entity entity )
        {
            bool bDeleteApproved = true;
            if( BeforeDelete != null )
            {
                foreach( BeforeDeleteHandler beforedeletecallback in BeforeDelete.GetInvocationList() )
                {
                    if( !beforedeletecallback( this, new DeleteEntityEventArgs( entity ) ) )
                    {
                        bDeleteApproved = false;
                    }
                }
            }
            
            if( !bDeleteApproved )
            {
                return false;
            }
            
            entities.Remove( entity );
            
            if( AfterDelete != null )
            {
                AfterDelete( this, new DeleteEntityEventArgs( entity ) );
            }
            
            return true;
        }
        
        // fires events, then calls __AddEntity to do the actual add, if not cancelled by an event callback
        public bool AddEntity( Entity entity )
        {
            bool bAddApproved = true;
            if( BeforeDelete != null )
            {
                foreach( BeforeCreateHandler beforecreatecallback in BeforeCreate.GetInvocationList() )
                {
                    if( !beforecreatecallback(  this, new CreateEntityEventArgs( entity ) ) )
                    {
                        bAddApproved = false;
                    }
                }
            }
            
            if( !bAddApproved )
            {
                return false;
            }
            
            entities.Add( entity );
            
            if( AfterDelete != null )
            {
                AfterDelete(  this, new DeleteEntityEventArgs( entity ) );
            }
            
            return true;
        }
        
        public void DumpWorld()
        {
            for( int i = 0; i < entities.Count; i++ )
            {
                Test.Debug( i.ToString() + ": " + entities[i].ToString() );
            }
        }
        
        public void Clear()
        {
            if( ClearEvent != null )
            {
                ClearEvent( this );
            }
            entities.Clear();
        }
    }
}

