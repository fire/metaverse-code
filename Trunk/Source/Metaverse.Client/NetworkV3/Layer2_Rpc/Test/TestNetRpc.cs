// Copyright Hugh Perkins 2006
// hughperkins@gmail.com http://manageddreams.com
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License version 2 as published by the
// Free Software Foundation;
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
//  more details.
//
// You should have received a copy of the GNU General Public License along
// with this program in the file licence.txt; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-
// 1307 USA
// You can find the licence also on the web at:
// http://www.opensource.org/licenses/gpl-license.php
//

using System;
using System.Collections;
using System.Threading;

namespace OSMP
{
    public class TestNetRpc
    {
        public class TestController
        {
            static TestController instance = new TestController();
            public static TestController GetInstance() { return instance; }

            public RpcController rpc;

            TestController()
            {

            }

            public void SayHello( object connection )
            {
                Console.WriteLine("Hello!");
                Testing.TestInterface_ClientProxy testinterfaceclientproxy = new OSMP.Testing.TestInterface_ClientProxy( rpc, connection );
                testinterfaceclientproxy.SayMessage("Hello sent via SayMessage!");
            }

            public void SayMessage(object connection, string message)
            {
                Console.WriteLine("Message: " + message);
                Testing.TestInterface_ClientProxy testinterfaceclientproxy = new OSMP.Testing.TestInterface_ClientProxy(rpc, connection);
                Testing.TestClass testobject = new OSMP.Testing.TestClass();
                testobject.name = "blue river";
                testobject.indexes = new int[0];
                testobject.childclass = new OSMP.Testing.ChildClass();
                testinterfaceclientproxy.SendObject(testobject);
            }

            public void SendObject(object connection, Testing.TestClass testobject)
            {
                Console.WriteLine("testobject name: " + testobject.name);
            }
        }

        class TestNetRpcClient
        {
            INetworkImplementation networkimplementation;
            string serveraddress = "127.0.0.1";
            int serverport = 3000;

            public void Go()
            {
                networkimplementation = NetworkImplementationFactory.CreateNewInstance();
                networkimplementation.ConnectAsClient(serveraddress, serverport);

                RpcController rpc = new RpcController( networkimplementation );
                TestController.GetInstance().rpc = rpc;
                Testing.TestInterface_ClientProxy testinterface_clientproxy = new OSMP.Testing.TestInterface_ClientProxy(rpc, null);
                testinterface_clientproxy.SayHello();

                while (true)
                {
                    networkimplementation.Tick();
                    Thread.Sleep(50);
                }
            }
        }

        class TestNetRpcServer
        {
            INetworkImplementation networkimplementation;
            int serverport = 3000;

            public void Go()
            {
                networkimplementation = NetworkImplementationFactory.CreateNewInstance();
                networkimplementation.ListenAsServer(serverport);

                RpcController rpc = new RpcController(networkimplementation);
                TestController.GetInstance().rpc = rpc;

                while (true)
                {
                    networkimplementation.Tick();
                    Thread.Sleep(50);
                }
                    //Testing.TestInterface_ClientProxy testinterface_clientproxy = new OSMP.Testing.TestInterface_ClientProxy( rpc, 
            }
        }

        public static void Go(string[] args)
        {
            bool IsServer = true;
            if (args.GetUpperBound(0) + 1 > 0 && args[args.GetUpperBound(0)] == "client")
            {
                IsServer = false;
            }

            try
            {
                if (IsServer)
                {
                    new TestNetRpcServer().Go();
                }
                else
                {
                    new TestNetRpcClient().Go();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

