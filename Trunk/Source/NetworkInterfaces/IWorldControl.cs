using System;
using System.Collections.Generic;
using System.Text;

namespace OSMP.NetworkInterfaces
{
    [AuthorizedRpcInterface]
    public interface IWorldControl
    {
        // from client to server; request whole world to be resent
        // eg following reconnect
        void RequestResendWorld();
    }
}
