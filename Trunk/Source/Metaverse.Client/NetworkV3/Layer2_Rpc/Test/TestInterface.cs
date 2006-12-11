using System;
using System.Collections.Generic;
using System.Text;

namespace OSMP.Testing
{
    class TestInterface : ITestInterface
    {
        object connection;

        public TestInterface( object connection)
        {
            this.connection = connection;
        }

        public void SayHello()
        {
            TestNetRpc.TestController.GetInstance().SayHello(connection);
        }

        public void SayMessage(string message)
        {
            TestNetRpc.TestController.GetInstance().SayMessage(connection, message);
        }

        public void SendObject(TestClass testobject)
        {
            TestNetRpc.TestController.GetInstance().SendObject(connection, testobject);
        }

    }
}
