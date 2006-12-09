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

namespace OSMP
{
    public class ReflectionHelper
    {
        // returns array of the types of the parameters of the method specified by methodinfo
        public static Type[] GetParameterTypes( MethodBase methodbase )
        {
            ParameterInfo[] parameterinfos = methodbase.GetParameters();
            Type[] paramtypes = new Type[ parameterinfos.GetUpperBound(0) + 1 ];
            for( int i = 0; i < parameterinfos.GetUpperBound(0) + 1; i ++ )
            {
                paramtypes[i] = parameterinfos[i].ParameterType;
            }
            return paramtypes;
        }
    
        // returns public class members having the attribute attributetype
        // if attributetype is null, returns all public members
        public static MemberInfo[] GetMembersForAttribute( Type objecttype, Type attributetype )
        {
            ArrayList members = new ArrayList();
            foreach( MemberInfo memberinfo in objecttype.GetMembers() )
            {
                if( attributetype == null || HasAttribute( memberinfo, attributetype ) )
                {
                    members.Add( memberinfo );
                }
            }
            return (MemberInfo[])members.ToArray( typeof( MemberInfo ) );
        }
        
        // returns true if the method specified by memberinfo has the attribute attributetype
        public static bool HasAttribute( MemberInfo memberinfo, Type attributetype )
        {
            foreach( object customattribute in memberinfo.GetCustomAttributes( false ) )
            {
                if( customattribute.GetType() == attributetype )
                {
                    return true;
                }            
            }
            return false;
        }
    
        // we can just use Ldarg_S systematically, but this version is more optimal; we can actually live without this to be honest ;-)
        public static void PushArg( ILGenerator generator, int argnum )
        {
            switch( argnum )
            {
                case 0:
                    generator.Emit( OpCodes.Ldarg_0 );
                    break;
                case 1:
                    generator.Emit( OpCodes.Ldarg_1 );
                    break;
                case 2:
                    generator.Emit( OpCodes.Ldarg_2 );
                    break;
                case 3:
                    generator.Emit( OpCodes.Ldarg_3 );
                    break;
                default:
                    generator.Emit( OpCodes.Ldarg_S, argnum );
                    break;
            }
        }    
        
        public static void CreateArray( ILGenerator generator, Type arraytype, int arraysize )
        {
            generator.Emit( OpCodes.Ldc_I4, arraysize );
            generator.Emit( OpCodes.Newarr, arraytype );
        }
        
        public static void Return( ILGenerator generator )
        {
            generator.Emit( OpCodes.Ret );
        }
    
        // Call stelem_ref to add a value to an array.  Need to box value types first
        public static void SetObjectArrayElement( ILGenerator generator, Type elementtype )
        {
            if( elementtype.IsValueType )
            {
                generator.Emit( OpCodes.Box, elementtype );
            }
            generator.Emit( OpCodes.Stelem_Ref );
        }
        
        // Console.WriteLine( message ); where message is a fixed string
        public static void ConsoleWriteLine( ILGenerator generator, string message )
        {
            generator.Emit( OpCodes.Ldstr, message );
            generator.EmitCall( OpCodes.Call, typeof( Console ).GetMethod( "WriteLine", new Type[]{ typeof( string ) } ), null );
        }
    }
}
