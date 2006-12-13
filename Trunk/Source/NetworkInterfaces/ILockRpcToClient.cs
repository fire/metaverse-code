using System;
using System.Collections.Generic;
using System.Text;

namespace OSMP.NetworkInterfaces
{
    public interface ILockRpcToClient
    {
        // send result to ILockConsumer, and add lock to locks
        void LockRequestResponse(object netconnection, IReplicated targetobject, bool LockCreated);

        // if this is one of our locks, signal the ILockConsumer, then mark lock released
        void LockReleased(object netconnection, IReplicated targetobject);
    }
}
