
using System;
using System.Collections;

namespace OSMP
{
    public class Avatar
    {
        public Vector3 pos;
        public Avatar()
        {
        }
        public Avatar( Vector3 pos )
        {
            this.pos = pos;
        }
    }
    
    public class MetaverseClient
    {
        static MetaverseClient instance = new MetaverseClient();
        public static MetaverseClient GetInstance(){ return instance; }
        
        public Avatar myavatar;
        public double fHeight;
    }
    
    public interface ITestEntity
    {
        void DoNothing();
        void SaySomething();
        void SendToGhost( string message );
        void AddTwoNumbers( int a, int b );
    }
    
    //[Ghostable]
    public class TestEntity : ITestEntity, IRpcGhostable
    {
        public TestEntity()
        {
            NetObjectReferenceController.GetInstance().Register( this );
            Console.WriteLine("Entity constructor");
        }
        
        public void DoNothing()
        {
            Console.WriteLine("DoNothing" );
        }
        
        public void SaySomething()
        {
            Console.WriteLine("test" );
        }
        
        [RpcToRemoteClient]
        public virtual void SendToGhost( string message )
        {
            Console.WriteLine( "message from server: " + message );
        }
    
        [RpcToRemoteClient]
        public virtual void AddTwoNumbers( int a, int b )
        {
            Console.WriteLine( "Sum is : " + ( a + b ).ToString() );
        }
    }
    
    public interface ITestBox : ITestEntity
    {
        void Render();
    }
    
    //[Ghostable]
    public class TestBox : TestEntity, ITestBox
    {
        public int X;
        public int Y;
            
        public TestBox()
        {
            NetObjectReferenceController.GetInstance().Register( this );
        }
        
        public TestBox( int X, int Y )
        {
            this.X = X;
            this.Y = Y;
        }
        
        [RpcToRemoteClient]
        public void Render()
        {
            Console.WriteLine( "rendering " + X.ToString() + "," + Y.ToString() );
        }
    }    
    
    class TestNetRpc
    {
        INetworkImplementation networkimplementation;
        string serveraddress = "127.0.0.1";
        int serverport = 3000;
        
        public void Go( bool IsServer )
        {
            networkimplementation = NetworkImplementationFactory.GetInstance();
            RpcController rpc = RpcController.GetInstance();
            
            if( IsServer )
            {
                networkimplementation.ListenAsServer( serverport );
            }
            else
            {
                networkimplementation.ConnectAsClient( serveraddress, serverport );
            }
                        
            TestEntity entity = new TestEntity();
            ITestEntity entityrpc = rpc.NetObject( null, entity ) as ITestEntity;
            entityrpc.SendToGhost( "Hi there ghost!" );
            entityrpc.SaySomething();
            entityrpc.AddTwoNumbers( 3, 5 );
            
            TestBox box3 = new TestBox( 5, 13 );
            Console.WriteLine( box3 );
            ITestBox box3rpc = rpc.NetObject( null, box3 ) as ITestBox;
            box3rpc.SendToGhost("send something to box ghost");
            box3rpc.Render();
            
            rpc.SaveAssembly();
        }
    }
    
    class entrypoint
    {
        public static void Main( string[] args )
        {
            bool IsServer = true;
            if( args.GetUpperBound(0) + 1 > 0 && args[args.GetUpperBound(0)] == "client" )
            {
                IsServer = false;
            }
            
            try{
                new TestNetRpc().Go( IsServer );
            }catch( Exception e )
            {
                Console.WriteLine( e );
            }
        }
    }
}

