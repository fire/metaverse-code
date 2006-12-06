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
using System.Windows.Forms;

namespace OSMP
{
    public class ObjectCreatedArgs : EventArgs
    {
        public DateTime TimeStamp;
        public object TargetObject;
        public ObjectCreatedArgs( DateTime timestamp, object TargetObject)
        {
            TimeStamp = timestamp;
            this.TargetObject = TargetObject;
        }
    }
    
    public class ObjectModifiedArgs : EventArgs
    {
        public DateTime TimeStamp;
        public object TargetObject;
        public int ModificationBitmask;
        public ObjectModifiedArgs( DateTime timestamp, object TargetObject, int ModificationBitmask )
        {
            TimeStamp = timestamp;
            this.TargetObject = TargetObject;
            this.ModificationBitmask = ModificationBitmask;
        }
    }
    
    public class ObjectDeletedArgs : EventArgs
    {
        public DateTime TimeStamp;
        public object TargetObject;
        public ObjectDeletedArgs( DateTime timestamp, object TargetObject )
        {
            TimeStamp = timestamp;
            this.TargetObject = TargetObject;
        }
    }
    
    /*
    public delegate void ObjectCreatedHandler( object source, ObjectCreatedArgs e );
    public delegate void ObjectModifiedHandler( object source, ObjectModifiedArgs e );
    public delegate void ObjectDeletedHandler( object source, ObjectDeletedArgs e );
    */
    
    // replicated objects should derive from this
    // just makes typing a little stronger sometimes (eg in LockController class)
    public interface IReplicated
    {
    }
      
    // derive from this on a replicated object controller that wants to create replication references itself
    // this will be the case forworldmodel/controller for example, in order to allow replicated object caching
    // note to self: perhaps caching can be done by the replicationcontroller???
    // so, do we really need this???
    public interface IHasReference
    {
        int Reference{ get; }
    }
    
    // derive from ths on any classes that manage replicated objects
    // for example, worldmodel(worldcontroller?) derives from this
    // you also need to get the replicatedobjectcaller to call NetReplicationController.GetInstance().RegisterReplicatedObjectController()
    public interface IReplicatedObjectController
    {
        void ReplicatedObjectCreated( object notifier, ObjectCreatedArgs e );
        void ReplicatedObjectModified( object notifier, ObjectModifiedArgs e );
        void ReplicatedObjectDeleted( object notifier, ObjectDeletedArgs e );        
    }
    
    public interface IReplicationController
    {
        void ObjectCreated( object notifier, ObjectCreatedArgs e );
        void ObjectModified( object notifier, ObjectModifiedArgs e );
        void ObjectDeleted( object notifier, ObjectDeletedArgs e );        
    }

    public interface INetReplicationControllerRpc
    {
        void ObjectCreatedRpc( string typename, int remoteclientreference, byte[]packedobject );
        void ObjectModifiedRpc( string typename, int reference, int changemask, byte[]packedobject );
        void ObjectDeletedRpc( string typename, int reference );
    }
    
    // on client, receives replications from network, creates or modifies the object, if it's a new object,  and creates events for this
    // doesnt delete the object if a deletion is received, since theres no way to delete it; instead notifies observers to delete their own reference of it
    //
    // on server, sends out objects as appropriate
    public class NetReplicationController : INetReplicationControllerRpc, IReplicationController
    {
        bool ismaster = false;
        NetObjectReferenceController referencecontroller;
        Hashtable objectcontrollers = new Hashtable();
        
        static NetReplicationController instance = new NetReplicationController();
        public static NetReplicationController GetInstance(){ return instance; }
        
        public NetReplicationController()
        {
            referencecontroller = NetObjectReferenceController.GetInstance();
        }
        
        public bool IsMaster
        {
            get{ return ismaster; }
            set{
                ismaster = value;
                Console.WriteLine( this.GetType().ToString() + " ismaster set to " + ismaster.ToString() );
            }
        }
        
        // called by object replication controllers, who typically pass themselves in as the first argument,
        // and the type of the entity they manage, and want to receive events for
        // for now, only one controller can register for each type
        // note that for class hierarchies, only the base type needs to be registered
        public void RegisterReplicatedObjectController( IReplicatedObjectController controller, Type managedtype )
        {
            if( objectcontrollers.Contains( managedtype ) && objectcontrollers[ managedtype ] != controller )
            {
                throw new Exception( "Error: a replicated type can only be registered by a single replicatedobjectcontroller.  " + 
                    "Duplicated type: " + managedtype.ToString() + " by " + controller.ToString() + " conflicts with " + objectcontrollers[ managedtype ].ToString() );
            }
            if( !objectcontrollers.Contains( managedtype ) )
            {
                objectcontrollers.Add( managedtype, controller );
            }
        }

        // events for incoming changes from object controllers
        public void ObjectCreated( object controller, ObjectCreatedArgs e )
        {
        }
        
        public void ObjectModified( object controller, ObjectModifiedArgs e )
        {
        }
        
        public void ObjectDeleted( object controller, ObjectDeletedArgs e )
        {
        }

        // rpcmethods
        // ==========
        
        [BidirectionalRpc]
        public void ObjectCreatedRpc( string typename, int remoteclientreference, byte[]packedobject )
        {
        }
        
        [BidirectionalRpc]
        public void ObjectModifiedRpc( string typename, int reference, int changemask, byte[]packedobject )
        {
        }
        
        [BidirectionalRpc]
        public void ObjectDeletedRpc( string typename, int reference )
        {
        }        

        // tick is going to prepare/send some packets to replicate dirty/new objects
        public void Tick()
        {
        }
    }
}
