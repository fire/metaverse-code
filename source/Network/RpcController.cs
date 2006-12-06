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
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Text;

namespace OSMP
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class RpcToRemoteClientAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class RpcToServerAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class BidirectionalRpcAttribute : Attribute
    {
    }
    
    //[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    //public class RpcSingletonAttribute : Attribute
    //{
    //}
    
    public interface IRpcGhostable
    {
    }
    
    public interface IProxyTarget
    {
        void SendRpcToServer( string originalclassname, MethodInfo callingmethod, object connection, object[]args );
        void SendRpcToRemoteClient( string originalclassname, MethodInfo callingmethod, object connection, object[]args );
    }
    
    public class RpcController : IProxyTarget
    {
        RpcProxyBuilder rpcproxybuilder;
        INetworkImplementation network;
        
        static RpcController instance = new RpcController();
        public static RpcController GetInstance(){ return instance; }
        
        bool isserver = true;
            
        public RpcController()
        {
            network = NetworkImplementationFactory.GetInstance();
            network.NetworkChangeState += new NetworkChangeStateHandler( _NetworkChangeState );
            isserver = network.IsServer;
            
            rpcproxybuilder = RpcProxyBuilder.GetInstance();
            rpcproxybuilder.RpcController = this;
        }
        
        public bool IsServer{
            get{ return isserver; }
        }
        
        public void _NetworkChangeState( object source, NetworkChangeStateArgs e )
        {
            isserver = e.IsServer;
        }
        
        // note to self: I guess we are ditching connction from this API call
        public object NetObject( object connection, object targetobject )
        {
            Type rpcobjecttype = rpcproxybuilder.GetRpcObjectType( isserver, targetobject.GetType() );   
            return Activator.CreateInstance( rpcobjecttype, new object[]{ connection, targetobject } );
        }
        
        void PackMethodCall( byte[]buffer, ref int nextposition, MethodInfo sourcemethod, object[] args )
        {
            // write method name to buffer
            BinaryPacker.WriteValueToBuffer( buffer, ref nextposition, typeof( string ), sourcemethod.Name );
            
            // write arguments to buffer
            ParameterInfo[] parameterinfos = sourcemethod.GetParameters();            
            for( int i = 0; i < parameterinfos.GetUpperBound(0) + 1; i++ )
            {
                Type parametertype = parameterinfos[i].ParameterType;
                BinaryPacker.WriteValueToBuffer( buffer, ref nextposition, parametertype, args[i] );                
            }
        }
        
        // rpc format:
        // (classname)(methodname)(arg1)(arg2)(arg3)...
        public void SendRpcToServer( string originalclassname, MethodInfo callingmethod, object connection, object[]args )
        {
            Console.WriteLine( originalclassname + ".SendRpcToServer from method " + callingmethod.Name );
            for( int i = 0; i < args.GetUpperBound(0) + 1; i++ )
            {
                Console.WriteLine("  arg: " + args[i].ToString() );
            }
            //byte[]packetmethodcall = Encoding.ASCII.GetBytes( callingmethod. ) + PackMethodCall( callingmethod, args );
            //Console.WriteLine( Encoding.ASCII.GetString( packetmethodcall, 0, packetmethodcall.Length ) );
        }
        
        // rpc format:
        // (classname)(methodname)(arg1)(arg2)(arg3)...
        public void SendRpcToRemoteClient( string originalclassname, MethodInfo callingmethod, object connection, object[]args )
        {
            // Test.WriteOut( args );
            Console.WriteLine( originalclassname + ".SendRpcToRemoteClient from method " + callingmethod.Name );
            for( int i = 0; i < args.GetUpperBound(0) + 1; i++ )
            {
                Console.WriteLine("  arg: " + args[i].ToString() );
            }
            
            byte[]packet = new byte[1400];
            int nextposition = 0;
            
            BinaryPacker.WriteValueToBuffer( packet, ref nextposition, typeof( string ), originalclassname );
            PackMethodCall( packet, ref nextposition, callingmethod, args );
            
            byte[]finalpacket = new byte[ nextposition ];
            Buffer.BlockCopy( packet, 0, finalpacket, 0, nextposition );
            
            Console.WriteLine( Encoding.ASCII.GetString( finalpacket, 0, finalpacket.Length ) );
            // NetworkControllerFactory.GetInstance().Send( connection, finalpacket );
        }
        
        // note that this can only be used; and also RpcProxyBuilder must have been configured to use RunAndSave, instead of Run
        // using RunAndSave is generally bad for performance
        public void SaveAssembly()
        {
            rpcproxybuilder.SaveAssembly();
        }
    }
}
