/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 12/28/2006
 * Time: 2:16 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Net;

namespace OSMP
{
	public class ObjectReplicationClientToServer : NetworkInterfaces.IObjectReplicationClientToServer
    {
        IPEndPoint connection;
        public ObjectReplicationClientToServer(IPEndPoint connection) { this.connection = connection; }

        public void ObjectCreated( int remoteclientreference, string typename, int attributebitmap, byte[] entitydata )
        {
            MetaverseServer.GetInstance().netreplicationcontroller.ObjectCreatedRpcClientToServer(connection,
                remoteclientreference, typename, attributebitmap, entitydata );
        }

        public void ObjectModified( int reference, string typename, int attributebitmap, byte[]entity )
        {
            MetaverseServer.GetInstance().netreplicationcontroller.ObjectModifiedRpc(connection,
                reference, typename, attributebitmap, entity);
        }

        public void ObjectDeleted(int reference, string typename)
        {
            MetaverseServer.GetInstance().netreplicationcontroller.ObjectDeletedRpc(connection,
                reference, typename );
        }
    }
	
	
}
