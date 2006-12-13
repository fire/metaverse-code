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
    public class RpcController
    {
        const char RpcType = 'Y';
        // this is for security, so the client cant ask for Assembly or Activator etc
        string[] allowedtypes = new string[] { "OSMP.Testing.ITestInterface" };

        NetworkLevel2Controller network;

        BinaryPacker binarypacker = new BinaryPacker();
        
        bool isserver = true;
            
        public RpcController( NetworkLevel2Controller network )
        {
            this.network = network;
            network.RegisterPacketConsumer(RpcType, new Level2ReceivedPacketHandler(network_ReceivedPacket));
            //network.ReceivedPacket += new Level2ReceivedPacketHandler(network_ReceivedPacket);
            //network.NetworkChangeState += new Level1NetworkChangeStateHandler( _NetworkChangeState );
            isserver = network.IsServer;
        }

        void network_ReceivedPacket( NetworkLevel2Connection connection, byte[] data, int offset, int length )
        {
            Console.WriteLine("rpc received packet " + Encoding.UTF8.GetString(data, offset, length));
            try
            {
                if (length > 1)
                {
                    int position = offset;
                    //char type = (char)binarypacker.ReadValueFromBuffer(data, ref position, typeof(Char));
                    //if (type == RpcType)
                    //{
                        string typename = (string)binarypacker.ReadValueFromBuffer(data, ref position, typeof(string));
                        string methodname = (string)binarypacker.ReadValueFromBuffer(data, ref position, typeof(string));
                        Console.WriteLine("Got rpc [" + typename + "] [" + methodname + "]");
                        if (ArrayHelper.IsInArray(allowedtypes, typename)) // security check to prevent arbitrary activation
                        {
                            int dotpos = typename.LastIndexOf(".");
                            string namespacename = "";
                            string interfacename;
                            if (dotpos >= 0)
                            {
                                namespacename = typename.Substring(0, dotpos );
                                interfacename = typename.Substring(dotpos + 1);
                            }
                            else
                            {
                                interfacename = typename;
                            }
                            Console.WriteLine("[" + namespacename + "][" + interfacename + "]");

                            string serverwrapperclassname = interfacename.Substring(1) + "";
                            if (namespacename != "")
                            {
                                serverwrapperclassname = namespacename + "." + serverwrapperclassname;
                            }
                            Console.WriteLine("[" + serverwrapperclassname + "]");

                            Type interfacetype = Type.GetType(typename);

                            Type serverwrapperttype = Type.GetType(serverwrapperclassname);
                            Console.WriteLine("[" + serverwrapperttype + "]");
                            object serverwrapperobject = Activator.CreateInstance(serverwrapperttype, new object[] { connection.connectioninfo.Connection });
                            MethodInfo methodinfo = serverwrapperttype.GetMethod(methodname);

                            ParameterInfo[] parameterinfos = methodinfo.GetParameters();
                            object[] parameters = new object[parameterinfos.GetLength(0)];
                            for (int i = 0; i < parameters.GetLength(0); i++)
                            {
                                parameters[i] = binarypacker.ReadValueFromBuffer(data, ref position, parameterinfos[i].ParameterType);
                            }

                            //foreach (object parameter in parameters)
                            //{
                             //   Console.WriteLine(parameter.GetType().ToString() + " " + parameter.ToString());
                            //}
                            methodinfo.Invoke(serverwrapperobject, parameters);
                        }
                        else
                        {
                            Console.WriteLine("Warning: unauthorized RPC type " + typename + ". Check RpcController.allowedtypes");
                        }
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        public bool IsServer{
            get{ return isserver; }
        }
        
        //public void _NetworkChangeState( object source, NetworkChangeStateArgs e )
        //{
          //  isserver = e.IsServer;
        //}
        
        // rpc format:
        // (classname)(methodname)(arg1)(arg2)(arg3)...
        // SendRpc( connection, "OSMP.Testing.ITestInterface", "SayHello",  new object[]{  } )
        public void SendRpc( object connection, string typename, string methodname, object[]args )
        {
            // Test.WriteOut( args );
            //Console.WriteLine( "SendRpc " + typename + " " + methodname );
            //for( int i = 0; i < args.GetUpperBound(0) + 1; i++ )
            //{
              //  Console.WriteLine("  arg: " + args[i].ToString() );
            //}
            
            byte[]packet = new byte[1400]; // note to self: make this a little more dynamic...
            int nextposition = 0;

            //binarypacker.WriteValueToBuffer(packet, ref nextposition, RpcType);
            binarypacker.WriteValueToBuffer(packet, ref nextposition, typename);
            binarypacker.WriteValueToBuffer(packet, ref nextposition, methodname);
            foreach (object parameter in args)
            {
                binarypacker.WriteValueToBuffer(packet, ref nextposition, parameter);
            }

            Console.WriteLine("Sending " + Encoding.UTF8.GetString(packet, 0, nextposition));
            //Console.WriteLine( nextposition + " bytes " + Encoding.ASCII.GetString( packet, 0, nextposition ) );
            network.Send(connection,RpcType, packet, 0, nextposition );
        }
    }
}
