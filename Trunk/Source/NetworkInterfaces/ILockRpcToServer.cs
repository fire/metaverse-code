using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkInterfaces
{
    public interface ILockRpcToServer
    {
        void RequestLock(object netconnection, IReplicated targetobject);
        void RenewLock(object netconnection, IReplicated targetobject);
        void ReleaseLock(object netconnection, IReplicated targetobject);
    }
}
