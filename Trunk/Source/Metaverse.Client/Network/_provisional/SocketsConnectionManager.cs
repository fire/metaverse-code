// Copyright Hugh Perkins 2004,2005,2006
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
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SocketsConnectionManager
{
    ArrayList connections = new ArrayList();
    
    Socket oursocket;  //!< socket on which we're listening for new connections

    int port;      //!< Our port number
    
    public SocketsConnectionManager( int port )
    {
        this.port = port;
        
        oursocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
        oursocket.Bind( new IPEndPoint( IPAddress.Any, port ) );
        oursocket.Listen(10);
    }
    
    NetConnection GetConnectionForForeignReference( int iForeignReference )
    {
        for( int i = 0; i < connections.Count; i++ )
        {
            if( ((NetConnection)connections[i]).iForeignReference == iForeignReference )
            {
                return (NetConnection)connections[i];
            }
        }
        return null;
    }
    
    ArrayList GetSocketList()
    {
        ArrayList sockets = new ArrayList();
        for( int i = 0; i < connections.Count; i++ )
        {
            sockets.Add( ((NetConnection)connections[i]).socket );
        }
        return sockets;
    }
    
    void CloseConnectionNow( NetConnection connection )
    {
        connection.socket.Close();
        connection.bConnected = false;
    }
    
    void Broadcast( string message )
    {
        for( int i = 0; i < connections.Count; i++ )
        {
            NetConnection thisconnection = (NetConnection)connections[i];
            thisconnection.socket.Send( Encoding.UTF8.GetBytes( message ) );
        }
    }
    
    void ShowCurrentConnections()
    {
        Test.Debug(  "Current connections on port " + port.ToString() + ":" ); // Test.Debug
    
        for( int i = 0; i < connections.Count; i++ )
        {
            NetConnection connection = (NetConnection)connections[i];
            Test.Debug(  "connectionref " + i.ToString() + " foreign ref " << connection.iForeignReference.ToString() +
                    " name " + connection.name + " socketnum " + connection.socket.RemoteEndpoint.ToString() );
        }
    }
    
    int SendThruConnection( NetConnection connection, string message )
    {
        connection.socket.Send( Encoding.UTF8.GetBytes( message ) );
        return 0;
    }
    
    void CheckForNewClients()
    {
        while( oursocket.Poll( 0, SelectMode.SelectRead ) )
        {
            Test.Debug(  "New connection available, port " + port.ToString() + "  Accepting..." ); // Test.Debug
            
            Socket newsocket = oursocket.Accept();
            
            NetConnection newconnection = new NetConnection();
            newconnection.socket = newsocket;
            newconnection.bAuthenticated = false;
            newconnection.name = "";
            newconnection.iForeignReference = -1;
            newconnection.bConnected = true;
            Test.Debug("New connection " + connection.ToString() );
            
            connections.Add( newconnection );
            ShowCurrentConnections();
        }
    }
    
    int NumClientConnectionsWithName( string name )
    {
        int iNum = 0;
        for( int i = 0; i < connections.Count; i++ )
        {
            NetConnection thisconnection = (NetConnection)connections[i];
            if( thisconnection.name == name )
            {
                iNum++;
            }
        }
        return iNum;
    }
    
    void PurgeDisconnectedConnections()
    {
        //Test.Debug("PurgeDisconnectedConnections()" );
        for( int i = 0; i < connections.Count; i++ )
        {
            NetConnection connection = (NetConnection)connections[i];
            if( !connection.bConnected )
            {
                Test.Debug(  "Purging connection of " << connection.name ); // Test.Debug
                connections.RemoveAt(i);
                i--;
            }
        }
        //  Test.Debug(  "purge done" ); // Test.Debug
    }
}
