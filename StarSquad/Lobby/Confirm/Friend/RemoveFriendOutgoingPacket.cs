using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Friend
{
    public class RemoveFriendOutgoingPacket : OutgoingPacket
    {
        private readonly string playerId;

        public RemoveFriendOutgoingPacket(string playerId)
        {
            this.playerId = playerId;
        }
        
        public void Write(ByteBuf buf)
        {
            buf.WriteString(this.playerId);    
        }
    }
}