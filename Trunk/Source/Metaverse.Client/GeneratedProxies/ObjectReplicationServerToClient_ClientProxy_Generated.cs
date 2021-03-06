// *** This is a generated file; if you want to change it, please change the generator or the appropriate interface file
// 
// This file was generated by NetworkProxyBuilder, by Hugh Perkins hughperkins@gmail.com http://manageddreams.com
// 

using System;
using System.Net;

namespace OSMP.NetworkInterfaces
{
public class ObjectReplicationServerToClient_ClientProxy : OSMP.NetworkInterfaces.IObjectReplicationServerToClient
{
   RpcController rpc;
   IPEndPoint connection;

   public ObjectReplicationServerToClient_ClientProxy( RpcController rpc, IPEndPoint connection )
   {
      this.rpc = rpc;
      this.connection = connection;
   }
   public void ObjectCreatedServerToCreatorClient( System.Int32 clientreference, System.Int32 globalreference )
   {
      rpc.SendRpc( connection, "OSMP.NetworkInterfaces.IObjectReplicationServerToClient", "ObjectCreatedServerToCreatorClient",  new object[]{ clientreference, globalreference } );
   }
   public void ObjectCreated( System.Int32 reference, System.String entitytypename, System.Int32 attributebitmap, System.Byte[] entitydata )
   {
      rpc.SendRpc( connection, "OSMP.NetworkInterfaces.IObjectReplicationServerToClient", "ObjectCreated",  new object[]{ reference, entitytypename, attributebitmap, entitydata } );
   }
   public void ObjectModified( System.Int32 reference, System.String entitytypename, System.Int32 attributebitmap, System.Byte[] entitydata )
   {
      rpc.SendRpc( connection, "OSMP.NetworkInterfaces.IObjectReplicationServerToClient", "ObjectModified",  new object[]{ reference, entitytypename, attributebitmap, entitydata } );
   }
   public void ObjectDeleted( System.Int32 reference, System.String entitytypename )
   {
      rpc.SendRpc( connection, "OSMP.NetworkInterfaces.IObjectReplicationServerToClient", "ObjectDeleted",  new object[]{ reference, entitytypename } );
   }
}
}
