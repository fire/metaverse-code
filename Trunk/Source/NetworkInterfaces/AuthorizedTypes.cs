using System;
using System.Collections.Generic;
using System.Text;

namespace OSMP.NetworkInterfaces
{
    public class AuthorizedTypes
    {
        static AuthorizedTypes instance = new AuthorizedTypes();
        public static AuthorizedTypes GetInstance() { return instance; }

        List<Type> authorizedtypes = new List<Type>();

        AuthorizedTypes()
        {
            authorizedtypes.Add(typeof(Testing.ITestInterface));

            authorizedtypes.Add(typeof(OSMP.NetworkInterfaces.ILockRpcToClient));
            authorizedtypes.Add(typeof(OSMP.NetworkInterfaces.ILockRpcToServer));
            authorizedtypes.Add(typeof(OSMP.NetworkInterfaces.IObjectReplicationClientToServer));
            authorizedtypes.Add(typeof(OSMP.NetworkInterfaces.IObjectReplicationServerToClient));
        }

        // This is for security, to prevent clients instantiating objects like Activator, etc...
        // RpcController consults this list, and only instantiates classes it finds here
        public List<Type> AuthorizedTypeList
        {
            get
            {
                return authorizedtypes;
            }
        }
    }
}
