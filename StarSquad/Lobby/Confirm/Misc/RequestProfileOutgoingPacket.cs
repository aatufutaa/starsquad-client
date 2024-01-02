using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Misc
{
    public class RequestProfileOutgoingPacket : OutgoingPacket
    {
        private readonly string playerId;

        public RequestProfileOutgoingPacket(string playerId)
        {
            this.playerId = playerId;
        }
        
        public void Write(ByteBuf buf)
        {
            buf.WriteString(this.playerId);
        }
    }
}