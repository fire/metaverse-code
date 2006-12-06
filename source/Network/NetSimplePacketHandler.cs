
namespace OSMP
{
    // I'm guessing this is not actually used???
    public class NetSimplePacketHandler
    {
        static NetSimplePacketHandler instance = new NetSimplePacketHandler();
        public static NetSimplePacketHandler GetInstance(){ return instance; }
        
        INetworkModel net;

        public event ReceivedPacketHandler ReceivedPacket; 
        
        public NetSimplePacketHandler
        {
            net = NetworkModelFactory.GetInstance();
            net.RegisterPacketHandler( 'P', this );
        }
        
        public void PacketArrived( int GlobalPacketSequence, IPEndPoint connection, byte[] packet )
        {
            if( ReceivedPacket != null )
            {
                byte[]packetdata = new byte[ packet.Length - 1 ];
                Buffer.BlockCopy( packet, 1, packetdata, 0, packet.Length - 1 ];
                ReceivedPacket( connection, packetdata );
            }
        }
    }
}
