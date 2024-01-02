using StarSquad.Net.Confirm;
using StarSquad.Net.Packet.Game.Hello;
using StarSquad.Net.Packet.Game.Misc;
using StarSquad.Net.Packet.Play;

namespace StarSquad.Net.Packet.Game
{
    public class GamePacketManager : ConfirmPacketManager
    {
        public GamePacketManager()
        {
            this.RegisterIncoming(0, typeof(HelloIncomingPacket)); // play packet
            this.RegisterOutgoing(0, typeof(HelloOutgoingPacket)); // play packet
            
            this.RegisterOutgoing(1, typeof(RequestDataOutgoingPacket)); // play packet
            
            this.RegisterIncoming(2, typeof(GameDataIncomingPacket));
            
            this.RegisterIncoming(3, typeof(KickIncomingPacket)); // play packet
            
            this.RegisterIncoming(4, typeof(StartUdpIncomingPacket));
            
            this.RegisterOutgoing(5, typeof(UdpReadyOutgoingPacket));

            this.RegisterIncoming(6, typeof(DynamicGameDataIncomingPacket));
            
            this.RegisterOutgoing(7, typeof(DataLoadedOutgoingPacket)); // play packet
            
            this.RegisterIncoming(8, typeof(TickIncomingPacket));
            this.RegisterOutgoing(8, typeof(TickOutgoingPacket));
            
            this.RegisterIncoming(9, typeof(GameResultIncomingPacket));
            this.RegisterOutgoing(9, typeof(HomeOutgoingPacket));
            
            this.RegisterIncoming(10, typeof(SendToServerIncomingPacket)); // play packet
            
            this.RegisterIncoming(20, typeof(ExitIncomingPacket));
            this.RegisterOutgoing(20, typeof(ExitOutgoingPacket));
        }
    }
}