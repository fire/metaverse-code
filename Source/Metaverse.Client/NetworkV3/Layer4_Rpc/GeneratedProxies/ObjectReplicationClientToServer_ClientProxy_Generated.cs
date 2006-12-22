// *** This is a generated file; if you want to change it, please change the generator or the appropriate interface file
// 
// This file was generated by NetworkProxyBuilder, by Hugh Perkins hughperkins@gmail.com http://manageddreams.com
// 

using System;
using System.Net;

namespace OSMP.NetworkInterfaces
{
public class ObjectReplicationClientToServer_ClientProxy : OSMP.NetworkInterfaces.IObjectReplicationClientToServer
{
   RpcController rpc;
   IPEndPoint connection;

   public ObjectReplicationClientToServer_ClientProxy( RpcController rpc, IPEndPoint connection )
   {
      this.rpc = rpc;
      this.connection = connection;
   }
   public void ObjectCreated( System.Int32 remoteclientreference, System.String entitytypename, System.Int32 attributebitmap, System.Byte[] entitydata )
   {
      rpc.SendRpc( connection, "OSMP.NetworkInterfaces.IObjectReplicationClientToServer", "ObjectCreated",  new object[]{ remoteclientreference, entitytypename, attributebitmap, entitydata } );
   }
   public void ObjectModified( System.Int32 reference, System.String entitytypename, System.Int32 attributebitmap, System.Byte[] entitydata )
   {
      rpc.SendRpc( connection, "OSMP.NetworkInterfaces.IObjectReplicationClientToServer", "ObjectModified",  new object[]{ reference, entitytypename, attributebitmap, entitydata } );
   }
   public void ObjectDeleted( System.Int32 reference, System.String entitytypename )
   {
      rpc.SendRpc( connection, "OSMP.NetworkInterfaces.IObjectReplicationClientToServer", "ObjectDeleted",  new object[]{ reference, entitytypename } );
   }
}
}
