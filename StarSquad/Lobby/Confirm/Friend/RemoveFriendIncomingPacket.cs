using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Friend
{
    public class RemoveFriendIncomingPacket : IncomingPacket
    {
        private string friend;

        public void Read(ByteBuf buf)
        {
            this.friend = buf.ReadString();
        }

        public void Handle()
        {
            LobbyManager.instance.friendsListener.RemoveFriend(this.friend);
        }
    }
}