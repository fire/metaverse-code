﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OSMP.NetworkInterfaces
{
    public interface IObjectReplicationClientToServer
    {
        void ObjectCreated( int remoteclientreference, string entitytypename, int attributebitmap, byte[] entitydata);
        void ObjectModified(int reference, string entitytypename, object entity);
        void ObjectDeleted( int reference);
    }
    public interface IObjectReplicationServerToClient
    {
        void ObjectCreatedServerToCreatorClient(int clientreference, int globalreference);
        void ObjectCreated(int reference, string entitytypename, object entity);
        void ObjectModified(int reference, string entitytypename, object entity);
        void ObjectDeleted(int reference);
    }
}
