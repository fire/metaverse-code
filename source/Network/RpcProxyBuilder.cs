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

namespace OSMP
{
    public class RpcProxyBuilder
    {
        static RpcProxyBuilder instance = new RpcProxyBuilder();
        public static RpcProxyBuilder GetInstance(){ return instance; }
        
        AssemblyBuilder assemblybuilder;
        AssemblyName assemblyname;
        ModuleBuilder modulebuilder;
        
        Hashtable servertypes = new Hashtable();
        Hashtable remoteclienttypes = new Hashtable();
        
        RpcController rpccontroller;
        
        public RpcController RpcController
        {
            set{ rpccontroller = value; }
        }
        
        public RpcProxyBuilder()
        {
        }
        
        public Type GetRpcObjectType( bool IsServer, Type targettype )
        {
            return CreateNetObject( IsServer, targettype );
        }

        Type CreateNetObject( bool IsServer, Type targettype )
        {
            if( IsServer && servertypes.Contains( targettype ) )
            {
                return (Type)servertypes[ targettype ];
            }
            else if( !IsServer && remoteclienttypes.Contains( targettype ) )
            {
                return (Type)remoteclienttypes[ targettype ];
            }
            
            CreateAssemblyBuilder();
            CreateType( IsServer, targettype );
            return CreateNetObject( IsServer, targettype );
        }
        
        void CreateAssemblyBuilder()
        {
            if( assemblybuilder == null )
            {
                Console.WriteLine("creating assembly..." );
                
                assemblyname = new AssemblyName();
                assemblyname.Name = "RpcProxy";
                
                assemblybuilder = Thread.GetDomain().DefineDynamicAssembly( assemblyname, AssemblyBuilderAccess.RunAndSave );
                modulebuilder = assemblybuilder.DefineDynamicModule( "RpcProxy.dll" );
            }
        }
    
        // should create:    
        //      originalclassRpc( object connection, originalclass component )
        //      {
        //          this.connection = connection;
        //          this.component = component;
        //          this.rpc = RpcController.GetInstance();
        //          base();
        //      }
        void CreateConstructor( TypeBuilder typebuilder, Type originalclasstype, FieldInfo component, FieldInfo rpc, FieldInfo connection )
        {
            ConstructorBuilder constructor;
            ILGenerator generator;
    
            //Type[] parametertypes = ReflectionHelper.GetParameterTypes( constructorinfo );
            constructor = typebuilder.DefineConstructor( MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                CallingConventions.Standard, new Type[]{ typeof( object ), originalclasstype } );
            generator = constructor.GetILGenerator();
            
            // assign first argument to connection field
            ReflectionHelper.PushArg( generator, 0 ); // have to push self first before using Stfld
            ReflectionHelper.PushArg( generator, 1 );
            generator.Emit( OpCodes.Stfld, connection );
            
            // assign second argument to component field
            ReflectionHelper.PushArg( generator, 0 ); // have to push self first before using Stfld
            ReflectionHelper.PushArg( generator, 2 );
            generator.Emit( OpCodes.Stfld, component );
            
            // RpcController.GetInstance() instance variable, so have to push self/this first
            // took me a long time to find that out so maybe dont delete this comment
            ReflectionHelper.PushArg( generator, 0 );
            generator.EmitCall( OpCodes.Call, rpccontroller.GetType().GetMethod( "GetInstance" ), null );
            generator.Emit( OpCodes.Stfld, rpc );
            
            // call base class contructor, which in this case is on "object" class
            ReflectionHelper.PushArg( generator, 0 );
            generator.Emit( OpCodes.Call, typeof( object ).GetConstructor( new Type[]{} ) );
                
            // return
            ReflectionHelper.Return( generator );
        }
    
        // create simple pass-thru function:
        // public void methodname( object connection, type1 arg1, type2 arg2, type3 arg3, ... )
        // {
        //     component.methodname( connection, arg1, arg2, arg3, ... );
        // }
        //
        // we need this because the actual base object we derove from contains no actual data; it's all contained in the contained component
        void CreatePassThru( Type targettype, TypeBuilder typebuilder, MethodInfo methodtooverride, FieldInfo component )
        {
            Type[] parametertypes = ReflectionHelper.GetParameterTypes( methodtooverride );
            MethodBuilder methodbuilder = typebuilder.DefineMethod( methodtooverride.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot,
                typeof( void ), parametertypes );
            ILGenerator generator = methodbuilder.GetILGenerator();
    
            MethodInfo methodinfotooveride = targettype.GetMethod( methodtooverride.Name );
            
            // push component pointer onto stack
            ReflectionHelper.PushArg( generator, 0 );
            generator.Emit( OpCodes.Ldfld, component );
            
            // push arguments onto the stack
            int NumArguments = methodinfotooveride.GetParameters().GetUpperBound(0) + 1;
            for( int i = 1; i <= NumArguments; i++ )
            {
                ReflectionHelper.PushArg( generator, i );
            }
            
            if( methodtooverride.IsVirtual )
            {
                generator.EmitCall( OpCodes.Callvirt, methodtooverride, null );
            }
            else
            {
                generator.EmitCall( OpCodes.Call, methodtooverride, null );
            }
            
            ReflectionHelper.Return( generator );
        }
        
