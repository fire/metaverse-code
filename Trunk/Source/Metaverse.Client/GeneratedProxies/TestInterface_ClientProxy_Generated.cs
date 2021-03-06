// *** This is a generated file; if you want to change it, please change the generator or the appropriate interface file
// 
// This file was generated by NetworkProxyBuilder, by Hugh Perkins hughperkins@gmail.com http://manageddreams.com
// 

using System;
using System.Net;

namespace OSMP.Testing
{
public class TestInterface_ClientProxy : OSMP.Testing.ITestInterface
{
   RpcController rpc;
   IPEndPoint connection;

   public TestInterface_ClientProxy( RpcController rpc, IPEndPoint connection )
   {
      this.rpc = rpc;
      this.connection = connection;
   }
   public void SayHello(  )
   {
      rpc.SendRpc( connection, "OSMP.Testing.ITestInterface", "SayHello",  new object[]{  } );
   }
   public void SayMessage( System.String message )
   {
      rpc.SendRpc( connection, "OSMP.Testing.ITestInterface", "SayMessage",  new object[]{ message } );
   }
   public void SendObject( OSMP.Testing.TestClass testobject )
   {
      rpc.SendRpc( connection, "OSMP.Testing.ITestInterface", "SendObject",  new object[]{ testobject } );
   }
}
}
