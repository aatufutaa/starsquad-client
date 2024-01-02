using StarSquad.Lobby.Confirm.Friend;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Tooltip;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Friends
{
    public class FriendListItem : CustomButton
    {
        public TMP_Text number;
        public TMP_Text username;
        public TMP_Text status;
        public TMP_Text rating;

        private string playerId;
        
        private void Awake()
        {
            this.onClick.AddListener(this.HandleClick1);
        }
        
        public void SetNumber(int num)
        {
            this.number.text = "" + num;
        }

        public void SetData(string playerId, string name, int rating)
        {
            this.playerId = playerId;
            
            this.username.text = name;
            this.rating.text = "" + rating;
        }

        public void SetStatus(UpdateFriendStatusIncomingPacket.FriendStatus status)
        {
            this.status.text = status == UpdateFriendStatusIncomingPacket.FriendStatus.Offline ? "Offline"
                : status == UpdateFriendStatusIncomingPacket.FriendStatus.Lobby ? "In-Menus"
                : "In-Game";
        }

        private void HandleClick1()
        {
            LobbyManager.instance.friendsListener.ShowSelected(this.transform, this.playerId);
        }
    }
}