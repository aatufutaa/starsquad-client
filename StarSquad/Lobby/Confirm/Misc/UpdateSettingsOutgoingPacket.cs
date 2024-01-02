using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Misc
{
    public class UpdateSettingsOutgoingPacket : OutgoingPacket
    {
        private readonly bool allowFriendRequests;
        private readonly int location;

        public UpdateSettingsOutgoingPacket(bool allowFriendRequests, int location)
        {
            this.allowFriendRequests = allowFriendRequests;
            this.location = location;
        }
        
        public void Write(ByteBuf buf)
        {
            buf.WriteBool(this.allowFriendRequests);
            buf.WriteByte((byte)this.location);
        }
    }
}