        //
        // rerouting methods will be something like:
        // public void methodname( type1 arg1, type2 arg2, type3 arg3, ... )
        //{
        //    this.rpc.SendRpcToRemoteClient( "classname", this.connection, new object[]{ arg1, arg2, arg3 ... } );
        //}
        //or
        // public void methodname( type1 arg1, type2 arg2, type3 arg3, ... )
        //{
        //    this.rpc.SendRpcToServer( "classname", this.connection, new object[]{ arg1, arg2, arg3 ... } );
        //}
        // ... depending on the value of IsServer
        // Whilst the generated code is not actually type-safe, the interfaces that are presented to the user are
        void CreateRpcRedirector( bool IsServer, Type targettype, TypeBuilder typebuilder, MethodInfo methodtooverride, FieldInfo rpc, FieldInfo connection )
        {
            // first we extract the parameters of the method we are overriding, and create a types array which
            // we use to generate our new generated method of the same name
            
            Type[] parametertypes = ReflectionHelper.GetParameterTypes( methodtooverride );
            MethodBuilder methodbuilder = typebuilder.DefineMethod( methodtooverride.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot, typeof( void ), parametertypes );
            ILGenerator generator = methodbuilder.GetILGenerator();
            
            // MethodInfos can be passed directly into the generator.Emit function calls
            // they act essentially like pointers to objects
            // when we actually use them in generator.Emit, if it's an instance (object), we're going to have to push
            // a value onto the stack somehow with the actual pointer
            // if we're using a static (class) method, we dont need this
            //
            MethodInfo methodinfotooveride = targettype.GetMethod( methodtooverride.Name );
            MethodInfo redirecttarget = null;
            if( IsServer )
            {
                redirecttarget = rpccontroller.GetType().GetMethod("SendRpcToRemoteClient" );
            }
            else
            {
                redirecttarget = rpccontroller.GetType().GetMethod("SendRpcToServer" );
            }
    
            // load rpc onto stack, as arg0 of redirect call
            ReflectionHelper.PushArg( generator, 0 );
            generator.Emit( OpCodes.Ldfld, rpc );
            
            // push classname onto stack, as arg to redirect call
            generator.Emit( OpCodes.Ldstr, GetClassName( targettype ) );
            
            // get methodinfo for current method, as arg to redirect call
            generator.EmitCall( OpCodes.Call, typeof( MethodBase ).GetMethod( "GetCurrentMethod" ), null );
            
            // push connection to stack, from object variable, as arg to redirect call
            ReflectionHelper.PushArg( generator, 0 );
            generator.Emit( OpCodes.Ldfld, connection );
            
            LocalBuilder objectarray = generator.DeclareLocal( typeof( object[] ) );
    
            // create an array: object[ NumArguments ]    
            int NumArguments = methodinfotooveride.GetParameters().GetUpperBound(0) + 1;
            ReflectionHelper.CreateArray( generator, typeof( object ), NumArguments );
            generator.Emit( OpCodes.Stloc_0 );
    
            // add the arguments of the overriden function into the array
            for( int i = 1; i <= NumArguments; i++ )
            {
                generator.Emit( OpCodes.Ldloc_0 );
                generator.Emit( OpCodes.Ldc_I4, i - 1 );
                ReflectionHelper.PushArg( generator, i );
                ReflectionHelper.SetObjectArrayElement( generator, parametertypes[ i - 1 ] );
            }
    
            generator.Emit( OpCodes.Ldloc_0 );
            generator.EmitCall( OpCodes.Call, redirecttarget, null );
    
            ReflectionHelper.Return( generator );
        }
        
