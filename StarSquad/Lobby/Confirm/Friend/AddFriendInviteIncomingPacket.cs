using StarSquad.Loader;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Friend
{
    public class AddFriendInviteIncomingPacket : IncomingPacket
    {
        private AddFriendIncomingPacket.Friend friend;
        private bool incoming;

        public void Read(ByteBuf buf)
        {
            this.friend = new AddFriendIncomingPacket.Friend();
            this.friend.Read(buf);
            this.incoming = buf.ReadBool();
        }

        public void Handle()
        {
            if (this.incoming)
            {
                LobbyManager.instance.friendsListener.AddIncomingInvite(this.friend, true);
                
                // TODO:
                //LoaderManager.instance.alertManager.Alert(this.friend.name + " has sent you a friend invite");
            }
            else
            {
                LobbyManager.instance.friendsListener.AddOutgoingInvite(this.friend, true);
                
                //LoaderManager.instance.alertManager.Alert("Sent a friend invite to " + this.friend.name);
            }
        }
    }
}