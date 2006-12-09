using System;
using System.Collections;
using System.Threading;

class ServerController
{
    static ServerController instance = new ServerController();
    public static ServerController GetInstance(){ return instance; }
    
    ServerControllerSlave servercontrollerslave;
    Thread servercontrollerthread;
    
    public void ConnectToServer( string hostname, int port )
    {
        servercontrollerslave = new ServerControllerSlave();
        servercontrollerthread = new Thread( new ThreadStart( servercontrollerslave.Go ) );
        servercontrollerthread.IsBackground = true;
        servercontrollerthread.Start();
    }
}

// thread for ServerController
class ServerControllerSlave
{
    public void Go()
    {
    }
}

class Client
{
    public void Go()
    {
        ServerController.GetInstance().ConnectToServer( "127.0.0.1", 8989 );
        ReplicationController.GetInstance().RegisterObjectToReplicate( World.GetInstance() );
        
        while( true )
        {
            Thread.Sleep( 200 );
        }
    }
}

class entrypoint
{
    public static void Main( string[] args )
    {
        try
        {
            new Client().Go();
        }
        catch( Exception e )
        {
            Console.WriteLine( e );
        }
    }
}
