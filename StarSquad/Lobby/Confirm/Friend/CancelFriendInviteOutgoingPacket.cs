using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Friend
{
    public class CancelFriendInviteOutgoingPacket : OutgoingPacket
    {
        private readonly string playerId;

        public CancelFriendInviteOutgoingPacket(string playerId)
        {
            this.playerId = playerId;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteString(this.playerId);
        }
    }
}