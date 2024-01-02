using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Friend
{
    public class RemoveFriendInviteIncomingPacket : IncomingPacket
    {
        private string playerId;
        private bool incoming;

        public void Read(ByteBuf buf)
        {
            this.playerId = buf.ReadString();
            this.incoming = buf.ReadBool();
        }

        public void Handle()
        {
            if (this.incoming)
            {
                LobbyManager.instance.friendsListener.RemoveIncomingInvite(this.playerId);
            }
            else
            {
                LobbyManager.instance.friendsListener.RemoveOutgoingInvite(this.playerId);
            }
        }
    }
}