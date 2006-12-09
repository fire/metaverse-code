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

namespace OSMP
{
    // note that char is treated as an ASCII char, not unicode.  This is by design, though it's arguably not a great design.
    // we do this for cases where we just want to use a single letter code for things
    // suggestions -> hughperkins@gmail.com
    public class BinaryPacker
    {
        public static void WriteValueToBuffer( byte[] buffer, ref int nextposition, Type type, object value )
        {    
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
                int length = payload.Length;
                byte[]lengthbytes = BitConverter.GetBytes( length );
                
                Buffer.BlockCopy( lengthbytes, 0, buffer, nextposition, lengthbytes.Length );
                nextposition += lengthbytes.Length;
                
                Buffer.BlockCopy( payload, 0, buffer, nextposition, payload.Length );
                nextposition += payload.Length;
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
            else if( type == typeof( short ) )
            {
                byte[]result = BitConverter.GetBytes( (short)value );
                Buffer.BlockCopy( result, 0, buffer, nextposition, result.Length );
                
                nextposition += result.Length;
            }
            else
            {
                throw new Exception("Unknown type: " + type.ToString() + " " + value.ToString() );
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
                int payloadlength = BitConverter.ToInt32( buffer, nextposition );
                nextposition += 4;
                
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
            else
            {
                throw new Exception("Unknown type: " + type.ToString() );
            }
        }
    }
}
