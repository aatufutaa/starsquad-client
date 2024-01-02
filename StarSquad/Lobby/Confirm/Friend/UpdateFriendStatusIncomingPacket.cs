using StarSquad.Net.Packet;
using UnityEngine;

namespace StarSquad.Lobby.Confirm.Friend
{
    public class UpdateFriendStatusIncomingPacket : IncomingPacket
    {
        public class FriendStatusUpdate
        {
            public string playerId;
            public FriendStatus status;

            public void Read(ByteBuf buf)
            {
                this.playerId = buf.ReadString();
                this.status = (FriendStatus)buf.ReadByte();
            }
        }

        public enum FriendStatus
        {
            Offline,
            Lobby,
            Game
        }

        private FriendStatusUpdate[] updates;

        public void Read(ByteBuf buf)
        {
            this.updates = new FriendStatusUpdate[buf.ReadByte()];
            for (var i = 0; i < this.updates.Length; i++)
            {
                var update = new FriendStatusUpdate();
                update.Read(buf);
                this.updates[i] = update;
            }
        }

        public void Handle()
        {
            foreach (var update in this.updates)
            {
                Debug.Log(LobbyManager.instance);
                Debug.Log(LobbyManager.instance.friendsListener);
                LobbyManager.instance.friendsListener.UpdateStatus(update.playerId, update.status);
            }
        }
    }
}