        // we use Reflection.Emit to create a proxy
        // which wraps all the methods on the class
        // just routing to the encapsulated object for normal functions
        // or routing to RpcController for those marked with an Rpc attribute
        //
        // Generated class will be something like:
        //
        // public class originalclassRpc : originalclass
        // {
        //      IoriginalclassRpc component;
        //      RpcController rpc;
        //      object connection;
        //
        //      originalclassRpc( object connection, IOriginalclass component )
        //      {
        //          this.component = component;
        //          this.rpc = RpcController.GetInstance();
        //      }
        //
        //    [ ... methods ... ]
        // }
        void CreateType( bool IsServer, Type targettype )
        {
            if( IsServer && servertypes.Contains( targettype ) || !IsServer && remoteclienttypes.Contains( targettype ) )
            {
                return;
            }
    
            Console.WriteLine("creating type " + TypeToDerivedClassName( IsServer, targettype ) + " ..." );
            
            // check type is public ,otherwise this will fail with a really bizarre error.  *This check is really important*
            if( !targettype.IsPublic )
            {
                throw new Exception( "Error: " + targettype.ToString() + " should be public if you want to use it for network rpc" );
            }
            // same check on ourselves
            if( !this.GetType().IsPublic )
            {
                throw new Exception( "Error: " + this.GetType().ToString() + " should be public if you want to use it for network rpc" );
            }
            
            Type interfacetype = targettype.GetInterface( TypeToInterfaceName( targettype ) );
            if( interfacetype == null )
            {
                throw new Exception(" Error: " + targettype.ToString() + " needs to derive from " + TypeToInterfaceName( targettype ) );
            }
            
            TypeBuilder typebuilder = modulebuilder.DefineType( TypeToDerivedClassName( IsServer, targettype ), TypeAttributes.Public );
            typebuilder.AddInterfaceImplementation( interfacetype );
    
            FieldInfo rpc = typebuilder.DefineField( "rpc", this.GetType(), FieldAttributes.Private );
            FieldInfo component = typebuilder.DefineField( "component", this.GetType(), FieldAttributes.Private );
            FieldInfo connection = typebuilder.DefineField( "connection", typeof( object ), FieldAttributes.Private );
            
            CreateConstructor( typebuilder, targettype, component, rpc, connection );
                    
            foreach( MethodInfo methodinfo in targettype.GetMethods() )
            {
                if( IsServer && ReflectionHelper.HasAttribute( methodinfo, typeof( RpcToRemoteClientAttribute ) ) ||
                    !IsServer && ReflectionHelper.HasAttribute( methodinfo, typeof( RpcToServerAttribute ) ) )
                {
                    CreateRpcRedirector( IsServer, targettype, typebuilder, methodinfo, rpc, connection );
                }
                else
                {
                    CreatePassThru( targettype, typebuilder, methodinfo, component );
                }
            }
            
            Type rpctype = typebuilder.CreateType();
            
            //Dump( rpctype );

            if( IsServer )
            {
                servertypes.Add( targettype, rpctype );
            }
            else
            {
                remoteclienttypes.Add( targettype, rpctype );
            }
        }
        
        // note that you can only call this once, I think, and the assembly must have been created with RunAndSave
        public void SaveAssembly()
        {
            assemblybuilder.Save( assemblybuilder.GetName().Name + ".dll" );
        }
        
        void Dump( Type type )
        {
            Console.WriteLine( "Dumping " + type.ToString() + ":");
            foreach( MethodInfo methodinfo in type.GetMethods() )
            {
                string methodstring = methodinfo.ReturnType.ToString() + " " + methodinfo.Name + "(";
                bool isfirst = true;
                foreach( ParameterInfo parameterinfo in methodinfo.GetParameters() )
                {
                    if( isfirst )
                    {
                        isfirst = false;
                    }
                    else
                    {
                        methodstring += ", ";
                    }
                    methodstring += parameterinfo.ParameterType.ToString() + " " + parameterinfo.Name;
                }
                methodstring += ");";
                Console.WriteLine( methodstring );
            }
            foreach( System.Reflection.PropertyInfo propertyinfo in type.GetProperties() )
            {
                Console.WriteLine( propertyinfo.PropertyType.ToString() + " " + propertyinfo.Name );
            }
        }
        
        string TypeToInterfaceName( Type type )
        {
            string[] splitstring = type.ToString().Split( new char[]{'.'} );
            if( splitstring.GetUpperBound(0) + 1 > 0 )
            {
            //    return splitstring[0] + ".I" + splitstring[1].ToString() + "Rpc";
                return splitstring[0] + ".I" + splitstring[1].ToString();
            }
            else
            {
                // return "I" + type.ToString() + "Rpc";
                return "I" + type.ToString();
            }
        }
        
        string TypeToDerivedClassName( bool IsServer, Type type )
        {
            if( IsServer )
            {
                return type.ToString() + "ServerRpc";
            }
            else
            {
                return type.ToString() + "RemoteClientRpc";
            }
        }

        string GetClassName( Type type )
        {
            string[] splitstring = type.ToString().Split( new char[]{'.'} );
            return splitstring[ splitstring.GetUpperBound(0) ];
        }
    }
}
