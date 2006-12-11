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
using System.Text;
using System.Reflection;

namespace OSMP
{
    // binarypacker can pack simple types, rank-1 arrays of simple types, classes, and combinations of these things
    // For writing, it just needs whatever you want to write, and an appropriately-sized array to write to
    // For reading, it needs the Type of what you are trying to read
    // We dont embed any type information in the binary, for compactness, so it is important
    // that reader knows accurately what it should be trying to read

    // note that char is treated as an ASCII char, not unicode.  This is by design, though it's arguably not a great design.
    // we do this for cases where we just want to use a single letter code for things
    // suggestions -> hughperkins@gmail.com
    public class BinaryPacker
    {
        public static void WriteValueToBuffer( byte[] buffer, ref int nextposition, object value )
        {
            if (value == null)
            {
                throw new Exception("Attempted to pack null value.  Please make sure all values are not null and try again.");
            }
            Type type = value.GetType();
            if( type == typeof( bool ) )
            {
                if( (bool)value )
                {
                    buffer[nextposition] = (byte)'T';
                }
                else
                {
                    buffer[nextposition] = (byte)'F';
                }
                nextposition += 1;
            }
            else if( type == typeof( char ) )
            {
                buffer[nextposition] = Encoding.ASCII.GetBytes( value.ToString() )[0];
                nextposition ++;
            }
            else if( type == typeof( string ) )
            {
                byte[]payload = Encoding.UTF8.GetBytes( (string)value );

                short length = (short)payload.GetLength(0);
                WriteValueToBuffer(buffer, ref nextposition, length);
                
                Buffer.BlockCopy( payload, 0, buffer, nextposition, payload.GetLength(0) );
                nextposition += payload.GetLength(0);
            }
            else if( type == typeof( double ) )
            {
                byte[]result = BitConverter.GetBytes( (double)value );
                Buffer.BlockCopy( result, 0, buffer, nextposition, result.Length );
                
                nextposition += result.Length;
            }
            else if( type == typeof( int ) )
            {
                byte[]result = BitConverter.GetBytes( (int)value );
                Buffer.BlockCopy( result, 0, buffer, nextposition, result.Length );
                
                nextposition += result.Length;
            }
            else if (type == typeof(short))
            {
                byte[] result = BitConverter.GetBytes((short)value);
                Buffer.BlockCopy(result, 0, buffer, nextposition, result.Length);

                nextposition += result.Length;
            }
            else if (type.IsArray && type.GetArrayRank() == 1)
                // we handle simple one-dimensional arrays of simple types
            {
                Array valueasarray = value as Array;
                int size = valueasarray.GetLength(0);
                // first we write the size, then the data
                WriteValueToBuffer(buffer, ref nextposition, size);
                foreach (object item in valueasarray)
                {
                    WriteValueToBuffer(buffer, ref nextposition, item);
                }
                //byte[] result = BitConverter.GetBytes((short)value);
                //Buffer.BlockCopy(result, 0, buffer, nextposition, result.Length);

                //nextposition += result.Length;
            }
            else if (type.IsClass) // pack public fields from class
            {
                //Console.WriteLine("Pack class " + type.Name);
                foreach (FieldInfo fieldinfo in type.GetFields())
                {
                  //  Console.WriteLine("packing " + fieldinfo.Name + " ...");
                    object fieldvalue = fieldinfo.GetValue(value);
                    WriteValueToBuffer(buffer, ref nextposition, fieldvalue);
                }
            }
            else
            {
                throw new Exception("Unknown type: " + type.ToString() + " " + value.ToString());
            }
        }
        
        // Note to self: for Macs, we probably need to swap endianness?
        // can detect endedness using BitConverter.IsLittleEndian
        public static object ReadValueFromBuffer( byte[] buffer, ref int nextposition, Type type )
        {    
            if( type == typeof( bool ) )
            {
                if( buffer[nextposition] == (byte)'T' )
                {
                    nextposition++;
                    return true;
                }
                nextposition++;
                return false;
            }
            else if( type == typeof( string ) )
            {
                short payloadlength = (short)ReadValueFromBuffer( buffer, ref nextposition, typeof( short ) );
                
                string datastring = Encoding.UTF8.GetString( buffer, nextposition, payloadlength );
                nextposition += payloadlength;
                
                return datastring;
            }
            else if( type == typeof( char ) )
            {
                char value = Encoding.ASCII.GetString( buffer, nextposition, 1 )[0];
                nextposition ++;
                return value;
            }
            else if( type == typeof( double ) )
            {
                object result = BitConverter.ToDouble( buffer, nextposition );
                nextposition += 8;
                return result;
            }
            else if( type == typeof( int ) )
            {
                object result = BitConverter.ToInt32( buffer, nextposition );
                nextposition += 4;
                return result;
            }
            else if( type == typeof( short ) )
            {
                object result = BitConverter.ToInt16( buffer, nextposition );
                nextposition += 2;
                return result;
            }
            else if (type.IsArray && type.GetArrayRank() == 1)
            // we handle simple one-dimensional arrays of simple types
            {
                // first we read the size, then the data
                int size = (int)ReadValueFromBuffer(buffer, ref nextposition, typeof(int));
                Type elementtype = type.GetElementType();
                Array valueasarray = Array.CreateInstance(elementtype, size);
                for( int i = 0; i < size; i++ )
                {
                    valueasarray.SetValue( ReadValueFromBuffer(buffer, ref nextposition, elementtype ), i );
                }
                return valueasarray;
            }
            else if (type.IsClass) // unpack public fields from class
            {
                //Console.WriteLine("unpack class " + type.Name);
                object newobject = Activator.CreateInstance(type);
                foreach (FieldInfo fieldinfo in type.GetFields())
                {
                  //  Console.WriteLine("unpacking " + fieldinfo.Name + " ...");
                    object fieldvalue = ReadValueFromBuffer(buffer, ref nextposition, fieldinfo.FieldType);
                    fieldinfo.SetValue(newobject, fieldvalue);
                }
                return newobject;
            }
            else
            {
                throw new Exception("Unknown type: " + type.ToString() );
            }
        }
    }
}
