using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Friend
{
    public class AcceptInviteOutgoingPacket : OutgoingPacket
    {
        private readonly string playerId;
        private readonly bool accepted;

        public AcceptInviteOutgoingPacket(string playerId, bool accepted)
        {
            this.playerId = playerId;
            this.accepted = accepted;
        }
        
        public void Write(ByteBuf buf)
        {
            buf.WriteString(this.playerId);
            buf.WriteBool(this.accepted);
        }
    }
}