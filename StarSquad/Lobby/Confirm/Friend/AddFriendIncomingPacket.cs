using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Friend
{
    public class AddFriendIncomingPacket : IncomingPacket
    {
        public class Friend
        {
            public string playerId;
            public string name;
            public int rating;

            public void Read(ByteBuf buf)
            {
                this.playerId = buf.ReadString();
                this.name = buf.ReadString();
                this.rating = buf.ReadInt();
            }
        }

        private Friend friend;
        private UpdateFriendStatusIncomingPacket.FriendStatus status;

        public void Read(ByteBuf buf)
        {
            this.friend = new Friend();
            this.friend.Read(buf);
            this.status = (UpdateFriendStatusIncomingPacket.FriendStatus)buf.ReadByte();
        }

        public void Handle()
        {
            LobbyManager.instance.friendsListener.AddFriend(friend.playerId, friend.name, friend.rating, this.status);
        }
    }
}