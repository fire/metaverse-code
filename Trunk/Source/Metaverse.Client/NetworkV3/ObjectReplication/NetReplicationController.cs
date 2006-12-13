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
using System.Collections.Generic;
//using System.Windows.Forms;
using System.Text;

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
    
    public delegate void ObjectCreatedHandler( object source, ObjectCreatedArgs e );
    public delegate void ObjectModifiedHandler( object source, ObjectModifiedArgs e );
    public delegate void ObjectDeletedHandler( object source, ObjectDeletedArgs e );
        
    // derive from this on a replicated object controller that wants to create replication references itself
    // this will be the case forworldmodel/controller for example, in order to allow replicated object caching
    // note to self: perhaps caching can be done by the replicationcontroller???
    // so, do we really need this???
    public interface IHasReference
    {
        int Reference { get; set; }
    }
    
    // derive from ths on any classes that manage replicated objects
    // for example, worldmodel(worldcontroller?) derives from this
    // you also need to get the replicatedobjectcaller to call NetReplicationController.GetInstance().RegisterReplicatedObjectController()
    public interface IReplicatedObjectController
    {
        event ObjectCreatedHandler ObjectCreated;
        event ObjectModifiedHandler ObjectModified;
        event ObjectDeletedHandler ObjectDeleted;

        void ReplicatedObjectCreated( object notifier, ObjectCreatedArgs e );
        void ReplicatedObjectModified( object notifier, ObjectModifiedArgs e );
        void ReplicatedObjectDeleted( object notifier, ObjectDeletedArgs e );        
    }

    public interface IReferenceGenerator
    {
        int GenerateReference();
    }
    
    public class ObjectReplicationClientToServer : NetworkInterfaces.IObjectReplicationClientToServer
    {
        object connection;
        public ObjectReplicationClientToServer(object connection) { this.connection = connection; }

        public void ObjectCreated( int remoteclientreference, string typename, int attributebitmap, byte[] entitydata )
        {
            MetaverseServer.GetInstance().netreplicationcontroller.ObjectCreatedRpcClientToServer(connection,
                remoteclientreference, typename, attributebitmap, entitydata );
        }

        public void ObjectModified( int reference, string typename, object entity )
        {
            MetaverseServer.GetInstance().netreplicationcontroller.ObjectModifiedRpc(connection,
                reference, typename, entity);
        }

        public void ObjectDeleted( int reference)
        {
            MetaverseServer.GetInstance().netreplicationcontroller.ObjectDeletedRpc(connection,
                reference );
        }
    }

    public class ObjectReplicationServerToClient : NetworkInterfaces.IObjectReplicationServerToClient
    {
        object connection;
        public ObjectReplicationServerToClient(object connection) { this.connection = connection; }

        public void ObjectCreatedServerToCreatorClient(int clientreference, int globalreference )
        {
            MetaverseClient.GetInstance().netreplicationcontroller.ObjectCreatedRpcServerToCreatorClient(connection,
                clientreference,globalreference);
        }

        public void ObjectCreated(int reference, string typename, object entity)
        {
            MetaverseClient.GetInstance().netreplicationcontroller.ObjectCreatedRpcServerToClient(connection,
                reference, typename, entity);
        }

        public void ObjectModified(int reference, string typename, object entity)
        {
            MetaverseClient.GetInstance().netreplicationcontroller.ObjectModifiedRpc(connection,
                reference, typename, entity);
        }

        public void ObjectDeleted(int reference)
        {
            MetaverseClient.GetInstance().netreplicationcontroller.ObjectDeletedRpc(connection,
                reference );
        }
    }

    // on client, receives replications from network, creates or modifies the object, if it's a new object,  and creates events for this
    // doesnt delete the object if a deletion is received, since theres no way to delete it; instead notifies observers to delete their own reference of it
    //
    // on server, sends out objects as appropriate
    public class NetReplicationController
    {
        public RpcController rpc;
        public NetObjectReferenceController referencecontroller;

        bool ismaster = false;
        Dictionary<Type,IReplicatedObjectController> objectcontrollers = new Dictionary<Type,IReplicatedObjectController>();
        Dictionary<int, object> tempreferences = new Dictionary<int,object>();
        
        //static NetReplicationController instance = new NetReplicationController();
        //public static NetReplicationController GetInstance(){ return instance; }
        
        public NetReplicationController( RpcController rpc )
        {
            this.rpc = rpc;
            referencecontroller = new NetObjectReferenceController();
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
            if( objectcontrollers.ContainsKey( managedtype ) && objectcontrollers[ managedtype ] != controller )
            {
                throw new Exception( "Error: a replicated type can only be registered by a single replicatedobjectcontroller.  " + 
                    "Duplicated type: " + managedtype.ToString() + " by " + controller.ToString() + " conflicts with " + objectcontrollers[ managedtype ].ToString() );
            }
            if( !objectcontrollers.ContainsKey( managedtype ) )
            {
                objectcontrollers.Add( managedtype, controller );
                controller.ObjectCreated += new ObjectCreatedHandler(controller_ObjectCreated);
                controller.ObjectDeleted += new ObjectDeletedHandler(controller_ObjectDeleted);
                controller.ObjectModified += new ObjectModifiedHandler(controller_ObjectModified);
            }
        }

        // events for incoming changes from object controllers
        void controller_ObjectModified(object source, ObjectModifiedArgs e)
        {
        }

        void controller_ObjectDeleted(object source, ObjectDeletedArgs e)
        {
        }

        void controller_ObjectCreated(object source, ObjectCreatedArgs e)
        {
            Console.WriteLine("netreplicationcontroller ObjectCreated " + e.TargetObject);
            if( this.rpc.IsServer )
            {
                //NetworkInterfaces.ObjectReplicationServerToClient_ClientProxy objectreplicationproxy = new OSMP.NetworkInterfaces.ObjectReplicationServerToClient_ClientProxy( rpc, 
                // handled by something like DirtyCacheController
            }
            else
            {
                int tempreference = GenerateTempReference();
                tempreferences.Add( tempreference, e.TargetObject );

                Type[]AttributeTypeList = new Type[]{ typeof( Replicate ) };
                int bitmap = new ReplicateAttributeHelper().TypeArrayToBitmap( AttributeTypeList );
                BinaryPacker binarypacker = new BinaryPacker();
                binarypacker.allowedattributes = AttributeTypeList;
                byte[] entitydata = new byte[4096];
                int nextposition = 0;
                binarypacker.WriteValueToBuffer(entitydata, ref nextposition, e.TargetObject);

                byte[]entitydatatotransmit = new byte[ nextposition ];
                Buffer.BlockCopy( entitydata,0, entitydatatotransmit, 0, nextposition );

                Console.WriteLine(Encoding.UTF8.GetString(entitydatatotransmit));

                NetworkInterfaces.ObjectReplicationClientToServer_ClientProxy objectreplicationproxy = new OSMP.NetworkInterfaces.ObjectReplicationClientToServer_ClientProxy(rpc, null);
                objectreplicationproxy.ObjectCreated(tempreference, e.TargetObject.GetType().ToString(), bitmap, entitydatatotransmit );
            }
        }

        int nextreference = 1;
        int GenerateTempReference()
        {
            nextreference++;
            return nextreference - 1;
        }

        // incoming rpc calls
        // ===================

        public void ObjectCreatedRpcClientToServer(object connection, int remoteclientreference, string typename, int attributebitmap, byte[] entitydata )
        {
            Console.WriteLine("ObjectCreatedRpcClientToServer " + typename );

            Console.WriteLine(Encoding.UTF8.GetString(entitydata));

            // note to self: should probably make a whitelist of allowed typenames, maybe via primitive class registration
            Type newtype = Type.GetType(typename);
            object newobject = Activator.CreateInstance(newtype);

            IReplicatedObjectController replicatedobjectcontroller = null;
            foreach (Type replicatedtype in objectcontrollers.Keys)
            {
                if (replicatedtype.IsInstanceOfType(newobject))
                {
                    replicatedobjectcontroller = objectcontrollers[replicatedtype];
                }
            }

            List<Type> AttributeTypeList = new ReplicateAttributeHelper().BitmapToAttributeTypeArray(attributebitmap);
            BinaryPacker binarypacker = new BinaryPacker();
            binarypacker.allowedattributes = AttributeTypeList.ToArray();

            int nextposition = 0;
            newobject = binarypacker.ReadValueFromBuffer(entitydata, ref nextposition, newtype );
            Console.WriteLine("server received replicated object: " + newobject);

            int newobjectreference = 0;
            if (replicatedobjectcontroller != null && replicatedobjectcontroller is IReferenceGenerator)
            {
                newobjectreference = (replicatedobjectcontroller as IReferenceGenerator).GenerateReference();
            }
            else
            {
                newobjectreference = nextreference;
                nextreference++;
            }

            Console.WriteLine("New object reference: " + newobjectreference);
            if( newobject is IHasReference )
            {
                ( newobject as IHasReference ).Reference = newobjectreference;
            }

            if (replicatedobjectcontroller != null)
            {
                replicatedobjectcontroller.ReplicatedObjectCreated(this,
        new ObjectCreatedArgs(DateTime.Now, newobject));
            }

            //new ObjectReplicationServerToClient( connection ).ObjectCreatedServerToCreatorClient(
        }

        public void ObjectCreatedRpcServerToCreatorClient(object connection, int clientreference, int globalreference)
        {
            Console.WriteLine("ObjectCreatedRpcServerToCreatorClient " + clientreference + " -> " + globalreference);
            object thisobject = this.tempreferences[clientreference];
            if (thisobject is IHasReference)
            {
                (thisobject as IHasReference).Reference = globalreference;
            }
            tempreferences.Remove(clientreference);
        }

        public void ObjectCreatedRpcServerToClient(object connection, int reference, string typename, object entity )
        {
            Console.WriteLine("ObjectCreatedRpcServerToClient " + entity);
        }

        public void ObjectModifiedRpc(object connection, int reference, string typename, object entity)
        {
            Console.WriteLine("ObjectModifiedRpc " + entity);
        }

        public void ObjectDeletedRpc(object connection, int reference)
        {
            Console.WriteLine("ObjectDeletedRpc " + reference);
        }        

        // tick is going to prepare/send some packets to replicate dirty/new objects
        public void Tick()
        {
        }
    }
}
