/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 12/28/2006
 * Time: 2:19 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace OSMP.Replication
{
	public class ObjectReplicationServerToClient : NetworkInterfaces.IObjectReplicationServerToClient
    {
        IPEndPoint connection;
        public ObjectReplicationServerToClient(IPEndPoint connection) { this.connection = connection; }

        public void ObjectCreatedServerToCreatorClient(int clientreference, int globalreference )
        {
            MetaverseClient.GetInstance().netreplicationcontroller.ObjectCreatedRpcServerToCreatorClient(connection,
                clientreference,globalreference);
        }

        public void ObjectCreated(int reference, string typename, int attributebitmap, byte[] entity)
        {
            MetaverseClient.GetInstance().netreplicationcontroller.ObjectCreatedRpcServerToClient(connection,
                reference, typename, attributebitmap, entity);
        }

        public void ObjectModified(int reference, string typename, int attributebitmap, byte[] entity)
        {
            MetaverseClient.GetInstance().netreplicationcontroller.ObjectModifiedRpc(connection,
                reference, typename, attributebitmap, entity);
        }

        public void ObjectDeleted(int reference, string typename)
        {
            MetaverseClient.GetInstance().netreplicationcontroller.ObjectDeletedRpc(connection,
                reference, typename );
        }
    }
}
