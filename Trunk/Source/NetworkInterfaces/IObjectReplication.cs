using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkInterfaces
{
    public interface IObjectReplicationClientToServer
    {
        void ObjectCreated( int remoteclientreference, string entitytypename, object entity);
        void ObjectModified(int reference, string entitytypename, object entity);
        void ObjectDeleted( int reference);
    }
    public interface IObjectReplicationServerToClient
    {
        void ObjectCreated(int reference, string entitytypename, object entity);
        void ObjectModified(int reference, string entitytypename, object entity);
        void ObjectDeleted(int reference);
    }
}
