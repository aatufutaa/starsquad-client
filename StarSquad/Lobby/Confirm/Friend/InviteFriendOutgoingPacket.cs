using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Friend
{
    public class InviteFriendOutgoingPacket : OutgoingPacket
    {
        private readonly string friendId;

        public InviteFriendOutgoingPacket(string friendId)
        {
            this.friendId = friendId;
        }
        
        public void Write(ByteBuf buf)
        {
            buf.WriteString(this.friendId);    
        }
    }
}