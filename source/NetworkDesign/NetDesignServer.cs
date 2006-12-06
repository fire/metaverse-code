using System;
using System.Collections;
using System.Threading;

class ClientController
{
    static ClientController instance = new ClientController();
    public static ClientController GetInstance(){ return instance; }
    
    public void Init( int port )
    {
    }
}

class Server
{
    public void Go()
    {
        ClientController.GetInstance().Init( 8989 );
        
        World.GetInstance().AddEntity( new Box("Tom") );
        World.GetInstance().AddEntity( new Sphere("Fred") );
        World.GetInstance().AddEntity( new Box("Simon") );
        
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
            new Server().Go();
        }
        catch( Exception e )
        {
            Console.WriteLine( e );
        }
    }
}
